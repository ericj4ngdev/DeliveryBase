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

    public int mRack_Num;
    // Start is called before the first frame update
    void Start()
    {
        
        GetTrayInfo();
    }

    private void Update()
    {
        GetTrayInfo();
    }

    public void PutTray()
    {
        // 트레이가 있으면 실행하기
        if (hasTray == true)
        {
            StartCoroutine(Co_PutTray());
        }
        else Debug.Log("트레이가 없습니다.");
    }

    public void GetTray()
    {
        // 트레이가 없으면 실행하기
        if (hasTray == false)
        {
            StartCoroutine(Co_GetTray());
        }
        else Debug.Log("트레이가 이미 있습니다.");
    }

    IEnumerator Co_PutTray()
    {
        // currentIndex 설명
        // 0 : pull
        // 1 : push
        // 2 : Hide
        if (mRack_Num < 5)
        {
            if (left_Handler.currentIndex != 2)
            {
                left_Handler.DetachTray();
                yield return StartCoroutine(left_Handler.Hide(loadingTime));
                // yield return new WaitForSeconds(loadingTime * 0.1f);
            }
            // 숨은 상태라면 보여주기
            if (right_Handler.currentIndex == 2)
            {
                yield return StartCoroutine(right_Handler.Show(loadingTime));
                // yield return new WaitForSeconds(loadingTime * 0.1f);
                right_Handler.AttachTray();         // < 추가
            }
            yield return new WaitForSeconds(loadingTime * 0.1f);
            yield return StartCoroutine(right_Handler.Push(loadingTime));
            right_Handler.DetachTray();
            yield return StartCoroutine(right_Handler.Pull(loadingTime));
        }
        else
        {
            // 숨은 상태라면 보여주기
            if (left_Handler.currentIndex == 2)
            {
                yield return StartCoroutine(left_Handler.Show(loadingTime));
                // yield return new WaitForSeconds(loadingTime * 0.1f);
                left_Handler.AttachTray();         // < 추가
            }
            if (right_Handler.currentIndex != 2)
            {
                right_Handler.DetachTray();
                yield return StartCoroutine(right_Handler.Hide(loadingTime));
                // yield return new WaitForSeconds(loadingTime * 0.1f);
            }
            yield return new WaitForSeconds(loadingTime * 0.1f);
            yield return StartCoroutine(left_Handler.Push(loadingTime));
            left_Handler.DetachTray();
            yield return StartCoroutine(left_Handler.Pull(loadingTime));
        }
        // 이미 숨은 상태이면 숨지 않기
        // 안 숨어있으면 숨기
        
    }
    
    IEnumerator Co_GetTray()
    {
        if (mRack_Num < 5)
        {
            if (left_Handler.currentIndex != 2)
            {
                yield return StartCoroutine(left_Handler.Hide(loadingTime));
                // yield return new WaitForSeconds(loadingTime * 0.1f);
            }
            // 숨은 상태라면 보여주기
            if (right_Handler.currentIndex == 2)
            {
                yield return StartCoroutine(right_Handler.Show(loadingTime));
                // yield return new WaitForSeconds(loadingTime * 0.1f);
            }
            yield return new WaitForSeconds(loadingTime * 0.1f);
            yield return StartCoroutine(right_Handler.Push(loadingTime));
            right_Handler.AttachTray();
            yield return StartCoroutine(right_Handler.Pull(loadingTime));
        }
        else
        {
            // 안 숨겨져있으면 숨기기
            if (right_Handler.currentIndex != 2)
            {
                yield return StartCoroutine(right_Handler.Hide(loadingTime));
                // yield return new WaitForSeconds(loadingTime * 0.1f);
            }
            // 숨은 상태라면 보여주기
            if (left_Handler.currentIndex == 2)
            {
                yield return StartCoroutine(left_Handler.Show(loadingTime));
                // yield return new WaitForSeconds(loadingTime * 0.1f);
            }
            yield return new WaitForSeconds(loadingTime * 0.1f);
            yield return StartCoroutine(left_Handler.Push(loadingTime));
            left_Handler.AttachTray();
            yield return StartCoroutine(left_Handler.Pull(loadingTime));
        }
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
