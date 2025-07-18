using UnityEngine;

public class BeetlePatrol : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;

    private Vector3 target;

    void Start()
    {
        target = pointB.position;
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target) < 0.5f)
        {
            target = target == pointA.position ? pointB.position : pointA.position;
            Flip();
        }

        //transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
        // // 🔁 Check if the beetle is close to the target
        // if (Vector2.Distance(transform.position, target) < 0.5f)
        // {
        //     // Use if-else instead of ternary operator
        //     if (target == pointA.position)
        //     {
        //         target = pointB.position;
        //     }
        //     else
        //     {
        //         target = pointA.position;
        //     }

        //     Flip(); // Flip the beetle's facing direction
        // }


    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
