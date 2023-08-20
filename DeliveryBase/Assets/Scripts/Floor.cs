using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    public Tray tray;
    
    // tray여부에 따라 달라짐
    public bool isFull
    {
        get { return tray != null; }
        private set { isFull = value; }
    }
    private void Start()
    {
        // 자식 오브젝트들을 리스트에 추가합니다.
        foreach (Transform child in transform)
        {
            tray = child.gameObject.GetComponent<Tray>();
        }
    }
    public bool isBlocked;

    private void Update()
    {
        // Debug.Log(isFull);
        CollideEnter();
    }

    public Vector3 center;
    public Vector3 size;

    // 그냥 각 floor가 막혀있는지 아닌지만 판단. 
    // 최초로 감지됐을 때만 정보 가져오기 
    private void CollideEnter()
    {
        // 감지 루프
        Collider[] colliders = Physics.OverlapBox(center, size / 2f);
        /*foreach (Collider collider in colliders)
        {
            Debug.Log("Collided with: " + collider.gameObject.name);
        }*/
        // 감지된게 없다면 isBlocked = false;
        if (colliders.Length == 0)
        {
            // Debug.Log("Empty");
            isBlocked = false;
        }
        else isBlocked = true;
        // 계속 호출이 안되게는 못하나...
    }
    
    private void OnDrawGizmos()
    {
        center = transform.position;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, size);
    }
}
