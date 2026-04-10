# Questions and Answers

-----------------------------------------------------------------------------

##  Coding Principles (Short Answer)
Describe two coding principles or practices you consider most important when working on real Unity projects that mix:

* 3D gameplay
* UI systems
* Iteration by designers

Explain why they matter and where you apply them.

-----------------------------------------------------------------------------


## Answer

### Separation of Concerns — decouple gameplay data from UI (MVC/MVVM)

Gameplay logic should never know about UI, and UI should
never poll gameplay state. Instead, gameplay exposes
events or interfaces, so UI listens and reacts only when
data changes.

**Why it matters:** Polling (like FixedUpdate scanning all
characters) wastes CPU and creates tight coupling.
Event-driven UI (like character.ValueChanged +=
RefreshDisplay) updates exactly when needed and lets you
replace the UI layer without touching gameplay code.

**Where I apply it:** Any HUD, status panel, or overlay that
reflects live game state — character stats, health bars,
inventories.

-----------------------------------------------------------------------------

### Data-driven design for designer iteration

Expose tunable data (popup content, save settings, enemy
stats, UI layout rules) through ScriptableObjects or external data rather
than hardcoded values or MonoBehaviour Inspector fields.

**Why it matters:** Designers can create variants, tweak
values, and swap configurations without touching prefabs
or code. It also makes systems reusable across scenes and
projects — one script, many SO configs.

**Where I apply it:** Anywhere a non-programmer needs to
author or iterate on content independently.

-----------------------------------------------------------------------------


## Honorable Mentions )))

- **Single Responsibility Principle** — each class should have one reason to change, making code easier to maintain and test.
- **Event-Driven Architecture** — using events to decouple systems and avoid tight coupling, especially for UI updates.
- **Object Pooling** — for performance, especially in UI elements that are frequently created/destroyed, like inventory slots or damage popups.
