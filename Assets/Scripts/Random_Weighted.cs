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
        int total = rndm;

        foreach(T obj in objs.Keys) {
            if (rndm < total) {
                return obj;
            }
            total += objs[obj];
        }

        return objs.Keys.ToArray()[0];
    }
}
