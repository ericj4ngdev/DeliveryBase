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
    [Header("Handle waypoint")]
    [Tooltip("Waypoints where handle move")]
    public List<Transform> waypoint;
    public Transform axis;
    public HandleType handleType;

    public int currentIndex;
    
    // private이지만 디버깅 용으로 public
    [Header("Debugging")]
    public bool hasControl;        // Tray 제어권 여부 
    public GameObject mTray;
    [SerializeField] private float timer;
    // [SerializeField] private float LoadingTime;
    [SerializeField] private Magnet magnet;

    
    // Start is called before the first frame update
    void Start()
    {
        Transform childTransform = transform.GetChild(0); // 첫 번째 자식 오브젝트
        magnet = childTransform.GetComponent<Magnet>();
        hasControl = false;
        currentIndex = 0;
    }
    
    // Update is called once per frame
    void Update()
    {
        // if (handleType == 0) LeftController();
        // else RightController();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // StartCoroutine(LoadLeftAllProcess());
        }
        // 자식 콜라이더에서 감지하는 tray를 가져온다. 
        // if (magnet.detectedmTray != null) print("감지");
    }

    public IEnumerator Push(float LoadingTime)
    {
        StartCoroutine(MoveHandler(0,1, LoadingTime));
        currentIndex = 1;
        yield return new WaitForSeconds(LoadingTime);
    }
    
    public IEnumerator Pull(float LoadingTime)
    {
        StartCoroutine(MoveHandler(1,0, LoadingTime));
        currentIndex = 0;
        yield return new WaitForSeconds(LoadingTime);
    }

    public void AttachTray()
    {
        // Magnet에서 감지조차 안되면 mTray는 null이므로 바로 함수 끝.
        if (magnet.detectedmTray == null)
        {
            print("감지된 Tray가 없습니다.");
            return;
        }
        
        if (mTray == null)
        {
            hasControl = true;
            mTray = magnet.detectedmTray;       // 마그넷 정보를 가져와서 감지된 tray를 넣는다. 
            mTray.transform.SetParent(transform);
        }
        else print("mTray가 이미 있습니다.");
    }
    
    public void DetachTray()
    {
        // // Magnet에서 감지조차 안되면 mTray는 null이므로 바로 함수 끝.
        // if (magnet.detectedmTray == null)
        // {
        //     print("감지된 Tray가 없습니다.");
        //     return;
        // }
        
        if (mTray != null)
        {
            print("mTray 해제");
            hasControl = false;
            // 자기 자식일때만 부모 해제.
            // 오른쪽이 이미 가져간 상태면 내비두기
            if(mTray.transform.parent == transform)
            {
                mTray.transform.SetParent(null);
            }
            mTray = null;
        }
        else print("mTray가 없습니다.");
    }
    
    public IEnumerator Hide(float LoadingTime)
    {
        StartCoroutine(MoveHandler(0, 2, LoadingTime));
        StartCoroutine(RotateHandler(0, 2, LoadingTime));
        currentIndex = 2;
        yield return new WaitForSeconds(LoadingTime);
    }

    public IEnumerator Show(float LoadingTime)
    {
        StartCoroutine(MoveHandler(2, 0, LoadingTime));
        StartCoroutine(RotateHandler(2, 0, LoadingTime));
        yield return new WaitForSeconds(LoadingTime);
    }
    
    public IEnumerator MoveHandler(int startIdx, int endIdx, float LoadingTime)
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
 
    public IEnumerator RotateHandler(int startIdx, int endIdx, float LoadingTime)
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
}

/*private void RightController()
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
            AttachTray();
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
            AttachTray();
        }
    }*/