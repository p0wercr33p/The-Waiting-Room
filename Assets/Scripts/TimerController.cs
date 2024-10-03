using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerController : MonoBehaviour
{
    public float timer;
    public float endTime;
    public int per;
    public TextMeshProUGUI text;
    string percentage;
    Manager manager;
    public bool ON;
    [Header("Spawn Times")]
    public int eachSHP;
    public int eachSEP;
    public int eachSCP;

    public List<int> spawnHazardPercentages;
    public List<int> spawnEnemyPercentages;
    public List<int> spawnCannonPercentages;

    void Start()
    {
        manager = Manager.Ins;
        int num = eachSHP;
        spawnEnemyPercentages = new();
        spawnHazardPercentages = new();

        while (num < 100)
        {
            spawnHazardPercentages.Add(num);
            num += eachSHP;
        }
        num = eachSEP;
        while (num < 100)
        {
            spawnEnemyPercentages.Add(num);
            num += eachSEP;
        }
        num = eachSCP;
        while (num < 100)
        {
            spawnCannonPercentages.Add(num);
            num += eachSCP;
        }
    }
    void Update()
    {
        timer += Time.deltaTime;

        timer = Mathf.Min(timer, endTime);

        per = (int)((timer / endTime) * 100);
        text.text = $"{per}%";
        if (per == spawnEnemyPercentages[0]){
            spawnEnemyPercentages.RemoveAt(0);
            int ran = UnityEngine.Random.Range(1, 3);
            for (int i = 0; i < ran; i++)
            {
                if (ON) manager.HandleTimedEvents(0);
            }

        } if (per == spawnHazardPercentages[0]){
            spawnHazardPercentages.RemoveAt(0);
            int ran = UnityEngine.Random.Range(1, 3);
            for (int i = 0; i < ran; i++)
            {
                if (ON) manager.HandleTimedEvents(1);
            }

        } if (per == spawnCannonPercentages[0]){
            spawnCannonPercentages.RemoveAt(0);
            int ran = UnityEngine.Random.Range(1, 3);
            for (int i = 0; i < ran; i++)
            {
                if (ON) manager.HandleTimedEvents(2);
            }
        }
    }
}
