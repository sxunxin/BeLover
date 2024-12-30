using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class TalkManager : MonoBehaviour
{
    MainSceneManager Msm;

    public GameObject dialoguePanel;
    public GameObject EndPanel;
    public GameObject EndingPanel;
    public GameObject EndingCreditPanel;
    public float scrollSpeed = 50f;    // 스크롤 속도

    public TypeEffect CinemaP1;
    public TypeEffect CinemaP2;

    public TypeEffect endP1;
    public TypeEffect endP2;
    public TypeEffect Ending;
    public TypeEffect EndingText;
    public Image EndingImage;

    public bool isDialogueFinished = false;
    public bool isEnd = true;

    private string[] P1Text = {
        "어 여기가 어디지?",
        "자려고 누웠는데 여기네요...",
        "에? 저는 잘 보이는데 움직여지지가 않네요",
        "그러면 전 보이니까 저기 길 끝까지 한번 가봐요.\n\n제가 설명해  드릴게요",
    };

    private string[] P2Text = {
        "누구세요?",
        "저도요! 근데 여기는 어딜까요?\n\n아무것도 보이지가 않아요",
        "전 안보이는데 앞에 조종간이 있어요!",
        "안보여서 무서우니까 잘 설명해주세요..." 
    };

    private int P1Index = 0;
    private int P2Index = 0;

    private bool isP1Talking = true; // P1부터 시작

    public string[] S2Text = {
        "외모스트레스로 인해 거울을 보지 못하고,\n\n스트레스를 받아 한이 맺힌 유령",
        "안녕? 너가 거울의 방 유령이니?",
        "(유령이 거울을 보자 소스라치게 놀라며 거울을 깨뜨리고 도망간다...)",
        "도망갔네요.. 거울을 무서워하는 거 같은데 무슨 일 일까요?",
        "우선 다시 만나게 거울을 다시 완성해봐요. 그럼 오지 않을까요?",
        "부서진 거울 조각을 찾아 거울을 완성해보자.\n\n단 두 플레이어가 조종할 수 있는 2개의 방향키가 다르고,\n\n조각을 먹을 때마다 방향키가 랜덤으로 바뀐다.",
        "(거울이 완성되자 유령이 다시 깨려고 달려든다)",
        "왜 자꾸 거울을 깨려고 하는거야?",
        "이리 줘! 거울이 있으면 내 모습이 보이잖아!",
        "(남자가 거울을 주지 않는다.)",
        "왜? 그렇게 이상하게 생기지도 않고 귀엽기만한데",
        "진짜?... 그렇게 말해준 사람들은 너희가 처음이야...고마워,\n\n나중에 꼭 필요할 물건을 줄게.",
        "(거울 5개를 받았다.)"
    };
    public string[] S3Text =
    {
        "외모스트레스로 인해 사람들을 무서워하고,\n\n사회와 분리되어 공황장애가 온 유령",
        "분리의 방이면 어떤 유령일까요? 어? 어디갔어요?",
        "제 목소리 들려요! 전 여기 무슨 방에 있어요!",
        "안녕! 너희들은 누구야? 내가 만든 퍼즐 풀어볼려고 왔어?\n\n(밝게 얘기하다 갑자기 힘들어한다.)",
        "미안 일단 내가 조금 힘이 들어서...\n\n둘이서 퍼즐 먼저 풀면 나중에 내가 다시 올게...",
        "플레이어들이 각각 다른 공간에서 문제를 풀어\n\n서로에게 도움을 주고 탈출해보자.\n\n처음엔 여자의 바닥 타일에 대응하는 버튼을 남자가 눌러 해결해야한다.",
        "퍼즐 다 깼는데 어디있어!",
        "나 왔어! 퍼즐 어땠어? 재미있었어?",
        "(퍼즐에 대한 재미있었다는 이야기를 해준다.)",
        "재밌게 즐겨줘서 고마워! 공황 때문에 항상 혼자 노니까 쓸쓸했어..\n\n너희 같이 좋은 얘들 덕분에 많이 좋아졌어",
        "고마워, 너희 덕분이야. 여기 나중에 꼭 필요할 선물이야.\n\n너희가 푼 퍼즐만큼 블럭을 줄게.",
        "(특별한 블럭 3개를 받았다.)",
    };
    public string[] S4Text =
    {
        "외모스트레스로 인해 밖을 나가지 못하고, \n\n어두운 방에서 혼자 지내는 유령",
        "여긴 또 어디지? 너무 어둡네...",
        "거기 계세요? 여기 바닥에 지도 같은게 보이는데!",
        "여기 불을 좀 켜야 할거 같은 데 거긴 어때요?",
        "지도에 위치가 나오는거 같은데 거기서 제가 말한대로 가봐요!",
        "서로 도움을 줘서 모든 불을 순서대로 전부 켜보자.\n\n모든 불은 순서가 있고 잘못되면\n\n조각상과 상호작용하여 초기화 하라.",
        "당신들은...누구세요?",
        "너를 찾고 있었어.",
        "저를…요?",
        "응. 이 어두운 곳에서 힘들지 않아?",
        "저는 지금 편한데... 어둠 속에 있으면 누군가를 볼 일도 없고 마음이 편한... 걸요...",
        "하지만 아무도 못 봐서 외롭지 않아?",
        "여기 촛불 가져왔어. 한번 켜볼래? 따듯할거야.",
        "(촛불에 불을 붙이고 서로의 얼굴이 보인다.)\n\n정말 따뜻하네요... 감사합니다...",
        "저도 선물을 드릴게요.\n\n나중에 꼭 필요하실거에요.",
        "(촛불 20개를 받았다.)"
    };

    private void Awake()
    {
        Msm = FindObjectOfType<MainSceneManager>();
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        // 대사를 바로 시작하려면 Start()에서 호출하도록 수정
        if (Msm != null && Msm.isCinemaFinished)
        {
            dialoguePanel.SetActive(true);
            StartCoroutine(ShowDialogue());
        }
    }

    // Update에서 코루틴을 매번 호출하지 않도록 수정
    private void Update()
    {
        // dialoguePanel은 계속 켜놓고, 시네마틱이 끝났을 때만 대사를 시작
        if (Msm != null && Msm.isCinemaFinished && !dialoguePanel.activeSelf)
        {
            dialoguePanel.SetActive(true);
            StartCoroutine(ShowDialogue());
        }
        if(GameManager.Instance.mainSceneEnterCount == 6 && isEnd)
        {
            isEnd = false;
            EndPanel.SetActive(true);
            StartCoroutine(ShowEnding());
        }
    }

    IEnumerator ShowDialogue()
    {
        // 시퀀스대로 대사 보여주기
        while (P1Index < P1Text.Length || P2Index < P2Text.Length)
        {
            if (isP1Talking && P1Index < P1Text.Length)
            {
                CinemaP1.SetMsg(P1Text[P1Index]);
                yield return new WaitForSeconds(P1Text[P1Index].Length / CinemaP1.CharPerSeconds); // 대사 길이만큼 기다림
                P1Index++;
                isP1Talking = false; // 다음은 P2의 차례
                yield return new WaitForSeconds(2f);
            }
            else if (!isP1Talking && P2Index < P2Text.Length)
            {
                CinemaP2.SetMsg(P2Text[P2Index]);
                yield return new WaitForSeconds(P2Text[P2Index].Length / CinemaP2.CharPerSeconds); // 대사 길이만큼 기다림
                P2Index++;
                isP1Talking = true; // 다음은 P1의 차례
                yield return new WaitForSeconds(2f);
            }
            yield return null;
        }
        isDialogueFinished = true;
        // 대사가 모두 끝나면 dialoguePanel을 비활성화
        dialoguePanel.SetActive(false);
    }

    public IEnumerator ImagePadeOut(Image image, Image image1)
    {
        RectTransform rectTransform = image.GetComponent<RectTransform>(); // RectTransform 가져오기
        CanvasGroup canvasGroup = image.GetComponent<CanvasGroup>(); // Alpha 값을 제어하기 위해 CanvasGroup 사용
        if (canvasGroup == null)
        {
            canvasGroup = image.gameObject.AddComponent<CanvasGroup>(); // 없으면 추가
        }

        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, 0); // 초기 Y 위치 설정
        canvasGroup.alpha = 1f; // 초기 Alpha 값 설정

        while (rectTransform.anchoredPosition.y < 1200)
        {
            rectTransform.anchoredPosition += new Vector2(0, 200) * Time.deltaTime; // Y 위치 증가
            rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, new Vector3(0.2f, 0.2f, 0.2f), Time.deltaTime); // 크기 점점 줄이기
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0f, Time.deltaTime); // 투명도 점점 감소
            yield return null; // 다음 프레임까지 대기
        }

        if (rectTransform.anchoredPosition.y > 1200)
        {
            image1.gameObject.SetActive(true);
            yield return new WaitForSeconds(5f);

            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel("MainScene");
            }
        }
    }
    IEnumerator ShowEnding()
    {
        Ending.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        endP1.SetMsg("저희는 뭐였을까요");
        yield return new WaitForSeconds(3f);

        endP2.SetMsg("그리고 저희는 어떻게 되는걸까요");
        yield return new WaitForSeconds(5f);

        endP1.gameObject.SetActive(false);
        endP2.gameObject.SetActive(false);

        Ending.SetMsg("그들은 각각 한 남자의 육체, 정신적 기능을 담당하는 자아였다.\n\n" +
            "그들로 하여금 성불된 유령들은 남자가 가지고 있던 병든 자아였고\n\n" +
            "마지막 유령은 자기혐오로 인해 앞서 나온 자아들 간의\n\n위로와 통합을 바라지 않고 자기 자신을 가장 싫어했던 자아였다.\n\n" +
            "그들의 노력으로 상처 받은 한 남자의 자아들을 회복한 것이다.");
        yield return new WaitForSeconds(30f);
        EndPanel.SetActive(false);

        EndingPanel.SetActive(true);
        StartCoroutine(FadeToWhite(7f)); // 3초 동안 페이드
        yield return new WaitForSeconds(5f);
        EndingText.SetMsg("남자가 잠에서 깬다.");
        yield return new WaitForSeconds(4f);
        EndingText.SetMsg("몸이 가볍고 더 이상 고통이 느껴지지 않는다.");
        yield return new WaitForSeconds(7f);
        EndingText.SetMsg("더 이상 과거에 얽매이지 않고, 내일을 시작할 용기가 생긴 것 같다.");
        yield return new WaitForSeconds(12f);

        EndingPanel.SetActive(false);
        EndingCreditPanel.SetActive(true);
        StartCoroutine(ScrollCredits());
    }
    private IEnumerator FadeToWhite(float duration)
    {
        if (EndingImage == null)
        {
            Debug.LogError("EndingImage가 할당되지 않았습니다.");
            yield break;
        }

        Color startColor = EndingImage.color; // 현재 색상 가져오기
        Color targetColor = new Color(1f, 1f, 1f); // 목표 색상

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            // 색상 선형 보간
            EndingImage.color = Color.Lerp(startColor, targetColor, t);
            yield return null; // 다음 프레임까지 대기
        }

        // 최종 색상 보장
        EndingImage.color = targetColor;
    }
    private IEnumerator ScrollCredits()
    {
        RectTransform creditsPanel = EndingCreditPanel.GetComponent<RectTransform>();
        if (creditsPanel == null)
        {
            Debug.LogError("Credits Panel이 할당되지 않았습니다.");
            yield break;
        }

        // 시작 위치와 종료 위치 설정
        float startY = creditsPanel.anchoredPosition.y;
        float endY = creditsPanel.rect.height + Screen.height;

        // 크레딧을 스크롤
        while (creditsPanel.anchoredPosition.y < endY)
        {
            creditsPanel.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        // 스크롤 완료 후 처리
        Debug.Log("크레딧이 끝났습니다.");
    }
}