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
        // �г����� �Էµ��� �ʾҴٸ�, �ƹ� ���۵� ���� ����
        if (string.IsNullOrEmpty(NickNameInput.text))
        {
            Debug.LogWarning("�г����� �Է��ϼ���.");
            return;
        }

        // �г����� �̸� ����
        PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;

        DisconnectPanel.SetActive(false);
        RoomPanel.SetActive(true);

        MyNick.text = PhotonNetwork.LocalPlayer.NickName;
        MyNick.color = Color.green;
        OtherNick.color = Color.red;

        // ���� �濡 �ٸ� �÷��̾ �ִٸ� �� �÷��̾��� �г����� �����ɴϴ�.
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
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            RoomPanel.SetActive(false);
        }
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
}