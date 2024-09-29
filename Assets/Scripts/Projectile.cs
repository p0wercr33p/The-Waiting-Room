using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;

public class Projectile : MonoBehaviour
{
    public float speed, life, distance;
    public int dmg, dmgVsEnemies, effect;
    float lifeLeft;
    public Rigidbody2D rb; 
    Player player; Transform plT;
    string playerTag, enemyTag, obstacleTag, groundTag;
    public enum Team { PLAYER, ENEMY, NEITHER };
    public Team team; 
    bool active;
    BoxCollider2D hitbox;
    bool isInstantiated;
    [SerializeField] private AIPath aiPath;
    Animator ani;
    public GameObject explosionPrefab;
    GameObject explosion;

    public bool heetSeeking, trigger;
    Dictionary<Transform, HealthData> enemyBook;
    Dictionary<Transform, Throwable> itemBook;
    public string projTypeName;


    private void Awake()
    {
        hitbox = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }
    void Start()
    { 
        player = Player.Ins;
        plT = player.transform;
        enemyTag = "Enemy"; 
        playerTag = "Player";
        obstacleTag = "Obstacle";
        groundTag = "Ground";
        
        active = true;
        if (heetSeeking) Invoke("SetTeamToNeither", 0.3f);
        if (ani == null) ani = GetComponent<Animator>();
        enemyBook = new Dictionary<Transform, HealthData>();
        if (explosionPrefab != null){
            explosion = Instantiate(explosionPrefab, GameObject.FindGameObjectWithTag("ProjectileHolder").transform);
            explosion.SetActive(false);
        }
        lifeLeft = life;
    }


    void SetTeamToNeither() => team = Team.NEITHER;
    private void Update()
    {
        lifeLeft -= Time.deltaTime;
        if (lifeLeft <= 0){
            Explode();
        }
        if (trigger)
        {
            float dist = Vector2.Distance(transform.position, plT.position);
            if (dist < distance)
                Explode();
        }
    }
    void Explode()
    {
        rb.velocity = Vector2.zero;
        if (ani == null) gameObject.SetActive(false);
        else
        {
            if (heetSeeking) aiPath.enabled = false;
            string explode = "Explode";
            ani.SetTrigger(explode);
        }
    }
    public void SpawnExplosion()
    {
        explosion.transform.position = transform.position;
        explosion.SetActive(true);
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
    }

    public void SwitcHitboxActiveState()
    {
        active = !active;
        hitbox.enabled = active;
    }
    public void BulletLifeOver(){
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
            if (!trigger) player.TakeDamage(dmg, effect); 
            Explode();
            
        } else if ((team == Team.PLAYER || team == Team.NEITHER) && enemy) {
            if (!enemyBook.ContainsKey(coll.transform))
                enemyBook[coll.transform] = coll.GetComponent<HealthData>();
            enemyBook[coll.transform].TakeDamage(dmgVsEnemies); 
            Explode();
            
        }
        if (obs || gr) Explode();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (distance > 0) Gizmos.DrawWireSphere(transform.position, distance);
    }
}
