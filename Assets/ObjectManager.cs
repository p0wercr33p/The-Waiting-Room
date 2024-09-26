using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager Ins;
    public DROIDS Droids;
    public _Bombs Bombs;
    public _Explosions Explosions;
    public _Saws Saws;
    public _Cannons Cannons;
    public Projectiles Projs;
    private void Awake() {
        Ins = this;
    }

    public void AddObject(GameObject add, string type, string classType)
    {
        switch (classType)
        {
            case "Hazard":

                Obstacle o = add.GetComponent<Obstacle>();
                switch (type)
                {
                    case "Saws":
                        Saws.arr.Add((add, o, o.effect));
                        break;
                    case "Bombs":
                        Throwable t2 = add.GetComponent<Throwable>();
                        Bombs.arr.Add((add, o, t2));
                        break;
                    case "Explosions":
                        Explosions.arr.Add((add, o));
                        break;
                }
                break;

            case "Cannon":
                Enemy enemy = add.GetComponent<Enemy>();
                MovingObject mO = add.GetComponent<MovingObject>();
                switch (type)
                {
                    case "CN":
                        Cannons.cn.arr.Add((add, enemy)); break;
                    case "MCN":
                        Cannons.mcn.arr.Add((add, mO, enemy)); break;
                    case "LC":
                        Cannons.lc.arr.Add((add, enemy)); break;
                    case "MLN":
                        Cannons.mlc.arr.Add((add, mO, enemy)); break;
                }
                break;

            case "Enemy":
                Enemy ene = add.GetComponent<Enemy>();
                MovingObject mO2 = add.GetComponent<MovingObject>();
                AIPath ai = add.GetComponent<AIPath>();
                switch (type)
                {
                    case "SH":
                        Droids.sh.arr.Add((add, ene, mO2)); break;
                    case "GD":
                        Droids.gd.arr.Add((add, ene, mO2)); break;
                    case "DR":
                        Droids.dr.arr.Add((add, ene, ai)); break;
                    case "RD":
                        Droids.rd.arr.Add((add, ene, ai)); break;
                }
                break;

            case "Projectile":
                Projectile pr = add.GetComponent<Projectile>();
                switch (type)
                {
                    case "DET": Projs.det.arr.Add((add, pr)); break;
                    case "RK": Projs.rk.arr.Add((add, pr, add.GetComponent<AIPath>())); break;
                    case "BUL": Projs.bul.arr.Add((add, pr)); break;
                    case "PB": Projs.pb.arr.Add((add, pr)); break;
                    case "SB": Projs.sb.arr.Add((add, pr)); break;
                    case "CBALL": Projs.cBall.arr.Add((add, pr)); break;
                }
                break;
            
        }
    }

    [System.Serializable]
    public struct _Bombs
    {
        public List<(GameObject obj, Obstacle obs, Throwable thr)> arr;
        public float lifeTime, curLifeTime;
        public Vector2 scale, curScale;
    }
    [System.Serializable]
    public struct _Saws
    {
        public List<(GameObject obj, Obstacle obs, int eff)> arr;
        public int dmg, curDmg;
        public Vector2 scale, curScale;
    }

    [System.Serializable]
    public struct _Explosions {
        public List<(GameObject obj, Obstacle obs)> arr;
        public int dmg, curDmg;
        public Vector2 scale, curScale;
    }
    [System.Serializable]
    public struct _Cannons
    {
        public CN cn;
        public LC lc;
        public MCN mcn;
        public MLC mlc;

        [System.Serializable]
        public struct CN {
            public List<(GameObject obj, Enemy en)> arr;
            public Vector2 scale, curScale;
            public float cooldown, curCD;
        }
        [System.Serializable]
        public struct MCN
        {
            public List<(GameObject obj, MovingObject wp, Enemy en)> arr;
            public float speed, curSpeed;
            public float cooldown, curCD;
            public Vector2 scale, curScale;
        }
        [System.Serializable]
        public struct LC {
            public List<(GameObject obj, Enemy en)> arr;
            public float laserTime, curLaserTime;
            public Vector2 scale, curScale;
            public float cooldown, curCD;
        }
        [System.Serializable]
        public struct MLC
        {
            public List<(GameObject obj, MovingObject wp, Enemy en)> arr;
            public float speed, curSpeed;
            public float cooldown, curCD;
            public float laserTime, curLaserTime;
            public Vector2 scale, curScale;
        }
        
    }

    [System.Serializable]
    public struct DROIDS
    {
        public RD rd;
        public DR dr;
        public GD gd;
        public SH sh;
        [System.Serializable]
        public struct RD
        {
            public List<(GameObject obj, Enemy en, AIPath ai)> arr;
            public int cooldown, curCooldown;
            public int speed, curSpeed;
            public Vector2 scale, curScale;
        }
        [System.Serializable]
        public struct DR
        {
            public List<(GameObject obj, Enemy en, AIPath ai)> arr;
            public int cooldown, curCooldown;
            public int speed, curSpeed;
            public Vector2 scale, curScale;
        }
        [System.Serializable]
        public struct SH
        {
            public List<(GameObject obj, Enemy en, MovingObject wp)> arr;
            public int cooldown, curCooldown;
            public int speed, curSpeed;
            public Vector2 scale, curScale;
        }
        [System.Serializable]
        public struct GD
        {
            public List<(GameObject obj, Enemy en, MovingObject wp)> arr;
            public int cooldown, curCooldown;
            public int speed, curSpeed;
            public Vector2 scale, curScale;
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
        [System.Serializable]
        public struct DET
        {
            public List<(GameObject obj, Projectile proj)> arr;
            public int dmg, curDmg;
            public float speed, curSpeed;
            public Vector2 scale, curScale;
            public float dist, curDist;
        }
        [System.Serializable]
        public struct PB
        {
            public List<(GameObject obj, Projectile proj)> arr;
            public int dmg, curDmg;
            public float speed, curSpeed;
            public Vector2 scale, curScale;
        }
        [System.Serializable]
        public struct SB
        {
            public List<(GameObject obj, Projectile proj)> arr;
            public int dmg, curDmg;
            public float speed, curSpeed;
            public Vector2 scale, curScale;
        }
        [System.Serializable]
        public struct BUL
        {
            public List<(GameObject obj, Projectile proj)> arr;
            public int dmg, curDmg;
            public float speed, curSpeed;
            public Vector2 scale, curScale;
        }
        [System.Serializable]
        public struct RK
        {
            public List<(GameObject obj, Projectile proj, AIPath ai)> arr;
            public int dmg, curDmg;
            public float speed, curSpeed;
            public Vector2 scale, curScale;
        }
        [System.Serializable]
        public struct CBALL
        {
            public List<(GameObject obj, Projectile proj)> arr;
            public int dmg, curDmg;
            public float speed, curSpeed;
            public Vector2 scale, curScale;
        }
    }
}
