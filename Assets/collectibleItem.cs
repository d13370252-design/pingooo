using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public enum ItemType { Fish, IceCube }
    
    [Header("道具設定")]
    public ItemType itemType; 
    public int scoreValue = 1; 

    [Header("冰塊專用設定")]
    public float speedBoostDuration = 5f; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (itemType == ItemType.Fish)
            {
                Debug.Log($"抓到魚了！分數增加 {scoreValue}");
                
                // 強制在全場景搜尋企鵝腳本並加分
                PenguinMove movement = Object.FindFirstObjectByType<PenguinMove>();
                if (movement != null)
                {
                    movement.AddScore(scoreValue);
                }
            }
            else if (itemType == ItemType.IceCube)
            {
                Debug.Log("吃到冰塊！企鵝獲得冰上加速！");
                
                // 強制在全場景搜尋企鵝腳本並加速
                PenguinMove movement = Object.FindFirstObjectByType<PenguinMove>();
                if (movement != null)
                {
                    movement.TriggerSpeedBoost(speedBoostDuration);
                }
            }

            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // 讓道具在原地自己旋轉
        transform.Rotate(Vector3.up * 50f * Time.deltaTime);
    }
}