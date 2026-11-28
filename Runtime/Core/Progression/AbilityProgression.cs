using System;
using System.Collections.Generic;
using UnityEngine;

namespace LOGIYGames.Abilities
{
    [Serializable]
    public sealed class AbilityProgressionDefinition
    {
        [Min(1)] public int MaxLevel = 1;
        [SerializeReference] public List<AbilityLevelDefinition> Levels = new();
        public AbilityLevelDefinition GetLevel(int level)
        {
            if (Levels == null) return null;
            for (int i = 0; i < Levels.Count; i++)
                if (Levels[i] != null && Levels[i].Level == level) return Levels[i];
            return null;
        }
    }

    [Serializable]
    public sealed class AbilityLevelDefinition
    {
        [Min(1)] public int Level = 1;
        [Min(0f)] public float DamageMultiplier = 1f;
        [Min(0f)] public float CooldownMultiplier = 1f;
        [SerializeReference] public List<AbilityModuleFactory> Modules = new();
    }

    [Serializable]
    public sealed class AbilityProgressionState
    {
        [SerializeField, Min(1)] private int _level = 1;
        public int Level => Mathf.Max(1, _level);

        public void SetLevel(AbilityDefinition definition, int level)
        {
            int maxLevel = definition != null && definition.Progression != null
                ? Mathf.Max(1, definition.Progression.MaxLevel) : 1;
            _level = Mathf.Clamp(level, 1, maxLevel);
        }

        public bool TryUpgrade(AbilityDefinition definition, AbilityContext activeContext = null)
        {
            if (definition == null) return false;
            int maxLevel = definition.Progression != null
                ? Mathf.Max(1, definition.Progression.MaxLevel) : 1;
            if (_level >= maxLevel) return false;

            int previousLevel = Level;
            _level++;
            if (activeContext != null && activeContext.Ability != null &&
                activeContext.Ability.Data == definition)
            {
                activeContext.AbilityLevel = Level;
                activeContext.ApplyProgressionParameters();
                activeContext.Services?.OnAbilityLevelChanged(previousLevel, Level);
            }
            return true;
        }
    }

    [Serializable]
    public sealed class AbilityParameter
    {
        public string Key;
        public float FloatValue;
        public int IntValue;
        public bool BoolValue;
    }

    [Serializable]
    public sealed class AbilityParameters
    {
        [SerializeField] private List<AbilityParameter> _values = new();

        public float GetFloat(string key, float defaultValue = 0f)
        {
            AbilityParameter value = Find(key);
            return value != null ? value.FloatValue : defaultValue;
        }
        public int GetInt(string key, int defaultValue = 0)
        {
            AbilityParameter value = Find(key);
            return value != null ? value.IntValue : defaultValue;
        }
        public bool GetBool(string key, bool defaultValue = false)
        {
            AbilityParameter value = Find(key);
            return value != null ? value.BoolValue : defaultValue;
        }
        public void SetFloat(string key, float value) => GetOrCreate(key).FloatValue = value;
        public void SetInt(string key, int value) => GetOrCreate(key).IntValue = value;
        public void SetBool(string key, bool value) => GetOrCreate(key).BoolValue = value;

        private AbilityParameter Find(string key)
        {
            if (string.IsNullOrEmpty(key)) return null;
            return _values.Find(value => value != null && value.Key == key);
        }
        private AbilityParameter GetOrCreate(string key)
        {
            AbilityParameter value = Find(key);
            if (value != null) return value;
            value = new AbilityParameter { Key = key };
            _values.Add(value);
            return value;
        }
    }
}