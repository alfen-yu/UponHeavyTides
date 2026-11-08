using UnityEngine;

namespace UponHeavyTides.Level0
{
    /// <summary>
    /// Simple persistence for Level 0 tutorial state using PlayerPrefs.
    /// Stores collectibles count, whether the elder turtle was met, and whether the clue was discovered.
    /// </summary>
    public static class Level0Save
    {
        private const string KeyCollectibles = "Level0_Collectibles";
        private const string KeyMetTurtle = "Level0_MetTurtle";
        private const string KeyClue = "Level0_ClueDiscovered";

        public static void Save(int collected, bool metTurtle, bool clue)
        {
            PlayerPrefs.SetInt(KeyCollectibles, collected);
            PlayerPrefs.SetInt(KeyMetTurtle, metTurtle ? 1 : 0);
            PlayerPrefs.SetInt(KeyClue, clue ? 1 : 0);
            PlayerPrefs.Save();
        }

        public static void Load(out int collected, out bool metTurtle, out bool clue)
        {
            collected = PlayerPrefs.GetInt(KeyCollectibles, 0);
            metTurtle = PlayerPrefs.GetInt(KeyMetTurtle, 0) == 1;
            clue = PlayerPrefs.GetInt(KeyClue, 0) == 1;
        }
    }
}
