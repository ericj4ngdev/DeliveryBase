using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class RobotController : MonoBehaviour
{
    [SerializeField] private GameObject mTray;
    private Rigidbody mTray_rb;

    public float timer;
    public float LoadingTime;
    public Vector3 offset;
    public Vector3 previousPos;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tray"))
        {
             mTray = other.gameObject;
             mTray_rb = mTray.GetComponent<Rigidbody>();
             
        }
    }
    private void FollowRobot()
    {
        // 로봇의 위치를 기준으로 택배를 따라다니게 합니다.
        // 나중에 저 offset대신에 tray의 조작값을 넣으면 되지 않을까 생각
        // 아니면 좌우이동하는 동안에는 높이 조절안하고
        // 칸에 딱 이동했을때만 높이 조절 가능하게 하는걸로 하면 가능
        mTray.transform.position = transform.position + offset;
        mTray.transform.rotation = transform.rotation;
    }
    private void FixedUpdate()
    {
        if (mTray != null)
        {
            FollowRobot();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LoadToRack();
        }
    }

    public void UpDown()
    {
        
    }
    
    // 택배를 랙함에 싣는 함수
    public void LoadToRack()
    {
        previousPos = mTray.transform.position;
        StartCoroutine(LoadParcel());
    }

    IEnumerator LoadParcel()
    {
        timer = 0;
        float LoadingDuration = 1f / LoadingTime;
        while (true)
        {
            timer += Time.deltaTime * LoadingDuration;
            mTray.transform.position = previousPos + new Vector3(0,0,Mathf.Lerp(previousPos.z, previousPos.z + 0.72f, timer));
            
            if (mTray.transform.position.z >= previousPos.z + 0.7f)
            {
                print("도달");
                mTray_rb.isKinematic = false;
                mTray_rb.useGravity = true;
                mTray = null;
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
