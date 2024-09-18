using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class RaycastController : MonoBehaviour
{
    public float dstBeRays = 0.25f;
    
    public int hRays, vRays;

    [HideInInspector]
    public float hRaySpacing;
    [HideInInspector] 
    public float vRaySpacing, skinWidth = .015f;

    [SerializeField]
    public RayCastOrigins rcOg; 
    [HideInInspector] public BoxCollider2D boxCol;
    public LayerMask mask;
    public static RaycastController Inst;
    public virtual void Awake()
    {
        boxCol = GetComponent<BoxCollider2D>();
        CalcRaySpacing();
    }
    // Start is called before the first frame update
    public virtual void Start()
    {
        CalcRaySpacing();
    }

    public void UpdateRayOrigins()
    {
        Bounds bounds = boxCol.bounds;
        bounds.Expand(skinWidth * 2);

        rcOg.topL = new Vector3(bounds.min.x, bounds.max.y);
        rcOg.topR = new Vector3(bounds.max.x, bounds.max.y);
        rcOg.botL = new Vector3(bounds.min.x, bounds.min.y);
        rcOg.botR = new Vector3(bounds.max.x, bounds.min.y);
    }
    public void CalcRaySpacing()
    {
        Bounds bounds = boxCol.bounds;
        bounds.Expand(skinWidth * 2);
        float boundsW = bounds.size.x, boundsH = bounds.size.y; 
        hRays = Mathf.Clamp((int)((boundsH / dstBeRays) * 1.5f), 4, 20);
        vRays = Mathf.Clamp((int)((boundsW / dstBeRays) * 1.5f), 4, 20);

        hRaySpacing = bounds.size.y / (hRays - 1);
        vRaySpacing = bounds.size.x / (vRays - 1);
    }

    [System.Serializable]
    public struct RayCastOrigins
    {
        public Vector2 topL, topR, botL, botR;
    }

    [System.Serializable]
    public struct ColInfo
    {
        public bool above, below, right, left, fTPlat;
        public void reset() { above = below = left = right = false; }
    }
}
