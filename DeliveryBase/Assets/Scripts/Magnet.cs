using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Magnet : MonoBehaviour
{
    [Header("Debugging")]
    public GameObject detectedmTray;
    private void OnCollisionEnter(Collision collision)
    {
        print("Magnet.OnCollisionStay");
        
        if (collision.collider.CompareTag("Tray"))
        {
            if (detectedmTray == null)
            {
                detectedmTray = collision.gameObject;
            }
        }
    }
    
    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Tray"))
        {
            print("Magnet.OnCollisionExit");
            detectedmTray = null;
        }
    }
}
