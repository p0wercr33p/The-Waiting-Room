using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    Manager manager;
    Slider slider;

    // Start is called before the first frame update
    void Awake(){
        slider = GetComponent<Slider>();
    }
    void Start()
    {
        manager = Manager.Ins;
        StartCoroutine(InitializeWithManager());
    }

    private IEnumerator InitializeWithManager()
    {
        yield return null; // Wait for the next frame

        if (manager != null){
            manager.curSlider = slider;
            manager.InitializeSlider();
        }else{
            print("I want to see your manager");
        }
    }
}
