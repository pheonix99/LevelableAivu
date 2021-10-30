using HarmonyLib;
using Kingmaker.Blueprints.Classes;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UnitLogic.Class.LevelUp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelableAivu
{
    

    [HarmonyPatch(typeof(LevelUpController), MethodType.Constructor)]
    [HarmonyPatch(new Type[] { typeof(UnitEntityData), typeof(bool), typeof(LevelUpState.CharBuildMode) })]
    [HarmonyPriority(9999)]
    internal static class LevelUpController_ctor_Patch
    {
        private static void Postfix(LevelUpController __instance, UnitEntityData unit, LevelUpState.CharBuildMode mode)
        {
            try
            {
                BlueprintFeature Aivu = Resources.GetModBlueprint<BlueprintFeature>("AivuUsesMythicXP");
                if (unit.Descriptor.Progression.Features.HasFact(Aivu))
                {
                    __instance.SelectName("Aivu");
                    

                }

                
            }
            catch (Exception e) { Main.Log(e.ToString()); }
        }
    }
}
