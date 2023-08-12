/*
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CabinetInfo : MonoBehaviour
{
    [Header("Cabinet")]
    public Cabinet cabinet;
    
    [Header("Text")] 
    public TextMeshProUGUI cabinetTypeText;
    public TextMeshProUGUI rackNumText;
    public TextMeshProUGUI heightText;
    public TextMeshProUGUI locationText;
    
    [Header("Tray Info")]
    public Toggle HasTray;
    public Toggle isLoaded;
    
    void Start()
    {
        mType = ' ';
        mLocation = "";
        mRack_num = -1;
        mHeight = -1;
        robot = FindObjectOfType<RobotController>();
        HasTray.isOn = false;
        isLoaded.isOn = false;
    }
    
    [Header("Debugging")]
    [SerializeField] private string mLocation;
    [SerializeField] private char mType;
    [SerializeField] private int mRack_num;
    [SerializeField] private int mHeight;
    [SerializeField] private int previous_mRack_num;


    public void GetCabinetInfo()
    {
        // 버튼을 누를때마다 정보 갱신이므로 버튼 이벤트에 추가한다.  
        HasTray.isOn = cabinet.hasTray;
        isLoaded.isOn = cabinet.isLoaded;
    }
    

    public void SelectCabinet()
    {
        foreach (var VARIABLE in rackBtns)
        {
            VARIABLE.gameObject.SetActive(true);
        }
        rackBtns[5].gameObject.SetActive(false);
        rackBtns[11].gameObject.SetActive(false);
        rackBtns[12].gameObject.SetActive(false);
        
        cabinetTypeText.text = "Left";
        mType = 'L';
        
        // 모드 바꾸면 모든 값 초기화.
        // 처음에 모드바꿔도 입력 안바뀜
        
        // 선택시 
        if (mRack_num != -1)
        {
            mRack_num = 0;
        }
        // 아직 선택안함. 정보 갱신 ㄴㄴ
        // 선택시 갱신
        if (mHeight != -1)
        {
            mHeight = 1;
        }
        UpdateInfo();
    }
    
    public void SelectRack(int num)
    {
        // 이전 랙함의 슬라이더값 초기화를 위해 이전 랙함 번호 저장
        // if (previous_mRack_num == -1) return;
        previous_mRack_num = mRack_num;
        // 랙함 버튼에 따라 랙함 UI이미지가 달라진다. 
        mRack_num = num;
        // 해당 랙함의 슬라이더 이미지 활성화
        foreach (var VARIABLE in sliders)
        {
            VARIABLE.gameObject.SetActive(false);
        }
        
        // 초기에 랙함 선택시 이전 랙함 인덱스는 -1이므로 이를 방지
        if (previous_mRack_num != -1)
        {
            // 이 구문만 -1일때 실행되지 않게 하면 된다.
            // -1이 아닐때 실행
            sliders[previous_mRack_num].value = 0;       // 랙함 바꿀때마다 이전 슬라이더 값 초기화.
        }
        sliders[mRack_num].gameObject.SetActive(true);
        rackNumText.text = mRack_num.ToString();
    }

    private void UpdateInfo()
    {
        rackNumText.text = mRack_num.ToString();
        heightText.text = mHeight.ToString();
        mLocation = mType + mRack_num.ToString("D2") + mHeight.ToString("D2");
        locationText.text = mLocation;
    }
    
    public void SetLocation()
    {
        // 세 값 모두 선택했다면
        if (mType != ' ' && mRack_num != -1 && mHeight != -1)
        {
            mLocation = mType + mRack_num.ToString("D2") + mHeight.ToString("D2");
            locationText.text = mLocation;
            MoveToLocation(mLocation);
        }
        else Debug.Log("위치를 모두 입력하십시오.");
    }


    public void MoveToLocation(string location)
    {
        if (mType == 'L')
        {
            if (mRack_num >= 6)
            {
                robot.MoveToRackNum(mRack_num - 6);
            }
            else
            {
                robot.MoveToRackNum(mRack_num);
            }
        }

        if (mType == 'R')
        {
            if (mRack_num >= 8)
            {
                robot.MoveToRackNum(mRack_num - 8);
            }
            else
            {
                robot.MoveToRackNum(mRack_num - 2);
            }
        }
        // L/R 캐비넷 정해주기
        robot.MoveToHeightNum(mType, mHeight);
    }

    public void MoveToDoor()
    {
        
    }
    
    public void SliderToHeight()
    {
        int temp;
        // 어떤 슬라이더인지 slider 인덱스를 받아야 한다. 
        switch (mRack_num)
        {
            case 7:
                temp = (int)Mathf.Round(sliders[mRack_num].value * 11) + 1; // temp는 1~12
                mHeight = Mathf.Clamp(temp, 1, 12) + 26;         // h는 26~38
                break;
            case 8:
                temp = (int)Mathf.Round(sliders[mRack_num].value * 11) + 1;
                mHeight = Mathf.Clamp(temp, 1, 12) + 26;
                break;
            case 9:
                temp = (int)Mathf.Round(sliders[mRack_num].value * 4) + 1;  // temp는 1~5
                mHeight = Mathf.Clamp(temp, 1, 5);
                break;
            default:
                mHeight = (int)Mathf.Round(sliders[mRack_num].value * 37) + 1;
                mHeight = Mathf.Clamp(mHeight, 1, 38);
                break;
        }
        // 가우스 함수처럼 슬라이더 값을 int형으로 변환
        heightText.text = mHeight.ToString();
        
    }
    
}
*/
