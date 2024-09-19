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
    public Enemy enemy;
    public Slider curSlider;
    public GameObject[] cannonPicker;
    Dictionary<GameObject, Enemy> enemyDict;
    Dictionary<GameObject, Obstacle> obsDict;
    Dictionary<GameObject, MovingObject> waypointDict;
    [HideInInspector]
    public GameObject[] hazards, warnings, headsUps, items, cannons;
    public GameObject warning, headsUp;
    public Vector2[] hazardPoints, itemPoints; 
    public CannonData[] cannonData;
    public int levelHazards, levelItems, levelCannons;
    public int lvl;
    public float maxTime, milestoneTime; float timePassed, nextMilestone;
    public bool draw;

    private void Awake()
    {
        if (Ins == null) Ins = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        StartLevel();
        player = Player.Ins;
        nextMilestone = milestoneTime;
        if (curSlider != null)
        {
            InitializeSlider();
        }
    }
    public void InitializeSlider()
    {
        if (curSlider != null)
        {
            curSlider.maxValue = maxTime;
            curSlider.value = 0;
        }
    }
    void StartLevel()
    {
        enemyDict = new();
        waypointDict = new();
        bank = GetComponent<ObjectBank>();
        levelHazards = hazardPoints.Length;
        levelItems = itemPoints.Length;
        obsDict = new Dictionary<GameObject, Obstacle>();
        levelCannons = cannonData.Length;
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
           
            GameObject h = Instantiate(bank.MakeHazard(lvl), this.transform);
            print(h.name);
            h.transform.position = hazardPoints[i];
            hazards[i] = h;
            obsDict[h] = h.GetComponent<Obstacle>();
            obsDict[h].index = i;
            h.SetActive(false);
        }
        for (int i = 0; i < levelItems; i++)
        {
            GameObject h = Instantiate(headsUp, transform);
            h.transform.position = itemPoints[i];
            headsUps[i] = h;
            h.SetActive(false); int ran = Random.Range(0, bank.lvlItems[lvl].Length);
            GameObject it = Instantiate(bank.lvlItems[lvl][ran], transform);
            print(it.name);
            h.name = $"HeadsUp! {it.name} {i}";
            it.transform.position = itemPoints[i];
            items[i] = it; it.SetActive(false);
        }
        
        for (int i = 0; i < levelCannons; i++)
        {
            int ran = Random.Range(0, c);
            GameObject cannon = Instantiate(cannonPicker[ran], transform);
            cannons[i] = cannon;
            enemyDict[cannon] = cannon.GetComponent<Enemy>();
            cannon.transform.position = cannonData[i].pos;
            bool wp = enemyDict[cannon].hasWayPoints;
            if (wp) { waypointDict[cannon] = cannon.GetComponent<MovingObject>();}
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
    IEnumerator SpawnHazards()
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
    GameObject FindCannon(int stI)
    {
        for (int i = stI; i < levelCannons; i++)
            if (!cannons[i].activeInHierarchy)
                return cannons[i];

        for (int i = stI; i >= 0; i--)
            if (!cannons[i].activeInHierarchy)
                return cannons[i];

        return null;
    }
    void SpawnCannon()
    {
        int stI = Random.Range(0, cannons.Length);
        GameObject cannon = FindCannon(stI);

        if (cannon != null)
            cannon.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) player.ChangeGravity(0);
        if (Input.GetKeyDown(KeyCode.O)) player.ChangeGravity(1);
        if (Input.GetKeyDown(KeyCode.I)) player.ChangeGravity(2);
        if (Input.GetKeyDown(KeyCode.U)) player.ChangeGravity(3);
        if (enemy != null && Input.GetKeyDown(KeyCode.L)) enemy.TakeDamage(1);
        if (Input.GetKeyDown(KeyCode.K)) StartCoroutine(SpawnHazards());
        if (Input.GetKeyDown(KeyCode.M)) StartCoroutine(SpawnItems());
        if (Input.GetKeyDown(KeyCode.C)) SpawnCannon();
  
    }
    void HandleTimedEvents()
    {
        if (curSlider == null) return;
     
        if (timePassed <= maxTime)
        {
            timePassed += Time.deltaTime;
            curSlider.value = timePassed;
            if (timePassed >= nextMilestone)
            {
                nextMilestone += milestoneTime;
                int ran = Random.Range(1, 4);
                for (int i = 0; i < ran; i++)
                {
                    int newRan = Random.Range(0, 6);
                    if (newRan >= 4) StartCoroutine(SpawnItems());
                    else StartCoroutine(SpawnHazards());
                }
            }
        }
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
            if (!draw) break;
            Gizmos.DrawLine(hazardPoints[i] - Vector2.up * .4f, hazardPoints[i] + Vector2.up * .4f);
            Gizmos.DrawLine(hazardPoints[i] - Vector2.left * .4f, hazardPoints[i] + Vector2.left * .4f);
        }
        Gizmos.color = Color.yellow;
        for (int i = 0; i < itemPoints.Length; i++)
        {
            if (!draw) break;
            Gizmos.DrawLine(itemPoints[i] - Vector2.up * .4f, itemPoints[i] + Vector2.up * .4f);
            Gizmos.DrawLine(itemPoints[i] - Vector2.left * .4f, itemPoints[i] + Vector2.left * .4f);
        }
        Gizmos.color = Color.white;
        for (int i = 0; i < cannonData.Length; i++)
        {
            if (draw) break;
            Gizmos.DrawLine(cannonData[i].pos - Vector2.up * .4f, cannonData[i].pos + Vector2.up * .4f);
            Gizmos.DrawLine(cannonData[i].pos - Vector2.left * .4f, cannonData[i].pos + Vector2.left * .4f);
        }
    }
}

