using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tray : MonoBehaviour
{
    public bool IsLoaded;
    private GameObject mPercel;
    private void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        // print("OnCollisionEnter");
        if (collision.gameObject.CompareTag("Parcel"))
        {
            if (mPercel == null)
            {
                collision.transform.SetParent(transform);
                mPercel = collision.gameObject;
                IsLoaded = true;
            }
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Parcel"))
        {
            print("Tray.OnCollisionExit");
            mPercel = null;
        }
    }
}
