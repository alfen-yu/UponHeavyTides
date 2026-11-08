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

        [MenuItem("Upon Heavy Tides/Build Level 0 (The Shallows)")]
        public static void BuildLevel0()
        {
            if (EditorSceneManager.GetActiveScene().path != ScenePath)
            {
                EditorSceneManager.OpenScene(ScenePath);
            }

            Undo.IncrementCurrentGroup();
            int undoGroup = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Build Level 0 The Shallows Complete");

            // 1. Setup Prototype & Thematic Materials
            Material matSand = GetOrCreateMaterial("Mat_ShallowsSand", new Color(0.92f, 0.86f, 0.70f), 0.15f, 0.05f);
            Material matRock = GetOrCreateMaterial("Mat_ShallowsRock", new Color(0.55f, 0.52f, 0.48f), 0.25f, 0.1f);
            Material matCoralPink = GetOrCreateMaterial("Mat_CoralPink", new Color(0.95f, 0.42f, 0.58f), 0.35f, 0.1f);
            Material matCoralOrange = GetOrCreateMaterial("Mat_CoralOrange", new Color(1.0f, 0.58f, 0.22f), 0.4f, 0.1f);
            Material matCoralTeal = GetOrCreateMaterial("Mat_CoralTeal", new Color(0.22f, 0.85f, 0.75f), 0.3f, 0.15f);
            Material matCoralPurple = GetOrCreateMaterial("Mat_CoralPurple", new Color(0.72f, 0.38f, 0.88f), 0.3f, 0.1f);
            Material matSeaweed = GetOrCreateMaterial("Mat_SeaweedShallows", new Color(0.24f, 0.68f, 0.32f), 0.4f, 0.05f);
            Material matSeaFlower = GetOrCreateMaterial("Mat_SeaFlower", new Color(1.0f, 0.82f, 0.30f), 0.5f, 0.2f);
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

            // 4. Setup Hierarchy Roots
            GameObject envRoot = GetOrCreateRoot("_ENVIRONMENT");
            GameObject npcsRoot = GetOrCreateRoot("_NPCS");
            GameObject tutorialRoot = GetOrCreateRoot("_TUTORIAL_PROPS");
            GameObject relicsRoot = GetOrCreateRoot("_EXPLORATION_RELICS");
            GameObject culminationRoot = GetOrCreateRoot("_CULMINATION_DISCOVERY");
            GameObject managerRoot = GetOrCreateRoot("_GAME_MANAGERS");

            ClearChildren(envRoot);
            ClearChildren(npcsRoot);
            ClearChildren(tutorialRoot);
            ClearChildren(relicsRoot);
            ClearChildren(culminationRoot);
            ClearChildren(managerRoot);

            // 5. Game Managers (DialogueManager, Level0Controller)
            SetupManagers(managerRoot);

            // 6. Perimeter Cliffs & Natural Reef Boundary
            BuildPerimeterReefs(envRoot.transform, matRock, matCoralOrange, matCoralTeal);

            // 7. Rich Lively Shallows Decor: Corals, Seaweed, Flowers
            BuildLivelyFlora(envRoot.transform, matCoralPink, matCoralOrange, matCoralTeal, matCoralPurple, matSeaweed, matSeaFlower, matRock);

            // 8. Player Setup
            GameObject player = GameObject.Find("Player");
            if (player != null)
            {
                player.transform.position = new Vector3(0f, 1f, -32f);
                player.transform.rotation = Quaternion.identity;
                player.tag = "Player";
            }

            // 9. Camera Tracker Setup
            GameObject mainCam = GameObject.Find("Main Camera");
            if (mainCam != null && player != null)
            {
                var tracker = mainCam.GetComponent<CameraTracker>();
                if (tracker != null)
                {
                    tracker.target = player.transform;
                    tracker.offset = new Vector3(0f, 4.5f, -7.5f);
                }
                mainCam.transform.position = player.transform.position + new Vector3(0f, 4.5f, -7.5f);
            }

            // 10. Crab Companion & Intro Interaction
            GameObject crab = GameObject.Find("Crab");
            if (crab == null)
            {
                crab = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                crab.name = "Crab";
                Undo.RegisterCreatedObjectUndo(crab, "Create Crab");
            }
            crab.transform.position = new Vector3(-3.5f, 0.4f, -30f);
            crab.transform.localScale = new Vector3(1f, 0.8f, 1f);

            var companion = crab.GetComponent<Companion>();
            if (companion == null) companion = crab.AddComponent<Companion>();

            // Setup Crab Intro Dialogue
            var crabDiag = crab.GetComponent<DialogueTrigger>();
            if (crabDiag == null) crabDiag = crab.AddComponent<DialogueTrigger>();
            SetupCrabIntroDialogue(crabDiag);

            // Shelter rock for Crab
            GameObject crabRock = GameObject.CreatePrimitive(PrimitiveType.Cube);
            crabRock.name = "Crab_Shelter_Rock";
            crabRock.transform.SetParent(npcsRoot.transform);
            crabRock.transform.position = new Vector3(-2.2f, 0.9f, -30f);
            crabRock.transform.localScale = new Vector3(1.8f, 1.8f, 1.8f);
            crabRock.transform.rotation = Quaternion.Euler(15f, 25f, 10f);
            crabRock.GetComponent<MeshRenderer>().sharedMaterial = matRock;

            // 11. Elder Sea Turtle Sanctuary
            BuildElderSeaTurtle(npcsRoot.transform, matTurtleShell, matTurtleSkin, matRock, matCoralTeal);

            // 12. Tutorial Obstacles: Jump Steps & Pushable Boulders
            BuildTutorialCourse(tutorialRoot.transform, matRock, matSand);

            // 13. Exploration Relics (Sprinkled collectibles for non-linear play)
            BuildExplorationRelics(relicsRoot.transform, matRelicPearl, matSeaFlower);

            // 14. Culmination: Final Boulder & Giant Sucker Scar
            BuildFinalTrialAndSuckerScar(culminationRoot.transform, matRock, matSuckerScar, matHookBarbs);

            // Clean up old standalone Rock from initial scene
            GameObject oldRock = GameObject.Find("Rock");
            if (oldRock != null && oldRock.transform.parent == null)
            {
                Undo.DestroyObjectImmediate(oldRock);
            }

            // 15. Bake Navigation Mesh
            navSurface.BuildNavMesh();

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveOpenScenes();

            Undo.CollapseUndoOperations(undoGroup);
            Debug.Log("<color=green><b>[Upon Heavy Tides]</b> Level 0 (The Shallows) fully built and baked!</color>");
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
                    l.intensity = 1.15f;
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
            if (root.GetComponent<Level0Controller>() == null)
            {
                root.AddComponent<Level0Controller>();
            }
        }

        private static void SetupCrabIntroDialogue(DialogueTrigger trigger)
        {
            trigger.AddLine("Crab Companion", "P-psst! Over here! Behind this rock!", new Color(1f, 0.65f, 0.3f));
            trigger.AddLine("Crab Companion", "By the Great Tides... you're not from the ocean! You're the creature from above!", new Color(1f, 0.65f, 0.3f));
            trigger.AddLine("Crab Companion", "A monster from the deep has stolen our world's light. Our kingdom is drowning in fear...", new Color(1f, 0.65f, 0.3f));
            trigger.AddLine("Crab Companion", "Follow me to the Elder Sea Turtle! He has guarded the ancient prophecy for generations!", new Color(1f, 0.65f, 0.3f));
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

            // Surrounding Ancient Coral Columns
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
            }

            // Turtle Model
            GameObject turtleObj = new GameObject("Elder_Sea_Turtle");
            turtleObj.transform.SetParent(sanctuary.transform);
            turtleObj.transform.localPosition = new Vector3(0f, 0f, 0.5f);
            turtleObj.transform.localRotation = Quaternion.Euler(0f, 180f, 0f); // Looking towards arriving player

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

            // Front Left Flipper
            GameObject flLeft = GameObject.CreatePrimitive(PrimitiveType.Cube);
            flLeft.name = "Flipper_Front_Left";
            flLeft.transform.SetParent(turtleObj.transform);
            flLeft.transform.localPosition = new Vector3(-2.6f, 0.7f, 2.0f);
            flLeft.transform.localScale = new Vector3(1.8f, 0.25f, 1.1f);
            flLeft.transform.localRotation = Quaternion.Euler(0f, -30f, 15f);
            flLeft.GetComponent<MeshRenderer>().sharedMaterial = matSkin;

            // Front Right Flipper
            GameObject flRight = GameObject.CreatePrimitive(PrimitiveType.Cube);
            flRight.name = "Flipper_Front_Right";
            flRight.transform.SetParent(turtleObj.transform);
            flRight.transform.localPosition = new Vector3(2.6f, 0.7f, 2.0f);
            flRight.transform.localScale = new Vector3(1.8f, 0.25f, 1.1f);
            flRight.transform.localRotation = Quaternion.Euler(0f, 30f, -15f);
            flRight.GetComponent<MeshRenderer>().sharedMaterial = matSkin;

            // Dialogue Trigger for Turtle
            var diag = turtleObj.AddComponent<DialogueTrigger>();
            diag.AddLine("Elder Sea Turtle", "Welcome, traveler of the surface. It was foretold that a creature from above would descend when our world's light began to fade.", new Color(0.35f, 0.85f, 0.75f));
            diag.AddLine("Elder Sea Turtle", "A colossal monster haunts the deepest trenches. It drains the life from our corals and the color from our tides.", new Color(0.35f, 0.85f, 0.75f));
            diag.AddLine("Elder Sea Turtle", "I appoint this young crab to be your guide and companion. He shall teach you the way of the water—to stride across the ocean bed, leap through currents, and shift heavy stones.", new Color(0.35f, 0.85f, 0.75f));
            diag.AddLine("Crab Companion", "It is an honor, Elder! Come, traveler! Use [WASD] to walk and [SPACE] to leap over the coral ridges. Push any stones in your way!", new Color(1f, 0.65f, 0.3f));
            diag.AddLine("Elder Sea Turtle", "Seek the gateway at the end of the shallows. The monster left a mark upon the stone... uncover it!", new Color(0.35f, 0.85f, 0.75f));

            diag.onDialogueFinished = new UnityEngine.Events.UnityEvent();
            diag.onDialogueFinished.AddListener(() =>
            {
                if (Level0Controller.Instance != null)
                {
                    Level0Controller.Instance.OnElderTurtleMet();
                }
            });
        }

        private static void BuildTutorialCourse(Transform parent, Material matRock, Material matSand)
        {
            GameObject course = new GameObject("Tutorial_Movement_And_Push_Course");
            course.transform.SetParent(parent);

            // 1. Stepping stone sandbars / jump platforms
            CreateJumpPlatform(course.transform, new Vector3(0f, 0.5f, 0f), new Vector3(5f, 0.8f, 4f), matRock, "StepPlatform_1");
            CreateJumpPlatform(course.transform, new Vector3(-1.5f, 1.0f, 5f), new Vector3(4f, 1.6f, 3.5f), matRock, "StepPlatform_2");
            CreateJumpPlatform(course.transform, new Vector3(1.5f, 1.5f, 10f), new Vector3(4.5f, 2.4f, 4f), matRock, "StepPlatform_3");

            // 2. Pushable tutorial rocks along the route
            CreatePushableStone(course.transform, new Vector3(1.8f, 0.6f, -3f), new Vector3(1.2f, 1.2f, 1.2f), 3f, matRock, "Pushable_Stone_1");
            CreatePushableStone(course.transform, new Vector3(-3.0f, 0.7f, 4f), new Vector3(1.4f, 1.4f, 1.4f), 4f, matRock, "Pushable_Stone_2");
            CreatePushableStone(course.transform, new Vector3(2.5f, 0.8f, 13f), new Vector3(1.5f, 1.5f, 1.5f), 4.5f, matRock, "Pushable_Stone_3");
            CreatePushableStone(course.transform, new Vector3(-1.0f, 0.9f, 20f), new Vector3(1.8f, 1.8f, 1.8f), 5f, matRock, "Pushable_Stone_4");
        }

        private static void BuildExplorationRelics(Transform parent, Material matPearl, Material matFlower)
        {
            GameObject relicsGroup = new GameObject("Shallows_Sprinkled_Relics");
            relicsGroup.transform.SetParent(parent);

            var relicPositions = new (Vector3 pos, string name)[]
            {
                (new Vector3(-20f, 1.2f, -16f), "Sunlit Pearl of the West"),
                (new Vector3(18f, 1.2f, -14f), "Golden Conch Shell"),
                (new Vector3(-15f, 3.2f, 8f), "Iridescent Reef Crystal"),
                (new Vector3(20f, 1.2f, 15f), "Ancient Nautilus Relic"),
                (new Vector3(-10f, 1.5f, 24f), "Prismatic Sea Star")
            };

            foreach (var r in relicPositions)
            {
                GameObject relic = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                relic.name = r.name;
                relic.transform.SetParent(relicsGroup.transform);
                relic.transform.position = r.pos;
                relic.transform.localScale = new Vector3(0.85f, 0.85f, 0.85f);
                relic.GetComponent<MeshRenderer>().sharedMaterial = matPearl;
                relic.GetComponent<Collider>().isTrigger = true;

                // Add CollectibleItem script
                var item = relic.AddComponent<CollectibleItem>();

                // Pedestal / coral mound underneath
                GameObject ped = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                ped.name = $"{r.name}_Pedestal";
                ped.transform.SetParent(relic.transform);
                ped.transform.localPosition = new Vector3(0f, -0.7f, 0f);
                ped.transform.localScale = new Vector3(1.2f, 0.3f, 1.2f);
                ped.GetComponent<MeshRenderer>().sharedMaterial = matFlower;
            }
        }

        private static void BuildFinalTrialAndSuckerScar(Transform parent, Material matRock, Material matScar, Material matHooks)
        {
            GameObject gateGroup = new GameObject("Final_Trial_Gateway");
            gateGroup.transform.SetParent(parent);
            gateGroup.transform.position = new Vector3(0f, 0f, 28f);

            // Gateway Cliffs
            GameObject leftPillar = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leftPillar.name = "Gateway_Left_Cliff";
            leftPillar.transform.SetParent(gateGroup.transform);
            leftPillar.transform.localPosition = new Vector3(-4.5f, 4f, 0f);
            leftPillar.transform.localScale = new Vector3(4f, 8f, 3f);
            leftPillar.GetComponent<MeshRenderer>().sharedMaterial = matRock;

            GameObject rightPillar = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rightPillar.name = "Gateway_Right_Cliff";
            rightPillar.transform.SetParent(gateGroup.transform);
            rightPillar.transform.localPosition = new Vector3(4.5f, 4f, 0f);
            rightPillar.transform.localScale = new Vector3(4f, 8f, 3f);
            rightPillar.GetComponent<MeshRenderer>().sharedMaterial = matRock;

            // Gateway Arch
            GameObject arch = GameObject.CreatePrimitive(PrimitiveType.Cube);
            arch.name = "Gateway_Lintel";
            arch.transform.SetParent(gateGroup.transform);
            arch.transform.localPosition = new Vector3(0f, 7.5f, 0f);
            arch.transform.localScale = new Vector3(13f, 2f, 3.5f);
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
            suckerDisc.transform.localScale = new Vector3(0.55f, 0.05f, 0.55f); // ~3.5m in world scale
            suckerDisc.GetComponent<MeshRenderer>().sharedMaterial = matScar;

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
            }

            // The Final Heavy Boulder (Blocking the scar)
            GameObject finalBoulder = GameObject.CreatePrimitive(PrimitiveType.Cube);
            finalBoulder.name = "Final_Trial_Boulder";
            finalBoulder.transform.SetParent(gateGroup.transform);
            finalBoulder.transform.localPosition = new Vector3(0f, 1.4f, 1.5f);
            finalBoulder.transform.localScale = new Vector3(2.8f, 2.8f, 2.8f);
            finalBoulder.transform.rotation = Quaternion.Euler(10f, 20f, 15f);
            finalBoulder.GetComponent<MeshRenderer>().sharedMaterial = matRock;

            Rigidbody rb = finalBoulder.AddComponent<Rigidbody>();
            rb.mass = 8f;
            rb.linearDamping = 1.4f;
            rb.angularDamping = 1.5f;
            finalBoulder.AddComponent<Pushable>();

            // Trigger on the scar wall for discovery
            var triggerCollider = scarWall.AddComponent<BoxCollider>();
            triggerCollider.isTrigger = true;
            triggerCollider.size = new Vector3(1.2f, 1.2f, 4.0f); // Expands in front

            var clueComp = scarWall.AddComponent<ClueDiscoveryTrigger>();
            // Wire blocking boulder via Reflection or public field
            var field = typeof(ClueDiscoveryTrigger).GetField("blockingBoulder", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null) field.SetValue(clueComp, finalBoulder.transform);
        }

        private static void BuildPerimeterReefs(Transform parent, Material matRock, Material matCoralO, Material matCoralT)
        {
            GameObject cliffs = new GameObject("Perimeter_Reef_Cliffs");
            cliffs.transform.SetParent(parent);

            float halfSize = 38f;
            float height = 12f;
            float thickness = 4f;

            // South Wall (Behind spawn)
            CreateReefWall(cliffs.transform, new Vector3(0f, height / 2, -halfSize), new Vector3(76f, height, thickness), matRock);
            // West Wall
            CreateReefWall(cliffs.transform, new Vector3(-halfSize, height / 2, 0f), new Vector3(thickness, height, 76f), matRock);
            // East Wall
            CreateReefWall(cliffs.transform, new Vector3(halfSize, height / 2, 0f), new Vector3(thickness, height, 76f), matRock);
            // North Wall (Flanks the exit gateway)
            CreateReefWall(cliffs.transform, new Vector3(-20f, height / 2, halfSize), new Vector3(36f, height, thickness), matRock);
            CreateReefWall(cliffs.transform, new Vector3(20f, height / 2, halfSize), new Vector3(36f, height, thickness), matRock);
        }

        private static void BuildLivelyFlora(Transform parent, Material matPink, Material matOrange, Material matTeal, Material matPurple, Material matSeaweed, Material matFlower, Material matRock)
        {
            GameObject floraRoot = new GameObject("Shallows_Flora_And_Corals");
            floraRoot.transform.SetParent(parent);

            // 1. Coral Garden Formations (8 distinct coral mounds across the map)
            var coralCenters = new Vector3[]
            {
                new Vector3(-12f, 0f, -22f),
                new Vector3(14f, 0f, -24f),
                new Vector3(-18f, 0f, -6f),
                new Vector3(16f, 0f, -4f),
                new Vector3(-12f, 0f, 12f),
                new Vector3(15f, 0f, 10f),
                new Vector3(-8f, 0f, 24f),
                new Vector3(10f, 0f, 22f)
            };

            Material[] coralMats = new Material[] { matPink, matOrange, matTeal, matPurple };

            for (int i = 0; i < coralCenters.Length; i++)
            {
                GameObject cluster = new GameObject($"CoralGarden_{i}");
                cluster.transform.SetParent(floraRoot.transform);
                cluster.transform.position = coralCenters[i];

                // Rock base
                GameObject mound = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                mound.name = "Reef_Base";
                mound.transform.SetParent(cluster.transform);
                mound.transform.localPosition = new Vector3(0f, 0.4f, 0f);
                mound.transform.localScale = new Vector3(3.5f, 1.2f, 3.5f);
                mound.GetComponent<MeshRenderer>().sharedMaterial = matRock;

                // Branching corals & Pipe corals
                for (int c = 0; c < 7; c++)
                {
                    float ang = (c / 7f) * Mathf.PI * 2f;
                    Vector3 coralPos = new Vector3(Mathf.Cos(ang) * 1.3f, 1.2f, Mathf.Sin(ang) * 1.3f);

                    GameObject coralBranch = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    coralBranch.name = $"Coral_Branch_{c}";
                    coralBranch.transform.SetParent(cluster.transform);
                    coralBranch.transform.localPosition = coralPos;

                    float h = 1.2f + (c % 3) * 0.5f;
                    coralBranch.transform.localScale = new Vector3(0.35f, h, 0.35f);
                    coralBranch.transform.localRotation = Quaternion.Euler(Mathf.Sin(c) * 15f, c * 40f, Mathf.Cos(c) * 15f);
                    coralBranch.GetComponent<MeshRenderer>().sharedMaterial = coralMats[(i + c) % coralMats.Length];
                }

                // Table coral canopy
                GameObject table = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                table.name = "Table_Coral_Plate";
                table.transform.SetParent(cluster.transform);
                table.transform.localPosition = new Vector3(0f, 2.2f, 0f);
                table.transform.localScale = new Vector3(2.2f, 0.12f, 2.2f);
                table.GetComponent<MeshRenderer>().sharedMaterial = coralMats[i % coralMats.Length];
            }

            // 2. Swaying Seaweed & Sea Anemone Flower Beds
            for (int s = 0; s < 12; s++)
            {
                float x = Mathf.Sin(s * 1.7f) * 26f;
                float z = Mathf.Cos(s * 1.3f) * 26f;
                Vector3 bedPos = new Vector3(x, 0f, z);

                GameObject bed = new GameObject($"SeaweedBed_{s}");
                bed.transform.SetParent(floraRoot.transform);
                bed.transform.position = bedPos;

                // Seaweed stalks
                for (int w = 0; w < 5; w++)
                {
                    float ang = (w / 5f) * Mathf.PI * 2f;
                    Vector3 weedPos = new Vector3(Mathf.Cos(ang) * 0.8f, 1.8f, Mathf.Sin(ang) * 0.8f);

                    GameObject stalk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    stalk.name = $"WeedStalk_{w}";
                    stalk.transform.SetParent(bed.transform);
                    stalk.transform.localPosition = weedPos;
                    stalk.transform.localScale = new Vector3(0.18f, 2.2f, 0.18f);
                    stalk.transform.localRotation = Quaternion.Euler(Mathf.Sin(w) * 10f, w * 45f, 0f);
                    stalk.GetComponent<MeshRenderer>().sharedMaterial = matSeaweed;
                }

                // Sea Flower (Anemone)
                GameObject flower = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                flower.name = "Sea_Anemone_Flower";
                flower.transform.SetParent(bed.transform);
                flower.transform.localPosition = new Vector3(0f, 0.4f, 0f);
                flower.transform.localScale = new Vector3(0.9f, 0.5f, 0.9f);
                flower.GetComponent<MeshRenderer>().sharedMaterial = matFlower;
            }
        }

        private static void CreateReefWall(Transform parent, Vector3 pos, Vector3 scale, Material mat)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = "Cliff_Reef_Wall";
            wall.transform.SetParent(parent);
            wall.transform.position = pos;
            wall.transform.localScale = scale;
            if (mat != null) wall.GetComponent<MeshRenderer>().sharedMaterial = mat;
        }

        private static void CreateJumpPlatform(Transform parent, Vector3 pos, Vector3 scale, Material mat, string name)
        {
            GameObject plat = GameObject.CreatePrimitive(PrimitiveType.Cube);
            plat.name = name;
            plat.transform.SetParent(parent);
            plat.transform.position = pos;
            plat.transform.localScale = scale;
            plat.GetComponent<MeshRenderer>().sharedMaterial = mat;
        }

        private static void CreatePushableStone(Transform parent, Vector3 pos, Vector3 scale, float mass, Material mat, string name)
        {
            GameObject stone = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stone.name = name;
            stone.transform.SetParent(parent);
            stone.transform.position = pos;
            stone.transform.localScale = scale;
            stone.transform.rotation = Quaternion.Euler(Random.Range(5f, 20f), Random.Range(0f, 180f), Random.Range(5f, 20f));
            stone.GetComponent<MeshRenderer>().sharedMaterial = mat;

            Rigidbody rb = stone.AddComponent<Rigidbody>();
            rb.mass = mass;
            rb.linearDamping = 1.2f;
            rb.angularDamping = 1.4f;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            stone.AddComponent<Pushable>();
        }

        private static Material GetOrCreateMaterial(string matName, Color color, float smoothness, float metallic)
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
