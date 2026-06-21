using UnityEngine;
using UnityEngine.EventSystems;

// 繼承 Unity 官方的螢幕觸控、拖曳介面，手機專用
public class JoystickController : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    private RectTransform background;
    private RectTransform handle;
    private float joystickRadius;

    // 開發給企鵝讀取的方向數據（會輸出一個二維向量）
    public Vector2 InputVector { get; private set; }

    void Start()
    {
        background = GetComponent<RectTransform>();
        if (transform.childCount > 0)
        {
            handle = transform.GetChild(0).GetComponent<RectTransform>();
        }
        
        // 算出搖桿可移動的半徑範圍
        joystickRadius = background.sizeDelta.x / 2f;
    }

    // 當手指或滑鼠在螢幕上拖曳搖桿時執行
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position;
        // 將螢幕點擊座標轉換為搖桿內部的局部座標
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(background, eventData.position, eventData.pressEventCamera, out position))
        {
            position.x = (position.x / background.sizeDelta.x);
            position.y = (position.y / background.sizeDelta.y);

            // 算出拖曳向量
            InputVector = new Vector2(position.x * 2, position.y * 2);
            InputVector = (InputVector.magnitude > 1.0f) ? InputVector.normalized : InputVector;

            // 讓畫面上的白色小圓球跟著手指移動
            if (handle != null)
            {
                handle.anchoredPosition = new Vector2(InputVector.x * joystickRadius, InputVector.y * joystickRadius);
            }
        }
    }

    // 當手指或滑鼠按下時執行
    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    // 當放開時，小搖桿自動回彈到最中央，企鵝停止移動
    public void OnPointerUp(PointerEventData eventData)
    {
        InputVector = Vector2.zero;
        if (handle != null)
        {
            handle.anchoredPosition = Vector2.zero;
        }
    }
}