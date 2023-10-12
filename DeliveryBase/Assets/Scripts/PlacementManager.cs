using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlacementManager : MonoBehaviour
{
    public List<Rack> racks = new List<Rack>();
    public string location;
    public CabinetUIManager cabinetUIManager;
    public TMP_Dropdown percelDr;

    [SerializeField] public int rect_num;
    [SerializeField] public int rect_height;            // 랙함 높이
    [SerializeField] public int percel_Size;
    // Start is called before the first frame update
    void Start()
    {
        cabinetUIManager = FindObjectOfType<CabinetUIManager>();
    }

    // 버튼 누를때마다 cabinetUIManager의 값을 읽어와서 동기화
    public void SetHeight() => rect_height = int.Parse(cabinetUIManager.heightText.text) - 1;
    public void SetRack() => rect_num = int.Parse(cabinetUIManager.rackNumText.text);

    public void AddTray()
    {
        if (racks[rect_num] == null) return;
        if (racks[rect_num].floors[rect_height] == null) return;

        // if the floor is blocked by percel or tray
        if (racks[rect_num].floors[rect_height].isBlocked == false)
        {
            // 배치 가능
            racks[rect_num].floors[rect_height].tray.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("배치할 수 없습니다");
        }
    }

    public void SetPercelSizebyServer()
    {
        switch (percel_Size)
        {
            case 1:
                racks[rect_num].floors[rect_height].tray.PercelAcive(false);
                break;
            case 2:
                {
                    // 배치하려는 짐의 5개의 층이 막혀있는지 확인
                    for (int i = 1; i < 6; i++)
                    {
                        // 한 층이라도 block이면 배치할수 없음
                        if (racks[rect_num].floors[rect_height + i].isFull)
                        {
                            Debug.Log("짐을 배치할 수 없습니다.");
                            return;
                        }
                    }
                    racks[rect_num].floors[rect_height].tray.PercelAcive(true);
                    // percelDr.value = percel_Size;
                    racks[rect_num].floors[rect_height].tray.PercelSize(percel_Size - 1);
                }
                break;
            case 3:
                {
                    // 배치하려는 짐의 10개의 층이 막혀있는지 확인
                    // 자기 자신은 제외해야 할듯...
                    for (int i = 1; i < 11; i++)
                    {
                        // 한 층이라도 block이면 배치할수 없음
                        if (racks[rect_num].floors[rect_height + i].isFull)
                        {
                            Debug.Log("짐을 배치할 수 없습니다.");
                            return;
                        }
                    }
                    racks[rect_num].floors[rect_height].tray.PercelAcive(true);
                    // percelDr.value = percel_Size;
                    racks[rect_num].floors[rect_height].tray.PercelSize(percel_Size - 1);
                }
                break;
            default:
                break;
        }
    }

    public void SetPercelSize()
    {
        // percelDr.value = percel_Size;
        // percelDr.value는 인덱스값이다. UI상에 나오는 값이 아니다. 
        // 즉, UI체크박스에서 1은 value = 0이다.
        switch (percelDr.value)
        {
            case 0:
                racks[rect_num].floors[rect_height].tray.PercelAcive(false);
                break;
            case 1:
                {
                    // 배치하려는 짐의 5개의 층이 막혀있는지 확인
                    for (int i = 1; i < 6; i++)
                    {
                        // 한 층이라도 block이면 배치할수 없음
                        if (racks[rect_num].floors[rect_height + i].isFull)
                        {
                            Debug.Log("짐을 배치할 수 없습니다.");
                            return;
                        }
                    }
                    racks[rect_num].floors[rect_height].tray.PercelAcive(true);
                    percel_Size = percelDr.value;
                    racks[rect_num].floors[rect_height].tray.PercelSize(percel_Size);
                }
                break;
            case 2:
                {
                    // 배치하려는 짐의 10개의 층이 막혀있는지 확인
                    // 자기 자신은 제외해야 할듯...
                    for (int i = 1; i < 11; i++)
                    {
                        // 한 층이라도 block이면 배치할수 없음
                        if (racks[rect_num].floors[rect_height + i].isFull)
                        {
                            Debug.Log("짐을 배치할 수 없습니다.");
                            return; 
                        }
                    }
                    racks[rect_num].floors[rect_height].tray.PercelAcive(true);
                    percel_Size = percelDr.value;
                    racks[rect_num].floors[rect_height].tray.PercelSize(percel_Size);
                }
                break;
            default:
                break;
        }
        
    }

    public void DeleteTray()
    {
        if (racks[rect_num] == null) return;
        if (racks[rect_num].floors[rect_height] == null) return;

        // 트레이가 있으면
        if (racks[rect_num].floors[rect_height].isFull == true)
        {
            // 삭제 가능
            racks[rect_num].floors[rect_height].tray.gameObject.SetActive(false);            
            racks[rect_num].floors[rect_height].tray.PercelSize(0);
        }
        else
        {
            Debug.Log("트레이가 없습니다.");
        }
    }

    public void DeleteAllTray()
    {
        foreach (var item in racks)
        {
            int max_height = item.floors.Count;     // 각 랙함에 floor수
            for (int i = 0; i < max_height; i++)
            {
                item.floors[i].tray.gameObject.SetActive(false);
                item.floors[i].tray.PercelAcive(false);
                item.floors[rect_height].tray.PercelSize(0);
            }
        }
        Debug.Log("모두 삭제");        
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
