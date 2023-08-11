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
    
    public TMP_Dropdown cabinetLocation;
    public List<GameObject> rackImage = new List<GameObject>();

    public Tray tray;

    public string mLocation;
    public string mType;
    public int mRack_num;
    public int mHeight;

    [Tooltip("Text")] 
    public TextMeshProUGUI cabinetTypeText;
    public TextMeshProUGUI rackNumText;
    public TextMeshProUGUI heightText;
    
    
    [SerializeField]
    private RobotController robot;
    


    public UnityEvent onSelectCabinet;


    // Start is called before the first frame update
    void Start()
    {
        robot = Transform.FindObjectOfType<RobotController>();
        // slider.wholeNumbers = true; // 슬라이더가 정수값만 가질 수 있도록 설정
        // UnityEvent 
    }

    // Update is called once per frame
    void Update()
    {

        SliderToHeight();
    }

    public void SelectCabinet()
    {
        foreach (var VARIABLE in rackBtns)
        {
            VARIABLE.gameObject.SetActive(true);
        }
        if (cabinetLocation.value == 0)
        {
            rackBtns[4].gameObject.SetActive(false);
            rackBtns[10].gameObject.SetActive(false);
            rackBtns[11].gameObject.SetActive(false);
            cabinetTypeText.text = "Left";
        }

        if (cabinetLocation.value == 1)
        {
            rackBtns[0].gameObject.SetActive(false);
            rackBtns[1].gameObject.SetActive(false);
            rackBtns[5].gameObject.SetActive(false);
            rackBtns[6].gameObject.SetActive(false);
            cabinetTypeText.text = "Right";
        }
        
        mType = cabinetTypeText.text;
        onSelectCabinet.Invoke();
    }

    public void SelectRack(int num)
    {
        // 랙함 버튼에 따라 랙함 UI이미지가 달라진다. 
        mRack_num = num;
        // 해당 랙함의 슬라이더 이미지 활성화
        foreach (var VARIABLE in sliders)
        {
            VARIABLE.gameObject.SetActive(false);
        }

        rackNumText.text = mRack_num.ToString();
        sliders[mRack_num].gameObject.SetActive(true);
    }
    
    public void SelectHeight(int height)
    {
        // mHeight = height;
    }

    public void MoveToLocation()
    {
        if (mType == "Left")
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

        if (mType == "Right")
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
        robot.MoveToHeightNum(mHeight);
    }

    private void SliderToHeight()
    {
        // 어떤 슬라이더인지 slider 인덱스를 받아야 한다. 
        
        // 가우스 함수처럼 슬라이더 값을 int형으로 변환
        mHeight = (int)Mathf.Round(sliders[mRack_num].value * 37) + 1;
        // 값의 범위를 1~38로 제한
        mHeight = Mathf.Clamp(mHeight, 1, 38);
        heightText.text = mHeight.ToString();
        
    }
}
