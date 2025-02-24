using UnityEngine;
namespace ALine
{
    public static class GameObjectExtensions
    {
        static public RectTransform RectTransform(this GameObject in_object)
        {
            return in_object.GetComponent<RectTransform>();
        }
    }
}