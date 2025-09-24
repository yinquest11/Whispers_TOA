using UnityEngine;

public class Missile : MonoBehaviour
{
    public float speed = 2f;
    public float rotateSpeed = 200f;
    public float lifeTime = 6f;

    private Transform target;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifeTime);
    }

    public void Initialize(Transform player, float angleOffset = 0f)
    {
        target = player;

        // Rotate initial direction slightly
        if (angleOffset != 0f)
        {
            transform.rotation = Quaternion.Euler(0, 0, angleOffset);
        }
    }

    void FixedUpdate()
    {
        if (target == null) return;

        Vector2 direction = ((Vector2)target.position - rb.position).normalized;
        float rotateAmount = Vector3.Cross(direction, transform.up).z;

        rb.angularVelocity = -rotateAmount * rotateSpeed;
        rb.linearVelocity = transform.up * speed;
    }
}


