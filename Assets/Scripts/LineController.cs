using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    private float fpsCounter;
    public float fps;

    public Texture[] textures;
    int animationStep;
    LineRenderer lineRenderer;
    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        fpsCounter += Time.deltaTime;
        if (fpsCounter >= 1f / fps)
        {
            animationStep = (animationStep + 1) % textures.Length;

            lineRenderer.material.SetTexture("_MainTex", textures[animationStep]);

            fpsCounter = 0f;
        }
    }
}
