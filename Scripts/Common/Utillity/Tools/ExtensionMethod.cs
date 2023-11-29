using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethod
{
    private const float dontThreshold = 0.5f;
    public static bool IsFacingTarget(this Transform transform, Transform target)
    {
        var vectorToTarget = target.position - transform.position;
        vectorToTarget.Normalize();

        float dot = Vector3.Dot(transform.forward, vectorToTarget);
        return dot >= dontThreshold;
    }

    public static bool ChangeType<T>(object data, out T outData)
    {
        if (data is T)
        {
            outData = (T)data;
            return true;
        }

        Debug.LogWarningFormat("Change Type wrong from {0} where type {1}", data, typeof(T));
        outData = default;
        return false;
    }
}
