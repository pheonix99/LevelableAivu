using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Items.Armors;
using Kingmaker.Blueprints.Items.Components;
using Kingmaker.Blueprints.Items.Equipment;
using Kingmaker.UnitLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelableAivu
{
    
   [HarmonyPatch(typeof(EquipmentRestrictionClass), "CanBeEquippedBy", new Type[] { typeof(UnitDescriptor) })]
    static class ModifyEquipmentRestrictions
    {
        

        static void Postfix(ref bool __result, EquipmentRestrictionClass __instance, UnitDescriptor unit)
        {
            BlueprintCharacterClass companion = Helpers.GetBlueprint<BlueprintCharacterClass>("26b10d4340839004f960f9816f6109fe");
            BlueprintFeature AivuUsesMythicXPNow = Resources.GetModBlueprint<BlueprintFeature>("AivuUsesMythicXP");
            if (__instance.OwnerBlueprint is BlueprintItemEquipment f)
            {
                if (unit.HasFact(AivuUsesMythicXPNow))//This is Aivu
                {
                    
                    if (__instance.Class.ToReference<BlueprintCharacterClassReference>().Equals(companion.ToReference<BlueprintCharacterClassReference>()))
                    {
                        __result = !__instance.Not;
                    }
                }
            }
            
        }
    }
}
