using System;
using UnityEngine;

namespace LOGIYGames.Abilities
{
    public enum AbilityEffectTarget
    {
        Caster,
        FirstTarget,
        AllTargets
    }

    [Serializable]
    public sealed class ApplyEffectModuleFactory : AbilityModuleFactory
    {
        public AbilityEffectDefinition Effect;
        public AbilityEventType Trigger = AbilityEventType.AbilityStarted;
        public bool FilterByPhase;
        public AbilityPhase Phase = AbilityPhase.Casting;
        public AbilityEffectTarget Target = AbilityEffectTarget.Caster;
        public AbilityResourceScope ResourceScope = AbilityResourceScope.Ability;

        public override AbilityModuleRuntime CreateRuntime(AbilityContext context)
            => new ApplyEffectModuleRuntime(this, context);
    }

    public sealed class ApplyEffectModuleRuntime : AbilityModuleRuntime
    {
        private readonly ApplyEffectModuleFactory _factory;
        public ApplyEffectModuleRuntime(ApplyEffectModuleFactory factory, AbilityContext context) : base(context)
            => _factory = factory;

        public override void OnEvent(in AbilityEvent abilityEvent)
        {
            if (_factory.Effect == null || abilityEvent.Type != _factory.Trigger ||
                (_factory.FilterByPhase && abilityEvent.Phase != _factory.Phase)) return;

            switch (_factory.Target)
            {
                case AbilityEffectTarget.Caster:
                    Apply(Context.Caster);
                    break;
                case AbilityEffectTarget.FirstTarget:
                    if (Context.TargetingData?.Targets != null && Context.TargetingData.Targets.Count > 0)
                        Apply(Context.TargetingData.Targets[0]);
                    break;
                case AbilityEffectTarget.AllTargets:
                    if (Context.TargetingData?.Targets == null) return;
                    foreach (GameObject target in Context.TargetingData.Targets)
                        Apply(target);
                    break;
            }
        }

        private void Apply(GameObject target)
        {
            if (target == null) return;
            MonoBehaviour[] components = target.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour component in components)
            {
                if (component is not IAbilityEffectReceiver receiver) continue;
                AbilityEffectHandle handle = receiver.ApplyEffect(
                    _factory.Effect,
                    new AbilityEffectContext(Context, target));

                if (handle != null && _factory.ResourceScope != AbilityResourceScope.Detached &&
                    Context.Services != null)
                {
                    Context.Services.Resources.Register(handle, _factory.ResourceScope,
                        _factory.ResourceScope == AbilityResourceScope.State ? Context.Services.CurrentStateId : 0);
                }
                return;
            }
        }
    }
}