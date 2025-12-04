# Door Interaction System - Unity Editor Setup Guide

**Date:** 2025-12-01
**Game:** At Hell's Gate - Forsaken
**Feature:** E-key Door Interaction with Custom Prompts

---

## Changes Summary

### Code Files Modified:
1. **CanvasManager.cs** - Added interaction prompt UI management
2. **Door.cs** - Complete rewrite with E-key interaction and closing logic

### New Features:
- All doors require pressing 'E' to open
- Custom interaction prompts per door (editable in Inspector)
- Doors close automatically when player leaves the area
- Locked doors show key requirements ("Requires Red Key", etc.)
- Optional `stayOpen` toggle for boss room doors

---

## Unity Editor Setup (Required!)

### Part 1: Create Interaction Prompt UI

#### Step 1: Create Prompt Panel
1. In **Hierarchy**, find your **Canvas** GameObject
2. Right-click Canvas → **UI** → **Panel**
3. Rename the new panel to: **"InteractionPromptPanel"**
4. In the Inspector for InteractionPromptPanel:
   - **Rect Transform:**
     - **Anchors:** Center (click anchor preset, hold Alt+Shift, click center)
     - **Pos X:** 0
     - **Pos Y:** 0
     - **Width:** 400
     - **Height:** 60
   - **Image Component:**
     - **Color:** Black with transparency (R=0, G=0, B=0, A=150)
   - **IMPORTANT:** Uncheck **"Active"** checkbox at the top (panel starts hidden)

#### Step 2: Create Prompt Text
1. Right-click **InteractionPromptPanel** → **UI** → **Text - TextMeshPro**
   - If prompted to import TMP Essentials, click "Import"
2. Rename the new text to: **"InteractionPromptText"**
3. In the Inspector for InteractionPromptText:
   - **Rect Transform:**
     - **Anchors:** Stretch/Stretch (fill parent)
     - **Left, Right, Top, Bottom:** all set to 0
   - **TextMeshProUGUI Component:**
     - **Text:** "Press [E] to Interact" (placeholder, will be replaced)
     - **Font Size:** 24-28
     - **Alignment:** Center (both horizontal and vertical)
     - **Color:** White
     - **Auto Size:** Leave unchecked
     - **Wrapping:** Enabled

#### Step 3: Assign References in CanvasManager
1. In Hierarchy, find your **CanvasManager** GameObject (likely under Canvas)
2. In the Inspector, scroll down to the **"Interaction Prompt"** header (new section)
3. Assign the references:
   - **Interaction Prompt Panel:** Drag **InteractionPromptPanel** from Hierarchy
   - **Interaction Prompt Text:** Drag **InteractionPromptText** from Hierarchy

---

### Part 2: Update Door Animators

Each door in your scene needs a "CloseDoor" trigger added to its Animator.

#### Step 1: Add CloseDoor Trigger Parameter
1. Select a **Door** GameObject in your scene
2. In the Inspector, find the **Animator** component
3. Click on the **Controller** field (e.g., "DoorAnimator") to open it
4. In the Animator window, look at the **Parameters** tab (left side)
5. Click the **+** button and select **"Trigger"**
6. Name it: **"CloseDoor"**

#### Step 2: Create Close Animation (if needed)
If you don't already have a door closing animation:

**Option A: Duplicate & Reverse Open Animation**
1. In Project window, find your door opening animation clip
2. Duplicate it (Ctrl+D) and rename to "DoorClose"
3. In the Animation window, you can reverse the keyframes manually

**Option B: Create New Animation**
1. Select the door GameObject
2. Open Animation window (Window → Animation → Animation)
3. Click "Create" and name it "DoorClose"
4. Animate the door moving back to closed position

#### Step 3: Set Up Animator Transitions
1. Open the Animator window for your door
2. You should see states like: **Idle**, **Open**, etc.
3. Create a transition:
   - Right-click the **Open** state → **Make Transition** → drag to **Idle** (or to a new "Closed" state if you have one)
4. Click on the new transition arrow
5. In Inspector:
   - **Has Exit Time:** Unchecked
   - **Transition Duration:** 0.1-0.5 (adjust for smoothness)
   - **Conditions:** Click **+** → Select **"CloseDoor"**

**Repeat for all doors in your scene!**

---

### Part 3: Configure Doors in Inspector

For each Door GameObject in your scene:

#### New Door Properties Available:

**Interaction Section:**
- **Interaction Prompt:** "Press [E] to Open" (customize per door!)
  - Examples:
    - "Press [E] to Open Door"
    - "Press [E] to Enter"
    - "Press [E] to Use Exit"
- **Locked Prompt:** "Requires {0} Key" (shows when locked)
  - `{0}` is automatically replaced with "Red", "Green", or "Blue"

**Door State Section:**
- **Stay Open:** ☐ Unchecked (door closes when player leaves)
  - Check this for boss room doors or one-way doors

#### Example Configurations:

**Regular Door:**
```
requiresKey: ☐
interactionPrompt: "Press [E] to Open"
stayOpen: ☐
```

**Locked Door (Red Key):**
```
requiresKey: ☑
reqRed: ☑
interactionPrompt: "Press [E] to Unlock"
lockedPrompt: "Requires {0} Key"
stayOpen: ☐
```

**Boss Room Door:**
```
requiresKey: ☐
interactionPrompt: "Press [E] to Enter Boss Room"
stayOpen: ☑ (door stays open once entered)
```

---

## How It Works

### Player Approaches Door:
1. Player enters door's trigger collider
2. Prompt appears center screen (custom text from Inspector)
3. If locked, shows "Requires [Color] Key"

### Player Presses 'E':
1. Door checks if player has required key (if locked)
2. If yes: door opens with animation and sound
3. If no: plays locked sound
4. Prompt disappears immediately
5. Area spawns enemies (if configured)

### Player Walks Away:
1. Player exits door's trigger collider
2. Prompt disappears
3. Door closes (unless `stayOpen` is checked)
4. "CloseDoor" animation trigger fires

---

## Testing Checklist

Test each door type in your scene:

- [ ] **Regular door:** Walk up, prompt shows, press E, door opens
- [ ] **Regular door:** Walk away, door closes automatically
- [ ] **Locked door without key:** Walk up, shows "Requires [Color] Key"
- [ ] **Locked door without key:** Press E, plays locked sound, doesn't open
- [ ] **Locked door with key:** Collect key, walk up, press E, door opens
- [ ] **stayOpen door:** Open door, walk away, door stays open
- [ ] **Custom prompts:** Each door shows its custom text
- [ ] **Area spawning:** Enemies spawn when door opens (if configured)

---

## Troubleshooting

### Issue: Prompt doesn't appear
**Checklist:**
- Is InteractionPromptPanel assigned in CanvasManager Inspector?
- Is InteractionPromptText assigned in CanvasManager Inspector?
- Does the door have a Collider with "Is Trigger" checked?
- Is the player tagged as "Player"?

### Issue: Door doesn't open when pressing E
**Checklist:**
- Is the door's Animator assigned?
- Does the Animator have an "OpenDoor" trigger parameter?
- Is the player actually inside the door's trigger collider?
- Check Console for errors

### Issue: Door doesn't close
**Checklist:**
- Does the Animator have a "CloseDoor" trigger parameter?
- Is there a transition from Open state using the "CloseDoor" trigger?
- Is `stayOpen` unchecked on the door?
- Did the player leave the trigger area?

### Issue: Prompt shows wrong key color
**Checklist:**
- Make sure only ONE of reqRed/reqGreen/reqBlue is checked
- If multiple are checked, only the first one will be shown

### Issue: Multiple doors showing prompts at once
**This is expected behavior** - each door independently shows/hides its prompt. The most recent door entered will show its prompt. This is fine for most games, but if you want only one prompt at a time, you'll need to track active doors globally.

---

## Advanced Customization

### Different Prompts for Different Platforms
Change the prompt text to match your controller:
- PC: "Press [E] to Open"
- Console: "Press [A] to Open" (you'd need to change the key code in Door.cs)

### Add Sound Variations
You can assign different AudioSources to different doors for variety in open/close/locked sounds.

### Fade In/Out Prompts
Modify CanvasManager's ShowInteractionPrompt/HideInteractionPrompt to use Animator or DOTween for smooth fade effects.

---

**Setup Complete!**
All doors now require 'E' to open, show custom prompts, and close when the player leaves!
