using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    public float speed, life; public int dmg; public float lifeLeft;
    public Rigidbody2D rb; Player player; string playerTag, enemyTag, projectileTag;
    public enum Team { PLAYER, ENEMY };
    public Team team; public int effect;
    Dictionary<Transform, Enemy> enemyBook;
    
    void Start()
    {
        enemyTag = "Enemy";
        player = Player.Ins; playerTag = "Player";
        projectileTag = "Projectile";
        enemyBook = new Dictionary<Transform, Enemy>();
        rb = GetComponent<Rigidbody2D>(); lifeLeft = life;
    }

    private void Update()
    {
        lifeLeft -= Time.deltaTime;
        if (lifeLeft <= 0){
            print($"My time on this planet is over"); this.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        lifeLeft = life;
    }
    private void OnTriggerEnter2D(Collider2D coll)
    {
        print($"Hit da {coll.tag}");
        bool pl = coll.CompareTag(playerTag);
        bool enemy = coll.CompareTag(enemyTag);
        bool pr = coll.CompareTag(projectileTag);
        if (team == Team.ENEMY && pl){
            player.TakeDamage(dmg, effect); gameObject.SetActive(false);
        }
        else if (team == Team.PLAYER && enemy)
        {
            if (!enemyBook.ContainsKey(coll.transform))
                enemyBook[coll.transform] = coll.GetComponent<Enemy>();
            enemyBook[coll.transform].TakeDamage(dmg); gameObject.SetActive(false);
        }
        if (!pl && !enemy && !pr) gameObject.SetActive(false);
    }
}
