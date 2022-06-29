using BlueprintCore.Blueprints.Configurators.Items;
using BlueprintCore.Blueprints.Configurators.Items.Equipment;
using BlueprintCore.Utils;
using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Items.Components;
using Kingmaker.Blueprints.Items.Equipment;
using Kingmaker.Blueprints.JsonSystem;
using LevelableAivu.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityModManagerNet;

namespace LevelableAivu
{
    class FixItemAccess
    {
        [HarmonyPatch(typeof(BlueprintsCache), "Init")]
    
        static class BlueprintsCache_Init_Patch
        {
            [HarmonyPriority(Priority.Last)]
            static void Postfix()
            {
                Main.Log($"Patching Glove Slot Items");
                string[] pads = new string[] { "6497e0f07a3b49d8b50a33f27f86d8d2",
"2ef8b0290fafaf647941af8c25a2d7be",
"b598f24088bc49b9af70dd13969e1eee",
"bfd3b4393040f854598dbdfff0feac79",
"6fbf1d1001da4d04f907794c50262ae9",
"efe72fa3d5164cdc93135c9e3cd91a22",
"c8ad87f3fc49ccd43a5560acb271e968",
"d657c55461344f748d76a8a03e6bdbd9" };

                var artiGloves = BlueprintTool.Get<BlueprintItemEquipmentGloves>("3230a07e17594084f88e770480277de2");
                var artiGlovesRes = artiGloves.Components.OfType<EquipmentRestrictionHasAnyClassFromList>().FirstOrDefault();
                var havocClassRef = BlueprintTool.GetRef<BlueprintCharacterClassReference>("c0ae648600bb416b81e0b3e705f264c6");
                var animalcompanionclass = BlueprintTool.GetRef<BlueprintCharacterClassReference>("26b10d4340839004f960f9816f6109fe");
                if (artiGlovesRes != null)
                {
                    artiGlovesRes.m_Classes = artiGlovesRes.m_Classes.AppendToArray(havocClassRef);
                }
                Main.LogPatch($"Blocked Aivu Access:", artiGloves);
               

                var refList = new List<BlueprintCharacterClassReference>() { havocClassRef, animalcompanionclass };
                if (UnityModManager.FindMod("ExpandedContent") != null)
                {
                    try
                    {
                        var drake = BlueprintTool.GetRef<BlueprintCharacterClassReference>("557496bca2644c2d93c4a88b2b546430");
                        if (drake != null)
                        {
                            refList.Add(drake);
                            Main.Log("Added Expanded Content Drake to pet extension list");
                        }
                    }
                    catch
                    {

                    }
                }

                foreach(string s in pads)
                {
                    FixPads(BlueprintTool.Get<BlueprintItemEquipmentGloves>(s));
                }

                void FixPads(BlueprintItemEquipmentGloves gloves)
                {
                    var anyType = gloves.Components.OfType<EquipmentRestrictionHasAnyClassFromList>().FirstOrDefault();
                    if (anyType!= null)
                    {
                        anyType.m_Classes = anyType.m_Classes.AppendToArray(havocClassRef);
                        Main.LogPatch("Fixed pet access by patching existing multi-class restriction", gloves);
                    }
                    else
                    {
                        ItemEquipmentGlovesConfigurator.For(gloves).RemoveComponents(x => x is EquipmentRestrictionClass).AddComponent<EquipmentRestrictionHasAnyClassFromList>(x =>
                        {

                           
                            x.m_Classes = refList.ToArray();
                        }).Configure();
                        Main.LogPatch("Fixed pet access by swapping in new multiclass restriction", gloves);

                    }
                    
                }

            }
        }
    }
}
