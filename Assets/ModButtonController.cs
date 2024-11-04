using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModButtonController : MonoBehaviour
{
    public string Mod;
    public bool Plus;
    bool pressed;
    ObjectManager Jim;
    public GameObject plusText;
    public GameObject text;
    TimerController timer;
    private void Start()
    {
        pressed = false;
        Jim = ObjectManager.Ins;
        timer = TimerController.Ins;
    }

    public void OnClick()
    {
        if (pressed) return;
        Jim.AddMod(Mod, Plus);
        pressed = true;
        timer.RemoveModCards();
    }

    private void OnDisable()
    {
        Plus = false;
        pressed = false;
    }
}
