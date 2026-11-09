using UnityEngine; 

namespace UponHeavyTides.Notebook
{
    [CreateAssetMenu(fileName = "NewClue", menuName = "Clues")]

    public class Clue : ScriptableObject
    {
        [SerializeField] private string clueId;
        [SerializeField] private string clueTitle;
        [SerializeField, TextArea(3, 8)] private string clueDescription;

        public string Id => clueId;
        public string Title => clueTitle;
        public string Description => clueDescription;
    }
}