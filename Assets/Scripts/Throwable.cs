using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerInput;
using static Unity.Burst.Intrinsics.X86.Avx;

public class Throwable : RaycastController
{
    Player player;
    PlayerHealth pHP;
    public Vector2 throwForce, grabHitBox, offset;
    public bool isFloating, beingCarried, inRange;
    public Vector2 velocity;
    Manager manager;
    public float gravity;
    PlayerInput plInput;
    [SerializeField]
    GrabBox grabBox; 
    [HideInInspector] public BoxCollider2D col, playerCol; Controller contr;
    [HideInInspector] public int Id; 
    public bool draw;
    bool below, above, left, right,justGrabbed; 
    float vXSmoothing;
    public Color grabBoxColor;
    public bool interactable, destroyOnceEmpty; 
    public int healAmount, usages;
    public string projName;
    public enum ItemType { BOMB, BLASTER, HEALING}
    public ItemType type;
    Obstacle bomb;
    public override void Awake()
    {
        base.Awake();
    }
    public override void Start()
    {
        base.Start();
        col = GetComponent<BoxCollider2D>();
        player = Player.Ins;
        pHP = PlayerHealth.Ins;
        plInput = PlayerInput.Ins;
        manager = Manager.Ins;
        contr = Controller.Ins;
        playerCol = player.GetComponent<BoxCollider2D>();
        Id = gameObject.GetInstanceID();
        grabBox = new GrabBox(col.bounds, grabHitBox, player, Id, offset);
        if (type == ItemType.BOMB) bomb = GetComponent<Obstacle>();
    }

    // Update is called once per frame
    void Update()
    {
        justGrabbed = false;
        UpdateRayOrigins();
        CalcRaySpacing();
        Reset();
        if (!beingCarried){
            if (player.gameObject != null) inRange = grabBox.IsPlayerInside(playerCol);
            if (!justGrabbed && inRange && Input.GetKeyDown(KeyCode.F))
                Carry();
            else if(!isFloating) {
                ApplyGravity();
                ApplyCollisions();
                transform.Translate(velocity);
            }
            grabBox.Move(transform.position);
        }else{
            if (interactable && Input.GetKeyDown(KeyCode.J))
                UseItem(); 
            if (!justGrabbed && Input.GetKeyDown(KeyCode.F))
                Throw();
        }
    }
    void UseItem(){
        if (usages <= 0 && !destroyOnceEmpty) return;

        if (type == ItemType.BLASTER)
        {
            plInput.usages[projName] += usages;
            HandleDestruction();
        }
        else if (type == ItemType.HEALING)
        {
            pHP.hp += healAmount;
            usages--;
        }

        if (usages <= 0 && destroyOnceEmpty)
            HandleDestruction();
    }
    public void HandleDestruction(){
        col.enabled = true; beingCarried = false; inRange = false;
        player.carryingItem = false; player.carryId = 0; transform.parent = manager.transform; 
        if (type != ItemType.BOMB) gameObject.SetActive(false);
        else { bomb.Explode(); }
    }
    
    void Carry()
    {
        if (player.carryingItem) return; 
        player.carryingItem = true;
        transform.parent = player.transform;
        beingCarried = true; col.enabled = false; justGrabbed = true; isFloating = false;
        Vector3 targetPosition = new Vector3(0, .25f, 0);
        transform.localPosition = targetPosition;
    }
    void Throw()
    {
        player.carryingItem = beingCarried = inRange = isFloating = false;
        transform.parent = manager.transform;
        col.enabled = true; 
        UpdateRayOrigins(); 
        CalcRaySpacing();
        if (player.crouching) { grabBox.Move(transform.position); return; }
        
        Vector2 throwDirection = player.flipped ? Vector2.down : Vector2.up;
        throwDirection += Vector2.right * contr.dirX;
        throwDirection.Normalize();

        velocity = throwDirection * throwForce.magnitude;
        grabBox.Move(transform.position);
    }
    private void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
    }

    private void ApplyCollisions()
    {
        if (velocity.x != 0)
            HorizontalCollision(ref velocity);
        if (velocity.y != 0)
            VerticalCollision(ref velocity);
        if (below || above)
        { velocity.y = velocity.x = 0; }
    }
    
    void HorizontalCollision(ref Vector2 velocity)
    {
        float dirX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) < skinWidth ? 2 * skinWidth : Mathf.Abs(velocity.x) + skinWidth;

        for (int i = 0; i < hRays; i++)
        {
            Vector2 rayOg = dirX == 1 ? rcOg.botR : rcOg.botL;
            rayOg += Vector2.up * (hRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOg, Vector2.right * dirX, rayLength, mask);
        

            if (hit)
            {
                velocity.x = (hit.distance - skinWidth) * dirX;
                rayLength = hit.distance;
                left = dirX == -1; right = dirX == 1;
            }
        }
    }
    void VerticalCollision(ref Vector2 velocity)
    {
        float dirY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;

        for (int i = 0; i < vRays; i++)
        {
            Vector2 rayOg = dirY == 1 ? rcOg.topL : rcOg.botL;
            rayOg += Vector2.right * (vRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOg, Vector2.up * dirY, rayLength, mask);

            Debug.DrawRay(rayOg, Vector2.up * dirY, Color.blue);
            Debug.DrawRay(rayOg, Vector2.up * dirY * rayLength, Color.magenta);

            if (hit)
            {
                if (hit.transform.CompareTag("Pass")) continue;
                velocity.y = (hit.distance - skinWidth) * dirY;
                rayLength = hit.distance;
                below = dirY == -1; above = dirY == 1;
            }
        }
    }
    private void Reset() => below = above = right = left = false;

    [System.Serializable]
    struct GrabBox
    {
        public Vector2 center, offset;
        public float right, left, top, bottom;
        Player pl; int id;
        public GrabBox(Bounds targetBounds, Vector2 size, Player _pl, int _id, Vector2 _offset)
        {
            offset = _offset;
            center = (Vector2)targetBounds.center + _offset; pl = _pl; id = _id;

            // Calculate edges based on the center
            left = center.x - size.x / 2;
            right = center.x + size.x / 2;
            bottom = center.y - size.y / 2;
            top = center.y + size.y / 2;
        }
        public void Move(Vector2 newCenter)
        {
            newCenter += offset;
            Vector2 newOffset = newCenter - center;
            center = newCenter;
            left += newOffset.x;
            right += newOffset.x;
            top += newOffset.y;
            bottom += newOffset.y;
        }
        public bool IsPlayerInside(BoxCollider2D playerColl) 
        {
            Bounds playerBounds = playerColl.bounds;

            bool yes = playerBounds.min.x <= right && playerBounds.max.x >= left &&
                   playerBounds.min.y <= top && playerBounds.max.y >= bottom;
            if (yes)
            {
                if (pl.carryId == 0)
                {
                    pl.carryId = id; return true;
                }
                else if (pl.carryId == id)
                {
                    return true;
                }
                else { return false; }
            } else { if (pl.carryId == id) pl.carryId = 0; }

            return false;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = grabBoxColor;

        if (draw) Gizmos.DrawCube(grabBox.center + offset, grabHitBox);
    }
}
