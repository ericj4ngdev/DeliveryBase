using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
public enum CabinetType
{
    Left,
    Right
};
public class CabinetUIManager : MonoBehaviour
{
    [Header("UI List")]
    public List<Slider> sliders = new List<Slider>();
    [Header("Cabinet")]
    public Cabinet cabinet;

    [Header("Debugging")]
    [SerializeField] private string mLocation;
    [SerializeField] private char mType;
    [SerializeField] private int mRack_num;
    [SerializeField] private int mHeight;
    [SerializeField] private int previous_mRack_num;

    public CabinetType cabinetType;
    
    [Header("Text")] 
    public TextMeshProUGUI cabinetTypeText;
    public TextMeshProUGUI rackNumText;
    public TextMeshProUGUI heightText;
    public TextMeshProUGUI locationText;
    
    [Header("Tray Info")]
    public Toggle HasTray;
    public Toggle isLoaded;
    
    [SerializeField] private RobotController robot;

    private void Start()
    {
        mLocation = "";
        mRack_num = -1;
        mHeight = -1;
        robot = FindObjectOfType<RobotController>();
        HasTray.isOn = false;
        isLoaded.isOn = false;
    }

    private void Update()
    {
        GetTrayInfo();
        // SetLocation();
    }

    /// <summary>
    /// 랙 버튼 이벤트 등록, 랙 번호를 인자로 받음
    /// </summary>
    /// <param name="rack_num"> 랙함 번호 </param>
    public void SetRack(int rack_num)
    {
        // 이전 랙함의 슬라이더값 초기화를 위해 이전 랙함 번호 저장
        previous_mRack_num = mRack_num;
        mRack_num = rack_num;
        cabinet.mRack_Num = rack_num;
        
        // 해당 랙함의 슬라이더 이미지 활성화
        foreach (var VARIABLE in sliders)
        {
            VARIABLE.gameObject.SetActive(false);
        }
        sliders[mRack_num].gameObject.SetActive(true);
        
        // 최초 랙함 선택시, 이전 랙함 인덱스는 -1이므로 이를 방지
        if (previous_mRack_num != -1)
        {
            // -1이 아닐때 실행
            sliders[previous_mRack_num].value = 0;       // 랙함 바꿀때마다 이전 슬라이더 값 초기화.
        }
        rackNumText.text = mRack_num.ToString();
        SetLocation();
    }
    
    /// <summary>
    /// 슬라이더 값으로 높이 조정
    /// </summary>
    public void SetHeight()
    {
        int temp;
        switch (mRack_num)
        {
            case 9:
                temp = (int)Mathf.Round(sliders[mRack_num].value * 11) + 1; // temp는 1~12
                mHeight = Mathf.Clamp(temp, 1, 12) + 26;         // h는 26~38
                break;
            case 10:
                temp = (int)Mathf.Round(sliders[mRack_num].value * 11) + 1;
                mHeight = Mathf.Clamp(temp, 1, 12) + 26;
                break;
            case 11:
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
        SetLocation();
    }

    /// <summary>
    /// 위치 갱신
    /// </summary>
    public void SetLocation()
    {
        // 세 값 모두 선택했다면
        if (mRack_num != -1 && mHeight != -1)
        {
            rackNumText.text = mRack_num.ToString();
            heightText.text = mHeight.ToString();
            mLocation = mType + mRack_num.ToString("D2") + mHeight.ToString("D2");
            locationText.text = mLocation;
        }
        else Debug.Log("위치를 모두 입력하십시오.");
    }

    /// <summary>
    /// Move 버튼에 연결
    /// </summary>
    /// <param name="location"></param>
    public void MoveToLocation()
    {
        if (mRack_num != -1 && mHeight != -1)
        {
            Debug.Log(cabinetType.ToString());
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
    }

    public void GetTrayInfo()
    {
        HasTray.isOn = cabinet.hasTray;
        isLoaded.isOn = cabinet.isLoaded;
    }
    
    
}
