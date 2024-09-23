using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;

public class Obstacle : MonoBehaviour
{
    public enum ObstacleClass { DANGER, TRIGGER, INTERACTABLE, HEALING, EFFECT };
    public enum ObstacleType { SPIKES, SAWS, FIRE, EXPLOSION, FURY, BOMB, NONE };
    public enum Effect { ICE, FIRE, ElECTRIC, NONE };
    public Effect property;
    public ObstacleType type;
    public ObstacleClass classType;

    private bool isInitialized = false;
    [SerializeField] bool inRange;
    public int effect = 0, dmg, healAmount, index;
    bool active = false, startTimer, healed;
    public bool hasTimer, hasWayPoins, triggerByKeyPress;
    public float lifeLeft, lifeTimer;

    Throwable throwable;
    public GameObject[] triggerObjs; Collider2D hitbox;
    Manager manager;
    SpriteRenderer sprite; Player player; GameObject explosion;


    private void Awake()
    {
        hitbox = GetComponent<Collider2D>();
        startTimer = true; active = healed = false;
        sprite = GetComponent<SpriteRenderer>();
        
    }
    void Start()
    {
        player = Player.Ins;
        manager = Manager.Ins;
        if (classType != ObstacleClass.DANGER || type == ObstacleType.NONE) dmg = 0;
        else if (type == ObstacleType.SPIKES || type == ObstacleType.SAWS) dmg = 1;
        else if (type == ObstacleType.FIRE) dmg = 2;
        else if (type == ObstacleType.EXPLOSION) dmg = 4;
        else if (type == ObstacleType.FURY){
            dmg = 4;
            int ran = UnityEngine.Random.Range(0,10);
            if (ran > 4){
                transform.rotation = Quaternion.Euler(0, 0, 90f);
                transform.position = new Vector2(45, transform.position.y);
            } else{
                transform.position = new Vector2(transform.position.x, -9);
            }
        }
        switch (property)
        {
            case Effect.ICE: effect = 1; break;
            case Effect.FIRE: effect = 2; break;
            case Effect.ElECTRIC: effect = 3; break;
            case Effect.NONE: effect = 0; break;
        }
        if (type == ObstacleType.BOMB) { explosion = Instantiate(triggerObjs[0]); explosion.SetActive(false); throwable = GetComponent<Throwable>(); }
        lifeLeft = lifeTimer;
  
    }

    void Update()
    {
        if (hasTimer && startTimer)
        {
            lifeLeft -= Time.deltaTime;
            if (lifeLeft <= 1f)
            {
                LifeTimeOver();
            }
        }

        if (inRange && Input.GetKeyDown(KeyCode.E)) Trigger();
    }
    void LifeTimeOver()
    {
        if (lifeLeft <= 0) { print("nice try"); return; }
        sprite.enabled = false;
        if (type == ObstacleType.BOMB)
        {
            Explode();
        }
        else { 
            startTimer = false;
            manager.Swap(index);
            gameObject.SetActive(false); 
        }
    }
    public void ActivateHitBox()
    {
        active = !active;
        hitbox.enabled = active;
        healed = false;
    }
    private void OnEnable()
    {
        if (isInitialized) Reactivated();
        else isInitialized = true;
    }
    void Reactivated()
    {
        lifeLeft = lifeTimer; startTimer = true; sprite.enabled = true;
    
    }
    void Explode()
    {
        if (!startTimer) { print("why are you trying to go boom?"); return; }
        print("boom");
        sprite.enabled = false;
        explosion.transform.position = transform.position;
        hitbox.enabled = false;
        explosion.SetActive(true);
        throwable.col.enabled = true; throwable.beingCarried = throwable.inRange = startTimer = false;
        transform.parent = manager.transform;
        if (index >= 0) manager.Swap(index);
        gameObject.SetActive(false);
    }
    void Trigger()
    {
        print($"Triggering on {gameObject.name} spR? {sprite != null}");
        foreach (var obj in triggerObjs)
        {
            obj.SetActive(!obj.activeSelf);
        }
        hitbox.enabled = sprite.enabled = triggerByKeyPress;
        this.enabled = triggerByKeyPress;
        AstarPath.active.Scan();
    }
    private void OnTriggerEnter2D(Collider2D coll)
    {
        bool isPlayer = coll.CompareTag("Player");
       
        if (isPlayer)
        {
            if (classType == ObstacleClass.DANGER){
                print($"Hit em with the {type}");
                player.TakeDamage(dmg, effect);
            } else if (classType == ObstacleClass.HEALING && !healed)
            {
                print("LIVEEEE!! OOOOBBAAAAA!!!");
                player.hp += healAmount;
            } else if (classType == ObstacleClass.EFFECT) { player.ApplyEffect(effect); }
            else if (classType == ObstacleClass.TRIGGER){
                print($"Triggering...........:)........");
                if (!triggerByKeyPress) Trigger();
                else inRange = true;
            }
        }
        if (type == ObstacleType.EXPLOSION)
        {
            if (coll.CompareTag("Item")) coll.GetComponent<Obstacle>().LifeTimeOver();
            else if (coll.CompareTag("Throwable")) coll.GetComponent<Throwable>().HandleDestruction();
            else if (coll.CompareTag("Enemy")) coll.GetComponent<Enemy>().TakeDamage(dmg);
        }
        healed = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (classType == ObstacleClass.TRIGGER && triggerByKeyPress && collision.CompareTag("Player"))
            inRange = false;
    }
}


