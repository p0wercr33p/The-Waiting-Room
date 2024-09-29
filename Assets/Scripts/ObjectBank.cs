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
    public GameObject toxicBarrel;

    [Header("Items")]
    public GameObject pBlaster;
    public GameObject sBlaster;
    public GameObject heart;

    [Header("Enemies")]
    public GameObject groundDroid;
    public GameObject stormHead;
    public GameObject rocketDroid;
    public GameObject droid;
    public GameObject fireDroid;

    [Header("HazardsInfo")]
    public int maxFuries;
    public int maxBombs;
    public int maxSaws;
    public int maxBlasts;
    public int maxBarrels;
    Dictionary<string, int> hazardCounts;
    Dictionary<string, int> maxHazardCounts;
    public string[] enemyNames;

    public GameObject[] lvlHazards;
    public GameObject[] lvlItems;
    public GameObject[] lvlEnemies;
    string[] hazardTypes;


    void Awake()
    {
        InitializeHazardCounts();
        AddObjectsInfo();
    }

    private void InitializeHazardCounts()
    {
        hazardCounts = new Dictionary<string, int>()
        {
            {"Saws", 0 },
            {"Bombs", 0 },
            {"Fury", 0 },
            {"FreezeBlast", 0 },
            {"ToxicBarrel", 0 }
        };
        maxHazardCounts = new Dictionary<string, int>
        {
            { "Saws", maxSaws },
            { "Bombs", maxBombs },
            { "Fury", maxFuries },
            { "FreezeBlast", maxBlasts },
            {"ToxicBarrel", maxBarrels }
        };
    }
    void AddObjectsInfo()
    {
        lvlEnemies = new GameObject[] { droid, rocketDroid, stormHead, groundDroid, fireDroid};
        lvlItems = new GameObject[] { sBlaster, heart, pBlaster};
        lvlHazards = new GameObject[] { iceSaws, saws, fireSaws, bombs, heavensFury, freezeBlast, toxicBarrel };
        hazardTypes = new string[] { "IceSaws", "Saws", "FireSaws", "Bombs", "Fury", "FreezeBlast", "ToxicBarrel" };
        enemyNames = new string[] { "DR", "RD", "SH", "GD", "FD" };
    }
    public (GameObject,string) MakeHazard()
    {
        int count = lvlHazards.Count();
        int stI = Random.Range(0, count);

        for (int i = stI; i < count; i++)
        {
            string hazardType = hazardTypes[i];
            if (i == 0 || i == 2) hazardType = "Saws";
            if (hazardCounts[hazardType] < maxHazardCounts[hazardType])
            {
                hazardCounts[hazardType]++;
                return (lvlHazards[i], hazardTypes[i]);
            }
        }
        for (int i = stI-1; i >= 0; i--)
        {
            string hazardType = hazardTypes[i];
            if (i == 0 || i == 2) hazardType = "Saws";
            if (hazardCounts[hazardType] < maxHazardCounts[hazardType])
            {
                hazardCounts[hazardType]++;
                return (lvlHazards[i], hazardTypes[i]);
            }
        }
        return (saws, "Saws");
    }
}
