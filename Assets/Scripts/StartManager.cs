using System.Collections;
using UnityEngine;
using UnityEngine.UI; // **Image 컴포넌트를 사용하기 위해 추가**
using UnityEngine.SceneManagement; // **씬 전환을 위해 추가**

public class StartManager : MonoBehaviour
{
    public Image panelImage; // **패널의 Image 컴포넌트 연결**
    public Button lobbyButton; // **LobbyScene으로 이동하는 버튼 연결**
    public float duration = 5f; // **색상 변화에 걸리는 시간**
    public float fadeDuration = 3f; // **LobbyScene으로 전환할 때의 페이드 아웃 시간**

    private bool isColorChanging = false; // **색상 변경 중인지 확인하는 플래그**

    private void Start()
    {
        // **코루틴 시작**
        StartCoroutine(ChangePanelColor(new Color(0f, 0f, 0f), new Color(200f / 255f, 200f / 255f, 200f / 255f), duration));
    }

    /// <summary>
    /// **패널의 색상을 서서히 변경하는 코루틴**
    /// </summary>
    /// <param name="startColor">시작 색상</param>
    /// <param name="endColor">끝 색상</param>
    /// <param name="duration">변경에 걸리는 시간(초)</param>
    /// <returns></returns>
    private IEnumerator ChangePanelColor(Color startColor, Color endColor, float duration)
    {
        isColorChanging = true; // **색상 변경 중임을 알림**
        lobbyButton.interactable = false; // **버튼 비활성화**

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            // **현재 색상을 Lerp로 계산**
            panelImage.color = Color.Lerp(startColor, endColor, elapsedTime / duration);
            yield return null; // **다음 프레임까지 대기**
        }

        // **최종 색상 보정**
        panelImage.color = endColor;

        isColorChanging = false; // **색상 변경 완료**
        lobbyButton.interactable = true; // **버튼 활성화**
    }

    /// <summary>
    /// **LobbyScene으로 이동하는 함수**
    /// </summary>
    public void GoToLobbyScene()
    {
        // **색상 변경 중이라면 버튼 클릭을 무시**
        if (isColorChanging)
        {
            Debug.Log("색상 변경 중에는 버튼을 누를 수 없습니다.");
            return;
        }

        StartCoroutine(LoadLobbySceneWithFade());
    }

    /// <summary>
    /// **페이드 아웃 후 LobbyScene으로 전환**
    /// </summary>
    private IEnumerator LoadLobbySceneWithFade()
    {
        float elapsedTime = 0f;

        // **페이드 아웃 (색상을 어둡게)**
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            panelImage.color = Color.Lerp(new Color(200f / 255f, 200f / 255f, 200f / 255f), new Color(0f, 0f, 0f), elapsedTime / fadeDuration);
            yield return null;
        }

        // **씬 전환 (LobbyScene)**
        SceneManager.LoadScene("LobbyScene");
    }
}
