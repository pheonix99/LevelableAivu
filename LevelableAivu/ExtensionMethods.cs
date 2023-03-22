using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.Facts;
using Kingmaker.ElementsSystem;
using Kingmaker.Localization;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using LevelableAivu.Utilities;
using UnityEngine;
using static Kingmaker.Blueprints.Classes.Prerequisites.Prerequisite;
using BlueprintCore.Utils;

namespace LevelableAivu{
    static class ExtentionMethods
    {
       
       
       
        

        
        public static T[] AppendToArray<T>(this T[] array, T value)
        {
            var len = array.Length;
            var result = new T[len + 1];
            Array.Copy(array, result, len);
            result[len] = value;
            return result;
        }

       

       

     

        public static T[] RemoveFromArray<T>(this T[] array, T value)
        {
            var list = array.ToList();
            return list.Remove(value) ? list.ToArray() : array;
        }


        public static void AddPrerequisite<T>(this BlueprintFeature obj, T prerequisite) where T : Prerequisite
        {
            obj.AddComponent(prerequisite);
            switch (prerequisite)
            {
                case PrerequisiteFeature p:
                    var feature = p.Feature;
                    if (feature.IsPrerequisiteFor == null) { feature.IsPrerequisiteFor = new List<BlueprintFeatureReference>(); }
                    if (!feature.IsPrerequisiteFor.Contains(obj.ToReference<BlueprintFeatureReference>()))
                    {
                        feature.IsPrerequisiteFor.Add(obj.ToReference<BlueprintFeatureReference>());
                    }
                    break;
                case PrerequisiteFeaturesFromList p:
                    var features = p.Features;
                    features.ForEach(f => {
                        if (f.IsPrerequisiteFor == null) { f.IsPrerequisiteFor = new List<BlueprintFeatureReference>(); }
                        if (!f.IsPrerequisiteFor.Contains(obj.ToReference<BlueprintFeatureReference>()))
                        {
                            f.IsPrerequisiteFor.Add(obj.ToReference<BlueprintFeatureReference>());
                        }
                    });
                    break;
                default:
                    break;
            }
        }

       


    
        

      

        

        public static void AddComponent(this BlueprintScriptableObject obj, BlueprintComponent component)
        {
            obj.SetComponents(obj.ComponentsArray.AppendToArray(component));
        }

        public static void AddComponent<T>(this BlueprintScriptableObject obj, Action<T> init = null) where T : BlueprintComponent, new()
        {
            obj.SetComponents(obj.ComponentsArray.AppendToArray(Helpers.Create(init)));
        }

        

        public static void RemoveComponents<T>(this BlueprintScriptableObject obj, Predicate<T> predicate) where T : BlueprintComponent
        {
            var compnents_to_remove = obj.GetComponents<T>().ToArray();
            foreach (var c in compnents_to_remove)
            {
                if (predicate(c))
                {
                    obj.SetComponents(obj.ComponentsArray.RemoveFromArray(c));
                }
            }
        }

     
       

        public static void SetComponents(this BlueprintScriptableObject obj, params BlueprintComponent[] components)
        {
            // Fix names of components. Generally this doesn't matter, but if they have serialization state,
            // then their name needs to be unique.
            var names = new HashSet<string>();
            foreach (var c in components)
            {
                if (string.IsNullOrEmpty(c.name))
                {
                    c.name = $"${c.GetType().Name}";
                }
                if (!names.Add(c.name))
                {
                    String name;
                    for (int i = 0; !names.Add(name = $"{c.name}${i}"); i++) ;
                    c.name = name;
                }
            }
            obj.ComponentsArray = components;
            obj.OnEnable(); // To make sure components are fully initialized
        }

       

       
       

        public static void SetName(this BlueprintUnitFact feature, String name)
        {
            feature.m_DisplayName = LocalizationTool.CreateString(feature.name + ".Name", name,false);
        }

        

       
        public static void SetDescription(this BlueprintUnitFact feature, String description)
        {
            var taggedDescription = DescriptionTools.TagEncyclopediaEntries(description);
            feature.m_Description = LocalizationTool.CreateString(feature.name + ".Description", taggedDescription, false);
        }

        


      
        


        

    }
}
