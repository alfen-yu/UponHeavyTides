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
            FindCrab,              // Crab sheltering behind rock
            LeadingToTurtle,       // Crab leading player to Elder Turtle
            TalkToTurtle,          // Arrived at Sanctuary, speak with Elder Turtle
            ExplorationTutorial,   // Controls taught, crab follows player, explore reefs
            ShiftBoulderToGate,    // Fish spoke about the boulder, head to gate and push it
            Completed              // Clue uncovered, tutorial finished
        }

        [Header("Current Progress")]
        [SerializeField] private StoryStage currentStage = StoryStage.FindCrab;

        [Header("Scene References")]
        [SerializeField] private Companion crabCompanion;
        [SerializeField] private DialogueTrigger crabDialogue;
        [SerializeField] private DialogueTrigger turtleDialogue;
        [SerializeField] private DialogueTrigger fishDialogue;
        [SerializeField] private Transform playerTransform;

        private bool hasTriggeredPearlPrompt = false;
        private bool hasTriggeredFishDialogue = false;
        private List<Transform> pearlTransforms = new List<Transform>();

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
            if (playerTransform == null)
            {
                var p = GameObject.FindGameObjectWithTag("Player");
                if (p != null) playerTransform = p.transform;
            }

            if (crabCompanion == null || crabDialogue == null)
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

            // Find all pearls
            pearlTransforms.Clear();
            var relicsRoot = GameObject.Find("_EXPLORATION_RELICS");
            if (relicsRoot != null)
            {
                foreach (Transform child in relicsRoot.transform)
                {
                    if (child.GetComponent<CollectibleItem>() != null)
                    {
                        pearlTransforms.Add(child);
                    }
                }
            }
        }

        public void ConfigureStoryTriggers()
        {
            // Crab Introduction Dialogue
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

                crabDialogue.onFinishedCallback = OnCrabIntroFinished;
                crabDialogue.onDialogueFinished.RemoveAllListeners();
                crabDialogue.onDialogueFinished.AddListener(OnCrabIntroFinished);
            }

            // Elder Sea Turtle Sanctuary Dialogue & Moves Tutorial
            if (turtleDialogue != null)
            {
                turtleDialogue.triggerOnlyOnce = true;
                turtleDialogue.triggerOnEnter = false;
                turtleDialogue.interactRadius = 6.0f;
                turtleDialogue.ClearLines();
                turtleDialogue.AddLine("Elder Sea Turtle", "Welcome, traveler of the surface. It was foretold that a creature from above would descend when our world's light began to fade.", new Color(0.35f, 0.85f, 0.75f));
                turtleDialogue.AddLine("Elder Sea Turtle", "A colossal monster haunts the deepest trenches. It drains the life from our corals and the color from our tides.", new Color(0.35f, 0.85f, 0.75f));
                turtleDialogue.AddLine("Elder Sea Turtle", "I appoint this young crab to be your guide on your journey. He shall teach you the way of the water.", new Color(0.35f, 0.85f, 0.75f));
                turtleDialogue.AddLine("Crab Companion", "It is an honor, Elder! Listen closely, traveler: Press [SPACE] to jump over coral ridges. Press [A] to turn the camera left, and [D] to turn the camera right. Move in any direction using [W][S] or the [ARROW KEYS]: Up Arrow for forward, Down Arrow for backward, and Left/Right Arrows to steer.", new Color(1f, 0.65f, 0.3f));
                turtleDialogue.AddLine("Crab Companion", "Now let's explore around the map! There are ancient relics and clues hidden across these reefs.", new Color(1f, 0.65f, 0.3f));
                turtleDialogue.AddLine("Elder Sea Turtle", "Seek the gateway at the end of the shallows. Explore the reefs, meet the school of fish, and uncover the clue hidden beneath the great stone.", new Color(0.35f, 0.85f, 0.75f));

                turtleDialogue.onFinishedCallback = OnTurtleDialogueFinished;
                turtleDialogue.onDialogueFinished.RemoveAllListeners();
                turtleDialogue.onDialogueFinished.AddListener(OnTurtleDialogueFinished);
            }

            // School of Fish Encounter Dialogue
            if (fishDialogue != null)
            {
                fishDialogue.triggerOnlyOnce = true;
                fishDialogue.triggerOnEnter = true; // Auto-trigger when close
                fishDialogue.interactRadius = 5.0f;
                fishDialogue.ClearLines();
                fishDialogue.AddLine("School of Fish", "Greetings, surface friend! The currents whispered of your arrival.", new Color(0.4f, 0.85f, 1f));
                fishDialogue.AddLine("School of Fish", "Beware the giant boulder blocking the trench gateway ahead! You can shift the heavy rock by walking directly into it—push it aside to reveal the beast's mark!", new Color(0.4f, 0.85f, 1f));
                fishDialogue.AddLine("Crab Companion", "Thank you, friends! Let's head to the trench gateway and push that boulder aside!", new Color(1f, 0.65f, 0.3f));

                fishDialogue.onFinishedCallback = OnFishDialogueFinished;
                fishDialogue.onDialogueFinished.RemoveAllListeners();
                fishDialogue.onDialogueFinished.AddListener(OnFishDialogueFinished);
            }
        }

        private void Update()
        {
            if (playerTransform == null)
            {
                var p = GameObject.FindGameObjectWithTag("Player");
                if (p != null) playerTransform = p.transform;
                if (playerTransform == null) return;
            }

            // Check proximity to pearl while exploring
            if (currentStage >= StoryStage.ExplorationTutorial && !hasTriggeredPearlPrompt)
            {
                CheckPearlProximity();
            }

            // Check proximity to school of fish while exploring
            if (currentStage >= StoryStage.ExplorationTutorial && !hasTriggeredFishDialogue && fishDialogue != null)
            {
                float distToFish = Vector3.Distance(playerTransform.position, fishDialogue.transform.position);
                if (distToFish <= fishDialogue.interactRadius)
                {
                    if (DialogueManager.Instance != null && !DialogueManager.Instance.IsDialogueActive)
                    {
                        fishDialogue.TriggerDialogue();
                    }
                }
            }
        }

        private void CheckPearlProximity()
        {
            if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive) return;

            for (int i = pearlTransforms.Count - 1; i >= 0; i--)
            {
                var pearl = pearlTransforms[i];
                if (pearl == null || !pearl.gameObject.activeInHierarchy)
                {
                    pearlTransforms.RemoveAt(i);
                    continue;
                }

                float dist = Vector3.Distance(playerTransform.position, pearl.position);
                if (dist <= 4.5f)
                {
                    hasTriggeredPearlPrompt = true;

                    if (DialogueManager.Instance != null)
                    {
                        DialogueManager.Instance.StartDialogue(
                            "Look over there! That glowing sunlit pearl carries the ancient memories of our reefs. Walk into it to collect it!",
                            "Crab Companion",
                            () =>
                            {
                                if (Level0Controller.Instance != null)
                                {
                                    Level0Controller.Instance.UpdateObjective("Gather the glowing Shallows Relics and explore the reefs.");
                                }
                            }
                        );
                    }
                    break;
                }
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
            hasTriggeredFishDialogue = true;
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
