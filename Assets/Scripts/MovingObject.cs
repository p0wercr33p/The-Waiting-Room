using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class MovingObject : RaycastController
{
    
    public LayerMask pMask, waypointMask; 
    int n;
    BoxCollider2D col;
    public Vector3[] waypoints, globalWP;
    public float speed;
    public enum ObjectType { PLATFORM, ENEMY, ITEM}
    public ObjectType type;
    [SerializeField]
    int fromWPi; [SerializeField] float perWP;
    public bool cyclic; 
    Dictionary<Transform, Controller> passport;
    Dictionary<Transform, Throwable> luggage;
    public Color wpcolor;
    public SpriteRenderer sprite;
    Enemy enemy;
    List<PassengerMovement> pMovement;
    public bool global;

    public override void Awake()
    {
        base.Awake();
    }
    public override void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.enabled = false;
        base.Start(); 
        passport = new();
        luggage = new();
        col = GetComponent<BoxCollider2D>();
        switch (type)
        {
            case ObjectType.PLATFORM: wpcolor = Color.green; break;
            case ObjectType.ENEMY: SetEnemyWayPoints(); wpcolor = Color.red; break;
            case ObjectType.ITEM: wpcolor = Color.yellow; break;
        }
        sprite.enabled = true;
    }
    public void SetEnemyWayPoints()
    {
        enemy = GetComponent<Enemy>();

     
        Vector2 dir = new Vector2(enemy.shootDir.y, enemy.shootDir.x);
        
        RaycastHit2D hit1 = Physics2D.Raycast(transform.position, dir, 100f, waypointMask);
        RaycastHit2D hit2 = Physics2D.Raycast(transform.position, -dir, 100f, waypointMask);

        float hitPointX1 = dir.x == 0 ? hit1.point.x : hit1.point.x - dir.x;
        float hitPointY1 = dir.y == 0 ? hit1.point.y : hit1.point.y - dir.y;
        float hitPointX2 = dir.x == 0 ? hit2.point.x : hit2.point.x + dir.x;
        float hitPointY2 = dir.y == 0 ? hit2.point.y : hit2.point.y + dir.y;

        waypoints = new Vector3[3];
        waypoints[0] = transform.position;
        waypoints[1] = new Vector2(hitPointX1, hitPointY1);
        waypoints[2] = new Vector2(hitPointX2, hitPointY2);

        globalWP = new Vector3[waypoints.Length];
        for (int i = 0; i < waypoints.Length; i++)
        {
            globalWP[i] = global ? waypoints[i] : waypoints[i] + transform.position;
        }
        enemy.Rotate();
    }
    void Update()
    {
        if (type == ObjectType.PLATFORM) UpdateRayOrigins();
        
        Vector2 velocity = CalcMovement();

        if (type == ObjectType.PLATFORM) CalcPassengerMovement(velocity);

        MovePassengers(true);
        transform.Translate(velocity, Space.World);
        MovePassengers(false);
    }
    Vector2 CalcMovement(){
        fromWPi %= globalWP.Length;
        int toWPi = (fromWPi + 1) % globalWP.Length;
        float WPdist = Vector2.Distance(globalWP[fromWPi], globalWP[toWPi]);
        perWP += Time.deltaTime * speed / WPdist;

        Vector2 newPos = Vector2.Lerp(globalWP[fromWPi], globalWP[toWPi], perWP);
        
        if (perWP >= 1)
        {
            perWP = 0;
            fromWPi++;
            if (!cyclic && fromWPi >= waypoints.Length - 1){
                fromWPi = 0;
                System.Array.Reverse(globalWP);
            }
        }
        return newPos - (Vector2)transform.position;
    }

    void MovePassengers(bool beforeMoving){
        if (type != ObjectType.PLATFORM) return;
        foreach (var p in pMovement){
            if (!passport.ContainsKey(p.transform))
                passport[p.transform] = p.transform.GetComponent<Controller>();

            if (p.moveBefore == beforeMoving)
                passport[p.transform].Move(p.velocity, p.isOnPlat);
        }
    }
    void CalcPassengerMovement(Vector2 velocity){

        HashSet<Transform> movedPs = new HashSet<Transform>();
        pMovement = new();
        float dirX = Mathf.Sign(velocity.x), dirY = Mathf.Sign(velocity.y);

        // vertically moving platform 
        if (velocity.y != 0)
        {
            float rayLength = Mathf.Abs(velocity.y) + skinWidth;

            for (int i = 0; i < vRays; i++)
            {
                Vector2 rayOg = dirY == 1 ? rcOg.topL : rcOg.botL;
                rayOg += Vector2.right * (vRaySpacing * i + velocity.x);
                RaycastHit2D hit = Physics2D.Raycast(rayOg, Vector2.up * dirY, rayLength, pMask);

                if (hit && hit.distance != 0)
                {
                    if (!movedPs.Contains(hit.transform))
                    {
                        movedPs.Add(hit.transform);
                        float pushX = dirY == 1 ? velocity.x : 0;
                        float pushY = velocity.y - (hit.distance - skinWidth) * dirY;

                        pMovement.Add(new PassengerMovement(hit.transform, dirY == 1, new Vector2(pushX, pushY), true));
                    }
                }
            }
        }
        
        // horizontally moving platforms :pq:
        if (velocity.x != 0)
        {
            float rayLength = Mathf.Abs(velocity.x) < skinWidth ? 2 * skinWidth : Mathf.Abs(velocity.x) + skinWidth;


            for (int i = 0; i < hRays; i++)
            {
                Vector2 rayOg = dirX == 1 ? rcOg.botR : rcOg.botL;
                rayOg += Vector2.up * (hRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOg, Vector2.right * dirX, rayLength, pMask);

                if (hit && hit.distance != 0)
                {
                    if (!movedPs.Contains(hit.transform))
                    {
                        movedPs.Add(hit.transform);
                        float pushX = velocity.x - (hit.distance - skinWidth) * dirX;
                        float pushY = -skinWidth;

                        pMovement.Add(new PassengerMovement(hit.transform, false, new Vector2(pushX, pushY), true));
                    }
                }
            }
        }
        // Passenger on top of horizontally or downward moving platform
        if (dirY == -1 || velocity.y == 0 && velocity.x != 0)
        {
            float rayLength = 2 * skinWidth;

            for (int i = 0; i < vRays; i++)
            {
                Vector2 rayOg = rcOg.topL;
                rayOg += (Vector2.right * (vRaySpacing * i));
                RaycastHit2D hit = Physics2D.Raycast(rayOg, Vector2.up, rayLength, pMask);

                if (hit && hit.distance != 0)
                {
                    if (!movedPs.Contains(hit.transform))
                    {
                        movedPs.Add(hit.transform);
                        float pushX = dirY == 1 ? velocity.x : 0;
                        float pushY = velocity.y - (hit.distance - skinWidth) * dirY;

                        pMovement.Add(new PassengerMovement(hit.transform, true, new Vector2(pushX, pushY), false));
                    }
                }
            }
        }
    }
    struct PassengerMovement{
        public Transform transform;
        public bool isOnPlat;
        public Vector2 velocity;
        public bool moveBefore;

        public PassengerMovement(Transform t, bool iOP, Vector2 v, bool mB){
            transform = t; isOnPlat = iOP; velocity = v; moveBefore = mB;
        }
    }
    private void OnDrawGizmos()
    {
        if (waypoints != null){
            Gizmos.color = wpcolor;
            float size = .3f;
            for (int i = 0; i < waypoints.Length; i++)
            {
                Vector3 wpPos = Application.isPlaying ? globalWP[i] : waypoints[i] + transform.position;
                Gizmos.DrawLine(wpPos - Vector3.up * size, wpPos + Vector3.up * size);
                Gizmos.DrawLine(wpPos - Vector3.left * size, wpPos + Vector3.left * size);
            }
        }
    }
}
