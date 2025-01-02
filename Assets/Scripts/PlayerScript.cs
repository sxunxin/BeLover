using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviourPunCallbacks
{
    public MainSceneManager Msm;

    [SerializeField]
    private float speed;
    public S3SceneManager S3sm; //s3

    [SerializeField] private int minSortingOrder = 10; // Y 값이 -500일 때의 최소 레이어
    [SerializeField] private int maxSortingOrder = 1000000; // Y 값이 500일 때의 최대 레이어
    [SerializeField] private float offsetMultiplier = 5f;
    private float minY = -500f; // Y 좌표 최소값
    private float maxY = 500f;  // Y 좌표 최대값

    public float h;
    public float v;

    bool isHorizonMove;
    bool isVerticalMove;

    string playerTag;

    Rigidbody2D rd;
    Animator anim;
    SpriteRenderer spriteRenderer;
    private AudioSource StoryPanelAudio; // AudioSource 변수 추가
    public PhotonView pv;
    public Text nicknameText;

    private float horizontalInput = 0f;
    private float verticalInput = 0f;

    bool isStoryUsing = false;

    //s3 raycast
    Vector3 dirVec;//바라보고 있는 방향
    GameObject scanObject;
    GameObject portalObject;
    private GameObject currentRoad; // Player2의 현재 Road 상태
    
    //s3 gimmick
    public int candlecount;

    public bool S3Interact = true;

    //s4
    S4Manager s4manager;

    //s5
    public int candles = 20;
    public int mirror = 5;
    public int bridge = 3;
    bool isCandleUsing = false;
    bool isMirrorUsing = false;
    S5Manager s5manager;

    //ending
    public bool isEndCandleUsing = false;
    public bool isEndP1ing = false;
    public int playerEndCandleCnt = 0;

    // 모바일 입력
    GameObject mobileSetting;

    public bool isUI;

    int up_Value;
    int down_Value;
    int left_Value;
    int right_Value;

    bool up_down;
    bool down_down;
    bool left_down;
    bool right_down;
    bool up_up;
    bool down_up;
    bool left_up;
    bool right_up;

    bool isInteract;
    bool isMobileInteract;

    void Awake()
    {
        rd = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        S3sm = FindObjectOfType<S3SceneManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        DontDestroyOnLoad(this.gameObject);

        nicknameText.text = pv.IsMine ? PhotonNetwork.NickName : pv.Owner.NickName;
        nicknameText.color = pv.IsMine ? Color.green : Color.red;
    }
    void Start()
    {
        if(Msm != null)
        {
            StoryPanelAudio = Msm.StoryPanel.GetComponent<AudioSource>(); // AudioSource 할당
        }
    }
    void Update()
    {
        if (pv.IsMine) // 자신의 플레이어만 동작
        {

            if (Msm != null && Msm.StoryPanel.activeSelf)
            {
                if (mobileSetting != null) mobileSetting.SetActive(false);
                
                if (StoryPanelAudio != null)
                {
                    StoryPanelAudio.Play();
                }
                //  플레이어의 이동을 정지시킴
                rd.velocity = Vector2.zero; // 움직임 정지
                return; //  더 이상 코드 실행 중지
            }
            if (Msm != null && Msm.StoryPanel1.activeSelf)
            {
                if (mobileSetting != null) mobileSetting.SetActive(false);

                if (CompareTag("player2")) anim.Play("Female_Down_Idle");
                else anim.Play("Male_Down_Idle");
                pv.RPC("SyncAnimationIdle", RpcTarget.Others);
                rd.velocity = Vector2.zero; // 움직임 정지
                return; //  더 이상 코드 실행 중지
            }
            if (S3sm != null && S3sm.storyPanel.activeSelf)
            {
                if (mobileSetting != null) mobileSetting.SetActive(false);

                rd.velocity = Vector2.zero; // 움직임 정지
                return; //  더 이상 코드 실행 중지
            }
            if (S3sm != null && S3sm.endPanel.activeSelf)
            {
                if (mobileSetting != null) mobileSetting.SetActive(false);

                if (CompareTag("player2")) anim.Play("Female_Up_Idle");
                else anim.Play("Male_Up_Idle");
                pv.RPC("SyncAnimationIdle", RpcTarget.Others);
                rd.velocity = Vector2.zero; // 움직임 정지
                return; //  더 이상 코드 실행 중지
            }
            if (mobileSetting != null)
            {
                if (isUI) mobileSetting.SetActive(true);
                else mobileSetting.SetActive(false);
            }

            // 입력값 처리 (PC & 모바일 통합)
            h = Input.GetAxisRaw("Horizontal") + right_Value + left_Value;
            v = Input.GetAxisRaw("Vertical") + up_Value + down_Value;

            bool hDown = Input.GetButtonDown("Horizontal") || right_down || left_down;
            bool vDown = Input.GetButtonDown("Vertical") || up_down || down_down;
            bool hUp = Input.GetButtonUp("Horizontal") || right_up || left_up;
            bool vUp = Input.GetButtonUp("Vertical") || up_up || down_up;

            // 상호작용 처리 (PC & 모바일 통합)
            isInteract = Input.GetButtonDown("Jump") || isMobileInteract;

            if (h != 0f && v == 0f)
            {
                horizontalInput = h;
                verticalInput = 0f;
            }
            else if (v != 0f && h == 0f)
            {
                verticalInput = v;
                horizontalInput = 0f;
            }
            else if (h == 0f && v == 0f)
            {
                horizontalInput = 0f;
                verticalInput = 0f;
            }

            // 태그 설정
            playerTag = gameObject.CompareTag("player1") ? "player1" : "player2";
            ExitGames.Client.Photon.Hashtable playerInput = new ExitGames.Client.Photon.Hashtable
            {
                { "Horizontal", h },
                { "Vertical", v },
                { "Tag", playerTag }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerInput);

            // Scene1일 때
            if (SceneManager.GetActiveScene().name == "Scene1" || SceneManager.GetActiveScene().name == "Scene2")
            {
                // 플레이어를 화면 밖으로 이동시켜서 보이지 않게 함
                transform.position = new Vector3(10000f, 10000f, 10000f);
            }
            else if (SceneManager.GetActiveScene().name == "Scene3-1")
            {
                SetDirection(hDown, vDown);
                if (scanObject != null)
                {
                    if (S3Interact && isInteract)
                    {
                        Debug.Log("S3 Interact 실행");
                        StartCoroutine(S3HandleInteraction());
                    }
                }  
            }
            else if (SceneManager.GetActiveScene().name == "MainScene")
            {
                SetDirection(hDown, vDown);

                if (scanObject != null && scanObject.tag == "MainMission")
                {
                    if (isInteract && !isStoryUsing)
                    {
                        StartCoroutine(HandleMissionSelectionWithDelay(0.2f, scanObject.name));
                        isStoryUsing = true;
                    }
                    
                }
                else if (scanObject != null)
                {
                    if (isInteract && !isStoryUsing)
                    {
                        StartCoroutine(HandleMissionWithDelay(0.2f, scanObject.name));
                        isStoryUsing = true;
                    }
                }
            }
            else if (SceneManager.GetActiveScene().name == "Scene4")
            {
                SetDirection(hDown, vDown);
                if (CompareTag("player2"))
                {
                    GameObject blindObject = GameObject.FindGameObjectWithTag("Blind");
                    if (blindObject != null)
                    {
                        blindObject.SetActive(false);
                    }
                }
                if (scanObject != null && scanObject.name == "ResetStatue")
                {
                    s4manager.ShowImage("ResetStatue");
                    if (isInteract)
                    {
                        s4manager.ResetButtons();
                    }
                }
                else
                {
                    s4manager.ShowImage("None");
                }
                
            }
            else if (SceneManager.GetActiveScene().name == "Scene5")
            {
                SetDirection(hDown, vDown);

                if (CompareTag("player1"))
                {
                    if (scanObject != null && scanObject.name == "mirrorNeed")
                    {
                        s5manager.ShowImage("mirrorNeed");
                        if (isInteract && mirror > 0)
                        {
                            isEndP1ing = true;
                            s5manager.ExecuteRPCEndMirror();
                        }
                    }
                    else if (scanObject != null && scanObject.name == "bridgeNeed")
                    {
                        s5manager.ShowImage("bridgeNeed");
                        if (isInteract && bridge > 0)
                        {
                            isEndP1ing = true;
                            s5manager.ExecuteRPCEndBridge();
                        }
                    }
                    else if (scanObject != null && scanObject.name.StartsWith("BridgeGenerator"))
                    {
                        s5manager.ShowImage("Bridge");

                        if (isInteract && bridge > 0)
                        {
                            Debug.Log(scanObject.name);
                            string targetBridgeName = scanObject.name.Replace("Generator", "");
                            s5manager.ExecuteRPC(targetBridgeName);
                        }
                    }
                    else
                    {
                        s5manager.ShowImage("Mirror");
                        if (!isEndP1ing && !isMirrorUsing && isInteract && mirror > 0)
                        {
                            StartCoroutine(UseMirror());
                        }
                    }
                    
                }

                if (CompareTag("player2"))
                {
                    s5manager.ShowImage("Candle");
                    if (scanObject != null && scanObject.name.StartsWith("candleNeed"))
                    {
                        s5manager.ShowImage("candleNeed");

                       if (!isEndCandleUsing && isInteract && candles > 0)
                       {
                            string objectName = scanObject.name; 
                            string numberPart = objectName.Substring("candleNeed".Length); 
                            int candleNumber;

                            if (int.TryParse(numberPart, out candleNumber)) 
                            {
                                StartCoroutine(UseEndCandle(candleNumber)); 
                            }
                        }
                    }
                    else if (!isEndCandleUsing && !isCandleUsing && isInteract && candles > 0)
                    {
                        StartCoroutine(UseCandle());
                    }
                }
            }

            // 모바일 초기화
            up_down = false;
            down_down = false;
            left_down = false;
            right_down = false;
            up_up = false;
            down_up = false;
            left_up = false;
            right_up = false;

        }
    }

    void LateUpdate()
    {

        if (SceneManager.GetActiveScene().name == "MainScene")
        {
            // Y 좌표를 sortingOrder 범위로 매핑
            float normalizedY = Mathf.InverseLerp(minY, maxY, transform.position.y);
            float lerpedOrder = Mathf.Lerp(maxSortingOrder, minSortingOrder, normalizedY) * offsetMultiplier;
            int calculatedOrder = Mathf.RoundToInt(lerpedOrder);

            // 정수로 변환하여 설정
            spriteRenderer.sortingOrder = Mathf.RoundToInt(calculatedOrder);
        }
        else
        {
            spriteRenderer.sortingOrder = 10;
        }
    }


    void FixedUpdate()
    {
        if (pv.IsMine)
        {
            Vector2 moveVec = new Vector2(horizontalInput, verticalInput);
            rd.velocity = moveVec * speed;

            anim.SetInteger("hAxisRaw", (int)moveVec.x);
            anim.SetInteger("vAxisRaw", (int)moveVec.y);

            pv.RPC("SyncAnimation", RpcTarget.Others, (int)horizontalInput, (int)verticalInput);

            // Ray 시작 위치를 더 오른쪽으로 이동
            Vector2 rayStartPos = rd.position + new Vector2(0.2f, -0.3f);

            // 디버그 선 추가
            Debug.DrawRay(rayStartPos, dirVec * 0.5f, Color.red);

            // Ray를 발사
            RaycastHit2D rayHit = Physics2D.Raycast(
                rayStartPos, // 시작 위치
                dirVec,      // 방향
                0.5f,        // 길이
                LayerMask.GetMask("Object") // Object 레이어만 탐색
            );

            if (rayHit.collider != null)
            {
                // 새로운 오브젝트를 스캔
                scanObject = rayHit.collider.gameObject;
            }
            else
            {
                // Raycast가 아무것도 감지하지 못한 경우
                scanObject = null;
            }
        }
    }

    private IEnumerator S3HandleInteraction()
    {
        // S3Interact = false;

        S3sm.Action(scanObject);
        S3ObjectData objData = scanObject.GetComponent<S3ObjectData>();

        if (scanObject.name == "Skull_True")
        {
            S3sm.Action(scanObject);
            Debug.Log("Player 2가 Skull_True 오브젝트와 상호작용했습니다. 클리어 화면을 모든 클라이언트에 표시합니다.");
            S3sm.photonView.RPC("ShowClearUI_RPC", RpcTarget.All, 2);
        }

        if (scanObject.CompareTag("Candle"))
        {
            if (objData != null && objData.isActivated)
            {
                Debug.Log($"이미 활성화된 {scanObject.name}과는 더 이상 상호작용할 수 없습니다.");
                yield break;
            }

            if (objData != null && objData.linkedCandleFire != null)
            {
                objData.linkedCandleFire.SetActive(true);
                Debug.Log($"{scanObject.name}의 CandleFire 활성화 완료");
            }
            else
            {
                Debug.LogWarning($"CandleFire가 {scanObject.name}와 연결되지 않았습니다.");
                yield break;
            }

            objData.isActivated = true;
            candlecount++;
            Debug.Log($"캔들 카운트 증가: {candlecount}");
            S3sm.CheckStageClear(candlecount);
        }

        yield return new WaitForSeconds(0.5f); // 0.2초 대기
        S3Interact = true; // 상호작용 다시 활성화
    }


    private IEnumerator HandleMissionSelectionWithDelay(float delay, string objectName)
    {
        yield return new WaitForSeconds(delay); // 0.2초 대기

        NetworkManager nm = FindAnyObjectByType<NetworkManager>();
        HandleMissionSelection(objectName, nm); // 0.2초 뒤에 실행
    }

    private IEnumerator HandleMissionWithDelay(float delay, string objectName)
    {
        yield return new WaitForSeconds(delay); // 0.5초 대기

        // HandleMissionClearCheck 호출
        HandleMissionClearCheck(objectName);

        // 3초 후 StoryPanel 비활성화
        StartCoroutine(DisableStoryPanelAfterDelay(3f));
    }

    private IEnumerator DisableStoryPanelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // 3초 대기
        if (Msm != null && Msm.StoryPanel != null)
        {
            isInteract = false;
            isStoryUsing = false;
            Msm.StoryPanel.SetActive(false); // StoryPanel 비활성화
        }
        else
        {
            Debug.LogWarning("Msm 또는 StoryPanel이 설정되지 않았습니다.");
        }
    }

    [PunRPC]
    void SyncAnimation(int hInput, int vInput)
    {
        anim.SetInteger("hAxisRaw", hInput);
        anim.SetInteger("vAxisRaw", vInput);
    }

    [PunRPC]
    void SyncAnimationIdle()
    {
        if (CompareTag("player2")) anim.Play("Female_Down_Idle");
        else anim.Play("Male_Down_Idle");
    }

    private IEnumerator UseMirror()
    {
        isMirrorUsing = true; 

        mirror--;
        photonView.RPC("UpdateMirrorUI", RpcTarget.All, mirror);

        GameObject boss = GameObject.FindWithTag("Boss"); // Enemy 태그로 적을 찾음
        if (boss != null)
        {
            PhotonView bossPhotonView = boss.GetComponent<PhotonView>();
            if (bossPhotonView != null)
            {
                bossPhotonView.RPC("TriggerStopAndResetSpeed", RpcTarget.All); // PunRPC 호출
            }
        }

        yield return new WaitForSeconds(1.1f);

        isMirrorUsing = false;
    }

    private IEnumerator UseCandle()
    {
        isCandleUsing = true; // 촛불 사용 중으로 설정

        // 촛불 감소
        candles--;
        Debug.Log($"candles : {candles}");

        // BlindPanelEffect의 코루틴 호출 (PUN RPC 사용)
        photonView.RPC("TriggerBlindEffect", RpcTarget.All);
        photonView.RPC("UpdateCandleUI", RpcTarget.All, candles);

        // 작업 완료 후 약간의 대기 시간 추가
        yield return new WaitForSeconds(10.2f);

        // 플래그 해제
        isCandleUsing = false;
    }

    private IEnumerator UseEndCandle(int cnt)
    {
        isEndCandleUsing = true; // 촛불 사용 중으로 설정

        // 촛불 감소
        candles--;
        playerEndCandleCnt++;
        Debug.Log($"candles : {candles}");
        Debug.Log($"EndCandleCnt : {playerEndCandleCnt}");

        // BlindPanelEffect의 코루틴 호출 (PUN RPC 사용)
        photonView.RPC("UpdateCandleUI", RpcTarget.All, candles);
        photonView.RPC("TriggerEndCandleProcess", RpcTarget.All, cnt);

        if (playerEndCandleCnt >= 5)
        {
            photonView.RPC("BlindEnd", RpcTarget.All);
        }

        // 작업 완료 후 약간의 대기 시간 추가
        yield return new WaitForSeconds(1.0f);

        // 플래그 해제
        isEndCandleUsing = false;
    }

    [PunRPC]
    public void BlindEnd()
    {
        s5manager.EndCandle();
        BlindPanelFollow blindPanelFollow = FindObjectOfType<BlindPanelFollow>();
        blindPanelFollow.ScaleAndDeactivate();
    }

    [PunRPC]
    public void TriggerEndCandleProcess(int cnt)
    {
        if (s5manager != null)
        {
            // S5Manager의 ProcessCandle 메서드 호출
            s5manager.ProcessEndCandle(cnt);
        }
        else
        {
            Debug.LogWarning("S5Manager not found!");
        }
    }

    [PunRPC]
    private void TriggerBlindEffect()
    {
        // BlindPanelEffect 스크립트에서 코루틴 실행
        BlindPanelFollow blindPanel = FindObjectOfType<BlindPanelFollow>();
        if (blindPanel != null)
        {
            blindPanel.StartCoroutine(blindPanel.ScaleBlindEffect());
        }
        else
        {
            Debug.LogError("BlindPanelFollow 스크립트를 찾을 수 없습니다.");
        }
    }

    [PunRPC]
    private void UpdateCandleUI(int candleCount)
    {
        if (s5manager != null)
        {
            s5manager.UpdateCandleUI(candleCount); // S5Manager에 UI 업데이트 요청
        }
    }

    [PunRPC]
    private void UpdateMirrorUI(int mirrorCount)
    {
        if (s5manager != null)
        {
            s5manager.UpdateMirrorUI(mirrorCount); // S5Manager에 UI 업데이트 요청
        }
    }

    void HandleMissionSelection(string missionName, NetworkManager nm)
    {
        // UI 패널에 메시지 표시
        if (missionName == "MainMission1")
        {
            Msm.StoryPanel.SetActive(true);
            Msm.StoryText.text = "거울의 방";
            Msm.ghostImage.sprite = Msm.GhostImage[0];
            Msm.ghostImage.color = new Color(1f, 1f, 1f);
        }
        else if (missionName == "MainMission2")
        {
            Msm.StoryPanel.SetActive(true);
            Msm.StoryText.text = "분리의 방";
            Msm.ghostImage.sprite = Msm.GhostImage[1];
            Msm.ghostImage.color = new Color(1f, 1f, 1f);
        }
        else if (missionName == "MainMission3")
        {
            Msm.StoryPanel.SetActive(true);
            Msm.StoryText.text = "어둠의 방";
            Msm.ghostImage.sprite = Msm.GhostImage[2];
            Msm.ghostImage.color = new Color(1f, 1f, 1f);
        }

        // 선택한 미션 정보를 Photon CustomProperties에 저장
        ExitGames.Client.Photon.Hashtable playerSelection = new ExitGames.Client.Photon.Hashtable
        {
            { "SelectedMission", missionName }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerSelection);

        // 모든 플레이어의 선택 정보 확인 (코루틴으로 확인)
        StartCoroutine(CheckBothPlayersMissionSelection(missionName, nm));
    }

    void HandleMissionClearCheck(string missionName)
    {
        if (missionName == "MainMission1" && GameManager.Instance.isMission1Clear == true)
        {
            Msm.StoryPanel.SetActive(true);
            Msm.ghostImage.color = new Color(1f, 1f, 1f);
            Msm.ghostImage.sprite = Msm.GhostImage[0];
            Msm.StoryText.text = "이미 성불한 방이다";
        }

        if (missionName == "MainMission2" && GameManager.Instance.isMission2Clear == true)
        {
            Msm.StoryPanel.SetActive(true);
            Msm.ghostImage.color = new Color(1f, 1f, 1f);
            Msm.ghostImage.sprite = Msm.GhostImage[1];
            Msm.StoryText.text = "이미 성불한 방이다";
        }

        if (GameManager.Instance.isMission2Clear == false)
        {
            if (missionName == "MainMission2" || missionName == "MainMission3")
            {
                Msm.StoryPanel.SetActive(true);
                Msm.StoryText.text = "거울의 방부터 성불해라";
                if(missionName == "MainMission2")
                {
                    Msm.ghostImage.sprite = Msm.GhostImage[1];
                    Msm.ghostImage.color = new Color(0f, 0f, 0f);
                }
                else if(missionName == "MainMission3")
                {
                    Msm.ghostImage.sprite = Msm.GhostImage[2];
                    Msm.ghostImage.color = new Color(0f, 0f, 0f);
                }
            }
        }

        if (GameManager.Instance.isMission3Clear == false && missionName == "MainMission3")
        {
            Msm.StoryPanel.SetActive(true);
            Msm.StoryText.text = "분리의 방부터 성불해라";
            Msm.ghostImage.sprite = Msm.GhostImage[2];
            Msm.ghostImage.color = new Color(0f, 0f, 0f);
        }
    }

    IEnumerator CheckBothPlayersMissionSelection(string missionName, NetworkManager nm)
    {
        float elapsedTime = 0f;
        bool missionMatched = false;

        while (elapsedTime < 3f) // 3초 동안 매 0.5초마다 확인
        {
            yield return new WaitForSeconds(1.5f);
            elapsedTime += 1.5f;

            // 모든 플레이어의 선택한 미션 정보를 가져옴
            string player1Mission = "";
            string player2Mission = "";

            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (player.CustomProperties.ContainsKey("SelectedMission"))
                {
                    string selectedMission = player.CustomProperties["SelectedMission"].ToString();
                    if (player.IsLocal)
                        player1Mission = selectedMission;
                    else
                        player2Mission = selectedMission;
                }
            }

            // 두 플레이어의 선택이 같은지 확인
            if (!string.IsNullOrEmpty(player1Mission) && player1Mission == player2Mission)
            {
                Debug.Log($"두 플레이어가 같은 미션을 선택했습니다: {missionName}");

                // 카운트다운 시작
                StartCoroutine(StartCountdownAndChangeScene(missionName, nm));
                missionMatched = true;
                break; // 코루틴 종료
            }
        }

        if (!missionMatched)
        {
            Debug.LogWarning("두 플레이어의 선택이 일치하지 않았습니다.");
        }
    }

    IEnumerator StartCountdownAndChangeScene(string missionName, NetworkManager nm)
    {
        string mapName = "알 수 없는 방";

        if (missionName == "MainMission1")
        {
            mapName = "거울의 방";
        }
        else if (missionName == "MainMission2")
        {
            mapName = "분리의 방";
        }
        else if (missionName == "MainMission3")
        {
            mapName = "어둠의 방";
        }
        int countdown = 3; // 카운트다운 시작 숫자

        while (countdown > 0)
        {
            // RPC를 통해 모든 클라이언트에 카운트다운 동기화
            pv.RPC("UpdateCountdownText", RpcTarget.All, mapName, countdown);

            yield return new WaitForSeconds(1f);
            countdown--;
        }

        // 씬 변경
        if (missionName == "MainMission1")
        {
            nm.photonView.RPC("RPC_StartScene2", RpcTarget.All);
        }
        else if (missionName == "MainMission2")
        {
            nm.photonView.RPC("RPC_StartScene3", RpcTarget.All);
        }
        else if (missionName == "MainMission3")
        {
            nm.photonView.RPC("RPC_StartScene4", RpcTarget.All);
        }
    }

    [PunRPC]
    void UpdateCountdownText(string mapName, int countdown)
    {
        if (Msm != null && Msm.countDown != null)
        {
            Msm.countDown.text = $"{mapName}에 입장합니다\n\n{countdown}";
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Goal")
        {
            if (PhotonNetwork.IsMasterClient)
            {
                s4manager.photonView.RPC("ShowEndPanel_RPC", RpcTarget.All); // 모든 클라이언트에서 패널 활성화
            }
        }
        if (collision.gameObject.tag == "Boss")
        {
            photonView.RPC("RPC_StartScene5", RpcTarget.All);
        }
        if (collision.gameObject.name == "M2_2Spawn")
        {
            Debug.Log("플레이어가 M2_2Spawn에 도착하여 속도가 정상화됩니다.");
            pv.RPC("SetSpeedRPC", RpcTarget.All, 1f); // 모든 클라이언트에서 속도 동기화
        }
        if (collision.CompareTag("Portal"))
        {
            // Portal과의 충돌 로직 추가
            S3Portal portal = collision.GetComponent<S3Portal>();
            if (portal != null)
            {
                portal.OnPlayerEnter(gameObject);
            }
            // **M1_2Portal과 충돌했을 때만 클리어 화면을 표시**
            if (collision.gameObject.name == "M1_2Portal" && CompareTag("player2"))
            {
                Debug.Log("Player 2가 M1_2Portal에 접촉했습니다. 클리어 화면을 모든 클라이언트에 표시합니다.");
                S3sm.photonView.RPC("ShowClearUI_RPC", RpcTarget.All, 1); // 조건 1번
            }

        }
        if (collision.gameObject.tag == "BossStage")
        {
            Debug.Log($"{tag}이 BossStage에 접촉했습니다.");

            // Photon Custom Properties에 접촉 상태 저장
            ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable
        {
            { "IsBossStageTouched", true }
        };
            PhotonNetwork.LocalPlayer.SetCustomProperties(properties);

            // 두 플레이어가 모두 접촉했는지 확인
            CheckBothPlayersTouched();
        }
        string objectName = collision.gameObject.name;
        if (CompareTag("player2") && objectName.StartsWith("Road"))
        {
            Debug.Log($"Player 22 {objectName}과 상호작용 시작.");

            ExitGames.Client.Photon.Hashtable player2Properties = new ExitGames.Client.Photon.Hashtable
            {
                { "Player2Road", objectName }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(player2Properties);

            S3SceneManager.Instance.CompareButtonAndRoad();
        }


        if (collision.CompareTag("Bridge"))
        {
            Debug.Log("지나갈 수 없어 보인다...");
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        string objectName = collision.gameObject.name;
        // Player 1과 Button 상호작용
        if (CompareTag("player1") && objectName.StartsWith("Button"))
        {
            Debug.Log($"Player 1이 {objectName}과 지속적으로 상호작용 중.");

            ExitGames.Client.Photon.Hashtable player1Properties = new ExitGames.Client.Photon.Hashtable
            {
                { "Player1Button", objectName }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(player1Properties);
        }
        // Player 2와 Road 상호작용
        if (CompareTag("player2") && objectName.StartsWith("Road"))
        {
            Debug.Log($"Player 22 {objectName}과 지속적으로 상호작용 중.");

            ExitGames.Client.Photon.Hashtable player2Properties = new ExitGames.Client.Photon.Hashtable
            {
                { "Player2Road", objectName }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(player2Properties);

            S3SceneManager.Instance.CompareButtonAndRoad();
        }


    }

    private void SetDirection(bool hDown, bool vDown)
    {
        if (vDown && v == 1) dirVec = Vector3.up;
        else if (vDown && v == -1) dirVec = Vector3.down;
        else if (hDown && h == -1) dirVec = Vector3.left;
        else if (hDown && h == 1) dirVec = Vector3.right;
    }
    new void OnEnable()
    {
        // SceneManager의 sceneLoaded에 OnSceneLoaded 연결
        SceneManager.sceneLoaded += OnSceneLoaded;
        if (pv.IsMine)
        {
            AttachCameraToPlayer(); // 씬이 전환되었을 때 카메라 타겟 다시 연결
        }
    }

    new void OnDisable()
    {
        // 장면이 전환될 때 메서드 호출을 중지하기 위해 콜백 제거
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 특정 장면이 로드될 때 호출할 함수
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isUI = true;
        mobileSetting = GameObject.Find("MobileSetting");

        if (pv.IsMine)
        {
            AttachCameraToPlayer(); // 씬이 전환되었을 때 카메라 타겟 다시 연결
        }
        if (scene.name == "MainScene")
        {
            isStoryUsing = false;

            horizontalInput = 0f;
            verticalInput = 0f;
            Vector2 moveVec = new Vector2(horizontalInput, verticalInput);
            rd.velocity = Vector2.zero;

            up_Value = 0;
            down_Value = 0;
            left_Value = 0;
            right_Value = 0;

            up_down = false;
            down_down = false;
            left_down = false;
            right_down = false;
            up_up = false;
            down_up = false;
            left_up = false;
            right_up = false;

            Msm = FindObjectOfType<MainSceneManager>(); // **씬이 로드될 때 Msm 다시 할당**
            if (Msm != null)
            {
                Debug.Log("MainSceneManager 연결 성공");
            }
            else
            {
                Debug.LogWarning("MainSceneManager를 찾을 수 없습니다.");
            }
            if (playerTag == "player1")
            {
                SetPosition(3.5f, -1f, 0f);
            }
            else if (playerTag == "player2")
            {
                SetPosition(2.8f, -1f, 0f);
            }
        }
        else if (scene.name == "Scene3-1")
        {
            // Scene3-1에서 S3SceneManager 찾기
            S3sm = FindObjectOfType<S3SceneManager>();
            if (S3sm != null)
            {
                Debug.Log("S3SceneManager가 Scene3-1에서 연결되었습니다.");
            }
            else
            {
                Debug.LogWarning("Scene3-1에 S3SceneManager 프리팹이 없습니다.");
            }

            if (playerTag == "player1")
            {
                SetPosition(-0.5f, 2f, 0f);
            }
            else if (playerTag == "player2")
            {
                SetPosition(3f, -2.5f, 0f);
            }

            if (playerTag == "player2")
            {
                SetSpeedRPC(0.5f); // Player2 속도를 느리게 설정
            }
        }
        else if (scene.name == "Scene4")
        {
            s4manager = FindObjectOfType<S4Manager>();
            if (playerTag == "player1")
            {
                SetPosition(-0.4f, 3.7f, 0f);
            }
            else if (playerTag == "player2")
            {
                SetPosition(-0.35f, -28.8f, 0f);
            }
        }
        else if (scene.name == "Scene5")
        {
            s5manager = FindObjectOfType<S5Manager>();
            candles = 20;
            mirror = 5;
            bridge = 3;
            isCandleUsing = false;
            isMirrorUsing = false;

            if (playerTag == "player1")
            {
                SetPosition(-0.4f, -0.9f, 0f);
            }
            else if (playerTag == "player2")
            {
                s5manager.ShowImage("Candle");
                SetPosition(-0.2f, -0.9f, 0f);
            }
        }
        else
        {
            // 다른 씬에서는 S3SceneManager 사용하지 않음
            S3sm = null;
        }
    }
    private void SetPosition(float x, float y, float z)
    {
        transform.position = new Vector3(x, y, z);
    }
    private void AttachCameraToPlayer()
    {
        // MainCamera의 S3Camera 스크립트에 이 플레이어의 Transform을 연결
        S3Camera mainCamera = Camera.main.GetComponent<S3Camera>();
        if (mainCamera != null)
        {
            mainCamera.SetTarget(transform);
            Debug.Log("카메라가 플레이어에 연결되었습니다: " + transform.name);
        }
        else
        {
            Debug.LogWarning("S3Camera를 찾을 수 없습니다.");
        }
    }
    [PunRPC]
    public void SetSpeedRPC(float newSpeed)
    {
        Debug.Log($"SetSpeedRPC 호출됨: {tag}, 속도: {newSpeed}");

        if (CompareTag("player2")) // Player 2만 속도 변경
        {
            speed = newSpeed;
            Debug.Log($"Player 2의 속도가 {newSpeed}로 설정되었습니다.");
        }
        else
        {
            Debug.Log($"SetSpeedRPC 호출됨: Player 1이므로 속도 변경 없음.");
        }
    }


    // 모바일 입력
    public void ButtonDown(string type, string sceneName)
    {
        if (!photonView.IsMine || SceneManager.GetActiveScene().name != sceneName) return;

        S3Interact = true;
        switch (type)
        {
            case "U":
                up_Value = 1;
                up_down = true;
                break;
            case "D":
                down_Value = -1;
                down_down = true;
                break;
            case "L":
                left_Value = -1;
                left_down = true;
                break;
            case "R":
                right_Value = 1;
                right_down = true;
                break;
        }

    }

    public void ButtonUp(string type, string sceneName)
    {
        if (!photonView.IsMine || SceneManager.GetActiveScene().name != sceneName) return;

        switch (type)
        {
            case "U":
                up_Value = 0;
                up_up = true;
                break;
            case "D":
                down_Value = 0;
                down_up = true;
                break;
            case "L":
                left_Value = 0;
                left_up = true;
                break;
            case "R":
                right_Value = 0;
                right_up = true;
                break;
        }
    }

    public void SetInteractState(bool state, string sceneName)
    {
        if (!photonView.IsMine || SceneManager.GetActiveScene().name != sceneName) return;
        isMobileInteract = state;
    }

    public void ResetInputValues()
    {
        up_Value = 0;
        down_Value = 0;
        left_Value = 0;
        right_Value = 0;

        up_down = false;
        down_down = false;
        left_down = false;
        right_down = false;

        up_up = false;
        down_up = false;
        left_up = false;
        right_up = false;

        isMobileInteract = false;
    }
    private void CheckBothPlayersTouched()
    {
        bool player1Touched = false;
        bool player2Touched = false;

        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.ContainsKey("IsBossStageTouched"))
            {
                if (player.CustomProperties["IsBossStageTouched"] is bool isTouched && isTouched)
                {
                    if (player.CustomProperties.ContainsKey("Tag") && player.CustomProperties["Tag"].ToString() == "player1")
                    {
                        player1Touched = true;
                    }
                    else if (player.CustomProperties.ContainsKey("Tag") && player.CustomProperties["Tag"].ToString() == "player2")
                    {
                        player2Touched = true;
                    }
                }
            }
        }

        // 두 플레이어가 모두 접촉했을 때 씬 전환
        if (player1Touched && player2Touched)
        {
            Debug.Log("두 플레이어가 BossStage에 모두 접촉했습니다. Scene5로 전환합니다.");
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel("Scene5");
            }
        }
    }

    [PunRPC]
    void RPC_StartScene5()
    {
        PhotonNetwork.LoadLevel("Scene5");
        DontDestroyOnLoad(this.gameObject);
    }

}