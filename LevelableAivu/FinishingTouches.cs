using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Blueprints.Root;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UI.MVVM._VM.CharGen;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Class.LevelUp;
using LevelableAivu.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelableAivu
{
    class FinishingTouches
    {
        [HarmonyPatch(typeof(BlueprintsCache), "Init")]
        [HarmonyPriority(Priority.Last)]
        static class BlueprintsCache_Init_Patch
        {
            static bool Initialized;
            [HarmonyPriority(Priority.VeryLow)]
            static void Postfix()
            {
                if (ModSettings.Settings.settings.GroupIsDisabled())
                    return;
                else
                {
                    AlterSpellBook();
                    AlterCompanionClasses();
                    AlterBarding();
                }




            }

            private static void AlterCompanionClasses()
            {
                BlueprintFeature AivuUsesMythicXPNow = Resources.GetModBlueprint<BlueprintFeature>("AivuUsesMythicXP");
                BlueprintRoot root = Resources.GetBlueprint<BlueprintRoot>("2d77316c72b9ed44f888ceefc2a131f6");
                
                foreach (BlueprintCharacterClass c in root.Progression.m_PetClasses)
                {
                    if (!c.name.Contains("Havoc"))
                    {
                        c.AddComponent(Helpers.Create<PrerequisiteNoFeature>(x =>
                         {
                             x.Group = Prerequisite.GroupType.All;
                             x.m_Feature = AivuUsesMythicXPNow.ToReference<BlueprintFeatureReference>();
                         }));
                    }

                }


            }

            static void AlterSpellBook()
            {


                BlueprintSpellList clericList = Resources.GetBlueprint<BlueprintSpellList>("8443ce803d2d31347897a3d85cc32f53");
                BlueprintSpellList bardList = Resources.GetBlueprint<BlueprintSpellList>("25a5013493bdcf74bb2424532214d0c8");
                BlueprintSpellList HavocDragonList = Resources.GetModBlueprint<BlueprintSpellList>("HavocDragonSpellList");
                for (int i = 0; i < 10; i++)
                {

                    List<BlueprintAbility> clericSpells = clericList.GetSpells(i);
                    List<BlueprintAbility> bardSpells = bardList.GetSpells(i);
                    SpellLevelList havocDragonSpells = new SpellLevelList(i);
                    foreach (BlueprintAbility s in clericSpells)
                    {
                       
                        if (!havocDragonSpells.m_Spells.Contains(s.ToReference<BlueprintAbilityReference>()) && !HavocDragonList.Contains(s))
                        {
                            havocDragonSpells.m_Spells.Add(s.ToReference<BlueprintAbilityReference>());
                            s.AddComponent(Helpers.Create<SpellListComponent>(x =>
                            {
                                x.m_SpellList = HavocDragonList.ToReference<BlueprintSpellListReference>();
                                x.SpellLevel = i;

                            }));
                            
                        }
                    }
                    if (ModSettings.Settings.settings.IsEnabled("AddBardSpellsToList"))
                    {
                        foreach (BlueprintAbility s in bardSpells)
                        {
                            
                            if (!havocDragonSpells.m_Spells.Contains(s.ToReference<BlueprintAbilityReference>()) && !HavocDragonList.Contains(s))
                            {
                                havocDragonSpells.m_Spells.Add(s.ToReference<BlueprintAbilityReference>());
                                s.AddComponent(Helpers.Create<SpellListComponent>(x =>
                                {
                                    x.m_SpellList = HavocDragonList.ToReference<BlueprintSpellListReference>();
                                    x.SpellLevel = i;

                                }));
                                
                            }


                        }
                    }
                    HavocDragonList.SpellsByLevel = HavocDragonList.SpellsByLevel.AppendToArray(havocDragonSpells);
                }
            }

            static void AlterBarding()
            {
                if (ModSettings.Settings.settings.IsDisabled("BardingForAivu"))
                    return;

                BlueprintCharacterClass HavocDragonAdded = Resources.GetModBlueprint<BlueprintCharacterClass>("HavocDragonClass");

                BlueprintFeature LightBardingProfLoaded = Resources.GetBlueprint<BlueprintFeature>("c62ba548b1a34b94b9802925b35737c2");

                BlueprintFeature MediumBardingProfLoaded = Resources.GetBlueprint<BlueprintFeature>("7213b7bd026d4414da2308df23715d8f");

                BlueprintFeature HeavyBardingProfLoaded = Resources.GetBlueprint<BlueprintFeature>("aed0b33e17a3b3d44a718852e87305bd");




                LightBardingProfLoaded.AddPrerequisite(Helpers.Create<PrerequisiteClassLevel>(x =>
                {
                    x.m_CharacterClass = HavocDragonAdded.ToReference<BlueprintCharacterClassReference>();
                    x.Group = Prerequisite.GroupType.Any;

                }));


                MediumBardingProfLoaded.AddPrerequisite(Helpers.Create<PrerequisiteClassLevel>(x =>
                {
                    x.m_CharacterClass = HavocDragonAdded.ToReference<BlueprintCharacterClassReference>();
                    x.Group = Prerequisite.GroupType.Any;

                }));
                HeavyBardingProfLoaded.AddPrerequisite(Helpers.Create<PrerequisiteClassLevel>(x =>
                {
                    x.m_CharacterClass = HavocDragonAdded.ToReference<BlueprintCharacterClassReference>();
                    x.Group = Prerequisite.GroupType.Any;

                }));

                
            }
        }

        [HarmonyPatch(typeof(CharGenVM), "NeedNamePhase")]
        static class NeedName_Patch
        {

            static void Postfix(CharGenVM __instance, ref bool __result)
            {
                try
                {

                    if (__instance.CharacterName.ToString().Equals("Aivu"))
                    {
                        __result = false;
                        return;
                    }

                }
                catch (Exception e) { Main.Error(e.ToString()); }
            }
        }

        [HarmonyPatch(typeof(LevelUpState), MethodType.Constructor)]
        [HarmonyPatch(new Type[] { typeof(UnitEntityData), typeof(LevelUpState.CharBuildMode), typeof(bool) })]
        [HarmonyPriority(9999)]
        static class LevelUpState_ctor_Patch
        {
            
            private static void Postfix(LevelUpState __instance, UnitEntityData unit, LevelUpState.CharBuildMode mode)
            {
                if (ModSettings.Settings.settings.GroupIsDisabled())
                    return;

                BlueprintFeature AivuUsesMythicXPNow = Resources.GetModBlueprint<BlueprintFeature>("AivuUsesMythicXP");
                if (unit.Descriptor.Progression.Features.HasFact(AivuUsesMythicXPNow))
                {
                    Traverse.Create(__instance).Property("CanSelectName", null).SetValue(false);
                    __instance.CanSelectName = false;
                }

            }
        }
    }
}
