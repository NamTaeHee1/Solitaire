using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesCache<T> where T : Object
{
    private static Dictionary<string, T> loadCache = new Dictionary<string, T>();
    private static Dictionary<string, T[]> loadAllCache = new Dictionary<string, T[]>();

    public static T Load(string path)
    {
        if (loadCache.TryGetValue(path, out T value) == false)
        {
            value = Resources.Load<T>(path);

            loadCache.Add(path, value);
        }

        return value;
    }

    public static T[] LoadAll(string path)
    {
        if (loadAllCache.TryGetValue(path, out T[] value) == false)
        {
            value = Resources.LoadAll<T>(path);

            loadAllCache.Add(path, value);
        }

        return value;
    }
}
