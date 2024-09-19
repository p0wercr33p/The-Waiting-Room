using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;

public class Enemy : MonoBehaviour
{
    public enum EnemyType { MELEE, RANGED, HYBRID, DUMMY }
    public enum BulletType { PROJECTILE, LASER, HITSCAN };
    Animator ani;
    public BulletType bullet;
    public EnemyType type; string playerTag; Player player;
    (GameObject pr, Projectile info)[] projectiles;
    public LayerMask laserMask;
    public GameObject projectile;
    public Color[] dummyRainbow;
    public SpriteRenderer spR;
    public LineRenderer laserLine;


    float maxLaserDist = 80f, laserTimeLeft;
    int colorInd = 0;
    public int dmg, cap, contactDmg, hp;
    public float timeLeft, cooldown;
    bool canAttack, attacking;
    public float laserTime;
    public bool aimAtPlayer, hasWayPoints, lookAtShootDir;
    private bool isInitialized;
    public Vector2 shootDir;
    [SerializeField] Transform firePoint;

    private void Awake()
    {

        spR = GetComponent<SpriteRenderer>();
        ani = GetComponent<Animator>();
        canAttack = false; timeLeft = cooldown;
        playerTag = "Player";
    }
    void Start()
    {
        player = Player.Ins;
        InitializeProjectiles();
        if (!hasWayPoints) Rotate();
    }
    public void Rotate()
    {
        if (lookAtShootDir)
        {
            switch (shootDir)
            {
                case Vector2 v when v == Vector2.up:
                    transform.rotation = Quaternion.Euler(0, 0, 0);  // No rotation, pointing upwards
                    break;
                case Vector2 v when v == Vector2.down:
                    transform.rotation = Quaternion.Euler(0, 0, 180);  // 180 degrees to point down
                    break;
                case Vector2 v when v == Vector2.right:
                    transform.rotation = Quaternion.Euler(0, 0, -90);  // -90 degrees to point right
                    break;
                case Vector2 v when v == Vector2.left:
                    transform.rotation = Quaternion.Euler(0, 0, 90);  // 90 degrees to point left
                    break;
                default:
                    transform.rotation = Quaternion.identity;  // Reset rotation to default
                    break;
            }
        }
    }
    void InitializeProjectiles()
    {
        if (type != EnemyType.MELEE && type != EnemyType.DUMMY && bullet == BulletType.PROJECTILE)
        {
            print("initializing");
            projectiles = new (GameObject, Projectile)[cap];
            for (int i = 0; i < cap; i++)
            {
                GameObject newProj = Instantiate(projectile);
                Projectile newProjInfo = newProj.GetComponent<Projectile>();
                projectiles[i] = (newProj, newProjInfo);
                newProj.SetActive(false);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!canAttack && !attacking)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0)
            {
                canAttack = true;
                timeLeft = cooldown;
            }
        }
        else if (canAttack && !attacking)
        {
            canAttack = false;
            print("attacking");
            attacking = true;
            ani.SetTrigger("AttackStartUp");
        }
    }

    public void ShootLogic()
    {
        Vector2 dir = (player.transform.position - firePoint.position).normalized;

        if (bullet == BulletType.PROJECTILE) Shoot(dir);
        else if (bullet == BulletType.LASER) StartCoroutine(HoldLaserFire(shootDir));

    }
    public IEnumerator HoldLaserFire(Vector2 dir)
    {
        print("holding down laser fire");
        attacking = true;
        laserTimeLeft = laserTime;

        ani.SetTrigger("HoldLaserFire");
        while (laserTimeLeft > 0)
        {
            laserTimeLeft -= Time.deltaTime;
            ShootLaser(dir);

            yield return null;
        }
        ani.SetTrigger("End");
        attacking = false;
        laserLine.SetPosition(0, transform.position);
        laserLine.SetPosition(1, transform.position);
    }

    void ShootLaser(Vector2 dir)
    {
        print("firing da laser");
        RaycastHit2D laserHit = Physics2D.Raycast((Vector2)firePoint.position, dir, maxLaserDist, laserMask);

        if (laserHit)
        {
            print($"hit {laserHit.transform.name} (its tag --> {laserHit.transform.tag}");
            DrawLaser(transform.position, laserHit.point);
            if (laserHit.transform.tag == playerTag) player.TakeDamage(dmg);
        }
        else
        {
            Vector2 endPos = (Vector2)transform.position + dir * maxLaserDist;
            DrawLaser(transform.position, endPos);
        }
    }

    void DrawLaser(Vector2 startPos, Vector2 endPos)
    {
        laserLine.SetPosition(0, startPos);
        laserLine.SetPosition(1, endPos);
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
    public void Shoot(Vector2 dir)
    {
        (GameObject proj, Projectile comp) = GetProjectile();


        if (proj != null)
        {
            proj.SetActive(true);
            proj.transform.position = firePoint.position;
            if (aimAtPlayer)
            {
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                proj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
                comp.rb.velocity = proj.transform.right * comp.speed;
            }
            else { comp.rb.velocity = shootDir * comp.speed; }
        }
        else print("couldn't find a prjectile");
        attacking = false;
    }
    public void TakeDamage(int dmg)
    {
        hp -= dmg;
        if (type == EnemyType.DUMMY)
        {
            spR.color = dummyRainbow[colorInd];
            print($"OUCH!!!! *blush*  you did ----  {dmg}  ---- Damage ");
            colorInd = (colorInd + 1) % dummyRainbow.Length;
        }
        if (hp <= 0) Die();
    }

    private void OnEnable()
    {
        if (isInitialized) Reactivated();
        else isInitialized = true;
    }
    void Reactivated()
    {
        spR.enabled = true;
    }
    void Die()
    {
        if (type == EnemyType.DUMMY) return;
        foreach (var p in projectiles)
            p.pr.SetActive(false);
        gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag) && contactDmg > 0)
        {
            player.TakeDamage(contactDmg);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawLine((Vector2)firePoint.position - Vector2.up * .4f, (Vector2)firePoint.position + Vector2.up * .4f);
        Gizmos.DrawLine((Vector2)firePoint.position - Vector2.left * .4f, (Vector2)firePoint.position + Vector2.left * .4f);
    }
}
