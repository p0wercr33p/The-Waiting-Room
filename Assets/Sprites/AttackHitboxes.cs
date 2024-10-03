using Pathfinding;
using Pathfinding.Util;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Enemy;
using static ObjectManager._Cannons;
using static Unity.Burst.Intrinsics.X86.Avx;

public class AttackHitboxes : MonoBehaviour
{
    public Collider2D[] hitboxes;
    private int activateInd, deactivateInd, nLength;
    (GameObject pr, Projectile info)[] projectiles;
    public GameObject projectile;
    public BoxCollider2D mainHurtBox, plCol;
    public Vector2 aggroRange;
    [SerializeField] public AggroBox aggro;
    [SerializeField] public int dmg, effect, cap;
    Player player;
    PlayerHealth pHP;
    public float timeLeft, cooldown;
    [SerializeField] private Transform drawPoint,firePoint;
    bool canAttack, attacking;
    public bool hasAggroBox, draw;
    public bool inRange;
    public Color aggroBoxColor;
    Animator ani;
 
    // Start is called before the first frame update
    void Start()
    {
        player = Player.Ins;
        pHP = PlayerHealth.Ins;
        plCol = player.GetComponent<BoxCollider2D>();
        if (hasAggroBox)
        {
            if (mainHurtBox != null) aggro = new AggroBox(mainHurtBox.bounds, aggroRange, player);
            else { aggro = new AggroBox(hitboxes[0].bounds, aggroRange, player); }
        }
        DeactivateAll();
        ani = GetComponent<Animator>();
        nLength = hitboxes.Length;
        if (projectile != null) InitializeProjectiles();
        timeLeft = cooldown;
    }

    public void ActivateHitBox()
    {
        hitboxes[activateInd].enabled = true;
        activateInd = (activateInd + 1) % nLength;
    }
    public void DeactivateAll(int yesInt = 0){
        bool yes = yesInt == 1;
        foreach (var h in hitboxes)
            h.enabled = yes;
        activateInd = 0;
        deactivateInd = 0;
        attacking = false;
    }
    public void DeactivateHitBox()
    {
        hitboxes[deactivateInd].enabled = false;
        deactivateInd = (deactivateInd + 1) % nLength;
        if (deactivateInd == 0) attacking = false;  
    }
    
    
    // Update is called once per frame
    void Update()
    {
        inRange = InRangeCheck(hasAggroBox);
        if (!canAttack && !attacking)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0)
            {
                canAttack = true;
                timeLeft = cooldown;
            }
        }
        else if (inRange)
        {
            if (canAttack && !attacking)
            {
                canAttack = false;
                attacking = true;
                ani.SetTrigger("Attack");
            }
        }
    }
    bool InRangeCheck(bool aggroBox = true)
    {
        if (aggroBox){
            aggro.Move(drawPoint.position);
            return aggro.IsPlayerInside(plCol);
        } else{
            return IsPlayerOnCorrectSide(player.transform, transform);
        }
    }
    public bool IsPlayerOnCorrectSide(Transform playerTransform, Transform objectTransform)
    {
        // Get the direction from the object to the player
        Vector3 directionToPlayer = playerTransform.position - objectTransform.position;

        // Check if the object's scale is positive or negative
        bool isScalePositive = objectTransform.localScale.x > 0;

        // Check if the player is on the correct side
        if (isScalePositive)
        {
            // If scale is positive, player should be on the left (negative x direction)
            return directionToPlayer.x < 0;
        }
        else
        {
            // If scale is negative, player should be on the right (positive x direction)
            return directionToPlayer.x > 0;
        }
    }
    private void OnEnable() => DeactivateAll();
    
    private void OnTriggerEnter2D(Collider2D col) {
        print(col.name);
        if (col.CompareTag("Player")) {
            print("hit player");
            pHP.TakeDamage(dmg, effect);
        }
    }

    [System.Serializable]
    public struct AggroBox
    {
        public Vector2 center;
        public float right, left, top, bottom;
        Player pl; 
        public AggroBox(Bounds targetBounds, Vector2 size, Player _pl)
        {
            center = targetBounds.center; 
            pl = _pl;

            // Calculate edges based on the center
            left = center.x - size.x / 2;
            right = center.x + size.x / 2;
            bottom = center.y - size.y / 2;
            top = center.y + size.y / 2;
        }
        public void Move(Vector2 newCenter)
        {
            Vector2 offset = newCenter - center;
            center = newCenter;
            left += offset.x;
            right += offset.x;
            top += offset.y;
            bottom += offset.y;
        }
       
        public bool IsPlayerInside(BoxCollider2D playerColl)
        {
            Bounds playerBounds = playerColl.bounds;

            bool yes = playerBounds.min.x <= right && playerBounds.max.x >= left &&
                   playerBounds.min.y <= top && playerBounds.max.y >= bottom;
  
            return yes;
        }
    }
    void InitializeProjectiles()
    {
        Transform kts = GameObject.FindGameObjectWithTag("ProjectileHolder").transform;
        projectiles = new (GameObject, Projectile)[cap];
        ObjectManager man = ObjectManager.Ins;
        for (int i = 0; i < cap; i++)
        {
            GameObject newProj = Instantiate(projectile, kts);
            Projectile newProjInfo = newProj.GetComponent<Projectile>();
            projectiles[i] = (newProj, newProjInfo);
            newProj.SetActive(false);
            man.AddObject(newProj, newProjInfo.projTypeName, "Projectile");
        }
    }
    public void ShootLogic()
    {
        float direc = Mathf.Sign(transform.localScale.x);

        Vector2 dir = direc == 1 ? Vector2.left : Vector2.right;
        Shoot(dir);
    }
    (GameObject, Projectile) GetProjectile()
    {
        for (int i = 0; i < cap; i++)
        {
            if (!projectiles[i].pr.activeInHierarchy)
                return projectiles[i];
        }
        return (null, null);
    }
    private void Shoot(Vector2 dir)
    {
        (GameObject proj, Projectile comp) = GetProjectile();


        if (proj != null)
        {
            proj.SetActive(true);
            proj.transform.position = firePoint.position;

            comp.rb.velocity = dir * comp.speed;
            print(comp.rb.velocity + "    speed - " + comp.speed);
        }
        else print("couldn't find a prjectile");
        attacking = false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = aggroBoxColor;

        if (draw) Gizmos.DrawCube(aggro.center, aggroRange);
    }
}
