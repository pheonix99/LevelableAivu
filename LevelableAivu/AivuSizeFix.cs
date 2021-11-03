using HarmonyLib;
using Kingmaker.Designers.Mechanics.Buffs;
using Kingmaker.EntitySystem;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.Buffs;
using Kingmaker.UnitLogic.Parts;
using Kingmaker.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelableAivu
{
    [HarmonyPatch(typeof(UnitPartSizeModifier), "UpdateSize")]

    static class AivuSizeFix
    {
        [HarmonyPriority(Priority.Normal)]
        static bool Prefix(UnitPartSizeModifier __instance)
        {
          
            var variableSize = __instance.Owner.Facts.m_Facts.SelectMany(x => x.Components).Select(x => x.SourceBlueprintComponent).OfType<VariableBaseSize>();
            if (variableSize.Count() == 0)
            {
              
                return true;
            }
            else
            {
              
                Size ogSize = __instance.Owner.OriginalSize + variableSize.Select(x => x.Shift).Sum();
                Size finalSize = ogSize;
                EntityFact entityFact = __instance.m_SizeChangeFacts.LastItem<EntityFact>();
                if (entityFact != null)
                {
                    foreach (EntityFactComponent entityFactComponent in entityFact.Components)
                    {
                        Polymorph polymorph = entityFactComponent.SourceBlueprintComponent as Polymorph;
                        ChangeUnitSize changeUnitSize = entityFactComponent.SourceBlueprintComponent as ChangeUnitSize;
                        if (polymorph != null)
                        {
                            Size? polySize = polymorph.GetUnitSize(entityFactComponent);
                            if (polySize != null)
                                finalSize = polySize.Value;
                            
                        }
                        else if (changeUnitSize != null)
                        {
                            if (changeUnitSize.IsTypeDelta)
                            {
                                finalSize = ogSize + changeUnitSize.SizeDelta;
                                //Main.Log($"Size Adjust: Size set to :{finalSize}");
                            }
                            else if (changeUnitSize.IsTypeValue)
                            {
                                finalSize = changeUnitSize.Size;
                                //Main.Log($"Size Set: Size set to :{finalSize}");
                            }
                        }
                    }
                }
                else
                {
                    __instance.Owner.Remove<UnitPartSizeModifier>();
                }
                Main.Log($"Final Size Setto :{finalSize}");
                __instance.Owner.State.Size = finalSize;
                return false;
            }
            

        }
    }

    static class AfterTheFactAivuSizeFix
    {

    }
}
