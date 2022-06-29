using HarmonyLib;
using JetBrains.Annotations;
using Kingmaker;
using Kingmaker.Blueprints.JsonSystem;
using LevelableAivu.Config;
using LevelableAivu.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityModManagerNet;


namespace LevelableAivu
{
    static class Main
    {
        public static bool Enabled;
  
        static bool Load(UnityModManager.ModEntry modEntry)
        {
            var harmony = new Harmony(modEntry.Info.Id);
            ModSettings.ModEntry = modEntry;
            ModSettings.LoadAllSettings();
            ModSettings.ModEntry.OnSaveGUI = OnSaveGUI;
            ModSettings.ModEntry.OnGUI = UMMSettingsUI.OnGUI;
            
            harmony.PatchAll();
            PostPatchInitializer.Initialize();
            return true;
        }
        public static void Error(Exception e, string message)
        {
            Log(message);
            Log(e.ToString());
            //PFLog.Mods.Error(message);
        }
        public static void Error(string message)
        {
            Log(message);
            //PFLog.Mods.Error(message);
        }

        static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            
        }
        public static void Log(string msg)
        {
            ModSettings.ModEntry.Logger.Log(msg);
        }

        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            Enabled = value;
            return true;
        }

        public static void LogDebug(string msg)
        {
            ModSettings.ModEntry.Logger.Log(msg);
        }
        public static void LogPatch(string action, [NotNull] IScriptableObjectWithAssetId bp)
        {
            Log($"{action}: {bp.AssetGuid} - {bp.name}");
        }
    }
}
