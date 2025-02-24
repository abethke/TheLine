using System;
using UnityEngine;

namespace ALine
{
    public static class Vector2Extensions
    {
        static public Vector2 SetX(this Vector2 in_vector, float in_value)
        {
            in_vector.x = in_value;
            return in_vector;
        }

        static public Vector2 SetY(this Vector2 in_vector, float in_value)
        {
            in_vector.y = in_value;
            return in_vector;
        }
        static public Vector2Int PlusXY(this Vector2Int in_vector, int in_x, int in_y)
        {
            return new Vector2Int(in_vector.x + in_x, in_vector.y + in_y);
        }
        static public Vector2 PlusY(this Vector2 in_vector, float in_value)
        {
            in_vector.y += in_value;
            return in_vector;
        }
    }
}