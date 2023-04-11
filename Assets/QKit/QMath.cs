using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VectorValue { x, y, z };

public class QMath
{
    /// <summary>
    /// Return a Vector3 from a string-formatted Vector 3, i.e. "(0,0,0)"
    /// </summary>
    /// <param name="sVector"></param>
    /// <returns></returns>
    public static Vector3 StringToVector3(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        // split the items
        string[] sArray = sVector.Split(',');

        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]));

        return result;
    }

    /// <summary>
    /// Return true 1/n times.
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public static bool Roll(int n)
    {
        int i = Random.Range(0, n);
        return (i == n - 1);
    }

    /// <summary>
    /// Roll a n-sided die, return true 1/n times, and also return the result
    /// </summary>
    /// <param name="n"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool Roll(int n, out int result)
    {
        result = Random.Range(0, n) + 1;
        return (result == n);
    }

    /// <summary>
    /// Returns true if a is between b and c
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    /// <returns></returns>
    public static bool Between(float a, float b, float c)
    {
        return a > b ? (a > b && a < c) : (a > c && a < b);
    }


    /// <summary>
    /// Gets the direction from one position to another, optional parameter to turn off normalising
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="normalised"></param>
    /// <returns></returns>
    public static Vector3 Direction(Vector3 from, Vector3 to, bool normalised = true)
    {
        Vector3 dir = to - from;
        if (normalised)
            dir = dir.normalized;
        return dir;
    }

    /// <summary>
    /// Return the difference between two numbers, always returns a positive value.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static float Difference(float a, float b)
    {
        return Mathf.Abs(a - b);
    }

    /// <summary>
    /// Takes a number n, and returns it. If it falls below min or above max, it will wrap to the opposite value.
    /// </summary>
    /// <param name="n"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static int WrapIndex(int n, int min, int max)
    {
        if (n < min)
        {
            n = max;
            return n;
        }
        if (n > max)
        {
            n = min;
            return n;
        }
        return n;
    }

    /// <summary>
    /// Clip float f to d decimal places
    /// </summary>
    /// <param name="f"></param>
    /// <param name="d"></param>
    /// <returns></returns>
    public static float ClipToDecimalPlace(float f, float d)
    {
        float precision = Mathf.Max(1, Mathf.Pow(10, d));
        return Mathf.Ceil(f * precision) / precision;
    }


    /// <summary>
    /// Returns a vector based on a source vector, replacing a single value using enum VectorValue.x, .y, or .z
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="vectorValue"></param>
    /// <returns></returns>
    public static Vector3 ReplaceVectorValue(Vector3 source, VectorValue vectorValue, float value)
    {
        return new Vector3(
            vectorValue == VectorValue.x ? value : source.x,
            vectorValue == VectorValue.y ? value : source.y,
            vectorValue == VectorValue.z ? value : source.z
            );
    }
}
