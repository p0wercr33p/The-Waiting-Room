using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    Player player;
    float dirX;
    public Transform keepToScale;
    [HideInInspector] public Vector2 input;
    [HideInInspector] public SpriteRenderer sprite;
    Animator ani; 
    [HideInInspector] bool running, grounded, attacking;
    Vector3 baseScale;
    Vector3 curPlayerScale;

    [Header("Projectile Dictionary Values")]
    [SerializeField] public ProjDict superBlast;
    [SerializeField] public ProjDict powerBlast;


    int ind;
    public string[] projNames;

    public Dictionary<string, ProjDict> projs;
    public Dictionary<string, int> usages;
    public static PlayerInput Ins;
    public string cur;
    private void Awake() => Ins = this;
    void Start()
    {
        projNames = new string[] { "SuperBlast", "PowerBlast" };
        ind = 0;
        projs = new();
        usages = new();
        attacking = false;
        dirX = 1;
        player = Player.Ins;
        sprite = GetComponent<SpriteRenderer>();
        ani = GetComponent<Animator>();
        baseScale = player.transform.localScale;
        curPlayerScale = baseScale;
        InitializeProjectiles();
    }
    void InitializeProjectiles()
    {
        usages["SuperBlast"] = 4;
        usages["PowerBlast"] = 4;
        superBlast.pr = new GameObject[superBlast.max];
        superBlast.info = new Projectile[superBlast.max];
        for (int i = 0; i < superBlast.max; i++)
        {
            GameObject pr = Instantiate(superBlast.projectile, keepToScale); 
            Projectile info = pr.GetComponent<Projectile>();
            pr.SetActive(false); 
            superBlast.pr[i] = pr;
            superBlast.info[i] = info;
        }
        projs["SuperBlast"] = superBlast;

        powerBlast.pr = new GameObject[powerBlast.max];
        powerBlast.info = new Projectile[powerBlast.max];
        for (int i = 0; i < powerBlast.max; i++)
        {
            GameObject pr = Instantiate(powerBlast.projectile, keepToScale);
            Projectile info = pr.GetComponent<Projectile>();
            pr.SetActive(false);
            powerBlast.pr[i] = pr;
            powerBlast.info[i] = info;
        }
        projs["PowerBlast"] = powerBlast;
    }

    // Update is called once per frame
    void Update()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        player.input = input;
        if (input.x != 0){
            running = true;
            curPlayerScale.x = input.x == -1 ? -baseScale.x : baseScale.x;
            dirX = input.x;
        } else running = false;
        curPlayerScale.y = player.flipped ? -baseScale.y : baseScale.y;
        player.transform.localScale = curPlayerScale;
        player.crouching = input.y == -1;

        cur = projNames[ind];
        if (Input.GetKeyDown(KeyCode.Q)) ind = (ind + 1) % projNames.Length;
        if (Input.GetKeyDown(KeyCode.G) && usages[cur] > 0 && !attacking) {
            attacking = true;
            ani.SetBool("Attacking", attacking);
            ani.SetTrigger(cur);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6)) usages[cur] += 3;
        HandleAnimations();
        if (Input.GetKeyDown(KeyCode.Space)) { player.OnJumpInputDown(); }
        if (Input.GetKeyUp(KeyCode.Space)) { player.OnJumpInputUp(); }
    }
    public void ShootLogic()
    {
        attacking = false;
        ani.SetBool("Attacking", attacking);
        Shoot(projs[cur], cur);
    }
    (GameObject, Projectile) GetProjectile(ProjDict pd)
    {
        for (int i = 0; i < pd.max; i++)
        {
            if (!pd.pr[i].activeInHierarchy)
                return (pd.pr[i], pd.info[i]);
        }
        return (null, null);
    }
    public void ChangeStruct(ProjDict pd, string _name){
        switch (name)
        {
            case "SuperBlast": superBlast = pd; projs[_name] = pd; break;
            case "PowerBlast": powerBlast = pd; projs[_name] = pd; break;
        }
    }
    void Shoot(ProjDict pd, string _name)
    {
        if (usages[_name] <= 0) return;

        (GameObject proj, Projectile comp) = GetProjectile(pd);
        

        if (proj != null)
        {
            usages[_name]--;
            proj.SetActive(true);

            proj.transform.position = pd.firePoint.position;
            if (pd.mouseAim)
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 dir = (mousePos - (Vector2)proj.transform.position).normalized;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                proj.transform.rotation = Quaternion.Euler(0, 0, angle);
                dirX = 1;
            }
            comp.rb.velocity = (proj.transform.right * comp.speed) * dirX;
        }
    }
    void HandleAnimations()
    {
        if (PauseEvent.Paused) return;
        grounded = player.flipped ? player.contr.cols.above : player.contr.cols.below;
        
        ani.SetInteger("MoveDirX", (int)input.x);
        ani.SetBool("Moving", running);
        ani.SetBool("WallSliding", player.wallClimbing);
        ani.SetBool("Grounded", grounded);
    }

    [System.Serializable]
    public struct ProjDict
    {
        public int max;
        public GameObject projectile;
        public GameObject[] pr;
        public Projectile[] info;
        public Transform firePoint;
        public bool mouseAim, isProj;
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        
        Vector2 pos = superBlast.firePoint.position;
        Gizmos.DrawLine(pos - Vector2.up * .4f, pos + Vector2.up * .4f);
        Gizmos.DrawLine(pos - Vector2.left * .4f, pos + Vector2.left * .4f);

        Gizmos.color = Color.green;
        
        pos = powerBlast.firePoint.position;
        Gizmos.DrawLine(pos - Vector2.up * .4f, pos + Vector2.up * .4f);
        Gizmos.DrawLine(pos - Vector2.left * .4f, pos + Vector2.left * .4f);
    }
}
