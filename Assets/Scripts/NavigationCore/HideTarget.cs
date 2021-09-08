using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideTarget : MonoBehaviour
{
    // Start is called before the first frame update
    public void MakeInvisible()
    {
        gameObject.GetComponent <Renderer>().enabled = false;
    }
}
