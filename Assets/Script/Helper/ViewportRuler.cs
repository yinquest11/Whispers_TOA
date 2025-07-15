using System;
using UnityEngine;

public class ViewportRuler : MonoBehaviour
{
    public  int totalDivision = 20;


    public int currentZone;

    [HideInInspector] public int previousZone; 

    public Vector2 mouseViewportPos;

    private int _mouseMoveDirection = 0; //   -1 = left, 0 = stationary, 1 = right

    public bool needDraw = true;

    public int GetMouseMoveDirection // 
    {
        get { return _mouseMoveDirection; }
    }

    public bool HasDirectionReversed //  这个值可以被外部读取（get），但只能被这个脚本自己修改（private set）。
    { get; private set; }

    public int GetCurrentMouseZone
    {
        get { return currentZone; }
    }

    

    void Start()
    {
        currentZone = CalculateCurrentZone();
        previousZone = currentZone;
    }

    
    void Update()
    {
        CalculateZone();
        
       if (needDraw == false)
            return;
       
        DrawDivisionLines();
        
    }

    private void CalculateZone()
    {
        CalculateCurrentZone();


        HasDirectionReversed = false;

        if (currentZone != previousZone)
        {
            int newDirection = 0;

            if (currentZone > previousZone)
            {
                newDirection = 1;
            }
            else
            {
                newDirection = -1;
            }


            if (newDirection != _mouseMoveDirection)
            {
                HasDirectionReversed = true;
            }

            _mouseMoveDirection = newDirection;

            previousZone = currentZone;


        }

    }

    public int CalculateCurrentZone()
    {
        mouseViewportPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);

        int currentZoneIndex = Mathf.FloorToInt(mouseViewportPos.x * totalDivision);// 2.5 = 2, 2 is index, so is third element
        currentZone = currentZoneIndex + 1;
        currentZone = Mathf.Clamp(currentZone, 1, totalDivision);
        return currentZone;
    }

    void DrawDivisionLines()
    {
        // 这个 Z 坐标值很重要。它代表了你希望画的线离相机有多远。
        // 如果是2D游戏，相机通常在 Z=-10，那么设置成 10 就可以让线画在 Z=0 的平面上。
        float distanceFromCamera = 10f;

        // 我们要画9条分割线 (i=1 到 9)
        for (int i = 1; i < totalDivision; i++)
        {
            // 1. 计算当前这条分割线在视口中的X坐标
            //    必须要做 (float) 类型转换，否则整数除法会得到0
            float viewportX = (float)i / totalDivision;

            // 2. 定义这条线在视口中的起点和终点
            Vector3 viewportStart = new Vector3(viewportX, 0f, distanceFromCamera); // 屏幕底部
            Vector3 viewportEnd = new Vector3(viewportX, 1f, distanceFromCamera);   // 屏幕顶部

            // 3. 【核心】将视口坐标转换为世界坐标
            Vector3 worldStart = Camera.main.ViewportToWorldPoint(viewportStart);
            Vector3 worldEnd = Camera.main.ViewportToWorldPoint(viewportEnd);

            // 4. 用世界坐标画出这条线
            Debug.DrawLine(worldStart, worldEnd, Color.yellow);
        }
    }
}
