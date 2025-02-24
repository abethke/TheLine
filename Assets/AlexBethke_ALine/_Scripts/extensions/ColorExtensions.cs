using System;
using System.Globalization;
using UnityEngine;

namespace ALine
{
    public static class ColorExtensions
    {
        static public Color SetAlpha(this Color in_color, float in_value)
        {
            in_color.a = in_value;
            return in_color;
        }
        static public Color PlusAlpha(this Color in_color, float in_value)
        {
            in_color.a = Mathf.Max(Mathf.Min(1, in_color.a + in_value), 0);
            return in_color;
        }
        static public Color FromHex(this Color in_color, string in_hex)
        {
            in_color = Color.white;
            //replace # occurences
            if (in_hex.IndexOf('#') != -1)
                in_hex = in_hex.Replace("#", "");

            float r, g, b = 0;

            try
            {
                r = int.Parse(in_hex.Substring(0, 2), NumberStyles.AllowHexSpecifier) / 255f;
                g = int.Parse(in_hex.Substring(2, 2), NumberStyles.AllowHexSpecifier) / 255f;
                b = int.Parse(in_hex.Substring(4, 2), NumberStyles.AllowHexSpecifier) / 255f;
                if (r > 1 || b > 1 || g > 1 || r < 0 || b < 0 || g < 0)
                    throw new Exception("Invalid color");
                in_color = new Color(r, g, b);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
            return in_color;
        }
    }
}