using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseEvent : MonoBehaviour
{
    public static PauseEvent Ins;
    public bool Paused;

    void Awake() => Ins = this;
    void Start()
    {
        Paused = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha9))
            PauseGame();
        
    }
    public void PauseGame()
    {
        Paused = !Paused;
        Time.timeScale = Paused ? 1f : 0f;
    }
    
}
