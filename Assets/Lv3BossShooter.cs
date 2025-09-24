using System.Reflection;
using UnityEngine;

public class Lv3BossShooter : MonoBehaviour
{
    public GameObject missilePrefab;
    public int missileCount = 3;
    public float spreadAngle = 30f;

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void ShootMissiles()
    {
        if (player == null) return;

        for (int i = 0; i < missileCount; i++)
        {
            GameObject missile = Instantiate(missilePrefab, transform.position, Quaternion.identity);

            // Offset angles so missiles don't overlap perfectly
            float angleOffset = (i - (missileCount - 1) / 2f) * spreadAngle;

            missile.GetComponent<Missile>().Initialize(player, angleOffset);
        }
    }
}

