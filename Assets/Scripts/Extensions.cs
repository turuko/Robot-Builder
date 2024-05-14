using System;
using UnityEngine;

public static class Extensions
{
    public static Tuple<int, int, int> IndexOf<T>(this T[,,] array, T value)
    {
        if (value == null)
        {
            Debug.Log("Value is null?");
            return Tuple.Create(-1, -1, -1);
        }
            
        int w = array.GetLength(0);
        int h = array.GetLength(1);
        int d = array.GetLength(2);

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                for (int z = 0; z < d; z++)
                {
                    if (array[x, y, z] != null && array[x, y, z].Equals(value))
                        return Tuple.Create(x, y, z);
                }
            }
        }

        return Tuple.Create(-1, -1, -1);
    }
}