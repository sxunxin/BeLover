using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField]
    private float speed;

    float h;
    float v;

    bool isHorizonMove;
    bool isVerticalMove;

    bool hasSpawned = false; // 태어난 위치가 설정되었는지 확인

    string playerTag;

    Rigidbody2D rd;
    Animator anim;
    public PhotonView pv;
    public Text nicknameText;

    void Awake()
    {
        rd = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        DontDestroyOnLoad(this.gameObject);

        nicknameText.text = pv.IsMine ? PhotonNetwork.NickName : pv.Owner.NickName;
        nicknameText.color = pv.IsMine ? Color.green : Color.red;
    }

    void Update()
    {
        if (pv.IsMine) // 자신의 플레이어만 동작
        {
            // Scene1일 때
            if (SceneManager.GetActiveScene().name == "Scene1")
            {
                // 플레이어를 화면 밖으로 이동시켜서 보이지 않게 함
                transform.position = new Vector3(10000f, 10000f, 10000f);
            }

            // MainScene일 때
            else if (SceneManager.GetActiveScene().name == "MainScene" && !hasSpawned)
            {
                transform.position = new Vector3(0f, 0f, 0f); // 태어나는 위치
                hasSpawned = true; // 초기 위치 설정 완료
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

            // 입력값 처리
            h = Input.GetAxisRaw("Horizontal");
            v = Input.GetAxisRaw("Vertical");

            bool hDown = Input.GetButtonDown("Horizontal");
            bool vDown = Input.GetButtonDown("Vertical");
            bool hUp = Input.GetButtonUp("Horizontal");
            bool vUp = Input.GetButtonUp("Vertical");

            if (hDown)
                isHorizonMove = true;
            else if (vDown)
                isHorizonMove = false;
            else if (vUp || hUp)
                isHorizonMove = h != 0;

            // 애니메이션 상태 처리
            if (anim.GetInteger("hAxisRaw") != h)
            {
                anim.SetBool("isChange", true);
                anim.SetInteger("hAxisRaw", (int)h);
            }
            else if (anim.GetInteger("vAxisRaw") != v)
            {
                anim.SetBool("isChange", true);
                anim.SetInteger("vAxisRaw", (int)v);
            }
            else
                anim.SetBool("isChange", false);
        }
    }

    void FixedUpdate()
    {
        if (pv.IsMine) // 자신의 플레이어만 물리 업데이트
        {
            Vector2 moveVec = isHorizonMove ? new Vector2(h, 0) : new Vector2(0, v);
            rd.velocity = moveVec * speed;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // Photon 상태 동기화용 (필요 시 구현)
    }
}