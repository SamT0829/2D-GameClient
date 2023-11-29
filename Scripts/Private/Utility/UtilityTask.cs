using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public static class UtilityTask
{
    public static async Task WaitForBool(bool _bool, Action callBack)
    {
        while (!_bool)
        {
            Debug.Log(_bool);
            await Task.Delay(TimeSpan.FromSeconds(0.1));
        }

        callBack.Invoke();
        Debug.Log("await Finish");
        await Task.CompletedTask;
    }

    public static Task DelayForSeconds(double value)
    {
        return Task.Delay(TimeSpan.FromSeconds(value));
    }

    public static Task CompletedTask { get => Task.CompletedTask; }
}

public static class Utility
{
    public static object[] VectorToList(Vector3 vector3)
    {
        List<object> positionList = new List<object>();
        positionList.Add(vector3.x);
        positionList.Add(vector3.y);
        positionList.Add(vector3.z);
        return positionList.ToArray();
    }
}