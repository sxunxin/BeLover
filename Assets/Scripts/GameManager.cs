using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // 싱글톤 인스턴스

    // 씬 입장 횟수를 기록하는 변수
    public int mainSceneEnterCount = 0;
    public bool isMission1Clear = false;
    public bool isMission2Clear = false;
    public bool isMission3Clear = false;

    private Button settingButton; // SettingBtn 참조
    private GameObject settingUI; // SettingUI 참조
    private Button resumeBtn;
    private Button gamesettingBtn;
    private Button finishBtn;

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
        }

        // 씬이 로드될 때 SettingBtn과 SettingUI 탐색
        FindSettingComponents();

        // SettingBtn 클릭 이벤트 등록
        if (settingButton != null)
        {
            settingButton.onClick.RemoveAllListeners(); // 기존 리스너 제거
            settingButton.onClick.AddListener(ToggleSettingUI);
        }
        if (resumeBtn != null)
        {
            resumeBtn.onClick.RemoveAllListeners(); // 기존 리스너 제거
            resumeBtn.onClick.AddListener(GameResume);
        }
        if (gamesettingBtn != null)
        {
            gamesettingBtn.onClick.RemoveAllListeners(); // 기존 리스너 제거
            gamesettingBtn.onClick.AddListener(OpenSettingsMenu);
        }
        if (finishBtn != null)
        {
            finishBtn.onClick.RemoveAllListeners(); // 기존 리스너 제거
            finishBtn.onClick.AddListener(FinishGame);
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

    // Setting 버튼과 UI를 찾는 메서드
    private void FindSettingComponents()
    {
        // 현재 씬에서 SettingBtn과 SettingUI 찾기
        settingButton = GameObject.Find("SettingBtn")?.GetComponent<Button>();
        settingUI = GameObject.Find("SettingUI");
        resumeBtn = GameObject.Find("ResumeBtn")?.GetComponent<Button>();
        gamesettingBtn = GameObject.Find("GameSetBtn")?.GetComponent<Button>();
        finishBtn = GameObject.Find("FinishBtn")?.GetComponent<Button>();

        if (settingUI != null)
        {
            // SettingUI를 처음에는 비활성화
            settingUI.SetActive(false);
        }
    }

    // SettingUI를 토글하는 메서드
    private void ToggleSettingUI()
    {
        if (settingUI != null)
        {
            settingUI.SetActive(!settingUI.activeSelf);
        }
    }
    private void GameResume()
    {
        settingUI.SetActive(false);
    }
    private void OpenSettingsMenu()
    {
        Debug.Log("Settings 메뉴 열기");
    }

    private void FinishGame()
    {
        Debug.Log("게임 종료");
        Application.Quit(); // 게임 종료
    }
}
