using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SmartAnchorObject_Tag : MonoBehaviour
{
    [HideInInspector] public Vector2 Q1Point; // 45
    [HideInInspector] public Vector2 Q2Point; // 135
    [HideInInspector] public Vector2 Q3Point; // 225
    [HideInInspector] public Vector2 Q4Point; // 315

    private CapsuleCollider2D _collider;
    private Vector2 _colliderSize;

    private Vector2 _toQ1Edge;
    private Vector2 _toQ2Edge;
    private Vector2 _toQ3Edge;
    private Vector2 _toQ4Edge;

    public Vector2 offsetQ1Point = Vector2.zero;
    public Vector2 offsetQ2Point = Vector2.zero;
    public Vector2 offsetQ3Point = Vector2.zero;
    public Vector2 offsetQ4Point = Vector2.zero;

    public float pointDistance = 0.4f;

    void Start()
    {
        _collider = GetComponent<CapsuleCollider2D>();
        

        _toQ1Edge = new Vector2(_colliderSize.x / 2, _colliderSize.y / 2);
        _toQ2Edge = new Vector2(- _colliderSize.x / 2, _colliderSize.y / 2);
        _toQ3Edge = -_toQ1Edge;
        _toQ4Edge = -_toQ2Edge;

        Q1Point = _toQ1Edge + new Vector2(1,1) * pointDistance;       
        Q2Point = _toQ2Edge + new Vector2(-1,1) * pointDistance;
        Q3Point = _toQ3Edge + new Vector2(-1,-1) * pointDistance;
        Q4Point = _toQ4Edge + new Vector2(1,-1) * pointDistance;

        

    }

    // Update is called once per frame
    void Update()
    {
        
        UpdateQPoint();
        DrawPoint();



    }

    

    private void UpdateQPoint()
    {


        _colliderSize = _collider.size * transform.localScale;

        _toQ1Edge = new Vector2(_colliderSize.x / 2, _colliderSize.y / 2);
        _toQ2Edge = new Vector2(-_colliderSize.x / 2, _colliderSize.y / 2);
        _toQ3Edge = -_toQ1Edge;
        _toQ4Edge = -_toQ2Edge;

        Q1Point = _toQ1Edge + new Vector2(1, 1) * pointDistance + (Vector2)transform.position;
        Q2Point = _toQ2Edge + new Vector2(-1, 1) * pointDistance + (Vector2)transform.position;
        Q3Point = _toQ3Edge + new Vector2(-1, -1) * pointDistance + (Vector2)transform.position;
        Q4Point = _toQ4Edge + new Vector2(1, -1) * pointDistance + (Vector2)transform.position;

        
        // open a new function start from here, adjust the Q point smart
        Q1Point += offsetQ1Point;
        Q2Point += offsetQ2Point;
        Q3Point += offsetQ3Point;
        Q4Point += offsetQ4Point;


    }

    private void DrawPoint()
    {
        Debug.DrawRay(transform.position, Q1Point - (Vector2)transform.position, Color.red);
        Debug.DrawRay(transform.position, Q2Point- (Vector2)transform.position, Color.red);
        Debug.DrawRay(transform.position, Q3Point-(Vector2)transform.position, Color.red);
        Debug.DrawRay(transform.position, Q4Point- (Vector2)transform.position, Color.red);
    }

    public Vector2 ReturnClosestQPoint(Vector2 firstPointPosition)
    {
        List<float> distanceList = new List<float>();

        distanceList.Add(Vector2.Distance(firstPointPosition, Q1Point));
        distanceList.Add(Vector2.Distance(firstPointPosition, Q2Point));
        distanceList.Add(Vector2.Distance(firstPointPosition, Q3Point));
        distanceList.Add(Vector2.Distance(firstPointPosition, Q4Point));

        int minimumPunyaIndex = distanceList.IndexOf(distanceList.Min());

        Vector2 closestPointPosition = Vector2.zero;

        switch (minimumPunyaIndex)
        {
            case 0:
                closestPointPosition = Q1Point;
                break;
            case 1:
                closestPointPosition = Q2Point;
                break;
            case 2:
                closestPointPosition = Q3Point;
                break;
             case 3:
                closestPointPosition = Q4Point;
                break;
             
        }

        return closestPointPosition;
    }
}
