using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tray : MonoBehaviour
{
    private bool hasControl;
    private void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        // print("OnCollisionEnter");
        if (collision.gameObject.CompareTag("Parcel"))
        {
            collision.transform.SetParent(transform);
        }
    }
}
