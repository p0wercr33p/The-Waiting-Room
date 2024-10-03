using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    Transform player;
    private SpriteRenderer sprite;
    public LayerMask mask;
    Vector2 scale, curScale;
    private Enemy enemy;
    private Player pl;
    string playerTag;

    void Start()
    {
        enemy = GetComponent<Enemy>();
        sprite = GetComponent<SpriteRenderer>();
        pl = Player.Ins;
        scale = transform.localScale;
        curScale = scale;
        playerTag = "Player";
        player = pl.transform;
        curScale.x = scale.x * Mathf.Sign(transform.position.x - player.position.x);
        transform.localScale = curScale;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        // Direction from the object to the player
        Vector2 direction = (player.position - transform.position).normalized;

        // Cast a ray from this object's position toward the player
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 150f, mask);

        // Debug the ray in the Scene view
        Debug.DrawRay(transform.position, hit.point, Color.red);

        if (hit.collider != null){
            if (hit.collider.CompareTag(playerTag)){
                enemy.inShootRange = true;
            }
            else { enemy.inShootRange = false; }
        }
    }
    public void LateUpdate()
    {
        curScale.x = scale.x * Mathf.Sign(transform.position.x + player.position.x);
        transform.localScale = curScale;
    }
}
