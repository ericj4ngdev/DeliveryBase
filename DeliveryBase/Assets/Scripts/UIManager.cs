using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Tooltip("List")]
    public List<Slider> sliders = new List<Slider>();
    public List<Button> rackBtns = new List<Button>();
    
    // public TMP_Dropdown cabinetLocation;
    
    
    public Cabinet leftCabinet;
    public Cabinet rightCabinet;
    
    private Tray tray;

    public string mLocation;
    public char mType;
    public int mRack_num;
    public int mHeight;

    private int previous_mRack_num;
    
    [Tooltip("Text")] 
    public TextMeshProUGUI cabinetTypeText;
    public TextMeshProUGUI rackNumText;
    public TextMeshProUGUI heightText;
    public TextMeshProUGUI locationText;
    
    
    [SerializeField]
    private RobotController robot;
    
    // public UnityEvent onSelectCabinet;


    // Start is called before the first frame update
    void Start()
    {
        mLocation = "";
        mRack_num = -1;
        mHeight = -1;
        robot = Transform.FindObjectOfType<RobotController>();
        // slider.wholeNumbers = true; // 슬라이더가 정수값만 가질 수 있도록 설정
        // UnityEvent 
    }

    // Update is called once per frame
    void Update()
    {

        // SliderToHeight();
    }

    public void SelectLeftCabinet()
    {
        foreach (var VARIABLE in rackBtns)
        {
            VARIABLE.gameObject.SetActive(true);
        }
        rackBtns[4].gameObject.SetActive(false);
        rackBtns[10].gameObject.SetActive(false);
        rackBtns[11].gameObject.SetActive(false);
        
        cabinetTypeText.text = "Left";
        mType = 'L';
        // 모드 바꾸면 모든 값 초기화.
        mRack_num = 0;
        mHeight = 0;
    }
    
    public void SelectRightCabinet()
    {
        foreach (var VARIABLE in rackBtns)
        {
            VARIABLE.gameObject.SetActive(true);
        }
        rackBtns[0].gameObject.SetActive(false);
        rackBtns[1].gameObject.SetActive(false);
        rackBtns[5].gameObject.SetActive(false);
        rackBtns[6].gameObject.SetActive(false);
        
        cabinetTypeText.text = "Right";
        mType = 'R';
        mRack_num = 0;
        mHeight = 0;
    }
    
    public void SelectRack(int num)
    {
        // 이전 랙함의 슬라이더값 초기화를 위해 이전 랙함 번호 저장
        if (previous_mRack_num == -1) return;
        previous_mRack_num = mRack_num;
        // 랙함 버튼에 따라 랙함 UI이미지가 달라진다. 
        mRack_num = num;
        // 해당 랙함의 슬라이더 이미지 활성화
        foreach (var VARIABLE in sliders)
        {
            VARIABLE.gameObject.SetActive(false);
        }

        sliders[previous_mRack_num].value = 0;       // 랙함 바꿀때마다 이전 슬라이더 값 초기화.
        sliders[mRack_num].gameObject.SetActive(true);
        rackNumText.text = (mRack_num + 1).ToString();
    }
    
    public void SelectHeight(int height)
    {
        // mHeight = height;
    }

    public void PullTray()
    {
        // if (mType != null && mRack_num != -1 && mHeight != -1)
    }
    
    public void SetLocation()
    {
        // 세 값 모두 선택했다면
        if (mType != null && mRack_num != -1 && mHeight != -1)
        {
            mLocation = mType + (mRack_num + 1).ToString("D2") + mHeight.ToString("D2");
            locationText.text = mLocation;
            MoveToLocation(mLocation);
        }
        else Debug.Log("위치를 모두 입력하십시오.");
    }


    public void MoveToLocation(string location)
    {
        if (mType == 'L')
        {
            if (mRack_num >= 5)
            {
                robot.MoveToRackNum(mRack_num - 5);
            }
            else
            {
                robot.MoveToRackNum(mRack_num);
            }
        }

        if (mType == 'R')
        {
            if (mRack_num >= 5)
            {
                
                robot.MoveToRackNum(mRack_num - 7);
            }
            else
            {
                robot.MoveToRackNum(mRack_num);
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
        // 어떤 슬라이더인지 slider 인덱스를 받아야 한다. 
        switch (mRack_num)
        {
            case 7:
                mHeight = (int)Mathf.Round(sliders[mRack_num].value * 11) + 1;
                mHeight = Mathf.Clamp(mHeight, 1, 12);
                break;
            case 8:
                mHeight = (int)Mathf.Round(sliders[mRack_num].value * 11) + 1;
                mHeight = Mathf.Clamp(mHeight, 1, 12);
                break;
            case 9:
                mHeight = (int)Mathf.Round(sliders[mRack_num].value * 4) + 1;
                mHeight = Mathf.Clamp(mHeight, 1, 5);
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
