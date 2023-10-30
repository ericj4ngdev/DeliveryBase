using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Floor : MonoBehaviour
{
    public Tray tray;
    public bool isBlocked;

    public Vector3 center;
    public Vector3 size;

    // tray여부에 따라 달라짐
    public bool isFull;
    /*{
        get { return tray != null; }
        private set { isFull = value; }
    }*/
    private void Start()
    {
        // 아래 코드는 활성화되었을때만 작동
        // tray = transform.gameObject.GetComponentInChildren<Tray>();
        // 자식 오브젝트들을 리스트에 추가합니다.
        foreach (Transform child in transform)
        {
            tray = child.gameObject.GetComponent<Tray>();
        }
    }

    private void Update()
    {
        // Debug.Log(isFull);
        CollideEnter();
        CheckTray();
    }

    // 해당 층에 Tray가 있는지 체크하는 함수
    void CheckTray()
    {
        if (tray.gameObject.activeSelf) isFull = true;
        else isFull = false;
    }   

    // 해당 floor에 Tray 또는 Percel로 막혀있는지 체크하는 함수    
    private void CollideEnter()
    {
        // 감지 루프
        Collider[] colliders = Physics.OverlapBox(center, size / 2f);

        // 감지된게 없다면 isBlocked = false;
        if (colliders.Length == 0)
        {
            // Debug.Log("Empty");
            isBlocked = false;
        }
        else
        {
            // 순회돌아서 Tray 얻어오기
            foreach (Collider child in colliders)
            {
                if(child.gameObject.GetComponent<Tray>() != null)
                {
                    tray = child.gameObject.GetComponent<Tray>();
                    break;
                }
            }

            isBlocked = true;
        }
        // 계속 호출이 안되게는 못하나...
    }
    
    private void OnDrawGizmos()
    {
        center = transform.position;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, size);
    }
}
