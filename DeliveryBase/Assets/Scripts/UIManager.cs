using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public List<Rack> racks = new List<Rack>();
    public string location;
    public CabinetUIManager cabinetUIManager;
    // Start is called before the first frame update
    void Start()
    {
        cabinetUIManager = FindObjectOfType<CabinetUIManager>();
    }

    public void PutTray()
    {
        location = cabinetUIManager.locationText.text;
        int rect_num = int.Parse(cabinetUIManager.rackNumText.text);
        int height = int.Parse(cabinetUIManager.heightText.text) - 1;
        // racks[rect_num].floors[height].tray
        if (racks[rect_num] == null) return;
        if (racks[rect_num].floors[height] == null) return;
        int maxFloor = 5;
        // for (int i = maxFloor; i > 0; i--)
        // {
            if (racks[rect_num].floors[height].isBlocked == false
            && racks[rect_num].floors[height + maxFloor].isBlocked == false)
            {
                // 배치 가능
                racks[rect_num].floors[height].tray.gameObject.SetActive(true);
            }
            else
            {
                Debug.Log("배치할 수 없습니다");
            }
        // }
        // 비어있다면
        
    }

    public void DeleteTray()
    {
        
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
