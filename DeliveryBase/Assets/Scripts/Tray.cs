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
    [SerializeField] private GameObject mPercel;     // �ݸ��� ������ ������
    public Transform spawnSpot;
    // private Percel percel;

    private void Start()
    {
        // percel = mPercel.GetComponent<Percel>();
        // PercelAcive(false);     // �ϴ� ��Ȱ��ȭ
    }

    // ������ �Է��� �ް� ���� Ȱ��ȭ/��Ȱ��ȭ ���θ� �����Ѵ�. 
    public void PercelAcive(bool isActive)
    {
        mPercel.SetActive(isActive);
    }
    public void PercelSize(int height)
    {
        // ť���� ���� ������ ���� ���ɴϴ�.
        Vector3 currentScale = new Vector3(0.2f, 0.2f, 0.2f);
        Vector3 currentPos = spawnSpot.localPosition;
        // ť���� �������� �����Ͽ� ���̸� �����մϴ�.
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
