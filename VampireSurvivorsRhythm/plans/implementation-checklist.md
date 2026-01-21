# Implementation Checklist

## Phase 1: Core Systems (Foundation)

### 1.1 Beat Manager System
- [ ] Create [`BeatManager.cs`](Assets/Scripts/Core/BeatManager.cs) singleton
- [ ] Implement BPM-based timing calculation
- [ ] Add UnityEvent for beat notifications
- [ ] Add beat counter and progress tracking
- [ ] Test beat accuracy with debug visualization
- [ ] Integrate with AudioManager for music playback

### 1.2 ScriptableObject Architecture
- [ ] Create [`GameConfigSO.cs`](Assets/ScriptableObjects/Config/GameConfigSO.cs)
  - BPM, beat tolerance, starting health, spawn radius
- [ ] Create [`StageConfigSO.cs`](Assets/ScriptableObjects/Stages/StageConfigSO.cs)
  - Stage number, enemy spawns, available upgrades, experience threshold
- [ ] Create [`EnemyTypeSO.cs`](Assets/ScriptableObjects/Enemies/EnemyTypeSO.cs)
  - Name, prefab, move distance, beats to move, damage, experience
- [ ] Create [`AttackTypeSO.cs`](Assets/ScriptableObjects/Attacks/AttackTypeSO.cs) (base)
  - Name, icon, prefab, beats to spawn, damage, category
- [ ] Create [`ProjectileAttackSO.cs`](Assets/ScriptableObjects/Attacks/ProjectileAttackSO.cs)
  - Extends AttackTypeSO with move distance, beats to move, pierce count
- [ ] Create [`AttackUpgradeSO.cs`](Assets/ScriptableObjects/Attacks/AttackUpgradeSO.cs)
  - Name, icon, description, target attack, upgrade type, value modifier

### 1.3 Game State Management
- [ ] Create [`GameStateManager.cs`](Assets/Scripts/Core/GameStateManager.cs) singleton
  - States: Gameplay, UpgradeSelection, GameOver
  - State transition methods
  - Pause/unpause functionality
- [ ] Create [`LevelManager.cs`](Assets/Scripts/Core/LevelManager.cs)
  - Track current level and experience
  - Load appropriate StageConfigSO
  - Emit level up events

---

## Phase 2: Player System

### 2.1 Player Controller
- [ ] Create [`PlayerController.cs`](Assets/Scripts/Player/PlayerController.cs)
  - InputActionReference for Move action
  - Store input direction every frame
  - Move on designated beats (every 2 beats default)
  - Calculate target position (current + direction * moveDistance)
  - Subscribe to BeatManager.OnBeat

### 2.2 Player Health & Animation
- [ ] Create [`PlayerHealth.cs`](Assets/Scripts/Player/PlayerHealth.cs)
  - Track current health
  - Handle damage with invincibility frames
  - Emit death event
- [ ] Create [`PlayerAnimator.cs`](Assets/Scripts/Player/PlayerAnimator.cs)
  - DOTween jump animation on movement
  - DOTween rotation (90° on X-axis)
  - Damage flash effect
  - Scale pulse on beat

### 2.3 Input Action Setup
- [ ] Create InputActions asset if not exists
- [ ] Add "Move" action (Vector2)
- [ ] Configure for mouse position and WASD/gamepad
- [ ] Test input responsiveness

---

## Phase 3: Enemy System

### 3.1 Base Enemy Class
- [ ] Create [`Enemy.cs`](Assets/Scripts/Enemies/Enemy.cs) base class
  - Reference to EnemyTypeSO
  - Subscribe to beat events
  - Beat counter for movement timing
  - Movement logic with collision avoidance
  - Health and damage handling
  - Drop gem on death

### 3.2 Enemy Variants
- [ ] Create [`EnemyChaser.cs`](Assets/Scripts/Enemies/EnemyChaser.cs)
  - Moves toward player every X beats
- [ ] Create [`EnemyStationary.cs`](Assets/Scripts/Enemies/EnemyStationary.cs)
  - Doesn't move, acts as obstacle
- [ ] Create [`EnemyRanged.cs`](Assets/Scripts/Enemies/EnemyRanged.cs)
  - Shoots projectiles every X beats
  - Aim at player position

### 3.3 Enemy Collision Avoidance
- [ ] Implement Physics.OverlapSphere check before movement
- [ ] Try alternative angles if collision detected
- [ ] Skip movement if no valid position found
- [ ] Maintain minimum distance between enemies

---

## Phase 4: Attack System

### 4.1 Attack Manager
- [ ] Create [`AttackManager.cs`](Assets/Scripts/Attacks/AttackManager.cs)
  - Track list of active attacks
  - Spawn attacks on designated beats
  - Handle attack upgrades
  - Reference to player transform

### 4.2 Base Attack Class
- [ ] Create [`AttackInstance.cs`](Assets/Scripts/Attacks/AttackInstance.cs) base
  - Reference to AttackTypeSO
  - Subscribe to beat events
  - Virtual methods for behavior
  - Collision detection

### 4.3 Attack Implementations
- [ ] Create [`ProjectileAttack.cs`](Assets/Scripts/Attacks/ProjectileAttack.cs)
  - Spawn at player position
  - Move in direction every X beats
  - Destroy on enemy collision
  - Support pierce count
- [ ] Create [`BeamAttack.cs`](Assets/Scripts/Attacks/BeamAttack.cs)
  - Continuous damage in direction
- [ ] Create [`ShieldAttack.cs`](Assets/Scripts/Attacks/ShieldAttack.cs)
  - Orbits player
  - Damages on contact
- [ ] Create [`BombAttack.cs`](Assets/Scripts/Attacks/BombAttack.cs)
  - Countdown timer
  - Area damage on explosion
- [ ] Create [`AutoAttackerAttack.cs`](Assets/Scripts/Attacks/AutoAttackerAttack.cs)
  - Stationary turret
  - Auto-targets nearest enemy

---

## Phase 5: Spawning Systems

### 5.1 Enemy Spawner
- [ ] Create [`EnemySpawner.cs`](Assets/Scripts/Spawning/EnemySpawner.cs)
  - Read current StageConfigSO
  - Spawn enemies at intervals (in beats)
  - Calculate spawn positions (circle around player)
  - Ensure minimum distance from player
  - Track alive enemy count

### 5.2 Collectible Spawner
- [ ] Create [`CollectibleSpawner.cs`](Assets/Scripts/Spawning/CollectibleSpawner.cs)
  - Listen to enemy death events
  - Spawn experience gem at death position
  - Handle gem collection by player
  - Add experience to LevelManager

---

## Phase 6: Camera System

### 6.1 Camera Controller
- [ ] Create [`CameraController.cs`](Assets/Scripts/Camera/CameraController.cs)
  - Follow player X and Z position
  - Maintain fixed Y position and rotation
  - Smooth damping for movement
  - Optional camera shake on damage

---

## Phase 7: UI Systems

### 7.1 HUD
- [ ] Create [`HUDManager.cs`](Assets/Scripts/UI/HUDManager.cs)
  - Health bar display
  - Experience bar display
  - Current level display
  - Active attacks icons
  - Update methods for each element

### 7.2 Upgrade Screen
- [ ] Create [`UpgradeScreenUI.cs`](Assets/Scripts/UI/UpgradeScreenUI.cs)
  - Show 3 random upgrade options
  - Display upgrade cards (icon, name, description)
  - Handle player selection
  - Apply upgrade/add attack
  - Resume game after selection

### 7.3 Game Over Screen
- [ ] Create [`GameOverUI.cs`](Assets/Scripts/UI/GameOverUI.cs)
  - Display final score/level
  - Restart button functionality
  - Quit button functionality

### 7.4 Beat Feedback
- [ ] Create [`BeatFeedbackUI.cs`](Assets/Scripts/UI/BeatFeedbackUI.cs)
  - Subscribe to beat events
  - Pulse screen overlay on each beat
  - DOTween scale/fade animation
  - Subtle vignette flash

---

## Phase 8: Visual Feedback

### 8.1 Feedback Manager
- [ ] Create [`FeedbackManager.cs`](Assets/Scripts/Feedback/FeedbackManager.cs)
  - Screen flash on beat
  - Damage feedback (shake, color flash)
  - Hit effects (particles, sound)
  - Level up effects
  - Death effects

### 8.2 DOTween Integration
- [ ] Player jump animation (DOJump)
- [ ] Player rotation (DOLocalRotate)
- [ ] UI pulse (DOScale)
- [ ] Screen flash (DOColor)
- [ ] Damage shake (DOShakePosition)
- [ ] Enemy death animation (DOScale + DOFade)

---

## Phase 9: Editor Script

### 9.1 Scene Setup Editor
- [ ] Create [`GameSetupEditor.cs`](Assets/Scripts/Editor/GameSetupEditor.cs)
- [ ] Add menu item: "Tools/Setup Rhythm Game Scene"
- [ ] Create GameManager GameObject with all core components
- [ ] Create Player GameObject with components and colliders
- [ ] Create Main Camera with CameraController
- [ ] Create Ground Plane for visual reference

### 9.2 UI Generation
- [ ] Create HUD Canvas with all elements
  - Health bar (Slider)
  - Experience bar (Slider)
  - Level text (TextMeshProUGUI)
  - Active attacks panel (Horizontal Layout Group)
  - Beat feedback overlay (full screen Image)
- [ ] Create Upgrade Screen Canvas
  - Background overlay
  - 3 upgrade card buttons
  - Title text
- [ ] Create Game Over Canvas
  - Game over text
  - Final score text
  - Restart button
  - Quit button

### 9.3 Prefab Generation
- [ ] Create Player prefab (cube with material)
- [ ] Create Enemy prefabs (different colored cubes)
  - Chaser enemy (red)
  - Stationary enemy (blue)
  - Ranged enemy (green)
- [ ] Create Projectile prefab (small sphere)
- [ ] Create Experience Gem prefab (rotating crystal)

### 9.4 ScriptableObject Asset Creation
- [ ] Create GameConfig asset with default values
- [ ] Create Stage 1-5 configs with increasing difficulty
- [ ] Create Enemy Type assets (Chaser, Stationary, Ranged)
- [ ] Create Attack Type assets (Projectile, Beam, Shield, Bomb, AutoAttacker)
- [ ] Create Upgrade assets (various upgrades)

---

## Phase 10: Integration & Polish

### 10.1 System Integration
- [ ] Connect all systems together
- [ ] Test beat synchronization across all systems
- [ ] Verify state transitions work correctly
- [ ] Test upgrade system end-to-end
- [ ] Test spawning and difficulty scaling

### 10.2 Audio Integration
- [ ] Integrate with existing AudioManager
- [ ] Play music track on game start
- [ ] Sync BeatManager with music BPM
- [ ] Add hit sounds
- [ ] Add level up sound
- [ ] Add attack sounds
- [ ] Add UI sounds

### 10.3 Balance & Polish
- [ ] Tune movement distances
- [ ] Balance enemy spawn rates
- [ ] Balance upgrade power levels
- [ ] Adjust beat timing feel
- [ ] Polish visual effects
- [ ] Test performance with many entities

### 10.4 Bug Fixes & Optimization
- [ ] Fix any collision issues
- [ ] Optimize enemy pathfinding
- [ ] Add object pooling if needed
- [ ] Fix any timing synchronization issues
- [ ] Test edge cases (player death, level up during damage, etc.)

---

## Testing Checklist

### Core Mechanics
- [ ] Beat timing feels accurate and responsive
- [ ] Player movement feels good
- [ ] Enemy movement is synchronized
- [ ] Attacks spawn and move correctly
- [ ] Collisions are detected properly

### Game Loop
- [ ] Can play through multiple levels
- [ ] Upgrade screen appears correctly
- [ ] Upgrades apply correctly
- [ ] Game over screen works
- [ ] Restart functionality works

### Balance
- [ ] Game difficulty scales appropriately
- [ ] Upgrades feel impactful
- [ ] Player can survive reasonable amount of time
- [ ] Enemy variety is interesting
- [ ] Attack variety is interesting

### Performance
- [ ] Maintains 60 FPS with many enemies
- [ ] No memory leaks
- [ ] No stuttering on beat events
- [ ] Smooth animations

---

## Known Challenges & Solutions

### Challenge: Beat Synchronization Drift
**Solution**: Use single BeatManager as source of truth, avoid Time.deltaTime for beat logic

### Challenge: Enemy Overlap
**Solution**: Physics.OverlapSphere check before movement, try alternative angles

### Challenge: Projectile Collision Timing
**Solution**: Check collisions immediately after movement on beat

### Challenge: Upgrade System Flexibility
**Solution**: Use Strategy pattern, each upgrade has Apply() method

### Challenge: Visual Clarity with Many Entities
**Solution**: Use distinct colors, scale effects, screen space UI indicators

---

## Post-Implementation

### Documentation
- [ ] Add XML comments to all public methods
- [ ] Document ScriptableObject fields
- [ ] Create user guide for designers
- [ ] Document upgrade creation process

### Future Enhancements
- [ ] More enemy types
- [ ] More attack types
- [ ] Boss battles
- [ ] Persistent progression
- [ ] Multiple music tracks with different BPMs
- [ ] Combo system for perfect timing
- [ ] Leaderboards
