using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Wraps the Random class to provide some convenience methods
public static class Randomer
{

    // returns a point inside the given bounds
    /// <summary>
    /// Given a Bounds object, return a point inside those bounds
    /// </summary>
    /// <param name="bounds"></param>
    /// <returns>A point within the bounds.</returns>
    public static Vector3 PointInBounds (Bounds bounds)
    {
        Vector3 min = bounds.min;
        Vector3 max = bounds.max;

        float x = Random.Range (min.x, max.x);
        float y = Random.Range (min.y, max.y);
        float z = Random.Range (min.z, max.z);

        return new Vector3 (x, y, z);
    }
    /// <summary>
    /// Returns a value from the given range (inclusive min, max)
    /// </summary>
    /// <param name="range">The range from which to get the value</param>
    /// <returns></returns>
    public static float InRange (Range range)
    {
        return Random.Range (range.min, range.max);
    }

    /// <summary>
    /// Returns a value from the given range (inclusive min, max)
    /// </summary>
    /// <param name="range">The range from which to get the value</param>
    /// <returns></returns>
    public static int InRange (RangeInt range)
    {
        return Random.Range (range.min, range.max + 1);
    }

    /// <summary>
    /// Given a weight table, will return an random index, based on those weights.
    /// </summary>
    /// <param name="weights"></param>
    /// <returns>An index chosen by weight</returns>
    public static int IndexForWeightTable (List<float> weights)
    {
        float total = 0;
        foreach (int frequency in weights)
        {
            total += frequency;
        }

        float rnd = Random.Range (0, total);
        total = 0;
        int i = 0;
        foreach (int frequency in weights)
        {
            total += frequency;
            if (rnd < total) return i;
            i++;
        }

        return 0;
    }

    /// <summary>
    /// Returns true or false with 50/50 chance.
    /// </summary>
    /// <returns></returns>
    public static bool Bool ()
    {
        float val = Random.value;
        return val > 0.5f;
    }

    /// <summary>
    /// Randomises the order of the given list, does not make a copy
    /// </summary>
    /// <param name="list">The list to randomise</param>
    public static void RandomiseList (IList list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int r = Random.Range (0, i);
            object tmp = list[i];
            list[i] = list[r];
            list[r] = tmp;
        }
    }

    /// <summary>
    /// Returns one or more of the items from the list, with equal chance of each.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static T FromList<T> (List<T> list)
    {
        int index = Random.Range (0, list.Count);
        return list[index];
    }

    public static List<T> FromList<T> (List<T> list, int count)
    {
        List<T> newList = new List<T> ();
        newList.AddRange (list);
        RandomiseList (newList);
        newList.RemoveRange (count, newList.Count - count);
        return newList;
    }

    public static T FromArray<T> (T[] array)
    {
        int index = Random.Range (0, array.Length);
        return array[index];
    }

    internal static T FromEnum<T> ()
    {
        System.Array array = System.Enum.GetValues (typeof (T));
        int index = Random.Range (0, array.Length);
        return (T) System.Convert.ChangeType (array.GetValue (index), typeof (T));
    }
}