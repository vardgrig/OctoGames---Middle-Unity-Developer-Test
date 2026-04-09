namespace Gameplay
{
    /// <summary>
    /// Represents any trackable gameplay entity in the scene.
    /// </summary>
    public interface IGameplayEntity
    {
        /// <summary>Stable unique identifier for this entity</summary>
        string EntityId { get; }

        /// <summary>
        /// True when this entity should be considered active by game systems.
        /// </summary>
        bool IsActive { get; }

        /// <summary>Fired when IsActive changes, so systems can react without polling.</summary>
        event System.Action<IGameplayEntity> OnActiveStateChanged;
    }
}
