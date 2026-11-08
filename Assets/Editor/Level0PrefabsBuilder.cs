using UnityEngine;
using UnityEditor;
using UponHeavyTides.Interaction;

namespace UponHeavyTides.Editor
{
    public static class Level0PrefabsBuilder
    {
        private const string PrefabFolder = "Assets/Prefabs/Level0";

        [MenuItem("Upon Heavy Tides/Build Level0 Prefabs")]
        public static void BuildPrefabs()
        {
            // Ensure folder exists
            if (!AssetDatabase.IsValidFolder(PrefabFolder))
            {
                AssetDatabase.CreateFolder("Assets/Prefabs", "Level0");
            }

            // Collectible prefab
            GameObject collectible = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            collectible.name = "CollectibleRelic";
            collectible.AddComponent<UponHeavyTides.Level0.CollectibleItem>();
            // Simple visual tweak
            var mat = new Material(Shader.Find("Standard")) { color = Color.yellow };
            collectible.GetComponent<Renderer>().sharedMaterial = mat;
            SavePrefab(collectible, "CollectibleRelic.prefab");
            GameObject.DestroyImmediate(collectible);

            // Pushable stone prefab
            GameObject stone = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stone.name = "PushableStone";
            var rb = stone.AddComponent<Rigidbody>();
            rb.mass = 2f;
            stone.AddComponent<UponHeavyTides.Interaction.Pushable>();
            SavePrefab(stone, "PushableStone.prefab");
            GameObject.DestroyImmediate(stone);

            // Crab companion prefab
            GameObject crab = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            crab.name = "CrabCompanion";
            crab.AddComponent<UponHeavyTides.Level0.DialogueTrigger>();
            SavePrefab(crab, "CrabCompanion.prefab");
            GameObject.DestroyImmediate(crab);

            // Elder turtle prefab
            GameObject turtle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            turtle.name = "ElderSeaTurtle";
            turtle.transform.localScale = Vector3.one * 2f;
            turtle.AddComponent<UponHeavyTides.Level0.DialogueTrigger>();
            SavePrefab(turtle, "ElderSeaTurtle.prefab");
            GameObject.DestroyImmediate(turtle);

            // Final boulder prefab (pushable)
            GameObject boulder = GameObject.CreatePrimitive(PrimitiveType.Cube);
            boulder.name = "FinalBoulder";
            var boulderRb = boulder.AddComponent<Rigidbody>();
            boulderRb.mass = 8f;
            boulder.AddComponent<UponHeavyTides.Interaction.Pushable>();
            SavePrefab(boulder, "FinalBoulder.prefab");
            GameObject.DestroyImmediate(boulder);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Level0 prefabs generated in " + PrefabFolder);
        }

        private static void SavePrefab(GameObject obj, string fileName)
        {
            string path = System.IO.Path.Combine(PrefabFolder, fileName);
            PrefabUtility.SaveAsPrefabAsset(obj, path);
        }
    }
}
