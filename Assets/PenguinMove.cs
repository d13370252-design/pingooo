using UnityEngine;
using System.Collections;
using TMPro;

public class PenguinMove : MonoBehaviour
{
    public float speed = 3f;
    public float rotateSpeed = 100f; 
    
    [Header("跳躍設定")]
    public float jumpHeight = 1.5f;   // 企鵝能跳多高（公尺）
    public float gravity = -9.81f;    // 地球標準重力
    private Vector3 playerVelocity;    // 用來計算跳躍和重力的物理速度向量
    private bool isGrounded;          // 記錄企鵝目前是否踩在地面上

    [Header("UI 與搖桿設定")]
    public TextMeshProUGUI scoreText; 
    public JoystickController joystick; 

    private int score = 0;           
    private CharacterController controller; 
    private float originalSpeed;   
    private bool isBoosting = false; 

    void Start()
    {
        controller = GetComponent<CharacterController>();
        UpdateScoreUI(); 
    }

    void Update()
    {
        // 0. 檢查企鵝是否踩在地面上
        isGrounded = controller.isGrounded;
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f; // 踩在地面上時，給一個微弱的向下力，讓企鵝緊貼地面
        }

        float h = 0f;
        float v = 0f;

        // 讀取搖桿或鍵盤
        if (joystick != null && joystick.InputVector != Vector2.zero)
        {
            h = joystick.InputVector.x; 
            v = joystick.InputVector.y; 
        }
        else
        {
            h = Input.GetAxis("Horizontal"); 
            v = Input.GetAxis("Vertical");   
        }

        // 1. 處理左右旋轉
        transform.Rotate(0, h * rotateSpeed * Time.deltaTime, 0);

        // 2. 處理前後移動方向
        Vector3 moveDirection = transform.forward * v * speed;
        controller.Move(moveDirection * Time.deltaTime);

        // 3. 【鍵盤跳躍測試】：如果踩在地上，而且按下空白鍵（Space）
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // 根據物理公式：v = sqrt(h * -2 * g) 算出完美的跳躍初速度
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // 4. 持續施加重力速度（自由落體）
        playerVelocity.y += gravity * Time.deltaTime;

        // 5. 執行重力和跳躍的縱向移動
        controller.Move(playerVelocity * Time.deltaTime);
    }

    // 【新增開發給手機按鈕呼叫的方法】：手指點擊畫面按鈕時觸發
    public void MobileJump()
    {
        if (isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            Debug.Log("手機觸控跳躍！");
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI(); 
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Fishes: " + score;
        }
    }

    public void TriggerSpeedBoost(float duration)
    {
        if (!isBoosting) 
        {
            StartCoroutine(SpeedBoostRoutine(duration));
        }
    }

    private IEnumerator SpeedBoostRoutine(float duration)
    {
        isBoosting = true;
        originalSpeed = speed; 
        speed *= 2f;           
        
        Debug.Log("企鵝正在滑行加速中！");
        
        yield return new WaitForSeconds(duration); 
        
        speed = originalSpeed; 
        isBoosting = false;
        Debug.Log("加速時間結束，變回原速。");
    }
}