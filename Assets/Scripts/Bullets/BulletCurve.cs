using DG.Tweening;
using UnityEngine;

public class BulletCurve : MonoBehaviour
{
    public float damage = 10f;
    public float Damage => damage;
    public int ID = 0;

    private Vector3 controlPoint; // Điểm kiểm soát trung gian
    [SerializeField] private float speed = 1.0f; // Tốc độ di chuyển
    private float t = 0; // Giá trị thời gian

    private Transform startPoint, endPoint;
    private Vector3 previousPosition;
    private BoxCollider boxCollider;


    public void Play(Transform _startPoint, Transform _endPoint, int id, float damage = 20)
    {
        this.damage = damage;
        this.ID = id;
        if (boxCollider == null) boxCollider = GetComponent<BoxCollider>();
        boxCollider.enabled = false;

        this.startPoint = _startPoint;
        this.endPoint = _endPoint;

        // Tạo điểm kiểm soát ngẫu nhiên giữa startPoint và endPoint
        Vector3 midpoint = (startPoint.position + endPoint.position) / 2;
        controlPoint = midpoint + new Vector3(
            0, // X ngẫu nhiên
            Random.Range(5, 20f), // Y ngẫu nhiên (để tạo độ cong)
            0 // Z ngẫu nhiên
        );

        previousPosition = startPoint.position;
        t = 0;

    }

    void Update()
    {
        if(startPoint == null || endPoint == null) return;
        
        // Tăng giá trị t theo thời gian
        t += Time.deltaTime * speed;

        // Tính vị trí hiện tại theo phương trình Bezier
        Vector3 position = Mathf.Pow(1 - t, 2) * startPoint.position +
                           2 * (1 - t) * t * controlPoint +
                           Mathf.Pow(t, 2) * endPoint.position;

        // Di chuyển viên đạn
        transform.position = position;

        // Tính toán hướng bay
        Vector3 direction = position - previousPosition;
        if (direction != Vector3.zero)
        {
            // Cập nhật hướng xoay để viên đạn hướng theo đường bay
            transform.rotation = Quaternion.LookRotation(direction);
        }

        // Lưu vị trí hiện tại làm vị trí trước đó
        previousPosition = position;

        if (t >= 0.9f)
        {
            boxCollider.enabled = true;
        }

        // Hủy viên đạn khi t >= 1
        if (t >= 1)
        {
            gameObject.SetActive(false);
            //Destroy(gameObject);
        }
    }
}