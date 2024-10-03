using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class FireSpread : MonoBehaviour
{
    public bool burning;
    public Vector2[] spreadPoints;
    public bool thirdSpread;
    public float spreadTimer;
    public int spreadCount;
    public float timeToSpread;
    public GameObject firePrefab;
    public LayerMask mask;
    GameObject[] fires;
    bool contLeft, contRight;
    void Start()
    {
        timeToSpread = spreadTimer;
        spreadCount = 1;
        var Jim = ObjectManager.Ins;
        fires = new GameObject[7];
        for (int i = 0; i < 7; i++)
        {
            fires[i] = Instantiate(firePrefab);
            fires[i].SetActive(false);
            Jim.AddObject(fires[i], "Fire", "Hazard");
        }
    }
    private void OnEnable() => spreadCount = 1;


    void Update()
    {
        if (burning)
        {
            timeToSpread -= Time.deltaTime;
            int maxSpread = thirdSpread ? 3 : 2;
            // SpreaLeft is how far the fire will spread. each new fire will
            // have there spread left subtracted by 1
            if (timeToSpread <= 0 && spreadCount <= maxSpread) Spread(spreadCount);
            else if (timeToSpread <= -.3f && spreadCount > maxSpread)
            {
                spreadCount = 1;
                foreach (var fire in fires)
                    fire.SetActive(false);
                gameObject.SetActive(false);
            }
        }
    }
    void Spread(int spread)
    {
        int i1 = 3 - spread, i2 = 3 + spread;
        timeToSpread = spreadTimer;
        Vector2 p1 = spreadPoints[i1] + (Vector2)transform.position;
        Vector2 p2 = spreadPoints[i2] + (Vector2)transform.position;
        RaycastHit2D hit1 = Physics2D.Raycast(p1, Vector2.down, 10f, mask);
        RaycastHit2D hit2 = Physics2D.Raycast(p2, Vector2.down, 10f, mask);

        bool wallCheck1 = Physics2D.OverlapPoint(p1, mask);
        bool wallCheck2 = Physics2D.OverlapPoint(p2, mask);

        if (hit1 && !wallCheck1)
        {
            
            if (hit1.point.y > transform.position.y - 2.5f && contLeft)
            {
                fires[i1].SetActive(true);
                fires[i1].transform.position = hit1.point;
            } else contLeft = false;
            print($"First Hit index {i1}    point {hit1.point}   position {(Vector2)fires[i1].transform.position}");
        }
        else contLeft = false;

        if (hit2 && !wallCheck2)
        {
            if (hit2.point.y > transform.position.y - 2.5f && contRight)
            {
                fires[i2].transform.position = hit2.point;
                fires[i2].SetActive(true);
            }
            else contRight = false;
            print($"Second Hit index {i1}    point {hit1.point}   position {(Vector2)fires[i1].transform.position}");
        }
        else contRight = false;

        spreadCount++;
    }
    void StartFire()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 4f, mask);
        if (hit)
        {
            contLeft = contRight = burning = true;
            fires[3].transform.position = hit.point;
            fires[3].SetActive(true);
            spreadCount = 1;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        foreach (var point in spreadPoints)
        {
            Vector2 pos = point + (Vector2)transform.position;
            Gizmos.DrawLine(pos - Vector2.up * .4f, pos + Vector2.up * .4f);
            Gizmos.DrawLine(pos - Vector2.left * .4f, pos + Vector2.left * .4f);
        }
    }
}
