using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Tray : MonoBehaviour
{
    public char[] id;
    public Int32 column;
    public Int32 row;
    public bool mIsLoaded;
    [SerializeField] private GameObject mPercel;     // 콜리전 감지로 가져옴
    public Transform spawnSpot;
    // private Percel percel;

    private void Start()
    {
        // percel = mPercel.GetComponent<Percel>();
        // PercelAcive(false);     // 일단 비활성화
    }

    // 서버의 입력을 받고 짐을 활성화/비활성화 여부를 결정한다. 
    public void PercelAcive(bool isActive)
    {
        mPercel.SetActive(isActive);
    }
    public void PercelSize(int height)
    {
        // 큐브의 현재 스케일 값을 얻어옵니다.
        Vector3 currentScale = new Vector3(0.2f, 0.2f, 0.2f);
        Vector3 currentPos = spawnSpot.localPosition;
        // 큐브의 스케일을 변경하여 높이를 조절합니다.
        mPercel.transform.localPosition = new Vector3(currentPos.x, currentPos.y * height, currentPos.z);
        mPercel.transform.localScale = new Vector3(currentScale.x, currentScale.y * height, currentScale.z);
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
                // percel = mPercel.GetComponent<Percel>();
                mIsLoaded = true;
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
