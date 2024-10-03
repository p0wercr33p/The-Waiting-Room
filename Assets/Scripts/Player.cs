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
    [HideInInspector] public Animator ani;
    float vXSmoothing, wallStickTime = 0, wDirX, tarVX, wallStickTimeLeft;
    [HideInInspector] private float accel = .2f, wallSp, airAccel = .1f; 
    [HideInInspector] private float[] jumpData = new float[] { 4, .5f, 2, .25f, 9, 1.2f };
    public float maxJPower, minJPower, maxJHeight = 4f, jumpApexTime = .5f, minJHeight;
    [HideInInspector] public Controller contr;
    [HideInInspector] public bool flipped, wallClimbing, crouching, carryingItem;
    [SerializeField] public float gravity, speed;
    public Vector3 velocity;
    public int carryId; 
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
        flipped = wallClimbing = false;
        contr = GetComponent<Controller>();
        JumpHeights(maxJHeight, jumpApexTime, minJHeight);
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
        CalcVelocity();
        HandleWallSliding();

        if (Input.GetKeyDown(KeyCode.Alpha0)) JumpHeights(maxJHeight, jumpApexTime, minJHeight);

        contr.Move(velocity * Time.deltaTime, input);
        if (contr.cols.above || contr.cols.below) velocity.y = 0;
        if (contr.cols.left || contr.cols.right) velocity.x = 0;
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
    
    public void Die() => gameObject.SetActive(false);
}

