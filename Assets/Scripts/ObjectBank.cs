using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectBank : MonoBehaviour
{
    [Header("Hazards")]
    public GameObject iceSaws;
    public GameObject saws;
    public GameObject spikes;
    public GameObject heavensFury;
    public GameObject freezeBlast, fire, bombs, fireSaws;

    [Header("Items")]
    public GameObject pBlaster;
    public GameObject sBlaster;
    public GameObject heart, troll;


    [Header("HazardsInfo")]
    public int maxFuries;
    public int maxBombs;
    public int maxSaws;
    public int maxBlasts;
    Dictionary<string, int> hazardCounts;
    Dictionary<string, int> maxHazardCounts;
  
    public Dictionary<GameObject, (MovingObject move, Enemy en)> cannonsInfo;
    public List<(GameObject obj, Obstacle obs, Throwable thr)> bombsInfo;
    public List<(GameObject obj, Obstacle obs)> sawsInfo;
    public List<(GameObject obj, Obstacle obs)> iceSawsInfo;
    public List<(GameObject obj, Obstacle obs)> fireSawsInfo;
    public List<GameObject[]> level;
    public List<GameObject[]> lvlItems;
    string[] hazardTypes;

    GameObject[] allHazards, allItems, allObjs;
    GameObject[] availableHazards;


    void Awake()
    {
        InitializeHazardCounts();
        AddLevelHazardsInfo();
        Trollololo();
        AddLevelItemsInfo();
    }
    void Trollololo()
    {
        allHazards = new GameObject[] { fireSaws, saws, iceSaws, bombs, freezeBlast, heavensFury };
        allItems = new GameObject[] { sBlaster, heart, pBlaster };
        int ran = Random.Range(0, 11);
        if (ran == 9)
        {
            ran = Random.Range(0, allHazards.Length);
            troll = allHazards[ran];
        }
        else
        {
            ran = Random.Range(0, allItems.Length);
            troll = allItems[ran];
        }
    }
    private void InitializeHazardCounts()
    {
        hazardCounts = new Dictionary<string, int>()
        {
            {"Saws", 0 },
            {"Bombs", 0 },
            {"HeavensFury", 0 },
            {"FreezeBlast", 0 }
        };
        maxHazardCounts = new Dictionary<string, int>
        {
            { "Saws", maxSaws },
            { "Bombs", maxBombs },
            { "HeavensFury", maxFuries },
            { "FreezeBlast", maxBlasts }
        };
    }
    void AddLevelHazardsInfo()
    {
        level = new List<GameObject[]>()
        {
            new GameObject[] { bombs },
            new GameObject[] { iceSaws, saws, fireSaws, bombs, heavensFury, freezeBlast },
            new GameObject[] { iceSaws, saws, fireSaws }
        };
        hazardTypes = new string[] { "Saws", "Saws", "Saws", "Bombs", "HeavensFury", "FreezeBlast" };
    }

    public GameObject MakeHazard(int lvl)
    {
        int count = level[lvl].Count();
        int stI = Random.Range(0, count);

        for (int i = stI; i < count; i++)
        {
            string hazardType = hazardTypes[i];
            if (hazardCounts[hazardType] < maxHazardCounts[hazardType])
            {
                hazardCounts[hazardType]++;
                print($"return {level[lvl][i].name}");
                return level[lvl][i];
            }
        }
        for (int i = stI-1; i >= 0; i--)
        {
            string hazardType = hazardTypes[i];
            if (hazardCounts[hazardType] < maxHazardCounts[hazardType])
            {
                hazardCounts[hazardType]++;
                print($"return {level[lvl][i].name}");
                return level[lvl][i];
            }
        }
        return saws;
    }
    void AddLevelItemsInfo()
    {
        lvlItems = new List<GameObject[]>();
        lvlItems.Add(new GameObject[] { heart });
        lvlItems.Add(new GameObject[] { sBlaster, heart, pBlaster, troll });
        lvlItems.Add(new GameObject[] { heart, sBlaster });
    }
    
    
}
