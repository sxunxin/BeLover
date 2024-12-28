using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class TalkManager : MonoBehaviour
{
    MainSceneManager Msm;

    public GameObject dialoguePanel;

    public TypeEffect CinemaP1;
    public TypeEffect CinemaP2;

    public bool isDialogueFinished = false;

    private string[] P1Text = {
        "어 여기가 어디지?",
        "누구세요?",
        "엥? 저는 잘 보이는데요?",
        "잘 모르겠는데 낭떠러지 같아요...\n\n그리고 무슨 로봇에 타있는데 조종이 안돼요...",
        "어? 앞으로 가고 있어요"
    };

    private string[] P2Text = {
        "거기 누구 있어요?",
        "저도 잘 모르겠어요.\n\n아무것도 기억이 나질 않아요...\n\n그리고 눈 앞이 깜깜해서 보이지가 않아요...",
        "그럼 여기가 어딘지 알아요?",
        "제 앞에 운전대가 있는거 같긴 한데 움직여볼게요",
        "저는 앞이 안 보이는데 말로 설명해주실 수 있어요?"
    };

    private int P1Index = 0;
    private int P2Index = 0;

    private bool isP1Talking = true; // P1부터 시작

    public string[] S2Text = {
        "외모스트레스로 인해 거울을 보지 못하고,\n\n스트레스를 받아 한이 맺힌 유령",
        "으아아아아아 거울이 너무 싫어 다 박살내버릴거야!!!",
        "부서진 거울 조각을 찾아 거울을 완성해보자.\n\n단 두 플레이어가 조종할 수 있는 2개의 방향키가 다르고,\n\n조각을 먹을 때마다 방향키가 랜덤으로 바뀐다.",
        "휴... 우리가 거울 조각을 모아왔어.",
        "으악! 저리 치워!",
        "아니야, 거울을 봐. 네가 아까와는 다르게 보여.",
        "그래...? 똑같은거 같은데...",
        "너의 빛나는 눈동자를 봐.",
        "그래! 거울보다 더 반짝거리는걸? 정말 예쁘다.",
        "정말 그렇게 생각해...?",
        "그럼. 너는 어떤 것 같아?",
        "나도...그런 것 같아...",
        "한을 풀어줘서 고마워, 나중에 꼭 필요할 물건을 줄게.\n\n너희가 먹었던 거울 조각 5개를 거울 5개로 갚아줄게."
    };
    public string[] S3Text =
    {
        "외모스트레스로 인해 사람들을 무서워하고,\n\n사회와 분리되어 공황장애가 온 유령",
        "죽을 것 같아...다시 예전으로 돌아갈 수 없어...",
        "플레이어들이 각각 다른 공간에서 문제를 풀어\n\n서로에게 도움을 주고 탈출해보자.\n\n처음엔 여자의 바닥 타일에 대응하는 버튼을 남자가 눌러 해결해야한다.",
        "나는 곧 나의 검은 자아에 잡아먹힐거야...\n\n내가 저지른 공포를 지울 수 없어.",
        "공포...? 우리가 오면서 봤던 해골들을 말하는거야?",
        "맞아...분명 내가 저지른 공포일거야...",
        "잘 기억해봐. 저건 너가 저지른 공포가 아니야.",
        "너를 잡아먹으려는 검은 자아의 짓이야.",
        "또 찾아오면 어떡해...?",
        "우리가 검은 자아가 오지 못하게 촛불을 켜 놓았어.",
        "이제 너 자신을 똑바로 바라볼 수 있을거야.",
        "고마워...이제 살 수 있을거 같아.",
        "한을 풀어줘서 고마워, 나중에 꼭 필요할 물건을 줄게.\n\n너희가 도와준 횟수만큼 3개의 블럭 더미를 줄게."
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
}