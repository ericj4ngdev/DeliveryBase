using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class PlacementManager : MonoBehaviour
{
    public List<Rack> racks = new List<Rack>();
    public string location;
    public CabinetUIManager cabinetUIManager;
    public TMP_Dropdown percelDr;

    [SerializeField] public int rect_num;
    [SerializeField] public int rect_height;            // 랙함 높이
    [SerializeField] public int percel_Size;

    [Header ("Entrance")]
    public Floor entranceFloor;


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
        // if (racks[rect_num].floors[rect_height].currentTray != null) return;
        if (racks[rect_num].floors[rect_height].isBlocked) return;      // 층이 막혀있는 경우

        // if the floor is blocked by percel or tray
        if (racks[rect_num].floors[rect_height].isFull == false)
        {
            // 배치 가능
            racks[rect_num].floors[rect_height].trayForPlace.gameObject.SetActive(true);
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
                Debug.Log("짐 크기 0");
                racks[rect_num].floors[rect_height].trayForPlace.PercelActive(false);
                break;
            case 2:
                {
                    // 배치하려는 짐의 5개의 층이 막혀있는지 확인
                    for (int i = 1; i < 6; i++)
                    {
                        // 한 층이라도 block이면 배치할수 없음
                        // 그러니까 다 비어있어야 for문을 다돌고 나온다.
                        if (racks[rect_num].floors[rect_height + i].isBlocked)
                        {
                            Debug.Log("짐을 배치할 수 없습니다.");
                            return;
                        }
                    }
                    racks[rect_num].floors[rect_height].trayForPlace.PercelActive(true);
                    // percelDr.value = percel_Size;
                    racks[rect_num].floors[rect_height].trayForPlace.PercelSize(percel_Size - 1);
                }
                break;
            case 3:
                {
                    // 배치하려는 짐의 10개의 층이 막혀있는지 확인
                    // 자기 자신은 제외해야 할듯...
                    for (int i = 1; i < 11; i++)
                    {
                        // 한 층이라도 block이면 배치할수 없음
                        if (racks[rect_num].floors[rect_height + i].isBlocked)
                        {
                            Debug.Log("짐을 배치할 수 없습니다.");
                            return;
                        }
                    }
                    racks[rect_num].floors[rect_height].trayForPlace.PercelActive(true);
                    // percelDr.value = percel_Size;
                    racks[rect_num].floors[rect_height].trayForPlace.PercelSize(percel_Size - 1);
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
                racks[rect_num].floors[rect_height].trayForPlace.PercelActive(false);
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
                    racks[rect_num].floors[rect_height].trayForPlace.PercelActive(true);
                    percel_Size = percelDr.value;
                    racks[rect_num].floors[rect_height].trayForPlace.PercelSize(percel_Size);
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
                    racks[rect_num].floors[rect_height].trayForPlace.PercelActive(true);
                    percel_Size = percelDr.value;
                    racks[rect_num].floors[rect_height].trayForPlace.PercelSize(percel_Size);
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
            racks[rect_num].floors[rect_height].currentTray.gameObject.SetActive(false);            
            racks[rect_num].floors[rect_height].currentTray.PercelSize(0);
            racks[rect_num].floors[rect_height].isFull = false;
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
                item.floors[i].trayForPlace.gameObject.SetActive(false);
                item.floors[i].trayForPlace.PercelActive(false);
                item.floors[rect_height].trayForPlace.PercelSize(0);
                item.floors[rect_height].isFull = false;
            }
        }
        Debug.Log("모두 삭제");        
    }

    public void AddPercelOnEntrance()
    {
        // 입구에 트레이가 있을 때만 실행
        if (entranceFloor.currentTray == null)
        {
            Debug.Log("트레이가 없습니다.");
            return;
        }
        if (entranceFloor.currentTray.mIsLoaded)
        {
            Debug.Log("짐이 있습니다.");
            return;
        }
        // 트레이가 없는 경우
        // 트레이에 짐이 없는 경우
        if (entranceFloor.currentTray != null
            || entranceFloor.currentTray.mIsLoaded == false)
        {
            // 짐 추가
            entranceFloor.currentTray.mPercel.gameObject.SetActive(true);
        }
        
    }

    public void DeletePercelOnEntrance()
    {
        if (entranceFloor.currentTray == null)
        {
            Debug.Log("트레이가 없습니다.");
            return;
        }

        if (entranceFloor.currentTray != null
            || entranceFloor.currentTray.mIsLoaded == false)
        {
            // 짐 삭제
            entranceFloor.currentTray.mPercel.gameObject.SetActive(false);
        }
    }

    public stAllParcelCheckRes[] CheckAllTray()
    {
        Tray[] trays = FindObjectsOfType<Tray>();
        stAllParcelCheckRes[] packets = new stAllParcelCheckRes[trays.Length];

        for (int i = 0; i < trays.Length - 1; i++)
        {
            packets[i] = trays[i].GetTrayInfo();
        }

        return packets;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
