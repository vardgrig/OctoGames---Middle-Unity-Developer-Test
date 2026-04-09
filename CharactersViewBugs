# CharactersView Bugs


-----------------------------------------------------------------------------

## Bug 1 — Typo: `[SerializedField]`

```csharp
 // ORIGINAL — typo, Unity ignores this attribute entirely
 [SerializedField] private List<Transform> _characters;
 
 // FIXED
 [SerializeField] private List<Character> _characters = new();
```

The field was never populated from the Inspector — silent
failure.

-----------------------------------------------------------------------------

## Bug 2 — GetComponents (plural) returns an array, not a component

```csharp
 // ORIGINAL — returns Character[], not Character, which is compile error / wrong type
 Character character = characterTransform.gameObject.GetComponents<Character>();
 
 // FIXED — store Character directly, no GetComponent needed at all
```

-----------------------------------------------------------------------------

## Bug 3 — List<T>.Length doesn't exist

```csharp
 // ORIGINAL — List has .Count, not .Length, which is compile error
 _characters.Length
 
 // FIXED
 _characters.Count
```
 
-----------------------------------------------------------------------------

## Bug 4 — Division is inverted (logic error)

```csharp
 // ORIGINAL — this is "count / total", not average
 _characters.Length / totalValue
 
 // FIXED — average = total / count
 totalValue / count
```
 
-----------------------------------------------------------------------------

## Bug 5 — Division by zero

```csharp
 // ORIGINAL — if list is empty, totalValue = 0, which is NaN or exception
 totalValue / count
 
 // FIXED — guard before dividing
```

-----------------------------------------------------------------------------

## Performance Issue 1 — GetComponent called inside a loop every frame

```csharp
 // ORIGINAL — GetComponent is expensive; called N×50 times per second
 
characterTransform.gameObject.GetComponents<Character>();
 
 // FIXED — store Character directly in the list, which is zero GetComponent calls at runtime
 [SerializeField] private List<Character> _characters;
```

-----------------------------------------------------------------------------

## Performance Issue 2 — GetComponent<Text>() called everyframe

```csharp
 // ORIGINAL — another GetComponent per frame
 gameObject.GetComponent<Text>().text = text;
 
 // FIXED — cached once in Awake
 private TMP_Text _text;
 private void Awake() 
 { 
    _text = GetComponent<TMP_Text>();
 }
```

-----------------------------------------------------------------------------

## Performance Issue 3 — FixedUpdate is the wrong place for UI (even Update, if we can use events instead)

```csharp
 // ORIGINAL — FixedUpdate runs at the physics rate (default: 50 times per second)
 // completely unrelated to rendering; UI doesn't need to update this often
 void FixedUpdate() { ... }
 
 // FIXED — event-driven: only refreshes when a character's Value actually changes
 character.ValueChanged += OnCharacterValueChanged;
 private void OnCharacterValueChanged(float _) => RefreshDisplay();
```

-----------------------------------------------------------------------------

## Performance Issue 4 — Debug.Log every FixedUpdate

```csharp
 // ORIGINAL — logging 50 times per second tanks performance in any build
 Debug.Log(text); // inside FixedUpdate
 
 // FIXED — only logs when RefreshDisplay() is called (event-driven)
```