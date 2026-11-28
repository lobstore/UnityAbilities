using UnityEngine;

namespace LOGIYGames.Abilities
{
    public sealed class AbilityObjectContext
    {
        public AbilityContext SourceContext { get; }
        public GameObject Caster { get; }
        public AbilityDefinition SourceAbility { get; }
        public AbilityTargetData TargetingData { get; }
        public int Iteration { get; }
        public int AbilityLevel { get; }

        public AbilityObjectContext(AbilityContext sourceContext)
        {
            SourceContext = sourceContext;
            Caster = sourceContext != null ? sourceContext.Caster : null;
            SourceAbility = sourceContext != null && sourceContext.Ability != null ? sourceContext.Ability.Data : null;
            TargetingData = sourceContext != null ? sourceContext.TargetingData : null;
            Iteration = sourceContext != null ? sourceContext.CurrentIteration : 0;
            AbilityLevel = sourceContext != null ? sourceContext.AbilityLevel : 1;
        }
    }
}