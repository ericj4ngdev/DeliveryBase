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
    public Percel mPercel;     // 콜리전 감지로 가져옴
    public Transform spawnSpot;
    // private Percel percel;

    private void Start()
    {
        // PercelActive(false);     // 일단 비활성화, 이게 짐 배치의 버그 원인
    }

    // 서버의 입력을 받고 짐을 활성화/비활성화 여부를 결정한다. 
    public void PercelActive(bool isActive)
    {
        mPercel.gameObject.SetActive(isActive);
        mIsLoaded = isActive;
    }

    private void Update()
    {
        // mIsLoaded = mPercel.gameObject.activeSelf;
    }

    public void PercelSize(int height)
    {
        // 큐브의 현재 스케일 값을 얻어옵니다.
        Vector3 currentScale = new Vector3(0.2f, 0.2f, 0.2f);
        Vector3 currentPos = spawnSpot.localPosition;
        // 큐브의 스케일을 변경하여 높이를 조절합니다.
        mPercel.transform.localPosition = new Vector3(currentPos.x, currentPos.y * height, currentPos.z);
        mPercel.transform.localScale = new Vector3(currentScale.x, currentScale.y * height, currentScale.z);
        mPercel.m_size = height + 1;
    }

    public stAllParcelCheckRes GetTrayInfo()
    {
        stAllParcelCheckRes pAllParcelCheckRes = new stAllParcelCheckRes();

        pAllParcelCheckRes.id = id;
        pAllParcelCheckRes.column = column;
        pAllParcelCheckRes.row = row;
        pAllParcelCheckRes.height = mPercel.m_size;
        pAllParcelCheckRes.trackingNum = mPercel.trackingNum;
        
        /*if (mIsLoaded)
        {
            switch (mPercel.m_size)
            {
                case 1:
                    {
                        pAllParcelCheckRes.height = 1;
                        pAllParcelCheckRes.trackingNum = null;
                    }
                    break;
                case 2:
                    pAllParcelCheckRes.height = 2;
                    pAllParcelCheckRes.trackingNum = mPercel.trackingNum;
                    break;
                case 3:
                    pAllParcelCheckRes.height = 3;
                    pAllParcelCheckRes.trackingNum = mPercel.trackingNum;
                    break;
                default:
                    break;
            }
            // pAllParcelCheckRes.trackingNum = mPercel.trackingNum;

        }
        else
        {
            pAllParcelCheckRes.height = 1;
            pAllParcelCheckRes.trackingNum = null;
        }*/

        return pAllParcelCheckRes;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // print("OnCollisionEnter");
        if (collision.gameObject.CompareTag("Parcel"))
        {
            if (mPercel == null)
            {
                collision.transform.SetParent(transform);
                mPercel = collision.gameObject.GetComponent<Percel>();
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
            mPercel = null;             // 짐이 트레이에서 벗어나면 update에 있는 조건과 충돌. Null오류 발생
            mIsLoaded = false;
        }
    }
}
