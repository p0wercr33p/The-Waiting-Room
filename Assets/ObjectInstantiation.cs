using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInstantiation : MonoBehaviour
{
    public bool marker;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Instantiation());
    }
    IEnumerator Instantiation()
    {
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        if (marker) yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
    }
}
