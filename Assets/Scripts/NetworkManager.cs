using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public InputField NickNameInput;
    public GameObject DisconnectPanel;
    public GameObject RoomPanel;
    public Text MyNick;
    public Text OtherNick;
    public Text NickError;
    public Text StartError;
    public bool isGameStart = false;
    public bool isConnect = false;
    public SpriteRenderer MaleSel;
    public SpriteRenderer FemaleSel;
    private bool isMaleSelected = false;
    private bool isFemaleSelected = false;

    private bool isCharSelectionLocked = false; // 캐릭터 선택 잠금

    [SerializeField]
    private byte maxPlayers = 2;

    private bool isMyCharSelected = false;
    private bool isOtherCharSelected = false;

    void Awake()
    {
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "kr";
        PhotonNetwork.ConnectUsingSettings();
        Screen.SetResolution(960, 540, false);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
        RoomPanel.SetActive(false);
        SetInitialCharacterColors();
    }

    void SetInitialCharacterColors()
    {
        Color maleColor = MaleSel.color;
        maleColor.a = 100 / 255f;
        MaleSel.color = maleColor;

        Color femaleColor = FemaleSel.color;
        femaleColor.a = 100 / 255f;
        FemaleSel.color = femaleColor;
    }

    public void Connect()
    {
        if (string.IsNullOrEmpty(NickNameInput.text))
        {
            Debug.LogWarning("닉네임을 입력하세요.");
            NickError.text = "닉네임을 입력하세요.";
            Invoke("ClearText", 3f);
            return;
        }

        // 닉네임 중복 확인
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.NickName == NickNameInput.text)
            {
                Debug.LogWarning("이미 사용 중인 닉네임입니다.");
                NickError.text = "이미 사용 중인 닉네임입니다.";
                Invoke("ClearText", 3f);
                return;
            }
        }

        PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
        DisconnectPanel.SetActive(false);
        RoomPanel.SetActive(true);

        MyNick.text = PhotonNetwork.LocalPlayer.NickName;
        MyNick.color = Color.green;
        OtherNick.color = Color.red;

        UpdateOtherPlayerNick();
    }

    public void GameStart()
    {
        if (!isMyCharSelected || !isOtherCharSelected)
        {
            Debug.LogWarning("두 명의 플레이어가 모두 캐릭터를 선택해야 합니다.");
            if (StartError != null)
            {
                StartError.text = "두 명의 플레이어가 모두 캐릭터를 선택해야 합니다.";
                Invoke("ClearText", 3f);
            }
            return;
        }

        if (PhotonNetwork.CurrentRoom == null || PhotonNetwork.CurrentRoom.PlayerCount != PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            Debug.LogWarning("2명의 플레이어가 준비가 돼야 합니다.");
            if (StartError != null)
            {
                StartError.text = "2명의 플레이어가 준비가 돼야 합니다.";
                Invoke("ClearText", 3f);
            }
            return;
        }

        RoomPanel.SetActive(false);
        isGameStart = true;
        Debug.Log("게임이 시작됩니다.");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined lobby, now creating or joining a room...");
        RoomOptions roomOptions = new RoomOptions { MaxPlayers = maxPlayers };
        PhotonNetwork.JoinOrCreateRoom("Room", roomOptions, TypedLobby.Default);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Failed to join or create room: {message}");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Successfully joined the room.");
        UpdateOtherPlayerNick();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("Player entered: " + newPlayer.NickName);
        UpdateOtherPlayerNick();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("Player left: " + otherPlayer.NickName);
        UpdateOtherPlayerNick();
        ResetCharacterSelection();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
        UpdateOtherPlayerNick();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        DisconnectPanel.SetActive(true);
        Debug.LogWarning($"Disconnected from Photon Server: {cause}");
    }

    void ClearText()
    {
        if (NickError != null)
            NickError.text = "";

        if (StartError != null)
            StartError.text = "";
    }

    void UpdateOtherPlayerNick()
    {
        if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (player != PhotonNetwork.LocalPlayer)
                {
                    OtherNick.text = player.NickName;
                    return;
                }
            }
        }
        else
        {
            OtherNick.text = "Waiting for other player...";
        }
    }

    public void SelectChar(string character)
    {
        if (MaleSel == null || FemaleSel == null)
        {
            Debug.LogError("MaleSel 또는 FemaleSel이 null입니다. Unity Editor에서 이 필드들이 제대로 연결되었는지 확인하세요.");
            return;
        }

        if (PhotonNetwork.CurrentRoom.PlayerCount < maxPlayers)
        {
            StartError.text = "두 명의 플레이어가 모두 입장해야 캐릭터를 선택할 수 있습니다.";
            Invoke("ClearText", 3f);
            return;
        }

        if (isGameStart || isCharSelectionLocked)
        {
            StartError.text = "당신은 이미 선택하였습니다.";
            Invoke("ClearText", 3f);
            return;
        }

        // 현재 캐릭터 선택 상태에 따라 처리
        if (character == "male")
        {
            if (!isMaleSelected || (isMaleSelected && !isOtherCharSelected))
            {
                isMyCharSelected = true;
                isMaleSelected = true;
                isFemaleSelected = false;
                isCharSelectionLocked = true;

                // 남성 캐릭터 선택
                Color maleColor = MaleSel.color;
                maleColor.a = 1f;
                MaleSel.color = maleColor;

                Color femaleColor = FemaleSel.color;
                femaleColor.a = 0.4f;
                FemaleSel.color = femaleColor;

                // 태그 설정
                gameObject.tag = "player1";

                PhotonView.Get(this).RPC("RPC_SelectChar", RpcTarget.Others, "male");
            }
            else
            {
                StartError.text = "이미 선택된 캐릭터입니다.";
                Invoke("ClearText", 3f);
            }
        }
        else if (character == "female")
        {
            if (!isFemaleSelected || (isFemaleSelected && !isOtherCharSelected))
            {
                isMyCharSelected = true;
                isFemaleSelected = true;
                isMaleSelected = false;
                isCharSelectionLocked = true;

                // 여성 캐릭터 선택
                Color femaleColor = FemaleSel.color;
                femaleColor.a = 1f;
                FemaleSel.color = femaleColor;

                Color maleColor = MaleSel.color;
                maleColor.a = 0.4f;
                MaleSel.color = maleColor;

                // 태그 설정
                gameObject.tag = "player2";

                PhotonView.Get(this).RPC("RPC_SelectChar", RpcTarget.Others, "female");
            }
            else
            {
                StartError.text = "이미 선택된 캐릭터입니다.";
                Invoke("ClearText", 3f);
            }
        }

        CheckIfBothSelected();
    }


    [PunRPC]
    void RPC_SelectChar(string character)
    {
        if (character == "male")
        {
            if (isMaleSelected && isOtherCharSelected)
            {
                StartError.text = "이미 선택된 캐릭터입니다.";
                Invoke("ClearText", 3f);
                return;
            }

            isOtherCharSelected = true;
            isMaleSelected = true;
            isFemaleSelected = false;

            Color maleColor = MaleSel.color;
            maleColor.a = 1f;
            MaleSel.color = maleColor;

            Color femaleColor = FemaleSel.color;
            femaleColor.a = 0.4f;
            FemaleSel.color = femaleColor;
        }
        else if (character == "female")
        {
            if (isFemaleSelected && isOtherCharSelected)
            {
                StartError.text = "이미 선택된 캐릭터입니다.";
                Invoke("ClearText", 3f);
                return;
            }

            isOtherCharSelected = true;
            isFemaleSelected = true;
            isMaleSelected = false;

            Color femaleColor = FemaleSel.color;
            femaleColor.a = 1f;
            FemaleSel.color = femaleColor;

            Color maleColor = MaleSel.color;
            maleColor.a = 0.4f;
            MaleSel.color = maleColor;
        }

        isCharSelectionLocked = false; // 선택 완료 후 잠금 해제

        CheckIfBothSelected();
    }

    void CheckIfBothSelected()
    {
        if (isMyCharSelected && isOtherCharSelected)
        {
            StartError.text = "모두 캐릭터를 선택했습니다. 게임을 시작할 수 있습니다.";

            // 두 캐릭터 모두 불투명하게 설정
            Color maleColor = MaleSel.color;
            maleColor.a = 1f;
            MaleSel.color = maleColor;

            Color femaleColor = FemaleSel.color;
            femaleColor.a = 1f;
            FemaleSel.color = femaleColor;
        }
    }

    void ResetCharacterSelection()
    {
        isMyCharSelected = false;
        isOtherCharSelected = false;
        isMaleSelected = false;
        isFemaleSelected = false;
        isCharSelectionLocked = false; // 선택 상태 초기화

        SetInitialCharacterColors();
    }
}
