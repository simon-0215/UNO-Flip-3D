using System;
using System.Collections.Generic;
using System.Linq;

public static class LinqExtensions
{
    // 手动实现 Chunk 方法
    public static IEnumerable<T[]> Chunk<T>(this IEnumerable<T> source, int size)
    {
        if (size <= 0) throw new ArgumentException("Chunk size must be positive.");

        using var enumerator = source.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var chunk = new T[size];
            chunk[0] = enumerator.Current;

            for (int i = 1; i < size; i++)
            {
                if (!enumerator.MoveNext())
                {
                    Array.Resize(ref chunk, i);
                    break;
                }
                chunk[i] = enumerator.Current;
            }

            yield return chunk;
        }
    }
}