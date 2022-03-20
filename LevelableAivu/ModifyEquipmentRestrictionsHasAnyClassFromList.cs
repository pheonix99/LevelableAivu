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
            BlueprintCharacterClass companion = Helpers.GetBlueprint<BlueprintCharacterClass>("01a754e7c1b7c5946ba895a5ff0faffc");
            BlueprintFeature AivuUsesMythicXPNow = Resources.GetModBlueprint<BlueprintFeature>("AivuUsesMythicXP");
            if (__instance.OwnerBlueprint is BlueprintItemEquipment or BlueprintItemArmor)
            {
                if (unit.HasFact(AivuUsesMythicXPNow))//This is Aivuz
                {

                    if (__instance.Class.ToReference<BlueprintCharacterClassReference>().Equals(companion.ToReference<BlueprintCharacterClassReference>()))
                    {
                        __result = !__instance.Not;
                    }
                }
            }

        }
    }




    [HarmonyPatch(typeof(EquipmentRestrictionHasAnyClassFromList), "CanBeEquippedBy", new Type[] { typeof(UnitDescriptor) })]
    static class ModifyEquipmentRestrictionsHasAnyClassFromList
    {
        

        static void Postfix(ref bool __result, EquipmentRestrictionHasAnyClassFromList __instance, UnitDescriptor unit)
        {
            var dragon = Helpers.GetBlueprint<BlueprintCharacterClass>("01a754e7c1b7c5946ba895a5ff0faffc").ToReference<BlueprintCharacterClassReference> ();
           
            BlueprintFeature AivuUsesMythicXPNow = Resources.GetModBlueprint<BlueprintFeature>("AivuUsesMythicXP");
            if (__instance.OwnerBlueprint is BlueprintItemEquipment or BlueprintItemArmor)
            {
                if (unit.HasFact(AivuUsesMythicXPNow))//This is Aivu
                {
                    
                    if (__instance.Classes.Any(x=>x.ToReference<BlueprintCharacterClassReference>().Equals(dragon)))
                    {
                        __result = !__instance.Not;
                    }
                }
            }
            
        }
    }
}
