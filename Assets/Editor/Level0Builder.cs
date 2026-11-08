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
            companion.SetWaiting(true);

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

            // 12. School of Fish NPC Group
            BuildSchoolOfFish(npcsRoot.transform, matRelicPearl, matCoralTeal);

            // 13. Tutorial Obstacles: Jump Steps & Pushable Boulders
            BuildTutorialCourse(tutorialRoot.transform, matRock, matSand);

            // 14. Exploration Relics (Sprinkled collectibles for exploration)
            BuildExplorationRelics(relicsRoot.transform, matRelicPearl, matSeaFlower);

            // 15. Culmination: Final Boulder & Giant Sucker Scar
            BuildFinalTrialAndSuckerScar(culminationRoot.transform, matRock, matSuckerScar, matHookBarbs);

            // Clean up old standalone Rock from initial scene
            GameObject oldRock = GameObject.Find("Rock");
            if (oldRock != null && oldRock.transform.parent == null)
            {
                Undo.DestroyObjectImmediate(oldRock);
            }

            // 16. Bake Navigation Mesh
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
            diag.AddLine("Crab Companion", "It is an honor, Elder! Listen closely, traveler: Press [SPACE] to jump over coral ridges. Press [A] to turn the camera right, and [D] to turn the camera left. Move in any direction using the [ARROW KEYS]: Up Arrow for forward, Down Arrow for backward, and Left/Right Arrows to steer.", new Color(1f, 0.65f, 0.3f));
            diag.AddLine("Elder Sea Turtle", "Seek the gateway at the end of the shallows. Explore the reefs, meet the school of fish, and uncover the clue hidden beneath the great stone.", new Color(0.35f, 0.85f, 0.75f));
        }

        private static void BuildSchoolOfFish(Transform parent, Material matFish, Material matGlow)
        {
            GameObject schoolGroup = new GameObject("School_Of_Fish_NPC");
            schoolGroup.transform.SetParent(parent);
            schoolGroup.transform.position = new Vector3(12f, 1.8f, 6f);
            schoolGroup.AddComponent<FishSchoolAnimator>();

            // 7 colorful fish swimming in gentle formation
            Vector3[] offsets = new Vector3[]
            {
                new Vector3(0f, 0f, 0f),
                new Vector3(-1.2f, 0.3f, -0.8f),
                new Vector3(1.2f, -0.2f, -0.8f),
                new Vector3(-2.0f, -0.3f, -1.8f),
                new Vector3(2.0f, 0.4f, -1.8f),
                new Vector3(-0.6f, 0.6f, -1.4f),
                new Vector3(0.6f, -0.5f, -1.4f)
            };

            for (int i = 0; i < offsets.Length; i++)
            {
                GameObject fish = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                fish.name = $"FishMember_{i}";
                fish.transform.SetParent(schoolGroup.transform);
                fish.transform.localPosition = offsets[i];
                fish.transform.localRotation = Quaternion.Euler(0f, 25f, 90f);
                fish.transform.localScale = new Vector3(0.2f, 0.5f, 0.2f);
                fish.GetComponent<MeshRenderer>().sharedMaterial = (i % 2 == 0) ? matFish : matGlow;
            }

            // Proximity interaction trigger
            var col = schoolGroup.AddComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = 4.5f;

            var diag = schoolGroup.AddComponent<DialogueTrigger>();
            diag.ClearLines();
            diag.triggerOnlyOnce = true;
            diag.triggerOnEnter = false;
            diag.interactRadius = 5.0f;
            diag.AddLine("School of Fish", "Greetings, surface friend! The currents whispered of your arrival.", new Color(0.4f, 0.85f, 1f));
            diag.AddLine("School of Fish", "The waters ahead grow murky and perilous, but we promise to help you in whatever way we can on your voyage!", new Color(0.4f, 0.85f, 1f));
            diag.AddLine("School of Fish", "Beware the giant boulder blocking the trench gateway ahead—it conceals an ancient mark of the beast.", new Color(0.4f, 0.85f, 1f));
        }

        private static void BuildTutorialCourse(Transform parent, Material matRock, Material matSand)
        {
            GameObject course = new GameObject("Tutorial_Movement_And_Push_Course");
            course.transform.SetParent(parent);

            // Stepping stone sandbars / jump platforms
            CreateJumpPlatform(course.transform, new Vector3(0f, 0.5f, 0f), new Vector3(5f, 0.8f, 4f), matRock, "StepPlatform_1");
            CreateJumpPlatform(course.transform, new Vector3(-1.5f, 1.0f, 5f), new Vector3(4f, 1.6f, 3.5f), matRock, "StepPlatform_2");
            CreateJumpPlatform(course.transform, new Vector3(1.5f, 1.5f, 10f), new Vector3(4.5f, 2.2f, 4f), matRock, "StepPlatform_3");

            // Tutorial Pushable Stones (Teaching pushing mechanic)
            CreatePushableStone(course.transform, new Vector3(-2.5f, 0.6f, 15f), new Vector3(1.2f, 1.2f, 1.2f), 2.5f, matRock, "Pushable_Stone_A");
            CreatePushableStone(course.transform, new Vector3(2.5f, 0.7f, 19f), new Vector3(1.4f, 1.4f, 1.4f), 3.0f, matRock, "Pushable_Stone_B");
        }

        private static void CreateJumpPlatform(Transform parent, Vector3 pos, Vector3 scale, Material mat, string name)
        {
            GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
            platform.name = name;
            platform.transform.SetParent(parent);
            platform.transform.position = pos;
            platform.transform.localScale = scale;
            platform.GetComponent<MeshRenderer>().sharedMaterial = mat;
        }

        private static void CreatePushableStone(Transform parent, Vector3 pos, Vector3 scale, float mass, Material mat, string name)
        {
            GameObject stone = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stone.name = name;
            stone.transform.SetParent(parent);
            stone.transform.position = pos;
            stone.transform.localScale = scale;
            stone.GetComponent<MeshRenderer>().sharedMaterial = mat;

            Rigidbody rb = stone.AddComponent<Rigidbody>();
            rb.mass = mass;
            rb.linearDamping = 1.0f;
            rb.angularDamping = 1.5f;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            stone.AddComponent<Pushable>();
        }

        private static void BuildExplorationRelics(Transform parent, Material matPearl, Material matFlower)
        {
            Vector3[] relicSpots = new Vector3[]
            {
                new Vector3(-16f, 0.8f, -22f),
                new Vector3(16f, 0.8f, -18f),
                new Vector3(-20f, 0.8f, 2f),
                new Vector3(20f, 0.8f, 8f),
                new Vector3(-14f, 0.8f, 22f)
            };

            for (int i = 0; i < relicSpots.Length; i++)
            {
                GameObject relic = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                relic.name = $"Relic_Pearl_{i + 1}";
                relic.transform.SetParent(parent);
                relic.transform.position = relicSpots[i];
                relic.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
                relic.GetComponent<MeshRenderer>().sharedMaterial = matPearl;
                relic.GetComponent<Collider>().isTrigger = true;

                var comp = relic.AddComponent<CollectibleItem>();
                comp.itemIdentifier = $"Relic_{i + 1}";
                comp.itemName = "Sunlit Pearl";

                // Visual coral base pedestal
                GameObject ped = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                ped.name = "Pedestal";
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
            suckerDisc.transform.localScale = new Vector3(0.55f, 0.05f, 0.55f);
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
            finalBoulder.name = "FinalBoulder";
            finalBoulder.transform.SetParent(gateGroup.transform);
            finalBoulder.transform.localPosition = new Vector3(0f, 1.4f, 1.2f);
            finalBoulder.transform.localScale = new Vector3(2.8f, 2.8f, 2.8f);
            finalBoulder.transform.rotation = Quaternion.Euler(0f, 15f, 0f);
            finalBoulder.GetComponent<MeshRenderer>().sharedMaterial = matRock;

            Rigidbody rb = finalBoulder.AddComponent<Rigidbody>();
            rb.mass = 4.5f;
            rb.linearDamping = 1.0f;
            rb.angularDamping = 1.5f;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            finalBoulder.AddComponent<Pushable>();

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

        private static void BuildPerimeterReefs(Transform parent, Material matRock, Material matCoralO, Material matCoralT)
        {
            GameObject cliffs = new GameObject("Perimeter_Reef_Cliffs");
            cliffs.transform.SetParent(parent);

            float halfSize = 38f;
            float height = 12f;
            float thickness = 4f;

            CreateReefWall(cliffs.transform, new Vector3(0f, height / 2, -halfSize), new Vector3(76f, height, thickness), matRock);
            CreateReefWall(cliffs.transform, new Vector3(-halfSize, height / 2, 0f), new Vector3(thickness, height, 76f), matRock);
            CreateReefWall(cliffs.transform, new Vector3(halfSize, height / 2, 0f), new Vector3(thickness, height, 76f), matRock);
            CreateReefWall(cliffs.transform, new Vector3(-20f, height / 2, halfSize), new Vector3(36f, height, thickness), matRock);
            CreateReefWall(cliffs.transform, new Vector3(20f, height / 2, halfSize), new Vector3(36f, height, thickness), matRock);
        }

        private static void CreateReefWall(Transform parent, Vector3 pos, Vector3 scale, Material mat)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.transform.SetParent(parent);
            wall.transform.position = pos;
            wall.transform.localScale = scale;
            wall.GetComponent<MeshRenderer>().sharedMaterial = mat;
        }

        private static void BuildLivelyFlora(Transform parent, Material matCoralPink, Material matCoralOrange, Material matCoralTeal, Material matCoralPurple, Material matSeaweed, Material matSeaFlower, Material matRock)
        {
            GameObject floraRoot = new GameObject("Lively_Reef_Flora");
            floraRoot.transform.SetParent(parent);

            Material[] coralMats = new Material[] { matCoralPink, matCoralOrange, matCoralTeal, matCoralPurple };

            // Rich Coral mounds
            for (int i = 0; i < 24; i++)
            {
                float x = ((i * 17) % 56) - 28f;
                float z = ((i * 23) % 56) - 28f;
                if (Mathf.Abs(x) < 4f && Mathf.Abs(z + 10f) < 8f) continue; // Clear path around sanctuary

                GameObject coral = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                coral.name = $"Coral_Mound_{i}";
                coral.transform.SetParent(floraRoot.transform);
                coral.transform.position = new Vector3(x, 0.4f, z);
                coral.transform.localScale = new Vector3(Random.Range(1.5f, 3.2f), Random.Range(1.0f, 2.5f), Random.Range(1.5f, 3.2f));
                coral.GetComponent<MeshRenderer>().sharedMaterial = coralMats[i % coralMats.Length];
            }

            // Swaying Seaweed Stalks
            for (int s = 0; s < 36; s++)
            {
                float x = ((s * 13) % 64) - 32f;
                float z = ((s * 19) % 64) - 32f;

                GameObject weed = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                weed.name = $"Seaweed_{s}";
                weed.transform.SetParent(floraRoot.transform);
                float h = Random.Range(2.5f, 4.5f);
                weed.transform.position = new Vector3(x, h / 2f, z);
                weed.transform.localScale = new Vector3(0.2f, h / 2f, 0.2f);
                weed.transform.rotation = Quaternion.Euler(Random.Range(-8f, 8f), Random.Range(0f, 360f), Random.Range(-8f, 8f));
                weed.GetComponent<MeshRenderer>().sharedMaterial = matSeaweed;
            }

            // Vibrant Sea Flowers
            for (int f = 0; f < 18; f++)
            {
                float x = ((f * 11) % 50) - 25f;
                float z = ((f * 29) % 50) - 25f;

                GameObject flower = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                flower.name = $"SeaFlower_{f}";
                flower.transform.SetParent(floraRoot.transform);
                flower.transform.position = new Vector3(x, 0.35f, z);
                flower.transform.localScale = new Vector3(0.7f, 0.5f, 0.7f);
                flower.GetComponent<MeshRenderer>().sharedMaterial = matSeaFlower;
            }
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
