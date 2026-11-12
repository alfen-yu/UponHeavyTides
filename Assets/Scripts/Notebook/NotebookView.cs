using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UponHeavyTides.Notebook
{
    class NotebookView : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private TMP_Text clueListText;
        [SerializeField] private InputAction toggleAction = new InputAction("ToggleNotebook", InputActionType.Button, "<Keyboard>/tab");
        private bool isOpen;
        private void Awake() { SetOpen(false); }

        private void OnEnable()
        {
            toggleAction.Enable();
            toggleAction.performed += OnTogglePressed;
        }

        private void OnDisable()
        {
            toggleAction.performed -= OnTogglePressed;
            toggleAction.Disable();
            if (isOpen) Time.timeScale = 1f;
        }

        private void OnTogglePressed(InputAction.CallbackContext ctx) { SetOpen(!isOpen); }

        private void SetOpen(bool open)
        {
            isOpen = open;
            panel.SetActive(open);
            Time.timeScale = open ? 0f : 1f;
            if (open) Refresh();
        }

        private void Refresh()
        {
            var sb = new StringBuilder();
            if (ClueJournal.JournalInstance != null)
            {
                foreach(var clue in ClueJournal.JournalInstance.Clues)
                {
                    sb.AppendLine($"- {clue.Title}\n{clue.Description}\n");
                }
                clueListText.text = sb.Length > 0 ? sb.ToString() : "No clues yet.";
            }
        }
    }
}