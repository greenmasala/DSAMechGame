# Ben - Jeep enemy

Suicide car bomb. It drives at the player like a car, arms when close, beeps and flashes for a moment, then explodes.

```
CHASE --(player within Attacking Range)--> ARMED (beeping, Fuse Time) --> EXPLODE
```

- Once armed it can't be cancelled. The player has to get out of the blast radius.
- Killing it at any time makes it explode right away.
- Explosions damage every `Entity` in range, including other Jeeps, so they can chain-react.

## Quick test

1. Open `Assets/Scenes/Ben.unity` (or any scene).
2. Click **Tools > Ben > Jeep Test Setup**. This adds a ground, a test player capsule and a Jeep.
3. Press Play, then drag the player around in the **Scene view**.
   - Yellow gizmo: arm distance. Red gizmo: blast radius. Select the Jeep to see them.

## Spawning from code (e.g. EnemyManager)

```csharp
Jeep jeep = Instantiate(jeepPrefab, position, rotation);
jeep.Initialize(player); // optional: if skipped, it finds the Player by itself
```

## Settings (Inspector)

| Field | From | What it does |
|---|---|---|
| Hp | Entity | Health. When it reaches 0, the Jeep explodes |
| Move Speed | Entity | Top speed |
| Attacking Range | Enemy | How close the player must get to arm the bomb |
| Damage | Enemy | Explosion damage |
| Acceleration | Jeep | How fast it reaches top speed |
| Turn Speed | Jeep | Degrees per second at top speed (slower when slow, 0 when stopped) |
| Brake Force | Jeep | How fast it stops once armed |
| Fuse Time | Jeep | Seconds from arming to exploding |
| Explode Distance | Jeep | Blast radius. Keep it bigger than Attacking Range |
| Explosion VFX | Jeep | Optional prefab spawned on explosion |
| Warning Lights | Jeep | Renderers that flash. If empty, the whole model flashes |

## Swapping in the real model

The prefab (`Assets/Ben/Prefabs/Jeep.prefab`) is split into two parts:

```
Jeep        <- Jeep script, Rigidbody, BoxCollider (keep these)
└─ Model    <- visuals only: delete the boxes and put the real model here
```

1. Delete `Body` and `Cabin` under `Model`, then drag the real model in as a child of `Model`.
2. Make sure the model's front faces **+Z** (blue arrow). Rotate it inside `Model` if needed.
3. Resize the root **BoxCollider** to fit the model.
4. Optional: drag a roof light or headlight renderer into **Warning Lights**, so only that part flashes.

## Changes in other people's files

These are all marked with `// [Ben]` in the code.

| File | Change |
|---|---|
| `Chai/Scripts/Entity.cs` | `OnDead()` is now `virtual` |
| `Chai/Scripts/Enemy.cs` | Added `player`, `attackingRange`, `damage`, `Initialize(Player)`, and an auto-find of the Player in `Start()` |
| `Chai/Scripts/Player.cs` | Now extends `Entity` (as in the class diagram), so it can take damage. **Set its Hp in the Inspector** |
| `Filly/Script/SoundType.cs` | Added `Jeep_Beep` and `Jeep_Explode`. Assign clips in the SoundDatabase to hear them |
