using System;
using UponHeavyTides.Notebook; 

namespace UponHeavyTides.Core
{
    public static class GameEvents
    {
        public static event Action<Clue> ClueCollected;
        public static void RaiseClueCollected(Clue clue)
        {
            ClueCollected?.Invoke(clue);
        }
    }
}   