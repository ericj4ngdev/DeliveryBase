using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Slider slider;
    public Slider slider_8;
    public Slider slider_9;
    public Slider slider_10;
    public TMP_Dropdown cabinetLocation;
    public TextMeshProUGUI heightText;
    public List<GameObject> rackImage = new List<GameObject>();
    
    
    public List<Button> rackNum = new List<Button>();
    private int height;

    public UnityEvent onSelectCabinet;


    // Start is called before the first frame update
    void Start()
    {
        // slider.wholeNumbers = true; // 슬라이더가 정수값만 가질 수 있도록 설정
        // UnityEvent 
    }

    // Update is called once per frame
    void Update()
    {

        SliderToHeight();
        SliderToHeight_2();
        SliderToHeight_3();
    }

    public void SelectCabinet()
    {
        foreach (var VARIABLE in rackNum)
        {
            VARIABLE.gameObject.SetActive(true);
        }
        if (cabinetLocation.value == 0)
        {
            rackNum[4].gameObject.SetActive(false);
            rackNum[10].gameObject.SetActive(false);
            rackNum[11].gameObject.SetActive(false);
        }

        if (cabinetLocation.value == 1)
        {
            rackNum[0].gameObject.SetActive(false);
            rackNum[1].gameObject.SetActive(false);
            rackNum[5].gameObject.SetActive(false);
            rackNum[6].gameObject.SetActive(false);
        }
        onSelectCabinet.Invoke();
    }

    public void SelectRack()
    {
        // 랙함 버튼에 따라 랙함 UI이미지가 달라진다. 
        
        
        
    }
    
    public void SelectHeight()
    {
        
    }

    private void SliderToHeight()
    {
        // 가우스 함수처럼 슬라이더 값을 int형으로 변환
        height = (int)Mathf.Round(slider.value * 37) + 1;
        // 값의 범위를 1~38로 제한
        height = Mathf.Clamp(height, 1, 38);
        heightText.text = height.ToString();
    }
    
    private void SliderToHeight_2()
    {
        // 가우스 함수처럼 슬라이더 값을 int형으로 변환
        height = (int)Mathf.Round(slider_10.value * 4) + 1;
        // 값의 범위를 1~5로 제한
        height = Mathf.Clamp(height, 1, 5);
        heightText.text = height.ToString();
    }
    
    private void SliderToHeight_3()
    {
        // 가우스 함수처럼 슬라이더 값을 int형으로 변환
        height = (int)Mathf.Round(slider_8.value * 11) + 1;
        // slider_9
        // 값의 범위를 1~12로 제한
        height = Mathf.Clamp(height, 1, 12);
        heightText.text = height.ToString();
    }
    
    
}
