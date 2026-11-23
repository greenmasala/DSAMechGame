using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Editor menu helpers for testing Ben's equipment (LaserGun, Shotgun, ShieldEquipment). Editor-only, not in builds.
/// Made for Tonnam's "Player in Terrain Demo" scene (it already has the Player, UI, map and Chai's guns).
///   Tools > Ben > Equipment Test Setup       -> adds Laser + Shotgun next to the GatlingGun, a Shield on the player,
///                                               a target dummy and a Turret, then equips Laser (LMB) + Shield (RMB)
///   Tools > Ben > Cycle Left Mouse Weapon    -> swaps LMB between Laser / Shotgun / Gatling / Artillery (in Play mode, Tab does the same)
///   Setup also adds EquipmentDevTools to the player (Tab to cycle guns + logs mouse clicks). Remove it before the real game.
///
/// Placeholder looks (primitive shapes, materials saved in Assets/Ben/Materials):
///   Laser   = long thin dark barrel, glowing cyan rings + tip, cyan beam (LineRenderer)
///   Shotgun = two short fat orange barrels side by side + brown stock
///   Shield  = see-through blue bubble around the player, only shown while the shield is up
///
/// Running Setup again removes the old "(Ben test)" objects first. Ctrl+Z undoes it.
/// The scene isn't saved for you - don't save it unless Tonnam is fine with it.
/// </summary>
public static class EquipmentTestMenu
{
    private const string TurretPath = "Assets/Ben/Prefabs/Turret.prefab";
    private const string MaterialFolder = "Assets/Ben/Materials";
    private const string TestTag = "(Ben test)";
    private const string UndoName = "Equipment Test Setup";

    [MenuItem("Tools/Ben/Equipment Test Setup")]
    public static void CreateTestSetup()
    {
        Player player = Object.FindFirstObjectByType<Player>();
        GatlingGun gatling = Object.FindFirstObjectByType<GatlingGun>(FindObjectsInactive.Include);
        if (player == null || gatling == null)
        {
            EditorUtility.DisplayDialog("Equipment Test Setup",
                "Couldn't find a Player and a GatlingGun in the open scene.\nOpen Assets/Tonnam/Scene/Player in Terrain Demo first.", "OK");
            return;
        }

        RemoveOldTestObjects();

        // Guns: copies of the GatlingGun (so they get its position + ShootPoint), with the script and model swapped
        LaserGun laser = CopyGun<LaserGun>(gatling, "LaserGun " + TestTag, new Vector3(-0.3f, 0f, 0f));
        laser.Name = "Laser";
        laser.InputType = EquipmentInputType.Click;
        laser.Damage = 50f;
        laser.FireRate = 0.5f;
        laser.Range = 200f;
        laser.EnergyCost = 25f;
        BuildLaserModel(laser);

        Shotgun shotgun = CopyGun<Shotgun>(gatling, "Shotgun " + TestTag, new Vector3(0.3f, 0f, 0f));
        shotgun.Name = "Shotgun";
        shotgun.InputType = EquipmentInputType.Click;
        shotgun.Damage = 10f;
        shotgun.FireRate = 0.8f;
        shotgun.Range = 15f;
        shotgun.Ammo = 20;
        BuildShotgunModel(shotgun);

        // Shield: script on an empty child of the player + a bubble that ShieldEquipment turns on/off
        GameObject shieldObj = new GameObject("Shield " + TestTag);
        shieldObj.transform.SetParent(player.transform, false);
        ShieldEquipment shield = shieldObj.AddComponent<ShieldEquipment>();
        shield.Name = "Shield";
        shield.InputType = EquipmentInputType.Hold;
        shield.ShieldVisual = BuildShieldBubble(shieldObj.transform, player.transform.position + Vector3.up);
        Undo.RegisterCreatedObjectUndo(shieldObj, UndoName);

        Vector3 forward = Vector3.ProjectOnPlane(player.transform.forward, Vector3.up).normalized;
        if (forward == Vector3.zero) forward = Vector3.forward;

        // Target dummy: plain Entity with 200 hp, 10m in front of the player. Watch its hp in the Inspector.
        GameObject dummy = GameObject.CreatePrimitive(PrimitiveType.Cube);
        dummy.name = "Target Dummy " + TestTag;
        dummy.transform.position = player.transform.position + forward * 10f + Vector3.up;
        dummy.transform.localScale = new Vector3(1.5f, 2f, 1.5f);
        Entity dummyEntity = dummy.AddComponent<Entity>();
        SerializedObject dummySo = new SerializedObject(dummyEntity);
        dummySo.FindProperty("hp").floatValue = 200f;
        dummySo.ApplyModifiedPropertiesWithoutUndo();
        Undo.RegisterCreatedObjectUndo(dummy, UndoName);

        // Turret: shoots the player, used to check the shield (damage should drop to 25%)
        GameObject turretPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(TurretPath);
        if (turretPrefab != null)
        {
            GameObject turret = (GameObject)PrefabUtility.InstantiatePrefab(turretPrefab);
            turret.name = "Turret " + TestTag;
            turret.transform.position = player.transform.position + forward * 25f + Vector3.Cross(Vector3.up, forward) * 8f;
            Undo.RegisterCreatedObjectUndo(turret, UndoName);
        }

        // Equip: LMB = Laser, RMB = Shield
        SetWeapon(player, 0, laser);
        SetWeapon(player, 1, shield);

        // Dev tools on the player: Tab cycles LMB through these 4 guns + mouse logging
        EquipmentDevTools dev = player.GetComponent<EquipmentDevTools>();
        if (dev == null) dev = Undo.AddComponent<EquipmentDevTools>(player.gameObject);
        Undo.RecordObject(dev, UndoName);
        dev.LeftMouseGuns = new Equipment[]
        {
            laser,
            shotgun,
            gatling,
            Object.FindFirstObjectByType<ArtilleryGun>(FindObjectsInactive.Include),
        };

        EditorSceneManager.MarkSceneDirty(player.gameObject.scene);
        Selection.activeGameObject = dummy;
        Debug.Log("[Ben] Equipment test ready: LMB = Laser, RMB = Shield. In Play mode press Tab to cycle LMB: Laser > Shotgun > Gatling > Artillery.");
    }

    [MenuItem("Tools/Ben/Cycle Left Mouse Weapon")]
    public static void CycleLeftWeapon()
    {
        Player player = Object.FindFirstObjectByType<Player>();
        if (player == null || player.Weapons == null || player.Weapons.Length == 0) return;

        Equipment[] options =
        {
            Object.FindFirstObjectByType<LaserGun>(FindObjectsInactive.Include),
            Object.FindFirstObjectByType<Shotgun>(FindObjectsInactive.Include),
            Object.FindFirstObjectByType<GatlingGun>(FindObjectsInactive.Include),
            Object.FindFirstObjectByType<ArtilleryGun>(FindObjectsInactive.Include),
        };

        int current = System.Array.IndexOf(options, player.Weapons[0]);
        for (int step = 1; step <= options.Length; step++)
        {
            Equipment next = options[(current + step + options.Length) % options.Length];
            if (next == null) continue;

            SetWeapon(player, 0, next);
            if (Application.isPlaying) player.UpdateText(0);
            Debug.Log($"[Ben] Left mouse is now: {next.Name}");
            return;
        }
    }

    // ---------- Looks ----------

    private static void BuildLaserModel(LaserGun laser)
    {
        Material body = GetMaterial("Laser_Body", new Color(0.15f, 0.17f, 0.2f));
        Material glow = GetMaterial("Laser_Glow", new Color(0.2f, 0.9f, 1f), emission: new Color(0f, 2.5f, 3f));

        Transform model = CreateModelRoot(laser);
        // Everything sits behind the ShootPoint (negative z), so the tip ends right at the muzzle
        AddPart(PrimitiveType.Cylinder, model, "Barrel", new Vector3(0f, 0f, -0.6f), new Vector3(0.08f, 0.6f, 0.08f), body, alongZ: true);
        AddPart(PrimitiveType.Cube, model, "Body", new Vector3(0f, -0.02f, -1.1f), new Vector3(0.2f, 0.22f, 0.5f), body);
        for (int i = 0; i < 3; i++)
        {
            AddPart(PrimitiveType.Cylinder, model, "Ring" + i, new Vector3(0f, 0f, -0.25f - i * 0.25f), new Vector3(0.13f, 0.02f, 0.13f), glow, alongZ: true);
        }
        AddPart(PrimitiveType.Sphere, model, "Tip", Vector3.zero, Vector3.one * 0.12f, glow);

        // Beam: LaserGun turns this on for a moment when it fires
        LineRenderer beam = laser.gameObject.AddComponent<LineRenderer>();
        beam.useWorldSpace = true;
        beam.positionCount = 2;
        beam.widthMultiplier = 0.08f;
        beam.shadowCastingMode = ShadowCastingMode.Off;
        beam.sharedMaterial = GetMaterial("Laser_Beam", new Color(0.3f, 1f, 1f), unlit: true);
        beam.enabled = false;
        laser.Beam = beam;
    }

    private static void BuildShotgunModel(Shotgun shotgun)
    {
        Material barrel = GetMaterial("Shotgun_Barrel", new Color(0.85f, 0.45f, 0.1f));
        Material stock = GetMaterial("Shotgun_Stock", new Color(0.35f, 0.2f, 0.1f));

        Transform model = CreateModelRoot(shotgun);
        AddPart(PrimitiveType.Cylinder, model, "BarrelLeft", new Vector3(-0.09f, 0f, -0.3f), new Vector3(0.16f, 0.3f, 0.16f), barrel, alongZ: true);
        AddPart(PrimitiveType.Cylinder, model, "BarrelRight", new Vector3(0.09f, 0f, -0.3f), new Vector3(0.16f, 0.3f, 0.16f), barrel, alongZ: true);
        AddPart(PrimitiveType.Cube, model, "Stock", new Vector3(0f, -0.05f, -0.8f), new Vector3(0.3f, 0.25f, 0.45f), stock);
    }

    private static GameObject BuildShieldBubble(Transform parent, Vector3 center)
    {
        Material bubble = GetMaterial("Shield_Bubble", new Color(0.3f, 0.6f, 1f, 0.25f),
                                      emission: new Color(0.1f, 0.3f, 0.8f), transparent: true);

        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.name = "ShieldBubble";
        Object.DestroyImmediate(sphere.GetComponent<Collider>()); // mustn't block bullets or the player's own shots
        sphere.transform.position = center;
        sphere.transform.localScale = Vector3.one * 3.5f;
        sphere.transform.SetParent(parent, true);
        Renderer r = sphere.GetComponent<Renderer>();
        r.sharedMaterial = bubble;
        r.shadowCastingMode = ShadowCastingMode.Off;
        sphere.SetActive(false); // ShieldEquipment shows it while the shield is up
        return sphere;
    }

    // Empty object at the ShootPoint, facing where the gun shoots. Model parts are built along its -Z.
    // The gatling's own model on the copy is hidden.
    private static Transform CreateModelRoot(GunEquipment gun)
    {
        foreach (Renderer r in gun.GetComponentsInChildren<Renderer>(true)) r.enabled = false;

        Transform shootPoint = gun.transform.Find("ShootPoint");
        Transform aim = shootPoint != null ? shootPoint : gun.transform;

        Transform root = new GameObject("Model").transform;
        root.SetPositionAndRotation(aim.position, aim.rotation);
        root.SetParent(gun.transform, true); // keep world size, whatever the gun's scale is
        return root;
    }

    private static void AddPart(PrimitiveType type, Transform parent, string name, Vector3 localPos, Vector3 scale, Material mat, bool alongZ = false)
    {
        GameObject part = GameObject.CreatePrimitive(type);
        part.name = name;
        Object.DestroyImmediate(part.GetComponent<Collider>()); // looks only, mustn't block the raycasts
        part.transform.SetParent(parent, false);
        part.transform.localPosition = localPos;
        part.transform.localRotation = alongZ ? Quaternion.Euler(90f, 0f, 0f) : Quaternion.identity; // cylinders point up by default
        part.transform.localScale = scale;
        Renderer r = part.GetComponent<Renderer>();
        r.sharedMaterial = mat;
        r.shadowCastingMode = ShadowCastingMode.Off;
    }

    // Makes (or updates) a URP material asset in Assets/Ben/Materials so the look survives saving the scene
    private static Material GetMaterial(string name, Color color, Color? emission = null, bool transparent = false, bool unlit = false)
    {
        if (!AssetDatabase.IsValidFolder(MaterialFolder)) AssetDatabase.CreateFolder("Assets/Ben", "Materials");

        string path = $"{MaterialFolder}/EquipTest_{name}.mat";
        Shader shader = Shader.Find(unlit ? "Universal Render Pipeline/Unlit" : "Universal Render Pipeline/Lit");
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat == null)
        {
            mat = new Material(shader);
            AssetDatabase.CreateAsset(mat, path);
        }
        mat.shader = shader;
        mat.SetColor("_BaseColor", color);

        if (emission.HasValue)
        {
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", emission.Value);
        }

        if (transparent)
        {
            mat.SetFloat("_Surface", 1f); // Transparent
            mat.SetFloat("_Blend", 0f);   // Alpha
            mat.SetFloat("_Cull", 0f);    // draw both sides, so you see the bubble from inside too
            mat.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
            mat.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
            mat.SetFloat("_ZWrite", 0f);
            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            mat.SetOverrideTag("RenderType", "Transparent");
            mat.renderQueue = (int)RenderQueue.Transparent;
        }

        EditorUtility.SetDirty(mat);
        return mat;
    }

    // ---------- Helpers ----------

    private static void RemoveOldTestObjects()
    {
        foreach (Transform t in Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (t != null && t.name.EndsWith(TestTag)) Undo.DestroyObjectImmediate(t.gameObject);
        }
    }

    // Copies the gatling gun object, removes GatlingGun and adds T in its place
    private static T CopyGun<T>(GatlingGun source, string name, Vector3 offset) where T : GunEquipment
    {
        GameObject copy = Object.Instantiate(source.gameObject, source.transform.parent);
        copy.name = name;
        copy.transform.localPosition = source.transform.localPosition + offset;
        copy.transform.localRotation = source.transform.localRotation;
        Object.DestroyImmediate(copy.GetComponent<GatlingGun>());
        T gun = copy.AddComponent<T>();
        Undo.RegisterCreatedObjectUndo(copy, UndoName);
        return gun;
    }

    private static void SetWeapon(Player player, int index, Equipment equipment)
    {
        Undo.RecordObject(player, UndoName);
        if (player.Weapons.Length <= index) System.Array.Resize(ref player.Weapons, index + 1);
        player.Weapons[index] = equipment;
        if (!Application.isPlaying) PrefabUtility.RecordPrefabInstancePropertyModifications(player);
    }
}
