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

            // 1. Setup Materials
            Material matSand = AssetDatabase.LoadAssetAtPath<Material>($"{MaterialsFolder}/Sand.mat");
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

            // 4. Setup Hierarchy Roots
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

            // 5. Boundary Walls
            BuildPerimeterWalls(envRoot.transform, matRock);

            // 6. Partition: 1/4 Area (South) & 3/4 Area (North)
            // Dividing Ridge Wall at Z = -18 with central opening
            BuildDividingRidge(envRoot.transform, matRock);

            // 7. Player & Crab Companion Spawn (at Z = -34, far south in the 1/4 area)
            GameObject player = GameObject.Find("Player");
            if (player != null)
            {
                player.transform.position = new Vector3(0f, 1f, -34f);
                player.transform.rotation = Quaternion.identity;
                player.tag = "Player";
                if (player.GetComponent<PlayerMovement>() == null) player.AddComponent<PlayerMovement>();
                if (player.GetComponent<PlayerPusher>() == null) player.AddComponent<PlayerPusher>();
            }

            GameObject crab = GameObject.Find("Crab");
            if (crab != null && player != null)
            {
                crab.transform.position = new Vector3(-2.2f, 0.4f, -34f);
                crab.transform.rotation = Quaternion.identity;
                var companion = crab.GetComponent<Companion>();
                if (companion == null) companion = crab.AddComponent<Companion>();
                companion.FollowPlayer();
            }

            // Main Camera
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

            // 8. In the 1/4 Area: Hermit Crab's House & Vulnerable NPC (First destination!)
            BuildHermitCrabHouse(npcRoot.transform, matHermitCrab, matRock, matSand);

            // 9. In the 3/4 Area: Pacman-Style Kelp Maze (Corridors, Loops & Hiding Groves)
            BuildPacmanKelpMaze(envRoot.transform, matKelp, matKelpDense, matHidingGrove);

            // 10. Patrol Groups: 2 Groups of Barracudas with AI State Machine
            BuildBarracudaPatrolGroups(npcRoot.transform, matBarracuda, matVisionCone);

            // 11. Pushable Props in the Maze (Rocks & Shells for distraction/puzzles)
            BuildPushableProps(propsRoot.transform, matRock, matSand);

            // 12. Objectives: The Lost Golden Shell (Center Chamber) & Monster Clue Tablet (North)
            BuildObjectivesAndClues(objectivesRoot.transform, matHermitShell, matClueTablet, matRock);

            // Clean up old standalone Rock if present
            GameObject oldRock = GameObject.Find("Rock");
            if (oldRock != null && oldRock.transform.parent == null)
            {
                Undo.DestroyObjectImmediate(oldRock);
            }

            // 13. Bake Navigation Mesh
            navSurface.BuildNavMesh();

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveOpenScenes();

            Undo.CollapseUndoOperations(undoGroup);
            Debug.Log("<color=green><b>[Upon Heavy Tides]</b> Level 1 (The Kelp Forest Pac-Man Maze) fully built and baked!</color>");
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
                    l.intensity = 0.95f;
                }
                dirLight.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            }

            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = 0.022f;
            RenderSettings.fogColor = new Color(0.08f, 0.24f, 0.18f); // Murky olive green
            RenderSettings.ambientSkyColor = new Color(0.12f, 0.35f, 0.28f);
        }

        private static void BuildPerimeterWalls(Transform parent, Material matRock)
        {
            GameObject wallGroup = new GameObject("PerimeterCliffs");
            wallGroup.transform.SetParent(parent);

            float halfSize = 38f;
            float wallHeight = 12f;
            float wallThickness = 4f;

            CreateWall(wallGroup.transform, new Vector3(0f, wallHeight / 2, halfSize), new Vector3(76f, wallHeight, wallThickness), matRock, "Cliff_North");
            CreateWall(wallGroup.transform, new Vector3(0f, wallHeight / 2, -halfSize), new Vector3(76f, wallHeight, wallThickness), matRock, "Cliff_South");
            CreateWall(wallGroup.transform, new Vector3(-halfSize, wallHeight / 2, 0f), new Vector3(wallThickness, wallHeight, 76f), matRock, "Cliff_West");
            CreateWall(wallGroup.transform, new Vector3(halfSize, wallHeight / 2, 0f), new Vector3(wallThickness, wallHeight, 76f), matRock, "Cliff_East");
        }

        private static void BuildDividingRidge(Transform parent, Material matRock)
        {
            GameObject ridgeGroup = new GameObject("Dividing_Ridge_Boundary_1_4_Split");
            ridgeGroup.transform.SetParent(parent);

            float wallHeight = 7f;
            float thickness = 3.5f;

            // West portion (X: -36 to -5)
            CreateWall(ridgeGroup.transform, new Vector3(-20f, wallHeight / 2, -18f), new Vector3(32f, wallHeight, thickness), matRock, "Ridge_West");

            // East portion (X: +5 to +36)
            CreateWall(ridgeGroup.transform, new Vector3(20f, wallHeight / 2, -18f), new Vector3(32f, wallHeight, thickness), matRock, "Ridge_East");

            // Central Archway Gateway
            GameObject arch = GameObject.CreatePrimitive(PrimitiveType.Cube);
            arch.name = "Kelp_Maze_Entrance_Arch";
            arch.transform.SetParent(ridgeGroup.transform);
            arch.transform.position = new Vector3(0f, 6.5f, -18f);
            arch.transform.localScale = new Vector3(10f, 2f, 3.5f);
            arch.GetComponent<MeshRenderer>().sharedMaterial = matRock;
        }

        private static void BuildHermitCrabHouse(Transform parent, Material matHermitCrab, Material matRock, Material matSand)
        {
            // Positioned right in front of spawn in the 1/4 starting area
            GameObject houseRoot = new GameObject("HermitCrab_Sanctuary_House");
            houseRoot.transform.SetParent(parent);
            houseRoot.transform.position = new Vector3(0f, 0f, -26f);

            // Coral Stone Alcove House Structure
            GameObject backWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            backWall.name = "House_Back_Cave";
            backWall.transform.SetParent(houseRoot.transform);
            backWall.transform.localPosition = new Vector3(0f, 2.5f, 3f);
            backWall.transform.localScale = new Vector3(8f, 5f, 2.5f);
            backWall.GetComponent<MeshRenderer>().sharedMaterial = matRock;

            GameObject leftWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leftWall.name = "House_Left_Wall";
            leftWall.transform.SetParent(houseRoot.transform);
            leftWall.transform.localPosition = new Vector3(-3.8f, 2.2f, 0.5f);
            leftWall.transform.localScale = new Vector3(2.2f, 4.4f, 4f);
            leftWall.GetComponent<MeshRenderer>().sharedMaterial = matRock;

            GameObject rightWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rightWall.name = "House_Right_Wall";
            rightWall.transform.SetParent(houseRoot.transform);
            rightWall.transform.localPosition = new Vector3(3.8f, 2.2f, 0.5f);
            rightWall.transform.localScale = new Vector3(2.2f, 4.4f, 4f);
            rightWall.GetComponent<MeshRenderer>().sharedMaterial = matRock;

            // Sandbed mat
            GameObject bed = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            bed.name = "Hermit_Bed";
            bed.transform.SetParent(houseRoot.transform);
            bed.transform.localPosition = new Vector3(0f, 0.1f, 0.8f);
            bed.transform.localScale = new Vector3(3.5f, 0.1f, 3.5f);
            bed.GetComponent<MeshRenderer>().sharedMaterial = matSand;

            // The Shell-Less Hermit Crab NPC
            GameObject crabObj = new GameObject("HermitCrab_Ailing_NPC");
            crabObj.transform.SetParent(houseRoot.transform);
            crabObj.transform.localPosition = new Vector3(0f, 0.45f, 0.8f);

            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            body.name = "Body_Vulnerable_Naked";
            body.transform.SetParent(crabObj.transform);
            body.transform.localPosition = Vector3.zero;
            body.transform.localScale = new Vector3(1.2f, 0.75f, 1.0f);
            body.GetComponent<MeshRenderer>().sharedMaterial = matHermitCrab;

            GameObject leftClaw = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            leftClaw.name = "Claw_Left";
            leftClaw.transform.SetParent(crabObj.transform);
            leftClaw.transform.localPosition = new Vector3(-0.7f, -0.1f, 0.5f);
            leftClaw.transform.localScale = new Vector3(0.5f, 0.35f, 0.6f);
            leftClaw.GetComponent<MeshRenderer>().sharedMaterial = matHermitCrab;

            GameObject rightClaw = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rightClaw.name = "Claw_Right";
            rightClaw.transform.SetParent(crabObj.transform);
            rightClaw.transform.localPosition = new Vector3(0.7f, -0.1f, 0.5f);
            rightClaw.transform.localScale = new Vector3(0.5f, 0.35f, 0.6f);
            rightClaw.GetComponent<MeshRenderer>().sharedMaterial = matHermitCrab;

            // Dialogue Trigger for Hermit Crab
            var diag = crabObj.AddComponent<DialogueTrigger>();
            diag.AddLine("Hermit Crab", "Oh, woe is me! Please don't look at me... I'm completely naked and defenseless without my shell!", new Color(1f, 0.55f, 0.3f));
            diag.AddLine("Crab Companion", "Where is your golden spiral shell, friend? It's dangerous out in the open ocean without your armor!", new Color(1f, 0.75f, 0.3f));
            diag.AddLine("Hermit Crab", "The barracudas! They ambushed me and chased me out of the kelp forest. My golden shell was knocked loose and dragged deep into that twisted maze to the north!", new Color(1f, 0.55f, 0.3f));
            diag.AddLine("Hermit Crab", "Watch out for the two barracuda patrol groups swimming through the corridors! Use the dense hiding groves along the walls to duck out of their line of sight!", new Color(1f, 0.55f, 0.3f));
            diag.AddLine("Crab Companion", "Don't you worry, friend! We will brave the kelp maze, evade the barracudas, and bring your shell back safely!", new Color(1f, 0.75f, 0.3f));

            // Proximity interaction trigger
            var col = crabObj.AddComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = 4.0f;
        }

        private static void BuildPacmanKelpMaze(Transform parent, Material matKelp, Material matKelpDense, Material matHidingGrove)
        {
            GameObject mazeRoot = new GameObject("Pacman_Kelp_Maze_3_4_Area");
            mazeRoot.transform.SetParent(parent);

            // Pacman-Style Corridor Kelp Wall Segments (Grid from Z: -16 to +36, X: -26 to +26)
            // Format: (CenterX, CenterZ, LengthX, LengthZ)
            var wallSegments = new List<Vector4>
            {
                // Outer corridor borders (Left & Right)
                new Vector4(-24f, 9f, 2.5f, 50f),
                new Vector4(24f, 9f, 2.5f, 50f),

                // Lower horizontal maze divider
                new Vector4(-14f, -10f, 14f, 2.5f),
                new Vector4(14f, -10f, 14f, 2.5f),

                // Lower vertical maze pillars
                new Vector4(-8f, -4f, 2.5f, 12f),
                new Vector4(8f, -4f, 2.5f, 12f),

                // Center T-blocks (forming Pacman corridors)
                new Vector4(0f, -2f, 12f, 2.5f),
                new Vector4(0f, 3f, 2.5f, 8f),

                // Center Chamber Alcove Walls (encasing the Golden Shell)
                new Vector4(-7f, 10f, 2.5f, 10f),
                new Vector4(7f, 10f, 2.5f, 10f),
                new Vector4(0f, 15f, 14f, 2.5f), // North wall of center chamber

                // Upper cross dividers
                new Vector4(-15f, 18f, 12f, 2.5f),
                new Vector4(15f, 18f, 12f, 2.5f),
                new Vector4(-8f, 24f, 2.5f, 10f),
                new Vector4(8f, 24f, 2.5f, 10f),

                // Far North Altar Gateway Wall
                new Vector4(-14f, 30f, 14f, 2.5f),
                new Vector4(14f, 30f, 14f, 2.5f)
            };

            int wallIdx = 0;
            foreach (var w in wallSegments)
            {
                BuildDenseKelpWall(mazeRoot.transform, new Vector3(w.x, 0f, w.y), w.z, w.w, matKelp, matKelpDense, $"KelpWall_{wallIdx++}");
            }

            // 4 Stealth Hiding Groves in corner junctions
            BuildHidingGrove(mazeRoot.transform, new Vector3(-18f, 0f, -12f), "HidingGrove_SW", matHidingGrove, matKelpDense);
            BuildHidingGrove(mazeRoot.transform, new Vector3(18f, 0f, -12f), "HidingGrove_SE", matHidingGrove, matKelpDense);
            BuildHidingGrove(mazeRoot.transform, new Vector3(-18f, 0f, 20f), "HidingGrove_NW", matHidingGrove, matKelpDense);
            BuildHidingGrove(mazeRoot.transform, new Vector3(18f, 0f, 20f), "HidingGrove_NE", matHidingGrove, matKelpDense);
        }

        private static void BuildDenseKelpWall(Transform parent, Vector3 center, float lengthX, float lengthZ, Material matKelp, Material matKelpDense, string name)
        {
            GameObject wallObj = new GameObject(name);
            wallObj.transform.SetParent(parent);
            wallObj.transform.position = center;

            // Physical block collider inside kelp to prevent walking through
            GameObject colBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
            colBlock.name = "Kelp_Collider";
            colBlock.transform.SetParent(wallObj.transform);
            colBlock.transform.localPosition = new Vector3(0f, 3.5f, 0f);
            colBlock.transform.localScale = new Vector3(lengthX, 7.0f, lengthZ);
            colBlock.GetComponent<MeshRenderer>().enabled = false; // Invisible collider

            // Stalk generation along the wall volume
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

            GameObject zone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            zone.name = "StealthSafeZone_Trigger";
            zone.transform.SetParent(grove.transform);
            zone.transform.localPosition = new Vector3(0f, 0.05f, 0f);
            zone.transform.localScale = new Vector3(4.8f, 0.05f, 4.8f);
            zone.GetComponent<MeshRenderer>().sharedMaterial = matGrove;
            zone.GetComponent<Collider>().isTrigger = true;

            // Stealth trigger logic
            var stealth = zone.AddComponent<StealthZoneTrigger>();

            // Surrounding curtain stalks
            for (int i = 0; i < 8; i++)
            {
                float ang = (i / 8f) * Mathf.PI * 2f;
                Vector3 p = new Vector3(Mathf.Cos(ang) * 2.3f, 4.5f, Mathf.Sin(ang) * 2.3f);
                GameObject s = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                s.name = $"Curtain_{i}";
                s.transform.SetParent(grove.transform);
                s.transform.localPosition = p;
                s.transform.localScale = new Vector3(0.35f, 4.5f, 0.35f);
                s.GetComponent<MeshRenderer>().sharedMaterial = matKelp;
            }
        }

        private static void BuildBarracudaPatrolGroups(Transform parent, Material matBarracuda, Material matVision)
        {
            GameObject enemyGroup = new GameObject("Barracuda_Patrol_Groups");
            enemyGroup.transform.SetParent(parent);

            // Group 1 Waypoints (South/Central Loop)
            var wpGroup1 = CreateWaypointLoop(enemyGroup.transform, "Waypoints_Group_1", new Vector3[]
            {
                new Vector3(-14f, 1.8f, -12f),
                new Vector3(14f, 1.8f, -12f),
                new Vector3(14f, 1.8f, 2f),
                new Vector3(-14f, 1.8f, 2f)
            });

            // Group 1: 2 Barracudas swimming the loop
            BuildBarracudaUnit(enemyGroup.transform, new Vector3(-14f, 1.8f, -12f), "Barracuda_Alpha_South", matBarracuda, matVision, wpGroup1);
            BuildBarracudaUnit(enemyGroup.transform, new Vector3(14f, 1.8f, 2f), "Barracuda_Beta_South", matBarracuda, matVision, wpGroup1);

            // Group 2 Waypoints (North/East Loop)
            var wpGroup2 = CreateWaypointLoop(enemyGroup.transform, "Waypoints_Group_2", new Vector3[]
            {
                new Vector3(-14f, 1.8f, 12f),
                new Vector3(14f, 1.8f, 12f),
                new Vector3(14f, 1.8f, 26f),
                new Vector3(-14f, 1.8f, 26f)
            });

            // Group 2: 2 Barracudas swimming the loop
            BuildBarracudaUnit(enemyGroup.transform, new Vector3(-14f, 1.8f, 12f), "Barracuda_Alpha_North", matBarracuda, matVision, wpGroup2);
            BuildBarracudaUnit(enemyGroup.transform, new Vector3(14f, 1.8f, 26f), "Barracuda_Beta_North", matBarracuda, matVision, wpGroup2);
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

        private static void BuildBarracudaUnit(Transform parent, Vector3 pos, string name, Material matFish, Material matVision, List<Transform> waypoints)
        {
            GameObject barracuda = new GameObject(name);
            barracuda.transform.SetParent(parent);
            barracuda.transform.position = pos;

            // Body
            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.name = "Predator_Body";
            body.transform.SetParent(barracuda.transform);
            body.transform.localPosition = Vector3.zero;
            body.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            body.transform.localScale = new Vector3(0.6f, 1.6f, 0.6f);
            body.GetComponent<MeshRenderer>().sharedMaterial = matFish;

            // Snout
            GameObject snout = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            snout.name = "Jaws_Snout";
            snout.transform.SetParent(barracuda.transform);
            snout.transform.localPosition = new Vector3(0f, 0f, 1.6f);
            snout.transform.localScale = new Vector3(0.45f, 0.4f, 0.8f);
            snout.GetComponent<MeshRenderer>().sharedMaterial = matFish;

            // Dorsal Fin
            GameObject fin = GameObject.CreatePrimitive(PrimitiveType.Cube);
            fin.name = "DorsalFin";
            fin.transform.SetParent(barracuda.transform);
            fin.transform.localPosition = new Vector3(0f, 0.55f, -0.3f);
            fin.transform.localScale = new Vector3(0.08f, 0.5f, 0.7f);
            fin.GetComponent<MeshRenderer>().sharedMaterial = matFish;

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

            var ai = barracuda.AddComponent<BarracudaAI>();
            ai.SetWaypoints(waypoints);

            var field = typeof(BarracudaAI).GetField("visionConeRenderer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null) field.SetValue(ai, vRend);
        }

        private static void BuildPushableProps(Transform parent, Material matRock, Material matSand)
        {
            GameObject propsGroup = new GameObject("Pushable_Physics_Props");
            propsGroup.transform.SetParent(parent);

            CreatePushableRock(propsGroup.transform, new Vector3(-12f, 0.6f, -6f), new Vector3(1.4f, 1.4f, 1.4f), 3.5f, matRock, "Pushable_Rock_SouthWest");
            CreatePushableRock(propsGroup.transform, new Vector3(12f, 0.6f, 6f), new Vector3(1.4f, 1.4f, 1.4f), 3.5f, matRock, "Pushable_Rock_EastLoop");
            CreatePushableRock(propsGroup.transform, new Vector3(0f, 0.7f, 22f), new Vector3(1.8f, 1.8f, 1.8f), 4.5f, matRock, "Pushable_Rock_NorthChamber");
        }

        private static void CreatePushableRock(Transform parent, Vector3 pos, Vector3 scale, float mass, Material mat, string name)
        {
            GameObject rock = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rock.name = name;
            rock.transform.SetParent(parent);
            rock.transform.position = pos;
            rock.transform.localScale = scale;
            rock.GetComponent<MeshRenderer>().sharedMaterial = mat;

            Rigidbody rb = rock.AddComponent<Rigidbody>();
            rb.mass = mass;
            rb.linearDamping = 1.0f;
            rb.angularDamping = 1.5f;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            rock.AddComponent<Pushable>();
        }

        private static void BuildObjectivesAndClues(Transform parent, Material matShell, Material matClue, Material matRock)
        {
            GameObject objGroup = new GameObject("Objectives_And_Clues");
            objGroup.transform.SetParent(parent);

            // 1. The Lost Golden Hermit Shell (Center Chamber at Z = 8)
            GameObject shellChamber = new GameObject("Objective_LostGoldenShell");
            shellChamber.transform.SetParent(objGroup.transform);
            shellChamber.transform.position = new Vector3(0f, 0f, 8f);

            GameObject pedestal = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pedestal.name = "Shell_Pedestal";
            pedestal.transform.SetParent(shellChamber.transform);
            pedestal.transform.localPosition = new Vector3(0f, 0.4f, 0f);
            pedestal.transform.localScale = new Vector3(2.5f, 0.4f, 2.5f);
            pedestal.GetComponent<MeshRenderer>().sharedMaterial = matRock;

            GameObject shell = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            shell.name = "Hermit_Golden_Shell";
            shell.transform.SetParent(shellChamber.transform);
            shell.transform.localPosition = new Vector3(0f, 1.2f, 0f);
            shell.transform.localScale = new Vector3(1.4f, 1.1f, 1.6f);
            shell.GetComponent<MeshRenderer>().sharedMaterial = matShell;
            shell.GetComponent<Collider>().isTrigger = true;

            // Collectible Item script
            var colItem = shell.AddComponent<CollectibleItem>();
            colItem.itemName = "Lost Golden Hermit Shell";
            colItem.itemIdentifier = "HermitShell";

            // 2. The Ancient Monster Clue Tablet (Far North Chamber at Z = 32)
            GameObject clueChamber = new GameObject("Objective_MonsterClue_Tablet");
            clueChamber.transform.SetParent(objGroup.transform);
            clueChamber.transform.position = new Vector3(0f, 0f, 32f);

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

            // Trigger for recording second clue
            var triggerCol = clueChamber.AddComponent<BoxCollider>();
            triggerCol.isTrigger = true;
            triggerCol.size = new Vector3(6f, 3f, 6f);
        }

        private static void CreateWall(Transform parent, Vector3 pos, Vector3 scale, Material mat, string name)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = name;
            wall.transform.SetParent(parent);
            wall.transform.position = pos;
            wall.transform.localScale = scale;
            wall.GetComponent<MeshRenderer>().sharedMaterial = mat;
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
