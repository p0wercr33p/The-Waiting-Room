using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasSpread : MonoBehaviour
{
    public Vector2 scale, curScale;
    public CircleCollider2D col;
    public float growthTimer;
    [HideInInspector] public int effect;
    float nextGrowthTime;
    Player player;
    bool canPoison;
    public float poisonTime;
    bool start;
    void Start()
    {
        scale = curScale = transform.localScale;
        effect = 4;
        col = GetComponent<CircleCollider2D>();
        nextGrowthTime = growthTimer;
        canPoison = true; start = false;
        player = Player.Ins;
    }
    IEnumerator StartGasCloud()
    {
        start = false;
        yield return new WaitForSeconds(8f);
        start = true;
    }


    // Update is called once per frame
    void Update()
    {
        nextGrowthTime -= start ? Time.deltaTime : 0;
        if (nextGrowthTime <= 0)
        {
            nextGrowthTime = growthTimer;
            curScale += (Vector2.one * 2);
            curScale.x = Mathf.Clamp(curScale.x, 1, 45);
            curScale.y = Mathf.Clamp(curScale.y, 1, 45);
            transform.localScale = curScale;
        }
    }
    private void OnEnable()
    {
        nextGrowthTime = growthTimer;
        curScale = scale;
        transform.localScale = curScale;
        StartCoroutine(StartGasCloud());
    }
    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.CompareTag("Player"))
        {
            if (canPoison && curScale.x > 7)
            {
                col.enabled = false;
                canPoison = false;
                player.ApplyEffect(effect);
                StartCoroutine(RefreshPoison());
            }
        }
    }
    IEnumerator RefreshPoison()
    {
        yield return new WaitForSeconds(poisonTime);
        canPoison = true;
        col.enabled = true;
    }
}
