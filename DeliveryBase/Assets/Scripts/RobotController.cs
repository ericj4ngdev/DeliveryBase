using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class RobotController : MonoBehaviour
{
    [SerializeField] private GameObject mTray;
    public GameObject body;
    public GameObject lPork;
    public GameObject rPork;

    public List<Transform> rackPoint = new List<Transform>();
    public List<Transform> heightPoint = new List<Transform>();
    public float timer;
    public float LoadingTime;
    public int currentRackIdx;
    public int currentHeightIdx;

    private void Start()
    {
        GetCurrentRackIndex();
        GetCurrentHeightIndex();
    }

    private void Update()
    {
        Controller();
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

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveToHeightNum(0);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveToHeightNum(heightPoint.Count-1);
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
        for (int i = 0; i < heightPoint.Count; i++)
        {
            if (Mathf.Abs(lPork.transform.position.y - heightPoint[i].position.y) < 0.01f)
            {
                currentHeightIdx = i;
            }
        }
    }
    
    // 좌우 이동
    public void MoveToRackNum(int idx)
    {
        StartCoroutine(MoveToRack(idx));
    }
    
    public void MoveToHeightNum(int idx)
    {
        StartCoroutine(MoveToHeight(idx));
    }
    
    IEnumerator MoveToRack(int endIdx)
    {
        timer = 0;
        float LoadingDuration = 1f / LoadingTime;
        while (true)
        {
            timer += Time.deltaTime * LoadingDuration;
            Vector3 startPoint = rackPoint[currentRackIdx].position;
            Vector3 endPoint = rackPoint[endIdx].position;
            transform.position = Vector3.Lerp(startPoint, endPoint, timer);
            
            if (Vector3.Distance(transform.position, endPoint) <= 0.001f)
            {
                GetCurrentRackIndex();
                yield break;
            }
            yield return null;
        }
    }
    
    IEnumerator MoveToHeight(int endIdx)
    {
        timer = 0;
        float LoadingDuration = 1f / LoadingTime;
        while (true)
        {
            timer += Time.deltaTime * LoadingDuration;
            float startPoint = heightPoint[currentHeightIdx].position.y;
            float endPoint = heightPoint[endIdx].position.y;
            lPork.transform.position = new Vector3(lPork.transform.position.x,Mathf.Lerp(startPoint, endPoint, timer),lPork.transform.position.z);
            
            if (Mathf.Abs(lPork.transform.position.y - endPoint) < 0.01f)
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
