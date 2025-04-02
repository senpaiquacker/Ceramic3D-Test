using UnityEngine;

public static class Matrix4x4Extension
{
    public static Matrix4x4 ConvertToUnitDeterminant(this Matrix4x4 matrix)
    {
        return matrix.DivideBy(matrix.determinant);
    }
    public static Quaternion ExtractRotation(this Matrix4x4 matrix)
    {
        Vector3 forward;
        forward.x = matrix.m02;
        forward.y = matrix.m12;
        forward.z = matrix.m22;

        Vector3 upwards;
        upwards.x = matrix.m01;
        upwards.y = matrix.m11;
        upwards.z = matrix.m21;

        return Quaternion.LookRotation(forward, upwards);
    }

    public static Vector3 ExtractPosition(this Matrix4x4 matrix)
    {
        Vector3 position;
        position.x = matrix.m03;
        position.y = matrix.m13;
        position.z = matrix.m23;
        return position;
    }

    public static Vector3 ExtractScale(this Matrix4x4 matrix)
    {
        Vector3 scale;
        scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
        scale.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
        scale.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
        return scale;
    }
    public static Matrix4x4 DivideBy(this Matrix4x4 matrix, float value)
    {
        return new Matrix4x4(
               matrix.GetColumn(0) / value,
               matrix.GetColumn(1) / value,
               matrix.GetColumn(2) / value,
               matrix.GetColumn(3) / value);
    }
    public static Matrix4x4 MultiplyBy(this Matrix4x4 matrix, float value)
    {
        return new Matrix4x4(
               matrix.GetColumn(0) * value,
               matrix.GetColumn(1) * value,
               matrix.GetColumn(2) * value,
               matrix.GetColumn(3) * value);
    }
    public static bool Equals(this Matrix4x4 matrix, Matrix4x4 other)
    {
        bool isEqual = true;
        for(int i = 0; i < 4; i++)
        {
            for(int j = 0; j < 4; j++)
            {
                if(matrix[i,j] != other[i,j])
                {
                    isEqual = false;
                    break;
                }
            }
        }
        return isEqual;
    }
}
