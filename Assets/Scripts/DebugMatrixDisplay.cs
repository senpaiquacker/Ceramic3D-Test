using UnityEngine;
using System.Linq;
using UnityEditor;
using System.Collections.Generic;

public class DebugMatrixDisplay : MonoBehaviour
{
    [CustomEditor(typeof(DebugMatrixDisplay))]
    public class DebugMatrixDisplayEditor : Editor
    {
        private SerializedProperty progressProperty;
        private DebugMatrixDisplay targetRef;
        public void OnEnable()
        {
            targetRef = (DebugMatrixDisplay)target;
            progressProperty = serializedObject.FindProperty("progress");
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (Application.isPlaying && targetRef.spaceMatrices != null)
            {
                if (GUILayout.Button("Start Check"))
                {
                    targetRef.StartCoroutine(targetRef.StartChecking());
                }
                Rect r = EditorGUILayout.BeginVertical();
                EditorGUI.ProgressBar(r, ((float)progressProperty.intValue)/ (targetRef.spaceMatrices.Length * targetRef.modelMatrices.Length), "Check Progress");
                GUILayout.Space(40);
                EditorGUILayout.EndVertical();
            }
            
        }
    }

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
    [SerializeField, HideInInspector]
    private int progress = 0;
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
        
    }
    private void GenerateCubes()
    {
        var empty = new GameObject("Space Reference");
        empty.transform.position = Vector3.zero;
        spaceObjects = new GameObject[spaceMatrices.Length];
        for(int i = 0; i < spaceMatrices.Length; i++)
        {
            spaceObjects[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            spaceObjects[i].transform.parent = empty.transform;
            spaceObjects[i].transform.localPosition = spaceMatrices[i].ExtractPosition();
            spaceObjects[i].transform.localRotation = spaceMatrices[i].ExtractRotation();
            spaceObjects[i].transform.localScale = spaceMatrices[i].ExtractScale();
        }
    }
    private System.Collections.IEnumerator StartChecking()
    {
        commonMatrices = new List<Matrix4x4>();
        for (int i = 0; i < spaceMatrices.Length; i++)
        {
            var K = spaceMatrices[i] * modelMatrices[0].inverse;
            bool isValid = true;
            for (int j = 0; j < modelMatrices.Length; j++)
            {

                var transformed = K * modelMatrices[j];
                var found = spaceMatrices.Contains(transformed, new Matrix4x4EqualityComparer());
                yield return WaitForEndFrameWithProgressBar(1);
                if (!found)
                {
                    isValid = false;
                    yield return WaitForEndFrameWithProgressBar(modelMatrices.Length - j);
                    break;
                }
                
            }
            if (isValid)
                commonMatrices.Add(K);
        }
        JsonImportExtension.WriteToJson(commonMatrices.ToArray(), System.IO.Path.Combine(Application.persistentDataPath + "output.json"));
        StartCoroutine(SpawnCorrectCubes());
        progress = 0;
    }
    private System.Collections.IEnumerator SpawnCorrectCubes()
    {
        for (int k = 0; k < commonMatrices.Count; k++)
        {
            var modelsFound = new GameObject($"Found Cluster ({k})");

            var m = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            m.color = Color.HSVToRGB((1.0f / commonMatrices.Count) * k, 0.5f, 0.5f);
            for (int i = 0; i < modelMatrices.Length; i++)
            {
                var check = commonMatrices[k] * modelMatrices[i];

                var checkObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                checkObject.transform.parent = modelsFound.transform;
                checkObject.transform.localPosition = check.ExtractPosition();
                checkObject.transform.localRotation = check.ExtractRotation();
                checkObject.transform.localScale = check.ExtractScale();
                checkObject.GetComponent<MeshRenderer>().material = m;

            }
            yield return null;
            modelsFound.SetActive(false);
        }
    }

    private System.Collections.IEnumerator WaitForTheMessageFrame(string message)
    {
        Debug.Log(message);
        yield return null;
    }
    private System.Collections.IEnumerator WaitForEndFrameWithProgressBar(int p)
    {
        progress += p ;
        yield return null;
    }

}
