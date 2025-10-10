using UnityEngine;

public class Vector3Utils
{
 
    public static Vector3 RandomVector3(float minX = -1f, float maxX = 1f, float minY = -1f, float maxY = 1f, float minZ = -1f, float maxZ = 1f)
    {
        return new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), Random.Range(minZ, maxZ));
    }
}
