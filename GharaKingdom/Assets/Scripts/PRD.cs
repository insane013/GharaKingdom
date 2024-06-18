using System;
using Unity.Mathematics;
using UnityEngine;

public class PRD
{
    private static readonly float RandomConstant = 0.3f;

    private int attemptCount = 0;
    private float currentProbability = 0;

    public bool CheckProbability(float baseProbability)
    {
        float roll = UnityEngine.Random.Range(0f, 1f);

        currentProbability = Pseudorandomize(baseProbability);
        
        if (currentProbability <= roll)
        {
            attemptCount = 0; return true;
        } else
        {
            attemptCount++; return false;
        }
    }

    private float Pseudorandomize(float probability)
    {
        for (int i = 0; i < attemptCount; i++)
        {
            probability += (probability * RandomConstant);
        }
        return probability;
    }
}
