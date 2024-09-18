using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    Player player;
    public Vector2 input;
    public SpriteRenderer sprite;
    Animator ani; 
    [SerializeField] bool running, grounded;
    Vector3 playerScale;
    Vector3 curPlayerScale;
    void Start()
    {
        player = Player.Ins;
        sprite = GetComponent<SpriteRenderer>();
        ani = GetComponent<Animator>();
        playerScale = player.transform.localScale;
        curPlayerScale = playerScale;
    }

    // Update is called once per frame
    void Update()
    {

        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        player.input = input;
        if (input.x != 0){
            running = true;
            sprite.flipX = input.x == -1;
        } else running = false;
        curPlayerScale.y = player.flipped ? -playerScale.y : playerScale.y;
        player.transform.localScale = curPlayerScale;
        player.crouching = input.y == -1;

        HandleAnimations();
        if (Input.GetKeyDown(KeyCode.Space)) { player.OnJumpInputDown(); }
        if (Input.GetKeyUp(KeyCode.Space)) { player.OnJumpInputUp(); }
    }
    void HandleAnimations()
    {
        if (PauseEvent.Paused) return;
        grounded = player.flipped ? player.contr.cols.above : player.contr.cols.below;
        
        ani.SetInteger("MoveDirX", (int)input.x);
        ani.SetBool("Moving", running);
        ani.SetBool("WallSliding", player.wallClimbing);
        ani.SetBool("Grounded", grounded);
    }
}
