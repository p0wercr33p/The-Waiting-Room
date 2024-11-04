using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class Multishot : MonoBehaviour
{
    public Transform[] firePoints;
    public Vector2[] dir;
    public GameObject iceOrbPrefab;
    (GameObject orb, Projectile comp)[] iceOrbs;
    public float cooldown, rotateSpeed, rotation;
    ObjectManager Jim;
    float timeLeft;
    public bool secondRound;
    public LayerMask mask;
    void Start()
    {
        timeLeft = cooldown;

        if (rotation == 1.69)
        {
            dir = new Vector2[4] {
                new Vector2(-1,1), new Vector2(1, 1),
                new Vector2(1, -1), new Vector2(-1, -1)
            };
        }
        
        rotation = 0;
        secondRound = false;
        InitializeBullets();  
    }
    void InitializeBullets()
    {
        Transform kts = GameObject.FindGameObjectWithTag("ProjectileHolder").transform;
        Jim = ObjectManager.Ins;
        iceOrbs = new (GameObject, Projectile)[16];
        for (int i = 0; i < 16; i++)
        {
            GameObject orb = Instantiate(iceOrbPrefab, kts);
            Projectile comp = orb.GetComponent<Projectile>();
            iceOrbs[i] = (orb, comp);
            Jim.AddObject(orb, "ORB", "Projectile");
        }
    }
    // Update is called once per frame
    void Update()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0)
        {
            timeLeft = cooldown;
            Shoot();
        }
        rotation += rotateSpeed * Time.deltaTime;
        rotation = rotation % 360;
        transform.localRotation = Quaternion.Euler(0, 0, rotation);
    }
    IEnumerator ShootLogic()
    {
        Shoot();
        if (secondRound)
        {
            yield return new WaitForSeconds(1.5f);
            Shoot();
        }
    } 
    void Shoot()
    {
        List<(GameObject orb, Projectile comp)> curOrbs = new();
        for (int i = 0; i < 12; i++)
        {
            if (curOrbs.Count == 4) break;
            if (!iceOrbs[i].orb.activeInHierarchy)
            {
                curOrbs.Add(iceOrbs[i]);
            }
        }
        if (curOrbs.Count != 0)
        {
            int ind = 0;
            foreach (var iceOrb in curOrbs)
            {
                bool wallCheck1 = Physics2D.OverlapPoint(firePoints[ind].position, mask);
                
                if (!wallCheck1)
                {
                    iceOrb.orb.transform.position = firePoints[ind].position;
                    Vector2 rotatedDir = RotateVector2(dir[ind], rotation);
                    iceOrb.orb.SetActive(true);
                    iceOrb.comp.rb.velocity = rotatedDir * iceOrb.comp.speed;
                }
                ind++;
            }
        }
    }
    Vector2 RotateVector2(Vector2 v, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);
        return new Vector2(
            cos * v.x - sin * v.y,
            sin * v.x + cos * v.y
        );
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        foreach (var point in firePoints)
        {
            Vector2 pos = point.position;
            Gizmos.DrawLine(pos - Vector2.up * .4f, pos + Vector2.up * .4f);
            Gizmos.DrawLine(pos - Vector2.left * .4f, pos + Vector2.left * .4f);
        }
    }
}
