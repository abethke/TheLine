using UnityEngine;

public static class Vector3Extensions
{
    /// <summary>
    /// Set the Y value on the vector.
    /// </summary>
    /// <param name="in_value">The value to set.</param>
    /// <returns>The modified Vector3.</returns>
    static public Vector3 SetY(this Vector3 in_vector, float in_value)
    {
        in_vector.y = in_value;
        return in_vector;
    }

    /// <summary>
    /// Set the X value on the vector.
    /// </summary>
    /// <param name="in_value">The value to set.</param>
    /// <returns>The modified Vector3.</returns>
    static public Vector3 SetX(this Vector3 in_vector, float in_value)
    {
        in_vector.x = in_value;
        return in_vector;
    }

    /// <summary>
    /// Set the Z value on the vector.
    /// </summary>
    /// <param name="in_value">The value to set.</param>
    /// <returns>The modified Vector3.</returns>
    static public Vector3 SetZ(this Vector3 in_vector, float in_value)
    {
        in_vector.z = in_value;
        return in_vector;
    }

    /// <summary>
    /// Set the X & Z value on the vector.
    /// </summary>
    /// <param name="in_value">The value to set.</param>
    /// <returns>The modified Vector3.</returns>
    static public Vector3 SetXZ(this Vector3 in_vector, float in_value)
    {
        in_vector.x = in_value;
        in_vector.z = in_value;
        return in_vector;
    }

    /// <summary>
    /// Set the X & Z value on the vector.
    /// </summary>
    /// <param name="in_value">The X value to set.</param>
    /// <param name="in_value2">The Z value to set.</param>
    /// <returns>The modified Vector3.</returns>
    static public Vector3 SetXZ(this Vector3 in_vector, float in_value, float in_value2)
    {
        in_vector.x = in_value;
        in_vector.z = in_value2;
        return in_vector;
    }

    /// <summary>
    /// Adds to the Y value of the vector.
    /// </summary>
    /// <param name="in_value">The value to add.</param>
    /// <returns>The modified Vector3.</returns>
    static public Vector3 PlusY(this Vector3 in_vector, float in_value)
    {
        in_vector.y += in_value;
        return in_vector;
    }

    /// <summary>
    /// Adds to the X value of the vector.
    /// </summary>
    /// <param name="in_value">The value to add.</param>
    /// <returns>The modified Vector3.</returns>
    static public Vector3 PlusX(this Vector3 in_vector, float in_value)
    {
        in_vector.x += in_value;
        return in_vector;
    }

    /// <summary>
    /// Adds to the Z value of the vector.
    /// </summary>
    /// <param name="in_value">The value to add.</param>
    /// <returns>The modified Vector3.</returns>
    static public Vector3 PlusZ(this Vector3 in_vector, float in_value)
    {
        in_vector.z += in_value;
        return in_vector;
    }

    /// <summary>
    /// Adds to the X & Y value of the vector.
    /// </summary>
    /// <param name="in_value">The value to add to X.</param>
    /// <param name="in_value2">The value to add to Y.</param>
    /// <returns>The modified Vector3.</returns>
    static public Vector3 PlusXY(this Vector3 in_vector, float in_value, float in_value2)
    {
        in_vector.x += in_value;
        in_vector.y += in_value2;
        return in_vector;
    }

    /// <summary>
    /// Adds to the X & Z value of the vector.
    /// </summary>
    /// <param name="in_value">The value to add to X.</param>
    /// <param name="in_value2">The value to add to Z.</param>
    /// <returns>The modified Vector3.</returns>
    static public Vector3 PlusXZ(this Vector3 in_vector, float in_value, float in_value2)
    {
        in_vector.x += in_value;
        in_vector.z += in_value2;
        return in_vector;
    }

    /// <summary>
    /// Adds to the Y & Z value of the vector.
    /// </summary>
    /// <param name="in_value">The value to add to Y.</param>
    /// <param name="in_value2">The value to add to Z.</param>
    /// <returns>The modified Vector3.</returns>
    static public Vector3 PlusYZ(this Vector3 in_vector, float in_value, float in_value2)
    {
        in_vector.y += in_value;
        in_vector.z += in_value2;
        return in_vector;
    }

    /// <summary>
    /// Truncates the vector to a maximum length if the vector is currently beyond that length.
    /// </summary>
    /// <param name="in_value">The maximum length.</param>
    /// <returns>The modified Vector3.</returns>
    static public Vector3 Truncate(this Vector3 in_vector, float in_maxValue)
    {
        if (in_vector.magnitude > in_maxValue)
        {
            in_vector = in_vector.normalized * in_maxValue;
        }
        return in_vector;
    }

    /// <summary>
    /// Multiplies a vector by a second vector.
    /// </summary>
    /// <param name="in_vector2">The vector by which to multiply.</param>
    /// <returns>The modified Vector3.</returns>
    static public Vector3 Multiply(this Vector3 in_vector, Vector3 in_vector2)
    {
        in_vector.x *= in_vector2.x;
        in_vector.y *= in_vector2.y;
        in_vector.z *= in_vector2.z;
        return in_vector;
    }

    /// <summary>
    /// Finds the closest point to a line from a set point
    /// </summary>
    /// <returns>
    /// The closest point.
    /// </returns>
    /// <param name='StartPoint'>
    /// Start point.
    /// </param>
    /// <param name='EndPoint'>
    /// End point.
    /// </param>
    /// <param name='CurentPoint'>
    /// Curent point.
    /// </param>
    public static Vector3 FindClosestPointToLineSegment(this Vector3 vec, Vector3 StartPoint, Vector3 EndPoint, Vector3 CurentPoint)
    {
		Vector3 Line = EndPoint - StartPoint;
		Vector3 NewPoint =  CurentPoint - StartPoint;
		Vector3 ReturnPoint = Vector3.Project(NewPoint, Line.normalized);
		return ReturnPoint + StartPoint;
	}
    static public GameObject FindClosest(this Vector3 in_point, params GameObject[] in_objects)
    {
        float closestDistance = float.MaxValue;
        GameObject closest = null;

        foreach (GameObject obj in in_objects)
        {
            if (!obj)
                continue;

            float distance = Vector3.Distance(obj.transform.position, in_point);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = obj;
            }
        }
        return closest;
    }
    public static Vector3 RandomPlusMinus(this Vector3 in_vector)
    {
        Vector3 randomized = Vector3.zero;
        randomized.x = Random.Range(-in_vector.x, in_vector.x);
        randomized.y = Random.Range(-in_vector.y, in_vector.y);
        randomized.z = Random.Range(-in_vector.z, in_vector.z);
        return randomized;
    }
}
