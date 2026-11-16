# Ben - enemies

| Enemy | Script | What it does |
|---|---|---|
| Jeep | `Jeep : Enemy` | Car bomb, drives at you and explodes |
| Tank | `Tank : RangedEnemy : Enemy` | Drives near you, stops, shoots |
| Turret | `Turret : Tank` | Doesn't move, shoots powerful fast bullets |

Shared shooting code is in `RangedEnemy`, and the projectile is `Bullet`.

# Jeep

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

# Tank and Turret

**Tank:** drives toward the player and stops at **Attacking Range**. It starts shooting earlier, at **Fire Radius**. If the player backs away, it drives again.

```
far away --> drives, Top turns toward you --> within Fire Radius: shoots while driving --> within Attacking Range: stops, keeps shooting
```

**Turret:** a Tank with Move Speed locked to 0. It turns slower but hits much harder, with a faster bullet.

The two parts turn separately:
- **Bottom** (hull) turns toward where it's driving. It turns first, then drives forward, like a real tank.
- **Top** (gun) turns left and right toward the player.
- **Gun** (the barrel, inside Top) tilts up and down toward the player, up to **Max Tilt Up** / **Max Tilt Down**.
- It only fires once the barrel is pointing at the player (**Aim Tolerance**). Running around it is a way to dodge, and so is standing right next to it, below how far the barrel can tilt down.

Bullets fly straight, only damage the **Player**, and disappear on any hit. Enemies don't hurt each other with bullets, but Jeep explosions still hurt Tanks and Turrets.

## Quick test

1. Open `Assets/Scenes/Ben.unity`.
2. Click **Tools > Ben > Tank Test Setup**. This adds a ground, a test player, a Tank and a Turret, and creates the prefabs if they don't exist yet.
3. Press Play and drag the player around in the **Scene view**.
   - Orange gizmo: Fire Radius. Yellow gizmo: Attacking Range. Select the Tank or Turret to see them.

**Tools > Ben > Create Tank And Turret Prefabs** rebuilds all four prefabs (`Tank`, `Turret`, `Bullet`, `TurretBullet`). It **overwrites** them, so don't run it after real models are in.

## Spawning from code

```csharp
Tank tank = Instantiate(tankPrefab, position, rotation);
tank.Initialize(player); // optional, same as the Jeep
```

## Settings (Inspector)

| Field | From | What it does |
|---|---|---|
| Hp, Move Speed | Entity | Health, top speed. Turret's Move Speed is always 0 |
| Attacking Range | Enemy | Tank stops driving at this distance. Not used by Turret |
| Damage | Enemy | Damage per bullet |
| Fire Radius | RangedEnemy | Starts shooting at this distance. Keep it bigger than Attacking Range |
| Fire Rate | RangedEnemy | Shots per second |
| Bullet Prefab | RangedEnemy | `Bullet` for Tank, `TurretBullet` for Turret |
| Muzzle | RangedEnemy | Where bullets come out (tip of the barrel) |
| Fire Sound | RangedEnemy | `Tank_Fire` / `Turret_Fire`. Silent until clips are set in the SoundDatabase |
| Bottom, Top | Tank | The two parts that turn separately |
| Gun | Tank | Barrel pivot inside Top that tilts up/down. Optional: if empty, the barrel doesn't tilt |
| Acceleration | Tank | How fast it reaches Move Speed |
| Bottom Turn Speed | Tank | Hull turning, degrees per second |
| Top Turn Speed | Tank | Gun turning, degrees per second. Lower = easier to dodge |
| Tilt Speed | Tank | Barrel tilting, degrees per second |
| Max Tilt Up / Down | Tank | How far the barrel can tilt. Players outside it can't be shot |
| Aim Tolerance | Tank | Fires only when the barrel is within this many degrees of where it's aiming |
| Aim Lag | Tank | WW1 jank: seconds the gunner trails behind the player. Moving players get missed. 0 = perfect tracking |
| Aim Wobble | Tank | WW1 jank: how far (units) the aim drifts around the player. 0 = dead accurate |
| Jank | Tank | WW1 jank, 0 to 1: the gun turns in fits and starts and the engine surges. 0 = smooth |
| Speed, Lifetime | Bullet | Bullet speed and how long a miss stays alive. Set on the bullet prefab |

Default values:

| | Tank | Turret |
|---|---|---|
| Hp | 100 | 80 |
| Damage | 10 | 35 |
| Fire Rate | 1 / sec | 0.4 / sec |
| Fire Radius | 25 | 40 |
| Move Speed | 3 | 0 |
| Top Turn Speed | 30°/s | 20°/s |
| Aim Lag | 0.8 s | 1.2 s |
| Bullet speed | 30 | 90 |

## Swapping in the real model

```
Tank          <- Tank script, Rigidbody, BoxCollider (keep these). Never rotates
├─ Bottom     <- hull/tracks: replace the boxes inside
└─ Top        <- turret head: replace the Head box. Turns left/right
   └─ Gun     <- pivot at the BACK of the barrel. Tilts up/down
      ├─ Barrel  <- replace with the real barrel
      └─ Muzzle  <- move to the tip of the real barrel
```

1. Put the real hull model inside `Bottom`, the head inside `Top` and the barrel inside `Gun`, and delete the placeholder shapes.
2. Fronts face **+Z** (blue arrow) for all of them.
3. Keep `Top` as a child of the **root**, not of `Bottom`. Otherwise the gun turns along with the hull.
4. Put `Gun` where the barrel joins the head, because the barrel tilts around that point.
5. Move `Muzzle` to the end of the barrel and resize the root **BoxCollider**.

The Turret is set up the same way.

## Changes in other people's files

These are all marked with `// [Ben]` in the code.

| File | Change |
|---|---|
| `Chai/Scripts/Entity.cs` | `OnDead()` is now `virtual` |
| `Chai/Scripts/Enemy.cs` | Added `player`, `attackingRange`, `damage`, `Initialize(Player)`, and an auto-find of the Player in `Start()` |
| `Chai/Scripts/Player.cs` | Now extends `Entity` (as in the class diagram), so it can take damage. **Set its Hp in the Inspector** |
| `Filly/Script/SoundType.cs` | Added `Jeep_Beep`, `Jeep_Explode`, `Tank_Fire` and `Turret_Fire`. Assign clips in the SoundDatabase to hear them |

## Differences from the class diagram

- `Bullet.Initialize(Player)` is `Bullet.Initialize(Entity owner, float damage)`. The bullet needs to know who fired it, so it doesn't hit them, and how much damage to do.
- `Turret` extends `Tank`, not `RangedEnemy` directly. This follows the "Tank with moveSpeed 0" note.
- `IsPlayerInRange()` and `Fire()` are `protected` instead of private, so Tank and Turret can use them.
