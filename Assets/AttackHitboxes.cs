using Pathfinding.Util;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackHitboxes : MonoBehaviour
{
    public Collider2D[] hitboxes;
    private int activateInd;
    private int deactivateInd;
    private int nLength;
    public BoxCollider2D mainHurtBox, plCol;
    public Vector2 aggroRange;
    [SerializeField] public AggroBox aggro;
    [SerializeField] public int dmg, effect;
    Player player;
    [SerializeField] private float attackTime, timeLeft;
    [SerializeField] bool inRange, draw, canAttack, attacking;
    public Color aggroBoxColor;
    Animator ani;
    // Start is called before the first frame update
    void Start()
    {
        player = Player.Ins;
        plCol = player.GetComponent<BoxCollider2D>();
        aggro = new AggroBox(mainHurtBox.bounds, aggroRange, player);
        DeactivateAll(false);
        ani = GetComponent<Animator>();
        nLength = hitboxes.Length;
        timeLeft = attackTime;
    }

    public void ActivateHitBox()
    {
        hitboxes[activateInd].enabled = true;
        activateInd = (activateInd + 1) % nLength;
    }
    public void DeactivateAll(bool yes = false){
        foreach (var h in hitboxes)
            h.enabled = yes;
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
        inRange = aggro.IsPlayerInside(plCol);
        if (!canAttack && !attacking)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0)
            {
                canAttack = true;
                timeLeft = attackTime;
            }
        }
        else if (inRange)
        {
            if (canAttack && !attacking)
            {
                canAttack = false;
                print("attacking");
                attacking = true;
                ani.SetTrigger("Attack");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col) { 
        if (col.CompareTag("Player"))
            player.TakeDamage(dmg, effect);
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
    private void OnDrawGizmos()
    {
        Gizmos.color = aggroBoxColor;

        if (draw) Gizmos.DrawCube(mainHurtBox.bounds.center, aggroRange);
    }
}
