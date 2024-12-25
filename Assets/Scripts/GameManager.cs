using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // 싱글톤 인스턴스

    // 씬 입장 횟수를 기록하는 변수
    public int mainSceneEnterCount = 0;
    public bool isMission1Clear = false;
    public bool isMission2Clear = false;
    public bool isMission3Clear = false;

    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject); // 중복 GameManager 제거
        }
    }

    // 씬 로드될 때 호출되는 Unity의 내장 메서드
    private void OnEnable()
    {
        // 씬 로드 이벤트에 핸들러 등록
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // 씬 로드 이벤트 핸들러 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬이 로드되었을 때 호출되는 함수
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainScene")
        {
            IncrementMainSceneEnterCount();
            //s3sm 삭제
            S3SceneManager s3Manager = FindObjectOfType<S3SceneManager>();
            if (s3Manager != null)
            {
                Destroy(s3Manager.gameObject); // MainScene에서는 S3SceneManager 제거
                Debug.Log("MainScene에서 S3SceneManager를 제거했습니다.");
            }
        }
    }

    // MainScene 입장 횟수를 증가시키는 함수
    private void IncrementMainSceneEnterCount()
    {
        mainSceneEnterCount++;
        Debug.Log("MainScene에 입장한 횟수: " + mainSceneEnterCount);
    }

    // 현재 MainScene 입장 횟수를 반환하는 함수
    public int GetMainSceneEnterCount()
    {
        return mainSceneEnterCount;
    }
}
