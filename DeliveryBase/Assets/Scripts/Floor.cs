using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class Floor : MonoBehaviour
{
    public Tray trayForPlace;
    public Tray currentTray;
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
            trayForPlace = child.gameObject.GetComponent<Tray>();
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
        if (currentTray == null) return;
        if (currentTray.mIsLoaded) isFull = true;
        else isFull = false;        
    }   

    // 해당 floor에 Tray 또는 Percel로 막혀있는지 체크하는 함수    
    private void CollideEnter()
    {
        // if (currentTray != null) return;    // 있으면 감지안해도 됨
        // 없으면 감지 로직

        // 감지 루프
        Collider[] colliders = Physics.OverlapBox(center, size / 2f);

        // 감지된게 없다면 isBlocked = false;
        if (colliders.Length == 0)
        {
            // Debug.Log("비워짐");
            currentTray = null;
            isBlocked = false;
            return;
        }
        else
        {
            isBlocked = true;       // 뭔가 물체가 있으면 Block되었다고 표시
        }
        

        // 해당 층이 채워져 있으면 감지 안함. isBlocked = false일때만 작동
        // 나가면 그때부터 감지 모드. 나가면 isBlocked = false이므로 조건에 추가
        if (colliders.Length != 0 && isBlocked == false)
        {
            // 순회돌아서 Tray 얻어오기
            foreach (Collider child in colliders)
            {
                Tray trayComponent = child.GetComponent<Tray>();
                if (trayComponent != null)
                {
                    currentTray = trayComponent;
                    // currentTray.transform.SetParent(transform);
                    isBlocked = true;
                    Invoke("GetCurrentTray", 3);
                    break; // 찾았으면 반복 종료
                }
            }
        }

        // 계속 호출이 안되게는 못하나...
    }
    private void GetCurrentTray() => currentTray.transform.SetParent(transform);


    private void OnDrawGizmos()
    {
        center = transform.position;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, size);
    }
}
