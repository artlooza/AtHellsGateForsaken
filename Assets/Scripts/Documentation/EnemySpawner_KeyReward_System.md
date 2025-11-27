# Enemy Spawner & Key Reward System

## Overview

This document covers the enemy spawner system that spawns enemies in rooms and awards keys when the player kills enough enemies.

---

## Files Modified

| File | Changes |
|------|---------|
| `Assets/Scripts/Managers/EnemySpawner.cs` | Added key reward system, distance-based detection, kill tracking |
| `Assets/Scripts/Player/PlayerInventory.cs` | Added `GiveKey(string keyColor)` method |

---

## EnemySpawner.cs

### Inspector Settings

#### Spawning Settings
| Setting | Type | Description |
|---------|------|-------------|
| `enemyPrefab` | GameObject | The enemy prefab to spawn |
| `maxEnemies` | int | Maximum enemies alive at once (default: 5) |
| `spawnInterval` | float | Seconds between spawn attempts (default: 2) |
| `minDistanceFromPlayer` | float | Enemies won't spawn closer than this to player (default: 5) |

#### Spawn Area
| Setting | Type | Description |
|---------|------|-------------|
| `spawnPoints` | Transform[] | Optional specific spawn locations |
| `useRandomPositionInRoom` | bool | If true, spawns randomly within roomSize bounds |
| `roomSize` | Vector3 | Dimensions of spawn area (X and Z used) |

#### Key Reward
| Setting | Type | Description |
|---------|------|-------------|
| `killsRequired` | int | Number of enemies to kill to get the key (default: 10) |
| `keyColor` | string | Key color: "red", "blue", or "green" |
| `keyPickupPrefab` | GameObject | Optional: spawn a pickup instead of giving key directly |
| `keySpawnPoint` | Transform | Where to spawn the key pickup |

#### State
| Setting | Type | Description |
|---------|------|-------------|
| `isActive` | bool | Whether spawner is currently spawning |
| `isCompleted` | bool | Whether room has been cleared |
| `startActiveOnPlay` | bool | Start spawning immediately (for testing) |
| `stopWhenPlayerLeaves` | bool | Stop spawning when player exits room |
| `useDistanceDetection` | bool | Use distance check instead of trigger (recommended: true) |

### How It Works

1. **Room Detection**: When player enters the room bounds (based on `roomSize`), spawning begins
2. **Spawning**: Enemies spawn up to `maxEnemies` at a time, at valid NavMesh positions
3. **Kill Tracking**: Destroyed enemies are counted as kills each spawn interval
4. **Completion**: When `killsRequired` kills are reached, the key is awarded
5. **Key Delivery**: Key is either given directly to player inventory OR spawned as a pickup

### Detection Methods

**Distance Detection (Recommended)**
- Set `useDistanceDetection = true`
- Player position is checked against `roomSize` bounds
- More reliable than trigger-based detection

**Trigger Detection (Alternative)**
- Set `useDistanceDetection = false`
- Requires Box Collider with "Is Trigger" checked
- Player must have "Player" tag

---

## PlayerInventory.cs

### Added Method

```csharp
public void GiveKey(string keyColor)
```

Sets the appropriate key flag based on color:
- `"red"` → `hasRed = true`
- `"green"` → `hasGreen = true`
- `"blue"` → `hasBlue = true`

---

## Setup Instructions

### Creating a Room Spawner

1. Create Empty GameObject, name it (e.g., "RedRoomSpawner")
2. Position it at the CENTER of your room
3. Add `EnemySpawner` component
4. Configure settings:
   - Drag Enemy prefab to `enemyPrefab`
   - Set `maxEnemies` (e.g., 5)
   - Set `killsRequired` (e.g., 10)
   - Set `keyColor` (e.g., "red")
   - Set `roomSize` to match your room dimensions
   - Ensure `useDistanceDetection` is checked

### Requirements

- Enemy prefab must exist
- Room floor must have baked NavMesh (Window → AI → Navigation → Bake)
- Player must have `PlayerMove` and `PlayerInventory` components
- `CanvasManager` singleton must exist in scene for HUD updates

---

## Gizmos (Editor Visualization)

When selecting the spawner in Scene view:
- **Colored cube**: Spawn area (color matches key color)
- **Yellow wire sphere**: Minimum distance from player
- **Green spheres**: Spawn points (if using specific points)
- **Colored sphere + line**: Key spawn point location

---

## Known Issues / Notes

1. **Trigger detection unreliable**: Use `useDistanceDetection = true` for best results
2. **NavMesh required**: Enemies only spawn on valid NavMesh positions
3. **Remaining enemies after key**: Currently enemies remain after getting key (ClearAllEnemies is commented out in CompleteRoom)

---

## Future Improvements (TODO)

- [ ] Option to spawn fixed total enemies (e.g., spawn exactly 10, must kill all to get key)
- [ ] Wave-based spawning
- [ ] Boss enemy support
- [ ] Audio/visual feedback when room is cleared
- [ ] Save/load room completion state

---

## Session Date

November 27, 2025
