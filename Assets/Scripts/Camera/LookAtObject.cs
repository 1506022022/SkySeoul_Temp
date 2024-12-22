using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtObject : MonoBehaviour
{
    Transform myTransform;


    void Start()
    {
        myTransform = GetComponent<Transform>();
    }

    void Update()
    {
    }
}
