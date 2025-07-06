using UnityEngine;
using System.Collections;

public class MovementHelper : MonoBehaviour
{
    public bool finish = false;

    // version 1: the object will reach the destination in given time how ever
    // animation curve x is the time (0 -> 1), y is the distance form the origin (0 -> 1)

    // Speed ​​is variable. In order to arrive within the specified time, it will move slowly for short distances and quickly for long distances.
    // Graph = 规定的时间内我和终点的距离的变化曲线
    public void MoveToByTime(Transform objectToMove, Vector3 destination, float duration, AnimationCurve curve)
    {
        // prevent does not draw curve in inspector
        if (curve == null || curve.keys.Length < 2)
        {
            Debug.LogWarning("MoveToByTime was called with an invalid curve. Using default EaseInOut.");
            curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        }
        StartCoroutine(MoveCoroutine(objectToMove, destination, duration, curve));
    }

    // Velocity is constant. An object moves at the same speed regardless of its distance.
    // graph = 根据我的速度返回一个时间给我，我在这个时间内和终点的距离变化

    public void MoveToBySpeed(Transform objectToMove, Vector3 destination, float speed, AnimationCurve curve)
    {
        // prevent 0 speed
        if (speed <= 0)
        {
            Debug.LogError("Speed must be greater than zero.");
            return;
        }

        // prevent does not draw curve in inspector
        if (curve == null || curve.keys.Length < 2)
        {
            Debug.LogWarning("MoveToBySpeed was called with an invalid curve. Using default EaseInOut.");
            curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        }

        float totalDistance = Vector3.Distance(objectToMove.position, destination);
        if (totalDistance < 0.001f) return;


        // speed is only for calculate the total duration
        float duration = totalDistance / speed;

        StartCoroutine(MoveCoroutine(objectToMove, destination, duration, curve));
    }


    // --- 统一的、唯一的协程 ---
    // 两个公开方法最终都调用这个协程来完成工作
    //一开始速度比较高意味着我希望我整体的时间是相对短的，控制 Graph，在这么短的时间内我和 target 的距离
    private IEnumerator MoveCoroutine(Transform objectToMove, Vector3 destination, float duration, AnimationCurve curve)
    {
        finish = false;

        Vector3 startPosition = objectToMove.position; // initial start position



        float elapsedTime = 0f; // time that already use

        while (elapsedTime < duration) // havent reach the required time
        {
            if (objectToMove == null) yield break;

            elapsedTime += Time.deltaTime; // 1 frame time

            float distanceProgress = curve.Evaluate(Mathf.Clamp01(elapsedTime / duration));

            objectToMove.position = Vector3.LerpUnclamped(startPosition, destination, distanceProgress);

            yield return null; // next frame only excute， START FROM HERE !!, inside the while, so startPosition will not be refresh
        }

        if (objectToMove != null)
        {
            objectToMove.position = destination; // still not at the position also force to that point
            finish = true;
        }
    }

    // stop moving, clear all remaing running coroutine
    public void StopMoving()
    {

        StopAllCoroutines();
    }
}