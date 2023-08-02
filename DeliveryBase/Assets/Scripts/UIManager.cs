using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public Slider slider;
    public TextMeshProUGUI heightText;
    private int height;


    // Start is called before the first frame update
    void Start()
    {
        // slider.wholeNumbers = true; // 슬라이더가 정수값만 가질 수 있도록 설정
    }

    // Update is called once per frame
    void Update()
    {
        
        // 가우스 함수처럼 슬라이더 값을 int형으로 변환
        height = (int)Mathf.Round(slider.value * 36) + 1;
        // 값의 범위를 1~37로 제한
        height = Mathf.Clamp(height, 1, 37);
        heightText.text = height.ToString();
    }
}
