using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI; // UI 사용을 위한 네임스페이스 추가
using UnityEngine.EventSystems; // 터치 이벤트 처리를 위한 네임스페이스 추가

public class HeartImageTracking : MonoBehaviour
{
    public ARTrackedImageManager trackedImageManager; // AR Tracked Image Manager
    public GameObject heartPrefab; // 심장 이미지 프리팹
    public GameObject circulationPrefab; // 순환계 이미지 프리팹
    public GameObject explanationPrefab; // 설명 이미지 프리팹

    private GameObject heartObject; // 생성된 심장 이미지 오브젝트
    private GameObject circulationObject; // 생성된 순환계 이미지 오브젝트
    private GameObject explanationObject; // 생성된 설명 이미지 오브젝트

    void OnEnable()
    {
        // 이미지 변화 이벤트를 구독합니다.
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        // 이미지 변화 이벤트 구독 해제
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    void Update()
    {
        // 터치가 감지되면
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); // 첫 번째 터치 가져오기

            if (touch.phase == TouchPhase.Began) // 터치가 시작되면
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position); // 터치 위치로부터 레이 생성
                RaycastHit hit;

                // 레이캐스트로 충돌 체크
                if (Physics.Raycast(ray, out hit))
                {
                    // 심장 이미지가 터치되면
                    if (hit.transform.CompareTag("HeartImage"))
                    {
                        // 순환계 이미지와 설명이 생성되지 않았다면
                        if (circulationObject == null && explanationObject == null)
                        {
                            circulationObject = Instantiate(circulationPrefab, hit.transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity); // 순환계 이미지 생성
                            explanationObject = Instantiate(explanationPrefab, hit.transform.position + new Vector3(0, -0.5f, 0), Quaternion.identity); // 설명 이미지 생성
                        }
                    }
                }
            }
        }
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        // 추가된 트래킹 이미지 처리
        foreach (var trackedImage in eventArgs.added)
        {
            if (trackedImage.referenceImage.name == "PersonImage") // 사람 이미지 이름 확인
            {
                // 사람 인식 시 심장 이미지 생성
                heartObject = Instantiate(heartPrefab, trackedImage.transform.position, Quaternion.identity); // 심장 이미지 생성
            }
        }

        // 업데이트된 트래킹 이미지 처리
        foreach (var trackedImage in eventArgs.updated)
        {
            if (heartObject != null && trackedImage.referenceImage.name == "PersonImage")
            {
                // 위치 및 회전 업데이트
                heartObject.transform.position = trackedImage.transform.position;
                heartObject.transform.rotation = trackedImage.transform.rotation;
            }
        }

        // 제거된 트래킹 이미지 처리
        foreach (var trackedImage in eventArgs.removed)
        {
            if (trackedImage.referenceImage.name == "PersonImage" && heartObject != null)
            {
                // 심장 이미지 삭제
                Destroy(heartObject);
                heartObject = null;
            }
        }
    }
}
