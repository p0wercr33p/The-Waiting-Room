using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public static Manager Ins;
    Player player;
    ObjectBank bank;
    ObjectManager Jim;
    public GameObject[] cannonPicker;
    Dictionary<GameObject, Enemy> enemyDict;
    Dictionary<GameObject, Obstacle> obsDict;
    [HideInInspector]
    public GameObject[] hazards, warnings, headsUps, items, cannons, enemies, cannonSigns, enemySigns;
    public GameObject warning, headsUp, cannonSign, enemySign;
    public Vector2[] hazardPoints, itemPoints, enemyPoints; 
    public CannonData[] cannonData;
    int levelHazards, levelItems, levelCannons, levelEnemies;
    public bool[] draw;

    private void Awake()
    {
        if (Ins == null) Ins = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        Jim = GetComponent<ObjectManager>();
        StartLevel();
        player = Player.Ins;
    }
    void StartLevel()
    {
        enemyDict = new();
        obsDict = new();
        bank = GetComponent<ObjectBank>();
        levelHazards = hazardPoints.Length;
        levelItems = itemPoints.Length;
        levelCannons = cannonData.Length;
        levelEnemies = enemyPoints.Length;
        cannonSigns = new GameObject[levelCannons];
        enemySigns = new GameObject[levelEnemies];
        enemies = new GameObject[levelEnemies];
        warnings = new GameObject[levelHazards];
        hazards = new GameObject[levelHazards];
        items = new GameObject[levelItems];
        headsUps = new GameObject[levelItems];
        cannons = new GameObject[levelCannons];
        int c = cannonPicker.Length;

        for (int i = 0; i < levelHazards; i++)
        {
            GameObject w = Instantiate(warning, this.transform);
            w.transform.position = hazardPoints[i];
            warnings[i] = w;
            w.SetActive(false);
            (GameObject make, string type) = bank.MakeHazard();
            GameObject h = Instantiate(make, this.transform);
            h.transform.position = hazardPoints[i];
            hazards[i] = h;
            Obstacle obs = h.GetComponent<Obstacle>();
            if (obs != null)
            {
                obsDict[h] = h.GetComponent<Obstacle>();
                obsDict[h].index = i;
            }  
            Jim.AddObject(h, type, "Hazard");
            h.SetActive(false);
        }
        for (int i = 0; i < levelItems; i++)
        {
            GameObject h = Instantiate(headsUp, transform);
            h.transform.position = itemPoints[i];
            headsUps[i] = h;
            h.SetActive(false); int ran = Random.Range(0, bank.lvlItems.Length);
            GameObject it = Instantiate(bank.lvlItems[ran], transform);
            h.name = $"HeadsUp! {it.name} {i}";
            it.transform.position = itemPoints[i];
            items[i] = it; it.SetActive(false);
        }
        
        for (int i = 0; i < levelCannons; i++)
        {
            GameObject cSign = Instantiate(cannonSign, transform);
            cSign.transform.position = cannonData[i].pos;
            cannonSigns[i] = cSign; cSign.SetActive(false);
            int ran = Random.Range(0, c);
            GameObject cannon = Instantiate(cannonPicker[ran], transform);
            cannons[i] = cannon;
            enemyDict[cannon] = cannon.GetComponent<Enemy>();
            cannon.transform.position = cannonData[i].pos;
            switch (cannonData[i].shootDir.ToLower())
            {
                case "up":
                    enemyDict[cannon].shootDir = Vector2.up;
                    break;
                case "down":
                    enemyDict[cannon].shootDir = Vector2.down;
                    break;
                case "left":
                    enemyDict[cannon].shootDir = Vector2.left;
                    break;
                case "right":
                    enemyDict[cannon].shootDir = Vector2.right;
                    break;
                default:
                    Debug.LogWarning("Unknown direction, defaulting to right");
                    enemyDict[cannon].shootDir = Vector2.right;
                    break;
            }
            cannon.SetActive(false);
            Jim.AddObject(cannon, enemyDict[cannon].typeName, "Cannon");
        }

        for (int i = 0; i < levelEnemies; i++)
        {
            GameObject eSign = Instantiate(enemySign, transform);
            eSign.transform.position = enemyPoints[i];
            enemySigns[i] = eSign; eSign.SetActive(false);

            int ran = Random.Range(0, bank.lvlEnemies.Length);
            GameObject e = Instantiate(bank.lvlEnemies[ran], transform);
            e.transform.position = enemyPoints[i];
            enemies[i] = e;
            e.SetActive(false);
            Jim.AddObject(e, bank.enemyNames[ran], "Droid");
        }
    }
    (GameObject, GameObject, int) FindHazard(int stI)
    {
        for (int i = stI; i < levelHazards; i++)
            if (!hazards[i].activeInHierarchy && !warnings[i].activeInHierarchy)
                return (hazards[i], warnings[i],i);

        for (int i = stI-1; i >= 0; i--)
            if (!hazards[i].activeInHierarchy && !warnings[i].activeInHierarchy)
                return (hazards[i], warnings[i],i);

        return (null, null,0);
    }
    IEnumerator SpawnHazard()
    {
        int stI = Random.Range(0, hazards.Length);
        (GameObject h, GameObject w,int i) = FindHazard(stI);

        if (h != null)
        {
            w.SetActive(true);
            yield return new WaitForSeconds(2.4f);
            w.SetActive(false);
            h.SetActive(true);
        }

    }
    (GameObject, GameObject,int) FindItems(int stI)
    {
        for (int i = stI; i < levelItems; i++)
            if (!items[i].activeInHierarchy && !headsUps[i].activeInHierarchy)
                return (items[i], headsUps[i], i);

        for (int i = stI-1; i >= 0; i--)
            if (!items[i].activeInHierarchy && !headsUps[i].activeInHierarchy)
                return (items[i], headsUps[i], i);

        return (null, null, 0);
    }
    IEnumerator SpawnItems()
    {
        int stI = Random.Range(0, items.Length);
        (GameObject it, GameObject hU, int i) = FindItems(stI);

        if (hU != null)
        {
            hU.SetActive(true);
            yield return new WaitForSeconds(2.4f);
            hU.SetActive(false);
            it.SetActive(true);
        }
    }
    (GameObject, GameObject, int) FindEnemy(int stI)
    {
        for (int i = stI; i < levelItems; i++)
            if (!enemies[i].activeInHierarchy && !enemySigns[i].activeInHierarchy)
                return (enemies[i], enemySigns[i], i);

        for (int i = stI - 1; i >= 0; i--)
            if (!enemies[i].activeInHierarchy && !enemySigns[i].activeInHierarchy)
                return (enemies[i], enemySigns[i], i);

        return (null, null, 0);
    }
    IEnumerator SpawnEnemy()
    {
        int stI = Random.Range(0, enemies.Length);
        (GameObject en, GameObject eS, int i) = FindEnemy(stI);

        if (en != null)
        {
            en.transform.position = enemyPoints[i];
            eS.SetActive(true);
            yield return new WaitForSeconds(2.4f);
            eS.SetActive(false);
            en.SetActive(true);
        }
    }
    (GameObject, int) FindHazardToSwapWith(int stI, int swapInd)
    {
        for (int i = stI; i < levelHazards; i++)
            if (!hazards[i].activeInHierarchy && !warnings[i].activeInHierarchy)
            {
                print($"found target {hazards[i].name} at index {i} (their index ->) {obsDict[hazards[i]].index} {hazards[i].activeSelf}");
                return (hazards[i], i);
            }

        for (int i = stI - 1; i >= 0; i--)
            if (!hazards[i].activeInHierarchy && !warnings[i].activeInHierarchy)
            {
                print($"found target {hazards[i].name} at index {i} (their index ->) {obsDict[hazards[i]].index} {hazards[i].activeSelf}");
                return (hazards[i], i);
            }

        
        return (null, 0);
    }
    public void Swap(int ind1)
    {
        int length = hazards.Length;
        if (ind1 < 0 || ind1 > length) return;
        GameObject subj1 = hazards[ind1];
        int stI = Random.Range(0, length);
        (GameObject h, int i) = FindHazardToSwapWith(stI, ind1);

        if (h != null)
        {
            subj1.transform.position = hazardPoints[i];
            h.transform.position = hazardPoints[ind1];
            obsDict[subj1].index = i;
            obsDict[h].index = ind1;
            hazards[i] = subj1;
            hazards[ind1] = h;
        }
    }
    (GameObject, GameObject) FindCannon(int stI)
    {
        for (int i = stI; i < levelCannons; i++)
            if (!cannons[i].activeInHierarchy)
                return (cannons[i], cannonSigns[i]);

        for (int i = stI; i >= 0; i--)
            if (!cannons[i].activeInHierarchy)
                return (cannons[i], cannonSigns[i]);

        return (null, null);
    }
    IEnumerator SpawnCannon()
    {
        int stI = Random.Range(0, cannons.Length);
        (GameObject cannon, GameObject sign) = FindCannon(stI);

        if (cannon != null)
        {
            sign.SetActive(true);
            yield return new WaitForSeconds(2.4f);
            sign.SetActive(false);
            cannon.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) StartCoroutine(SpawnHazard());
        if (Input.GetKeyDown(KeyCode.M)) StartCoroutine(SpawnItems());
        if (Input.GetKeyDown(KeyCode.C)) StartCoroutine(SpawnCannon());
        if (Input.GetKeyDown(KeyCode.T)) StartCoroutine(SpawnEnemy());
    }

    public void HandleTimedEvents(int eventI)
    {
        if (eventI == 0) StartCoroutine(SpawnEnemy());
        else if (eventI == 1) StartCoroutine(SpawnHazard());
        else if (eventI == 2) StartCoroutine(SpawnCannon());
    }

    [System.Serializable]
    public struct CannonData
    {
        public Vector2 pos;
        public string shootDir; // Can be "left", "right", "up", or "down"
    }
   
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        for (int i = 0; i < hazardPoints.Length; i++)
        {
            if (!draw[0]) break;
            Gizmos.DrawLine(hazardPoints[i] - Vector2.up * .4f, hazardPoints[i] + Vector2.up * .4f);
            Gizmos.DrawLine(hazardPoints[i] - Vector2.left * .4f, hazardPoints[i] + Vector2.left * .4f);
        }
        Gizmos.color = Color.yellow;
        for (int i = 0; i < itemPoints.Length; i++)
        {
            if (!draw[1]) break;
            Gizmos.DrawLine(itemPoints[i] - Vector2.up * .4f, itemPoints[i] + Vector2.up * .4f);
            Gizmos.DrawLine(itemPoints[i] - Vector2.left * .4f, itemPoints[i] + Vector2.left * .4f);
        }
        Gizmos.color = Color.white;
        for (int i = 0; i < cannonData.Length; i++)
        {
            if (!draw[2]) break;
            Gizmos.DrawLine(cannonData[i].pos - Vector2.up * .4f, cannonData[i].pos + Vector2.up * .4f);
            Gizmos.DrawLine(cannonData[i].pos - Vector2.left * .4f, cannonData[i].pos + Vector2.left * .4f);
        }
        Gizmos.color = Color.red;
        for (int i = 0; i < enemyPoints.Length; i++)
        {
            if (!draw[3]) break;
            Gizmos.DrawLine(enemyPoints[i] - Vector2.up * .4f, enemyPoints[i] + Vector2.up * .4f);
            Gizmos.DrawLine(enemyPoints[i] - Vector2.left * .4f, enemyPoints[i] + Vector2.left * .4f);
        }
    }
}

