using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 구동부
/// </summary>
public class Simulate : MonoBehaviour
{    
    [SerializeField] private RobotController robotController;
    [SerializeField] private CabinetUIManager l_CabinetUIManager;
    [SerializeField] private CabinetUIManager r_CabinetUIManager;
    // 서버 제어시 RightCabinetPanel 오브젝트 활성화
    // 클라(유니티)에서 제어시 시작전 RightCabinetPanel 비활성화

    [SerializeField] private Cabinet l_Cabinet;
    [SerializeField] private Cabinet r_Cabinet;


    public int Handler;        // { get; set; }
    public int Column;      // { get; set; }
    public int Row;         // { get; set; }
    public int Height { get; set; }
    public int Result { get; set; }
    // public int Handler { get; set; }
    public int Ret { get; set; }


    private void Start()
    {
        
    }

    // 왼쪽, 오른쪽 핸들
    public void SetType()
    {

    }

    // 랙함 번호 
    public void SetColumn()
    {

    }

    // 높이
    public void SetRow()
    {

    }

    // 결과 번호 
    public void SetResult()
    {

    }

    // Ret 번호 
    public void SetRet()
    {

    }

    
    
    // 움직임 담당
    public void MoveHandler()
    {
        if (Column != -1 && Row != -1)
        {
            if (Handler == 1)
            {
                if (Column >= 6)
                {
                    robotController.MoveToRackNum(Column - 6);
                }
                else
                {
                    robotController.MoveToRackNum(Column);
                }
            }

            if (Handler == 2)
            {
                if (Column == 0 && Column == 1 && Column == 1 && Column == 8) 
                {
                    Debug.Log("이동 불가");                    
                    return; 
                }
                if (Column >= 8)
                {
                    robotController.MoveToRackNum(Column - 8);
                }
                else
                {
                    robotController.MoveToRackNum(Column - 2);
                }
            }
            // L/R 캐비넷 정해주기
            robotController.MoveToHeightNum(Handler, Row + 1);
        }
    }

    public void MoveAndLoadTray()
    {
        if (Column != -1 && Row != -1)
        {
            if (Handler == 1)
            {
                if (Column >= 6)
                {
                    robotController.MoveToRackNum(Column - 6);
                }
                else
                {
                    robotController.MoveToRackNum(Column);
                }
            }

            if (Handler == 2)
            {
                if (Column == 0 && Column == 1 && Column == 1 && Column == 8)
                {
                    Debug.Log("이동 불가");
                    return;
                }
                if (Column >= 8)
                {
                    robotController.MoveToRackNum(Column - 8);
                }
                else
                {
                    robotController.MoveToRackNum(Column - 2);
                }
            }

            // L/R 캐비넷 정해주기
            robotController.MoveToHeightNum(Handler, Row + 1);
        }
        Invoke("LoadTray", 3.0f);
    }

    private void LoadTray()
    {        
        // Cabinet의 mRack_Num = simulate.Column
        // 동기화해서 호출하기
        switch (Handler)
        {
            case 1:
                l_Cabinet.mRack_Num = Column;
                l_Cabinet.PutTray();
                break;
            case 2:
                r_Cabinet.mRack_Num = Column;
                r_Cabinet.PutTray();
                break;
            default:
                break;
        }
    }

    public void MoveAndUnloadTray()
    {
        if (Column != -1 && Row != -1)
        {
            if (Handler == 1)
            {
                if (Column >= 6)
                {
                    robotController.MoveToRackNum(Column - 6);
                }
                else
                {
                    robotController.MoveToRackNum(Column);
                }
            }

            if (Handler == 2)
            {
                if (Column == 0 && Column == 1 && Column == 1 && Column == 8)
                {
                    Debug.Log("이동 불가");
                    return;
                }
                if (Column >= 8)
                {
                    robotController.MoveToRackNum(Column - 8);
                }
                else
                {
                    robotController.MoveToRackNum(Column - 2);
                }
            }

            // L/R 캐비넷 정해주기
            robotController.MoveToHeightNum(Handler, Row + 1);
        }
        Invoke("UnloadTray", 3.0f);
    }

    private void UnloadTray()
    {
        switch (Handler)
        {
            case 1:
                l_Cabinet.mRack_Num = Column;
                l_Cabinet.GetTray();
                break;
            case 2:
                r_Cabinet.mRack_Num = Column;
                r_Cabinet.GetTray();
                break;
            default:
                break;
        }
    }


}
