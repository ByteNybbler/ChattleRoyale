// Author(s): Paul Calande
// Helper functions for ordered collections.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class UtilCollection
{
    // Returns the element with the highest value.
    public static T GetLargestElement<T>(IEnumerable<T> collection)
    {
        return collection.OrderByDescending(x => x).First();
    }

    // Returns true if a tie exists in the number 1 spot.
    // TODO: This function is broken?? Fix it!
    public static bool IsLargestElementTied<T>(IEnumerable<T> collection)
    {
        List<T> ordered = collection.OrderByDescending(x => x).ToList();
        return UtilGeneric.IsEqualTo(ordered[0], ordered[1]);
    }

    // Gets the first key that has the given value.
    // Returns false if the given value doesn't exist in the dictionary.
    public static bool TryGetKey<TKey, TValue>(this Dictionary<TKey, TValue> dict,
        TValue value, out TKey key)
    {
        key = dict.FirstOrDefault(x => UtilGeneric.IsEqualTo(x.Value, value)).Key;
        return dict.ContainsValue(value);
    }

    // Returns the sum of every value in the collection.
    public static float Sum(this IEnumerable<float> values)
    {
        float result = 0.0f;
        foreach (float val in values)
        {
            result += val;
        }
        return result;
    }
    public static int Sum(this IEnumerable<int> values)
    {
        int result = 0;
        foreach (int val in values)
        {
            result += val;
        }
        return result;
    }

    // Returns the product of every value in the collection.
    public static float Product(this IEnumerable<float> values)
    {
        float result = 1.0f;
        foreach (float val in values)
        {
            result *= val;
        }
        return result;
    }
    public static int Product(this IEnumerable<int> values)
    {
        int result = 1;
        foreach (int val in values)
        {
            result *= val;
        }
        return result;
    }
}