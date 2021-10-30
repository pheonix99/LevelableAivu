using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Blueprints.Root;
using Kingmaker.EntitySystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.Enums;
using Kingmaker.UI.ServiceWindow.CharacterScreen;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Parts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelableAivu
{

    [HarmonyPatch(typeof(AddPet), "TryLevelUpPet")]
    
    static class PatchTryLevelUpPet
    {
        [HarmonyPriority(Priority.Normal)]
        static bool Prefix(AddPet __instance)
        {

            
            
            if (__instance.Type != PetType.AzataHavocDragon)
            {


               
                return true;
            }
            else
            {
                
                UnitEntityData value = __instance.Data.SpawnedPetRef.Value;
                if (value == null)
                {
                    return false;
                }
                BlueprintComponentAndRuntime<AddClassLevels> blueprintComponentAndRuntime = (from f in value.Facts.List
                                                                                             select f.GetComponentWithRuntime<AddClassLevels>()).FirstOrDefault((BlueprintComponentAndRuntime<AddClassLevels> c) => c.Component != null);
                if (blueprintComponentAndRuntime.Component == null)
                {
                    return false;
                }
                int characterLevel = value.Descriptor.Progression.CharacterLevel;
                int petLevel = __instance.GetPetLevel();
                int num = petLevel - characterLevel;
           
                if (num > 0)
                {
                    UnitPartCompanion unitPartCompanion = __instance.Owner.Get<UnitPartCompanion>();
                    if (unitPartCompanion != null && unitPartCompanion.State != CompanionState.None && unitPartCompanion.State != CompanionState.ExCompanion && !__instance.Data.AutoLevelup && __instance.Type == PetType.AzataHavocDragon)
                    {
                        

                        int bonus = BlueprintRoot.Instance.Progression.LegendXPTable.GetBonus(petLevel);
                        value.Progression.AdvanceExperienceTo(bonus, false, true);

                    }
                    else
                    {
                        return true;
                    }
                }
                if (__instance.GetPetLevel() >= __instance.UpgradeLevel && __instance.UpgradeFeature != null)
                {
                    value.Progression.Features.AddFeature(__instance.UpgradeFeature, null);
                }
                __instance.Data.AutoLevelup = false;
                return false;
            }

        }

    }



}
