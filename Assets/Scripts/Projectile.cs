using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;

public class Projectile : MonoBehaviour
{
    public float speed, life, lifeLeft;
    public int dmg, dmgVsEnemies, effect;
    [HideInInspector] public float rotate;
    public Rigidbody2D rb; 
    Player player; 
    string playerTag, enemyTag, obstacleTag, groundTag;
    public enum Team { PLAYER, ENEMY, NEITHER };
    public Team team; 
    bool active;
    BoxCollider2D hitbox;
    bool isInstantiated;
    [SerializeField] private AIPath aiPath;
    public Vector2 scale, explosionScale;
    Animator ani;
    public Vector2 defaultHitboxSize = new Vector2(1, 1);
    public Vector2 explosionHitboxSize = new Vector2(3, 3);
    public bool heetSeeking;
    Dictionary<Transform, Enemy> enemyBook;
    
    void Start()
    { 
        player = Player.Ins;
        enemyTag = "Enemy"; 
        playerTag = "Player";
        obstacleTag = "Obstacle";
        groundTag = "Ground";
        hitbox = GetComponent<BoxCollider2D>();
        active = true;
        if (heetSeeking) Invoke("SetTeamToNeither", 0.3f);
        if (ani == null) ani = GetComponent<Animator>();
        enemyBook = new Dictionary<Transform, Enemy>();
        scale = transform.localScale;
        explosionScale = new Vector2((scale.x*1.4f)+.5f, (scale.y * 1.4f) + .5f);
        rb = GetComponent<Rigidbody2D>(); 
        lifeLeft = life;
    }


    void SetTeamToNeither() => team = Team.NEITHER;
    private void Update()
    {
        lifeLeft -= Time.deltaTime;
        if (lifeLeft <= 0){
            print($"My time on this planet is over");
            Explode();
            rb.velocity = Vector2.zero;
        }
    }
    void Explode()
    {
        if (ani == null) gameObject.SetActive(false);
        else
        {
            if (heetSeeking) aiPath.enabled = false;
            hitbox.size = explosionHitboxSize;
            ani.SetTrigger("Explode");
            transform.localScale = explosionScale;
        }
    }
    private void OnEnable()
    {
        if (isInstantiated) OnEnableAfter();
        else isInstantiated = true;
    }
    private void OnEnableAfter()
    {
        if (heetSeeking){
            aiPath.enabled = true;
            Invoke("SetTeamToNeither", 0.3f);
        }
        lifeLeft = life; hitbox.enabled = true; active = true;
        transform.localScale = scale;
    }

    public void SwitcHitboxActiveState()
    {
        active = !active;
        hitbox.enabled = active;
    }
    public void BulletLifeOver(){
        hitbox.size = defaultHitboxSize;
        if (heetSeeking) team = Team.ENEMY;
        gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D coll)
    {
        bool pl = coll.CompareTag(playerTag);
        bool enemy = coll.CompareTag(enemyTag);
        bool obs = coll.CompareTag(obstacleTag);
        bool gr = coll.CompareTag(groundTag);
        print($"Hit da {coll.tag} -- {coll.name} --- {pl}-{enemy}-{obs}-{gr}");

        if ((team == Team.ENEMY || team == Team.NEITHER) && pl) {
            player.TakeDamage(dmg, effect); Explode();
            rb.velocity = Vector2.zero;
            print("enemy check");
        } else if ((team == Team.PLAYER || team == Team.NEITHER) && enemy) {
            if (!enemyBook.ContainsKey(coll.transform))
                enemyBook[coll.transform] = coll.GetComponent<Enemy>();
            enemyBook[coll.transform].TakeDamage(dmgVsEnemies); 
            Explode();
            print("player check");
            rb.velocity = Vector2.zero;
        }
        if (obs || gr) { print("ground check"); Explode(); rb.velocity = Vector2.zero; }
    }
}
