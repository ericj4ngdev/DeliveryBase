using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simulate : MonoBehaviour
{
    [SerializeField]
    private RobotController robotController;
    private CabinetUIManager cabinetUIManager;

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

    // 점유량
    // public void SetHei

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
            robotController.MoveToHeightNum(Handler, Row);
        }
    }

}
