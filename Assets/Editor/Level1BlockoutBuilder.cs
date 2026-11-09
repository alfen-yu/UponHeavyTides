using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.AI;
using Unity.AI.Navigation;
using UponHeavyTides.AI;
using UponHeavyTides.Interaction;
using UponHeavyTides.Player;
using UponHeavyTides.Level0;

namespace UponHeavyTides.Editor
{
    public static class Level1BlockoutBuilder
    {
        private const string ScenePath = "Assets/Scenes/Level1.unity";
        private const string MaterialsFolder = "Assets/Materials";

        // Asset Prefab Paths
        private const string PrefabSandMat = "Assets/Shaded Spectrum/Sun And Sea Beach Materials Pack/Materials/Golden Shore Sand.mat";

        // Predatory fish prefab
        private const string PrefabSharkV1 = "Assets/Alstra Infinite/Fish - PolyPack/Prefabs/SharkV1.prefab";

        // New Pixit undersea assets
        private const string PrefabSeaShell = "Assets/PixitGames/Pixit3D/PixitUnderseaCollection/LowPolyFishFree/Prefabs/SeaShell_Static_Unity.prefab";

        // Kelp & Seaweed prefabs
        private const string PrefabFishHookKelp1 = "Assets/Jonathan Foust/Prefabs/Fish-Hook Kelp_1.prefab";
        private const string PrefabFishHookKelp2 = "Assets/Jonathan Foust/Prefabs/Fish-Hook Kelp_2.prefab";
        private const string PrefabFishHookKelp3 = "Assets/Jonathan Foust/Prefabs/Fish-Hook Kelp_3.prefab";
        private const string PrefabFishHookKelpWall = "Assets/Jonathan Foust/Prefabs/Fish-Hook Kelp Bush Wall.prefab";
        private const string PrefabFishHookKelpRadial = "Assets/Jonathan Foust/Prefabs/Fish-Hook Kelp Bush_Radial.prefab";

        // Rock Prefabs
        private const string PrefabRock1A = "Assets/Rocks and Boulders 2/Rocks/Prefabs/Rock1A.prefab";
        private const string PrefabRock1B = "Assets/Rocks and Boulders 2/Rocks/Prefabs/Rock1B.prefab";
        private const string PrefabRock1C = "Assets/Rocks and Boulders 2/Rocks/Prefabs/Rock1C.prefab";
        private const string PrefabRock1D = "Assets/Rocks and Boulders 2/Rocks/Prefabs/Rock1D.prefab";
        private const string PrefabRock1E = "Assets/Rocks and Boulders 2/Rocks/Prefabs/Rock1E.prefab";
        private const string PrefabRock1Grp1 = "Assets/Rocks and Boulders 2/Rocks/Prefabs/Rock1_grup1.prefab";
        private const string PrefabRock1Grp2 = "Assets/Rocks and Boulders 2/Rocks/Prefabs/Rock1_grup2.prefab";
        private const string PrefabRock1Grp3 = "Assets/Rocks and Boulders 2/Rocks/Prefabs/Rock1_grup3.prefab";
        private const string PrefabRock1Grp4 = "Assets/Rocks and Boulders 2/Rocks/Prefabs/Rock1_grup4.prefab";
        private const string PrefabRock1Grp5 = "Assets/Rocks and Boulders 2/Rocks/Prefabs/Rock1_grup5.prefab";
        private const string PrefabRock2 = "Assets/Rocks and Boulders 2/Rocks/Prefabs/Rock2.prefab";
        private const string PrefabRock3 = "Assets/Rocks and Boulders 2/Rocks/Prefabs/Rock3.prefab";
        private const string PrefabRock4A = "Assets/Rocks and Boulders 2/Rocks/Prefabs/Rock4A.prefab";
        private const string PrefabRock4B = "Assets/Rocks and Boulders 2/Rocks/Prefabs/Rock4B.prefab";
        private const string PrefabRock5A = "Assets/Rocks and Boulders 2/Rocks/Prefabs/Rock5A.prefab";
        private const string PrefabRock6A = "Assets/Rocks and Boulders 2/Rocks/Prefabs/Rock6A.prefab";

        // Flora & Photogrammetry Plants
        private const string PrefabWaterLilyFlower = "Assets/Jonathan Foust/Prefabs/Water Lily Flower.prefab";
        private const string PrefabWaterLilyLeaf = "Assets/Jonathan Foust/Prefabs/Water Lily Leaf.prefab";
        private const string PrefabRafflesia = "Assets/Jonathan Foust/Prefabs/Rafflesia.prefab";
        private const string PrefabSancheziaStar = "Assets/Jonathan Foust/Prefabs/Sanchezia Star.prefab";
        private const string PrefabDayAreshaBush1 = "Assets/Jonathan Foust/Prefabs/Day Aresha Bush_1.prefab";
        private const string PrefabDayAreshaBush2 = "Assets/Jonathan Foust/Prefabs/Day Aresha Bush_2.prefab";
        private const string PrefabGrassA1 = "Assets/Jonathan Foust/Prefabs/Grass A_1.prefab";
        private const string PrefabGrassA2 = "Assets/Jonathan Foust/Prefabs/Grass A_2.prefab";
        private const string PrefabGrassB1 = "Assets/Jonathan Foust/Prefabs/Grass B_1.prefab";
        private const string PrefabGrassB2 = "Assets/Jonathan Foust/Prefabs/Grass B_2.prefab";
        private const string PrefabBloodChuteCluster = "Assets/Jonathan Foust/Prefabs/Blood Chute Cluster.prefab";
        private const string PrefabSpinReef1A = "Assets/Splash of Color - Unique Photogrammetry Plants/Prefabs/High Resolution Prefabs/Spin Reef/Spin Reef 1A.prefab";
        private const string PrefabSpinReefFlower1A = "Assets/Splash of Color - Unique Photogrammetry Plants/Prefabs/High Resolution Prefabs/Spin Reef/Spin Reef Flower 1A.prefab";
        private const string PrefabAntlerSprout1A = "Assets/Splash of Color - Unique Photogrammetry Plants/Prefabs/High Resolution Prefabs/Antler Sprout/Antler Sprout 1A.prefab";
        private const string PrefabPadderPod1A = "Assets/Splash of Color - Unique Photogrammetry Plants/Prefabs/High Resolution Prefabs/Padder Pod/Padder Pod 1A.prefab";
        private const string PrefabBasinBushA = "Assets/Splash of Color - Unique Photogrammetry Plants/Prefabs/High Resolution Prefabs/Basin Bush/Basin Bush A.prefab";
        private const string PrefabPeacockPlant1A = "Assets/Splash of Color - Unique Photogrammetry Plants/Prefabs/High Resolution Prefabs/Peacock Plant/Peacock Plant 1A.prefab";
        private const string PrefabChewChute1A = "Assets/Splash of Color - Unique Photogrammetry Plants/Prefabs/High Resolution Prefabs/Chew Chute/Chew Chute 1A.prefab";
        private const string PrefabRazorRoot1A = "Assets/Splash of Color - Unique Photogrammetry Plants/Prefabs/High Resolution Prefabs/Razor Root/Razor Root 1A.prefab";
        private const string PrefabFilterFruit1A = "Assets/Splash of Color - Unique Photogrammetry Plants/Prefabs/High Resolution Prefabs/Filter Fruit/Filter Fruit 1A.prefab";

        // Rigged Crab Companion & Pufferfish NPC
        private const string PrefabCrab = "Assets/PixitGames/Pixit3D/PixitUnderseaCollection/LowPolyFishFree/Prefabs/Crab_Rigged_Unity.prefab";
        private const string PrefabPufferfish = "Assets/PixitGames/Pixit3D/PixitUnderseaCollection/LowPolyFishFree/Prefabs/BabyPutterfish_Rigged_Unity.prefab";

        [MenuItem("Upon Heavy Tides/Build Level 1 Blockout")]
        public static void BuildLevel1()
        {
            if (EditorSceneManager.GetActiveScene().path != ScenePath)
            {
                EditorSceneManager.OpenScene(ScenePath);
            }

            Undo.IncrementCurrentGroup();
            int undoGroup = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Build Level 1 Kelp Forest Pacman Maze");

            // 1. Setup Materials (Golden Shore Sand identical to Level 0)
            Material matSand = AssetDatabase.LoadAssetAtPath<Material>(PrefabSandMat);
            if (matSand == null) matSand = GetOrCreateMaterial("Mat_SandLevel1", new Color(0.85f, 0.78f, 0.62f), 0.1f, 0.05f);

            Material matRock = AssetDatabase.LoadAssetAtPath<Material>($"{MaterialsFolder}/Rock.mat");
            if (matRock == null) matRock = GetOrCreateMaterial("Mat_RockLevel1", new Color(0.42f, 0.40f, 0.38f), 0.2f, 0.1f);

            Material matKelp = GetOrCreateMaterial("Mat_KelpGreen", new Color(0.18f, 0.48f, 0.22f), 0.2f, 0.1f);
            Material matKelpDense = GetOrCreateMaterial("Mat_KelpDense", new Color(0.09f, 0.32f, 0.14f), 0.3f, 0.1f);
            Material matHermitCrab = GetOrCreateMaterial("Mat_HermitCrab", new Color(0.92f, 0.38f, 0.22f), 0.4f, 0.2f);
            Material matBarracuda = GetOrCreateMaterial("Mat_Barracuda", new Color(0.22f, 0.35f, 0.52f), 0.6f, 0.7f);
            Material matVisionCone = GetOrCreateMaterial("Mat_VisionCone", new Color(0.2f, 0.8f, 0.9f, 0.35f), 0.1f, 0.0f, true);
            Material matHermitShell = GetOrCreateMaterial("Mat_HermitShell", new Color(1.0f, 0.82f, 0.22f), 0.6f, 0.6f);
            Material matClueTablet = GetOrCreateMaterial("Mat_ClueTablet", new Color(0.25f, 0.85f, 0.95f), 0.8f, 0.3f);
            Material matHidingGrove = GetOrCreateMaterial("Mat_HidingGrove", new Color(0.12f, 0.65f, 0.35f, 0.45f), 0.1f, 0.0f, true);

            // 2. Atmosphere & Lighting
            SetupAtmosphere();

            // 3. Ground Setup
            GameObject ground = GameObject.Find("Ground");
            if (ground == null)
            {
                ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
                ground.name = "Ground";
                Undo.RegisterCreatedObjectUndo(ground, "Create Ground");
            }
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = new Vector3(8f, 1f, 8f); // 80m x 80m play area
            ground.GetComponent<MeshRenderer>().sharedMaterial = matSand;

            NavMeshSurface navSurface = ground.GetComponent<NavMeshSurface>();
            if (navSurface == null) navSurface = ground.AddComponent<NavMeshSurface>();
            navSurface.collectObjects = Unity.AI.Navigation.CollectObjects.All;
            navSurface.useGeometry = UnityEngine.AI.NavMeshCollectGeometry.PhysicsColliders;

            // 4. Setup Root Hierarchy
            GameObject envRoot = GetOrCreateRoot("_ENVIRONMENT");
            GameObject propsRoot = GetOrCreateRoot("_PROPS_AND_INTERACTIONS");
            GameObject npcRoot = GetOrCreateRoot("_NPCS_AND_ENEMIES");
            GameObject objectivesRoot = GetOrCreateRoot("_OBJECTIVES_AND_CLUES");
            GameObject managersRoot = GetOrCreateRoot("_MANAGERS");

            ClearChildren(envRoot);
            ClearChildren(propsRoot);
            ClearChildren(npcRoot);
            ClearChildren(objectivesRoot);
            ClearChildren(managersRoot);

            // Managers
            if (managersRoot.GetComponent<DialogueManager>() == null) managersRoot.AddComponent<DialogueManager>();
            if (managersRoot.GetComponent<UponHeavyTides.Level1.Level1Controller>() == null) managersRoot.AddComponent<UponHeavyTides.Level1.Level1Controller>();

            // 5. Boundary Perimeter Cliffs with natural rocks & invisible walls
            BuildPerimeterCliffs(envRoot.transform, matRock);

            // 6. Partition: 1/4 Area (South) & 3/4 Area (North) with Left Opening & Arch
            BuildDividingRidge(envRoot.transform, matRock);

            // 7. Player & Crab Companion Spawn
            GameObject player = GameObject.Find("Player");
            if (player != null)
            {
                player.transform.position = new Vector3(0f, 1f, -28f);
                player.transform.rotation = Quaternion.identity;
                player.tag = "Player";
                if (player.GetComponent<PlayerMovement>() == null) player.AddComponent<PlayerMovement>();
                if (player.GetComponent<PlayerPusher>() == null) player.AddComponent<PlayerPusher>();
            }

            // 7. Crab Companion: Find root Crab, clean nested models, and setup single 3D rigged companion
            GameObject crab = null;
            foreach (var go in EditorSceneManager.GetActiveScene().GetRootGameObjects())
            {
                if (go.name == "Crab")
                {
                    crab = go;
                    break;
                }
            }
            if (crab == null)
            {
                crab = new GameObject("Crab");
                Undo.RegisterCreatedObjectUndo(crab, "Create Crab");
            }
            crab.transform.position = new Vector3(-2.2f, 0.4f, -28f);
            crab.transform.rotation = Quaternion.identity;

            // Remove any primitive mesh/colliders on Crab root
            var sphereRenderer = crab.GetComponent<MeshRenderer>();
            if (sphereRenderer != null) Object.DestroyImmediate(sphereRenderer);
            var sphereFilter = crab.GetComponent<MeshFilter>();
            if (sphereFilter != null) Object.DestroyImmediate(sphereFilter);
            var sphereCollider = crab.GetComponent<Collider>();
            if (sphereCollider != null) Object.DestroyImmediate(sphereCollider);
            var crabDialogue = crab.GetComponent<DialogueTrigger>();
            if (crabDialogue != null) Object.DestroyImmediate(crabDialogue);

            // Clean ALL children to completely eliminate nested/duplicate crab models
            for (int i = crab.transform.childCount - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(crab.transform.GetChild(i).gameObject);
            }

            // Single Visual 3D Crab model child using rigged Crab prefab matching Level 0
            GameObject crabVisual = InstantiatePrefab(PrefabCrab, crab.transform, crab.transform.position, Quaternion.identity, new Vector3(1.2f, 1.2f, 1.2f), true);
            crabVisual.name = "Visual_Crab_3D";
            crabVisual.transform.localPosition = Vector3.zero;
            crabVisual.transform.localRotation = Quaternion.identity;

            // Rename the internal mesh child so GameObject.Find("Crab") will NEVER match the inner mesh!
            var innerMesh = crabVisual.transform.Find("Crab");
            if (innerMesh != null) innerMesh.name = "Crab_RiggedMesh";

            var companion = crab.GetComponent<Companion>();
            if (companion == null) companion = crab.AddComponent<Companion>();
            companion.FollowPlayer();

            // Main Camera Tracker
            GameObject mainCam = GameObject.Find("Main Camera");
            if (mainCam != null && player != null)
            {
                var tracker = mainCam.GetComponent<CameraTracker>();
                if (tracker == null) tracker = mainCam.AddComponent<CameraTracker>();
                tracker.target = player.transform;
                tracker.distance = 6.5f;
                tracker.height = 3.5f;
                mainCam.transform.position = player.transform.position + new Vector3(0f, 3.5f, -6.5f);
                mainCam.transform.LookAt(player.transform.position + Vector3.up * 1.2f);
            }

            // 8. Hermit Crab Sanctuary House (Grotto + intact Hermit Crab NPC)
            BuildHermitCrabHouse(npcRoot.transform, matHermitCrab, matRock, matSand);

            // 9. Lush Flora, Flowers, Corals & Sea Grass around the Hermit House area (1/4 South)
            BuildSouthFloraAndDecorations(envRoot.transform);

            // 10. Pacman-Style Kelp Maze (Long seaweed prefabs + invisible collision barriers + 3 hiding zones)
            BuildPacmanKelpMaze(envRoot.transform, matKelp, matKelpDense, matHidingGrove);

            // 11. Predatory Patrol Groups: Sharks (SharkV1) with AI State Machine & detection arcs
            BuildPredatoryPatrolGroups(npcRoot.transform, matBarracuda, matVisionCone);

            // 12. Pushable Props in the Maze (Natural 3D rocks with Rigidbody & Pushable)
            BuildPushableProps(propsRoot.transform, matRock, matSand);

            // 13. Objectives: The Lost Golden Shell (Center Chamber) & Monster Clue Tablet (North)
            BuildObjectivesAndClues(objectivesRoot.transform, matHermitShell, matClueTablet, matRock);

            // Clean up old standalone Rock if present
            GameObject oldRock = GameObject.Find("Rock");
            if (oldRock != null && oldRock.transform.parent == null)
            {
                Undo.DestroyObjectImmediate(oldRock);
            }

            // 14. Bake Navigation Mesh
            navSurface.BuildNavMesh();

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveOpenScenes();

            Undo.CollapseUndoOperations(undoGroup);
            Debug.Log("<color=green><b>[Upon Heavy Tides]</b> Level 1 fully decorated with imported seaweed, flowers, natural rocks, and predatory sharks!</color>");
        }

        private static void SetupAtmosphere()
        {
            GameObject dirLight = GameObject.Find("Directional Light");
            if (dirLight != null)
            {
                Light l = dirLight.GetComponent<Light>();
                if (l != null)
                {
                    l.color = new Color(0.42f, 0.76f, 0.65f); // Deep Kelp Emerald
                    l.intensity = 1.15f;
                }
                dirLight.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            }

            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogDensity = 0.018f;
            RenderSettings.fogColor = new Color(0.08f, 0.24f, 0.18f); // Murky olive green
            RenderSettings.ambientSkyColor = new Color(0.12f, 0.35f, 0.28f);
        }

        private static void BuildPerimeterCliffs(Transform parent, Material matRock)
        {
            GameObject wallGroup = new GameObject("PerimeterCliffs");
            wallGroup.transform.SetParent(parent);

            float halfSize = 40f;
            float wallHeight = 12f;
            float wallThickness = 4f;
            float wallLength = 80f;

            // Physical boundary invisible walls so nothing falls into the void
            CreateInvisibleWall(wallGroup.transform, new Vector3(0f, wallHeight / 2, halfSize), new Vector3(wallLength, wallHeight, wallThickness), "Cliff_Barrier_North");
            CreateInvisibleWall(wallGroup.transform, new Vector3(0f, wallHeight / 2, -halfSize), new Vector3(wallLength, wallHeight, wallThickness), "Cliff_Barrier_South");
            CreateInvisibleWall(wallGroup.transform, new Vector3(-halfSize, wallHeight / 2, 0f), new Vector3(wallThickness, wallHeight, wallLength), "Cliff_Barrier_West");
            CreateInvisibleWall(wallGroup.transform, new Vector3(halfSize, wallHeight / 2, 0f), new Vector3(wallThickness, wallHeight, wallLength), "Cliff_Barrier_East");

            // Dense jagged 3D rock cliffs lining the perimeter
            string[] cliffPrefabs = new string[] { PrefabRock2, PrefabRock3, PrefabRock5A, PrefabRock6A, PrefabRock1A };

            // South border
            for (float x = -36f; x <= 36f; x += 11f)
            {
                string p = cliffPrefabs[Mathf.Abs((int)x) % cliffPrefabs.Length];
                InstantiatePrefab(p, wallGroup.transform, new Vector3(x, 0f, -halfSize + Random.Range(0f, 2.0f)), Quaternion.Euler(0f, Random.Range(0f, 360f), 0f), Vector3.one * Random.Range(0.85f, 1.25f), false);
            }

            // North border
            for (float x = -36f; x <= 36f; x += 11f)
            {
                string p = cliffPrefabs[Mathf.Abs((int)x) % cliffPrefabs.Length];
                InstantiatePrefab(p, wallGroup.transform, new Vector3(x, 0f, halfSize - Random.Range(0f, 2.0f)), Quaternion.Euler(0f, Random.Range(0f, 360f), 0f), Vector3.one * Random.Range(0.85f, 1.25f), false);
            }

            // West border
            for (float z = -34f; z <= 34f; z += 11f)
            {
                string p = cliffPrefabs[Mathf.Abs((int)z) % cliffPrefabs.Length];
                InstantiatePrefab(p, wallGroup.transform, new Vector3(-halfSize + Random.Range(0f, 2.0f), 0f, z), Quaternion.Euler(0f, Random.Range(0f, 360f), 0f), Vector3.one * Random.Range(0.85f, 1.25f), false);
            }

            // East border
            for (float z = -34f; z <= 34f; z += 11f)
            {
                string p = cliffPrefabs[Mathf.Abs((int)z) % cliffPrefabs.Length];
                InstantiatePrefab(p, wallGroup.transform, new Vector3(halfSize - Random.Range(0f, 2.0f), 0f, z), Quaternion.Euler(0f, Random.Range(0f, 360f), 0f), Vector3.one * Random.Range(0.85f, 1.25f), false);
            }
        }

        private static void BuildDividingRidge(Transform parent, Material matRock)
        {
            GameObject ridgeGroup = new GameObject("Dividing_Ridge_Boundary_1_4_Split");
            ridgeGroup.transform.SetParent(parent);

            float wallHeight = 9f;
            float thickness = 4.0f;

            // SINGLE OPENING ON THE LEFT SIDE: from X = -24 to X = -16 (centered at X = -20)
            // Left Wall Cap: from X = -40 to X = -24 (width = 16m, center at X = -32)
            CreateInvisibleWall(ridgeGroup.transform, new Vector3(-32f, wallHeight / 2, -18f), new Vector3(16f, wallHeight, thickness), "Ridge_FarLeft_Barrier");
            InstantiatePrefab(PrefabRock5A, ridgeGroup.transform, new Vector3(-34f, 0f, -18f), Quaternion.Euler(0f, 40f, 0f), new Vector3(0.9f, 0.9f, 0.9f), false);
            InstantiatePrefab(PrefabRock2, ridgeGroup.transform, new Vector3(-27f, 0f, -18f), Quaternion.Euler(0f, -25f, 0f), new Vector3(1.1f, 1.1f, 1.1f), false);

            // Left Entry/Exit Arch Lintel (overhead arch clearly visible at Y = 4.8f across the gateway)
            GameObject arch = GameObject.CreatePrimitive(PrimitiveType.Cube);
            arch.name = "Kelp_Maze_Left_Entry_Exit_Arch";
            arch.transform.SetParent(ridgeGroup.transform);
            arch.transform.position = new Vector3(-20f, 4.8f, -18f);
            arch.transform.localScale = new Vector3(8.5f, 2.0f, 3.5f);
            arch.GetComponent<MeshRenderer>().sharedMaterial = matRock;

            // Gateway flanking stone pillars framing the opening
            InstantiatePrefab(PrefabRock5A, ridgeGroup.transform, new Vector3(-24.5f, 0f, -18f), Quaternion.Euler(0f, 20f, 0f), new Vector3(0.85f, 0.85f, 0.85f), false);
            InstantiatePrefab(PrefabRock5A, ridgeGroup.transform, new Vector3(-15.5f, 0f, -18f), Quaternion.Euler(0f, -20f, 0f), new Vector3(0.85f, 0.85f, 0.85f), false);

            // Glowing sea flowers & water lilies marking the entrance archway
            InstantiatePrefab(PrefabWaterLilyFlower, ridgeGroup.transform, new Vector3(-23f, 0.1f, -18.5f), Quaternion.identity, new Vector3(1.4f, 1.4f, 1.4f), true);
            InstantiatePrefab(PrefabWaterLilyFlower, ridgeGroup.transform, new Vector3(-17f, 0.1f, -18.5f), Quaternion.identity, new Vector3(1.4f, 1.4f, 1.4f), true);
            InstantiatePrefab(PrefabSpinReefFlower1A, ridgeGroup.transform, new Vector3(-20f, 0f, -18.8f), Quaternion.identity, new Vector3(0.9f, 0.9f, 0.9f), true);

            // SOLID BARRIER ACROSS CENTER & ENTIRE RIGHT SIDE: from X = -16 to X = +40 (width = 56m, center at X = 12)
            CreateInvisibleWall(ridgeGroup.transform, new Vector3(12f, wallHeight / 2, -18f), new Vector3(56f, wallHeight, thickness), "Ridge_Solid_Barrier");

            // Line the solid ridge with natural cliff rocks
            for (float x = -14f; x <= 36f; x += 8.5f)
            {
                string p = ((int)Mathf.Abs(x) % 2 == 0) ? PrefabRock3 : PrefabRock6A;
                InstantiatePrefab(p, ridgeGroup.transform, new Vector3(x, 0f, -18f + Random.Range(-0.4f, 0.4f)), Quaternion.Euler(0f, Random.Range(0f, 360f), 0f), Vector3.one * Random.Range(0.85f, 1.15f), false);
            }
        }

        private static void BuildHermitCrabHouse(Transform parent, Material matHermitCrab, Material matRock, Material matSand)
        {
            GameObject houseRoot = new GameObject("HermitCrab_Sanctuary_House");
            houseRoot.transform.SetParent(parent);
            // Positioned prominently on the right side of the south 1/4 area
            houseRoot.transform.position = new Vector3(18f, 0f, -25f);

            // Sandbed resting mat out in the open on clean sand
            GameObject bed = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            bed.name = "Hermit_Bed";
            bed.transform.SetParent(houseRoot.transform);
            bed.transform.localPosition = new Vector3(0f, 0.05f, 0f);
            bed.transform.localScale = new Vector3(4.5f, 0.05f, 4.5f);
            bed.GetComponent<MeshRenderer>().sharedMaterial = matSand;

            // Decorative flower and coral backdrop positioned safely BEHIND him (Z = -28.5) so nothing ever clips or hides him
            InstantiatePrefab(PrefabSpinReefFlower1A, houseRoot.transform, new Vector3(16f, 0f, -28.5f), Quaternion.Euler(0f, 35f, 0f), new Vector3(1.1f, 1.1f, 1.1f), true);
            InstantiatePrefab(PrefabWaterLilyFlower, houseRoot.transform, new Vector3(20.5f, 0.1f, -28.5f), Quaternion.identity, new Vector3(1.4f, 1.4f, 1.4f), true);
            InstantiatePrefab(PrefabSancheziaStar, houseRoot.transform, new Vector3(18f, 0.1f, -29.0f), Quaternion.identity, new Vector3(1.2f, 1.2f, 1.2f), true);
            InstantiatePrefab(PrefabGrassA1, houseRoot.transform, new Vector3(18.5f, 0f, -27.8f), Quaternion.identity, Vector3.one, true);

            // The Hermit Crab NPC: Using the 3D Rigged Pufferfish asset out in the open on the right reef
            GameObject crabObj = InstantiatePrefab(PrefabPufferfish, houseRoot.transform, new Vector3(18f, 0.75f, -25f), Quaternion.Euler(0f, -60f, 0f), new Vector3(1.75f, 1.75f, 1.75f), false);
            crabObj.name = "HermitCrab_Ailing_NPC";
            crabObj.transform.localPosition = new Vector3(0f, 0.75f, 0f);
            crabObj.transform.localRotation = Quaternion.Euler(0f, -60f, 0f);

            // Gentle bobbing motion in the water
            var bobbing = crabObj.AddComponent<BobbingMotion>();

            // Restored Golden Shell: Placed on top of the Pufferfish, initially inactive until returned!
            GameObject restoredShell = InstantiatePrefab(PrefabSeaShell, crabObj.transform, crabObj.transform.position + new Vector3(0f, 0.45f, -0.05f), Quaternion.Euler(-10f, 40f, 0f), new Vector3(1.3f, 1.3f, 1.3f), true);
            restoredShell.name = "Restored_Golden_Shell";
            restoredShell.transform.localPosition = new Vector3(0f, 0.35f, -0.05f);
            restoredShell.transform.localRotation = Quaternion.Euler(-10f, 40f, 0f);
            restoredShell.SetActive(false);

            var ctrl = Object.FindObjectOfType<UponHeavyTides.Level1.Level1Controller>();
            if (ctrl != null)
            {
                var field = typeof(UponHeavyTides.Level1.Level1Controller).GetField("restoredShellOnHermit", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null) field.SetValue(ctrl, restoredShell);
            }

            // Dialogue Trigger for Hermit Crab (Pufferfish)
            var diag = crabObj.AddComponent<DialogueTrigger>();
            diag.triggerOnEnter = true;
            diag.triggerOnlyOnce = true;
            diag.interactRadius = 4.5f;
            diag.AddLine("Hermit Crab", "Oh, woe is me! Please don't look at me... I'm completely naked and defenseless without my shell!", new Color(1f, 0.55f, 0.3f));
            diag.AddLine("Crab Companion", "Where is your golden spiral shell, friend? It's dangerous out in the open ocean without your armor!", new Color(1f, 0.75f, 0.3f));
            diag.AddLine("Hermit Crab", "The predators! They ambushed me and chased me out of the kelp forest. My golden shell was knocked loose and dragged deep into that twisted maze to the north!", new Color(1f, 0.55f, 0.3f));
            diag.AddLine("Hermit Crab", "The ONLY entrance into the maze is through the archway on the left wall! The rest of the ridge is completely blocked by stone!", new Color(1f, 0.55f, 0.3f));
            diag.AddLine("Hermit Crab", "Watch out for the predatory patrol groups! If they spot you, they'll chase you all the way back out! Duck into the 3 glowing kelp hiding zones along the paths to stay out of their sight!", new Color(1f, 0.55f, 0.3f));
            diag.AddLine("Crab Companion", "Don't you worry, friend! We will sneak in through the left archway, evade the predators, and bring your shell back safely!", new Color(1f, 0.75f, 0.3f));

            var col = crabObj.AddComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = 3.5f;
        }

        private static void BuildSouthFloraAndDecorations(Transform parent)
        {
            GameObject floraRoot = new GameObject("South_Sanctuary_Flora");
            floraRoot.transform.SetParent(parent);

            // Blooming sea flowers across the 1/4 area
            string[] flowerPrefabs = new string[] { PrefabWaterLilyFlower, PrefabSpinReefFlower1A, PrefabSancheziaStar, PrefabRafflesia };
            for (int f = 0; f < 30; f++)
            {
                float x = ((f * 13) % 68) - 34f;
                float z = -20f - ((f * 7) % 15);
                // Keep center entrance and right Pufferfish sanctuary clear of clutter
                if (Mathf.Abs(x) < 4.5f && Mathf.Abs(z + 22f) < 4.5f) continue;
                if (Vector2.Distance(new Vector2(x, z), new Vector2(18f, -25f)) < 5.5f) continue;

                string fp = flowerPrefabs[f % flowerPrefabs.Length];
                float s = (fp == PrefabWaterLilyFlower) ? Random.Range(1.2f, 1.7f) : Random.Range(0.75f, 1.2f);
                InstantiatePrefab(fp, floraRoot.transform, new Vector3(x, 0f, z), Quaternion.Euler(0f, Random.Range(0f, 360f), 0f), Vector3.one * s, true);

                if (f % 2 == 0)
                {
                    InstantiatePrefab(PrefabWaterLilyLeaf, floraRoot.transform, new Vector3(x + 0.35f, 0.05f, z + 0.25f), Quaternion.Euler(0f, Random.Range(0f, 360f), 0f), Vector3.one * Random.Range(1.1f, 1.5f), true);
                }
            }

            // Photogrammetry corals & bushes
            string[] coralPrefabs = new string[] { PrefabSpinReef1A, PrefabAntlerSprout1A, PrefabDayAreshaBush1, PrefabBloodChuteCluster };
            for (int c = 0; c < 32; c++)
            {
                float x = ((c * 17) % 66) - 33f;
                float z = -20f - ((c * 11) % 15);
                if (Mathf.Abs(x) < 5f && Mathf.Abs(z + 22f) < 5f) continue;
                if (Vector2.Distance(new Vector2(x, z), new Vector2(18f, -25f)) < 5.5f) continue;

                string cp = coralPrefabs[c % coralPrefabs.Length];
                InstantiatePrefab(cp, floraRoot.transform, new Vector3(x, 0f, z), Quaternion.Euler(0f, Random.Range(0f, 360f), 0f), Vector3.one * Random.Range(0.8f, 1.3f), true);
            }

            // Sea grasses
            string[] grassPrefabs = new string[] { PrefabGrassA1, PrefabGrassA2, PrefabGrassB1, PrefabGrassB2 };
            for (int g = 0; g < 40; g++)
            {
                float x = ((g * 19) % 68) - 34f;
                float z = -20f - ((g * 13) % 15);
                if (Vector2.Distance(new Vector2(x, z), new Vector2(18f, -25f)) < 4.5f) continue;
                string gp = grassPrefabs[g % grassPrefabs.Length];
                InstantiatePrefab(gp, floraRoot.transform, new Vector3(x, 0f, z), Quaternion.Euler(0f, Random.Range(0f, 360f), 0f), Vector3.one * Random.Range(0.85f, 1.35f), true);
            }

            // Natural rock clusters
            string[] rockClusters = new string[] { PrefabRock1Grp1, PrefabRock1Grp3, PrefabRock1C, PrefabRock1E };
            for (int r = 0; r < 14; r++)
            {
                float x = ((r * 23) % 62) - 31f;
                float z = -20f - ((r * 17) % 15);
                if (Mathf.Abs(x) < 6f && Mathf.Abs(z + 22f) < 6f) continue;
                if (Vector2.Distance(new Vector2(x, z), new Vector2(18f, -25f)) < 6.0f) continue;
                string rp = rockClusters[r % rockClusters.Length];
                InstantiatePrefab(rp, floraRoot.transform, new Vector3(x, 0f, z), Quaternion.Euler(0f, Random.Range(0f, 360f), 0f), Vector3.one * Random.Range(0.75f, 1.2f), false);
            }
        }

        private static void BuildPacmanKelpMaze(Transform parent, Material matKelp, Material matKelpDense, Material matHidingGrove)
        {
            GameObject mazeRoot = new GameObject("Pacman_Kelp_Maze_3_4_Area");
            mazeRoot.transform.SetParent(parent);

            // Pacman-Style Spacious Corridor Kelp Wall Segments (Wide 8-10m pathways)
            // Format: (CenterX, CenterZ, LengthX, LengthZ)
            var wallSegments = new List<Vector4>
            {
                // Outer corridor borders (Left & Right) - at X = -30 and X = +30
                new Vector4(-30f, 9f, 2.0f, 50f),
                new Vector4(30f, 9f, 2.0f, 50f),

                // Lower horizontal maze dividers (Z = -8, creating 10m wide south corridor)
                new Vector4(-18f, -8f, 16f, 2.0f),
                new Vector4(18f, -8f, 16f, 2.0f),

                // Lower vertical maze pillars
                new Vector4(-10f, -2f, 2.0f, 10f),
                new Vector4(10f, -2f, 2.0f, 10f),

                // Center T-blocks (forming Pacman corridors)
                new Vector4(0f, -1f, 14f, 2.0f),

                // Center Chamber Alcove Walls (encasing the Golden Shell at Z = 8)
                new Vector4(-6.5f, 8f, 2.0f, 10f),
                new Vector4(6.5f, 8f, 2.0f, 10f),
                new Vector4(0f, 13f, 15f, 2.0f), // North wall of center chamber

                // Mid-maze vertical dividers (X = -18 and X = 18)
                new Vector4(-18f, 8f, 2.0f, 14f),
                new Vector4(18f, 8f, 2.0f, 14f),

                // Upper cross dividers (Z = 20)
                new Vector4(-18f, 20f, 16f, 2.0f),
                new Vector4(18f, 20f, 16f, 2.0f),
                new Vector4(-8f, 25f, 2.0f, 8f),
                new Vector4(8f, 25f, 2.0f, 8f),

                // Far North Altar Gateway Wall (Z = 29, open in center for altar at Z = 32)
                new Vector4(-18f, 29f, 16f, 2.0f),
                new Vector4(18f, 29f, 16f, 2.0f)
            };

            int wallIdx = 0;
            foreach (var w in wallSegments)
            {
                BuildDenseKelpWall(mazeRoot.transform, new Vector3(w.x, 0f, w.y), w.z, w.w, matKelp, matKelpDense, $"KelpWall_{wallIdx++}");
            }

            // Exactly 3 Strategic Stealth Hiding Groves inside the maze corridors
            BuildHidingGrove(mazeRoot.transform, new Vector3(-24f, 0f, -8f), "HidingGrove_1_EntranceHaven", matHidingGrove, matKelpDense);
            BuildHidingGrove(mazeRoot.transform, new Vector3(-12f, 0f, 8f), "HidingGrove_2_CenterAlcove", matHidingGrove, matKelpDense);
            BuildHidingGrove(mazeRoot.transform, new Vector3(24f, 0f, 20f), "HidingGrove_3_NorthEastLoop", matHidingGrove, matKelpDense);
        }

        private static void BuildDenseKelpWall(Transform parent, Vector3 center, float lengthX, float lengthZ, Material matKelp, Material matKelpDense, string name)
        {
            GameObject wallObj = new GameObject(name);
            wallObj.transform.SetParent(parent);
            wallObj.transform.position = center;

            // Physical block collider inside kelp - solid barrier for BOTH player and enemies!
            GameObject colBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
            colBlock.name = "Kelp_Collider";
            colBlock.transform.SetParent(wallObj.transform);
            colBlock.transform.localPosition = new Vector3(0f, 3.5f, 0f);
            colBlock.transform.localScale = new Vector3(lengthX, 7.0f, lengthZ);
            colBlock.GetComponent<MeshRenderer>().enabled = false; // Invisible collider

            // NavMeshObstacle ensures Barracudas cannot pathfind through kelp
            var navObs = colBlock.AddComponent<UnityEngine.AI.NavMeshObstacle>();
            navObs.carving = true;
            navObs.size = Vector3.one;

            // Stalk generation along the wall volume (Restored original stylized dense emerald kelp forest)
            int stalkCountX = Mathf.Max(2, Mathf.RoundToInt(lengthX / 1.1f));
            int stalkCountZ = Mathf.Max(2, Mathf.RoundToInt(lengthZ / 1.1f));

            for (int ix = 0; ix < stalkCountX; ix++)
            {
                for (int iz = 0; iz < stalkCountZ; iz++)
                {
                    float xPos = -lengthX / 2f + (ix + 0.5f) * (lengthX / stalkCountX);
                    float zPos = -lengthZ / 2f + (iz + 0.5f) * (lengthZ / stalkCountZ);

                    GameObject stalk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    stalk.name = $"KelpStalk_{ix}_{iz}";
                    stalk.transform.SetParent(wallObj.transform);

                    // Destroy default capsule collider on individual stalks so wall collider governs collision smoothly
                    var stalkCol = stalk.GetComponent<Collider>();
                    if (stalkCol != null) Object.DestroyImmediate(stalkCol);

                    float height = Random.Range(7.5f, 10.5f);
                    float width = Random.Range(0.4f, 0.65f);
                    stalk.transform.localPosition = new Vector3(xPos, height / 2f, zPos);
                    stalk.transform.localScale = new Vector3(width, height / 2f, width);
                    stalk.transform.localRotation = Quaternion.Euler(Random.Range(-5f, 5f), Random.Range(0f, 360f), Random.Range(-5f, 5f));
                    stalk.GetComponent<MeshRenderer>().sharedMaterial = ((ix + iz) % 2 == 0) ? matKelp : matKelpDense;
                }
            }
        }

        private static void BuildHidingGrove(Transform parent, Vector3 center, string name, Material matGrove, Material matKelp)
        {
            GameObject grove = new GameObject(name);
            grove.transform.SetParent(parent);
            grove.transform.position = center;

            // Bioluminescent glowing floor safe zone disc
            GameObject zone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            zone.name = "StealthSafeZone_Trigger";
            zone.transform.SetParent(grove.transform);
            zone.transform.localPosition = new Vector3(0f, 0.08f, 0f);
            zone.transform.localScale = new Vector3(4.5f, 0.08f, 4.5f);
            zone.GetComponent<MeshRenderer>().sharedMaterial = matGrove;
            zone.GetComponent<Collider>().isTrigger = true;

            // Stealth trigger logic
            zone.AddComponent<StealthZoneTrigger>();

            // Rich array of unique photogrammetry plants & exaggerated foliage filling the hiding zone
            string[] foliagePrefabs = new string[]
            {
                PrefabPadderPod1A,
                PrefabAntlerSprout1A,
                PrefabSpinReef1A,
                PrefabSpinReefFlower1A,
                PrefabBasinBushA,
                PrefabPeacockPlant1A,
                PrefabChewChute1A,
                PrefabRazorRoot1A,
                PrefabFilterFruit1A,
                PrefabFishHookKelpRadial,
                PrefabWaterLilyFlower,
                PrefabDayAreshaBush1,
                PrefabSancheziaStar
            };

            // Outer thicket curtain (16 lush photogrammetry plants surrounding the perimeter, radius 2.1m - 2.5m)
            int outerCount = 16;
            for (int i = 0; i < outerCount; i++)
            {
                float ang = (i / (float)outerCount) * Mathf.PI * 2f;
                float r = Random.Range(2.1f, 2.5f);
                Vector3 p = center + new Vector3(Mathf.Cos(ang) * r, 0f, Mathf.Sin(ang) * r);
                Quaternion rot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

                string prefab = foliagePrefabs[i % foliagePrefabs.Length];
                float s = Random.Range(1.3f, 2.0f);
                // Colliders stripped on decorative plants so player and companion never get stuck!
                InstantiatePrefab(prefab, grove.transform, p, rot, Vector3.one * s, true);
            }

            // Inner thicket clusters (10 plants along inner fringes, radius 1.2m - 1.8m)
            int innerCount = 10;
            for (int j = 0; j < innerCount; j++)
            {
                float ang = (j / (float)innerCount) * Mathf.PI * 2f + 0.35f;
                float r = Random.Range(1.2f, 1.8f);
                Vector3 p = center + new Vector3(Mathf.Cos(ang) * r, 0f, Mathf.Sin(ang) * r);
                Quaternion rot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

                string prefab = foliagePrefabs[(j * 3 + 2) % foliagePrefabs.Length];
                float s = Random.Range(1.0f, 1.6f);
                InstantiatePrefab(prefab, grove.transform, p, rot, Vector3.one * s, true);
            }

            // Water lily leaves scattered along the grove bed
            for (int k = 0; k < 6; k++)
            {
                float ang = (k / 6f) * Mathf.PI * 2f + 0.15f;
                Vector3 lp = center + new Vector3(Mathf.Cos(ang) * 1.5f, 0.05f, Mathf.Sin(ang) * 1.5f);
                InstantiatePrefab(PrefabWaterLilyLeaf, grove.transform, lp, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f), Vector3.one * Random.Range(1.2f, 1.6f), true);
            }
        }

        private static void BuildPredatoryPatrolGroups(Transform parent, Material matFish, Material matVision)
        {
            GameObject enemyGroup = new GameObject("Barracuda_Patrol_Groups");
            enemyGroup.transform.SetParent(parent);

            // Group 1: South Avenue Loop (Guards entrance & south avenue)
            var wpGroup1 = CreateWaypointLoop(enemyGroup.transform, "Waypoints_Group_1_South", new Vector3[]
            {
                new Vector3(-22f, 1.5f, -13f),
                new Vector3(0f, 1.5f, -13f),
                new Vector3(22f, 1.5f, -13f),
                new Vector3(22f, 1.5f, -4f),
                new Vector3(0f, 1.5f, -4f),
                new Vector3(-22f, 1.5f, -4f)
            });
            BuildSharkPredatorUnit(enemyGroup.transform, new Vector3(-22f, 1.5f, -13f), "Barracuda_South_Alpha", matFish, matVision, wpGroup1);
            BuildSharkPredatorUnit(enemyGroup.transform, new Vector3(22f, 1.5f, -4f), "Barracuda_South_Beta", matFish, matVision, wpGroup1);

            // Group 2: West Flank Loop (Guards western corridors)
            var wpGroup2 = CreateWaypointLoop(enemyGroup.transform, "Waypoints_Group_2_West", new Vector3[]
            {
                new Vector3(-24f, 1.5f, -3f),
                new Vector3(-24f, 1.5f, 14f),
                new Vector3(-12f, 1.5f, 14f),
                new Vector3(-12f, 1.5f, 2f)
            });
            BuildSharkPredatorUnit(enemyGroup.transform, new Vector3(-24f, 1.5f, -3f), "Barracuda_West_Flank", matFish, matVision, wpGroup2);

            // Group 3: East Flank Loop (Guards eastern corridors)
            var wpGroup3 = CreateWaypointLoop(enemyGroup.transform, "Waypoints_Group_3_East", new Vector3[]
            {
                new Vector3(24f, 1.5f, -3f),
                new Vector3(24f, 1.5f, 14f),
                new Vector3(12f, 1.5f, 14f),
                new Vector3(12f, 1.5f, 2f)
            });
            BuildSharkPredatorUnit(enemyGroup.transform, new Vector3(24f, 1.5f, -3f), "Barracuda_East_Flank", matFish, matVision, wpGroup3);

            // Group 4: North Altar Loop (Guards Ancient Monster Clue Altar)
            var wpGroup4 = CreateWaypointLoop(enemyGroup.transform, "Waypoints_Group_4_North", new Vector3[]
            {
                new Vector3(-24f, 1.5f, 24f),
                new Vector3(24f, 1.5f, 24f),
                new Vector3(24f, 1.5f, 33f),
                new Vector3(-24f, 1.5f, 33f)
            });
            BuildSharkPredatorUnit(enemyGroup.transform, new Vector3(-24f, 1.5f, 24f), "Barracuda_North_Alpha", matFish, matVision, wpGroup4);
            BuildSharkPredatorUnit(enemyGroup.transform, new Vector3(24f, 1.5f, 33f), "Barracuda_North_Beta", matFish, matVision, wpGroup4);
        }

        private static List<Transform> CreateWaypointLoop(Transform parent, string groupName, Vector3[] points)
        {
            GameObject group = new GameObject(groupName);
            group.transform.SetParent(parent);
            var list = new List<Transform>();

            for (int i = 0; i < points.Length; i++)
            {
                GameObject wp = new GameObject($"WP_{i}");
                wp.transform.SetParent(group.transform);
                wp.transform.position = points[i];
                list.Add(wp.transform);
            }
            return list;
        }

        private static void BuildSharkPredatorUnit(Transform parent, Vector3 pos, string name, Material matFish, Material matVision, List<Transform> waypoints)
        {
            GameObject barracuda = new GameObject(name);
            barracuda.transform.SetParent(parent);
            barracuda.transform.position = pos;

            // Visual Predatory Shark Model (SharkV1) - Rotated 90 deg Y so head faces +Z forward!
            GameObject sharkVisual = InstantiatePrefab(PrefabSharkV1, barracuda.transform, pos, Quaternion.Euler(0f, 90f, 0f), new Vector3(0.6f, 0.6f, 0.6f), true);
            sharkVisual.name = "Predator_Shark_Visual";
            sharkVisual.transform.localPosition = Vector3.zero;
            sharkVisual.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);

            // Vision Cone (Detection Arc)
            GameObject visionCone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            visionCone.name = "VisionCone_DetectionZone";
            visionCone.transform.SetParent(barracuda.transform);
            visionCone.transform.localPosition = new Vector3(0f, 0f, 4.5f);
            visionCone.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            visionCone.transform.localScale = new Vector3(4.5f, 3.5f, 4.5f);
            var vRend = visionCone.GetComponent<MeshRenderer>();
            vRend.sharedMaterial = matVision;
            visionCone.GetComponent<Collider>().isTrigger = true;

            // NavMeshAgent & AI State Machine
            var agent = barracuda.AddComponent<NavMeshAgent>();
            agent.speed = 3.2f;
            agent.stoppingDistance = 1.0f;
            agent.baseOffset = 1.5f;
            agent.radius = 0.5f;
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            agent.avoidancePriority = 50;
            agent.autoRepath = true;

            var ai = barracuda.AddComponent<BarracudaAI>();
            ai.SetWaypoints(waypoints);

            var field = typeof(BarracudaAI).GetField("visionConeRenderer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null) field.SetValue(ai, vRend);
        }

        private static void BuildPushableProps(Transform parent, Material matRock, Material matSand)
        {
            GameObject propsGroup = new GameObject("Pushable_Physics_Props");
            propsGroup.transform.SetParent(parent);

            CreatePushableRock(propsGroup.transform, PrefabRock1C, new Vector3(-12f, 0.6f, -6f), new Vector3(1.3f, 1.3f, 1.3f), 3.5f, "Pushable_Rock_SouthWest");
            CreatePushableRock(propsGroup.transform, PrefabRock4B, new Vector3(12f, 0.6f, 6f), new Vector3(0.9f, 0.9f, 0.9f), 3.5f, "Pushable_Rock_EastLoop");
            CreatePushableRock(propsGroup.transform, PrefabRock4A, new Vector3(0f, 0.7f, 22f), new Vector3(1.1f, 1.1f, 1.1f), 4.5f, "Pushable_Rock_NorthChamber");
        }

        private static void CreatePushableRock(Transform parent, string prefabPath, Vector3 pos, Vector3 scale, float mass, string name)
        {
            GameObject rock = InstantiatePrefab(prefabPath, parent, pos, Quaternion.identity, scale, false);
            rock.name = name;

            // Ensure all MeshColliders are convex so dynamic Rigidbodies never throw concave PhysX errors
            foreach (var mc in rock.GetComponentsInChildren<MeshCollider>())
            {
                mc.convex = true;
            }

            Rigidbody rb = rock.GetComponent<Rigidbody>();
            if (rb == null) rb = rock.AddComponent<Rigidbody>();
            rb.mass = mass;
            rb.linearDamping = 1.0f;
            rb.angularDamping = 1.5f;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            if (rock.GetComponent<Pushable>() == null) rock.AddComponent<Pushable>();
        }

        private static void BuildObjectivesAndClues(Transform parent, Material matShell, Material matClue, Material matRock)
        {
            GameObject objGroup = new GameObject("Objectives_And_Clues");
            objGroup.transform.SetParent(parent);

            // 1. The Lost Golden Hermit Shell (Center Chamber at Z = 8)
            GameObject shellChamber = new GameObject("Objective_LostGoldenShell");
            shellChamber.transform.SetParent(objGroup.transform);
            shellChamber.transform.position = new Vector3(0f, 0f, 8f);

            // Natural rock & flower pedestal bed
            InstantiatePrefab(PrefabRock1Grp2, shellChamber.transform, new Vector3(0f, 0f, 8f), Quaternion.identity, new Vector3(1.4f, 0.8f, 1.4f), false);
            InstantiatePrefab(PrefabSpinReefFlower1A, shellChamber.transform, new Vector3(1.2f, 0f, 8.8f), Quaternion.Euler(0f, 45f, 0f), new Vector3(0.9f, 0.9f, 0.9f), true);
            InstantiatePrefab(PrefabWaterLilyFlower, shellChamber.transform, new Vector3(-1.2f, 0.1f, 7.4f), Quaternion.identity, new Vector3(1.3f, 1.3f, 1.3f), true);

            GameObject pedestal = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pedestal.name = "Shell_Pedestal";
            pedestal.transform.SetParent(shellChamber.transform);
            pedestal.transform.localPosition = new Vector3(0f, 0.4f, 0f);
            pedestal.transform.localScale = new Vector3(2.5f, 0.4f, 2.5f);
            pedestal.GetComponent<MeshRenderer>().sharedMaterial = matRock;

            // Real 3D Big Seashell model for the Hermit Crab's Lost Golden Shell!
            GameObject shell = InstantiatePrefab(PrefabSeaShell, shellChamber.transform, new Vector3(0f, 1.0f, 8f), Quaternion.Euler(-15f, 45f, 0f), new Vector3(1.8f, 1.8f, 1.8f), false);
            shell.name = "Hermit_Golden_Shell";

            var col = shell.GetComponent<Collider>();
            if (col == null) col = shell.AddComponent<SphereCollider>();
            col.isTrigger = true;

            var colItem = shell.AddComponent<CollectibleItem>();
            colItem.itemName = "Lost Golden Hermit Shell";
            colItem.itemIdentifier = "HermitShell";
            var shellTrig = shell.AddComponent<UponHeavyTides.Level1.Level1ObjectiveTrigger>();
            shellTrig.SetTriggerType(UponHeavyTides.Level1.Level1ObjectiveTrigger.TriggerType.GoldenShell);

            var ctrl = Object.FindObjectOfType<UponHeavyTides.Level1.Level1Controller>();
            if (ctrl != null)
            {
                var field = typeof(UponHeavyTides.Level1.Level1Controller).GetField("goldenShellInMaze", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null) field.SetValue(ctrl, shell);
            }

            // 2. The Ancient Monster Clue Tablet (Far North Chamber at Z = 32)
            GameObject clueChamber = new GameObject("Objective_MonsterClue_Tablet");
            clueChamber.transform.SetParent(objGroup.transform);
            clueChamber.transform.position = new Vector3(0f, 0f, 32f);

            // Altar decorated with ancient pillars & blooming flowers
            InstantiatePrefab(PrefabRock5A, clueChamber.transform, new Vector3(-4.5f, 0f, 32f), Quaternion.Euler(0f, 30f, 0f), new Vector3(0.85f, 0.85f, 0.85f), false);
            InstantiatePrefab(PrefabRock5A, clueChamber.transform, new Vector3(4.5f, 0f, 32f), Quaternion.Euler(0f, -30f, 0f), new Vector3(0.85f, 0.85f, 0.85f), false);
            InstantiatePrefab(PrefabWaterLilyFlower, clueChamber.transform, new Vector3(-2.0f, 0.1f, 30.5f), Quaternion.identity, new Vector3(1.4f, 1.4f, 1.4f), true);
            InstantiatePrefab(PrefabWaterLilyFlower, clueChamber.transform, new Vector3(2.0f, 0.1f, 30.5f), Quaternion.identity, new Vector3(1.4f, 1.4f, 1.4f), true);

            GameObject altar = GameObject.CreatePrimitive(PrimitiveType.Cube);
            altar.name = "Clue_Altar";
            altar.transform.SetParent(clueChamber.transform);
            altar.transform.localPosition = new Vector3(0f, 1.0f, 0f);
            altar.transform.localScale = new Vector3(4.0f, 2.0f, 2.0f);
            altar.GetComponent<MeshRenderer>().sharedMaterial = matRock;

            GameObject tablet = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tablet.name = "Glowing_Clue_Tablet";
            tablet.transform.SetParent(clueChamber.transform);
            tablet.transform.localPosition = new Vector3(0f, 2.4f, 0f);
            tablet.transform.localRotation = Quaternion.Euler(15f, 0f, 0f);
            tablet.transform.localScale = new Vector3(2.2f, 1.4f, 0.25f);
            tablet.GetComponent<MeshRenderer>().sharedMaterial = matClue;

            var triggerCol = clueChamber.AddComponent<BoxCollider>();
            triggerCol.isTrigger = true;
            triggerCol.center = new Vector3(0f, 1.5f, 0f);
            triggerCol.size = new Vector3(8f, 4f, 8f);
            var clueTrig = clueChamber.AddComponent<UponHeavyTides.Level1.Level1ObjectiveTrigger>();
            clueTrig.SetTriggerType(UponHeavyTides.Level1.Level1ObjectiveTrigger.TriggerType.ClueTablet);

            var altarTrig = altar.AddComponent<UponHeavyTides.Level1.Level1ObjectiveTrigger>();
            altarTrig.SetTriggerType(UponHeavyTides.Level1.Level1ObjectiveTrigger.TriggerType.ClueTablet);

            var tabletTrig = tablet.AddComponent<UponHeavyTides.Level1.Level1ObjectiveTrigger>();
            tabletTrig.SetTriggerType(UponHeavyTides.Level1.Level1ObjectiveTrigger.TriggerType.ClueTablet);
        }

        private static void CreateInvisibleWall(Transform parent, Vector3 pos, Vector3 scale, string name)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = name;
            wall.transform.SetParent(parent);
            wall.transform.position = pos;
            wall.transform.localScale = scale;
            wall.GetComponent<MeshRenderer>().enabled = false;
        }

        private static GameObject InstantiatePrefab(string assetPath, Transform parent, Vector3 position, Quaternion rotation, Vector3 scale, bool removeColliders = false)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (prefab == null)
            {
                Debug.LogWarning($"[Level1BlockoutBuilder] Could not load prefab at: {assetPath}");
                return new GameObject($"Missing_{System.IO.Path.GetFileNameWithoutExtension(assetPath)}");
            }

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            instance.transform.SetParent(parent);
            instance.transform.position = position;
            instance.transform.rotation = rotation;
            instance.transform.localScale = scale;

            if (removeColliders)
            {
                var colliders = instance.GetComponentsInChildren<Collider>();
                foreach (var c in colliders)
                {
                    Object.DestroyImmediate(c);
                }
            }

            return instance;
        }

        private static GameObject GetOrCreateRoot(string name)
        {
            GameObject obj = GameObject.Find(name);
            if (obj == null)
            {
                obj = new GameObject(name);
                Undo.RegisterCreatedObjectUndo(obj, $"Create {name}");
            }
            return obj;
        }

        private static void ClearChildren(GameObject obj)
        {
            var children = new List<GameObject>();
            foreach (Transform child in obj.transform)
            {
                children.Add(child.gameObject);
            }
            foreach (var child in children)
            {
                Undo.DestroyObjectImmediate(child);
            }
        }

        private static Material GetOrCreateMaterial(string name, Color color, float metallic = 0f, float smoothness = 0.5f, bool transparent = false)
        {
            string path = $"{MaterialsFolder}/{name}.mat";
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null)
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Lit");
                if (shader == null) shader = Shader.Find("Standard");
                mat = new Material(shader) { name = name };

                if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
                else if (mat.HasProperty("_Color")) mat.SetColor("_Color", color);

                if (mat.HasProperty("_Metallic")) mat.SetFloat("_Metallic", metallic);
                if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smoothness);

                if (transparent)
                {
                    mat.SetFloat("_Surface", 1);
                    mat.renderQueue = 3000;
                }

                AssetDatabase.CreateAsset(mat, path);
            }
            return mat;
        }
    }
}
