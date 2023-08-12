using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class RobotController : MonoBehaviour
{
    [Header("Pork")]
    public GameObject lPork;
    public GameObject rPork;
    
    [Header("Robot Waypoints")]
    public List<Transform> rackPoint = new List<Transform>();
    public List<Transform> heightPoint = new List<Transform>();
    
    [Header("Debugging")]
    [SerializeField] private float timer_Rack;
    [SerializeField] private float timer_Height;
    [SerializeField] private float LoadingTime;
    [SerializeField] private int currentRackIdx;
    [SerializeField] private int currentLCabinetHeightIdx;
    [SerializeField] private int currentRCabinetHeightIdx;

    // private 
    
    private void Start()
    {
        GetCurrentRackIndex();
        GetCurrentHeightIndex();
    }

    private void Update()
    {
        
    }

    private void Controller()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            MoveToRackNum(0);
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            MoveToRackNum(1);
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            MoveToRackNum(2);
        }
        // if (Input.GetKeyDown(KeyCode.Keypad4))
        // {
        //     MoveToRackNum(3);
        // }
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            MoveToRackNum(4);
        }
    }
    
    public void GetCurrentRackIndex()
    {
        for (int i = 0; i < rackPoint.Count; i++)
        {
            if (Vector3.Distance(transform.position, rackPoint[i].position) < 0.01f)
            {
                currentRackIdx = i;
            }
        }
    }

    public void GetCurrentHeightIndex()
    {
        GetCurrentLeftCabinetHeightIndex();
        GetCurrentRightCabinetHeightIndex();
    }
    public void GetCurrentLeftCabinetHeightIndex()
    {
        for (int i = 0; i < heightPoint.Count; i++)
        {
            if (Mathf.Abs(lPork.transform.position.y - heightPoint[i].position.y) < 0.01f)
            {
                currentLCabinetHeightIdx = i;
            }
        }
    }
    public void GetCurrentRightCabinetHeightIndex()
    {
        for (int i = 0; i < heightPoint.Count; i++)
        {
            if (Mathf.Abs(rPork.transform.position.y - heightPoint[i].position.y) < 0.01f)
            {
                currentRCabinetHeightIdx = i;
            }
        }
    }

    public void GetCabinetInfo()
    {
        
    }
    // 좌우 이동
    public void MoveToRackNum(int idx)
    {
        StartCoroutine(MoveToRack(idx));
    }
    
    public void MoveToHeightNum(char whichCabinet, int endIdx)
    {
        if (whichCabinet == 'L')
        {
            StartCoroutine(MoveToHeight(lPork, currentLCabinetHeightIdx, endIdx));
        }
        if (whichCabinet == 'R')
        {
            StartCoroutine(MoveToHeight(rPork, currentRCabinetHeightIdx, endIdx));
        }
    }
    
    IEnumerator MoveToRack(int endIdx)
    {
        timer_Rack = 0;
        float LoadingDuration = 1f / LoadingTime;
        while (true)
        {
            timer_Rack += Time.deltaTime * LoadingDuration;
            Vector3 startPoint = rackPoint[currentRackIdx].position;
            Vector3 endPoint = rackPoint[endIdx].position;
            transform.position = Vector3.Lerp(startPoint, endPoint, timer_Rack);
            
            if (Vector3.Distance(transform.position, endPoint) <= 0.001f)
            {
                GetCurrentRackIndex();
                yield break;
            }
            yield return null;
        }
    }
    
    IEnumerator MoveToHeight(GameObject pork, int currentHeightIdx, int endIdx)
    {
        timer_Height = 0;
        float LoadingDuration = 1f / LoadingTime;
        while (true)
        {
            timer_Height += Time.deltaTime * LoadingDuration;
            float startPoint = heightPoint[currentHeightIdx].position.y;
            float endPoint = heightPoint[endIdx - 1].position.y;
            pork.transform.position = new Vector3(pork.transform.position.x,Mathf.Lerp(startPoint, endPoint, timer_Height),pork.transform.position.z);
            
            if (Mathf.Abs(pork.transform.position.y - endPoint) < 0.001f)
            {
                GetCurrentHeightIndex();
                yield break;
            }
            yield return null;
        }
    }
    
    
    // 택배를 랙함에서 빼내는 함수
    public void RetrieveFromRack()
    {
        
    }
}
