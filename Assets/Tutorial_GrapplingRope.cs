using UnityEngine;

public class Tutorial_GrapplingRope : MonoBehaviour
{
    [Header("General Refernces:")]
    public Tutorial_GrapplingGun grapplingGun;
    public LineRenderer m_lineRenderer;

    [Header("General Settings:")]
    [SerializeField] private int percision = 40;
    [Range(0, 20)][SerializeField] private float straightenLineSpeed = 5;

    [Header("Rope Animation Settings:")]
    public AnimationCurve ropeAnimationCurve;
    [Range(0.01f, 4)][SerializeField] private float StartWaveSize = 2;
    float waveSize = 0;

    [Header("Rope Progression:")]
    public AnimationCurve ropeProgressionCurve;
    [SerializeField][Range(1, 500)] private float ropeProgressionSpeed = 1;

    private float moveTime = 0;

    [HideInInspector] public bool isGrappling = true;

    bool strightLine = true;

    
    // 1.
    private void OnEnable()
    {
        moveTime = 0; // 每次启用绳索时（例如，发射新的抓钩时）重置 'moveTime' 计时器。
        m_lineRenderer.positionCount = percision; // 根据 'percision' 设置，设定 LineRenderer 将使用的点数。
        waveSize = StartWaveSize; // 将 'waveSize' 初始化为在检视面板中配置的 'StartWaveSize'。
        strightLine = false; // 将 'strightLine' 设置为 false，意味着绳索应该以波浪动画开始。

        

        // 调用一个辅助函数，将 LineRenderer 的所有点都设置到枪的发射点。
        // 这可以防止绳索短暂地从 (0,0,0) 或其最后位置出现。
        LinePointsToFirePoint();

        // 启用 LineRenderer 组件，使绳索可见。
        m_lineRenderer.enabled = true;
    }

    private void OnDisable()
    {
        m_lineRenderer.enabled = false;
        isGrappling = false;
    }

    // 2.
    private void LinePointsToFirePoint()
    {
        for (int i = 0; i < percision; i++)
        {
            // 一开始先将每个点设置在firPoint.position
            // 先将每个点设置在一样的地方
            m_lineRenderer.SetPosition(i, grapplingGun.firePoint.position);
        }
    }

    // 3.
    private void Update()
    {
        // 将 'moveTime' 增加自上一帧以来经过的时间 (Time.deltaTime)。
        // 这使得 'moveTime' 充当绳索动画的运行时计时器。
        moveTime += Time.deltaTime;

        // 每帧调用 'DrawRope' 方法来更新绳索的外观
        DrawRope();
    }

    // 4.
    // 这个方法包含绘制绳索的主要逻辑，决定是用波浪还是用直线绘制它。
    void DrawRope()
    {
        
        // 检查绳索是否还不应该是直线（即，它仍处于初始的波浪状延伸阶段）。
        if (!strightLine)
        {
            // 这个条件检查 LineRenderer 最后一个点的X坐标是否，与抓取点的X坐标匹配。
            if (m_lineRenderer.GetPosition(percision - 1).x == grapplingGun.grapplePoint.x)
            {
                // 如果（基于X坐标）它已经到达，则将 'strightLine' 设置为 true。
                // 绳索现在将过渡到直线状态或已经是直线。
                strightLine = true;
            }
            else
            {
                // 如果绳索还没有“到达”抓取点，则继续用波浪绘制它。
                DrawRopeWaves();
            }
        }
        else // 如果 'strightLine' 为 true，则执行此块。意思是如果 (m_lineRenderer.GetPosition(percision - 1).x == grapplingGun.grapplePoint.x) true 了
        {
            // 6.
            if (!isGrappling)
            {
                grapplingGun.Grapple();
                isGrappling = true;
            }
            if (waveSize > 0)
            {
                // 慢慢减少waveSize
                waveSize -= Time.deltaTime * straightenLineSpeed;
                // 更新我们的rope
                DrawRopeWaves();
            }
            else
            {
                // 小过0之后让它等于0
                waveSize = 0;

                // 直接强制变两个点，变成直线
                if (m_lineRenderer.positionCount != 2) { m_lineRenderer.positionCount = 2; }
                // 更新rope
                // 绘制一条直线，不用到之前的什么percision，固定直线的函数
                DrawRopeNoWaves();
            }
        }
    }

    // 5.
    // 此方法用波浪效果绘制绳索。
    void DrawRopeWaves()
    {
        // 遍历定义绳索曲线的每个点。
        for (int i = 0; i < percision; i++)
        {
            // 它是想把一个从0开始计数的整数 i 转换成一个从0.0到1.0之间均匀分布的小数 delta
            // 得到当前的粒子对应整个线的第几 %
            float delta = (float)i / ((float)percision - 1f);

            // 一个Vector2，他是起点到重点这条直线 逆时针的垂直向量 -y/x，然后normalized变成1只保留方向，乘对应目前ropeAnimationCurve的delta值
            // 然后 * waveSize进行缩放
            // 得到目前%的粒子该有的偏移量
            Vector2 offset = Vector2.Perpendicular(grapplingGun.grappleDistanceVector).normalized * ropeAnimationCurve.Evaluate(delta) * waveSize;

            // 用.Lerp来插值，计算枪口到落点这个距离的， 整个距离的% 对应我的第几个countPoint
            // +offset 是在直线上的某个位置应用计算之前算好的 需要的 prependicular 偏移量
            // 所以我们先计算了直线，再用先前算好了的在直线上的特定位置该有的偏移量并应用在这个直线上
            // 目前的点在整个线的，应该的比例长度（在绳索上由 delta 所代表的当前比例位置）上该因为那个AnimationCurve有的垂直偏移量 
            // 把对应%粒子直接放到完全伸展并带有完整波浪效果时，应该在的理想世界位置 （对于一样的第i粒子来说, targetPosition每次都是一样的）
            Vector2 targetPosition = Vector2.Lerp(grapplingGun.firePoint.position, grapplingGun.grapplePoint, delta/*目前的粒子是第几%的长度*/) + offset/*垂直的偏移量*/;

            // 负责实现绳索“射出”或“生长”的动画效果
            // 'currentPosition' 计算这个点从发射点向其 'targetPosition' "行进"了多远：
            // 它实际在屏幕上应该被插值在哪个位置。
            // 插值，在枪口和落点位置之间插值（距离），插值Ratio越靠近1代表越靠近落点位置了
            // 用一个AnimationCurve，起点 00，终点 11，用之前一直重置的moveTime作为X来自动更新并且获取AnimationCurve这个Graph的 Y 来引用在插值ratio慢慢插去targetPosition
            // 最后乘与一个变量 ropeProgressionSpeed 是为了让Evalute出来的Y值更快到达1，所以有可能我的x还没到1我的y就到1了，提早抵达
            // 实质上是在移动点，而点和点之间会自动渲染线条
            // 所以这些点在游戏的过程中，在我的发射绳索的时候会慢慢被lerp去理想的位置，而点和点之间会绘制线条
            // 把当前的粒子通过.Lerp慢慢移动到targetPosition，moveTime会一直自动增加，所以会一直自动Lerp去1
            // 比较远的被插值的更远
            // 所以每个粒子都是直线向着自己的targetPosition移动，由于总距离不同，所以就算同样的插值比例，targetPosition在比较远的点要前进到下一个插值点的时候会移动到相对远
            // 核心就是利用自动增加的moveTime Evalaute Graph的Y，慢慢让插值比例变成1
            Vector2 currentPosition = Vector2.Lerp(grapplingGun.firePoint.position, targetPosition, (ropeProgressionCurve.Evaluate(moveTime) * ropeProgressionSpeed));
            m_lineRenderer.SetPosition(i, currentPosition);
            
        }
    }

    // 专门独立的，画直线的函数
    void DrawRopeNoWaves()
    {
        m_lineRenderer.SetPosition(0, grapplingGun.firePoint.position);
        m_lineRenderer.SetPosition(1, grapplingGun.grapplePoint);
    }
}