using System;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static List<float> AddToEachElement(List<float> list, float addition)
    {
        for (int i = 0; i < list.Count; i++)
        {
            list[i] += addition;
        }

        return list;
    }

    public static List<float> AddToEachElement(List<float> list, List<float> additions)
    {
        if (list.Count != additions.Count)
        {
            Debug.LogError(list.ToString() + " list does not match divisions list: " + additions);
            return null;
        }

        for (int i = 0; i < list.Count; i++)
        {
            list[i] += additions[i];
        }

        return list;
    }

    public static List<float> MultiplyEachElement(List<float> list, float factor)
    {
        for (int i = 0; i < list.Count; i++)
        {
            list[i] *= factor;
        }

        return list;
    }

    public static List<float> MultiplyEachElement(List<float> list, List<float> factors)
    {
        if (list.Count != factors.Count)
        {
            Debug.LogError(list.ToString() + " list does not match divisions list: " + factors);
            return null;
        }

        for (int i = 0; i < list.Count; i++)
        {
            list[i] *= factors[i];
        }

        return list;
    }

    public static List<float> DivideEachElement(List<float> list, float division)
    {
        if (division == 0f)
        {
            throw new DivideByZeroException();
        }

        for (int i = 0; i < list.Count; i++)
        {
            list[i] /= division;
        }

        return list;
    }

    public static List<float> DivideEachElement(List<float> list, List<float> divisions)
    {
        if (list.Count != divisions.Count)
        {
            Debug.LogError(list.ToString() + " list does not match divisions list: " + divisions);
            return null;
        }

        for (int i = 0; i < list.Count; i++)
        {
            if (divisions[i] == 0f)
            {
                throw new DivideByZeroException();
            }

            list[i] /= divisions[i];
        }

        return list;
    }
}