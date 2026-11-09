using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor menu helpers for the Jeep (Ben). Editor-only, not included in builds.
///   Tools > Ben > Create Jeep Prefab      -> makes Assets/Ben/Prefabs/Jeep.prefab (placeholder boxes)
///   Tools > Ben > Jeep Test Setup         -> adds ground + test player + a Jeep to the open scene
/// </summary>
public static class JeepSetupMenu
{
    private const string PrefabFolder = "Assets/Ben/Prefabs";
    private const string PrefabPath = PrefabFolder + "/Jeep.prefab";

    [MenuItem("Tools/Ben/Create Jeep Prefab")]
    public static GameObject CreateJeepPrefab()
    {
        // Root: logic + physics. Keep these on the root when swapping in the real model.
        GameObject root = new GameObject("Jeep");
        Rigidbody rb = root.AddComponent<Rigidbody>();
        rb.mass = 800f;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        BoxCollider col = root.AddComponent<BoxCollider>();
        col.center = new Vector3(0f, 0.6f, 0f);
        col.size = new Vector3(2f, 1.2f, 3.5f);

        Jeep jeep = root.AddComponent<Jeep>();
        SerializedObject so = new SerializedObject(jeep);
        so.FindProperty("hp").floatValue = 30f;
        so.FindProperty("moveSpeed").floatValue = 12f;
        so.ApplyModifiedPropertiesWithoutUndo();

        // "Model": visuals only. Replace everything under it with the real Jeep model later.
        GameObject model = new GameObject("Model");
        model.transform.SetParent(root.transform, false);
        AddBox(model.transform, "Body", new Vector3(0f, 0.5f, 0f), new Vector3(2f, 0.8f, 3.5f));
        AddBox(model.transform, "Cabin", new Vector3(0f, 1.2f, -0.5f), new Vector3(1.8f, 0.6f, 1.8f)); // at the back, so you can see which way is forward

        if (!AssetDatabase.IsValidFolder(PrefabFolder)) AssetDatabase.CreateFolder("Assets/Ben", "Prefabs");
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
        Object.DestroyImmediate(root);

        Debug.Log("Created " + PrefabPath, prefab);
        return prefab;
    }

    [MenuItem("Tools/Ben/Jeep Test Setup")]
    public static void CreateTestSetup()
    {
        if (GameObject.Find("Ground") == null)
        {
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.localScale = new Vector3(10f, 1f, 10f); // 100 x 100
            Undo.RegisterCreatedObjectUndo(ground, "Jeep Test Setup");
        }

        // Stand-in player. Drag it around in the Scene view during Play mode to test.
        if (Object.FindAnyObjectByType<Player>() == null)
        {
            GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Test Player";
            player.transform.position = new Vector3(0f, 1f, 0f);
            Player p = player.AddComponent<Player>();
            p.Weapons = new Equipment[0];
            SerializedObject so = new SerializedObject(p);
            so.FindProperty("hp").floatValue = 100f;
            so.ApplyModifiedPropertiesWithoutUndo();
            Undo.RegisterCreatedObjectUndo(player, "Jeep Test Setup");
        }

        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
        if (prefab == null) prefab = CreateJeepPrefab();

        GameObject jeep = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        jeep.transform.position = new Vector3(0f, 0f, -25f);
        Undo.RegisterCreatedObjectUndo(jeep, "Jeep Test Setup");
        Selection.activeGameObject = jeep;
    }

    private static void AddBox(Transform parent, string name, Vector3 position, Vector3 size)
    {
        GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
        box.name = name;
        box.transform.SetParent(parent, false);
        box.transform.localPosition = position;
        box.transform.localScale = size;
        Object.DestroyImmediate(box.GetComponent<Collider>()); // the root BoxCollider handles collisions
    }
}
