﻿using JetBrains.Annotations;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.Localization;
using Kingmaker.Localization.Shared;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.UnitLogic.Mechanics.Properties;
using Kingmaker.Utility;
using LevelableAivu.Config;

using ModKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelableAivu
{
    public static class Helpers
    {
        
        

        public static T Create<T>(Action<T> init = null) where T : new()
        {
            var result = new T();
            init?.Invoke(result);
            return result;
        }

        public static T CreateBlueprint<T>([NotNull] string name, Action<T> init = null) where T : SimpleBlueprint, new()
        {
            var result = new T
            {
                name = name,
                AssetGuid = ModSettings.Blueprints.GetGUID(name)
            };
            Resources.AddBlueprint(result);
            init?.Invoke(result);
            return result;
        }


       
        public static ContextRankConfig CreateContextRankConfig(ContextRankBaseValueType baseValueType = ContextRankBaseValueType.CasterLevel,
    ContextRankProgression progression = ContextRankProgression.AsIs,
    AbilityRankType type = AbilityRankType.Default,
    int? min = null, int? max = null, int startLevel = 0, int stepLevel = 0,
    bool exceptClasses = false, StatType stat = StatType.Unknown,
    BlueprintUnitProperty customProperty = null,
    BlueprintCharacterClass[] classes = null, BlueprintArchetype[] archetypes = null, BlueprintArchetype archetype = null,
    BlueprintFeature feature = null, BlueprintFeature[] featureList = null,
    (int, int)[] customProgression = null)
        {
            var config = new ContextRankConfig()
            {
                m_Type = type,
                m_BaseValueType = baseValueType,
                m_Progression = progression,
                m_UseMin = min.HasValue,
                m_Min = min.GetValueOrDefault(),
                m_UseMax = max.HasValue,
                m_Max = max.GetValueOrDefault(),
                m_StartLevel = startLevel,
                m_StepLevel = stepLevel,
                m_Feature = feature.ToReference<BlueprintFeatureReference>(),
                m_ExceptClasses = exceptClasses,
                m_CustomProperty = customProperty.ToReference<BlueprintUnitPropertyReference>(),
                m_Stat = stat,
                m_Class = classes == null ? Array.Empty<BlueprintCharacterClassReference>() : classes.Select(c => c.ToReference<BlueprintCharacterClassReference>()).ToArray(),
                Archetype = archetype.ToReference<BlueprintArchetypeReference>(),
                m_AdditionalArchetypes = archetypes == null ? Array.Empty<BlueprintArchetypeReference>() : archetypes.Select(c => c.ToReference<BlueprintArchetypeReference>()).ToArray(),
                m_FeatureList = featureList == null ? Array.Empty<BlueprintFeatureReference>() : featureList.Select(c => c.ToReference<BlueprintFeatureReference>()).ToArray()
            };
#if false
            var config = Helpers.Create<ContextRankConfig>(bp => {
                bp.m_Type = type;
                bp.m_BaseValueType = baseValueType;
                bp.m_Progression = progression;
                bp.m_UseMin = min.HasValue;
                bp.m_Min = min.GetValueOrDefault();
                bp.m_UseMax = max.HasValue;
                bp.m_Max = max.GetValueOrDefault();
                bp.m_StartLevel = startLevel;
                bp.m_StepLevel = stepLevel;
                bp.m_Feature = feature.ToReference<BlueprintFeatureReference>();
                bp.m_ExceptClasses = exceptClasses;
                bp.m_CustomProperty = customProperty.ToReference<BlueprintUnitPropertyReference>();
                bp.m_Stat = stat;
                bp.m_Class = classes == null ? Array.Empty<BlueprintCharacterClassReference>() : classes.Select(c => c.ToReference<BlueprintCharacterClassReference>()).ToArray();
                bp.Archetype = archetype.ToReference<BlueprintArchetypeReference>();
                bp.m_AdditionalArchetypes = archetypes == null ? Array.Empty<BlueprintArchetypeReference>() : archetypes.Select(c => c.ToReference<BlueprintArchetypeReference>()).ToArray();
                bp.m_FeatureList = featureList == null ? Array.Empty<BlueprintFeatureReference>() : featureList.Select(c => c.ToReference<BlueprintFeatureReference>()).ToArray();
            });
#endif
            return config;
        }

        public static LevelEntry LevelEntry(int level, BlueprintFeatureBase feature)
        {
            return new LevelEntry
            {
                Level = level,
                Features = {
                    feature
                }
            };
        }

        public static LevelEntry CreateLevelEntry(int level, params BlueprintFeatureBase[] features)
        {
            LevelEntry levelEntry = new LevelEntry();
            levelEntry.Level = level;
            features.ForEach(f => levelEntry.Features.Add(f));
            return levelEntry;
        }

        public static ContextRankConfig CreateContextRankConfig(Action<ContextRankConfig> init)
        {
            var config = CreateContextRankConfig();
            init?.Invoke(config);
            return config;
        }

        public static FastRef<T, S> CreateFieldSetter<T, S>(string name)
        {
            return new FastRef<T, S>(HarmonyLib.AccessTools.FieldRefAccess<T, S>(HarmonyLib.AccessTools.Field(typeof(T), name)));
            //return new FastSetter<T, S>(HarmonyLib.FastAccess.CreateSetterHandler<T, S>(HarmonyLib.AccessTools.Field(typeof(T), name)));
        }
        public static FastRef<T, S> CreateFieldGetter<T, S>(string name)
        {
            return new FastRef<T, S>(HarmonyLib.AccessTools.FieldRefAccess<T, S>(HarmonyLib.AccessTools.Field(typeof(T), name)));
            //return new FastGetter<T, S>(HarmonyLib.FastAccess.CreateGetterHandler<T, S>(HarmonyLib.AccessTools.Field(typeof(T), name)));
        }

        public static ContextValue CreateContextValueRank(AbilityRankType value = AbilityRankType.Default) => value.CreateContextValue();
        public static ContextValue CreateContextValue(this AbilityRankType value)
        {
            return new ContextValue() { ValueType = ContextValueType.Rank, ValueRank = value };
        }
        public static ContextValue CreateContextValue(this AbilitySharedValue value)
        {
            return new ContextValue() { ValueType = ContextValueType.Shared, ValueShared = value };
        }
        public static ActionList CreateActionList(params GameAction[] actions)
        {
            if (actions == null || actions.Length == 1 && actions[0] == null) actions = Array.Empty<GameAction>();
            return new ActionList() { Actions = actions };
        }
    }
    public delegate ref S FastRef<T, S>(T source = default);
    public delegate void FastSetter<T, S>(T source, S value);
    public delegate S FastGetter<T, S>(T source);
    public delegate object FastInvoke(object target, params object[] paramters);
}
