using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;

public class S1PlayerScript : MonoBehaviourPunCallbacks
{
    // scene2에서의 거울조각 개수
    public int mirrorCount = 0;

    public float speed;

    Animator anim;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rigid;

    // Player 1과 Player 2의 입력을 따로 관리 (Scene2)
    private float horizontalInputPlayer1 = 0f;
    private float verticalInputPlayer1 = 0f;

    private float horizontalInputPlayer2 = 0f;
    private float verticalInputPlayer2 = 0f;

    // Scene1의 입력을 관리 (Scene1)
    private float horizontalInput = 0f;
    private float verticalInput = 0f;

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
        if (SceneManager.GetActiveScene().name == "Scene1")
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (player.CustomProperties.ContainsKey("Horizontal") &&
                    player.CustomProperties.ContainsKey("Vertical") &&
                    player.CustomProperties.ContainsKey("Tag"))
                {
                    float h = (float)player.CustomProperties["Horizontal"];
                    float v = (float)player.CustomProperties["Vertical"];
                    string tag = (string)player.CustomProperties["Tag"];

                    if (tag == "player1")
                    {
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
                    }
                }
            }
        }
        else if (SceneManager.GetActiveScene().name == "Scene2")
        {
            if (photonView.IsMine)
            {
                float localHorizontal = Input.GetAxisRaw("Horizontal");
                float localVertical = Input.GetAxisRaw("Vertical");

                Hashtable props = new Hashtable
                {
                    { "Horizontal", localHorizontal },
                    { "Vertical", localVertical }
                };
                PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            }

            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (player.CustomProperties.ContainsKey("Horizontal") &&
                    player.CustomProperties.ContainsKey("Vertical") &&
                    player.CustomProperties.ContainsKey("Tag"))
                {
                    float h = (float)player.CustomProperties["Horizontal"];
                    float v = (float)player.CustomProperties["Vertical"];
                    string tag = (string)player.CustomProperties["Tag"];

                    if (tag == "player1")
                    {
                        switch (mirrorCount)
                        {
                            case 0:
                                horizontalInputPlayer1 = 0f;
                                verticalInputPlayer1 = v;
                                break;
                            case 1:
                                horizontalInputPlayer1 = Mathf.Max(0f, h);
                                verticalInputPlayer1 = Mathf.Max(0f, v);
                                break;
                            case 2:
                                horizontalInputPlayer1 = Mathf.Max(0f, h);
                                verticalInputPlayer1 = Mathf.Min(0f, v);
                                break;
                            case 3:
                                horizontalInputPlayer1 = Mathf.Min(0f, h);
                                verticalInputPlayer1 = Mathf.Min(0f, v);
                                break;
                            case 4:
                                horizontalInputPlayer1 = Mathf.Min(0f, h);
                                verticalInputPlayer1 = Mathf.Max(0f, v);
                                break;
                            default:
                                horizontalInputPlayer1 = 0f;
                                verticalInputPlayer1 = 0f;
                                break;
                        }
                    }
                    else if (tag == "player2")
                    {
                        switch (mirrorCount)
                        {
                            case 0:
                                horizontalInputPlayer2 = h;
                                verticalInputPlayer2 = 0f;
                                break;
                            case 1:
                                horizontalInputPlayer2 = Mathf.Min(0f, h);
                                verticalInputPlayer2 = Mathf.Min(0f, v);
                                break;
                            case 2:
                                horizontalInputPlayer2 = Mathf.Min(0f, h);
                                verticalInputPlayer2 = Mathf.Max(0f, v);
                                break;
                            case 3:
                                horizontalInputPlayer2 = Mathf.Max(0f, h);
                                verticalInputPlayer2 = Mathf.Max(0f, v);
                                break;
                            case 4:
                                horizontalInputPlayer2 = Mathf.Max(0f, h);
                                verticalInputPlayer2 = Mathf.Min(0f, v);
                                break;
                            default:
                                horizontalInputPlayer2 = 0f;
                                verticalInputPlayer2 = 0f;
                                break;
                        }
                    }
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (isFalling) return;

        Vector2 moveVec;
        if (SceneManager.GetActiveScene().name == "Scene1")
        {
            moveVec = new Vector2(horizontalInput, verticalInput) * speed;
        }
        else if (SceneManager.GetActiveScene().name == "Scene2")
        {
            moveVec = new Vector2(horizontalInputPlayer1 + horizontalInputPlayer2, verticalInputPlayer1 + verticalInputPlayer2) * speed;
        }
        else
        {
            moveVec = Vector2.zero;
        }

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
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel("MainScene");
            }
        }

        if (other.gameObject.tag == "MirrorPiece")
        {
            mirrorCount++;
            Debug.Log("MirrorPiece 개수 : " + mirrorCount);
        }
    }

    System.Collections.IEnumerator FallAndRespawn()
    {
        isFalling = true;
        rigid.velocity = Vector2.zero;

        float fallDuration = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < fallDuration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainScene")
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
