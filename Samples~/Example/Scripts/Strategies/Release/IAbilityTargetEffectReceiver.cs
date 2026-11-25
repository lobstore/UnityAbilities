namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Implement on a target that can receive an ability effect.
    /// The release strategy discovers this interface on the selected target and its parents.
    /// </summary>
    public interface IAbilityTargetEffectReceiver
    {
        /// <summary>Applies the ability represented by the supplied context to this receiver.</summary>
        void ApplyAbility(AbilityContext context);
    }
}
