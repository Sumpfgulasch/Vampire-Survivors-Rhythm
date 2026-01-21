# System Interactions Diagram

## Core Game Loop Flow

```mermaid
graph TD
    A[Game Start] --> B[BeatManager Starts]
    B --> C[Music Plays]
    C --> D[Beat Event Fired]
    D --> E[Player Movement Check]
    D --> F[Enemy Movement Check]
    D --> G[Attack Spawn Check]
    D --> H[Projectile Movement Check]
    D --> I[Visual Beat Feedback]
    
    E --> J{Is Player Beat?}
    J -->|Yes| K[Move Player in Input Direction]
    J -->|No| D
    
    F --> L{Is Enemy Beat?}
    L -->|Yes| M[Move Enemies Toward Player]
    L -->|No| D
    
    G --> N{Is Attack Spawn Beat?}
    N -->|Yes| O[Spawn Attack Instance]
    N -->|No| D
    
    H --> P{Is Projectile Move Beat?}
    P -->|Yes| Q[Move Projectiles]
    P -->|No| D
    
    K --> R[Check Collisions]
    M --> R
    Q --> R
    
    R --> S{Enemy Hit?}
    S -->|Yes| T[Damage Enemy]
    T --> U{Enemy Dead?}
    U -->|Yes| V[Drop Experience Gem]
    U -->|No| D
    
    R --> W{Player Hit?}
    W -->|Yes| X[Damage Player]
    X --> Y{Player Dead?}
    Y -->|Yes| Z[Game Over Screen]
    Y -->|No| D
    
    R --> AA{Gem Collected?}
    AA -->|Yes| AB[Add Experience]
    AB --> AC{Level Up?}
    AC -->|Yes| AD[Show Upgrade Screen]
    AD --> AE[Player Selects Upgrade]
    AE --> AF[Apply Upgrade]
    AF --> D
    AC -->|No| D
    
    V --> D
    I --> D
```

## Beat Synchronization System

```mermaid
sequenceDiagram
    participant BM as BeatManager
    participant P as Player
    participant E as Enemies
    participant A as Attacks
    participant UI as BeatFeedbackUI
    
    Note over BM: Calculate beat timing from BPM
    
    loop Every Beat
        BM->>BM: Increment Beat Counter
        BM->>P: OnBeat Event
        BM->>E: OnBeat Event
        BM->>A: OnBeat Event
        BM->>UI: OnBeat Event
        
        P->>P: Check if movement beat
        alt Is Movement Beat
            P->>P: Move in input direction
            P->>P: Play jump animation
        end
        
        E->>E: Check if movement beat
        alt Is Movement Beat
            E->>E: Calculate direction to player
            E->>E: Move toward player
            E->>E: Check collision avoidance
        end
        
        A->>A: Check if spawn beat
        alt Is Spawn Beat
            A->>A: Spawn new attack instance
        end
        
        A->>A: Check if projectile move beat
        alt Is Move Beat
            A->>A: Move all projectiles
        end
        
        UI->>UI: Pulse screen overlay
    end
```

## Player Input and Movement Flow

```mermaid
graph LR
    A[Input System] -->|Mouse Position or WASD| B[PlayerController]
    B -->|Store Direction| C[Direction Vector]
    
    D[BeatManager] -->|OnBeat Event| E{Is Player Movement Beat?}
    E -->|Yes| F[Read Stored Direction]
    E -->|No| G[Wait for Next Beat]
    
    F --> H[Calculate Target Position]
    H --> I[Current Position + Direction * MoveDistance]
    I --> J[Move to Target]
    J --> K[DOTween Jump Animation]
    J --> L[DOTween Rotation Animation]
    
    K --> M[Check Collisions]
    L --> M
    M --> N[Update Position]
    
    G --> D
    N --> D
```

## Enemy Behavior System

```mermaid
graph TD
    A[Enemy Spawned] --> B[Subscribe to Beat Events]
    B --> C[Wait for Beat]
    
    C --> D[OnBeat Event]
    D --> E[Increment Beat Counter]
    E --> F{beatCounter % BeatsToMove == 0?}
    
    F -->|No| C
    F -->|Yes| G[Calculate Direction to Player]
    
    G --> H[Target Position = Current + Direction * MoveDistance]
    H --> I[Check for Obstacles]
    
    I --> J{Path Clear?}
    J -->|Yes| K[Move to Target]
    J -->|No| L[Try Alternative Angle]
    
    L --> M{Alternative Found?}
    M -->|Yes| K
    M -->|No| N[Skip Movement This Beat]
    
    K --> O[Check Player Collision]
    O --> P{Colliding with Player?}
    P -->|Yes| Q[Damage Player]
    P -->|No| C
    
    N --> C
    Q --> C
```

## Attack System Architecture

```mermaid
graph TD
    A[AttackManager] --> B[Track Active Attacks]
    B --> C[List of AttackInstance]
    
    D[BeatManager OnBeat] --> E{Check Each Attack}
    E --> F[ProjectileAttack]
    E --> G[BeamAttack]
    E --> H[ShieldAttack]
    E --> I[BombAttack]
    E --> J[AutoAttackerAttack]
    
    F --> K{Is Spawn Beat?}
    K -->|Yes| L[Spawn Projectile at Player]
    L --> M[Set Direction from Input]
    M --> N{Is Move Beat?}
    N -->|Yes| O[Move Projectile]
    O --> P[Check Enemy Collision]
    P --> Q{Hit Enemy?}
    Q -->|Yes| R[Damage Enemy]
    R --> S[Destroy Projectile]
    Q -->|No| N
    
    G --> T[Continuous Beam Logic]
    H --> U[Orbit Player Logic]
    I --> V[Countdown to Explosion]
    J --> W[Auto-Target Nearest Enemy]
```

## Upgrade System Flow

```mermaid
graph TD
    A[Player Levels Up] --> B[GameStateManager]
    B --> C[Pause Game]
    C --> D[Get Current StageConfig]
    
    D --> E[Available Upgrades List]
    D --> F[Available New Attacks List]
    
    E --> G[Randomly Select Options]
    F --> G
    
    G --> H[Generate 3 Upgrade Cards]
    H --> I[Display Upgrade Screen UI]
    
    I --> J[Player Clicks Card]
    J --> K{Is New Attack?}
    
    K -->|Yes| L[Add Attack to AttackManager]
    K -->|No| M[Apply Upgrade to Existing Attack]
    
    L --> N[Update UI Active Attacks]
    M --> O[Modify Attack Properties]
    
    N --> P[Hide Upgrade Screen]
    O --> P
    P --> Q[Resume Game]
    Q --> R[Continue Gameplay]
```

## Collision Detection System

```mermaid
graph TD
    A[Movement Occurs] --> B[Check Collisions]
    
    B --> C[Player vs Enemies]
    B --> D[Player vs Gems]
    B --> E[Projectiles vs Enemies]
    B --> F[Enemies vs Enemies]
    
    C --> G{Collision Detected?}
    G -->|Yes| H[Apply Damage to Player]
    H --> I[Trigger Invincibility Frames]
    I --> J[Visual Feedback]
    
    D --> K{Collision Detected?}
    K -->|Yes| L[Collect Gem]
    L --> M[Add Experience]
    M --> N[Update UI]
    
    E --> O{Collision Detected?}
    O -->|Yes| P[Apply Damage to Enemy]
    P --> Q{Enemy Health <= 0?}
    Q -->|Yes| R[Destroy Enemy]
    R --> S[Spawn Gem]
    Q -->|No| T[Continue]
    
    F --> U{Too Close?}
    U -->|Yes| V[Adjust Position]
    U -->|No| W[Continue]
```

## Data Flow Architecture

```mermaid
graph LR
    A[ScriptableObjects] --> B[GameConfigSO]
    A --> C[StageConfigSO]
    A --> D[EnemyTypeSO]
    A --> E[AttackTypeSO]
    A --> F[AttackUpgradeSO]
    
    B --> G[BeatManager]
    B --> H[GameStateManager]
    
    C --> I[LevelManager]
    C --> J[EnemySpawner]
    C --> K[UpgradeScreenUI]
    
    D --> L[Enemy Instances]
    
    E --> M[AttackManager]
    E --> N[AttackInstance]
    
    F --> O[Upgrade Application]
    
    G --> P[All Beat-Synchronized Systems]
    I --> Q[Experience Tracking]
    J --> R[Enemy Creation]
    M --> S[Attack Spawning]
```

## Editor Script Generation Flow

```mermaid
graph TD
    A[Run GameSetupEditor] --> B[Create Scene Structure]
    
    B --> C[Create GameManager GameObject]
    C --> D[Add Core Components]
    D --> E[BeatManager]
    D --> F[GameStateManager]
    D --> G[LevelManager]
    D --> H[EnemySpawner]
    D --> I[AttackManager]
    
    B --> J[Create Player GameObject]
    J --> K[Add Player Components]
    K --> L[PlayerController]
    K --> M[PlayerHealth]
    K --> N[PlayerAnimator]
    
    B --> O[Create Camera]
    O --> P[Add CameraController]
    
    B --> Q[Create UI Canvas]
    Q --> R[Create HUD Elements]
    Q --> S[Create Upgrade Screen]
    Q --> T[Create Game Over Screen]
    
    B --> U[Create Prefabs]
    U --> V[Enemy Prefabs]
    U --> W[Attack Prefabs]
    U --> X[Collectible Prefabs]
    
    B --> Y[Create ScriptableObject Assets]
    Y --> Z[GameConfig]
    Y --> AA[Stage Configs]
    Y --> AB[Enemy Types]
    Y --> AC[Attack Types]
    Y --> AD[Upgrades]
    
    E --> AE[Link References]
    F --> AE
    G --> AE
    H --> AE
    I --> AE
    L --> AE
    M --> AE
    N --> AE
    P --> AE
    
    AE --> AF[Save Scene]
    AF --> AG[Complete Setup]
```
