using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerController : MonoBehaviour
{
    public ModCards cards;
    public Transform[] cardPos;
    public float timer;
    public float endTime;
    public int per;
    public TextMeshProUGUI text;
    string percentage;
    Manager manager;
    public bool h_ON, e_ON, c_ON, m_ON;
    [Header("Spawn Times")]
    public int eachSHP;
    public int eachSEP;
    public int eachSCP;
    public int eachAMP;

    public List<int> spawnHazardPercentages;
    public List<int> spawnEnemyPercentages;
    public List<int> spawnCannonPercentages;
    public List<int> addModsPercentages;
    PauseEvent pause;
    ObjectManager objMan;
    GameObject[] curCards;
    
    public static TimerController Ins;

    private void Awake() => Ins = this;
    void Start()
    {
        pause = PauseEvent.Ins;
        objMan = ObjectManager.Ins;
        manager = Manager.Ins;
        curCards = new GameObject[3];
        cards.b = new ModButtonController[cards.mC.Length];
        for (int i = 0; i < cards.mC.Length; i++)
            cards.b[i] = cards.mC[i].GetComponentInChildren<ModButtonController>();
        int num = eachSHP;
        spawnEnemyPercentages = new();
        spawnHazardPercentages = new();
        spawnCannonPercentages = new();
        addModsPercentages = new();

        if (h_ON)
        {
            while (num < 100)
            {

                spawnHazardPercentages.Add(num);
                num += eachSHP;
            }
        }else spawnHazardPercentages.Add(300);
        num = eachSEP;
        if (e_ON)
        {
            while (num < 100)
            {
                spawnEnemyPercentages.Add(num);
                num += eachSEP;
            }
        }else spawnEnemyPercentages.Add(300);
        num = eachSCP;
        if (c_ON)
        {
            while (num < 100)
            {
                spawnCannonPercentages.Add(num);
                num += eachSCP;
            }
        }else spawnCannonPercentages.Add(300);
        num = eachAMP;
        if (m_ON)
        {
            while (num < 100)
            {
                addModsPercentages.Add(num);
                num += eachAMP;
            }
        }else addModsPercentages.Add(300);
        
        
    }
    void Update()
    {
        timer += Time.deltaTime;

        timer = Mathf.Min(timer, endTime);

        per = (int)((timer / endTime) * 100);
        text.text = $"{per}%";

        if (e_ON && per == spawnEnemyPercentages[0]){
            spawnEnemyPercentages.RemoveAt(0);
            int ran = Random.Range(1, 3);
            for (int i = 0; i < ran; i++)
                manager.HandleTimedEvents(0);
            
        } if (h_ON && per == spawnHazardPercentages[0]){
            spawnHazardPercentages.RemoveAt(0);
            int ran = Random.Range(1, 3);
            for (int i = 0; i < ran; i++)
                manager.HandleTimedEvents(1);

        } if (c_ON && per == spawnCannonPercentages[0]){
            spawnCannonPercentages.RemoveAt(0);
            int ran = Random.Range(1, 3);
            for (int i = 0; i < ran; i++)
                manager.HandleTimedEvents(2);
            
        } if (m_ON && per == addModsPercentages[0]){
            addModsPercentages.RemoveAt(0);
            AddModCards();
        }
        if (Input.GetKeyDown(KeyCode.V)) AddModCards();
    }
    public void AddModCards()
    {
        pause.PauseGame();
        List<int> plusVals = new List<int> { 6, 8, 9 };
        List<GameObject> mcs = new List<GameObject>(cards.mC);
        List<ModButtonController> mbc = new List<ModButtonController>(cards.b);

        int ran = Random.Range(0, mcs.Count);
        GameObject m1 = mcs[ran];
        ModButtonController b1 = mbc[ran];
        mcs.RemoveAt(ran);
        mbc.RemoveAt(ran);

        int ran2 = Random.Range(0, mcs.Count);
        GameObject m2 = mcs[ran2];
        ModButtonController b2 = mbc[ran2];
        mcs.RemoveAt(ran2);
        mbc.RemoveAt(ran2);

        int ran3 = Random.Range(0, mcs.Count);
        GameObject m3 = mcs[ran3];
        ModButtonController b3 = mbc[ran3];

        m1.transform.position = cardPos[0].position;
        m2.transform.position = cardPos[1].position;
        m3.transform.position = cardPos[2].position;
        curCards[0] = m1; curCards[1] = m2; curCards[2] = m3;

        m1.SetActive(true); m2.SetActive(true); m3.SetActive(true);

       

        int ranPlus = Random.Range(1, 11); 
        b1.Plus = ranPlus <= 2; b1.plusText.SetActive(b1.Plus); b1.text.SetActive(!b1.Plus);
        b2.Plus = ranPlus == 6; b2.plusText.SetActive(b2.Plus); b2.text.SetActive(!b2.Plus);
        b3.Plus = ranPlus == 9; b3.plusText.SetActive(b3.Plus); b3.text.SetActive(!b3.Plus);
    }
    public void RemoveModCards()
    {
        for (int i = 0; i < curCards.Length; i++){
            curCards[i].SetActive(false);
            curCards[i] = null;
        }
        pause.PauseGame();
    }

    [System.Serializable]
    public struct ModCards
    {
        public GameObject[] mC;
        public ModButtonController[] b;
    }
}
