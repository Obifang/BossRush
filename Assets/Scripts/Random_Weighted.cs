using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class Random_Weighted<T>
{
    public static T GetRandomObject(Dictionary<T, int> objs)
    {
        var totalWeight = objs.Keys.Sum(x => objs[x]);
        int rndm = Random.Range(0, totalWeight);
        int total = 0;

        foreach(T obj in objs.Keys) {
            total += objs[obj];
            if (rndm < total) {
                return obj;
            }
        }

        return objs.Keys.ToArray()[0];
    }
}
