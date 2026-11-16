using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor menu helpers for the Tank, Turret and their bullets (Ben). Editor-only, not included in builds.
///   Tools > Ben > Create Tank And Turret Prefabs -> makes Tank, Turret, Bullet, TurretBullet in Assets/Ben/Prefabs (placeholder shapes)
///   Tools > Ben > Tank Test Setup                -> adds ground + test player + a Tank + a Turret to the open scene
///
/// Running "Create" again overwrites the prefabs, so don't use it after you've put real models in them.
/// </summary>
public static class TankSetupMenu
{
    private const string PrefabFolder = "Assets/Ben/Prefabs";
    private const string TankPath = PrefabFolder + "/Tank.prefab";
    private const string TurretPath = PrefabFolder + "/Turret.prefab";
    private const string BulletPath = PrefabFolder + "/Bullet.prefab";
    private const string TurretBulletPath = PrefabFolder + "/TurretBullet.prefab";

    [MenuItem("Tools/Ben/Create Tank And Turret Prefabs")]
    public static void CreateAllPrefabs()
    {
        if (!AssetDatabase.IsValidFolder(PrefabFolder)) AssetDatabase.CreateFolder("Assets/Ben", "Prefabs");

        Bullet bullet = CreateBulletPrefab(BulletPath, "Bullet", new Vector3(0.3f, 0.3f, 0.3f), speed: 30f, lifetime: 4f);
        Bullet turretBullet = CreateBulletPrefab(TurretBulletPath, "TurretBullet", new Vector3(0.35f, 0.35f, 1.2f), speed: 90f, lifetime: 3f);
        CreateTankPrefab(bullet);
        CreateTurretPrefab(turretBullet);
    }

    [MenuItem("Tools/Ben/Tank Test Setup")]
    public static void CreateTestSetup()
    {
        JeepSetupMenu.EnsureGroundAndTestPlayer();

        if (AssetDatabase.LoadAssetAtPath<GameObject>(TankPath) == null ||
            AssetDatabase.LoadAssetAtPath<GameObject>(TurretPath) == null)
        {
            CreateAllPrefabs();
        }

        GameObject tank = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(TankPath));
        tank.transform.position = new Vector3(0f, 0f, -35f);
        Undo.RegisterCreatedObjectUndo(tank, "Tank Test Setup");

        GameObject turret = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(TurretPath));
        turret.transform.position = new Vector3(25f, 0f, -15f);
        Undo.RegisterCreatedObjectUndo(turret, "Tank Test Setup");

        Selection.activeGameObject = tank;
    }

    private static void CreateTankPrefab(Bullet bullet)
    {
        // Root: logic + physics. It never rotates, Top and Bottom do.
        GameObject root = new GameObject("Tank");
        Rigidbody rb = root.AddComponent<Rigidbody>();
        rb.mass = 2000f;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        BoxCollider col = root.AddComponent<BoxCollider>();
        col.center = new Vector3(0f, 0.9f, 0f);
        col.size = new Vector3(2.8f, 1.8f, 3.8f);

        // Bottom: hull and tracks. Front faces +Z.
        Transform bottom = new GameObject("Bottom").transform;
        bottom.SetParent(root.transform, false);
        AddShape(PrimitiveType.Cube, bottom, "Hull", new Vector3(0f, 0.6f, 0f), new Vector3(2.2f, 0.7f, 3.4f));
        AddShape(PrimitiveType.Cube, bottom, "TrackLeft", new Vector3(-1.2f, 0.4f, 0f), new Vector3(0.5f, 0.8f, 3.8f));
        AddShape(PrimitiveType.Cube, bottom, "TrackRight", new Vector3(1.2f, 0.4f, 0f), new Vector3(0.5f, 0.8f, 3.8f));
        AddShape(PrimitiveType.Cube, bottom, "FrontPlate", new Vector3(0f, 0.8f, 1.75f), new Vector3(1.6f, 0.3f, 0.2f)); // shows which way the hull faces

        // Top: head and barrel. A child of the root, NOT of Bottom, so they turn separately.
        Transform top = new GameObject("Top").transform;
        top.SetParent(root.transform, false);
        top.localPosition = new Vector3(0f, 1.1f, 0f);
        AddShape(PrimitiveType.Cube, top, "Head", new Vector3(0f, 0.3f, 0f), new Vector3(1.6f, 0.6f, 1.8f));
        Transform gun = AddGun(top, new Vector3(0f, 0.3f, 0.8f), thickness: 0.25f, length: 2f, out Transform muzzle);

        Tank tank = root.AddComponent<Tank>();
        SerializedObject so = new SerializedObject(tank);
        so.FindProperty("hp").floatValue = 100f;
        so.FindProperty("moveSpeed").floatValue = 3f;
        so.FindProperty("attackingRange").floatValue = 15f;
        so.FindProperty("damage").floatValue = 10f;
        so.FindProperty("fireRadius").floatValue = 25f;
        so.FindProperty("fireRate").floatValue = 1f;
        so.FindProperty("bulletPrefab").objectReferenceValue = bullet;
        so.FindProperty("muzzle").objectReferenceValue = muzzle;
        so.FindProperty("fireSound").intValue = (int)SoundType.Tank_Fire;
        so.FindProperty("bottom").objectReferenceValue = bottom;
        so.FindProperty("top").objectReferenceValue = top;
        so.FindProperty("gun").objectReferenceValue = gun;
        so.ApplyModifiedPropertiesWithoutUndo();

        Save(root, TankPath);
    }

    private static void CreateTurretPrefab(Bullet bullet)
    {
        GameObject root = new GameObject("Turret");
        Rigidbody rb = root.AddComponent<Rigidbody>();
        rb.isKinematic = true;

        BoxCollider col = root.AddComponent<BoxCollider>();
        col.center = new Vector3(0f, 1.25f, 0f);
        col.size = new Vector3(2f, 2.5f, 2f);

        // Bottom: fixed base.
        Transform bottom = new GameObject("Bottom").transform;
        bottom.SetParent(root.transform, false);
        AddShape(PrimitiveType.Cylinder, bottom, "Base", new Vector3(0f, 0.75f, 0f), new Vector3(2f, 0.75f, 2f));

        // Top: head with a long barrel.
        Transform top = new GameObject("Top").transform;
        top.SetParent(root.transform, false);
        top.localPosition = new Vector3(0f, 1.5f, 0f);
        AddShape(PrimitiveType.Cube, top, "Head", new Vector3(0f, 0.4f, 0f), new Vector3(1.4f, 0.8f, 1.4f));
        Transform gun = AddGun(top, new Vector3(0f, 0.4f, 0.7f), thickness: 0.2f, length: 3f, out Transform muzzle);

        Turret turret = root.AddComponent<Turret>();
        SerializedObject so = new SerializedObject(turret);
        so.FindProperty("hp").floatValue = 80f;
        so.FindProperty("moveSpeed").floatValue = 0f;
        so.FindProperty("attackingRange").floatValue = 0f; // doesn't move, so not used
        so.FindProperty("damage").floatValue = 35f;
        so.FindProperty("fireRadius").floatValue = 40f;
        so.FindProperty("fireRate").floatValue = 0.4f;
        so.FindProperty("bulletPrefab").objectReferenceValue = bullet;
        so.FindProperty("muzzle").objectReferenceValue = muzzle;
        so.FindProperty("fireSound").intValue = (int)SoundType.Turret_Fire;
        so.FindProperty("bottom").objectReferenceValue = bottom;
        so.FindProperty("top").objectReferenceValue = top;
        so.FindProperty("gun").objectReferenceValue = gun;
        so.FindProperty("topTurnSpeed").floatValue = 20f;
        so.FindProperty("tiltSpeed").floatValue = 10f;
        so.FindProperty("aimLag").floatValue = 1.2f;
        so.FindProperty("aimTolerance").floatValue = 3f;
        so.ApplyModifiedPropertiesWithoutUndo();

        Save(root, TurretPath);
    }

    private static Bullet CreateBulletPrefab(string path, string name, Vector3 size, float speed, float lifetime)
    {
        GameObject root = new GameObject(name);
        AddShape(PrimitiveType.Sphere, root.transform, "Model", Vector3.zero, size);

        Bullet bullet = root.AddComponent<Bullet>();
        SerializedObject so = new SerializedObject(bullet);
        so.FindProperty("speed").floatValue = speed;
        so.FindProperty("lifetime").floatValue = lifetime;
        so.ApplyModifiedPropertiesWithoutUndo();

        return Save(root, path).GetComponent<Bullet>();
    }

    private static GameObject Save(GameObject root, string path)
    {
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
        Debug.Log("Created " + path, prefab);
        return prefab;
    }

    // Gun = the pivot the barrel tilts around, placed at the back end of the barrel.
    // Barrel (a cylinder lying along +Z) and Muzzle go inside it so they tilt with it.
    private static Transform AddGun(Transform top, Vector3 pivotPosition, float thickness, float length, out Transform muzzle)
    {
        Transform gun = new GameObject("Gun").transform;
        gun.SetParent(top, false);
        gun.localPosition = pivotPosition;

        // Unity's cylinder stands along Y and is 2 units tall, so rotate it flat and scale Y to half the length.
        Transform barrel = AddShape(PrimitiveType.Cylinder, gun, "Barrel", new Vector3(0f, 0f, length / 2f), new Vector3(thickness, length / 2f, thickness));
        barrel.localRotation = Quaternion.Euler(90f, 0f, 0f);

        muzzle = new GameObject("Muzzle").transform;
        muzzle.SetParent(gun, false);
        muzzle.localPosition = new Vector3(0f, 0f, length + 0.1f);

        return gun;
    }

    private static Transform AddShape(PrimitiveType type, Transform parent, string name, Vector3 position, Vector3 scale)
    {
        GameObject shape = GameObject.CreatePrimitive(type);
        shape.name = name;
        shape.transform.SetParent(parent, false);
        shape.transform.localPosition = position;
        shape.transform.localScale = scale;
        Object.DestroyImmediate(shape.GetComponent<Collider>()); // the root collider handles collisions; bullets use raycasts
        return shape.transform;
    }
}
