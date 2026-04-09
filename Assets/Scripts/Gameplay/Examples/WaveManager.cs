using UnityEngine;

namespace Gameplay.Examples
{
    /// <summary>
    /// Example consumer that checks how many active enemies remain.
    /// </summary>
    public class WaveManager : MonoBehaviour
    {
        private void Update()
        {
            var active = EntityRegistry.Instance.GetActive();

            if (active.Count == 0)
            {
                Debug.Log("[WaveManager] All enemies cleared — spawn next wave.");
                enabled = false;
            }
        }
    }
}
