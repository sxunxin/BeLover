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

        // ???? ?????? ?? ???? storyPanel?? ??????
        storyPanel.SetActive(true);
        StartBtn.gameObject.SetActive(false);

        // ???????? PlayerPrefs ??????
        PlayerPrefs.SetInt("StoryPanelHidden", 0);
    }
    private void Start()
    {
        // ???? ???????? ???????? ?????????? ????
        StartCoroutine(CinemaSequence());
    }
    void Update()
    {

        // ================= ??? ?? ?? (?? ??) =================
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SpawnChar();
        }
        // ==========================================================


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
        // ?????? ???????????? ?? ???? ?????? ???? ????????.
        StartBtn.gameObject.SetActive(false);

        // storyPanel ???? ???? ????
        PlayerPrefs.SetInt("StoryPanelHidden", 1);

        // Spawn ???? ViewID ????
        nm.Spawn();

        // ?????? ???? ???????? ???? ????????
        UpdatePlayerReadyStatus();

        // ?? ?????????? ???? ???????? ????
        StartCoroutine(CheckBothPlayersReady());
    }

    void UpdatePlayerReadyStatus()
    {
        // ???? ?????????? ???? ???????? ????
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
            // ???? ?????????? CustomProperties ????
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

            // ?? ?????????? ???? ???? ?????? Scene1???? ????
            if (allReady)
            {
                yield return new WaitForSeconds(3f); // 3?? ????
                nm.StartScene1(); // Scene1???? ????
                yield break; // Coroutine ????
            }

            yield return null; // ???? ?????????? ????
        }
    }

    // =========================
    //   ???????? ?????????? ????
    // =========================

    IEnumerator CinemaSequence()
    {
        CinemaText.SetMsg("?????? ?? ?? ?????? ?????????? ????");

        yield return new WaitForSeconds(3f); // 3?? ???? ?? ?????? ???? ????

        // CinemaImage1 ?????? ?????? CinemaImage2 ?????? ???? ?????? ????
        StartCoroutine(FadeOutImage(CinemaImage1, 3f)); // 3?? ???? ?????? ????

        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(FadeInImage(CinemaImage2, 5f)); // 7?? ???? ?????? ??

        CinemaText.SetMsg("?????? ???????????? ???? ??????");
        // ?????? ?? ???? ?? 5?? ???? CinemaImage2?? ?????? 3???? ????
        yield return StartCoroutine(ScaleImage(CinemaImage2, 2.2f, 5f)); // 5?? ???? 3?? ????

        // ?????? ???? ?? 3?? ???? ????
        yield return new WaitForSeconds(3f);

        // ?????? ???? ?? 3?? ???? CinemaImage2?? ?????????? ????
        StartCoroutine(FadeToBlackImage(CinemaImage2, 3f)); // ?????? RGB ????
        yield return StartCoroutine(FadeToBlackText(CinemaText, 3f)); // ?????? ???? ????

        // ???????? ???? ????
        isCinemaFinished = true;
    }

    IEnumerator FadeOutImage(Image image, float duration)
    {
        float elapsedTime = 0f;
        Color color = image.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsedTime / duration); // ???? ?? 1???? 0????
            image.color = color;
            yield return null;
        }

        color.a = 0f; // ?????? 0???? ????
        image.color = color;
    }

    IEnumerator FadeInImage(Image image, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // ???? ?? 0???? 1??
            Color color = image.color;
            color.a = Mathf.Lerp(0f, 1f, t);

            // RGB ???? ???? (255, 255, 255)?? ????
            color.r = Mathf.Lerp(image.color.r, 1f, t);
            color.g = Mathf.Lerp(image.color.g, 1f, t);
            color.b = Mathf.Lerp(image.color.b, 1f, t);

            image.color = color;
            yield return null;
        }

        // ???? ?????? (255, 255, 255)?? ????
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
        Vector3 target = new Vector3(targetScale, targetScale, targetScale); // ???? ???? (3??)

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            image.rectTransform.localScale = Vector3.Lerp(initialScale, target, elapsedTime / duration); // ???? ????
            yield return null;
        }

        image.rectTransform.localScale = target; // ???? ???? ????
    }
    IEnumerator FadeToBlackImage(Image image, float duration)
    {
        float elapsedTime = 0f;
        Color initialColor = image.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // RGB?? (255,255,255)???? (0,0,0)???? ????
            Color color = image.color;
            color.r = Mathf.Lerp(initialColor.r, 0f, t);
            color.g = Mathf.Lerp(initialColor.g, 0f, t);
            color.b = Mathf.Lerp(initialColor.b, 0f, t);
            image.color = color;

            yield return null;
        }

        // ???? ?????? ??????(0,0,0)???? ????
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

        TextMeshProUGUI text = textEffect.MsgText; // TypeEffect???? TextMeshProUGUI ????????
        float elapsedTime = 0f;
        Color initialColor = text.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // RGB?? ?????????? (0,0,0)???? ????
            Color color = text.color;
            color.r = Mathf.Lerp(initialColor.r, 0f, t);
            color.g = Mathf.Lerp(initialColor.g, 0f, t);
            color.b = Mathf.Lerp(initialColor.b, 0f, t);
            text.color = color;

            yield return null;
        }

        // ???? ?????? ??????(0,0,0)???? ????
        Color finalColor = text.color;
        finalColor.r = 0f;
        finalColor.g = 0f;
        finalColor.b = 0f;
        text.color = finalColor;
    }
}
