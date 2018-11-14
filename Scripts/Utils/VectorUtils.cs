using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class VectorUtils {

    public static Vector2 Centroid(this IEnumerable<Vector2> path)
    {
        Vector2 result = path.Aggregate(Vector2.zero, (current, point) => current + point);
        result /= path.Count();

        return result;
    }

    public static Vector3 Centroid(this IEnumerable<Vector3> path)
    {
        Vector3 result = path.Aggregate(Vector3.zero, (current, point) => current + point);
        result /= path.Count();

        return result;
    }
}
