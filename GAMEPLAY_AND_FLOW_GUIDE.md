# Upon Heavy Tides — Gameplay & Architecture Guide

## 1. Project Overview
- **Game Title:** Upon Heavy Tides
- **Engine Version:** Unity 6000.3.13f1
- **Input System:** Unity Input System (Package) — keyboard and mouse
- **Genre:** 3D Underwater Adventure & Puzzle

---

## 2. Controls Quick Reference
| Action | Key / Input | Details |
|---|---|---|
| **Move** | **Arrow Keys** (or **W, S**) | Up = Forward, Down = Backward, Left/Right = Turn/Steer |
| **Camera Rotate** | **A** / **D** | **A** = Turn camera right, **D** = Turn camera left |
| **Jump** | **Space** | Leap upward through water currents / over ridges |
| **Interact / Talk** | **E** or **Enter** | Speak to NPCs, inspect objects, advance dialogues |
| **Push Objects** | Walk directly into object | Physics-based push powered by `Pushable.cs` and `PlayerPusher.cs` |

---

## 3. "Shadow Relics" Explained
In the game design and user interface, these items are named **Shallows Relics** (also referred to in narrative as **Sunlit Pearls**).
- **Count:** 5 relics scattered throughout the Level 0 shallows.
- **Visual:** Glowing iridescent pearls resting atop sculpted coral pedestals.
- **HUD Counter:** Displayed in the top right corner as:
  ```
  Shallows Relics: X/5
  ```
- **Purpose:** Optional exploration collectibles that encourage the player to explore reefs and practice jumping and navigation before entering the abyss.

---

## 4. Level 0: The Shallows (Narrative & Tutorial Flow)

### Stage 1: Awakening Behind the Rock
- **Spawn:** The player arrives on the seabed at `(0, 1, -32)`.
- **HUD Objective:** `[OBJECTIVE] Awakening: Talk to the nervous crab sheltering behind the rock.`
- **NPC:** The Crab Companion waits at `(-3.5, 0.4, -30)` behind a large shelter rock.
- **Interaction:** Player approaches within 4.5m and presses **E**.
- **Dialogue (Fires ONCE only):**
  1. *Crab:* "P-psst! Over here! Behind this rock!"
  2. *Crab:* "By the Great Tides... you're not from the ocean! You're the creature from above!"
  3. *Crab:* "A monster from the deep has stolen our world's light. Our kingdom is drowning in fear..."
  4. *Crab:* "Follow me! Let's go to the Elder Sea Turtle at the ancient sanctuary!"
- **State Transition:** 
  - Crab's intro dialogue trigger is permanently disabled.
  - Crab transitions to `LeadingToTarget` mode and walks smoothly to the turtle sanctuary at `(0, 0.4, -7.5)`.
  - HUD Objective updates to: `Follow the Crab Companion to the Elder Sea Turtle sanctuary.`

---

### Stage 2: The Elder Sea Turtle Sanctuary
- **Location:** Ancient coral altar and pillars at `(0, 0, -10)`.
- **NPCs:** Elder Sea Turtle and Crab Companion waiting by the entrance.
- **Interaction:** Player approaches the Elder Turtle and presses **E**.
- **Dialogue (Fires ONCE only):**
  1. *Elder Turtle:* "Welcome, traveler of the surface. It was foretold that a creature from above would descend when our world's light began to fade."
  2. *Elder Turtle:* "A colossal monster haunts the deepest trenches. It drains the life from our corals and the color from our tides."
  3. *Elder Turtle:* "I appoint this young crab to be your guide on your journey. He shall teach you the way of the water."
  4. *Crab Companion:* "It is an honor, Elder! Listen closely, traveler: Press [SPACE] to jump over coral ridges. Press [A] to turn the camera right, and [D] to turn the camera left. Move in any direction using the [ARROW KEYS]: Up Arrow for forward, Down Arrow for backward, and Left/Right Arrows to steer."
  5. *Elder Turtle:* "Seek the gateway at the end of the shallows. Explore the reefs, meet the school of fish, and uncover the clue hidden beneath the great stone."
- **State Transition:**
  - Turtle dialogue is permanently disabled.
  - Crab transitions to `FollowingPlayer` mode and actively follows the player everywhere.
  - HUD Objective updates to: `The Way of the Water: Practice moving (Arrows / WASD), jumping (Space), and camera rotation (A/D). Explore the shallows!`

---

### Stage 3: Exploring the Shallows & School of Fish
- **Location:** The swimming school of fish near the reef at `(12, 1.8, 6)`.
- **Interaction:** Player approaches within 5m and presses **E**.
- **Dialogue (Fires ONCE only):**
  1. *School of Fish:* "Greetings, surface friend! The currents whispered of your arrival."
  2. *School of Fish:* "The waters ahead grow murky and perilous, but we promise to help you in whatever way we can on your voyage!"
  3. *School of Fish:* "Beware the giant boulder blocking the trench gateway ahead—it conceals an ancient mark of the beast."
- **State Transition:**
  - School of Fish dialogue disabled.
  - HUD Objective updates to: `Investigate the trench gateway and push the heavy boulder aside to reveal the clue.`

---

### Stage 4: Culmination — Shifting the Boulder & Clue Discovery
- **Location:** The Trench Gateway at `(0, 0, 26)`.
- **Puzzle:** A heavy sandstone boulder (`FinalBoulder`) blocks a stone wall carving.
- **Action:** Player walks forward into the boulder. `Pushable.cs` allows physics movement.
- **Trigger:** When pushed more than 1.8m away from its starting position:
  - The recessed **Colossal Sucker Scar** is revealed (complete with 8 rotating hooked barbs).
  - Clue popup modal displays the journal discovery.
  - Crab comments on the glowing notebook sketch.
  - HUD Objective updates to: `<color=#00FF7F>Tutorial Complete!</color> The first monster clue has been recorded in your journal.`
  - Level 0 is complete and Level 1 is unlocked.

---

## 5. Level 1: The Kelp Forest Maze (Preview)
- **First 1/4 of Level:** Safe sanctuary zone containing the shell-less Hermit Crab's house. The player speaks with the Hermit Crab to learn about their stolen shell.
- **Remaining 3/4 of Level:** A Pac-Man inspired kelp labyrinth.
- **Predators:** 2 groups of Barracudas (2 fish per group) patrolling set waypoint loops.
- **AI Behavior:** `BarracudaAI.cs` Finite State Machine:
  - **Patrol:** Swimming along patrol circuit.
  - **Investigate:** Heard a noise or saw movement.
  - **Chase:** Pursuing player within sight cone.
  - **Return:** Losing sight and returning to waypoint route.
- **Stealth:** Hiding groves where the player can blend in to break predator line-of-sight (`StealthZoneTrigger.cs`).

---

## 6. Architecture & Code Structure
```
Assets/
├── Scripts/
│   ├── Level0/
│   │   ├── Level0StoryFlow.cs       # Orchestrates narrative stages & dialogue progression
│   │   ├── Level0Controller.cs      # Manages HUD, objectives, relics, and level save
│   │   ├── DialogueManager.cs       # Typewriter dialogue UI singleton
│   │   ├── DialogueTrigger.cs       # Handles interact key (E/Enter) & single execution
│   │   └── ClueDiscoveryTrigger.cs  # Detects boulder displacement and reveals clue
│   ├── GameAI/
│   │   ├── Companion.cs             # Crab companion FSM (Waiting, Leading, Following)
│   │   ├── BarracudaAI.cs           # Predator FSM for Level 1 maze
│   │   └── StealthZoneTrigger.cs    # Hiding zones in kelp forest
│   ├── Player/
│   │   ├── PlayerMovement.cs        # Input System movement & camera-relative steering
│   │   └── PlayerPusher.cs          # Passes player momentum to pushable objects
│   ├── CameraTracker.cs             # Third-person camera with A/D rotation
│   └── Interaction/
│       └── Pushable.cs              # Pushable physics component
└── Editor/
    ├── Level0Builder.cs             # One-click generator for Level 0
    └── Level1BlockoutBuilder.cs     # One-click generator for Level 1
```
