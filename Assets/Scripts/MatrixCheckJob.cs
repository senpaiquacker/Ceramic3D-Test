using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using System.Linq;
public struct MatrixCheckJob : IJob
{
    public NativeArray<bool> isValidsRef;
    public Matrix4x4 check;
    public Matrix4x4 modelMatrix;
    public int validId;
    public NativeArray<Matrix4x4> spaceMatricesRef;
    public void Execute()
    {
        var transformed = check * modelMatrix;
        var found = spaceMatricesRef.Contains(transformed, new Matrix4x4EqualityComparer());
        if (!found)
            isValidsRef[validId] = false;
    }
}
