using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cabinet : MonoBehaviour
{
    public Handler left_Handler;
    public Handler right_Handler;
    
    public Magnet left_Magnet;
    public Magnet right_Magnet;

    private GameObject mTray;
    private Tray tray;
    public bool hasTray;
    public bool isLoaded;
    
    // Start is called before the first frame update
    void Start()
    {
        GetCabinetInfo();
    }

    public void GetCabinetInfo()
    {
        // 두 Magnet 중 하나라도 mTray가 있다면 가져오기
        if (left_Magnet.mTray != null || right_Magnet.mTray != null)
        {
            hasTray = true;                 // Tray 여부
            mTray = left_Magnet.mTray != null ? left_Magnet.mTray : right_Magnet.mTray;
            tray = mTray.GetComponent<Tray>();
            isLoaded = tray.IsLoaded;       // 짐 여부
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
