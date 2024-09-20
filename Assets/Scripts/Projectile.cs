using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    public float speed, life; public int dmg; public float lifeLeft;
    public Rigidbody2D rb; Player player; string playerTag, enemyTag, obstacleTag, groundTag;
    public enum Team { PLAYER, ENEMY };
    public Team team; public int effect;
    bool active;
    Collider2D hitbox;
    Animator ani;
    Dictionary<Transform, Enemy> enemyBook;
    
    void Start()
    {
        player = Player.Ins;
        enemyTag = "Enemy"; 
        playerTag = "Player";
        obstacleTag = "Obstacle";
        groundTag = "Ground";
        hitbox = GetComponent<Collider2D>();
        active = true;
        ani = GetComponent<Animator>();
        enemyBook = new Dictionary<Transform, Enemy>();
        rb = GetComponent<Rigidbody2D>(); lifeLeft = life;
    }

    private void Update()
    {
        lifeLeft -= Time.deltaTime;
        if (lifeLeft <= 0){
            print($"My time on this planet is over"); 
            ani.SetTrigger("Explode");
            rb.velocity = Vector2.zero;
        }
    }

    private void OnEnable()
    {
        lifeLeft = life; hitbox.enabled = true; active = true;
    }

    public void SwitcHitboxActiveState()
    {
        active = !active;
        hitbox.enabled = active;
    }
    public void BulletLifeOver(){
        gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D coll)
    {
        print($"Hit da {coll.tag}");
        bool pl = coll.CompareTag(playerTag);
        bool enemy = coll.CompareTag(enemyTag);
        bool obs = coll.CompareTag(obstacleTag);
        bool gr = coll.CompareTag(groundTag);

        if (team == Team.ENEMY && pl){
            player.TakeDamage(dmg, effect); ani.SetTrigger("Explode");
            rb.velocity = Vector2.zero;
        } else if (team == Team.PLAYER && enemy) {
            if (!enemyBook.ContainsKey(coll.transform))
                enemyBook[coll.transform] = coll.GetComponent<Enemy>();
            enemyBook[coll.transform].TakeDamage(dmg); ani.SetTrigger("Explode");
            rb.velocity = Vector2.zero;
        }
        if (obs || gr) { ani.SetTrigger("Explode"); rb.velocity = Vector2.zero; }
    }
}
