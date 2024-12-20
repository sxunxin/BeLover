using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class MainSceneManager : MonoBehaviour
{
    NetworkManager nm;
    TalkManager tm;

    public GameObject storyPanel;
    public Button StartBtn;
    public Image CinemaImage1;
    public Image CinemaImage2;

    public GameObject StoryPanel;
    public TextMeshProUGUI StoryText;
    public TypeEffect CinemaText;

    public GameObject[] mainMission;
    public Sprite[] GhostImage;
    public Image ghostImage;

    public bool isCinemaFinished = false;
    public bool isStart = false;

    void Awake()
    {
        nm = FindObjectOfType<NetworkManager>();
        tm = FindObjectOfType<TalkManager>();

        // 게임 재실행 시 항상 storyPanel을 활성화
        storyPanel.SetActive(true);
        StartBtn.gameObject.SetActive(false);

        // 필요하면 PlayerPrefs 초기화
        PlayerPrefs.SetInt("StoryPanelHidden", 0);
    }
    private void Start()
    {
        // 씬이 시작되면 시네마틱 애니메이션 시작
        StartCoroutine(CinemaSequence());
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SpawnChar();
        }

        if (tm.isDialogueFinished && isStart == false)
        {
            StartBtn.gameObject.SetActive(true);
        }

        if (GameManager.Instance.mainSceneEnterCount >= 2)
        {
            storyPanel.SetActive(false);
            if (GameManager.Instance.mainSceneEnterCount == 2 && GameManager.Instance.isMission1Clear == true && GameManager.Instance.isMission2Clear == false && GameManager.Instance.isMission3Clear == false)
            {
                mainMission[0].tag = "MainMission";
            }
            else
                mainMission[0].tag = "Untagged";

            if (GameManager.Instance.mainSceneEnterCount == 3 && GameManager.Instance.isMission1Clear == true && GameManager.Instance.isMission2Clear == true && GameManager.Instance.isMission3Clear == false)
            {
                mainMission[1].tag = "MainMission";
            }
            else
                mainMission[1].tag = "Untagged";

            if (GameManager.Instance.mainSceneEnterCount == 4 && GameManager.Instance.isMission1Clear == true && GameManager.Instance.isMission2Clear == true && GameManager.Instance.isMission3Clear == true)
            {
                mainMission[2].tag = "MainMission";
            }
            else
                mainMission[2].tag = "Untagged";
        }
    }

    public void SpawnChar()
    {
        isStart = true;
        // 버튼을 비활성화하여 더 이상 보이지 않게 만듭니다.
        StartBtn.gameObject.SetActive(false);

        // storyPanel 숨김 상태 저장
        PlayerPrefs.SetInt("StoryPanelHidden", 1);

        // Spawn 전에 ViewID 관리
        nm.Spawn();

        // 버튼을 누른 플레이어 정보 업데이트
        UpdatePlayerReadyStatus();

        // 두 플레이어가 준비 상태인지 확인
        StartCoroutine(CheckBothPlayersReady());
    }

    void UpdatePlayerReadyStatus()
    {
        // 현재 플레이어가 준비 상태임을 설정
        var props = new ExitGames.Client.Photon.Hashtable
        {
            { "IsReady", true }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    IEnumerator CheckBothPlayersReady()
    {
        while (true)
        {
            // 모든 플레이어의 CustomProperties 확인
            bool allReady = true;
            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (player.CustomProperties.ContainsKey("IsReady"))
                {
                    allReady &= (bool)player.CustomProperties["IsReady"];
                }
                else
                {
                    allReady = false;
                }
            }

            // 두 플레이어가 모두 준비 상태면 Scene1으로 이동
            if (allReady)
            {
                yield return new WaitForSeconds(3f); // 3초 대기
                nm.StartScene1(); // Scene1으로 이동
                yield break; // Coroutine 종료
            }

            yield return null; // 다음 프레임까지 대기
        }
    }

    // =========================
    //   시네마틱 애니메이션 부분
    // =========================

    IEnumerator CinemaSequence()
    {
        CinemaText.SetMsg("어두운 방 한 남자가 괴로워하고 있다");

        yield return new WaitForSeconds(3f); // 3초 대기 후 페이드 아웃 시작

        // CinemaImage1 페이드 아웃과 CinemaImage2 페이드 인을 동시에 실행
        StartCoroutine(FadeOutImage(CinemaImage1, 3f)); // 3초 동안 페이드 아웃

        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(FadeInImage(CinemaImage2, 5f)); // 7초 동안 페이드 인

        CinemaText.SetMsg("한참을 괴로워하다가 잠에 들었다");
        // 페이드 인 완료 후 5초 동안 CinemaImage2의 크기를 3배로 확대
        yield return StartCoroutine(ScaleImage(CinemaImage2, 2.2f, 5f)); // 5초 동안 3배 확대

        // 확대가 끝난 후 3초 동안 대기
        yield return new WaitForSeconds(3f);

        // 확대가 끝난 후 3초 동안 CinemaImage2를 검은색으로 변경
        StartCoroutine(FadeToBlackImage(CinemaImage2, 3f)); // 이미지 RGB 변경
        yield return StartCoroutine(FadeToBlackText(CinemaText, 3f)); // 텍스트 색상 변경

        // 시네마틱 종료 표시
        isCinemaFinished = true;
    }

    IEnumerator FadeOutImage(Image image, float duration)
    {
        float elapsedTime = 0f;
        Color color = image.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsedTime / duration); // 알파 값 1에서 0으로
            image.color = color;
            yield return null;
        }

        color.a = 0f; // 확실히 0으로 고정
        image.color = color;
    }

    IEnumerator FadeInImage(Image image, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // 알파 값 0에서 1로
            Color color = image.color;
            color.a = Mathf.Lerp(0f, 1f, t);

            // RGB 값을 점점 (255, 255, 255)로 변경
            color.r = Mathf.Lerp(image.color.r, 1f, t);
            color.g = Mathf.Lerp(image.color.g, 1f, t);
            color.b = Mathf.Lerp(image.color.b, 1f, t);

            image.color = color;
            yield return null;
        }

        // 최종 색상을 (255, 255, 255)로 고정
        Color finalColor = image.color;
        finalColor.a = 1f;
        finalColor.r = 1f;
        finalColor.g = 1f;
        finalColor.b = 1f;
        image.color = finalColor;
    }

    IEnumerator ScaleImage(Image image, float targetScale, float duration)
    {
        float elapsedTime = 0f;
        Vector3 initialScale = image.rectTransform.localScale;
        Vector3 target = new Vector3(targetScale, targetScale, targetScale); // 목표 크기 (3배)

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            image.rectTransform.localScale = Vector3.Lerp(initialScale, target, elapsedTime / duration); // 크기 변경
            yield return null;
        }

        image.rectTransform.localScale = target; // 최종 크기 고정
    }
    IEnumerator FadeToBlackImage(Image image, float duration)
    {
        float elapsedTime = 0f;
        Color initialColor = image.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // RGB를 (255,255,255)에서 (0,0,0)으로 변경
            Color color = image.color;
            color.r = Mathf.Lerp(initialColor.r, 0f, t);
            color.g = Mathf.Lerp(initialColor.g, 0f, t);
            color.b = Mathf.Lerp(initialColor.b, 0f, t);
            image.color = color;

            yield return null;
        }

        // 최종 색상을 검은색(0,0,0)으로 고정
        Color finalColor = image.color;
        finalColor.r = 0f;
        finalColor.g = 0f;
        finalColor.b = 0f;
        image.color = finalColor;
    }
    IEnumerator FadeToBlackText(TypeEffect textEffect, float duration)
    {
        if (textEffect.MsgText == null)
        {
            Debug.LogError("TypeEffect.MsgText is null!");
            yield break;
        }

        TextMeshProUGUI text = textEffect.MsgText; // TypeEffect에서 TextMeshProUGUI 가져오기
        float elapsedTime = 0f;
        Color initialColor = text.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // RGB를 초기값에서 (0,0,0)으로 변경
            Color color = text.color;
            color.r = Mathf.Lerp(initialColor.r, 0f, t);
            color.g = Mathf.Lerp(initialColor.g, 0f, t);
            color.b = Mathf.Lerp(initialColor.b, 0f, t);
            text.color = color;

            yield return null;
        }

        // 최종 색상을 검은색(0,0,0)으로 고정
        Color finalColor = text.color;
        finalColor.r = 0f;
        finalColor.g = 0f;
        finalColor.b = 0f;
        text.color = finalColor;
    }
}
