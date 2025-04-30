using UnityEngine;

public class Heishiying : MonoBehaviour
{
    [Header("圆周运动设置")]
    [Tooltip("绕行中心点(父物体)")]
    public Transform centerPoint;
    [Tooltip("初始绕行半径")]
    public float radius = 2.0f;
    [Tooltip("最小允许半径")]
    public float minRadius = 0.5f;
    [Tooltip("目标绕行速度(度/秒)")]
    public float targetSpeed = 30.0f;
    [Tooltip("速度变化阻尼系数")]
    [Range(0.1f, 10f)]
    public float damping = 2.0f;
    [Tooltip("是否在开始时随机位置")]
    public bool randomStartPosition = true;
    [Tooltip("是否面向运动方向")]
    public bool faceMovingDirection = true;
    [Tooltip("运动平面 (XY平面或XZ平面)")]
    public bool useXYPlane = true; // false则为XZ平面

    [Header("障碍物检测设置")]
    [Tooltip("检测半径")]
    public float detectionRadius = 1.5f;
    [Tooltip("避障速度(半径缩小/恢复速度)")]
    public float avoidanceSpeed = 0.5f;
    [Tooltip("障碍物层级")]
    public LayerMask obstacleLayer;

    [Header("鼠标右键冲刺设置")]
    [Tooltip("冲刺时半径变化速度")]
    public float dashRadiusSpeed = 2.0f;
    [Tooltip("冲刺时的移动速度倍数")]
    public float dashSpeedMultiplier = 2.0f;
    [Tooltip("冲刺后恢复速度")]
    public float recoverySpeed = 1.0f;

    private float currentAngle = 0f;
    private float currentSpeed = 0f;
    private float currentRadius;
    private Vector3 upDirection = Vector3.up;
    private bool isAvoiding = false;
    private bool isDashing = false;
    private float originalSpeed;

    void Start()
    {
        // 如果没有指定中心点，尝试获取父物体
        if (centerPoint == null && transform.parent != null)
        {
            centerPoint = transform.parent;
        }

        // 随机起始角度
        if (randomStartPosition)
        {
            currentAngle = Random.Range(0f, 360f);
        }

        // 初始化半径和速度
        currentRadius = radius;
        currentSpeed = targetSpeed;
        originalSpeed = targetSpeed;

        // 更新初始位置
        UpdatePosition();
    }

    void Update()
    {
        // 检测鼠标右键按下
        if (Input.GetMouseButtonDown(1)) // 1代表鼠标右键
        {
            StartDash();
        }
        
        // 检测鼠标右键释放
        if (Input.GetMouseButtonUp(1))
        {
            EndDash();
        }
    }

    void FixedUpdate()
    {
        // 处理冲刺状态
        if (isDashing)
        {
            // 快速减小半径到最小值
            currentRadius = Mathf.Max(minRadius, currentRadius - dashRadiusSpeed * Time.deltaTime);
            
            // 如果已经达到最小半径，停止冲刺
            if (currentRadius <= minRadius + 0.01f)
            {
                
            }
        }
        else
        {
            // 正常状态下检测障碍物
            CheckForObstacles();
        }

        // 平滑过渡到目标速度（阻尼效果）
        float targetSpeedValue = isDashing ? originalSpeed * dashSpeedMultiplier : (isAvoiding ? 0f : originalSpeed);
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeedValue, damping * Time.deltaTime);

        // 更新角度
        currentAngle += currentSpeed * Time.deltaTime;
        currentAngle %= 360f;

        // 更新位置和旋转
        UpdatePosition();
    }

    void StartDash()
    {
        isDashing = true;
        isAvoiding = false; // 取消避障状态
    }

    void EndDash()
    {
        isDashing = false;
        // 不需要立即恢复半径，让FixedUpdate自然恢复
    }

    private Collider2D[] results = new Collider2D[10];
    void CheckForObstacles()
    {
        bool obstacleDetected = false;
        
        // 检测指定半径内的碰撞体
        var size = Physics2D.OverlapCircleNonAlloc(transform.position, detectionRadius, results, obstacleLayer);
        if (size > 0)
        {
            obstacleDetected = true;
        }

        // 根据检测结果调整半径
        if (obstacleDetected)
        {
            // 缩小半径
            currentRadius = Mathf.Max(minRadius, currentRadius - avoidanceSpeed * Time.deltaTime);
            isAvoiding = currentRadius <= minRadius + 0.1f; // 添加一点缓冲值
        }
        else
        {
            // 恢复半径
            currentRadius = Mathf.Min(radius, currentRadius + recoverySpeed * Time.deltaTime);
            isAvoiding = false;
        }
    }

    void UpdatePosition()
    {
        if (centerPoint == null) return;

        // 计算新位置
        float angleInRadians = currentAngle * Mathf.Deg2Rad;
        Vector3 offset;
        
        if (useXYPlane)
        {
            // XY平面运动
            offset = new Vector3(
                Mathf.Cos(angleInRadians) * currentRadius,
                Mathf.Sin(angleInRadians) * currentRadius,
                0f
            );
        }
        else
        {
            // XZ平面运动
            offset = new Vector3(
                Mathf.Cos(angleInRadians) * currentRadius,
                0f,
                Mathf.Sin(angleInRadians) * currentRadius
            );
        }

        transform.position = centerPoint.position + offset;

        // 设置旋转
        if (faceMovingDirection)
        {
            Vector3 tangent;
            if (useXYPlane)
            {
                tangent = new Vector3(-Mathf.Sin(angleInRadians), Mathf.Cos(angleInRadians), 0f);
            }
            else
            {
                tangent = new Vector3(-Mathf.Sin(angleInRadians), 0f, Mathf.Cos(angleInRadians));
            }
            
            // 确保up方向与父物体一致
            Quaternion targetRotation = Quaternion.LookRotation(tangent, upDirection);
            transform.rotation = targetRotation;
        }
        else
        {
            // 如果不面向运动方向，只保持up方向与父物体一致
            if (centerPoint != null)
            {
                upDirection = centerPoint.up;
            }
            transform.rotation = Quaternion.FromToRotation(transform.up, upDirection) * transform.rotation;
        }
    }

    // 在编辑器中绘制运动轨迹和检测范围
    void OnDrawGizmosSelected()
    {
        if (centerPoint != null)
        {
            // 绘制运动轨迹
            Gizmos.color = Color.cyan;
            if (useXYPlane)
            {
                // 在XY平面绘制圆形
                Vector3 prevPos = centerPoint.position + new Vector3(radius, 0f, 0f);
                for (int i = 1; i <= 36; i++)
                {
                    float angle = i * 10f * Mathf.Deg2Rad;
                    Vector3 newPos = centerPoint.position + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f);
                    Gizmos.DrawLine(prevPos, newPos);
                    prevPos = newPos;
                }
            }
            else
            {
                // 在XZ平面绘制圆形
                Gizmos.DrawWireSphere(centerPoint.position, radius);
            }

            // 绘制检测范围
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f); // 半透明橙色
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}