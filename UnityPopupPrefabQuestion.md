# Unity Components Question

-----------------------------------------------------------------------------

## Question

Which **Unity components** would you use to build the popup prefab, and **why**?

-----------------------------------------------------------------------------

## Answer

### Core Structure

| Component                       | Why                                                                                        |
|---------------------------------|--------------------------------------------------------------------------------------------|
| Canvas (Screen Space - Overlay) | Renders on top of everything; no camera dependency                                         |
| CanvasGroup                     | Single alpha/interactable toggle for fade animations and blocking input during transitions |
| RectTransform                   | All UI positioning — anchors let it scale across resolutions                               |

### Content

| Component              | Why                                                                           |
|------------------------|-------------------------------------------------------------------------------|
| TextMeshPro (TMP_Text) | For title + body — better rendering, rich text, no blurry text at any scale   |
| Image                  | Background panel; supports 9-slice sprites so it stretches without distortion |
| VerticalLayoutGroup    | Auto-stacks title, body, button row — no manual positioning                   |
| ContentSizeFitter      | Makes the panel grow/shrink to fit dynamic text content                       |

### Buttons

| Component             | Why                                                     |
|-----------------------|---------------------------------------------------------|
| Button                | Built-in click handling, interactable state, navigation |
| HorizontalLayoutGroup | Evenly spaces 1–5 buttons automatically                 |
| TMP_Text (on button)  | Button label                                            |


**Why this matters in context of our popup system:**

- **CanvasGroup** is what makes `PopupManager.Hide()` clean: `alpha = 0` + `interactable = false`
  instead of toggling 10 children
- **LayoutGroup** + **ContentSizeFitter** means designers can change button count or body text length
  without breaking the layout — no hardcoded positions