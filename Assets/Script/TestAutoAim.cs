using UnityEngine;


public class TestAutoAim : MonoBehaviour
{
    public Vector2 originalDirection;


    public float angle;
    public int rayCount;

    void Start()
    {

    }


    void Update()
    {

        float baseAngle = Mathf.Atan2(originalDirection.y, originalDirection.x) * Mathf.Rad2Deg; // find the direction that I start with

        for (int i = 1; i <= rayCount; ++i)
        {
            float currentAngle = 0f;

            if (i % 2 != 0) // i is odd number
            {
                currentAngle = baseAngle + ((angle * 0.5f) / rayCount) * ((i * 0.5f) + 0.5f);

            }
            else if (i % 2 == 0) // i is even number
            {
                currentAngle = baseAngle - ((angle * 0.5f) / rayCount) * (i * 0.5f);

            }

            currentAngle *= Mathf.Deg2Rad;

            Vector2 direction = new Vector2(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle));


            Debug.DrawRay(transform.position, direction * 10f, Color.yellow);

        }

        //Debug.DrawRay(transform.position, originalDirection, Color.green);





    }
}
