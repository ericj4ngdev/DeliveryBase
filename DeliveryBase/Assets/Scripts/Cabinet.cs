using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cabinet : MonoBehaviour
{
    [Header("Handler")]
    [Tooltip("각 캐비넷의 핸들에 딸린 Tray정보를 가져오기 위해 생성")]
    public Handler left_Handler;
    public Handler right_Handler;
    
    [Header("TrayInfo")]
    [Tooltip("UIManager에서 해당 정보를 가져감")]
    public bool hasTray;
    public bool isLoaded;

    public float loadingTime;
    private GameObject mTray;
    private Tray tray;

    // Start is called before the first frame update
    void Start()
    {
        
        GetTrayInfo();
    }

    private void Update()
    {
        GetTrayInfo();
    }

    public void Push()
    {
        StartCoroutine(left_Handler.Push(loadingTime));
    }
    public void Pull()
    {
        StartCoroutine(left_Handler.Pull(loadingTime));
    }
    
    public void GetTrayInfo()
    {
        if (left_Handler.mTray != null || right_Handler.mTray != null)
        {
            // 둘중 한 핸들에 Tray가 잡혀 있다면 mTray에 저장
            mTray = left_Handler.mTray != null ? left_Handler.mTray : right_Handler.mTray;
            // mTray를 잡고있는 핸들의 hasControl를 가져온다.   
            hasTray = left_Handler.mTray != null ? left_Handler.hasControl : right_Handler.hasControl;
            tray = mTray.GetComponent<Tray>();
            // 저장한 mTray의 tray 컴포넌트에 접근해서 짐 여부 저장 
            isLoaded = tray.mIsLoaded;       // 짐 여부
        }
        else
        {
            // 둘다 null인 경우
            tray = null;
            mTray = null;
            hasTray = false;
            isLoaded = false;
        }
    }

}
