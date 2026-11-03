using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.AI;
using Unity.AI.Navigation;
using UponHeavyTides.AI;
using UponHeavyTides.Interaction;
using UponHeavyTides.Player;

namespace UponHeavyTides.Editor
{
    public static class Level1BlockoutBuilder
    {
        private const string ScenePath = "Assets/Scenes/Level1.unity";
        private const string MaterialsFolder = "Assets/Materials";

        [MenuItem("Upon Heavy Tides/Build Level 1 Blockout")]
        public static void BuildLevel1()
        {
            // Ensure Level1 is open
            if (EditorSceneManager.GetActiveScene().path != ScenePath)
            {
                EditorSceneManager.OpenScene(ScenePath);
            }

            Undo.IncrementCurrentGroup();
            int undoGroup = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Build Level 1 Kelp Forest Blockout");

            // 1. Setup Materials
            Material matSand = AssetDatabase.LoadAssetAtPath<Material>($"{MaterialsFolder}/Sand.mat");
            Material matRock = AssetDatabase.LoadAssetAtPath<Material>($"{MaterialsFolder}/Rock.mat");
            Material matKelp = GetOrCreateMaterial("Mat_KelpGreen", new Color(0.18f, 0.48f, 0.22f), 0.2f, 0.1f);
            Material matKelpDense = GetOrCreateMaterial("Mat_KelpDense", new Color(0.09f, 0.32f, 0.14f), 0.3f, 0.1f);
            Material matHermitCrab = GetOrCreateMaterial("Mat_HermitCrab", new Color(0.92f, 0.38f, 0.22f), 0.4f, 0.2f);
            Material matBarracuda = GetOrCreateMaterial("Mat_Barracuda", new Color(0.22f, 0.35f, 0.52f), 0.6f, 0.7f);
            Material matVisionCone = GetOrCreateMaterial("Mat_VisionCone", new Color(1.0f, 0.88f, 0.25f, 0.35f), 0.1f, 0.0f, true);
            Material matHermitShell = GetOrCreateMaterial("Mat_HermitShell", new Color(0.96f, 0.78f, 0.24f), 0.5f, 0.6f);
            Material matClueTablet = GetOrCreateMaterial("Mat_ClueTablet", new Color(0.25f, 0.85f, 0.95f), 0.8f, 0.3f);
            Material matHidingGrove = GetOrCreateMaterial("Mat_HidingGrove", new Color(0.12f, 0.65f, 0.35f, 0.5f), 0.1f, 0.0f, true);

            // 2. Underwater Atmospheric Lighting & Fog
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
            if (matSand != null)
            {
                ground.GetComponent<MeshRenderer>().sharedMaterial = matSand;
            }

            // Ensure NavMeshSurface exists on ground
            NavMeshSurface navSurface = ground.GetComponent<NavMeshSurface>();
            if (navSurface == null)
            {
                navSurface = ground.AddComponent<NavMeshSurface>();
            }

            // 4. Setup Hierarchy Roots
            GameObject envRoot = GetOrCreateRoot("_ENVIRONMENT");
            GameObject propsRoot = GetOrCreateRoot("_PROPS_AND_INTERACTIONS");
            GameObject npcRoot = GetOrCreateRoot("_NPCS_AND_ENEMIES");
            GameObject objectivesRoot = GetOrCreateRoot("_OBJECTIVES_AND_CLUES");

            // Clear previous generated children under roots (if re-running)
            ClearChildren(envRoot);
            ClearChildren(propsRoot);
            ClearChildren(npcRoot);
            ClearChildren(objectivesRoot);

            // 5. Boundary Walls / Sea Cliffs (Enclose the arena)
            BuildPerimeterWalls(envRoot.transform, matRock);

            // 6. Player & Crab Companion Start Position
            GameObject player = GameObject.Find("Player");
            if (player != null)
            {
                player.transform.position = new Vector3(0f, 1f, -32f);
                player.transform.rotation = Quaternion.identity;
            }

            GameObject crab = GameObject.Find("Crab");
            if (crab != null)
            {
                crab.transform.position = new Vector3(-2.5f, 0.3f, -31f);
                crab.transform.rotation = Quaternion.identity;
            }

            // Reposition Main Camera
            GameObject mainCam = GameObject.Find("Main Camera");
            if (mainCam != null && player != null)
            {
                mainCam.transform.position = player.transform.position + new Vector3(0f, 4f, -7f);
            }

            // 7. Hermit Crab (Ailing Friend)
            BuildHermitCrab(npcRoot.transform, matHermitCrab);

            // 8. Kelp Forest Maze (Pillars, Paths, and Hiding Groves)
            BuildKelpForestMaze(envRoot.transform, matKelp, matKelpDense, matHidingGrove);

            // 9. Barracudas (Patrolling Enemies with Vision Cones)
            BuildBarracudas(npcRoot.transform, matBarracuda, matVisionCone);

            // 10. Pushable Distraction Props (Rocks & Shells)
            BuildPushableProps(propsRoot.transform, matRock, matSand);

            // 11. Objective: The Lost Shell & Clue Monument (Huge Eyes)
            BuildObjectiveAndClue(objectivesRoot.transform, matHermitShell, matClueTablet, matRock);

            // 12. Choice Branches & Signs
            BuildChoiceBranches(objectivesRoot.transform, matClueTablet, matRock);

            // Clean up old standalone Rock from Level 0 if still at root
            GameObject oldRock = GameObject.Find("Rock");
            if (oldRock != null && oldRock.transform.parent == null)
            {
                Undo.DestroyObjectImmediate(oldRock);
            }

            // 13. Bake NavMesh
            navSurface.BuildNavMesh();

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveOpenScenes();

            Undo.CollapseUndoOperations(undoGroup);
            Debug.Log("<color=green><b>[Upon Heavy Tides]</b> Level 1 (The Kelp Forest) blockout successfully generated and baked!</color>");
        }

        private static void SetupAtmosphere()
        {
            // Underwater green murky directional light
            GameObject dirLight = GameObject.Find("Directional Light");
            if (dirLight != null)
            {
                Light l = dirLight.GetComponent<Light>();
                if (l != null)
                {
                    l.color = new Color(0.42f, 0.76f, 0.65f); // Underwater turquoise/green
                    l.intensity = 0.85f;
                }
                dirLight.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            }

            // Underwater fog settings
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = 0.022f;
            RenderSettings.fogColor = new Color(0.07f, 0.22f, 0.18f); // Murky olive/deep green
            RenderSettings.ambientSkyColor = new Color(0.12f, 0.35f, 0.28f);
        }

        private static void BuildPerimeterWalls(Transform parent, Material matRock)
        {
            GameObject wallGroup = new GameObject("PerimeterCliffs");
            wallGroup.transform.SetParent(parent);

            float halfSize = 38f;
            float wallHeight = 12f;
            float wallThickness = 4f;

            // North Wall (with opening for exit)
            CreateWall(wallGroup.transform, new Vector3(-18f, wallHeight / 2, halfSize), new Vector3(36f, wallHeight, wallThickness), matRock, "Cliff_North_West");
            CreateWall(wallGroup.transform, new Vector3(22f, wallHeight / 2, halfSize), new Vector3(28f, wallHeight, wallThickness), matRock, "Cliff_North_East");

            // South Wall (behind spawn)
            CreateWall(wallGroup.transform, new Vector3(0f, wallHeight / 2, -halfSize), new Vector3(76f, wallHeight, wallThickness), matRock, "Cliff_South");

            // West Wall
            CreateWall(wallGroup.transform, new Vector3(-halfSize, wallHeight / 2, 0f), new Vector3(wallThickness, wallHeight, 76f), matRock, "Cliff_West");

            // East Wall
            CreateWall(wallGroup.transform, new Vector3(halfSize, wallHeight / 2, 0f), new Vector3(wallThickness, wallHeight, 76f), matRock, "Cliff_East");
        }

        private static void BuildHermitCrab(Transform parent, Material matHermitCrab)
        {
            GameObject hermitGroup = new GameObject("HermitCrab_Ailing_NPC");
            hermitGroup.transform.SetParent(parent);
            hermitGroup.transform.position = new Vector3(5f, 0f, -27f);

            // Body (soft vulnerable shell-less body)
            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            body.name = "Body_Vulnerable";
            body.transform.SetParent(hermitGroup.transform);
            body.transform.localPosition = new Vector3(0f, 0.45f, 0f);
            body.transform.localScale = new Vector3(1.1f, 0.7f, 0.9f);
            body.GetComponent<MeshRenderer>().sharedMaterial = matHermitCrab;

            // Left claw
            GameObject leftClaw = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            leftClaw.name = "Claw_Left";
            leftClaw.transform.SetParent(hermitGroup.transform);
            leftClaw.transform.localPosition = new Vector3(-0.65f, 0.35f, 0.4f);
            leftClaw.transform.localScale = new Vector3(0.45f, 0.3f, 0.55f);
            leftClaw.GetComponent<MeshRenderer>().sharedMaterial = matHermitCrab;

            // Right claw
            GameObject rightClaw = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rightClaw.name = "Claw_Right";
            rightClaw.transform.SetParent(hermitGroup.transform);
            rightClaw.transform.localPosition = new Vector3(0.65f, 0.35f, 0.4f);
            rightClaw.transform.localScale = new Vector3(0.45f, 0.3f, 0.55f);
            rightClaw.GetComponent<MeshRenderer>().sharedMaterial = matHermitCrab;

            // Anxious shelter rock behind hermit crab
            GameObject shelterRock = GameObject.CreatePrimitive(PrimitiveType.Cube);
            shelterRock.name = "Shelter_Rock";
            shelterRock.transform.SetParent(hermitGroup.transform);
            shelterRock.transform.localPosition = new Vector3(1.2f, 1.2f, -0.6f);
            shelterRock.transform.localScale = new Vector3(1.8f, 2.4f, 2.2f);
            shelterRock.transform.localRotation = Quaternion.Euler(15f, 35f, 10f);

            // NPC Objective Marker/Banner
            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            marker.name = "NPC_ObjectiveMarker_ReturnShellHere";
            marker.transform.SetParent(hermitGroup.transform);
            marker.transform.localPosition = new Vector3(0f, 2.8f, 0f);
            marker.transform.localScale = new Vector3(0.6f, 0.15f, 0.6f);
            marker.GetComponent<Collider>().isTrigger = true;
        }

        private static void BuildKelpForestMaze(Transform parent, Material matKelp, Material matKelpDense, Material matHidingGrove)
        {
            GameObject mazeGroup = new GameObject("KelpForest_Maze");
            mazeGroup.transform.SetParent(parent);

            // Algorithmic Kelp Clusters to create labyrinth paths
            // Path leads from Z: -25 to Z: +20
            var clusterCenters = new List<Vector3>
            {
                // West Barrier Ridge
                new Vector3(-18f, 0f, -20f),
                new Vector3(-20f, 0f, -10f),
                new Vector3(-18f, 0f, 0f),
                new Vector3(-22f, 0f, 12f),

                // East Barrier Ridge
                new Vector3(18f, 0f, -20f),
                new Vector3(20f, 0f, -8f),
                new Vector3(18f, 0f, 4f),
                new Vector3(22f, 0f, 14f),

                // Maze Partitions & Islands
                new Vector3(-7f, 0f, -18f),
                new Vector3(8f, 0f, -16f),
                new Vector3(-12f, 0f, -6f),
                new Vector3(3f, 0f, -8f),
                new Vector3(-4f, 0f, 5f),
                new Vector3(10f, 0f, 6f),
                new Vector3(-14f, 0f, 16f),
                new Vector3(12f, 0f, 18f),
                new Vector3(-4f, 0f, 20f),
                new Vector3(5f, 0f, 22f)
            };

            int clusterIdx = 0;
            foreach (var center in clusterCenters)
            {
                GameObject clusterObj = new GameObject($"KelpCluster_{clusterIdx++}");
                clusterObj.transform.SetParent(mazeGroup.transform);
                clusterObj.transform.position = center;

                int stalkCount = 10;
                for (int s = 0; s < stalkCount; s++)
                {
                    float angle = (s / (float)stalkCount) * Mathf.PI * 2f;
                    float radius = 1.2f + (s % 3) * 0.8f;
                    Vector3 stalkPos = center + new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);

                    GameObject stalk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    stalk.name = $"Stalk_{s}";
                    stalk.transform.SetParent(clusterObj.transform);

                    float stalkHeight = 7f + (s % 5) * 1.5f;
                    float stalkWidth = 0.45f + (s % 3) * 0.2f;
                    stalk.transform.position = new Vector3(stalkPos.x, stalkHeight / 2f, stalkPos.z);
                    stalk.transform.localScale = new Vector3(stalkWidth, stalkHeight / 2f, stalkWidth);
                    stalk.transform.rotation = Quaternion.Euler(Mathf.Sin(s) * 4f, s * 30f, Mathf.Cos(s) * 4f);

                    Material stalkMat = (s % 2 == 0) ? matKelp : matKelpDense;
                    stalk.GetComponent<MeshRenderer>().sharedMaterial = stalkMat;
                }
            }

            // 3 Stealth / Hiding Groves (Zones where player can break line of sight)
            BuildHidingGrove(mazeGroup.transform, new Vector3(-10f, 0f, -12f), "HidingGrove_West_1", matHidingGrove, matKelpDense);
            BuildHidingGrove(mazeGroup.transform, new Vector3(0f, 0f, -1f), "HidingGrove_Center_2", matHidingGrove, matKelpDense);
            BuildHidingGrove(mazeGroup.transform, new Vector3(8f, 0f, 12f), "HidingGrove_East_3", matHidingGrove, matKelpDense);
        }

        private static void BuildHidingGrove(Transform parent, Vector3 center, string name, Material matGrove, Material matKelp)
        {
            GameObject grove = new GameObject(name);
            grove.transform.SetParent(parent);
            grove.transform.position = center;

            // Visual ground zone
            GameObject zone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            zone.name = "StealthSafeZone_Trigger";
            zone.transform.SetParent(grove.transform);
            zone.transform.position = new Vector3(center.x, 0.05f, center.z);
            zone.transform.localScale = new Vector3(5f, 0.05f, 5f);
            zone.GetComponent<MeshRenderer>().sharedMaterial = matGrove;
            zone.GetComponent<Collider>().isTrigger = true;

            // Dense curtain stalks
            for (int i = 0; i < 8; i++)
            {
                float ang = (i / 8f) * Mathf.PI * 2f;
                Vector3 p = center + new Vector3(Mathf.Cos(ang) * 2.2f, 0f, Mathf.Sin(ang) * 2.2f);
                GameObject s = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                s.name = $"CurtainStalk_{i}";
                s.transform.SetParent(grove.transform);
                s.transform.position = new Vector3(p.x, 4.5f, p.z);
                s.transform.localScale = new Vector3(0.5f, 4.5f, 0.5f);
                s.GetComponent<MeshRenderer>().sharedMaterial = matKelp;
            }
        }

        private static void BuildBarracudas(Transform parent, Material matBarracuda, Material matVision)
        {
            GameObject enemyGroup = new GameObject("Barracuda_Predator_Patrols");
            enemyGroup.transform.SetParent(parent);

            // Barracuda 1 (Patrolling South-Central lane)
            BuildBarracudaUnit(enemyGroup.transform, new Vector3(-6f, 1.8f, -7f), Quaternion.Euler(0f, 90f, 0f), "Barracuda_Patrol_South", matBarracuda, matVision);

            // Barracuda 2 (Patrolling North-Puzzle approach)
            BuildBarracudaUnit(enemyGroup.transform, new Vector3(6f, 1.8f, 11f), Quaternion.Euler(0f, -90f, 0f), "Barracuda_Patrol_North", matBarracuda, matVision);
        }

        private static void BuildBarracudaUnit(Transform parent, Vector3 pos, Quaternion rot, string name, Material matFish, Material matVision)
        {
            GameObject barracuda = new GameObject(name);
            barracuda.transform.SetParent(parent);
            barracuda.transform.position = pos;
            barracuda.transform.rotation = rot;

            // Torpedo Fish Body
            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.name = "Predator_Body";
            body.transform.SetParent(barracuda.transform);
            body.transform.localPosition = Vector3.zero;
            body.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            body.transform.localScale = new Vector3(0.6f, 1.6f, 0.6f); // 3.2m long predator
            body.GetComponent<MeshRenderer>().sharedMaterial = matFish;

            // Head snout
            GameObject snout = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            snout.name = "Jaws_Snout";
            snout.transform.SetParent(barracuda.transform);
            snout.transform.localPosition = new Vector3(0f, 0f, 1.6f);
            snout.transform.localScale = new Vector3(0.45f, 0.4f, 0.8f);
            snout.GetComponent<MeshRenderer>().sharedMaterial = matFish;

            // Dorsal fin
            GameObject fin = GameObject.CreatePrimitive(PrimitiveType.Cube);
            fin.name = "DorsalFin";
            fin.transform.SetParent(barracuda.transform);
            fin.transform.localPosition = new Vector3(0f, 0.55f, -0.3f);
            fin.transform.localScale = new Vector3(0.08f, 0.5f, 0.7f);
            fin.GetComponent<MeshRenderer>().sharedMaterial = matFish;

            // Vision Cone Representation (Forward detection arc)
            GameObject visionCone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            visionCone.name = "VisionCone_DetectionZone";
            visionCone.transform.SetParent(barracuda.transform);
            visionCone.transform.localPosition = new Vector3(0f, 0f, 4.5f);
            visionCone.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            visionCone.transform.localScale = new Vector3(4.5f, 3.5f, 4.5f);
            visionCone.GetComponent<MeshRenderer>().sharedMaterial = matVision;
            Collider c = visionCone.GetComponent<Collider>();
            if (c != null) c.isTrigger = true;
        }

        private static void BuildPushableProps(Transform parent, Material matRock, Material matSand)
        {
            GameObject propsGroup = new GameObject("Pushable_Physics_Props");
            propsGroup.transform.SetParent(parent);

            // Pushable Distraction Boulders
            CreatePushableRock(propsGroup.transform, new Vector3(-3f, 0.6f, -16f), new Vector3(1.2f, 1.2f, 1.2f), 3.5f, matRock, "Pushable_Rock_Entrance");
            CreatePushableRock(propsGroup.transform, new Vector3(-9f, 0.75f, -5f), new Vector3(1.5f, 1.5f, 1.5f), 5f, matRock, "Pushable_Rock_WestFlank");
            CreatePushableRock(propsGroup.transform, new Vector3(4f, 0.6f, 3f), new Vector3(1.2f, 1.2f, 1.2f), 3.5f, matRock, "Pushable_Rock_CenterCorridor");
            CreatePushableRock(propsGroup.transform, new Vector3(0f, 0.9f, 17f), new Vector3(1.8f, 1.8f, 1.8f), 6f, matRock, "Pushable_Puzzle_BoulderGate");

            // Pushable Large Shell Distraction
            CreatePushableShell(propsGroup.transform, new Vector3(9f, 0.5f, -9f), matSand, "Pushable_Shell_Distraction_East");
        }

        private static void CreatePushableRock(Transform parent, Vector3 pos, Vector3 scale, float mass, Material mat, string name)
        {
            GameObject rock = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rock.name = name;
            rock.transform.SetParent(parent);
            rock.transform.position = pos;
            rock.transform.localScale = scale;
            rock.transform.rotation = Quaternion.Euler(Random.Range(5f, 25f), Random.Range(0f, 180f), Random.Range(5f, 25f));
            rock.GetComponent<MeshRenderer>().sharedMaterial = mat;

            Rigidbody rb = rock.AddComponent<Rigidbody>();
            rb.mass = mass;
            rb.linearDamping = 1.2f;
            rb.angularDamping = 1.5f;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            rock.AddComponent<Pushable>();
        }

        private static void CreatePushableShell(Transform parent, Vector3 pos, Material mat, string name)
        {
            GameObject shell = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            shell.name = name;
            shell.transform.SetParent(parent);
            shell.transform.position = pos;
            shell.transform.localScale = new Vector3(1.4f, 0.8f, 1.2f);
            shell.GetComponent<MeshRenderer>().sharedMaterial = mat;

            Rigidbody rb = shell.AddComponent<Rigidbody>();
            rb.mass = 3f;
            rb.linearDamping = 1.0f;
            rb.angularDamping = 1.0f;

            shell.AddComponent<Pushable>();
        }

        private static void BuildObjectiveAndClue(Transform parent, Material matShell, Material matClue, Material matRock)
        {
            GameObject objGroup = new GameObject("Objective_LostShell_And_Clue");
            objGroup.transform.SetParent(parent);
            objGroup.transform.position = new Vector3(0f, 0f, 25f);

            // Ancient Coral Shrine / Pedestal
            GameObject pedestal = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pedestal.name = "Altar_Pedestal";
            pedestal.transform.SetParent(objGroup.transform);
            pedestal.transform.localPosition = new Vector3(0f, 0.45f, 0f);
            pedestal.transform.localScale = new Vector3(3.2f, 0.45f, 3.2f);
            pedestal.GetComponent<MeshRenderer>().sharedMaterial = matRock;

            // The Hermit Crab's Lost Shell (Golden Target Shell)
            GameObject shellTarget = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            shellTarget.name = "HermitCrab_Shell_Target";
            shellTarget.transform.SetParent(objGroup.transform);
            shellTarget.transform.localPosition = new Vector3(0f, 1.4f, 0f);
            shellTarget.transform.localScale = new Vector3(1.3f, 1.1f, 1.5f);
            shellTarget.GetComponent<MeshRenderer>().sharedMaterial = matShell;

            // Shell coil crown
            GameObject coil = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            coil.name = "Shell_CoilCrown";
            coil.transform.SetParent(shellTarget.transform);
            coil.transform.localPosition = new Vector3(0f, 0.4f, -0.3f);
            coil.transform.localScale = new Vector3(0.6f, 0.35f, 0.6f);
            coil.transform.localRotation = Quaternion.Euler(30f, 0f, 0f);
            coil.GetComponent<MeshRenderer>().sharedMaterial = matShell;

            // Clue Monument (The Monster's Huge Eyes)
            GameObject clueMonument = new GameObject("Clue_Monument_HugeEyes");
            clueMonument.transform.SetParent(objGroup.transform);
            clueMonument.transform.localPosition = new Vector3(4f, 0f, 0f);

            GameObject tablet = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tablet.name = "Clue_Tablet_Bioluminescent";
            tablet.transform.SetParent(clueMonument.transform);
            tablet.transform.localPosition = new Vector3(0f, 1.5f, 0f);
            tablet.transform.localScale = new Vector3(1.6f, 2.8f, 0.4f);
            tablet.GetComponent<MeshRenderer>().sharedMaterial = matClue;

            // Clue Eye glyph
            GameObject eyeGlyph = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            eyeGlyph.name = "Glyph_HugeEye";
            eyeGlyph.transform.SetParent(tablet.transform);
            eyeGlyph.transform.localPosition = new Vector3(0f, 0.2f, 0.55f);
            eyeGlyph.transform.localScale = new Vector3(0.5f, 0.35f, 0.2f);
        }

        private static void BuildChoiceBranches(Transform parent, Material matClue, Material matRock)
        {
            GameObject choiceGroup = new GameObject("Choice_Fork_ExitPaths");
            choiceGroup.transform.SetParent(parent);

            // Choice A: The Selfish Path (Take Shell Shield & Leave)
            GameObject pathSelfish = new GameObject("Choice_BranchA_SelfishExit");
            pathSelfish.transform.SetParent(choiceGroup.transform);
            pathSelfish.transform.position = new Vector3(14f, 0f, 32f);

            GameObject archA = GameObject.CreatePrimitive(PrimitiveType.Cube);
            archA.name = "Arch_TakeShell_EasyExit";
            archA.transform.SetParent(pathSelfish.transform);
            archA.transform.localPosition = new Vector3(0f, 3f, 0f);
            archA.transform.localScale = new Vector3(4f, 5.5f, 1f);
            archA.GetComponent<MeshRenderer>().sharedMaterial = matRock;

            // Choice B: The Loyal Return Path (Return Shell to Hermit Crab)
            GameObject pathLoyal = new GameObject("Choice_BranchB_LoyalReturnPath");
            pathLoyal.transform.SetParent(choiceGroup.transform);
            pathLoyal.transform.position = new Vector3(-14f, 0f, 28f);

            GameObject archB = GameObject.CreatePrimitive(PrimitiveType.Cube);
            archB.name = "Arch_ReturnToHermitCrab_GainAlly";
            archB.transform.SetParent(pathLoyal.transform);
            archB.transform.localPosition = new Vector3(0f, 3f, 0f);
            archB.transform.localScale = new Vector3(4f, 5.5f, 1f);
            archB.GetComponent<MeshRenderer>().sharedMaterial = matClue;
        }

        private static GameObject CreateWall(Transform parent, Vector3 pos, Vector3 scale, Material mat, string name)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = name;
            wall.transform.SetParent(parent);
            wall.transform.position = pos;
            wall.transform.localScale = scale;
            if (mat != null) wall.GetComponent<MeshRenderer>().sharedMaterial = mat;
            return wall;
        }

        private static Material GetOrCreateMaterial(string matName, Color color, float smoothness, float metallic, bool transparent = false)
        {
            string path = $"{MaterialsFolder}/{matName}.mat";
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null)
            {
                Shader litShader = Shader.Find("Universal Render Pipeline/Lit");
                mat = new Material(litShader);
                mat.SetColor("_BaseColor", color);
                mat.SetFloat("_Smoothness", smoothness);
                mat.SetFloat("_Metallic", metallic);

                if (transparent)
                {
                    mat.SetFloat("_Surface", 1); // Transparent
                    mat.SetFloat("_Blend", 0);   // Alpha
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat.SetInt("_ZWrite", 0);
                    mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                    mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                }

                AssetDatabase.CreateAsset(mat, path);
            }
            return mat;
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

        private static void ClearChildren(GameObject parent)
        {
            int childCount = parent.transform.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                Undo.DestroyObjectImmediate(parent.transform.GetChild(i).gameObject);
            }
        }
    }
}
