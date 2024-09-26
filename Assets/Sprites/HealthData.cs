using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthData : MonoBehaviour
{
    public int hp, maxHp;
    [SerializeField] MovingObject waypoints;
    [SerializeField] AIPath ai;
    bool dying;
    SpriteRenderer spR;
    [SerializeField] Material[] colorStates;
    public Animator ani;
    // Start is called before the first frame update
    void Start()
    {
        ani = GetComponent<Animator>();
        spR = GetComponent<SpriteRenderer>();
        hp = maxHp;
    }

    private void OnEnable() {
        hp = maxHp;
        dying = false;
    }
    public void TakeDamage(int dmg)
    {
        hp -= dmg;

        
        if (dying) return;
        StartCoroutine(DamagedColors());
        if (hp <= 0)
        {
            dying = true;
            ani.SetTrigger("Death");
        }
    }
    IEnumerator DamagedColors()
    {
        spR.material = colorStates[1];
        yield return new WaitForSeconds(0.2f);
        spR.material = colorStates[0];
    }
    public void Die()
    {
        gameObject.SetActive(false);
    }
    // Update is called once per frame
    public void WaypointsSwitch(int yes = 0) => waypoints.enabled = yes == 1;
    public void AISwitch(int yes = 0) => ai.enabled = yes == 1;
}
