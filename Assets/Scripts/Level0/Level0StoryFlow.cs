using System.Collections.Generic;
using UnityEngine;
using UponHeavyTides.AI;

namespace UponHeavyTides.Level0
{
    public class Level0StoryFlow : MonoBehaviour
    {
        public static Level0StoryFlow Instance { get; private set; }

        public enum StoryStage
        {
            FindCrab,              // Stage 0: Crab sheltering behind rock
            LeadingToTurtle,       // Stage 1: Crab leading player to Elder Turtle
            TalkToTurtle,          // Stage 2: Arrived at Sanctuary, speak with Elder Turtle
            ExplorationTutorial,   // Stage 3: Controls taught, crab follows player, explore reefs & meet fish
            ShiftBoulderToGate,    // Stage 4: Found fish, reach boulder & push it
            Completed              // Stage 5: Clue uncovered, tutorial finished
        }

        [Header("Current Progress")]
        [SerializeField] private StoryStage currentStage = StoryStage.FindCrab;

        [Header("Scene References")]
        [SerializeField] private Companion crabCompanion;
        [SerializeField] private DialogueTrigger crabDialogue;
        [SerializeField] private DialogueTrigger turtleDialogue;
        [SerializeField] private DialogueTrigger fishDialogue;

        public StoryStage CurrentStage => currentStage;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            FindSceneReferences();
            ConfigureStoryTriggers();

            if (currentStage == StoryStage.FindCrab)
            {
                if (Level0Controller.Instance != null)
                {
                    Level0Controller.Instance.UpdateObjective("Awakening: Talk to the nervous crab sheltering behind the rock.");
                }
                if (crabCompanion != null)
                {
                    crabCompanion.SetWaiting(true);
                }
            }
        }

        public void FindSceneReferences()
        {
            if (crabCompanion == null)
            {
                var crabObj = GameObject.Find("Crab");
                if (crabObj != null)
                {
                    crabCompanion = crabObj.GetComponent<Companion>();
                    crabDialogue = crabObj.GetComponent<DialogueTrigger>();
                }
            }

            if (turtleDialogue == null)
            {
                var turtleObj = GameObject.Find("Elder_Sea_Turtle");
                if (turtleObj != null)
                {
                    turtleDialogue = turtleObj.GetComponent<DialogueTrigger>();
                }
            }

            if (fishDialogue == null)
            {
                var fishObj = GameObject.Find("School_Of_Fish_NPC");
                if (fishObj != null)
                {
                    fishDialogue = fishObj.GetComponent<DialogueTrigger>();
                }
            }
        }

        public void ConfigureStoryTriggers()
        {
            // 1. Crab Intro Dialogue
            if (crabDialogue != null)
            {
                crabDialogue.triggerOnlyOnce = true;
                crabDialogue.triggerOnEnter = false;
                crabDialogue.interactRadius = 4.5f;
                crabDialogue.ClearLines();
                crabDialogue.AddLine("Crab Companion", "P-psst! Over here! Behind this rock!", new Color(1f, 0.65f, 0.3f));
                crabDialogue.AddLine("Crab Companion", "By the Great Tides... you're not from the ocean! You're the creature from above!", new Color(1f, 0.65f, 0.3f));
                crabDialogue.AddLine("Crab Companion", "A monster from the deep has stolen our world's light. Our kingdom is drowning in fear...", new Color(1f, 0.65f, 0.3f));
                crabDialogue.AddLine("Crab Companion", "Follow me! Let's go to the Elder Sea Turtle at the ancient sanctuary!", new Color(1f, 0.65f, 0.3f));

                crabDialogue.onDialogueFinished.RemoveAllListeners();
                crabDialogue.onDialogueFinished.AddListener(OnCrabIntroFinished);
            }

            // 2. Elder Sea Turtle Sanctuary Dialogue & Control Tutorial
            if (turtleDialogue != null)
            {
                turtleDialogue.triggerOnlyOnce = true;
                turtleDialogue.triggerOnEnter = false;
                turtleDialogue.interactRadius = 6.0f;
                turtleDialogue.ClearLines();
                turtleDialogue.AddLine("Elder Sea Turtle", "Welcome, traveler of the surface. It was foretold that a creature from above would descend when our world's light began to fade.", new Color(0.35f, 0.85f, 0.75f));
                turtleDialogue.AddLine("Elder Sea Turtle", "A colossal monster haunts the deepest trenches. It drains the life from our corals and the color from our tides.", new Color(0.35f, 0.85f, 0.75f));
                turtleDialogue.AddLine("Elder Sea Turtle", "I appoint this young crab to be your guide on your journey. He shall teach you the way of the water.", new Color(0.35f, 0.85f, 0.75f));
                turtleDialogue.AddLine("Crab Companion", "It is an honor, Elder! Listen closely, traveler: Press [SPACE] to jump over coral ridges. Press [A] to turn the camera right, and [D] to turn the camera left. Move in any direction using the [ARROW KEYS]: Up Arrow for forward, Down Arrow for backward, and Left/Right Arrows to steer.", new Color(1f, 0.65f, 0.3f));
                turtleDialogue.AddLine("Elder Sea Turtle", "Seek the gateway at the end of the shallows. Explore the reefs, meet the school of fish, and uncover the clue hidden beneath the great stone.", new Color(0.35f, 0.85f, 0.75f));

                turtleDialogue.onDialogueFinished.RemoveAllListeners();
                turtleDialogue.onDialogueFinished.AddListener(OnTurtleDialogueFinished);
            }

            // 3. School of Fish Encounter Dialogue
            if (fishDialogue != null)
            {
                fishDialogue.triggerOnlyOnce = true;
                fishDialogue.triggerOnEnter = false;
                fishDialogue.interactRadius = 5.0f;
                fishDialogue.ClearLines();
                fishDialogue.AddLine("School of Fish", "Greetings, surface friend! The currents whispered of your arrival.", new Color(0.4f, 0.85f, 1f));
                fishDialogue.AddLine("School of Fish", "The waters ahead grow murky and perilous, but we promise to help you in whatever way we can on your voyage!", new Color(0.4f, 0.85f, 1f));
                fishDialogue.AddLine("School of Fish", "Beware the giant boulder blocking the trench gateway ahead—it conceals an ancient mark of the beast.", new Color(0.4f, 0.85f, 1f));

                fishDialogue.onDialogueFinished.RemoveAllListeners();
                fishDialogue.onDialogueFinished.AddListener(OnFishDialogueFinished);
            }
        }

        public void OnCrabIntroFinished()
        {
            currentStage = StoryStage.LeadingToTurtle;
            if (crabDialogue != null) crabDialogue.enabled = false;

            if (crabCompanion != null)
            {
                crabCompanion.LeadTowards(new Vector3(0f, 0.4f, -7.5f), () =>
                {
                    currentStage = StoryStage.TalkToTurtle;
                    if (Level0Controller.Instance != null)
                    {
                        Level0Controller.Instance.UpdateObjective("Speak with the Elder Sea Turtle at the sanctuary.");
                    }
                });
            }

            if (Level0Controller.Instance != null)
            {
                Level0Controller.Instance.UpdateObjective("Follow the Crab Companion to the Elder Sea Turtle sanctuary.");
            }
        }

        public void OnTurtleDialogueFinished()
        {
            currentStage = StoryStage.ExplorationTutorial;
            if (turtleDialogue != null) turtleDialogue.enabled = false;

            if (crabCompanion != null)
            {
                crabCompanion.FollowPlayer();
            }

            if (Level0Controller.Instance != null)
            {
                Level0Controller.Instance.OnElderTurtleMet();
                Level0Controller.Instance.UpdateObjective("The Way of the Water: Practice moving (Arrows / WASD), jumping (Space), and camera rotation (A/D). Explore the shallows!");
            }
        }

        public void OnFishDialogueFinished()
        {
            currentStage = StoryStage.ShiftBoulderToGate;
            if (fishDialogue != null) fishDialogue.enabled = false;

            if (Level0Controller.Instance != null)
            {
                Level0Controller.Instance.UpdateObjective("Investigate the trench gateway and push the heavy boulder aside to reveal the clue.");
            }
        }

        public void OnClueRevealed()
        {
            currentStage = StoryStage.Completed;
        }
    }
}
