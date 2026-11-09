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
    public static class Level0Builder
    {
        private const string ScenePath = "Assets/Scenes/Level0.unity";
        private const string MaterialsFolder = "Assets/Materials";

        // Asset Prefab Paths
        private const string PrefabFishV1 = "Assets/Alstra Infinite/Fish - PolyPack/Prefabs/FishV1.prefab";
        private const string PrefabFishV2 = "Assets/Alstra Infinite/Fish - PolyPack/Prefabs/FishV2.prefab";
        private const string PrefabFishV3 = "Assets/Alstra Infinite/Fish - PolyPack/Prefabs/FishV3.prefab";
        private const string PrefabFishV4 = "Assets/Alstra Infinite/Fish - PolyPack/Prefabs/FishV4.prefab";
        private const string PrefabFish02 = "Assets/Floreswa/Prefabs/fish02.prefab";
        private const string PrefabFish03 = "Assets/Floreswa/Prefabs/fish03.prefab";

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

        private const string PrefabWaterLilyFlower = "Assets/Jonathan Foust/Prefabs/Water Lily Flower.prefab";
        private const string PrefabWaterLilyLeaf = "Assets/Jonathan Foust/Prefabs/Water Lily Leaf.prefab";
        private const string PrefabRafflesia = "Assets/Jonathan Foust/Prefabs/Rafflesia.prefab";
        private const string PrefabSancheziaStar = "Assets/Jonathan Foust/Prefabs/Sanchezia Star.prefab";
        private const string PrefabDayAreshaBush1 = "Assets/Jonathan Foust/Prefabs/Day Aresha Bush_1.prefab";
        private const string PrefabDayAreshaBush2 = "Assets/Jonathan Foust/Prefabs/Day Aresha Bush_2.prefab";
        private const string PrefabFishHookKelp1 = "Assets/Jonathan Foust/Prefabs/Fish-Hook Kelp_1.prefab";
        private const string PrefabFishHookKelpRadial = "Assets/Jonathan Foust/Prefabs/Fish-Hook Kelp Bush_Radial.prefab";
        private const string PrefabGrassA1 = "Assets/Jonathan Foust/Prefabs/Grass A_1.prefab";
        private const string PrefabGrassA2 = "Assets/Jonathan Foust/Prefabs/Grass A_2.prefab";
        private const string PrefabGrassB1 = "Assets/Jonathan Foust/Prefabs/Grass B_1.prefab";
        private const string PrefabGrassB2 = "Assets/Jonathan Foust/Prefabs/Grass B_2.prefab";
        private const string PrefabBloodChuteCluster = "Assets/Jonathan Foust/Prefabs/Blood Chute Cluster.prefab";

        private const string PrefabSpinReef1A = "Assets/Splash of Color - Unique Photogrammetry Plants/Prefabs/High Resolution Prefabs/Spin Reef/Spin Reef 1A.prefab";
        private const string PrefabSpinReefFlower1A = "Assets/Splash of Color - Unique Photogrammetry Plants/Prefabs/High Resolution Prefabs/Spin Reef/Spin Reef Flower 1A.prefab";
        private const string PrefabAntlerSprout1A = "Assets/Splash of Color - Unique Photogrammetry Plants/Prefabs/High Resolution Prefabs/Antler Sprout/Antler Sprout 1A.prefab";
        private const string PrefabSpiralChute1A = "Assets/Splash of Color - Unique Photogrammetry Plants/Prefabs/High Resolution Prefabs/Spiral Chute/Spiral Chute 1A.prefab";
        private const string PrefabPadderPod1A = "Assets/Splash of Color - Unique Photogrammetry Plants/Prefabs/High Resolution Prefabs/Padder Pod/Padder Pod 1A.prefab";

        // New Pixit undersea assets
        private const string PrefabCrab3D = "Assets/PixitGames/Pixit3D/PixitUnderseaCollection/LowPolyFishFree/Prefabs/Crab_Rigged_Unity.prefab";
        private const string PrefabPearlSeashell = "Assets/PixitGames/Pixit3D/PixitUnderseaCollection/LowPolyFishFree/Prefabs/PearlSeashell_Rigged_Unity.prefab";
        private const string PrefabOctopus = "Assets/PixitGames/Pixit3D/PixitUnderseaCollection/LowPolyFishFree/Prefabs/Octopus_Rigged_Unity.prefab";

        [MenuItem("Upon Heavy Tides/Build Level 0 (The Shallows)")]
        public static void BuildLevel0()
        {
            if (EditorSceneManager.GetActiveScene().path != ScenePath)
            {
                EditorSceneManager.OpenScene(ScenePath);
            }

            Undo.IncrementCurrentGroup();
            int undoGroup = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Build Level 0 The Shallows Decorated");

            // 1. Setup Materials (Real Sun & Sea Beach Golden Sand)
            Material matSand = AssetDatabase.LoadAssetAtPath<Material>("Assets/Shaded Spectrum/Sun And Sea Beach Materials Pack/Materials/Golden Shore Sand.mat");
            if (matSand == null) matSand = GetOrCreateMaterial("Mat_ShallowsSand", new Color(0.92f, 0.86f, 0.70f), 0.15f, 0.05f);

            Material matRock = AssetDatabase.LoadAssetAtPath<Material>("Assets/Rocks and Boulders 2/Rocks/Source/Materials/Unity 5/Rock1.mat");
            if (matRock == null) matRock = GetOrCreateMaterial("Mat_ShallowsRock", new Color(0.55f, 0.52f, 0.48f), 0.25f, 0.1f);

            Material matCoralTeal = GetOrCreateMaterial("Mat_CoralTeal", new Color(0.22f, 0.85f, 0.75f), 0.3f, 0.15f);
            Material matTurtleShell = GetOrCreateMaterial("Mat_ElderTurtleShell", new Color(0.22f, 0.42f, 0.30f), 0.4f, 0.2f);
            Material matTurtleSkin = GetOrCreateMaterial("Mat_ElderTurtleSkin", new Color(0.48f, 0.55f, 0.42f), 0.3f, 0.05f);
            Material matRelicPearl = GetOrCreateMaterial("Mat_RelicPearl", new Color(1.0f, 0.95f, 0.72f), 0.85f, 0.3f);
            Material matSuckerScar = GetOrCreateMaterial("Mat_SuckerScar", new Color(0.48f, 0.12f, 0.25f), 0.6f, 0.1f);
            Material matHookBarbs = GetOrCreateMaterial("Mat_HookBarbs", new Color(0.95f, 0.95f, 0.90f), 0.7f, 0.4f);

            // 2. Setup Shallows Sunlight & Pale Underwater Atmosphere
            SetupShallowsAtmosphere();

            // 3. Ground Arena & Navigation Surface
            GameObject ground = GameObject.Find("Ground");
            if (ground == null)
            {
                ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
                ground.name = "Ground";
                Undo.RegisterCreatedObjectUndo(ground, "Create Ground");
            }
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = new Vector3(8f, 1f, 8f); // 80m x 80m
            ground.GetComponent<MeshRenderer>().sharedMaterial = matSand;

            NavMeshSurface navSurface = ground.GetComponent<NavMeshSurface>();
            if (navSurface == null) navSurface = ground.AddComponent<NavMeshSurface>();
            navSurface.collectObjects = CollectObjects.All;
            navSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;

            // 4. Setup Hierarchy Roots
            GameObject envRoot = GetOrCreateRoot("_ENVIRONMENT");
            GameObject npcsRoot = GetOrCreateRoot("_NPCS");
            GameObject tutorialRoot = GetOrCreateRoot("_TUTORIAL_PROPS");
            GameObject relicsRoot = GetOrCreateRoot("_EXPLORATION_RELICS");
            GameObject culminationRoot = GetOrCreateRoot("_CULMINATION_DISCOVERY");
            GameObject marineLifeRoot = GetOrCreateRoot("_AMBIENT_MARINE_LIFE");
            GameObject managerRoot = GetOrCreateRoot("_GAME_MANAGERS");

            ClearChildren(envRoot);
            ClearChildren(npcsRoot);
            ClearChildren(tutorialRoot);
            ClearChildren(relicsRoot);
            ClearChildren(culminationRoot);
            ClearChildren(marineLifeRoot);
            ClearChildren(managerRoot);

            // 5. Game Managers (DialogueManager, Level0Controller, Level0StoryFlow)
            SetupManagers(managerRoot);

            // 6. Perimeter Cliffs & Natural Rock Assets Boundary
            BuildPerimeterReefs(envRoot.transform, matRock);

            // 7. Rich Lively Shallows Decor: Flowers, Corals, Seaweed, Plants, Rock Clusters
            BuildLivelyFlora(envRoot.transform);

            // 8. Player Setup
            GameObject player = GameObject.Find("Player");
            if (player != null)
            {
                player.transform.position = new Vector3(0f, 1f, -32f);
                player.transform.rotation = Quaternion.identity;
                player.tag = "Player";
                if (player.GetComponent<PlayerMovement>() == null) player.AddComponent<PlayerMovement>();
                if (player.GetComponent<PlayerPusher>() == null) player.AddComponent<PlayerPusher>();
            }

            // 9. Camera Tracker Setup
            GameObject mainCam = GameObject.Find("Main Camera");
            if (mainCam != null && player != null)
            {
                var tracker = mainCam.GetComponent<CameraTracker>();
                if (tracker == null) tracker = mainCam.AddComponent<CameraTracker>();
                tracker.target = player.transform;
                tracker.distance = 7.5f;
                tracker.height = 4.5f;
                mainCam.transform.position = player.transform.position + new Vector3(0f, 4.5f, -7.5f);
            }

            // 10. Crab Companion & Intro Interaction (Stays Waiting at Shelter Rock until found)
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
            crab.transform.position = new Vector3(-3.8f, 0.4f, -28.5f);
            crab.transform.localScale = Vector3.one;

            // Remove any primitive mesh/collider components on Crab root
            var sphereRenderer = crab.GetComponent<MeshRenderer>();
            if (sphereRenderer != null) Object.DestroyImmediate(sphereRenderer);
            var sphereFilter = crab.GetComponent<MeshFilter>();
            if (sphereFilter != null) Object.DestroyImmediate(sphereFilter);
            var sphereCol = crab.GetComponent<Collider>();
            if (sphereCol != null) Object.DestroyImmediate(sphereCol);

            // Clean ALL children to completely eliminate nested/duplicate crab models
            for (int i = crab.transform.childCount - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(crab.transform.GetChild(i).gameObject);
            }

            // Single Visual 3D Crab model child using rigged asset
            GameObject crabVisual = InstantiatePrefab(PrefabCrab3D, crab.transform, crab.transform.position, Quaternion.identity, new Vector3(1.2f, 1.2f, 1.2f), true);
            crabVisual.name = "Visual_Crab_3D";
            crabVisual.transform.localPosition = Vector3.zero;
            crabVisual.transform.localRotation = Quaternion.identity;

            // Rename the internal mesh child so GameObject.Find("Crab") will NEVER match the inner mesh!
            var innerMesh = crabVisual.transform.Find("Crab");
            if (innerMesh != null) innerMesh.name = "Crab_RiggedMesh";

            var companion = crab.GetComponent<Companion>();
            if (companion == null) companion = crab.AddComponent<Companion>();
            companion.SetWaiting(true);

            // Setup Crab Intro Dialogue
            var crabDiag = crab.GetComponent<DialogueTrigger>();
            if (crabDiag == null) crabDiag = crab.AddComponent<DialogueTrigger>();
            SetupCrabIntroDialogue(crabDiag);

            // Shelter rock for Crab using genuine Rock4B asset
            BuildCrabShelterRock(npcsRoot.transform);

            // 11. Elder Sea Turtle Sanctuary
            BuildElderSeaTurtle(npcsRoot.transform, matTurtleShell, matTurtleSkin, matRock, matCoralTeal);

            // 12. School of Fish NPC Group (Using one type: FishV1)
            BuildSchoolOfFish(npcsRoot.transform);

            // 13. Ambient Marine Life (Different fish types swimming peacefully around the shallows)
            BuildAmbientMarineLife(marineLifeRoot.transform);

            // Bobbing Octopus near the rocks
            BuildBobbingOctopus(marineLifeRoot.transform);

            // 14. Tutorial Obstacles: Jump Steps & Pushable Boulders with rock assets
            BuildTutorialCourse(tutorialRoot.transform, matRock);

            // 15. Exploration Relics (Framed with blooming sea flowers & rock pedestals)
            BuildExplorationRelics(relicsRoot.transform, matRelicPearl);

            // 16. Culmination: Final Boulder & Giant Sucker Scar
            BuildFinalTrialAndSuckerScar(culminationRoot.transform, matRock, matSuckerScar, matHookBarbs);

            // Clean up old standalone Rock from initial scene
            GameObject oldRock = GameObject.Find("Rock");
            if (oldRock != null && oldRock.transform.parent == null)
            {
                Undo.DestroyObjectImmediate(oldRock);
            }

            // 17. Bake Navigation Mesh
            navSurface.BuildNavMesh();

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveOpenScenes();

            Undo.CollapseUndoOperations(undoGroup);
            Debug.Log("<color=green><b>[Upon Heavy Tides]</b> Level 0 (The Shallows) fully decorated with imported rocks, flowers, plants, and fish!</color>");
        }

        private static void SetupShallowsAtmosphere()
        {
            GameObject dirLight = GameObject.Find("Directional Light");
            if (dirLight != null)
            {
                Light l = dirLight.GetComponent<Light>();
                if (l != null)
                {
                    l.color = new Color(0.85f, 0.96f, 0.94f); // Pale sunlit aquatic
                    l.intensity = 1.25f;
                }
                dirLight.transform.rotation = Quaternion.Euler(55f, -30f, 0f);
            }

            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogDensity = 0.015f;
            RenderSettings.fogColor = new Color(0.28f, 0.58f, 0.62f); // Pale turquoise
            RenderSettings.ambientSkyColor = new Color(0.38f, 0.58f, 0.58f);
        }

        private static void SetupManagers(GameObject root)
        {
            if (root.GetComponent<DialogueManager>() == null)
            {
                root.AddComponent<DialogueManager>();
            }
            var ctrl = root.GetComponent<Level0Controller>();
            if (ctrl == null)
            {
                ctrl = root.AddComponent<Level0Controller>();
            }
            ctrl.totalCollectibles = 6;
            if (root.GetComponent<Level0StoryFlow>() == null)
            {
                root.AddComponent<Level0StoryFlow>();
            }
        }

        private static void SetupCrabIntroDialogue(DialogueTrigger trigger)
        {
            trigger.ClearLines();
            trigger.triggerOnlyOnce = true;
            trigger.triggerOnEnter = false;
            trigger.interactRadius = 4.5f;
            trigger.AddLine("Crab Companion", "P-psst! Over here! Behind this rock!", new Color(1f, 0.65f, 0.3f));
            trigger.AddLine("Crab Companion", "By the Great Tides... you're not from the ocean! You're the creature from above!", new Color(1f, 0.65f, 0.3f));
            trigger.AddLine("Crab Companion", "A monster from the deep has stolen our world's light. Our kingdom is drowning in fear...", new Color(1f, 0.65f, 0.3f));
            trigger.AddLine("Crab Companion", "Follow me! Let's go to the Elder Sea Turtle at the ancient sanctuary!", new Color(1f, 0.65f, 0.3f));
        }

        private static void BuildCrabShelterRock(Transform parent)
        {
            // The sheltering rock behind which the crab waits
            GameObject rock = InstantiatePrefab(PrefabRock4B, parent, new Vector3(-2.2f, 0f, -30.5f), Quaternion.Euler(-10f, 35f, 5f), new Vector3(1.3f, 1.3f, 1.3f));
            rock.name = "Crab_Shelter_Rock";

            // Sea flowers decorating the crab's hiding spot
            InstantiatePrefab(PrefabSpinReefFlower1A, parent, new Vector3(-4.5f, 0f, -28.2f), Quaternion.Euler(0f, 40f, 0f), new Vector3(0.9f, 0.9f, 0.9f), true);
            InstantiatePrefab(PrefabWaterLilyFlower, parent, new Vector3(-1.2f, 0.1f, -29.2f), Quaternion.identity, new Vector3(1.5f, 1.5f, 1.5f), true);
            InstantiatePrefab(PrefabGrassA1, parent, new Vector3(-2.8f, 0f, -31.5f), Quaternion.Euler(0f, 110f, 0f), Vector3.one, true);
        }

        private static void BuildElderSeaTurtle(Transform parent, Material matShell, Material matSkin, Material matRock, Material matCoral)
        {
            GameObject sanctuary = new GameObject("Elder_Sea_Turtle_Sanctuary");
            sanctuary.transform.SetParent(parent);
            sanctuary.transform.position = new Vector3(0f, 0f, -10f);

            // Altar Sanctuary Floor
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            floor.name = "Sanctuary_Floor";
            floor.transform.SetParent(sanctuary.transform);
            floor.transform.localPosition = new Vector3(0f, 0.1f, 0f);
            floor.transform.localScale = new Vector3(9f, 0.1f, 9f);
            floor.GetComponent<MeshRenderer>().sharedMaterial = matRock;

            // Surrounding Ancient Coral Columns & Flower Garlands
            for (int i = 0; i < 6; i++)
            {
                float ang = (i / 6f) * Mathf.PI * 2f;
                Vector3 colPos = new Vector3(Mathf.Cos(ang) * 5.2f, 2.5f, Mathf.Sin(ang) * 5.2f);
                GameObject col = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                col.name = $"Pillar_{i}";
                col.transform.SetParent(sanctuary.transform);
                col.transform.localPosition = colPos;
                col.transform.localScale = new Vector3(0.7f, 2.5f, 0.7f);
                col.GetComponent<MeshRenderer>().sharedMaterial = matCoral;

                // Blooming flowers at the base of each pillar
                Vector3 flowerPos = new Vector3(Mathf.Cos(ang) * 4.6f, 0f, Mathf.Sin(ang) * 4.6f);
                InstantiatePrefab(PrefabSpinReefFlower1A, sanctuary.transform, sanctuary.transform.position + flowerPos, Quaternion.Euler(0f, i * 60f, 0f), new Vector3(0.85f, 0.85f, 0.85f), true);
                InstantiatePrefab(PrefabWaterLilyFlower, sanctuary.transform, sanctuary.transform.position + flowerPos + new Vector3(0.4f, 0f, 0.3f), Quaternion.identity, new Vector3(1.3f, 1.3f, 1.3f), true);
            }

            // Turtle Model
            GameObject turtleObj = new GameObject("Elder_Sea_Turtle");
            turtleObj.transform.SetParent(sanctuary.transform);
            turtleObj.transform.localPosition = new Vector3(0f, 0f, 0.5f);
            turtleObj.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

            // Turtle Shell (Dome)
            GameObject shell = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            shell.name = "Turtle_Carapace";
            shell.transform.SetParent(turtleObj.transform);
            shell.transform.localPosition = new Vector3(0f, 1.4f, 0f);
            shell.transform.localScale = new Vector3(4.2f, 1.8f, 5.0f);
            shell.GetComponent<MeshRenderer>().sharedMaterial = matShell;

            // Turtle Head
            GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.name = "Turtle_Head";
            head.transform.SetParent(turtleObj.transform);
            head.transform.localPosition = new Vector3(0f, 1.6f, 3.2f);
            head.transform.localScale = new Vector3(1.2f, 0.9f, 1.4f);
            head.GetComponent<MeshRenderer>().sharedMaterial = matSkin;

            // Flippers
            GameObject flLeft = GameObject.CreatePrimitive(PrimitiveType.Cube);
            flLeft.name = "Flipper_Front_Left";
            flLeft.transform.SetParent(turtleObj.transform);
            flLeft.transform.localPosition = new Vector3(-2.6f, 0.7f, 2.0f);
            flLeft.transform.localScale = new Vector3(1.8f, 0.25f, 1.1f);
            flLeft.transform.localRotation = Quaternion.Euler(0f, -30f, 15f);
            flLeft.GetComponent<MeshRenderer>().sharedMaterial = matSkin;

            GameObject flRight = GameObject.CreatePrimitive(PrimitiveType.Cube);
            flRight.name = "Flipper_Front_Right";
            flRight.transform.SetParent(turtleObj.transform);
            flRight.transform.localPosition = new Vector3(2.6f, 0.7f, 2.0f);
            flRight.transform.localScale = new Vector3(1.8f, 0.25f, 1.1f);
            flRight.transform.localRotation = Quaternion.Euler(0f, 30f, -15f);
            flRight.GetComponent<MeshRenderer>().sharedMaterial = matSkin;

            // Dialogue Trigger for Turtle
            var diag = turtleObj.AddComponent<DialogueTrigger>();
            diag.ClearLines();
            diag.triggerOnlyOnce = true;
            diag.triggerOnEnter = false;
            diag.interactRadius = 6.0f;
            diag.AddLine("Elder Sea Turtle", "Welcome, traveler of the surface. It was foretold that a creature from above would descend when our world's light began to fade.", new Color(0.35f, 0.85f, 0.75f));
            diag.AddLine("Elder Sea Turtle", "A colossal monster haunts the deepest trenches. It drains the life from our corals and the color from our tides.", new Color(0.35f, 0.85f, 0.75f));
            diag.AddLine("Elder Sea Turtle", "I appoint this young crab to be your guide on your journey. He shall teach you the way of the water.", new Color(0.35f, 0.85f, 0.75f));
            diag.AddLine("Crab Companion", "It is an honor, Elder! Listen closely, traveler: Press [SPACE] to jump over coral ridges. Press [A] to turn the camera left, and [D] to turn the camera right. Move in any direction using [W][S] or the [ARROW KEYS]: Up Arrow for forward, Down Arrow for backward, and Left/Right Arrows to steer.", new Color(1f, 0.65f, 0.3f));
            diag.AddLine("Crab Companion", "Now let's explore around the map! There are ancient relics and clues hidden across these reefs.", new Color(1f, 0.65f, 0.3f));
            diag.AddLine("Elder Sea Turtle", "Seek the gateway at the end of the shallows. Explore the reefs, meet the school of fish, and uncover the clue hidden beneath the great stone.", new Color(0.35f, 0.85f, 0.75f));
        }

        private static void BuildSchoolOfFish(Transform parent)
        {
            GameObject schoolGroup = new GameObject("School_Of_Fish_NPC");
            schoolGroup.transform.SetParent(parent);
            schoolGroup.transform.position = new Vector3(12f, 1.8f, 6f);
            schoolGroup.AddComponent<FishSchoolAnimator>();

            // Cohesive swimming formation of 10 FishV1 instances
            Vector3[] offsets = new Vector3[]
            {
                new Vector3(0f, 0f, 0f),
                new Vector3(-1.0f, 0.25f, -0.7f),
                new Vector3(1.0f, -0.2f, -0.7f),
                new Vector3(-1.8f, -0.3f, -1.5f),
                new Vector3(1.8f, 0.35f, -1.5f),
                new Vector3(-0.6f, 0.5f, -1.2f),
                new Vector3(0.6f, -0.4f, -1.2f),
                new Vector3(-2.4f, 0.1f, -2.2f),
                new Vector3(2.4f, -0.1f, -2.2f),
                new Vector3(0f, 0.4f, -2.0f)
            };

            for (int i = 0; i < offsets.Length; i++)
            {
                Quaternion fishRot = Quaternion.Euler(0f, 25f + Random.Range(-5f, 5f), Random.Range(-8f, 8f));
                GameObject fish = InstantiatePrefab(PrefabFishV1, schoolGroup.transform, schoolGroup.transform.position + offsets[i], fishRot, new Vector3(1.2f, 1.2f, 1.2f), true);
                fish.name = $"SchoolMember_{i}";
                fish.transform.localPosition = offsets[i];
            }

            // Proximity interaction trigger
            var col = schoolGroup.AddComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = 4.5f;

            var diag = schoolGroup.AddComponent<DialogueTrigger>();
            diag.ClearLines();
            diag.triggerOnlyOnce = true;
            diag.triggerOnEnter = true;
            diag.interactRadius = 5.0f;
            diag.AddLine("School of Fish", "Greetings, surface friend! The currents whispered of your arrival.", new Color(0.4f, 0.85f, 1f));
            diag.AddLine("School of Fish", "The waters ahead grow murky and perilous, but we promise to help you in whatever way we can on your voyage!", new Color(0.4f, 0.85f, 1f));
            diag.AddLine("School of Fish", "Beware the giant boulder blocking the trench gateway ahead—it conceals an ancient mark of the beast.", new Color(0.4f, 0.85f, 1f));
        }

        private static void BuildAmbientMarineLife(Transform parent)
        {
            // Ambient fish groups swimming peacefully around the shallows
            // Group 1: 3 Angelfish (FishV2) swimming over southwest gardens
            CreateAmbientSwimmerSchool(parent, PrefabFishV2, new Vector3(-16f, 2.0f, -18f), 3, 3.8f, 1.8f, 1.3f);

            // Group 2: 3 Striped Butterflyfish (FishV3) swimming near the center reef
            CreateAmbientSwimmerSchool(parent, PrefabFishV3, new Vector3(2f, 2.3f, -2f), 3, 4.2f, 2.0f, 1.3f);

            // Group 3: 3 Moorish Idols (FishV4) swimming along the eastern sand flats
            CreateAmbientSwimmerSchool(parent, PrefabFishV4, new Vector3(18f, 2.5f, 16f), 3, 4.5f, 1.7f, 1.2f);

            // Group 4: 2 Tropical Fish (fish02) swimming through the northern kelp groves
            CreateAmbientSwimmerSchool(parent, PrefabFish02, new Vector3(-18f, 2.2f, 14f), 2, 3.5f, 1.5f, 0.6f);

            // Group 5: 2 Reef Fish (fish03) swimming near the relics
            CreateAmbientSwimmerSchool(parent, PrefabFish03, new Vector3(-6f, 2.4f, 20f), 2, 3.2f, 1.6f, 0.65f);
        }

        private static void CreateAmbientSwimmerSchool(Transform parent, string prefabPath, Vector3 center, int count, float radius, float speed, float scale)
        {
            GameObject group = new GameObject($"AmbientSwimmerGroup_{System.IO.Path.GetFileNameWithoutExtension(prefabPath)}");
            group.transform.SetParent(parent);
            group.transform.position = center;

            for (int i = 0; i < count; i++)
            {
                float ang = (i / (float)count) * Mathf.PI * 2f;
                Vector3 pos = center + new Vector3(Mathf.Cos(ang) * radius, Random.Range(-0.3f, 0.3f), Mathf.Sin(ang) * radius);
                GameObject fish = InstantiatePrefab(prefabPath, group.transform, pos, Quaternion.identity, Vector3.one * scale, true);
                fish.name = $"Swimmer_{i}";

                var swimmer = fish.AddComponent<AmbientFishSwimmer>();
                swimmer.Configure(center, radius + Random.Range(-0.4f, 0.4f), speed + Random.Range(-0.2f, 0.2f), Random.Range(1.8f, 2.6f));
            }
        }

        private static void BuildTutorialCourse(Transform parent, Material matRock)
        {
            GameObject course = new GameObject("Tutorial_Movement_And_Push_Course");
            course.transform.SetParent(parent);

            // Stepping stone rock platforms (Rock6A natural flat slabs)
            CreateRockPlatform(course.transform, PrefabRock6A, new Vector3(0f, 0.3f, 0f), new Vector3(0.45f, 0.35f, 0.4f), Quaternion.Euler(0f, 20f, 0f), "StepPlatform_1");
            CreateRockPlatform(course.transform, PrefabRock6A, new Vector3(-1.8f, 0.8f, 5f), new Vector3(0.5f, 0.5f, 0.45f), Quaternion.Euler(5f, -30f, 5f), "StepPlatform_2");
            CreateRockPlatform(course.transform, PrefabRock6A, new Vector3(1.8f, 1.3f, 10f), new Vector3(0.55f, 0.65f, 0.5f), Quaternion.Euler(-5f, 45f, -5f), "StepPlatform_3");

            // Tutorial Pushable Stones (Teaching pushing mechanic with real rock models)
            CreatePushableStone(course.transform, PrefabRock1C, new Vector3(-2.5f, 0.6f, 15f), new Vector3(1.1f, 1.1f, 1.1f), 2.5f, "Pushable_Stone_A");
            CreatePushableStone(course.transform, PrefabRock4B, new Vector3(2.5f, 0.7f, 19f), new Vector3(0.85f, 0.85f, 0.85f), 3.0f, "Pushable_Stone_B");
        }

        private static void CreateRockPlatform(Transform parent, string prefabPath, Vector3 pos, Vector3 scale, Quaternion rot, string name)
        {
            GameObject platform = InstantiatePrefab(prefabPath, parent, pos, rot, scale, false);
            platform.name = name;
        }

        private static void CreatePushableStone(Transform parent, string prefabPath, Vector3 pos, Vector3 scale, float mass, string name)
        {
            GameObject stone = InstantiatePrefab(prefabPath, parent, pos, Quaternion.identity, scale, false);
            stone.name = name;

            // Ensure smooth physical rolling/sliding
            var rb = stone.GetComponent<Rigidbody>();
            if (rb == null) rb = stone.AddComponent<Rigidbody>();
            rb.mass = mass;
            rb.linearDamping = 1.0f;
            rb.angularDamping = 1.5f;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            if (stone.GetComponent<Pushable>() == null) stone.AddComponent<Pushable>();
        }

        private static void BuildExplorationRelics(Transform parent, Material matPearl)
        {
            Vector3[] relicSpots = new Vector3[]
            {
                new Vector3(-16f, 0.8f, -22f),
                new Vector3(16f, 0.8f, -18f),
                new Vector3(-20f, 0.8f, 2f),
                new Vector3(20f, 0.8f, 8f),
                new Vector3(-14f, 0.8f, 22f),
                new Vector3(1.8f, 3.1f, 10.0f) // 6th relic resting atop StepPlatform_3 rock surface (Y=2.63)
            };

            for (int i = 0; i < relicSpots.Length; i++)
            {
                // Real 3D Pearl Seashell holding a glowing pearl!
                GameObject relic = InstantiatePrefab(PrefabPearlSeashell, parent, relicSpots[i], Quaternion.Euler(0f, i * 65f, 0f), new Vector3(1.15f, 1.15f, 1.15f), true);
                relic.name = $"Relic_Pearl_{i + 1}";

                var col = relic.AddComponent<SphereCollider>();
                col.isTrigger = true;
                col.radius = 1.0f;

                var comp = relic.AddComponent<CollectibleItem>();
                comp.itemIdentifier = $"Relic_{i + 1}";
                comp.itemName = "Sunlit Pearl Shell";

                if (i < 5)
                {
                    // Natural rock & flower pedestal bed on sand
                    InstantiatePrefab(PrefabRock1Grp2, parent, relicSpots[i] + new Vector3(0f, -0.6f, 0f), Quaternion.identity, new Vector3(1.2f, 0.8f, 1.2f), false);
                    InstantiatePrefab(PrefabSpinReefFlower1A, parent, relicSpots[i] + new Vector3(0.8f, -0.7f, 0.6f), Quaternion.Euler(0f, i * 45f, 0f), new Vector3(0.75f, 0.75f, 0.75f), true);
                    InstantiatePrefab(PrefabWaterLilyFlower, parent, relicSpots[i] + new Vector3(-0.7f, -0.7f, -0.5f), Quaternion.identity, new Vector3(1.2f, 1.2f, 1.2f), true);
                }
                else
                {
                    // 6th relic rests atop StepPlatform_3 - add blooming flower decoration and gentle warm glow
                    InstantiatePrefab(PrefabSpinReefFlower1A, parent, relicSpots[i] + new Vector3(0.55f, -0.2f, 0.45f), Quaternion.Euler(0f, 30f, 0f), new Vector3(0.75f, 0.75f, 0.75f), true);

                    // Add an unmistakable, warm luminous pearl glow so it is clearly visible
                    var pearlLight = relic.AddComponent<Light>();
                    pearlLight.type = LightType.Point;
                    pearlLight.color = new Color(1.0f, 0.95f, 0.75f);
                    pearlLight.range = 4.5f;
                    pearlLight.intensity = 2.0f;
                }
            }
        }

        private static void BuildBobbingOctopus(Transform parent)
        {
            GameObject oct = InstantiatePrefab(PrefabOctopus, parent, new Vector3(-14f, 2.3f, -12f), Quaternion.Euler(15f, 40f, 0f), new Vector3(1.4f, 1.4f, 1.4f), true);
            oct.name = "Bobbing_Octopus_NPC";
            oct.AddComponent<BobbingMotion>();

            // Blooming water lilies & rock perch around the octopus
            InstantiatePrefab(PrefabRock1Grp3, parent, new Vector3(-14f, 0f, -12f), Quaternion.identity, new Vector3(1.2f, 1.0f, 1.2f), false);
            InstantiatePrefab(PrefabWaterLilyFlower, parent, new Vector3(-13f, 0.1f, -11f), Quaternion.identity, new Vector3(1.4f, 1.4f, 1.4f), true);
            InstantiatePrefab(PrefabSpinReefFlower1A, parent, new Vector3(-15f, 0f, -13f), Quaternion.Euler(0f, 30f, 0f), new Vector3(0.9f, 0.9f, 0.9f), true);
        }

        private static void BuildFinalTrialAndSuckerScar(Transform parent, Material matRock, Material matScar, Material matHooks)
        {
            GameObject gateGroup = new GameObject("Final_Trial_Gateway");
            gateGroup.transform.SetParent(parent);
            gateGroup.transform.position = new Vector3(0f, 0f, 28f);

            // Gateway Cliffs built with impressive natural Rock5A and Rock4A pillars flanking the entrance
            InstantiatePrefab(PrefabRock5A, gateGroup.transform, gateGroup.transform.position + new Vector3(-6.5f, 0f, 0f), Quaternion.Euler(0f, 40f, 0f), new Vector3(0.8f, 0.8f, 0.8f), false);
            InstantiatePrefab(PrefabRock5A, gateGroup.transform, gateGroup.transform.position + new Vector3(6.5f, 0f, 0f), Quaternion.Euler(0f, -40f, 0f), new Vector3(0.8f, 0.8f, 0.8f), false);

            // Gateway Arch Lintel
            GameObject arch = GameObject.CreatePrimitive(PrimitiveType.Cube);
            arch.name = "Gateway_Lintel";
            arch.transform.SetParent(gateGroup.transform);
            arch.transform.localPosition = new Vector3(0f, 7.5f, 0f);
            arch.transform.localScale = new Vector3(14f, 2f, 3.5f);
            arch.GetComponent<MeshRenderer>().sharedMaterial = matRock;

            // The Giant Sucker Scar Rock Wall (behind the boulder)
            GameObject scarWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            scarWall.name = "Sucker_Scar_RockFace";
            scarWall.transform.SetParent(gateGroup.transform);
            scarWall.transform.localPosition = new Vector3(0f, 2.5f, 4f);
            scarWall.transform.localScale = new Vector3(7f, 5f, 2f);
            scarWall.GetComponent<MeshRenderer>().sharedMaterial = matRock;

            // The Giant Sucker Carving (Recessed dark circular sucker with hooked barbs)
            GameObject suckerDisc = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            suckerDisc.name = "Giant_Sucker_Indentation";
            suckerDisc.transform.SetParent(scarWall.transform);
            suckerDisc.transform.localPosition = new Vector3(0f, 0f, -0.55f);
            suckerDisc.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            suckerDisc.transform.localScale = new Vector3(0.55f, 0.05f, 0.55f);
            suckerDisc.GetComponent<MeshRenderer>().sharedMaterial = matScar;
            var discCol = suckerDisc.GetComponent<Collider>();
            if (discCol != null) Object.DestroyImmediate(discCol);

            // Swiveling Hook Barbs around the sucker rim
            for (int h = 0; h < 8; h++)
            {
                float ang = (h / 8f) * Mathf.PI * 2f;
                GameObject barb = GameObject.CreatePrimitive(PrimitiveType.Cube);
                barb.name = $"HookBarb_{h}";
                barb.transform.SetParent(suckerDisc.transform);
                barb.transform.localPosition = new Vector3(Mathf.Cos(ang) * 0.42f, 0.1f, Mathf.Sin(ang) * 0.42f);
                barb.transform.localScale = new Vector3(0.08f, 0.25f, 0.08f);
                barb.transform.localRotation = Quaternion.Euler(15f, h * 45f, 0f);
                barb.GetComponent<MeshRenderer>().sharedMaterial = matHooks;
                var barbCol = barb.GetComponent<Collider>();
                if (barbCol != null) Object.DestroyImmediate(barbCol);
            }

            // The Final Heavy Boulder (Blocking the scar) - Positioned cleanly in front of the carving
            GameObject finalBoulder = InstantiatePrefab(PrefabRock4B, gateGroup.transform, gateGroup.transform.position + new Vector3(0f, 1.35f, 0.4f), Quaternion.Euler(0f, 15f, 0f), new Vector3(1.4f, 1.4f, 1.4f), false);
            finalBoulder.name = "FinalBoulder";

            Rigidbody rb = finalBoulder.GetComponent<Rigidbody>();
            if (rb == null) rb = finalBoulder.AddComponent<Rigidbody>();
            rb.mass = 4.5f;
            rb.linearDamping = 1.0f;
            rb.angularDamping = 1.5f;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            if (finalBoulder.GetComponent<Pushable>() == null) finalBoulder.AddComponent<Pushable>();

            // Dedicated Discovery Zone trigger encompassing the front of the carving and the boulder
            GameObject discoveryZone = new GameObject("ClueDiscoveryZone");
            discoveryZone.transform.SetParent(gateGroup.transform);
            discoveryZone.transform.localPosition = new Vector3(0f, 1.5f, 1.5f);
            var triggerCollider = discoveryZone.AddComponent<BoxCollider>();
            triggerCollider.isTrigger = true;
            triggerCollider.size = new Vector3(8f, 4f, 8f);

            var clueComp = discoveryZone.AddComponent<ClueDiscoveryTrigger>();
            var field = typeof(ClueDiscoveryTrigger).GetField("blockingBoulder", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null) field.SetValue(clueComp, finalBoulder.transform);
        }

        private static void BuildPerimeterReefs(Transform parent, Material matRock)
        {
            GameObject cliffs = new GameObject("Perimeter_Reef_Cliffs");
            cliffs.transform.SetParent(parent);

            float halfSize = 38f;
            float height = 12f;
            float thickness = 4f;

            // Physical boundary invisible wall barriers so player/companion never fall into void
            CreateInvisibleBarrier(cliffs.transform, new Vector3(0f, height / 2, -halfSize), new Vector3(76f, height, thickness), "Barrier_South");
            CreateInvisibleBarrier(cliffs.transform, new Vector3(-halfSize, height / 2, 0f), new Vector3(thickness, height, 76f), "Barrier_West");
            CreateInvisibleBarrier(cliffs.transform, new Vector3(halfSize, height / 2, 0f), new Vector3(thickness, height, 76f), "Barrier_East");
            CreateInvisibleBarrier(cliffs.transform, new Vector3(-20f, height / 2, halfSize), new Vector3(36f, height, thickness), "Barrier_NorthWest");
            CreateInvisibleBarrier(cliffs.transform, new Vector3(20f, height / 2, halfSize), new Vector3(36f, height, thickness), "Barrier_NorthEast");

            // Dense jagged 3D rock cliffs lining the perimeter
            string[] cliffPrefabs = new string[] { PrefabRock2, PrefabRock3, PrefabRock5A, PrefabRock6A, PrefabRock1A };

            // South border
            for (float x = -35f; x <= 35f; x += 11f)
            {
                string p = cliffPrefabs[Mathf.Abs((int)x) % cliffPrefabs.Length];
                InstantiatePrefab(p, cliffs.transform, new Vector3(x, 0f, -halfSize + Random.Range(0f, 2.5f)), Quaternion.Euler(0f, Random.Range(0f, 360f), 0f), Vector3.one * Random.Range(0.75f, 1.25f), false);
            }

            // North border (leaving opening for gateway at X = 0)
            for (float x = -35f; x <= -10f; x += 11f)
            {
                string p = cliffPrefabs[Mathf.Abs((int)x) % cliffPrefabs.Length];
                InstantiatePrefab(p, cliffs.transform, new Vector3(x, 0f, halfSize - Random.Range(0f, 2.5f)), Quaternion.Euler(0f, Random.Range(0f, 360f), 0f), Vector3.one * Random.Range(0.75f, 1.25f), false);
            }
            for (float x = 10f; x <= 35f; x += 11f)
            {
                string p = cliffPrefabs[Mathf.Abs((int)x) % cliffPrefabs.Length];
                InstantiatePrefab(p, cliffs.transform, new Vector3(x, 0f, halfSize - Random.Range(0f, 2.5f)), Quaternion.Euler(0f, Random.Range(0f, 360f), 0f), Vector3.one * Random.Range(0.75f, 1.25f), false);
            }

            // West border
            for (float z = -32f; z <= 32f; z += 11f)
            {
                string p = cliffPrefabs[Mathf.Abs((int)z) % cliffPrefabs.Length];
                InstantiatePrefab(p, cliffs.transform, new Vector3(-halfSize + Random.Range(0f, 2.5f), 0f, z), Quaternion.Euler(0f, Random.Range(0f, 360f), 0f), Vector3.one * Random.Range(0.75f, 1.25f), false);
            }

            // East border
            for (float z = -32f; z <= 32f; z += 11f)
            {
                string p = cliffPrefabs[Mathf.Abs((int)z) % cliffPrefabs.Length];
                InstantiatePrefab(p, cliffs.transform, new Vector3(halfSize - Random.Range(0f, 2.5f), 0f, z), Quaternion.Euler(0f, Random.Range(0f, 360f), 0f), Vector3.one * Random.Range(0.75f, 1.25f), false);
            }
        }

        private static void CreateInvisibleBarrier(Transform parent, Vector3 pos, Vector3 scale, string name)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = name;
            wall.transform.SetParent(parent);
            wall.transform.position = pos;
            wall.transform.localScale = scale;
            wall.GetComponent<MeshRenderer>().enabled = false;
        }

        private static void BuildLivelyFlora(Transform parent)
        {
            GameObject floraRoot = new GameObject("Lively_Reef_Flora");
            floraRoot.transform.SetParent(parent);

            // 1. Blooming Sea Flower Beds across the Shallows
            string[] flowerPrefabs = new string[] { PrefabSpinReefFlower1A, PrefabWaterLilyFlower, PrefabSancheziaStar, PrefabRafflesia };

            for (int f = 0; f < 38; f++)
            {
                float x = ((f * 13) % 64) - 32f;
                float z = ((f * 23) % 64) - 32f;
                if (Mathf.Abs(x) < 4f && Mathf.Abs(z + 10f) < 8f) continue; // Clear path around turtle
                if (Mathf.Abs(x) < 3f && z < -24f) continue; // Clear start path

                string fp = flowerPrefabs[f % flowerPrefabs.Length];
                float s = (fp == PrefabRafflesia) ? Random.Range(0.45f, 0.75f) : ((fp == PrefabWaterLilyFlower) ? Random.Range(1.2f, 1.8f) : Random.Range(0.7f, 1.2f));

                InstantiatePrefab(fp, floraRoot.transform, new Vector3(x, 0f, z), Quaternion.Euler(0f, Random.Range(0f, 360f), 0f), Vector3.one * s, true);

                // Complement with water lily leaves
                if (f % 2 == 0)
                {
                    InstantiatePrefab(PrefabWaterLilyLeaf, floraRoot.transform, new Vector3(x + 0.4f, 0.05f, z + 0.3f), Quaternion.Euler(0f, Random.Range(0f, 360f), 0f), Vector3.one * Random.Range(1.0f, 1.6f), true);
                }
            }

            // 2. Corals, Antler Sprouts & Photogrammetry Plants
            string[] coralPrefabs = new string[] { PrefabSpinReef1A, PrefabAntlerSprout1A, PrefabDayAreshaBush1, PrefabDayAreshaBush2, PrefabBloodChuteCluster, PrefabSpiralChute1A, PrefabPadderPod1A };

            for (int c = 0; c < 48; c++)
            {
                float x = ((c * 17) % 66) - 33f;
                float z = ((c * 19) % 66) - 33f;
                if (Mathf.Abs(x) < 4.5f && Mathf.Abs(z + 10f) < 8.5f) continue;
                if (Mathf.Abs(x) < 3.5f && z < -24f) continue;

                string cp = coralPrefabs[c % coralPrefabs.Length];
                float s = Random.Range(0.7f, 1.35f);

                InstantiatePrefab(cp, floraRoot.transform, new Vector3(x, 0f, z), Quaternion.Euler(0f, Random.Range(0f, 360f), 0f), Vector3.one * s, true);
            }

            // 3. Dense Sea Grass & Swaying Kelp Groves
            string[] grassPrefabs = new string[] { PrefabGrassA1, PrefabGrassA2, PrefabGrassB1, PrefabGrassB2, PrefabFishHookKelp1, PrefabFishHookKelpRadial };

            for (int g = 0; g < 70; g++)
            {
                float x = ((g * 11) % 68) - 34f;
                float z = ((g * 29) % 68) - 34f;
                if (Mathf.Abs(x) < 4f && Mathf.Abs(z + 10f) < 7f) continue;

                string gp = grassPrefabs[g % grassPrefabs.Length];
                float s = Random.Range(0.85f, 1.4f);

                InstantiatePrefab(gp, floraRoot.transform, new Vector3(x, 0f, z), Quaternion.Euler(0f, Random.Range(0f, 360f), 0f), Vector3.one * s, true);
            }

            // 4. Natural Boulder & Rock Clusters scattered across the seafloor
            string[] rockClusterPrefabs = new string[] { PrefabRock1Grp1, PrefabRock1Grp2, PrefabRock1Grp3, PrefabRock1Grp4, PrefabRock1Grp5, PrefabRock1C, PrefabRock1E };

            for (int r = 0; r < 24; r++)
            {
                float x = ((r * 23) % 60) - 30f;
                float z = ((r * 31) % 60) - 30f;
                if (Mathf.Abs(x) < 5f && Mathf.Abs(z + 10f) < 9f) continue;
                if (Mathf.Abs(x) < 4f && z < -22f) continue;

                string rp = rockClusterPrefabs[r % rockClusterPrefabs.Length];
                float s = Random.Range(0.8f, 1.3f);

                InstantiatePrefab(rp, floraRoot.transform, new Vector3(x, 0f, z), Quaternion.Euler(0f, Random.Range(0f, 360f), 0f), Vector3.one * s, false);
            }
        }

        private static GameObject InstantiatePrefab(string assetPath, Transform parent, Vector3 position, Quaternion rotation, Vector3 scale, bool removeColliders = false)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (prefab == null)
            {
                Debug.LogWarning($"[Level0Builder] Could not load prefab at: {assetPath}");
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
