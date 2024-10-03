using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;

public class PlayerHealth : MonoBehaviour
{
    public int hp, maxHp, acidStacks, curEffects, maxEffects;
    public float defense;
    [Header("Timers")]
    public float fireTimer;
    public float iceTimer;
    public float invincibleTimer;
    public float shockTimer;
    public float acidicTimer;
    float shockTimeLeft, fireTimeLeft, iceTimeLeft, invincTimeLeft, acidicTimeLeft;
    bool acidic, shocked, onFire, iced, invincible;
    [Header("Hit Effects")]
    public GameObject[] effects;
    public Color[] colorStates;
    SpriteRenderer sprite;
    Player player;
    Controller contr;
    public static PlayerHealth Ins;

    private void Awake() => Ins = this;
    void Start()
    {
        player = GetComponent<Player>();
        contr = GetComponent<Controller>();
        sprite = GetComponent<SpriteRenderer>();
        hp = maxHp;
    }

    // Update is called once per frame
    void Update()
    {
        CalcCooldowns();
    }
    public void CalcCooldowns()
    {
        if (invincible){
            if (invincTimeLeft > 0) invincTimeLeft -= Time.deltaTime;
            else if (invincTimeLeft <= 0){
                invincible = false; invincTimeLeft = invincibleTimer;
            }
        }

        if (iced)
        {
            if (iceTimeLeft > 0) iceTimeLeft -= Time.deltaTime;
            else if (iceTimeLeft <= 0){
                iced = false; iceTimeLeft = iceTimer; player.speed = 15;
                curEffects--;
                effects[0].SetActive(false);
            }
        }

        if (onFire)
        {
            if (!invincible && fireTimeLeft > 0) fireTimeLeft -= Time.deltaTime;
            else if (fireTimeLeft <= 0){
                onFire = false; fireTimeLeft = fireTimer;
                curEffects--;
                effects[1].SetActive(false);
            }
        }

        if (shocked)
        {
            if (!invincible && shockTimeLeft > 0) shockTimeLeft -= Time.deltaTime;
            else if (shockTimeLeft <= 0)
            {
                shocked = false; shockTimeLeft = shockTimer;
                curEffects--;
                effects[2].SetActive(false);
            }
        }
        if (acidic)
        {
            if (!invincible && acidicTimeLeft > 0) acidicTimeLeft -= Time.deltaTime;
            else if (acidicTimeLeft <= 0)
            {
                acidic = false; acidicTimeLeft = acidicTimer; acidStacks = 0;
                curEffects--;
                effects[3].SetActive(false);
            }
        }

    }
    public void TakeDamage(int dmg, int effect = 0)
    {
        if (invincible) { return; }
        invincible = true;
        player.ani.SetTrigger("Hurt");

        dmg = (int)(dmg * defense);
        dmg = shocked ? (int)(dmg * 1.5f) : dmg;
        hp = hp - (dmg + acidStacks);
        player.velocity.y = player.dmgBounce.y;
        player.velocity.x = player.dmgBounce.x * -contr.dirX;
        if (effect > 0) ApplyEffect(effect);
        StartCoroutine(DamagedColors());

        if (hp <= 0) { print($"dead hp = {hp}"); player.ani.SetTrigger("Die"); }
    }
    public void ApplyEffect(int effect = 0)
    {
        bool canContinue = (acidic && effect == 4);
        if (curEffects < maxEffects || (effect > 0 && canContinue)) StartCoroutine(Effects(effect));
    }
    private void OnEnable() => hp = maxHp;
    IEnumerator Effects(int effect)
    {
        if (!acidic) curEffects++;
        switch (effect)
        {
            case 1: iced = true; iceTimeLeft = iceTimer; player.speed = 5; 
                effects[0].SetActive(true); break;
            case 2:
                onFire = true; fireTimeLeft = fireTimer;
                effects[1].SetActive(true);
                while (onFire)
                {
                    hp = invincible ? hp : hp - 1;
                    if (hp <= 0) { onFire = false; player.ani.SetTrigger("Die"); }
                    yield return new WaitForSeconds(1f);
                }
                break;
            case 3: shocked = true; shockTimeLeft = shockTimer; effects[2].SetActive(true); break;
            case 4:
                acidic = true; acidicTimeLeft = acidicTimer;
                effects[3].SetActive(true);
                acidStacks = Mathf.Min(acidStacks + 1, 3); break;
        }
    }
    IEnumerator DamagedColors()
    {
        while (invincible)
        {
            sprite.color = colorStates[0];
            yield return new WaitForSeconds(.25f);
            sprite.color = colorStates[1];
            yield return new WaitForSeconds(.25f);
        }
        sprite.color = colorStates[2];
    }
}
