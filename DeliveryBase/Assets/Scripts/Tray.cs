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
    public Percel mPercel;     // �ݸ��� ������ ������
    public Transform spawnSpot;
    // private Percel percel;

    private void Start()
    {
        // PercelActive(false);     // �ϴ� ��Ȱ��ȭ, �̰� �� ��ġ�� ���� ����
    }

    // ������ �Է��� �ް� ���� Ȱ��ȭ/��Ȱ��ȭ ���θ� �����Ѵ�. 
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
        // ť���� ���� ������ ���� ���ɴϴ�.
        Vector3 currentScale = new Vector3(0.2f, 0.2f, 0.2f);
        Vector3 currentPos = spawnSpot.localPosition;
        // ť���� �������� �����Ͽ� ���̸� �����մϴ�.
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
            mPercel = null;             // ���� Ʈ���̿��� ����� update�� �ִ� ���ǰ� �浹. Null���� �߻�
            mIsLoaded = false;
        }
    }
}
