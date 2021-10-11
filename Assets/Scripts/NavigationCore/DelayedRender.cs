using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedRender : MonoBehaviour
{
    [SerializeField] GameObject Object;
    void Awake ()
    {
        StartCoroutine (AfterTime ());
    }
 
    IEnumerator AfterTime ()
    {
        yield return new WaitForSeconds(2f);
        Object.GetComponent<MeshRenderer>().enabled = true;
    }
}
