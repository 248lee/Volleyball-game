using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathV : MonoBehaviour
{
    /// <summary>
    /// Cross operatioin of R^2 vectors.
    /// </summary>
    /// <param name="from">The first vector.</param>
    /// <param name="to">The second vector</param>
    /// <returns>A float as a result of crossing. Also known as determinants.</returns>
    public static float cross(Vector2 from, Vector2 to)
    {
        return from.x * to.y - from.y * to.x;
    }

    /// <summary>
    /// Determines if a target vector falls within a given rotation range defined by two boundary vectors.
    /// The input vectors need not to be normalized, and PLEASE, think everything counterclockwise. DON'T MAKE THINGS COMPLICATED!
    /// </summary>
    /// <param name="from">The starting boundary vector</param>
    /// <param name="to">The ending boundary vector</param>
    /// <param name="target">The vector to be checked</param>
    /// <returns>True if the target vector falls within the rotation range, otherwise false</returns>
    public static bool isVectorBetween(Vector2 from, Vector2 to, Vector2 target)
    {
        if (cross(from, to) > 0f)
        {
            return cross(from, target) >= 0f && cross(to, target) <= 0f;
        }
        else
        {
            return (cross(from, target) <= 0f && cross(to, target) >= 0f);
        }
    }

    /// <summary>
    /// Cosine value of 2 vectors. Notice that the order of the parameters don't mind.
    /// </summary>
    /// <param name="aVector"></param>
    /// <param name="anotherVector"></param>
    /// <returns></returns>
    public static float cos(Vector2 aVector, Vector2 anotherVector)
    {
        return Vector2.Dot(aVector, anotherVector) / (aVector.magnitude * anotherVector.magnitude);
    }
}
