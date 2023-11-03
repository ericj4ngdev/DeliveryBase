using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rack : MonoBehaviour
{
    public List<Floor> floors = new List<Floor>();
    private void Start()
    {
        // 자식 오브젝트들을 리스트에 추가합니다.
        foreach (Transform child in transform)
        {
            floors.Add(child.gameObject.GetComponent<Floor>());            
        }
        SetColumnForAllFloors();
    }

    // 본인 랙함 번호를 Floor에게 전달 
    private void SetColumnForAllFloors()
    {
        int childIndex = transform.GetSiblingIndex();
        Debug.Log(childIndex);
        foreach (var item in floors)
        {
            item.column = childIndex;
        }
    }

}
