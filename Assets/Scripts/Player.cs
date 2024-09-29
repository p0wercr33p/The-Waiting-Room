using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.UIElements;
using static Unity.Burst.Intrinsics.X86;

public class Player : MonoBehaviour
{
    Animator ani;
    float vXSmoothing, wallStickTime = 0;
    [HideInInspector] private float accel = .2f, wallSp, airAccel = .1f; 
    [HideInInspector] private float[] jumpData = new float[] { 4, .5f, 2, .25f, 9, 1.2f };
    public float maxJPower, minJPower, maxJHeight = 4f, jumpApexTime = .5f, minJHeight;
    [HideInInspector] public Controller contr;
    [HideInInspector] public bool flipped, wallClimbing, crouching;

    [SerializeField] private float gravity, speed, fireTimer, iceTimer, invincibleTimer, shockTimer, shockTimeLeft,acidicTimer;
    [SerializeField] private Vector3 velocity;
    private float wDirX, tarVX, wallStickTimeLeft, iceTimeLeft, fireTimeLeft, invincTimeLeft = 2,acidicTimeLeft;
    public int carryId, hp, maxHp, acidStacks; 
    private bool hasStatusEffectOn, invincible, onFire, iced, acidic;
    SpriteRenderer sprite;
    public Color[] colorStates;
    public bool shocked, carryingItem;
    public Vector2 wallKick, wallClimb, dmgBounce, input;
    public static Player Ins;

    private void Awake()
    {
        if (Ins == null)
            Ins = this;
        
        ani = GetComponent<Animator>();
    }
    void Start()
    {
        invincible = flipped = wallClimbing = false;
        sprite = GetComponent<SpriteRenderer>();
        contr = GetComponent<Controller>();
        JumpHeights(maxJHeight, jumpApexTime, minJHeight);
        hp = maxHp;
    }

    public void OnJumpInputDown()
    {
        if (wallClimbing)
        {

            if (wDirX == input.x)
            {
                velocity.x = -wDirX * wallClimb.x;
                velocity.y = wallClimb.y;
            }
            else
            {
                velocity.x = -wDirX * wallKick.x;
                velocity.y = wallKick.y;
            }
            ani.SetTrigger("Jumped");
        }
        else if ((!flipped && (contr.cols.below || contr.canCJump)) || (flipped && (contr.cols.above || contr.canCJump)))
        { velocity.y = flipped ? -maxJPower : maxJPower; contr.canCJump = contr.wasGrounded = false; contr.justJumped = true; ani.SetTrigger("Jumped"); }
    }
    public void OnJumpInputUp()
    {

        if ((!flipped && velocity.y > minJPower) || (flipped && velocity.y < -minJPower))
            velocity.y = flipped ? -minJPower : minJPower;
    }
    // Update is called once per frame
    void Update()
    {
        if (hasStatusEffectOn || invincible) CalcCooldowns();
        CalcVelocity();
        HandleWallSliding();

        if (Input.GetKeyDown(KeyCode.Alpha0)) JumpHeights(maxJHeight, jumpApexTime, minJHeight);

        contr.Move(velocity * Time.deltaTime, input);
        if (contr.cols.above || contr.cols.below) velocity.y = 0;
        if (contr.cols.left || contr.cols.right) velocity.x = 0;
    }
    public void CalcCooldowns()
    {
        if (invincible)
        {
            if (invincTimeLeft > 0) invincTimeLeft -= Time.deltaTime;
            else if (invincTimeLeft <= 0)
            {
                invincible = false; invincTimeLeft = invincibleTimer;
                sprite.color = colorStates[3];
            }
        }

        if (iced)
        {
            if (iceTimeLeft > 0)
            { iceTimeLeft -= Time.deltaTime; if (!invincible) sprite.color = colorStates[4]; }
            else if (iceTimeLeft <= 0)
            {
                iced = false; iceTimeLeft = iceTimer; speed = 15;
                sprite.color = colorStates[3]; hasStatusEffectOn = false;
            }
        }
        
        if (onFire)
        {
            if (!invincible && fireTimeLeft > 0)
            { fireTimeLeft -= Time.deltaTime; sprite.color = colorStates[5]; }
            else if (fireTimeLeft <= 0)
            {
                onFire = false; fireTimeLeft = fireTimer;
                sprite.color = colorStates[3]; hasStatusEffectOn = false;
            }
        }
        
        if (shocked)
        {
            if (!invincible && shockTimeLeft > 0)
            { shockTimeLeft -= Time.deltaTime; sprite.color = colorStates[6]; }
            else if (shockTimeLeft <= 0)
            {
                shocked = false; shockTimeLeft = shockTimer;
                sprite.color = colorStates[3]; hasStatusEffectOn = false;
            }
        }
        if (acidic)
        {
            if (!invincible && acidicTimeLeft > 0)
            { acidicTimeLeft -= Time.deltaTime; sprite.color = colorStates[7]; }
            else if (acidicTimeLeft <= 0)
            {
                acidic = false; acidicTimeLeft = acidicTimer; acidStacks = 0;
                sprite.color = colorStates[3]; hasStatusEffectOn = false;
            }
        }

    }
    public void HandleWallSliding()
    {
        wDirX = contr.cols.left ? -1 : 1;
        wallClimbing = false;
        if ((!contr.cols.above && !contr.cols.below && velocity.y < 0) && (contr.cols.right || contr.cols.left))
        {
            wallClimbing = true;

            if (velocity.y < -wallSp) velocity.y = -wallSp;

            if (wallStickTimeLeft > 0)
            {
                vXSmoothing = 0;
                velocity.x = 0;
                if (input.x == -wDirX)
                    wallStickTimeLeft -= Time.deltaTime;
                else { wallStickTimeLeft = wallStickTime; }
            }
            else { wallStickTimeLeft = wallStickTime; }
        }
    }
    public void CalcVelocity()
    {
        tarVX = input.x * speed;
        velocity.x = Mathf.SmoothDamp(velocity.x, tarVX, ref vXSmoothing, (contr.cols.below) ? accel : airAccel);
        velocity.y += !flipped ? gravity * Time.deltaTime : 0;
        velocity.y -= flipped ? gravity * Time.deltaTime : 0;
    }
    void JumpHeights(float maxJpHeight, float JApex, float minJHeight)
    {
        gravity = -(2 * maxJpHeight) / Mathf.Pow(JApex, 2);
        maxJPower = Mathf.Abs(gravity) * jumpApexTime;
        minJPower = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJHeight);
    }
    public void ChangeGravity(int val)
    {
        switch (val)
        {
            case 0: flipped = !flipped; break;
            case 1: jumpApexTime = jumpData[5]; maxJHeight = jumpData[4]; minJHeight = 2.5f; break;
            case 2: jumpApexTime = jumpData[3]; maxJHeight = jumpData[2]; minJHeight = .5f; break;
            case 3: jumpApexTime = jumpData[1]; maxJHeight = jumpData[0]; minJHeight = 1f; break;
        }
        JumpHeights(maxJHeight, jumpApexTime, minJHeight);
    }
    public void TakeDamage(int dmg, int effect = 0)
    {
        if (invincible) { print("hes.... invinci"); return; }
        invincible = true;
        ani.SetTrigger("Hurt");
        dmg = shocked ? (int)(dmg * 1.5f) : dmg;
        hp = hp - (dmg + acidStacks);
        velocity.y = dmgBounce.y;
        velocity.x = dmgBounce.x * -contr.dirX;
        if (effect > 0) ApplyEffect(effect);
        StartCoroutine(DamagedColors());

        if (hp <= 0) { print($"dead hp = {hp}"); ani.SetTrigger("Die"); }
    }
    public void ApplyEffect(int effect = 0)
    {
        bool canContinue = (!hasStatusEffectOn || (acidic && effect == 4));
        if (effect > 0 && canContinue) StartCoroutine(Effects(effect));
    }
    IEnumerator Effects(int effect)
    {
        hasStatusEffectOn = true;
        switch (effect)
        {
            case 1: iced = true; iceTimeLeft = iceTimer; speed = 5; break;
            case 2:
                onFire = true; fireTimeLeft = fireTimer;
                while (onFire)
                {
                    hp = invincible ? hp : hp - 1;
                    if (hp <= 0) { onFire = false; print("you burned to death! bum!"); ani.SetTrigger("Die"); }
                    yield return new WaitForSeconds(1f);
                }
                break;
            case 3: shocked = true; shockTimeLeft = shockTimer; break;
            case 4: acidic = true; acidicTimeLeft = acidicTimer; 
                acidStacks = Math.Min(acidStacks + 1, 3); break;
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
        sprite.color = colorStates[3];
    }
    private void OnEnable() => hp = maxHp;
    public void Die() => gameObject.SetActive(false);
}

