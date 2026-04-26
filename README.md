# ⭐ Star Collector

![Unity](https://img.shields.io/badge/Unity-6000.3.10f1-black?style=flat&logo=unity)
![Language](https://img.shields.io/badge/Language-C%23-blue)
![Platform](https://img.shields.io/badge/Platform-2D%20Top--Down-purple)
![Status](https://img.shields.io/badge/Status-In%20Development-yellow)

**Star Collector** is a 2D top-down arcade game built in Unity 6. The player navigates an open area, collecting stars while evading patrolling enemies. The core challenge is maximising your score before an enemy catches you.

This project demonstrates **clean C# scripting** for Unity, featuring an Object Pool pattern for performance, a state-machine-driven enemy AI, and a decoupled Singleton-based game loop.

---

## 🎮 Gameplay Features

* **Star Collection**: Stars spawn randomly within a defined area. Collect as many as possible to increase your score.
* **Infinite Respawn**: A new star spawns immediately after one is collected, keeping the play area active at all times.
* **Enemy AI**: Enemies patrol waypoints and switch to a chase state when the player enters their detection radius. They return to their patrol route if the player escapes.
* **8-Directional Animation**: Both the player and enemies use a sprite-sheet-based animation system driven by movement angle, with `SpriteRenderer.flipX` to mirror diagonal frames.
* **Game Over**: Contact with an enemy triggers a game over, freezing the player and displaying the final score.
* **Restart**: A single button on the Game Over panel reloads the scene instantly.

---

## 🕹️ Controls

| Key | Action |
| :--- | :--- |
| **W / Up Arrow** | Move Up |
| **S / Down Arrow** | Move Down |
| **A / Left Arrow** | Move Left |
| **D / Right Arrow** | Move Right |

> Diagonal movement is fully supported. The player sprite automatically mirrors to reflect the correct facing direction.

---

## 🛠️ Technical Architecture & Optimizations

### 1. Design Patterns

* **Singleton Pattern**: `GameManager` and `StarObjectPool` use a classic Singleton with duplicate-destruction logic (`Awake` guard), ensuring a single source of truth for game state and pooled objects.
* **Object Pool Pattern (`StarObjectPool.cs`)**: Stars are pre-instantiated into a `Queue<StarCollectible>` at startup. When a star is collected, it is returned to the pool and re-used for the next spawn — eliminating runtime `Instantiate`/`Destroy` calls and their associated GC allocations entirely.
* **State Machine Pattern (`EnemyController.cs`)**: Enemy behaviour is governed by a clean `enum`-based state machine (`Patrolling → Chasing → Returning`), making each state's logic isolated and easy to extend.

### 2. Performance Optimizations

* **Garbage Collection (GC) Reduction**:
    * `Rigidbody2D` and `SpriteRenderer` references are cached in `Awake()`, avoiding any `GetComponent` calls during `Update` or `FixedUpdate`.
    * The Object Pool eliminates per-frame heap allocations from star spawning entirely.
* **Physics-based Movement**: Player and enemy movement uses `Rigidbody2D.linearVelocity` set inside `FixedUpdate`, keeping physics calculations decoupled from the render loop.
* **`sqrMagnitude` over `magnitude`**: Movement checks use `sqrMagnitude > 0.01f` to determine if an entity is moving, avoiding a `Mathf.Sqrt` call every frame.

### 3. Core Scripts Overview

| Script | Responsibility |
| :--- | :--- |
| `GameManager.cs` | Singleton game loop; tracks score, manages Game Over state, handles scene reload. |
| `PlayerController.cs` | Physics-based 8-directional movement using the New Input System; drives directional sprite animation. |
| `EnemyController.cs` | State-machine AI (Patrol / Chase / Return); mirrors player animation system; kills player on contact. |
| `StarCollectible.cs` | Collision detection for collection; notifies `StarSpawnArea` and increments score via `GameManager`. |
| `StarSpawnArea.cs` | Manages the active star list; triggers pool retrieval and random placement within a `Collider2D` bounds. |
| `StarObjectPool.cs` | Pre-warms a `Queue`-based object pool of `StarCollectible` instances to avoid runtime allocations. |
| `CameraFollow.cs` | Smoothly interpolates the camera to the player's position using `Vector3.Lerp` in `LateUpdate`. |

---

## 🚀 How to Run (Unity Editor)

1. Clone the repository:
