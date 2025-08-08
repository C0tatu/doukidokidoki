using UnityEngine;
using UnityEditor;
using UdonSharp;
using VRC.Udon;

public class UdonCloner : EditorWindow
{
    private UdonSharpProgramAsset udonProgramAsset;

    [MenuItem("UdonCloner/AttachUdonScript")]
    public static void ShowWindow()
    {
        GetWindow<UdonCloner>("Batch Udon Attach");
    }

    void OnGUI()
    {
        GUILayout.Label("UdonSharp Batch Attacher", EditorStyles.boldLabel);
        udonProgramAsset = (UdonSharpProgramAsset)EditorGUILayout.ObjectField(
            "Udon Program Asset", udonProgramAsset, typeof(UdonSharpProgramAsset), false);

        if (GUILayout.Button("Attach to Selected Objects"))
        {
            AttachScriptToSelectedObjects();
        }
    }

    void AttachScriptToSelectedObjects()
    {
        if (udonProgramAsset == null)
        {
            Debug.LogError("Udon Program Asset is not set.");
            return;
        }

        GameObject[] selectedObjects = Selection.gameObjects;

        foreach (GameObject obj in selectedObjects)
        {
            if (obj == null) continue;

            UdonBehaviour udon = obj.GetComponent<UdonBehaviour>();
            if (udon == null)
            {
                udon = Undo.AddComponent<UdonBehaviour>(obj);
            }

            Undo.RecordObject(udon, "Assign Udon Program");
            udon.programSource = udonProgramAsset;
            EditorUtility.SetDirty(udon);
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"Udon script assigned to {selectedObjects.Length} objects.");
    }
}