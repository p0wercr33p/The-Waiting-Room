using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;

public class Projectile : MonoBehaviour
{
    public float speed, life, distance;
    public int dmg, dmgVsEnemies, effect, maxBounces;
    float lifeLeft;
    public Rigidbody2D rb; 
    Player player; Transform plT; PlayerHealth pHealth;
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
    int bounceCount;
    public LayerMask mask;

    private void Awake()
    {
        hitbox = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }
    void Start()
    { 
        player = Player.Ins; pHealth = PlayerHealth.Ins;
        plT = player.transform;
        enemyTag = "Enemy"; 
        playerTag = "Player";
        obstacleTag = "Obstacle";
        groundTag = "Ground";
        bounceCount = 0;
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
        lifeLeft = life; hitbox.enabled = true; active = true; bounceCount = 0;
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
        bool obs = coll.CompareTag(obstacleTag);
        bool gr = coll.CompareTag(groundTag);
        if (obs || gr)
        {
            if (maxBounces <= 0) Explode();
            else
            {
                float yDir = Mathf.Sign(rb.velocity.y);
                float xDir = Mathf.Sign(rb.velocity.x);
                float mag = rb.velocity.magnitude * Time.deltaTime;
                Vector2 pos = transform.position;
                RaycastHit2D roof = Physics2D.Raycast(pos, Vector2.up * yDir, 4, mask);
                RaycastHit2D wall = Physics2D.Raycast(pos, Vector2.right * xDir, 4, mask);
                float rDist = roof.collider != null ? roof.distance : -1;
                float wDist = wall.collider != null ? wall.distance : -1;
                print($"roof{rDist} >< wall{wDist}");
                Debug.DrawRay(transform.position, Vector2.up * yDir, Color.yellow, 2f);
                Debug.DrawRay(transform.position, Vector2.right * xDir, Color.yellow, 2f);
                if (roof.collider != null && (wall.collider == null || roof.distance < wall.distance)){
                    print("roof hit");
                    Bounce(roof.normal);
                }else if (wall.collider != null) { 
                    print("wall hit");
                    Bounce(wall.normal);
                } else { print("miss"); Bounce(rb.velocity.normalized); }
            }
        }

        bool pl = coll.CompareTag(playerTag);
        bool enemy = coll.CompareTag(enemyTag);

        if (pl && (team == Team.ENEMY || team == Team.NEITHER))
        {
            if (!trigger) pHealth.TakeDamage(dmg, effect);
            Explode();

        }
        else if (enemy && (team == Team.PLAYER || team == Team.NEITHER))
        {
            if (!enemyBook.ContainsKey(coll.transform))
                enemyBook[coll.transform] = coll.GetComponent<HealthData>();
            enemyBook[coll.transform].TakeDamage(dmgVsEnemies);
            Explode();
        }
    }
    private void Bounce(Vector2 normal)
    {
        bounceCount++;
        if (bounceCount > maxBounces)
        {
            Explode();
            return;
        }
        print($"velocity {rb.velocity}   direction {rb.velocity.normalized}");
        Vector2 dir = Vector2.Reflect(rb.velocity.normalized, normal);
        print($"hit.normal: {normal.y}Y and {normal.x}X");

        rb.velocity = dir * speed;
        print($"New velocity {rb.velocity}   New direction {dir}");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (distance > 0) Gizmos.DrawWireSphere(transform.position, distance);
    }
}
