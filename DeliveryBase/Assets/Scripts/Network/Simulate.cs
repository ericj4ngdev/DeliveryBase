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

    // ����, ������ �ڵ�
    public void SetType()
    {

    }

    // ���� ��ȣ 
    public void SetColumn()
    {

    }

    // ����
    public void SetRow()
    {

    }

    // ������
    // public void SetHei

    // ��� ��ȣ 
    public void SetResult()
    {

    }

    // Ret ��ȣ 
    public void SetRet()
    {

    }

    
    
    // ������ ���
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

            // L/R ĳ��� �����ֱ�
            robotController.MoveToHeightNum(Handler, Row);
        }
    }

}
