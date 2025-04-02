using UnityEngine;
using System.Linq;
using System.Collections.Generic;
public class DebugMatrixDisplay : MonoBehaviour
{
    [SerializeField]
    private TextAsset modelsJson;
    [SerializeField]
    private TextAsset spaceJson;
    private Matrix4x4[] modelMatrices;
    [SerializeField]
    private GameObject[] spaceObjects;
    private Matrix4x4[] spaceMatrices;
    [SerializeField]
    List<Matrix4x4> commonMatrices;
    void Start()
    {
        if (modelsJson != null)
        {
            modelMatrices = JsonImportExtension.To4x4Array(modelsJson.text);
        }
        if(spaceJson != null)
        {
            spaceMatrices = JsonImportExtension.To4x4Array(spaceJson.text);
            GenerateCubes();
        }
        StartCoroutine(StartChecking());
    }
    private void GenerateCubes()
    {
        var empty = new GameObject("Space Reference");
        empty.transform.position = Vector3.zero;
        spaceObjects = new GameObject[spaceMatrices.Length];
        for(int i = 0; i < spaceMatrices.Length; i++)
        {
            var unit = spaceMatrices[i].ConvertToUnitDeterminant();
            {
                spaceObjects[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                spaceObjects[i].transform.parent = empty.transform;
                spaceObjects[i].transform.localPosition = spaceMatrices[i].ExtractPosition();
                spaceObjects[i].transform.localRotation = spaceMatrices[i].ExtractRotation();
                spaceObjects[i].transform.localScale = spaceMatrices[i].ExtractScale();
            }
        }
    }
    private System.Collections.IEnumerator StartChecking()
    {
        var commonInterlude = new List<Matrix4x4>();
        commonMatrices = new List<Matrix4x4>();
        for (int i = 0; i < spaceMatrices.Length; i++)
        {
            commonInterlude.Add(spaceMatrices[i] * modelMatrices[0].inverse);
            yield return WaitForTheMessageFrame($"Getting K for space matrix {i}");
        }
        for(int i = 0; i < commonInterlude.Count; i++)
        {
            bool containsAll = true;
            for(int j = 1; j < modelMatrices.Length; j++)
            {
                if(!spaceMatrices.Contains(modelMatrices[j] * commonInterlude[i]))
                {
                    containsAll = false;
                    break;
                }

            }
            yield return WaitForTheMessageFrame($"Reading K of {i} spaceMatrix");
            if (containsAll)
                commonMatrices.Add(commonInterlude[i]);
        }
    }
    private System.Collections.IEnumerator WaitForTheMessageFrame(string message)
    {
        Debug.Log(message);
        yield return null;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
