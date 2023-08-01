using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum HandleType
{
    Left,
    Right
};

public class Handler : MonoBehaviour
{
    public List<Transform> waypoint;
    public float timer;
    public float LoadingTime;
    public Transform axis;
    private bool hasControl;
    public HandleType handleType;
    [SerializeField] private GameObject mTray;
    private Magnet magnet;

    
    // Start is called before the first frame update
    void Start()
    {
        Transform childTransform = transform.GetChild(0); // 첫 번째 자식 오브젝트
        magnet = childTransform.GetComponent<Magnet>();
        hasControl = false;
        
    }
    
    // Update is called once per frame
    void Update()
    {
        if (handleType == 0)
            LeftController();
        else RightController();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(LoadLeftAllProcess());
        }
        
        // 자식 콜라이더에서 감지하는 tray를 가져온다. 
        if (magnet.mTray != null) print("감지");
    }
    
    IEnumerator LoadLeftAllProcess()
    {

        if (handleType == (HandleType)0)
        {
            yield return StartCoroutine(Hide());
        }

        yield return new WaitForSeconds(LoadingTime * 0.5f);
        
        if (handleType == (HandleType)1)
        {
            GetTray();
            yield return StartCoroutine(Push());
            UnloadTray();
            yield return StartCoroutine(Pull());
            // 다시 가져오기
            yield return StartCoroutine(Push());
            GetTray();
            yield return StartCoroutine(Pull());
            UnloadTray();
            yield return StartCoroutine(Hide());
        }
        
        yield return new WaitForSeconds(LoadingTime * 2.9f);
        
        // 나타나서 잡고 넣기 
        if (handleType == (HandleType)0)
        {
            yield return StartCoroutine(Show());
            GetTray();
            yield return StartCoroutine(Push());
            UnloadTray();
            yield return StartCoroutine(Pull());
            yield return StartCoroutine(Push());
            GetTray();
            yield return StartCoroutine(Pull());
            UnloadTray();
        }
        
        yield return new WaitForSeconds(LoadingTime);
        
        if (handleType == (HandleType)1)
        {
            yield return StartCoroutine(Show());
        }
        print("코루틴 끝");
        
    }
    
    IEnumerator Push()
    {
        StartCoroutine(MoveTray(0,1));
        yield return new WaitForSeconds(LoadingTime);
    }
    
    IEnumerator Pull()
    {
        StartCoroutine(MoveTray(1,0));
        yield return new WaitForSeconds(LoadingTime);
    }

    public void GetTray()
    {
        // Magnet에서 감지조차 안되면 mTray는 null이므로 바로 함수 끝.
        if (magnet.mTray == null)
        {
            print("감지된 Tray가 없습니다.");
            return;
        }
        
        if (mTray == null)
        {
            hasControl = true;
            mTray = magnet.mTray;
            mTray.transform.SetParent(transform);
        }
        else print("mTray가 이미 있습니다.");

    }
    
    public void UnloadTray()
    {
        // Magnet에서 감지조차 안되면 mTray는 null이므로 바로 함수 끝.
        if (magnet.mTray == null)
        {
            print("감지된 Tray가 없습니다.");
            return;
        }
        
        if (mTray != null)
        {
            hasControl = false;
            mTray.transform.SetParent(null);
            mTray = null;
        }
        else print("mTray가 없습니다.");
    }
    
    IEnumerator Hide()
    {
        StartCoroutine(MoveTray(0, 2));
        StartCoroutine(RotateTray(0, 2));
        yield return new WaitForSeconds(LoadingTime);
    }

    IEnumerator Show()
    {
        StartCoroutine(MoveTray(2, 0));
        StartCoroutine(RotateTray(2, 0));
        yield return new WaitForSeconds(LoadingTime);
    }

    
    
    IEnumerator MoveTray(int startIdx, int endIdx)
    {
        timer = 0;
        float LoadingDuration = 1f / LoadingTime;
        while (true)
        {
            timer += Time.deltaTime * LoadingDuration;
            Vector3 startPoint = waypoint[startIdx].position;
            Vector3 endPoint = waypoint[endIdx].position;
            transform.position = Vector3.Lerp(startPoint, endPoint, timer);
            
            if (Vector3.Distance(transform.position, endPoint) <= 0.001f)
            {
                yield break;
            }
            yield return null;
        }
    }

    IEnumerator RotateTray(int startIdx, int endIdx)
    {
        timer = 0;
        float LoadingDuration = 1f / LoadingTime;
        while (true)
        {
            timer += Time.deltaTime * LoadingDuration;
            Transform startPoint = waypoint[startIdx];
            Transform endPoint = waypoint[endIdx];
            // transform.position = Vector3.Lerp(startPoint.position, endPoint.position, timer);
            axis.transform.rotation = Quaternion.Lerp(startPoint.rotation, endPoint.rotation, timer);
            
            if (Quaternion.Angle(axis.transform.rotation, endPoint.rotation) <= 0.001f)
            {
                yield break;
            }
            yield return null;
        }
    }

    private void RightController()
    {
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            StartCoroutine(Push());
        }
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            StartCoroutine(Pull());
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            StartCoroutine(Hide());
        }
        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            StartCoroutine(Show());
        }
        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            UnloadTray();
        }
        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            GetTray();
        }
        
    }

    private void LeftController()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(Pull());
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            StartCoroutine(Push());
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            StartCoroutine(Hide());
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            StartCoroutine(Show());
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UnloadTray();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            GetTray();
        }
    }

}
