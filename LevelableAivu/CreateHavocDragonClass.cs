﻿using BlueprintCore.Blueprints.Configurators;
using BlueprintCore.Blueprints.Configurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Utils;
using HarmonyLib;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Blueprints.Root;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using Kingmaker.Designers.EventConditionActionSystem.Conditions;
using Kingmaker.Designers.EventConditionActionSystem.Evaluators;
using Kingmaker.Designers.EventConditionActionSystem.Events;
using Kingmaker.Designers.Mechanics.Buffs;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Abilities.Components.AreaEffects;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.UnitLogic.Mechanics.Components;
using LevelableAivu.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelableAivu.Create
{


    class CreateHavocDragonClass
    {


        [HarmonyPatch(typeof(BlueprintsCache), "Init")]
        [HarmonyPriority(Priority.First)]
        static class BlueprintsCache_Init_Patch
        {

            static BlueprintFeature AivuUsesMythixXPNew;
            static BlueprintCharacterClass HavocDragon;
            static BlueprintSpellbook HavocDragonSpellbookLoaded;

            static BlueprintSpellList HavocDragonSpellListNew;
            static bool Initialized;

            [HarmonyPriority(Priority.First)]
            static void Postfix()
            {

                if (Initialized)
                    return;
                Initialized = true;

                BuildHavocDragonClasses();
                RemoveUnneededElements();
                AddFlagsToAivu();
                FixHeroismAuraSFX();

            }

            private static void FixHeroismAuraSFX()
            {

                var auraSource = BlueprintTool.Get<BlueprintBuff>("17831f3fa25cf52458a34b0acc034b40");
                var aoe = BlueprintTool.Get<BlueprintAbilityAreaEffect>("ce6652b6fb8d1504181a9f3e2aa520e3");
                var baseHeroism = BlueprintTool.Get<BlueprintBuff>("87ab2fed7feaaff47b62a3320a57ad8d");

               

                auraSource.FxOnStart = auraSource.FxOnRemove;

                

                var knockoffConfig = BuffConfigurator.New("AivuHeroismBuff", GUIDs.AivuHeroismBuffGUID);
                knockoffConfig.SetDisplayName(baseHeroism.m_DisplayName);
                knockoffConfig.SetDescription(baseHeroism.m_Description);
                knockoffConfig.SetFlags(baseHeroism.m_Flags);   
                foreach(var c in baseHeroism.Components)
                {
                    knockoffConfig.AddComponent(c);
                }
                knockoffConfig.SetDescriptionShort(baseHeroism.m_DescriptionShort);
                knockoffConfig.SetIcon(baseHeroism.m_Icon);
                var knockoff = knockoffConfig.Configure();
                
                var applier = aoe.Components.OfType<AbilityAreaEffectBuff>().FirstOrDefault();

                if (applier != null)
                {

                    applier.m_Buff = knockoff.ToReference<BlueprintBuffReference>();
                }
            }

            private static void AddFlagsToAivu()
            {
                UnitConfigurator.For("32a037e97c3d5c54b85da8f639616c57").AddFacts(new List<Blueprint<BlueprintUnitFactReference>>() { AivuUsesMythixXPNew}).RemoveComponents(x=> x is LockEquipmentSlot y && y.m_SlotType == LockEquipmentSlot.SlotType.Armor).Configure();




            }

            static void RemoveUnneededElements()
            {
                if (ModSettings.Settings.settings.GroupIsDisabled())
                    return;

                BlueprintProgression AzataProgressionLoaded = BlueprintTool.Get<BlueprintProgression>("9db53de4bf21b564ca1a90ff5bd16586");
                BlueprintFeature T2PassToAivuFeatureLoaded = BlueprintTool.Get<BlueprintFeature>("4d9785fa28ab443289497ccb05e49fe2");
                BlueprintFeature T3PassToAivuFeatureLoaded = BlueprintTool.Get<BlueprintFeature>("1bfc72ee31e349ab91991d14e1db471e");
                BlueprintFeature T4PassToAivuFeatureLoaded = BlueprintTool.Get<BlueprintFeature>("e0cd072417ac444a99e83eae51eea8df");
                AzataProgressionLoaded.LevelEntries.FirstOrDefault(x => x.Level == 3).m_Features.Remove(T2PassToAivuFeatureLoaded.ToReference<BlueprintFeatureBaseReference>());

                AzataProgressionLoaded.LevelEntries.FirstOrDefault(x => x.Level == 5).m_Features.Remove(T3PassToAivuFeatureLoaded.ToReference<BlueprintFeatureBaseReference>());

                AzataProgressionLoaded.LevelEntries.FirstOrDefault(x => x.Level == 7).m_Features.Remove(T4PassToAivuFeatureLoaded.ToReference<BlueprintFeatureBaseReference>());
            }

            static void BuildHavocDragonClasses()
            {
                
                BlueprintUnit AivuUnitLoaded = BlueprintTool.Get<BlueprintUnit>("32a037e97c3d5c54b85da8f639616c57");
                AivuUsesMythixXPNew = Helpers.CreateBlueprint<BlueprintFeature>("AivuUsesMythicXP", x =>
                {

                    //x.SetName("Aivu Mythic Powers");
                    //x.SetDescription("Aivu is amped up by Azata Mythic Power");
                    x.Ranks = 1;
                    x.ReapplyOnLevelUp = true;
                    x.AddComponent(Helpers.Create<AddMechanicsFeature>(x =>
                    {
                        x.m_Feature = AddMechanicsFeature.MechanicsFeatureType.LegendaryHero;
                    }));

                });
                FeatureConfigurator.For(AivuUsesMythixXPNew).SetDisplayName(LocalizationTool.CreateString("AivuUsesMythicXP.Name", "Aivu Mythic Powers")).SetDescription(LocalizationTool.CreateString("AivuUsesMythicXP.Desc", "Aivu is amped up by Azata Mythic Power")).Configure();
                Main.LogPatch("Added",AivuUsesMythixXPNew);
          

                BlueprintAbility HavocBreathLoaded = BlueprintTool.Get<BlueprintAbility>("42a9104e5cff51f46996d7d1ad65c0a6");
                BlueprintFeature SmartBreathWeapon = BlueprintTool.Get<BlueprintFeature>("491c677a0a602c34fbd9530ff53d6d4a");
                BlueprintBuff DragonFearIconSourceLoaded = BlueprintTool.Get<BlueprintBuff>("c0e8f767f87ac354495865ce3dc3ee46");


                BlueprintBuff DragonFearBuffLoaded = BlueprintTool.Get<BlueprintBuff>("3d01fc3ae83ca834ba645013315aaec0");


                BlueprintActivatableAbility AivuDragonfearAbility = Helpers.CreateBlueprint<BlueprintActivatableAbility>("AivuDragonfearToggle", x =>
                {

                    x.m_Buff = DragonFearBuffLoaded.ToReference<BlueprintBuffReference>();
                    x.SetName("Havoc Dragonfear");
                    x.SetDescription("Aivu becomes very scary. Opponents within range may become frightened or shaken. Opponents with less than Aivu's HD within 30 feet will be shaken if they fail a will save - chaff with less then 4 HD are friegtened.");
                    x.m_Icon = DragonFearIconSourceLoaded.m_Icon;
                    x.IsOnByDefault = true;
                    x.DoNotTurnOffOnRest = true;
                    x.DeactivateImmediately = true;

                });
                Main.LogPatch("Added", AivuDragonfearAbility);
                BlueprintFeature AivuSizeUpToMedium = BlueprintTool.Get<BlueprintFeature>("50853b0623b844ac86129db459907797");
                AivuSizeUpToMedium.SetName("Aivu Size Up");
                AivuSizeUpToMedium.SetDescription("Aivu Is Now Medium Size");
                AivuSizeUpToMedium.HideInCharacterSheetAndLevelUp = false;
                AivuSizeUpToMedium.HideInUI = false;
                Main.LogPatch("Added", AivuSizeUpToMedium);
                ChangeUnitSize mediumMechanic = AivuSizeUpToMedium.Components.OfType<ChangeUnitSize>().FirstOrDefault();
                if (mediumMechanic != null)
                {


                    mediumMechanic.SizeDelta = 0;
                    AivuSizeUpToMedium.AddComponent(new VariableBaseSize()
                    {
                        Shift = 1
                    });

                    Main.LogPatch("Patched Size", AivuSizeUpToMedium);

                }
                else
                {
                    Main.Log("Medium Mechanic not found");
                }


                BlueprintFeature AivuSizeUpToMediumDummy = Helpers.CreateBlueprint<BlueprintFeature>("AivuSmallToMedium", x =>
                {
                    x.SetName("Aivu Size Up Dummy");
                    x.SetDescription("If you see this, respec");

                });
                BlueprintFeature AivuSizeUpToLargeDummy = Helpers.CreateBlueprint<BlueprintFeature>("AivuMediumToLarge", x =>
                {
                    x.SetName("Aivu Size Up Dummy");
                    x.SetDescription("If you see this, respec");

                });



                BlueprintFeature AivuDragonfear = Helpers.CreateBlueprint<BlueprintFeature>("AivuDragonfearFeature", x =>
                {
                    x.SetName("Frightful Presence");
                    x.SetDescription("Whenever an opponent with fewer hit dice than Aivu comes within a 30 feet range of her, they must make a Will saving throw. If it fails, the opponent becomes shaken (or, if they have less than 5 hit dice, panicked) for 5d6 rounds. A successful saving throw makes the creature immune to Aivu's Frightful Presence for 24 hours. This is a mind-affecting fear effect.");
                    x.AddComponent(Helpers.Create<AddFacts>(c =>
                    {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                        Resources.GetBlueprint<BlueprintActivatableAbility>("a2e0cbebe3bb4a90a22b75d3c22d952c").ToReference<BlueprintUnitFactReference>(),
                    };
                    }));




                });
                Main.LogPatch("Added", AivuDragonfear);
                HavocDragonSpellbookLoaded = BlueprintTool.Get<BlueprintSpellbook>("778f544f8ed404649a261dce9d514655");

                HavocDragonSpellbookLoaded.Name = LocalizationTool.CreateString(HavocDragonSpellbookLoaded.name + ".Name", "Havoc Dragon", false);//This fixes a base game UI bug, it stays
                HavocDragonSpellListNew = Helpers.CreateBlueprint<BlueprintSpellList>("HavocDragonSpellList", x =>
                {
                    x.FilterBySchool = true;
                });
                HavocDragonSpellbookLoaded.m_SpellList = HavocDragonSpellListNew.ToReference<BlueprintSpellListReference>();

                BlueprintProgression HavocDragonProgressionAdded = Helpers.CreateBlueprint<BlueprintProgression>("HavocDragonProgress", x =>
                {
                    x.IsClassFeature = true;
                    x.SetName("");
                    x.SetDescription("");
                });
                Main.LogPatch("Added", HavocDragonProgressionAdded);
                BlueprintProgression HavocDragonT2ProgressionAdded = Helpers.CreateBlueprint<BlueprintProgression>("HavocDragonProgress2", x =>
                {
                    x.IsClassFeature = true;
                    x.SetName("");
                    x.SetDescription("");
                });
                Main.LogPatch("Added", HavocDragonT2ProgressionAdded);
                BlueprintStatProgression saveHigh = BlueprintTool.Get<BlueprintStatProgression>("ff4662bde9e75f145853417313842751");
                BlueprintStatProgression bab = BlueprintTool.Get<BlueprintStatProgression>("b3057560ffff3514299e8b93e7648a9d");
                HavocDragon = Helpers.CreateBlueprint<BlueprintCharacterClass>("HavocDragonClass", bp =>
                {
                    bp.HitDie = Kingmaker.RuleSystem.DiceType.D12;
                    bp.m_FortitudeSave = saveHigh.ToReference<BlueprintStatProgressionReference>();
                    bp.m_ReflexSave = saveHigh.ToReference<BlueprintStatProgressionReference>();
                    bp.m_WillSave = saveHigh.ToReference<BlueprintStatProgressionReference>();
                    bp.m_BaseAttackBonus = bab.ToReference<BlueprintStatProgressionReference>();
                    bp.name = "HavocDragonClass";
                    bp.m_Progression = HavocDragonProgressionAdded.ToReference<BlueprintProgressionReference>();
                    bp.m_SignatureAbilities = new BlueprintFeatureReference[]
                    {
                            HavocBreathLoaded.ToReference<BlueprintFeatureReference>()
                    };



                    bp.HideIfRestricted = true;
                    bp.LocalizedName = LocalizationTool.CreateString(bp.name + ".Name", "Havoc Dragon", false);
                    bp.LocalizedDescription = LocalizationTool.CreateString(bp.name + "Desc.Name", "A dragon is a reptilelike creature, usually winged, with magical or unusual abilities", false);
                    bp.LocalizedDescriptionShort = LocalizationTool.CreateString(bp.name + "Desc.Name", "A dragon is a reptilelike creature, usually winged, with magical or unusual abilities", false);

                    bp.SkillPoints = 6;
                    bp.m_SignatureAbilities = new BlueprintFeatureReference[] { };
                    bp.ClassSkills = new Kingmaker.EntitySystem.Stats.StatType[]
                    {
                    StatType.SkillAthletics, StatType.SkillMobility, StatType.SkillPerception, StatType.SkillPersuasion, StatType.SkillLoreReligion, StatType.SkillKnowledgeArcana, StatType.SkillStealth, StatType.SkillLoreNature
                    };
                    bp.m_Spellbook = HavocDragonSpellbookLoaded.ToReference<BlueprintSpellbookReference>();

                    bp.AddComponent(Helpers.Create<PrerequisiteFeature>(x =>
                    {

                        x.m_Feature = AivuUsesMythixXPNew.ToReference<BlueprintFeatureReference>();
                    }));



                });
                Main.LogPatch("Added", HavocDragon);
                BlueprintCharacterClass MythicHavocDragon = Helpers.CreateBlueprint<BlueprintCharacterClass>("HavocDragonClass20To40", bp =>
                {
                    bp.HitDie = HavocDragon.HitDie;
                    bp.m_FortitudeSave = HavocDragon.m_FortitudeSave;
                    bp.m_WillSave = HavocDragon.m_WillSave;
                    bp.m_ReflexSave = HavocDragon.m_ReflexSave;
                    bp.m_BaseAttackBonus = HavocDragon.m_BaseAttackBonus;
                    bp.name = "HavocDragon20To40";
                    bp.m_Progression = HavocDragonT2ProgressionAdded.ToReference<BlueprintProgressionReference>();
                    bp.HideIfRestricted = true;
                    bp.LocalizedName = LocalizationTool.CreateString(bp.name + ".Name", "Mythic Havoc Dragon", false);
                    bp.LocalizedDescription = HavocDragon.LocalizedDescription;
                    bp.LocalizedDescriptionShort = HavocDragon.LocalizedDescriptionShort;

                    bp.SkillPoints = 6;
                    bp.m_SignatureAbilities = new BlueprintFeatureReference[] { };
                    bp.ClassSkills = new Kingmaker.EntitySystem.Stats.StatType[]
                    {
                    StatType.SkillAthletics, StatType.SkillMobility, StatType.SkillPerception, StatType.SkillPersuasion, StatType.SkillLoreReligion, StatType.SkillKnowledgeArcana, StatType.SkillStealth, StatType.SkillLoreNature, StatType.SkillKnowledgeWorld, StatType.SkillUseMagicDevice, StatType.SkillStealth
                    };
                    bp.m_Spellbook = HavocDragonSpellbookLoaded.ToReference<BlueprintSpellbookReference>();
                    bp.AddComponent(Helpers.Create<PrerequisiteFeature>(x =>
                    {

                        x.m_Feature = AivuUsesMythixXPNew.ToReference<BlueprintFeatureReference>();
                    }));
                    
                    bp.AddComponent(Helpers.Create<PrerequisiteClassLevel>(x =>
                    {
                        x.m_CharacterClass = HavocDragon.ToReference<BlueprintCharacterClassReference>();
                        x.Level = 20;
                    }));
                    
                });
                Main.LogPatch("Added", MythicHavocDragon);
                HavocDragon.AddComponent<PrerequisiteClassLevel>(x =>
                {
                    x.Not = true;
                    x.m_CharacterClass = MythicHavocDragon.ToReference<BlueprintCharacterClassReference>();
                    x.Level = 1;

                });
                HavocDragon.AddComponent<PrerequisiteClassLevel>(x =>
                {
                    x.Not = true;
                    x.m_CharacterClass = HavocDragon.ToReference<BlueprintCharacterClassReference>();
                    x.Level = 20;

                });
                void AddToClasslLevelEntry(BlueprintProgression progression, int level, BlueprintFeature element)
                {

                    if (progression.LevelEntries == null)
                    {

                        progression.LevelEntries = new LevelEntry[] { };
                    }

                    if (progression.LevelEntries.FirstOrDefault(x => x.Level == level) == null)
                    {

                        LevelEntry[] arr = progression.LevelEntries;

                        LevelEntry add = new LevelEntry
                        {
                            m_Features = new List<BlueprintFeatureBaseReference>() { element.ToReference<BlueprintFeatureBaseReference>() },
                            Level = level

                        };
                        LevelEntry[] arr2 = arr.Append(add).ToArray();


                        progression.LevelEntries = arr2;


                    }
                    else
                    {


                        progression.LevelEntries.First(x => x.Level == level).m_Features.Add(element.ToReference<BlueprintFeatureBaseReference>());
                    }
                    Main.LogPatch($"Progression {progression.NameSafe()} patched: ", element);

                }


                BlueprintFeature DragonType = Resources.GetBlueprint<BlueprintFeature>("455ac88e22f55804ab87c2467deff1d6");
                #region Tier One Aivu Upgrades
                //Feature Block 1 - T2 upgrade. MR5, HD range 16 - 20
                BlueprintFeature AzataDragonDR1 = Helpers.CreateBlueprint<BlueprintFeature>("AivuDRTier1", x =>
                {
                    x.IsClassFeature = true;
                    x.SetName("Havoc Dragon DR, Lesser");
                    x.SetDescription("DR 5/Lawful");
                    AddDamageResistancePhysical dr = Helpers.Create<AddDamageResistancePhysical>(x =>
                    {
                        x.Alignment = Kingmaker.Enums.Damage.DamageAlignment.Lawful;
                        x.BypassedByAlignment = true;
                        x.Value = new Kingmaker.UnitLogic.Mechanics.ContextValue
                        {
                            Value = 5
                        };
                    });

                    x.AddComponent(dr);



                });

                Main.LogPatch("Added", AzataDragonDR1);
                #endregion
                //End t1 block
                #region Teir 2 Aivu Upgrades - MR 7 
                BlueprintFeature AzataDragonDR2 = Helpers.CreateBlueprint<BlueprintFeature>("AivuDRTier2", x =>
                {
                    x.IsClassFeature = true;
                    x.SetName("Havoc Dragon DR");
                    x.SetDescription("DR 15/Lawful");
                    AddDamageResistancePhysical dr = Helpers.Create<AddDamageResistancePhysical>(x =>
                    {
                        x.Alignment = Kingmaker.Enums.Damage.DamageAlignment.Lawful;
                        x.BypassedByAlignment = true;
                        x.Value = new Kingmaker.UnitLogic.Mechanics.ContextValue
                        {
                            Value = 15
                        };
                    });

                    x.AddComponent(dr);



                });
                Main.LogPatch("Added", AzataDragonDR2);
                BlueprintFeature AivuSizeUpToLarge = Resources.GetBlueprint<BlueprintFeature>("600c4d652b6e4684a7a4b77946903c30");
                AivuSizeUpToLarge.SetName("Aivu Size Up");
                AivuSizeUpToLarge.SetDescription("Aivu Is Now Large Size");
                AivuSizeUpToLarge.HideInCharacterSheetAndLevelUp = false;
                AivuSizeUpToLarge.HideInUI = false;
                ChangeUnitSize largeMechanic = AivuSizeUpToLarge.Components.OfType<ChangeUnitSize>().FirstOrDefault();




                if (largeMechanic != null)
                {

                    largeMechanic.SizeDelta = 0;
                    AivuSizeUpToLarge.AddComponent(new VariableBaseSize()
                    {
                        Shift = 1,

                    });

                    Main.LogPatch("Patched Size", AivuSizeUpToLarge);

                }
                else
                {
                    Main.Log("LargeMechanic not found");
                }
                BlueprintFeature HeroicAura = Resources.GetBlueprint<BlueprintFeature>("bb0be011191b77f418d2225399109f0c");

                Main.LogPatch("Added", AzataDragonDR2);



                #endregion

                BlueprintFeature AzataDragonDR3 = Helpers.CreateBlueprint<BlueprintFeature>("AivuDRTier3", x =>
                {
                    x.IsClassFeature = true;
                    x.SetName("Havoc Dragon DR, Greater");
                    x.SetDescription("DR 20/Lawful");
                    AddDamageResistancePhysical dr = Helpers.Create<AddDamageResistancePhysical>(x =>
                    {
                        x.Alignment = Kingmaker.Enums.Damage.DamageAlignment.Lawful;
                        x.BypassedByAlignment = true;
                        x.Value = new Kingmaker.UnitLogic.Mechanics.ContextValue
                        {
                            Value = 20
                        };
                    });

                    x.AddComponent(dr);


                });



                Main.LogPatch("Added", AzataDragonDR3);

                //int mediumLev = 2;
                //int largeLev = 3;
                int mediumLev = 16;
                int largeLev = 26;



                AddToClasslLevelEntry(HavocDragonProgressionAdded, 1, DragonType);

                AddToClasslLevelEntry(HavocDragonProgressionAdded, mediumLev, AivuSizeUpToMedium);
                if (largeLev < 21)
                {
                    AddToClasslLevelEntry(HavocDragonProgressionAdded, largeLev, AivuSizeUpToLarge);
                }
                else
                {
                    AddToClasslLevelEntry(HavocDragonT2ProgressionAdded, largeLev - 20, AivuSizeUpToLarge);
                }


                //AddToClasslLevelEntry(HavocDragonProgressionAdded, 16, AivuSizeUpToMedium);
                AddToClasslLevelEntry(HavocDragonProgressionAdded, 16, SmartBreathWeapon);

                AddToClasslLevelEntry(HavocDragonProgressionAdded, 17, AivuDragonfear);
                AddToClasslLevelEntry(HavocDragonProgressionAdded, 18, AzataDragonDR1);
                //AddToClasslLevelEntry(HavocDragonT2ProgressionAdded, 6, AivuSizeUpToLarge);
                //AddToClasslLevelEntry(HavocDragonProgressionAdded, 6, HeroicAura);
               AddToClasslLevelEntry(HavocDragonT2ProgressionAdded, 7, HeroicAura);
               AddToClasslLevelEntry(HavocDragonProgressionAdded, 27, HeroicAura);
                AddToClasslLevelEntry(HavocDragonT2ProgressionAdded, 8, AzataDragonDR2);
                AddToClasslLevelEntry(HavocDragonProgressionAdded, 28, AzataDragonDR2);


                AddToClasslLevelEntry(HavocDragonT2ProgressionAdded, 10, AzataDragonDR3);
                AddToClasslLevelEntry(HavocDragonProgressionAdded, 30, AzataDragonDR3);

                HavocDragonProgressionAdded.m_Classes = new BlueprintProgression.ClassWithLevel[] { new BlueprintProgression.ClassWithLevel { m_Class = HavocDragon.ToReference<BlueprintCharacterClassReference>() } };

                HavocDragonT2ProgressionAdded.m_Classes = new BlueprintProgression.ClassWithLevel[] { new BlueprintProgression.ClassWithLevel { m_Class = MythicHavocDragon.ToReference<BlueprintCharacterClassReference>() } };

              
                HavocBreathLoaded.Components.OfType<ContextRankConfig>().FirstOrDefault().m_Class = HavocBreathLoaded.Components.OfType<ContextRankConfig>().FirstOrDefault().m_Class.Append(HavocDragon.ToReference<BlueprintCharacterClassReference>()).ToArray();
                HavocBreathLoaded.Components.OfType<ContextRankConfig>().FirstOrDefault().m_Class = HavocBreathLoaded.Components.OfType<ContextRankConfig>().FirstOrDefault().m_Class.Append(MythicHavocDragon.ToReference<BlueprintCharacterClassReference>()).ToArray();

                BlueprintRoot root = Resources.GetBlueprint<BlueprintRoot>("2d77316c72b9ed44f888ceefc2a131f6");
                if (ModSettings.Settings.settings.GroupIsDisabled())
                    return;
                root.Progression.m_PetClasses = root.Progression.m_PetClasses.AddToArray(HavocDragon.ToReference<BlueprintCharacterClassReference>());
                root.Progression.m_PetClasses = root.Progression.m_PetClasses.AddToArray(MythicHavocDragon.ToReference<BlueprintCharacterClassReference>());
               
            }


        }
    }
}
