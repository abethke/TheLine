namespace ALine
{
    public static class ArrayExtensions
    {
        static public void Shuffle<T>(this T[] in_array)
        {
            for (int i = in_array.Length - 1; i > 0; i--)
            {
                int randomIndex = UnityEngine.Random.Range(0, i + 1);
                T tmp = in_array[i];
                in_array[i] = in_array[randomIndex];
                in_array[randomIndex] = tmp;
            }
        }
        static public string ToStringForReal<T>(this T[] in_array)
        {
            string description = "";
            for (int i = 0; i < in_array.Length; i++)
            {
                if (in_array[i] == null)
                {
                    description += "[NULL]";
                }
                else
                {
                    description += in_array[i].ToString();
                }
                if (i != in_array.Length - 1)
                {
                    description += ", ";
                }
            }
            return description;
        }
    }
}