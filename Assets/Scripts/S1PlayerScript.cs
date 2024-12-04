using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;

public class S1PlayerScript : MonoBehaviourPunCallbacks
{
    public float speed;

    Animator anim;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rigid;
    private float horizontalInput = 0f; // 수평 입력값
    private float verticalInput = 0f;   // 수직 입력값
    bool isFalling = false;

    void Awake()
    {
        DontDestroyOnLoad(gameObject); // 씬 전환 시 객체 유지
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            // Player의 CustomProperties에 값이 있는지 확인
            if (player.CustomProperties.ContainsKey("Horizontal") &&
                player.CustomProperties.ContainsKey("Vertical") &&
                player.CustomProperties.ContainsKey("Tag"))
            {
                float h = (float)player.CustomProperties["Horizontal"];
                float v = (float)player.CustomProperties["Vertical"];
                string tag = (string)player.CustomProperties["Tag"];

                if (tag == "player1")
                {
                    if (h != 0f && v == 0f)  // 수평 입력만 있을 때
                    {
                        horizontalInput = h;
                        verticalInput = 0f;  // 수직 입력을 0으로
                    }
                    else if (v != 0f && h == 0f)  // 수직 입력만 있을 때
                    {
                        verticalInput = v;
                        horizontalInput = 0f;  // 수평 입력을 0으로
                    }
                    else if (h == 0f && v == 0f)  // 아무 입력도 없을 때 (가만히 있을 때)
                    {
                        horizontalInput = 0f;
                        verticalInput = 0f;  // 가만히 있을 때는 입력을 0으로
                    }
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (isFalling) return;

        Vector2 moveVec = new Vector2(horizontalInput, verticalInput) * speed;
        rigid.velocity = moveVec;

        anim.SetInteger("hAxisRaw", (int)moveVec.x);
        anim.SetInteger("vAxisRaw", (int)moveVec.y);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "DeathZone" && !isFalling)
        {
            StartCoroutine(FallAndRespawn());
        }

        if (other.gameObject.tag == "Goal")
        {
            if (PhotonNetwork.IsMasterClient) // 마스터 클라이언트만 씬 전환 제어
            {
                PhotonNetwork.LoadLevel("MainScene");
            }
        }
    }

    System.Collections.IEnumerator FallAndRespawn()
    {
        isFalling = true;
        rigid.velocity = Vector2.zero;  // 이동 정지

        float fallDuration = 0.5f;  // 낙하 시간
        float elapsedTime = 0f;
        Vector3 initialPosition = transform.position; // 초기 위치

        while (elapsedTime < fallDuration)
        {
            elapsedTime += Time.deltaTime;

            transform.position = Vector3.Lerp(initialPosition, initialPosition + Vector3.down * 2, elapsedTime / fallDuration);
            transform.Rotate(Vector3.forward * 360 * Time.deltaTime);

            yield return null;
        }

        // 리스폰
        transform.position = Vector2.zero;
        transform.rotation = Quaternion.identity;

        isFalling = false;
    }

    new void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    new void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainScene")
        {
            Destroy(gameObject); // MainScene으로 전환 후 객체 삭제
        }
    }
}
