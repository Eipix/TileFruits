# TileFruits — Mahjong Match-3 Puzzle

Casual Tile Match puzzle game with an infinite level loop and a custom ScriptableObject level editor built for Unity.

> **[▶ Play WebGL Demo](https://eipix.github.io/TileFruits/)**

---

## Gameplay

* **Tile Match 3:** Selecting unlocked tiles moves them to the bottom tray. Matching 3 identical fruits clears them from the board.
* **Layer Occlusion:** Tiles in lower layers remain locked until all overlapping tiles above them are removed.
* **Infinite Level Loop:** Level layouts cycle endlessly using a shuffle algorithm that strictly preserves tile type parity, guaranteeing level solvability.

---

https://github.com/user-attachments/assets/d5aa81b5-4435-4bb7-8bad-92aa7200d01d


## Custom Level Editor

Custom inspector for `CustomLevel` ScriptableObjects to design and validate layouts directly in the Unity Inspector:

* **Layer Grid:** Interactive visualization and placement of tiles across spatial coordinates and depth layers.
* **Editing Tools:** Modes for placing, removing, and modifying tile types on the grid.
* **Solvability Validation:** Automated checks for total tile count parity, fruit type distribution, and unreachable tile detection.
* **Instant Serialization:** Real-time data updates directly in the asset, ready for immediate Play Mode testing.
---

https://github.com/user-attachments/assets/a633adab-8392-4ead-b66d-772ed5591037


## Tech Stack & Architecture

* **Unity 6**
* **Zenject:** Dependency injection for loose coupling between core gameplay systems, UI, and audio without relying on singletons.
* **UniTask:** Zero-allocation async/await pipelines for tile animations, board initialization, and level transitions with cancellation support.
* **DOTween:** Procedural animations for UI, tile flight, and match feedback.
* **Playgama Bridge SDK:** Web platform integration handling platform lifecycle events, storage, and monetization.
* **ScriptableObjects:** Data-driven level configurations and balance presets.

---
