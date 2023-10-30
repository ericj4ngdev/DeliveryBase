using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// ������
/// </summary>
public class Simulate : MonoBehaviour
{    
    [SerializeField] private RobotController robotController;
    [SerializeField] private CabinetUIManager l_CabinetUIManager;
    [SerializeField] private CabinetUIManager r_CabinetUIManager;
    // ���� ����� RightCabinetPanel ������Ʈ Ȱ��ȭ
    // Ŭ��(����Ƽ)���� ����� ������ RightCabinetPanel ��Ȱ��ȭ

    [SerializeField] private Cabinet l_Cabinet;
    [SerializeField] private Cabinet r_Cabinet;


    public int Handler;        // { get; set; }
    public int Column;      // { get; set; }
    public int Row;         // { get; set; }

    private int entranceColumn;
    private int entranceRow;

    public int Height { get; set; }
    public int Result { get; set; }
    // public int Handler { get; set; }
    public int Ret { get; set; }


    private void Start()
    {
        entranceColumn = 10;
        entranceRow = 9;
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
                if (Column == 0 && Column == 1 && Column == 8) 
                {
                    Debug.Log("�̵� �Ұ�");                    
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
            // L/R ĳ��� �����ֱ�
            robotController.MoveToHeightNum(Handler, Row + 1);
        }
    }

    public void MoveAndLoadTray()
    {        
        MoveHandler();
        Invoke("LoadTray", 3.0f);
    }

    public void MoveAndUnloadTray()
    {
        MoveHandler();
        Invoke("UnloadTray", 3.0f);
    }

    private void LoadTray()
    {        
        // Cabinet�� mRack_Num = simulate.Column
        // ����ȭ�ؼ� ȣ���ϱ�
        switch (Handler)
        {
            case 1:
                l_Cabinet.mRack_Num = Column;
                l_Cabinet.PutTray();
                break;
            case 2:
                r_Cabinet.mRack_Num = Column - 6;
                r_Cabinet.PutTray();
                break;
            default:
                break;
        }
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
                r_Cabinet.mRack_Num = Column - 6;
                r_Cabinet.GetTray();
                break;
            default:
                break;
        }
    }

    public void EntranceLoadTray()
    {
        MoveToEntrance();
        Invoke("LoadTray", 3.0f);
    }

    public void EntranceUnloadTray()
    {
        MoveToEntrance();
        Invoke("UnloadTray", 3.0f);
    }


    private void MoveToEntrance()
    {
        // Column = 10 ����(�̵��� 4�� waypoint, ���� ��ȣ�� 10��)
        // Row = 9 ����
        Column = entranceColumn;
        if (Handler == 1) robotController.MoveToRackNum(entranceColumn - 6);

        if (Handler == 2) robotController.MoveToRackNum(entranceColumn - 8);    // 0~4 ���� ��

        // L/R ĳ��� �����ֱ�
        robotController.MoveToHeightNum(Handler, entranceRow);
    }

}
