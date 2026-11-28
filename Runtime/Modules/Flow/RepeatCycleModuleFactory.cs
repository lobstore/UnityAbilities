using System;
using UnityEngine;

namespace LOGIYGames.Abilities
{
    [Serializable]
    public sealed class RepeatCycleModuleFactory : AbilityFlowModuleFactory
    {
        [Min(0)] public int RepeatCount = 1;
        public AbilityPhase RepeatFrom = AbilityPhase.Targeting;
        public override AbilityFlowModuleRuntime CreateRuntime(AbilityContext context)
            => new RepeatCycleModuleRuntime(this, context);
    }

    public sealed class RepeatCycleModuleRuntime : AbilityFlowModuleRuntime
    {
        private readonly AbilityPhase _repeatFrom;
        private int _repeatsRemaining;

        public RepeatCycleModuleRuntime(RepeatCycleModuleFactory factory, AbilityContext context) : base(context)
        {
            _repeatFrom = factory.RepeatFrom;
            _repeatsRemaining = Mathf.Max(0, factory.RepeatCount);
        }

        public override bool TryResolveNextPhase(out AbilityPhase nextPhase)
        {
            if (_repeatsRemaining <= 0 || _repeatFrom == AbilityPhase.None || _repeatFrom == AbilityPhase.Exiting)
            {
                nextPhase = AbilityPhase.None;
                return false;
            }
            _repeatsRemaining--;
            nextPhase = _repeatFrom;
            return true;
        }
    }
}