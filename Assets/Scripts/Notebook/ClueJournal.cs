using System;
using UnityEngine;
using UponHeavyTides.Core;
using System.Collections.Generic;

namespace UponHeavyTides.Notebook
{
    public class ClueJournal : MonoBehaviour
    {
        // PublicMethods

        // singleton implemented for clue journal, there can be only one journal in the entire game 
        public static ClueJournal JournalInstance { get; private set; }

        [SerializeField] private Clue debugClue;

        [SerializeField] private Clue[] debugClues;


        public IReadOnlyList<Clue> Clues => clues;

        public event Action<Clue> ClueAdded;

        // Private Methods
        private readonly List<Clue> clues = new List<Clue>();
        private void Awake()
        {
            if (JournalInstance != null && JournalInstance != this)
            {
                // if an extra instance is created for the notebook, destroy it immediately
                Destroy(gameObject);
                return;
            }

            JournalInstance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            GameEvents.ClueCollected += AddClue;
        }

        private void OnDisable()
        {
            GameEvents.ClueCollected -= AddClue;
        }

        // if the clue exists then return otherwise add it to our notebook 
        private void AddClue(Clue clue)
        {
            if (clue == null || Contains(clue)) return;
            clues.Add(clue);
            ClueAdded?.Invoke(clue);
            Debug.Log($"Clue added: {clue.Title} (total {clues.Count})");
        }

        private bool Contains(Clue clue)
        {
            return clues.Exists(c => c.Id ==  clue.Id);
        }

        [ContextMenu("Debug: Collect Clue")]
        private void DebugCollectClue()
        {
            GameEvents.RaiseClueCollected(debugClue);
        }

        [ContextMenu("Debug: Collect All Clues")]
        private void DebugCollectAll()
        {
            foreach (var c in debugClues) GameEvents.RaiseClueCollected(c);
        }

    }
}