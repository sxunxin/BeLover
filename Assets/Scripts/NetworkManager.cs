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

    [SerializeField]
    private byte maxPlayers = 2;

    void Awake()
    {
        Screen.SetResolution(960, 540, false);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;

        RoomPanel.SetActive(false);
    }
    public void Connect()
    {
        // 닉네임이 입력되지 않았다면, 아무 동작도 하지 않음
        if (string.IsNullOrEmpty(NickNameInput.text))
        {
            Debug.LogWarning("닉네임을 입력하세요.");
            NickError.text = "닉네임을 입력하세요.";
            Invoke("ClearText", 3f);
            return;
        }

        // 닉네임을 미리 설정
        PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;

        DisconnectPanel.SetActive(false);
        RoomPanel.SetActive(true);

        MyNick.text = PhotonNetwork.LocalPlayer.NickName;
        MyNick.color = Color.green;
        OtherNick.color = Color.red;

        // 현재 방에 다른 플레이어가 있다면 그 플레이어의 닉네임을 가져옵니다.
        if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (player != PhotonNetwork.LocalPlayer)
                {
                    OtherNick.text = player.NickName;
                    break;
                }
            }
        }
        else
        {
            OtherNick.text = "Waiting for other player...";
        }
    }
    public void GameStart()
    {
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
    }
    public override void OnConnectedToMaster()
    {
        RoomOptions roomOptions = new RoomOptions { MaxPlayers = maxPlayers };
        PhotonNetwork.JoinOrCreateRoom("Room", roomOptions, null);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        DisconnectPanel.SetActive(true);
    }
    void ClearText()
    {
        if (NickError != null)
            NickError.text = "";

        if (StartError != null)
            StartError.text = "";
    }
}