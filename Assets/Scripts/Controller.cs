using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : RaycastController
{
    
    public bool canCJump;
    [SerializeField] public ColInfo cols; public bool wasGrounded, justJumped;
    [HideInInspector] public BoxCollider2D bCol; Vector2 playerInput;
    public float dirX, dirY; Player player;
    public static Controller Ins;
    [HideInInspector] public float coyoteDur; float heldDir;
    public override void Awake()
    {
        base.Awake();
        if (Ins == null)
            Ins = this;
        CalcRaySpacing();
    }

    public override void Start()
    {
        base.Start(); bCol = GetComponent<BoxCollider2D>();
        player = Player.Ins;
    }

    public void Move(Vector2 v, bool _onPlat)
    {
        Move(v, Vector2.zero, _onPlat);
    }
    public void Move(Vector2 velocity, Vector2 input, bool _onPlat = false)
    {
        playerInput = input;
        wasGrounded = !player.flipped ? cols.below : cols.above;
        bool onPlat = _onPlat;
        UpdateRayOrigins();
        cols.reset();
        CalcRaySpacing();

        if (velocity.x != 0)
            HorizontolCollisions(ref velocity);
        if (velocity.y != 0)
            VerticalCollisions(ref velocity);

        transform.Translate(velocity);
        if (wasGrounded && ((!Player.Ins.flipped && !cols.below) || (Player.Ins.flipped && !cols.above)) && !justJumped) 
            StartCoroutine(CoyoteTimeCheck(coyoteDur));
        justJumped = false;
        if (onPlat) cols.below = true;
    }
    void HorizontolCollisions(ref Vector2 velocity)
    {
        dirX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) < skinWidth ? 2 * skinWidth : Mathf.Abs(velocity.x) + skinWidth;


        for (int i = 0; i < hRays; i++)
        {
            Vector2 rayOg = dirX == 1 ? rcOg.botR : rcOg.botL;
            rayOg += Vector2.up * (hRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOg, Vector2.right * dirX, rayLength, mask);

            Debug.DrawRay(rayOg, Vector2.right * dirX, Color.red);
            Debug.DrawRay(rayOg, Vector2.right * dirX * rayLength, Color.green);

            if (hit)
            {
                if (hit.distance == 0) continue;
                velocity.x = (hit.distance - skinWidth) * dirX;
                rayLength = hit.distance;
                cols.left = dirX == -1; cols.right = dirX == 1;
            }
        }
        
    }
    void VerticalCollisions(ref Vector2 velocity)
    {
        dirY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;

        for (int i = 0; i < vRays; i++)
        {
            Vector2 rayOg = dirY == 1 ? rcOg.topL : rcOg.botL;
            rayOg += Vector2.right * (vRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOg, Vector2.up * dirY, rayLength, mask);

            Debug.DrawRay(rayOg, Vector2.up * dirY, Color.red);
            Debug.DrawRay(rayOg, Vector2.up * dirY * rayLength, Color.green);

            if (hit)
            {
                if (hit.collider.tag == "Pass") {
                    if (dirY == 1 || hit.distance == 0) continue;
                    if (cols.fTPlat) continue;
                    if (playerInput.y == -1) { cols.fTPlat = true; Invoke("ResetfTP", .6f); continue; }
                }
                velocity.y = (hit.distance - skinWidth) * dirY;
                rayLength = hit.distance;
                cols.below = dirY == -1; cols.above = dirY == 1;
            }
        }
    }

    private void ResetfTP() => cols.fTPlat = false;
    IEnumerator CoyoteTimeCheck(float cTime)
    {
        canCJump = true;
        yield return new WaitForSeconds(cTime); 
        canCJump = false;
    }
    
}
