# Rhythm Vampire Survivors - Unity Game

A rhythm-based Vampire Survivors clone where all movement and actions are synchronized to the beat of music.

## 🎮 Game Concept

- **Genre**: Rhythm-based Action Survivor
- **Core Mechanic**: All movement (player, enemies, projectiles) happens on specific beats
- **Movement**: Free-form (not grid-based) but beat-synchronized with pauses between beats
- **Goal**: Survive waves of enemies, collect experience, level up, and choose upgrades

## 📁 Project Structure

```
Assets/
├── Scripts/
│   ├── ScriptableObjects/     ✅ Complete (6 files)
│   ├── Core/                   ✅ Complete (3 files)
│   ├── Player/                 ✅ Complete (2 files)
│   ├── Enemies/                ⏳ In Progress (1/5 files)
│   ├── Attacks/                ⏳ Not Started (0/7 files)
│   ├── Spawning/               ⏳ Not Started (0/3 files)
│   ├── Camera/                 ⏳ Not Started (0/1 files)
│   ├── UI/                     ⏳ Not Started (0/4 files)
│   ├── Feedback/               ⏳ Not Started (0/1 files)
│   └── Editor/                 ⏳ Not Started (0/1 files)
└── ...
```

## ✅ Completed Systems

### 1. ScriptableObject Architecture (100%)
All data-driven configuration classes:
- [`GameConfigSO.cs`](Assets/Scripts/ScriptableObjects/GameConfigSO.cs) - Global settings (BPM, health, spawn radius)
- [`EnemyTypeSO.cs`](Assets/Scripts/ScriptableObjects/EnemyTypeSO.cs) - Enemy definitions
- [`AttackTypeSO.cs`](Assets/Scripts/ScriptableObjects/AttackTypeSO.cs) - Base attack class
- [`ProjectileAttackSO.cs`](Assets/Scripts/ScriptableObjects/ProjectileAttackSO.cs) - Projectile attack config
- [`AttackUpgradeSO.cs`](Assets/Scripts/ScriptableObjects/AttackUpgradeSO.cs) - Upgrade definitions
- [`StageConfigSO.cs`](Assets/Scripts/ScriptableObjects/StageConfigSO.cs) - Per-stage configuration

### 2. Core Systems (100%)
- [`BeatManager.cs`](Assets/Scripts/Core/BeatManager.cs) - Central rhythm timing system
  - Calculates beat timing from BPM
  - Emits beat events for all systems
  - Provides beat synchronization utilities
- [`GameStateManager.cs`](Assets/Scripts/Core/GameStateManager.cs) - Game state transitions
  - Manages Gameplay, UpgradeSelection, GameOver states
  - Handles pause/unpause
- [`LevelManager.cs`](Assets/Scripts/Core/LevelManager.cs) - Level progression
  - Tracks experience and levels
  - Loads stage configurations
  - Triggers upgrade screen

### 3. Player System (100%)
- [`PlayerController.cs`](Assets/Scripts/Player/PlayerController.cs) - Beat-synchronized movement
  - Reads input every frame, stores direction
  - Moves on designated beats (every 2 beats default)
  - DOTween jump and rotation animations
- [`PlayerHealth.cs`](Assets/Scripts/Player/PlayerHealth.cs) - Health management
  - Damage handling with invincibility frames
  - Visual feedback (flash, fade)
  - Death handling

### 4. Enemy System (20%)
- [`Enemy.cs`](Assets/Scripts/Enemies/Enemy.cs) - Base enemy class
  - Beat-synchronized movement
  - Collision avoidance
  - Health and damage
  - Death and gem dropping

## ⏳ Remaining Work

### Priority 1: Complete Enemy System
Create these files in `Assets/Scripts/Enemies/`:
1. `EnemyChaser.cs` - Inherits from Enemy, uses default chase behavior
2. `EnemyStationary.cs` - Inherits from Enemy, overrides to not move
3. `EnemyRanged.cs` - Inherits from Enemy, shoots projectiles
4. `EnemyProjectile.cs` - Projectile fired by ranged enemies

### Priority 2: Attack System
Create these files in `Assets/Scripts/Attacks/`:
1. `AttackManager.cs` - Manages all active attacks
2. `AttackInstance.cs` - Base class for attack instances
3. `ProjectileAttack.cs` - Player projectile implementation
4. `BeamAttack.cs`, `ShieldAttack.cs`, `BombAttack.cs`, `AutoAttackerAttack.cs` - Other attack types

### Priority 3: Spawning System
Create these files in `Assets/Scripts/Spawning/`:
1. `EnemySpawner.cs` - Spawns enemies in waves
2. `ExperienceGem.cs` - Collectible gem
3. `CollectibleSpawner.cs` - Spawns gems when enemies die

### Priority 4: Camera & UI
Create these files:
1. `Assets/Scripts/Camera/CameraController.cs` - Follows player
2. `Assets/Scripts/UI/HUDManager.cs` - Health/XP display
3. `Assets/Scripts/UI/UpgradeScreenUI.cs` - Upgrade selection
4. `Assets/Scripts/UI/GameOverUI.cs` - Game over screen
5. `Assets/Scripts/UI/BeatFeedbackUI.cs` - Beat visual feedback

### Priority 5: Editor Script (CRITICAL)
Create `Assets/Scripts/Editor/GameSetupEditor.cs`:
- Generates entire scene with one click
- Creates all GameObjects, UI, prefabs
- Creates ScriptableObject assets
- Sets up references

## 🚀 Quick Start (After Completion)

### Step 1: Run Editor Script
1. In Unity, go to `Tools > Setup Rhythm Game Scene`
2. This will generate everything needed

### Step 2: Configure
1. Assign GameConfig to BeatManager
2. Set BPM (default: 120)
3. Adjust other settings as needed

### Step 3: Play
1. Press Play in Unity
2. Use WASD or mouse to move
3. Player moves every 2 beats
4. Kill enemies, collect gems, level up!

## 🎵 Key Features

### Beat Synchronization
- **BPM**: Configurable (default 120 BPM = 0.5s per beat)
- **Player Movement**: Every 2 beats
- **Enemy Movement**: Configurable per enemy type
- **Projectile Movement**: Configurable per attack type
- **Visual Feedback**: Screen pulse on each beat

### Movement System
- **Free-form**: Not grid-based, smooth movement
- **Beat-synchronized**: Movement only on specific beats
- **Pauses**: Everything stands still between beats
- **Direction**: Player moves in input direction, enemies toward player

### Upgrade System
- **Level Up**: Collect experience gems to level up
- **Upgrade Screen**: Choose 1 of 3 options (pause game)
- **Options**: New attacks or upgrades to existing attacks
- **Stacking**: Upgrades can stack for more power

## 🔧 Technical Details

### Dependencies
- **Unity Input System**: For player input
- **DOTween**: For animations (jump, rotation, UI)
- **FMOD**: For audio (existing AudioManager)

### Architecture Patterns
- **Singleton**: BeatManager, GameStateManager, LevelManager
- **Observer**: Beat events, state change events
- **Strategy**: Enemy behaviors, upgrade application
- **ScriptableObject**: Data-driven design

### Tags Required
- `Player`
- `Enemy`
- `EnemyProjectile`
- `ExperienceGem`

### Layers (Optional)
- `Player`
- `Enemy`
- `Projectile`
- `Collectible`

## 📚 Documentation

See the `plans/` directory for detailed documentation:
- [`rhythm-vampire-survivors-architecture.md`](plans/rhythm-vampire-survivors-architecture.md) - Complete architecture
- [`system-interactions-diagram.md`](plans/system-interactions-diagram.md) - Flow diagrams
- [`implementation-checklist.md`](plans/implementation-checklist.md) - Detailed checklist
- [`IMPLEMENTATION_STATUS.md`](IMPLEMENTATION_STATUS.md) - Current progress

## 🐛 Known Issues

1. **Input Actions**: Need to create InputActions asset manually or via editor script
2. **Tags**: Need to set up tags in Unity
3. **Materials**: Need to create materials for visual distinction
4. **Testing**: Beat synchronization needs thorough testing

## 💡 Design Decisions

### Why Free-form Movement?
- More fluid gameplay
- Better feel for rhythm game
- Easier to dodge enemies
- Still maintains beat synchronization

### Why Beat Synchronization?
- Creates unique tactical gameplay
- Forces player to think ahead
- Rhythm creates satisfying feedback loop
- Differentiates from standard Vampire Survivors

### Why ScriptableObjects?
- Easy to balance without code changes
- Designer-friendly
- Supports rapid iteration
- Clear separation of data and logic

## 🎯 Next Steps for Developer

1. **Continue Implementation**: Create remaining enemy, attack, and spawning systems
2. **Create Editor Script**: Most important for quick setup
3. **Test Core Loop**: Ensure beat synchronization feels good
4. **Balance**: Adjust enemy counts, damage, experience values
5. **Polish**: Add more visual effects, sounds, screen shake

## 📝 Notes

- All scripts use XML documentation comments
- Follow existing code style and patterns
- Test each system incrementally
- Use the planning documents as reference
- The editor script will tie everything together

## 🤝 Contributing

When adding new features:
1. Follow the ScriptableObject pattern for configuration
2. Subscribe to BeatManager events for timing
3. Check GameStateManager before acting
4. Use DOTween for animations
5. Add XML documentation comments

## 📄 License

[Your License Here]

---

**Current Progress**: 12/35 files (34%)  
**Estimated Time Remaining**: ~3-4 hours  
**Status**: Foundation Complete, Core Systems Working
