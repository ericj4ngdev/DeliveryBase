using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    public GameObject mTray;
    private void OnCollisionEnter(Collision collision)
    {
        print("Magnet.OnCollisionStay");
        
        if (collision.collider.CompareTag("Tray"))
        {
            if (mTray == null)
            {
                mTray = collision.gameObject;
            }
        }
    }
    
    private void OnCollisionExit(Collision collision)
    {
        
        if (collision.collider.CompareTag("Tray"))
        {
            print("Magnet.OnCollisionExit");
            mTray = null;
        }
    }
}
