using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectBank : MonoBehaviour
{
    [Header("Hazards")]
    public GameObject iceSaws;
    public GameObject saws;
    public GameObject heavensFury;
    public GameObject freezeBlast;
    public GameObject bombs;
    public GameObject fireSaws;

    [Header("Items")]
    public GameObject pBlaster;
    public GameObject sBlaster;
    public GameObject heart, troll;

    [Header("Enemies")]
    public GameObject groundDroid;
    public GameObject stormHead;
    public GameObject rocketDroid;
    public GameObject droid;

    [Header("HazardsInfo")]
    public int maxFuries;
    public int maxBombs;
    public int maxSaws;
    public int maxBlasts;
    Dictionary<string, int> hazardCounts;
    Dictionary<string, int> maxHazardCounts;
  

    public Dictionary<GameObject, bool> nul;
    public Dictionary<GameObject, Projectile> droidBullet;
    public GameObject[] lvlHazards;
    public GameObject[] lvlItems;
    public GameObject[] lvlEnemies;
    string[] hazardTypes;
    GameObject me;
    GameObject[] allHazards, allItems, allObjs;
    GameObject[] availableHazards;


    void Awake()
    {
        InitializeHazardCounts();
        AddObjectsInfo();
        Trollololo();
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
    void AddObjectsInfo()
    {
        lvlEnemies = new GameObject[] { droid, rocketDroid, stormHead, groundDroid};
        lvlItems = new GameObject[] { sBlaster, heart, pBlaster, troll };
        lvlHazards = new GameObject[] { iceSaws, saws, fireSaws, bombs, heavensFury, freezeBlast };
        hazardTypes = new string[] { "Saws", "Saws", "Saws", "Bombs", "HeavensFury", "FreezeBlast" };
    }

    public (GameObject,string) MakeHazard()
    {
        int count = lvlHazards.Count();
        int stI = Random.Range(0, count);

        for (int i = stI; i < count; i++)
        {
            string hazardType = hazardTypes[i];
            if (hazardCounts[hazardType] < maxHazardCounts[hazardType])
            {
                hazardCounts[hazardType]++;
                return (lvlHazards[i], hazardTypes[i]);
            }
        }
        for (int i = stI-1; i >= 0; i--)
        {
            string hazardType = hazardTypes[i];
            if (hazardCounts[hazardType] < maxHazardCounts[hazardType])
            {
                hazardCounts[hazardType]++;
                return (lvlHazards[i], hazardTypes[i]);
            }
        }
        return (saws, "Saws");
    }
}
