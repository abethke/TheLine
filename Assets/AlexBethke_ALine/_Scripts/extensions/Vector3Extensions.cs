using UnityEngine;

namespace ALine
{
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
    }
}