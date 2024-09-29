using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static Obstacle;

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager Ins;
    public string curMod;
    public bool Plus;
    public string[] mods; 
    public int ind;
    public DROIDS Droids;
    public _Bombs Bombs;
    public _Explosions Explosions;
    public _Saws Saws;
    public _Flames Fire;
    public _Cannons Cannons;
    public Projectiles Projs;
    public HeavensFuries Furies;
    public _Toxic Toxic;
    private void Awake() {
        Ins = this;
        Droids = new DROIDS(1);
        Bombs = new _Bombs(1);
        Saws = new _Saws(1);
        Explosions = new _Explosions(1);
        Cannons = new _Cannons(1);
        Projs = new Projectiles(1);
        Furies = new HeavensFuries(1);
        Toxic = new _Toxic(1);
        Fire = new _Flames(1);
        ind = 0;
        mods = new string[] { 
            "Bigger Bombs", "Tankier Tanks", "Amber Alert", "Hasty Rockets", "Bigger Gun Diplomacy",
            "Schrodinger's Damager Numbers", "Property Purgatory", "Newton's New Law",
            "Wrath Of The Gods", "Foot Soldier", "Slower Than A Speeding Bullet", 
            "Give Me Liberty Give Me Fire", "Make Love Not Waste"
        };
        curMod = mods[ind];
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ind = (ind) % mods.Length;
            curMod = mods[ind];
            AddMod(curMod);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ind = (ind + 1) % mods.Length;
            curMod = mods[ind];
        }
    }
    public void AddMod(string ModToAdd)
    {
        switch (ModToAdd)
        {
            case "Bigger Bombs": BiggerBombs(Plus); break;
            case "Tankier Tanks": TankierTanks(Plus); break;
            case "Amber Alert": AmberAlert(Plus); break;
            case "Hasty Rockets": HastyRockets(Plus); break;
            case "Schrodinger's Damager Numbers": SchrodingersDamagerNumbers(Plus); break;
            case "Property Purgatory": PropertyPurgatory(Plus); break;
            case "Newton's New Law": NewtonsNewLaw(); break;
            case "Bigger Gun Diplomacy": BiggerGunDiplomacy(Plus); break;
            case "Wrath Of The Gods": WrathOfTheGods(Plus); break;
            case "Foot Soldier": FootSoldier(Plus); break;
            case "Slower Than A Speeding Bullet": SlowerThanASpeedingBullet(Plus); break;
            case "Give Me Liberty Give Me Fire": GiveMeLibertyGiveMeFire(Plus); break;
            case "Make Love Not Waste": MakeLoveNotWaste(Plus); break;
        }
    }
    public void BiggerGunDiplomacy(bool plus = false)
    {
        // Cannons and their projectiles get 35% bigger a deal 1 extra damage
        // Plus+ -- 50% bigger. 2 extra damage
        int dmg = plus ? 2 : 1;
        float scaleInc = plus ? 1.5f : 1.35f;
        Cannons.lc.curDmg += dmg;
        Cannons.lc.curScale = Cannons.lc.scale * scaleInc;
        Cannons.mlc.curDmg += dmg;
        Cannons.mlc.curScale = Cannons.mlc.scale * scaleInc;
        Cannons.cn.curScale = Cannons.cn.scale * scaleInc;
        Cannons.mcn.curScale = Cannons.mcn.scale * scaleInc;
        Projs.cBall.curDmg += dmg;
        Projs.cBall.SetNewStats();
        Cannons.SetNewStatsAll();
    }
    public void BiggerBombs(bool plus = false)
    {
        // Bombs and all Explosions grow by 50%. Explosions deal 1 extra damage
        // Plus+ -- Growth is by 100%. 2 extra damaeg instead of 1
        Bombs.curScale = Bombs.scale * (plus ? 2 : 1.5f);
        Bombs.SetNewStats();
        Explosions.curScale = Explosions.scale * (plus ? 2 : 1.5f);
        Explosions.curDmg += Explosions.dmg + (plus ? 2 : 1);
        Explosions.SetNewStats();
    }
    public void GiveMeLibertyGiveMeFire(bool plus = false)
    {
        // All fire is 100% bigger and deals 2 extra damage
        // Plus+ -- fire gets 120% bigger and fire spreads cause 2 extra flames
        Fire.curScale *= plus ? 2.2f : 2f;
        Fire.curDmg += 2;
        Projs.fb.thirdSpread = plus;
        Fire.SetNewStats();
        Projs.fb.SetNewStats();
    }
    public void TankierTanks(bool plus = false)
    {
        // All enemies have an extra 3 hp.
        // Plus+ -- 5 hp instead of 3
        Droids.rd.curHp += plus ? 5 : 3;
        Droids.sh.curHp += plus ? 5 : 3;
        Droids.gd.curHp += plus ? 5 : 3;
        Droids.dr.curHp += plus ? 5 : 3;
        Droids.fd.curHp += plus ? 5 : 3;
        Droids.SetNewStatsAll(new HashSet<string>() { "SH","DR","GR","RD", "FD"});
    }
    public void HastyRockets(bool plus = false)
    {
        // HeetSeeking rockets move and turn faster;
        Projs.rk.curSpeed = plus ? 25 : 20;
        Projs.rk.curTurnSpeed = plus ? 270 : 220;
        Projs.rk.SetNewStats();
    }
    public void AmberAlert(bool plus = false)
    {
        //Storm head get 20% bigger, 50% faster, and deal 1 extra damage
        //Plus+ -- 30%, 100% faster, 2 extra damage
        float speed = Droids.sh.speed;
        Droids.sh.curSpeed = plus ? speed * 2f : speed * 1.5f;
        Droids.sh.curScale = Droids.sh.scale * (plus ? 1.3f : 1.2f);
        Droids.sh.curDmg += plus ? 2 : 1;
        Droids.SetNewStatsAll(new HashSet<string>() { "SH" });
    }
    public void FootSoldier(bool plus = false)
    {
        // Ground Droid has 2 more HP and shoots 30% faster
        // 4 more HP, 50% faster
        float newCD = Droids.gd.cooldown * (plus ? .50f : .70f);
        Droids.gd.curCD = newCD;
        Droids.gd.curHp += plus ? 4 : 2;
        Droids.SetNewStatsAll(new HashSet<string> { "GD" });
    }
    public void WrathOfTheGods(bool plus = false)
    {
        // Heavens Furies get 30% bigger and deal 1 extra damage
        // Plus+ 55% bigger and they apply fire
        Furies.curScale = Furies.scale * (plus ? 1.55f : 1.30f);
        Furies.curDmg += 1;
        if (plus) Furies.curEffect = 2;
        Furies.SetNewStats();
    }
    public void SlowerThanASpeedingBullet(bool plus = false)
    {
        // Bullets and Cannon Fire are 50% faster
        // Plus+ -- 100%: Yellow
        Projs.bul.curSpeed = Projs.bul.speed * (plus ? 2 : 1.5f);
        Projs.cBall.curSpeed = Projs.cBall.speed * (plus ? 2 : 1.5f);
        Projs.SetNewStatsAll(new HashSet<string> { "CBALL", "BUL"});
    }
    public void MakeLoveNotWaste(bool plus = false)
    {
        // Toxic waste barrels create a toxic gas cloud that applies acidic stacks.
        // It gradually grows bigger over time if not destroyed
        // Plus+ -- base size of Toxic barrel and gas cloud is increased by 25%
        // ^^^^+ -- Gas Cloud grows in size 30% faster 
        Toxic.spread = true;
        if (plus)
        {
            Toxic.curScale *= 1.25f;
            Toxic.curGrowthSpeed *= .7f;
        }
        Toxic.SetNewStats();
    }
    public void NewtonsNewLaw()
    {
        // Elemental Hazards now move
        Saws.active = true;
        Saws.SetNewStatsAll();
    }
    public void PropertyPurgatory(bool plus = false)
    {
        //All projectiles get a (probably) new random properties,
        //potentially from none to having one, and vice-versa.
        // Plus+ -- Projectiles cannont randomize to having no property: Orange
        int min = plus ? 1 : 0;
        Projs.bul.curEffect = UnityEngine.Random.Range(min, 5);
        Projs.rk.curEffect = UnityEngine.Random.Range(min, 5);
        Projs.cBall.curEffect = UnityEngine.Random.Range(min, 5);
        Projs.pb.curEffect = UnityEngine.Random.Range(min, 5);
        Projs.sb.curEffect = UnityEngine.Random.Range(min, 5);
        Projs.fb.curEffect = UnityEngine.Random.Range(min, 5);

        Projs.SetNewStatsAll(new HashSet<string> { "CBALL", "BUL","RK","PB","SB", "FB" });
    }
    public void SchrodingersDamagerNumbers(bool plus = false)
    {
        // All damage from all sources are randomized from 1 - 8, only once way to find out
        // Plus+ -- minimun and maximum range of damage increase by 2
        int minRange = plus ? 3 : 1; int max = plus ? 11 : 9;
        Projs.bul.curDmg = UnityEngine.Random.Range(minRange, max);
        Projs.rk.curDmg = UnityEngine.Random.Range(minRange, max);
        Projs.cBall.curDmg = UnityEngine.Random.Range(minRange, max);
        Projs.pb.curDmg = UnityEngine.Random.Range(minRange, max);
        Projs.sb.curDmg = UnityEngine.Random.Range(minRange, max);
        Projs.fb.curDmg = UnityEngine.Random.Range(minRange, max);
        Projs.SetNewStatsAll(new HashSet<string> { "CBALL", "BUL", "RK","PB","SB", "FB" });
        Explosions.curDmg = UnityEngine.Random.Range(minRange, max);
        Explosions.SetNewStats();
        Furies.curDmg = UnityEngine.Random.Range(minRange, max);
        Furies.SetNewStats();
        Cannons.lc.curDmg = UnityEngine.Random.Range(minRange, max);
        Cannons.mlc.curDmg = UnityEngine.Random.Range(minRange, max);
        Cannons.SetNewStatsAll();
        Saws.ice.curDmg = UnityEngine.Random.Range(minRange, max);
        Saws.elec.curDmg = UnityEngine.Random.Range(minRange, max);
        Saws.fire.curDmg = UnityEngine.Random.Range(minRange, max);
        Saws.SetNewStatsAll();
        Droids.sh.curDmg = UnityEngine.Random.Range(minRange, max);
        Droids.SetNewStatsAll(new HashSet<string>() { "SH" });
    }
    public void AddObject(GameObject add, string type, string classType)
    {
        switch (classType)
        {
            case "Hazard":
                Obstacle o = add.GetComponent<Obstacle>();
                MovingObject wp = add.GetComponent<MovingObject>();
                switch (type)
                {
                    case "ToxicBarrel":
                        GasSpread gas = add.GetComponentInChildren<GasSpread>();
                        AttackHitboxes att = add.GetComponentInChildren<AttackHitboxes>();
                        Toxic.arr.Add((add, gas, att));
                        Toxic.count.Add(add); break;
                    case "Fire": Fire.arr.Add((add, o)); Fire.count.Add(add); break;
                    case "Saws":
                        Saws.elec.arr.Add((add, o,wp)); Saws.elec.count.Add(add);
                        break;
                    case "IceSaws": Saws.ice.arr.Add((add, o,wp)); Saws.ice.count.Add(add); break;
                    case "FireSaws": Saws.fire.arr.Add((add, o,wp)); Saws.fire.count.Add(add); break;
                    case "Bombs":
                        Throwable t2 = add.GetComponent<Throwable>();
                        Bombs.arr.Add((add, o, t2)); Bombs.count.Add(add);
                        break;
                    case "Explosions":
                        Explosions.arr.Add((add, o));
                        break;
                    case "Fury":
                        Furies.arr.Add((add, o)); Furies.count.Add(add);
                        break;
                }
                break;

            case "Cannon":
                Enemy enemy = add.GetComponent<Enemy>();
                MovingObject mO = add.GetComponent<MovingObject>();
                switch (type)
                {
                    case "CN":
                        Cannons.cn.arr.Add((add, enemy)); Cannons.cn.count.Add(add); break;
                    case "MCN":
                        Cannons.mcn.arr.Add((add, mO, enemy)); Cannons.mcn.count.Add(add);  break;
                    case "LC":
                        Cannons.lc.arr.Add((add, enemy)); Cannons.lc.count.Add(add);  break;
                    case "MLC":
                        Cannons.mlc.arr.Add((add, mO, enemy)); Cannons.mlc.count.Add(add);  break;
                }
                break;

            case "Droid":
                Enemy ene = add.GetComponent<Enemy>();
                AttackHitboxes aH = add.GetComponent<AttackHitboxes>();
                MovingObject mO2 = add.GetComponent<MovingObject>();
                AIPath ai = add.GetComponent<AIPath>();
                HealthData HP = add.GetComponent<HealthData>();
                switch (type)
                {
                    case "FD": Droids.fd.arr.Add((add, ene, ai, HP)); Droids.fd.count.Add(add); break;
                    case "SH":
                        Droids.sh.arr.Add((add, aH, mO2, HP)); Droids.sh.count.Add(add); break;
                    case "GD":
                        Droids.gd.arr.Add((add, aH, mO2,HP)); Droids.gd.count.Add(add);  break;
                    case "DR":
                        Droids.dr.arr.Add((add, ene, ai,HP)); Droids.dr.count.Add(add); break;
                    case "RD":
                        Droids.rd.arr.Add((add, ene, ai,HP)); Droids.rd.count.Add(add); break;
                }
                break;

            case "Projectile":
                Projectile pr = add.GetComponent<Projectile>();
                switch (type)
                {
                    case "DET": Projs.det.arr.Add((add, pr)); Projs.det.count.Add(add); break;
                    case "RK": Projs.rk.arr.Add((add, pr, add.GetComponent<AIPath>())); Projs.rk.count.Add(add); break;
                    case "BUL": Projs.bul.arr.Add((add, pr)); Projs.bul.count.Add(add);  break;
                    case "PB": Projs.pb.arr.Add((add, pr)); Projs.pb.count.Add(add);  break;
                    case "SB": Projs.sb.arr.Add((add, pr)); Projs.sb.count.Add(add);  break;
                    case "FB": Projs.fb.arr.Add((add, pr, add.GetComponent<FireSpread>())); Projs.fb.count.Add(add); break;
                    case "CBALL": Projs.cBall.arr.Add((add, pr)); Projs.cBall.count.Add(add);  break;
                }
                break;
            
        }
    }

    [System.Serializable]
    public struct _Toxic
    {
        public List<(GameObject obj, GasSpread gas, AttackHitboxes en)> arr;
        public List<GameObject> count;
        public float cooldown, curCD,growthSpeed, curGrowthSpeed;
        public int effect, curEffect;
        public int dmg, curDmg;
        public Vector2 scale, curScale;
        public bool spread;
        public _Toxic(int i)
        {
            arr = new(); count = new();
            spread = false;
            cooldown = curCD = 4.5f;
            growthSpeed = curGrowthSpeed = 2.2f;
            effect = curEffect = 4;
            dmg = curDmg = 2;
            scale = curScale = new Vector2(1, 1);
        }
        public void SetNewStats()
        {
            foreach (var barrel in arr)
            {
                barrel.en.cooldown = curCD;
                barrel.gas.growthTimer = curGrowthSpeed;
                barrel.en.effect = curEffect;
                barrel.gas.enabled = spread;
                barrel.gas.effect = curEffect;
                barrel.en.dmg = curDmg;
                barrel.obj.transform.localScale = curScale;
            }
        }
    }
    
    [System.Serializable]
    public struct _Bombs
    {
        public List<(GameObject obj, Obstacle obs, Throwable thr)> arr;
        public List<GameObject> count;
        public float lifeTime, curLifeTime;
        public Vector2 scale, curScale;

        public _Bombs(int i)
        {
            arr = new();
            count = new();
            lifeTime = curLifeTime = 20f;
            scale = curScale = new Vector2(0.4f,0.4f);
        }

        public void SetNewStats()
        {
            foreach (var bomb in arr)
            {
                bomb.obs.lifeTimer = curLifeTime;
                bomb.obj.transform.localScale = curScale;
            }
        }
    }
    [System.Serializable]
    public struct _Saws
    {
        public Ice ice;
        public Fire fire;
        public Electric elec;
        public bool active;
        public void SetNewStatsAll()
        {
            foreach (var obj in ice.arr){
                obj.obs.dmg = ice.curDmg;
                obj.obj.transform.localScale = ice.curScale;
                obj.wp.enabled = active;
            }
            foreach (var obj in fire.arr){
                obj.obs.dmg = fire.curDmg;
                obj.obj.transform.localScale = fire.curScale;
                obj.wp.enabled = active;
            }
            foreach (var obj in elec.arr){
                obj.obs.dmg = elec.curDmg;
                obj.obj.transform.localScale = elec.curScale;
                obj.wp.enabled = active;
            }
        }
        public _Saws(int i)
        {
            active = false;
            ice = new Ice(i);
            fire = new Fire(i);
            elec = new Electric(i);
        }
        public struct Ice
        {
            public List<(GameObject obj, Obstacle obs, MovingObject wp)> arr;
            public List<GameObject> count;
            public int dmg, curDmg;
            public Vector2 scale, curScale;
            public Ice(int i)
            {
                arr = new(); count = new();
                dmg = curDmg = 1;
                scale = curScale = new Vector2(9, 9);
            }
        }
        public struct Fire
        {
            public List<(GameObject obj, Obstacle obs, MovingObject wp)> arr;
            public List<GameObject> count;
            public int dmg, curDmg;
            public Vector2 scale, curScale;
            public Fire(int i)
            {
                arr = new(); count = new();
                dmg = curDmg = 1;
                scale = curScale = new Vector2(20, 20);
            }
        }
        public struct Electric
        {
            public List<(GameObject obj, Obstacle obs, MovingObject wp)> arr;
            public List<GameObject> count;
            public int dmg, curDmg;
            public Vector2 scale, curScale;
            public Electric(int i)
            {
                arr = new(); count = new();
                dmg = curDmg = 1;
                scale = curScale = new Vector2(12, 12);
            }
        }
    }
    [System.Serializable]
    public struct _Flames
    {
        public List<(GameObject obj, Obstacle obs)> arr;
        public List<GameObject> count;
        public int dmg, curDmg;
        public float lifeTime, curLifeTime;
        public Vector2 scale, curScale;
        public _Flames(int i)
        {
            arr = new();
            count = new();
            dmg = curDmg = 2;
            lifeTime = curLifeTime = 3f;
            scale = curScale = new Vector2(6f, 6f);
        }

        public void SetNewStats()
        {
            foreach (var flame in arr)
            {
                flame.obs.dmg = curDmg;
                flame.obs.lifeTimer = curLifeTime;
                flame.obj.transform.localScale = curScale;
            }
        }
    }
    [System.Serializable]
    public struct HeavensFuries
    {
        public List<(GameObject obj, Obstacle obs)> arr;
        public List<GameObject> count;
        public int dmg, curDmg;
        public int effect, curEffect;
        public Vector2 scale, curScale;

        public HeavensFuries(int i)
        {
            arr = new(); count = new();
            dmg = curDmg = 4; 
            effect = curEffect = 0;
            scale = curScale = new Vector2(1.3f, .4f);
        }
        public void SetNewStats()
        {
            foreach (var obj in arr)
            {
                obj.obs.dmg = curDmg;
                obj.obj.transform.localScale = curScale;
                obj.obs.effect = curEffect;
            }
        }
        public void SetBaseStats()
        {
            foreach (var obj in arr)
            {
                obj.obs.dmg = dmg;
                obj.obj.transform.localScale = scale;
                obj.obs.effect = effect;
            }
        }
    }

    [System.Serializable]
    public struct _Explosions {
        public List<(GameObject obj, Obstacle obs)> arr;
        public List<GameObject> count;
        public int dmg, curDmg;
        public Vector2 scale, curScale;

        public _Explosions(int i)
        {
            arr = new();
            count = new();
            dmg = curDmg = 4;
            scale = curScale = new Vector2(0.4f, 0.4f);
        }

        public void SetNewStats()
        {
            foreach (var explosion in arr)
            {
                explosion.obs.dmg = curDmg;
                explosion.obj.transform.localScale = curScale;
            }
        }
        public void SetBaseStats()
        {
            foreach(var explosion in arr)
            {
                explosion.obs.dmg = dmg;
                explosion.obj.transform.localScale = scale;
            }
            curDmg = dmg;
            curScale = scale;
        }
    }
    [System.Serializable]
    public struct _Cannons
    {
        public CN cn;
        public LC lc;
        public MCN mcn;
        public MLC mlc;

        public void SetNewStatsAll()
        {
            cn.SetNewStats();
            lc.SetNewStats();
            mcn.SetNewStats();
            mlc.SetNewStats();
        }
        public _Cannons(int i)
        {
            cn = new CN(1);
            lc = new LC(1);
            mcn = new MCN(1);
            mlc = new MLC(1);
        }
        [System.Serializable]
        public struct CN {
            public List<(GameObject obj, Enemy en)> arr;
            public List<GameObject> count;
            public Vector2 scale, curScale;
            public float cooldown, curCD;

            public CN(int i)
            {
                scale = curScale = new Vector2(5.5f, 5.5f);
                cooldown = curCD = 3;
                count = new(); arr = new();
            }
            public void SetNewStats()
            {
                foreach (var obj in arr)
                {
                    obj.obj.transform.localScale = curScale;
                    obj.en.cooldown = curCD;
                }
            }
            public void SetBaseStats()
            {
                foreach (var obj in arr)
                {
                    obj.obj.transform.localScale = scale;
                    obj.en.cooldown = cooldown;
                }
            }
        }
        [System.Serializable]
        public struct MCN
        {
            public List<(GameObject obj, MovingObject wp, Enemy en)> arr;
            public List<GameObject> count;
            public float speed, curSpeed;
            public float cooldown, curCD;
            public Vector2 scale, curScale;

            public MCN(int i)
            {
                scale = curScale = new Vector2(5.5f, 5.5f);
                cooldown = curCD = 3;
                speed = curSpeed = 8;
                count = new(); arr = new();
            }
            public void SetNewStats()
            {
                foreach (var obj in arr)
                {
                    obj.wp.speed = curSpeed;
                    obj.obj.transform.localScale = curScale;
                    obj.en.cooldown = curCD;
                }
            }
            public void SetBaseStats()
            {
                foreach (var obj in arr)
                {
                    obj.wp.speed = speed;
                    obj.obj.transform.localScale = scale;
                    obj.en.cooldown = cooldown;
                }
            }
        }
        [System.Serializable]
        public struct LC {
            public List<(GameObject obj, Enemy en)> arr;
            public List<GameObject> count;
            public float laserTime, curLaserTime;
            public Vector2 scale, curScale;
            public float cooldown, curCD;
            public int dmg, curDmg;

            public LC(int i)
            {
                scale = curScale = new Vector2(.6f, .6f);
                cooldown = curCD = 3;
                count = new(); arr = new();
                laserTime = curLaserTime = 2.5f;
                dmg = curDmg = 2;
            }
            public void SetNewStats()
            {
                foreach (var obj in arr)
                {
                    obj.obj.transform.localScale = curScale;
                    obj.en.laserTime = curLaserTime;
                    obj.en.cooldown = curCD;
                    obj.en.contactDmg = curDmg;
                }
            }
            public void SetBaseStats()
            {
                foreach (var obj in arr)
                {
                    obj.obj.transform.localScale = scale;
                    obj.en.laserTime = laserTime;
                    obj.en.cooldown = cooldown;
                    obj.en.contactDmg = dmg;
                }
            }
        }
        [System.Serializable]
        public struct MLC
        {
            public List<(GameObject obj, MovingObject wp, Enemy en)> arr;
            public List<GameObject> count;
            public float speed, curSpeed;
            public float cooldown, curCD;
            public float laserTime, curLaserTime;
            public Vector2 scale, curScale;
            public int dmg, curDmg;
            public MLC(int i)
            {
                scale = curScale = new Vector2(.6f, .6f);
                cooldown = curCD = speed = curSpeed = 3;
                count = new(); arr = new();
                dmg = curDmg = 2;
                laserTime = curLaserTime = 2.5f;
            }
            public void SetNewStats()
            {
                foreach (var obj in arr)
                {
                    obj.wp.speed = curSpeed;
                    obj.obj.transform.localScale = curScale;
                    obj.en.laserTime = curLaserTime;
                    obj.en.cooldown = curCD;
                    obj.en.contactDmg = curDmg;
                }
            }
            public void SetBaseStats()
            {
                foreach (var obj in arr)
                {
                    obj.wp.speed = speed;
                    obj.obj.transform.localScale = scale;
                    obj.en.laserTime = laserTime;
                    obj.en.cooldown = cooldown;
                    obj.en.contactDmg = dmg;
                }
            }
        }
        
    }

    [System.Serializable]
    public struct DROIDS
    {
        public RD rd;
        public DR dr;
        public GD gd;
        public SH sh;
        public FD fd;
        public DROIDS(int i)
        {
            rd = new RD(1);
            dr = new DR(1);
            gd = new GD(1);
            sh = new SH(1);
            fd = new FD(1);
        }
        public void SetNewStatsAll(HashSet<string> change)
        {
            if (change.Contains("RD")){
                foreach (var obj in rd.arr)
                {
                    obj.ai.maxSpeed = rd.curSpeed;
                    obj.HP.maxHp = rd.curHp;
                    obj.en.cooldown = rd.curCD;
                    obj.obj.transform.localScale = rd.curScale;
                }
                print("Set new stats for ROCKET DROID");
            }
            if (change.Contains("FD"))
            {
                foreach (var obj in fd.arr)
                {
                    obj.ai.maxSpeed = fd.curSpeed;
                    obj.HP.maxHp = fd.curHp;
                    obj.en.cooldown = fd.curCD;
                    obj.obj.transform.localScale = fd.curScale;
                }
                print("Set new stats for FIRE DROID");
            }
            if (change.Contains("DR"))
            {
                foreach (var obj in dr.arr)
                {
                    obj.ai.maxSpeed = dr.curSpeed;
                    obj.HP.maxHp = dr.curHp;
                    obj.en.cooldown = dr.curCD;
                    obj.obj.transform.localScale = dr.curScale;
                }
                print("Set new stats for DROID");
            }
            if (change.Contains("SH"))
            {
                foreach (var obj in sh.arr)
                {
                    obj.wp.speed = sh.curSpeed;
                    obj.HP.maxHp = sh.curHp;
                    obj.en.dmg = sh.curDmg;
                    obj.en.cooldown = sh.curCD;
                    obj.obj.transform.localScale = sh.curScale;
                }
                print("Set new stats for STORM HEAD");
            }
            if (change.Contains("GR"))
            {
                foreach (var obj in gd.arr)
                {
                    obj.wp.speed = gd.curSpeed;
                    obj.HP.maxHp = gd.curHp;
                    obj.en.cooldown = gd.curCD;
                    obj.obj.transform.localScale = gd.curScale;
                }
                print("Set new stats for GROUND DROID");
            }
        }

        [System.Serializable]
        public struct FD
        {
            public List<(GameObject obj, Enemy en, AIPath ai, HealthData HP)> arr;
            public List<GameObject> count;
            public float cooldown, curCD;
            public float speed, curSpeed;
            public int hp, curHp;
            public Vector2 scale, curScale;

            public FD(int i)
            {
                scale = curScale = new Vector2(9, 9);
                hp = curHp = 6;
                cooldown = curCD = 6.5f;
                count = new(); arr = new();
                speed = curSpeed = 2.5f;
            }
        }
        [System.Serializable]
        public struct RD
        {
            public List<(GameObject obj, Enemy en, AIPath ai, HealthData HP)> arr;
            public List<GameObject> count;
            public float cooldown, curCD;
            public float speed, curSpeed;
            public int hp, curHp;
            public Vector2 scale, curScale;

            public RD(int i)
            {
                scale = curScale = new Vector2(9, 9);
                hp = curHp = 6;
                cooldown = curCD = 6.5f;
                count = new(); arr = new();
                speed = curSpeed = 2.5f;
            }
        }
        [System.Serializable]
        public struct DR
        {
            public List<(GameObject obj, Enemy en, AIPath ai, HealthData HP)> arr;
            public List<GameObject> count;
            public int cooldown, curCD;
            public float speed, curSpeed;
            public int hp, curHp;
            public Vector2 scale, curScale;
            public DR(int i)
            {
                scale = curScale = new Vector2(10, 10);
                cooldown = curCD = 3;
                hp = curHp = 5;
                count = new(); arr = new();
                speed = curSpeed = 5;
            }
        }
        [System.Serializable]
        public struct SH
        {
            public List<(GameObject obj, AttackHitboxes en, MovingObject wp, HealthData HP)> arr;
            public List<GameObject> count;
            public float cooldown, curCD;
            public float speed, curSpeed;
            public int dmg, curDmg;
            public Vector2 scale, curScale;
            public int hp, curHp;
            public SH(int i)
            {
                scale = curScale = new Vector2(15, 15);
                cooldown = curCD = 5;
                dmg = curDmg = 2;
                hp = curHp = 7;
                count = new(); arr = new();
                speed = curSpeed = 8;
            }
        }
        [System.Serializable]
        public struct GD
        {
            public List<(GameObject obj, AttackHitboxes en, MovingObject wp, HealthData HP)> arr;
            public List<GameObject> count;
            public float cooldown, curCD;
            public float speed, curSpeed;
            public Vector2 scale, curScale;
            public int hp, curHp;

            public GD(int i)
            {
                scale = curScale = new Vector2(15f, 15f);
                hp = curHp = 4;
                cooldown = curCD = 3;
                count = new(); arr = new();
                speed = curSpeed = 3;
            }
        }
    }
    public struct Projectiles
    {
        public DET det;
        public PB pb;
        public SB sb;
        public BUL bul;
        public CBALL cBall;
        public RK rk;
        public FB fb;
        public void SetNewStatsAll(HashSet<string> change)
        {
            if (change.Contains("DET")) det.SetNewStats();
            if (change.Contains("PB")) pb.SetNewStats();
            if (change.Contains("SB")) sb.SetNewStats();
            if (change.Contains("BUL")) bul.SetNewStats();
            if (change.Contains("RK")) rk.SetNewStats();
            if (change.Contains("CBALL")) cBall.SetNewStats();
            if (change.Contains("FB")) fb.SetNewStats();
        }
        public void SetBaseStatsAll()
        {
            det.SetBaseStats();
            pb.SetBaseStats();
            sb.SetBaseStats();
            bul.SetBaseStats();
            rk.SetBaseStats();
            cBall.SetBaseStats();
        }
        public Projectiles(int i)
        {
            det = new DET(1);
            pb = new PB(1);
            sb = new SB(1);
            fb = new FB(1);
            bul = new BUL(1);
            cBall = new CBALL(1);
            rk = new RK(1);
        }
        [System.Serializable]
        public struct DET
        {
            public List<(GameObject obj, Projectile proj)> arr;
            public List<GameObject> count;
            public float speed, curSpeed;
            public Vector2 scale, curScale;
            public float dist, curDist;
            public int effect, curEffect;
            public DET(int i)
            {
                scale = curScale = new Vector2(9f, 9f);
                count = new(); arr = new();
                effect = curEffect = 0;
                speed = curSpeed = 9;
                dist = curDist = 15;
            }
            public void SetNewStats()
            {
                foreach (var obj in arr)
                {
                    obj.proj.speed = curSpeed;
                    obj.proj.distance = curDist;
                    obj.obj.transform.localScale = curScale;
                    obj.proj.effect = curEffect;
                }
            }
            public void SetBaseStats()
            {
                foreach (var obj in arr)
                {
                    obj.proj.speed = speed;
                    obj.proj.distance = dist;
                    obj.obj.transform.localScale = scale;
                    obj.proj.effect = effect;
                }
            }
        }
        [System.Serializable]
        public struct PB
        {
            public List<(GameObject obj, Projectile proj)> arr;
            public List<GameObject> count;
            public int dmg, curDmg;
            public int effect, curEffect;
            public float speed, curSpeed;
            public Vector2 scale, curScale;

            public PB(int i)
            {
                scale = curScale = new Vector2(11f, 11f);
                count = new(); arr = new();
                effect = curEffect = 0;
                speed = curSpeed = 25;
                dmg = curDmg = 2;
            }
            public void SetNewStats()
            {
                foreach (var obj in arr)
                {
                    obj.proj.dmg = curDmg;
                    obj.proj.speed = curSpeed;
                    obj.obj.transform.localScale = curScale;
                    obj.proj.effect = curEffect;
                }
            }
            public void SetBaseStats()
            {
                foreach (var obj in arr)
                {
                    obj.proj.dmg = dmg;
                    obj.proj.speed = speed;
                    obj.obj.transform.localScale = scale;
                    obj.proj.effect = effect;
                }
            }
        }
        [System.Serializable]
        public struct SB
        {
            public List<(GameObject obj, Projectile proj)> arr;
            public List<GameObject> count;
            public int dmg, curDmg;
            public float speed, curSpeed;
            public int effect, curEffect;
            public Vector2 scale, curScale;

            public SB(int i)
            {
                scale = curScale = new Vector2(5f, 5f);
                count = new(); arr = new();
                speed = curSpeed = 21;
                effect = curEffect = 0;
                dmg = curDmg = 1;
            }
            public void SetNewStats()
            {
                foreach (var obj in arr)
                {
                    obj.proj.dmg = curDmg;
                    obj.proj.speed = curSpeed;
                    obj.obj.transform.localScale = curScale;
                    obj.proj.effect = curEffect;
                }
            }
            public void SetBaseStats()
            {
                foreach (var obj in arr)
                {
                    obj.proj.dmg = dmg;
                    obj.proj.speed = speed;
                    obj.obj.transform.localScale = scale;
                    obj.proj.effect = effect;
                }
            }
        }
        [System.Serializable]
        public struct BUL
        {
            public List<(GameObject obj, Projectile proj)> arr;
            public List<GameObject> count;
            public int dmg, curDmg;
            public float speed, curSpeed;
            public int effect, curEffect;
            public Vector2 scale, curScale;
            public BUL(int i)
            {
                scale = curScale = new Vector2(11f, 11f);
                count = new(); arr = new();
                speed = curSpeed = 15;
                effect = curEffect = 0;
                dmg = curDmg = 1;
            }
            public void SetNewStats()
            {
                foreach (var obj in arr)
                {
                    obj.proj.dmg = curDmg;
                    obj.proj.speed = curSpeed;
                    obj.obj.transform.localScale = curScale;
                    obj.proj.effect = curEffect;
                }
            }
            public void SetBaseStats()
            {
                foreach (var obj in arr)
                {
                    obj.proj.dmg = dmg;
                    obj.proj.speed = speed;
                    obj.obj.transform.localScale = scale;
                    obj.proj.effect = effect;
                }
            }
        }
        [System.Serializable]
        public struct RK
        {
            public List<(GameObject obj, Projectile proj, AIPath ai)> arr;
            public List<GameObject> count;
            public int dmg, curDmg;
            public float speed, curSpeed;
            public float turnSpeed, curTurnSpeed;
            public int effect, curEffect;
            public Vector2 scale, curScale;

            public RK(int i)
            {
                scale = curScale = new Vector2(15f, 15f);
                count = new(); arr = new();
                turnSpeed = curTurnSpeed = 160f;
                speed = curSpeed = 15;
                effect = curEffect = 2;
                dmg = curDmg = 2;
            }
            public void SetNewStats()
            {
                foreach (var obj in arr)
                {
                    obj.proj.dmg = curDmg;
                    obj.ai.maxSpeed = curSpeed;
                    obj.obj.transform.localScale = curScale;
                    obj.proj.effect = curEffect;
                }
            }
            public void SetBaseStats()
            {
                foreach (var obj in arr)
                {
                    obj.proj.dmg = dmg;
                    obj.ai.maxSpeed = speed;
                    obj.obj.transform.localScale = scale;
                    obj.proj.effect = effect;
                }
            }
        }
        [System.Serializable]
        public struct CBALL
        {
            public List<(GameObject obj, Projectile proj)> arr;
            public List<GameObject> count;
            public int dmg, curDmg;
            public float speed, curSpeed;
            public int effect, curEffect;
            public Vector2 scale, curScale;

            public CBALL(int i)
            {
                scale = curScale = new Vector2(.4f, .4f);
                count = new(); arr = new();
                effect = curEffect = 0;
                speed = curSpeed = 7;
                dmg = curDmg = 2;
            }
            public void SetNewStats()
            {
                foreach (var obj in arr)
                {
                    obj.proj.dmg = curDmg;
                    obj.proj.speed = curSpeed;
                    obj.obj.transform.localScale = curScale;
                    obj.proj.effect = curEffect;
                }
            }
            public void SetBaseStats()
            {
                foreach (var obj in arr)
                {
                    obj.proj.dmg = dmg;
                    obj.proj.speed = speed;
                    obj.obj.transform.localScale = scale;
                    obj.proj.effect = effect;
                }
            }
        }
        [System.Serializable]
        public struct FB
        {
            public List<(GameObject obj, Projectile proj, FireSpread spr)> arr;
            public List<GameObject> count;
            public int dmg, curDmg;
            public float speed, curSpeed;
            public int effect, curEffect;
            public Vector2 scale, curScale;
            public bool thirdSpread;
            public FB(int i)
            {
                scale = curScale = new Vector2(10f, 10f);
                count = new(); arr = new();
                effect = curEffect = 2;
                thirdSpread = false;
                speed = curSpeed = 16;
                dmg = curDmg = 3;
            }
            public void SetNewStats()
            {
                foreach (var obj in arr)
                {
                    obj.proj.dmg = curDmg;
                    obj.proj.speed = curSpeed;
                    obj.obj.transform.localScale = curScale;
                    obj.proj.effect = curEffect;
                    obj.spr.thirdSpread = thirdSpread;
                }
            }
            public void SetBaseStats()
            {
                foreach (var obj in arr)
                {
                    obj.proj.dmg = dmg;
                    obj.proj.speed = speed;
                    obj.obj.transform.localScale = scale;
                    obj.proj.effect = effect;
                    obj.spr.thirdSpread = thirdSpread;
                }
            }
        }
    }
}
