using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Percent
{
    public float MinPercent;
    public float MaxPercent;
    public float PercentValue;

}

public class Probability<E> where E : Enum
{
    private Dictionary<E, Percent> probablilityTable = new Dictionary<E, Percent>();
    private float startPercent = 0f;

    public void AddProbabilityState(E stateName, float statePercent)
    {
        Percent percent;
        if (!probablilityTable.TryGetValue(stateName, out percent))
        {
            percent = new Percent();
            percent.MinPercent = startPercent;
            percent.MaxPercent = startPercent + statePercent;
            percent.PercentValue = statePercent;
            probablilityTable.Add(stateName, percent);
            startPercent += statePercent;
        }
    }

    public E GetRandomValue()
    {
        float totalValue = 0f;
        foreach (var percent in probablilityTable)
        {
            totalValue += percent.Value.PercentValue;
        }

        var value = UnityEngine.Random.Range(0, totalValue);
        Debug.Log(totalValue);

        foreach (var percent in probablilityTable)
        {
            if (value > percent.Value.MinPercent && value < percent.Value.MaxPercent)
                return percent.Key;
        }

        Debug.Log(value);
        return default;
    }

    public float GetRandomPercent(E stateName)
    {
        float totalValue = 0f;
        foreach (var probablility in probablilityTable)
        {
            totalValue += probablility.Value.PercentValue;
        }

        if (probablilityTable.TryGetValue(stateName, out Percent percent))
        {
            return percent.PercentValue / totalValue * 100;
        }

        return default;
    }
}
