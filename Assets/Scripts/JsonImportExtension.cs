using UnityEngine;
using System.Linq;
using System.Globalization;

public static class JsonImportExtension
{
    public static Matrix4x4[] To4x4Array(string json)
    {
        Matrix4x4Array jsons = JsonUtility.FromJson<Matrix4x4Array>("{\"matrices\":" + json + "}");
        return jsons.matrices.Select(a => a.ToRealMatrix()).ToArray();
    }
    public static void WriteToJson(Matrix4x4[] matrices, string filename)
    {
        Matrix4x4Array mJsons = new Matrix4x4Array();
        mJsons.matrices = matrices.Select(a => JsonMatrix4x4.FromRealMatrix(a)).ToArray();
        System.IO.File.WriteAllText(filename, JsonUtility.ToJson(mJsons));
    }
    [System.Serializable]
    private class Matrix4x4Array
    {
        public JsonMatrix4x4[] matrices;
    }
    [System.Serializable]
    private class JsonMatrix4x4
    {
        public string m00;
        public string m01;
        public string m02;
        public string m03;
        public string m10;
        public string m11;
        public string m12;
        public string m13;
        public string m20;
        public string m21;
        public string m22;
        public string m23;
        public string m30;
        public string m31;
        public string m32;
        public string m33;
        public override string ToString()
        {
            return $"{m00} {m01} {m02} {m03}\n" +
                   $"{m10} {m11} {m12} {m13}\n" +
                   $"{m20} {m21} {m22} {m23}\n" +
                   $"{m30} {m31} {m32} {m33}";
        }
        public Matrix4x4 ToRealMatrix()
        {
            
            var answ = new Matrix4x4(
                new Vector4(float.Parse(m00, CultureInfo.InvariantCulture), 
                            float.Parse(m10, CultureInfo.InvariantCulture), 
                            float.Parse(m20, CultureInfo.InvariantCulture), 
                            float.Parse(m30, CultureInfo.InvariantCulture)),
                new Vector4(float.Parse(m01, CultureInfo.InvariantCulture), 
                            float.Parse(m11, CultureInfo.InvariantCulture), 
                            float.Parse(m21, CultureInfo.InvariantCulture), 
                            float.Parse(m31, CultureInfo.InvariantCulture)),
                new Vector4(float.Parse(m02, CultureInfo.InvariantCulture), 
                            float.Parse(m12, CultureInfo.InvariantCulture), 
                            float.Parse(m22, CultureInfo.InvariantCulture), 
                            float.Parse(m32, CultureInfo.InvariantCulture)),
                new Vector4(float.Parse(m03, CultureInfo.InvariantCulture), 
                            float.Parse(m13, CultureInfo.InvariantCulture), 
                            float.Parse(m23, CultureInfo.InvariantCulture), 
                            float.Parse(m33, CultureInfo.InvariantCulture))
                );
            return answ;
        }
        public static JsonMatrix4x4 FromRealMatrix(Matrix4x4 real)
        {
            var answ = new JsonMatrix4x4();
            answ.m00 = real.m00.ToString(); answ.m10 = real.m10.ToString(); answ.m20 = real.m20.ToString(); answ.m30 = real.m30.ToString();
            answ.m00 = real.m01.ToString(); answ.m11 = real.m10.ToString(); answ.m21 = real.m20.ToString(); answ.m31 = real.m30.ToString();
            answ.m00 = real.m02.ToString(); answ.m12 = real.m10.ToString(); answ.m22 = real.m20.ToString(); answ.m32 = real.m30.ToString();
            answ.m00 = real.m03.ToString(); answ.m13 = real.m10.ToString(); answ.m23 = real.m20.ToString(); answ.m33 = real.m30.ToString();
            return answ;
        }
    }
}
