# OctoGames Middle Unity Developer Test

-----------------------------------------------------------------------------

### Programs and versions used for development
- Rider 2024.2.8
- Unity 6000.4.1f1

-----------------------------------------------------------------------------

## [QuestionsAndAnswers](QuestionsAndAnswers)

## [UnityPopupPrefabQuestion](UnityPopupPrefabQuestion)

## [CharactersViewBugs](CharactersViewBugs)

-----------------------------------------------------------------------------


## Optional Bonus Questions and Answers

#### How would you scale these systems for larger projects?
- #### For the popup system, I would implement an object pooling mechanism to reuse popup instances instead of creating/destroying them frequently, which improves performance. I would also design a more flexible data structure for popup content, allowing for different types of popups (e.g., notifications, tooltips) with varying layouts and behaviors.
- #### For the character view, I would implement a more robust data binding system, possibly using a Model-View-ViewModel (MVVM) pattern, to ensure that UI updates are efficient and decoupled from the underlying data. I would also consider implementing a more modular UI architecture, allowing for easier maintenance and scalability as the number of characters and UI elements grows.
- #### For both systems, I would also implement a more comprehensive event system to handle interactions and updates, ensuring that the UI remains responsive and efficient even as the complexity of the project increases.
- #### Also, I would consider implementing DI (Zenject or similar) to manage dependencies and improve testability, remove singletons, and make the codebase more modular and maintainable.

#### How would designers interact with this code? 
- #### For the popup system, designers would interact with the code through ScriptableObjects that define the content and behavior of each popup type. They could create new popup configurations without needing to modify any code, allowing for easy iteration and customization.\

#### How would you profile or debug performance issues? 
- #### I would use Unity's built-in Profiler to identify any bottlenecks in the popup system, such as excessive instantiation or inefficient UI updates. I would look for spikes in CPU usage when popups are created or updated and analyze the call stack to pinpoint the source of the issue. Additionally, I would implement logging and debugging tools to track the lifecycle of popups and identify any potential memory leaks or performance issues related to object pooling.

-----------------------------------------------------------------------------

## AI Usage (Github Copilot CLI)

AI was used to generate tester classes for popup system, save/load system and entity system. 
The AI-generated code was reviewed and modified by me to ensure it met the requirements of the test.
Additionally, it helped me find encryption bug related to the save/load system, which were XORing the encrypted data with the key instead of using it as a key for a proper encryption algorithm.
It suggested using AES encryption, which I implemented and tested to ensure the save data is properly encrypted and decrypted.