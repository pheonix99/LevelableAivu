using Kingmaker.Utility;
using ModKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LevelableAivu.Config;
using UnityEngine;
using UnityModManagerNet;

namespace LevelableAivu
{
    public static class UMMSettingsUI
    {
        private static int selectedTab;
        public static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            UI.AutoWidth();
            UI.TabBar(ref selectedTab,
                    () => UI.Label("SETTINGS WILL NOT BE UPDATED UNTIL YOU RESTART YOUR GAME.".yellow().bold()),
                    new NamedAction("Settings", () => SettingsTabs.Settings())
                    
            );
        }
    }

    static class SettingsTabs
    {
        public static void Settings()
        {
            var TabLevel = SetttingUI.TabLevel.Zero;
            var Settings = ModSettings.Settings;
            UI.Div(0, 15);
            using (UI.VerticalScope())
            {
                SetttingUI.SettingGroup("Settings", TabLevel, Settings.settings);
            }
        }

       
        
    }

    static class SetttingUI
    {
        public enum TabLevel : int
        {
            Zero,
            One,
            Two,
            Three,
            Four,
            Five
        }
        public static void Increase(ref this TabLevel level)
        {
            level += 1;
        }
        public static void Decrease(ref this TabLevel level)
        {
            if ((int)level > 0)
            {
                level -= 1;
            }
        }
        public static int Spacing(this TabLevel level)
        {
            return (int)level * 50;
        }
        public static void Indent(this TabLevel level)
        {
            UI.Space(level.Spacing());
        }

        

        

        public static void SettingGroup(string name, TabLevel level, SettingGroup group)
        {
            if (group.Settings.Empty()) { return; }
            RootGroup(name, level, group);
            if (group.IsExpanded)
            {
                level.Increase();
                if (group.Settings.Any()) { TabbedItem(level, () => UI.Div(Color.grey, 500)); }
                group.Settings.ForEach(entry => {
                    TabbedItem(level,
                        () => Toggle(String.Join(" ", entry.Key.SplitOnCapitals()), group.IsEnabled(entry.Key), (enabled) => group.ChangeSetting(entry.Key, enabled), UI.Width(500 - level.Spacing())),
                        () => Label(entry.Value.Description.green()));
                    TabbedItem(level, () => UI.Div(Color.grey, 500));
                });
                level.Decrease();
            }
        }

        public static void RootGroup(string name, TabLevel level, ICollapseableGroup rootGroup)
        {
            using (UI.HorizontalScope())
            {
                level.Indent();
               
                UI.DisclosureToggle(name.bold(), ref rootGroup.IsExpanded(), 140);
            }
        }

        public static void TabbedItem(TabLevel level, params Action[] actions)
        {
            using (UI.HorizontalScope())
            {
                level.Indent();
                actions.ForEach(action => action.Invoke());
            }
        }

        public static bool Toggle(string title, bool value, Action<bool> action, params GUILayoutOption[] options)
        {
            options = options.AddDefaults();
            var changed = ModKit.Private.UI.CheckBox(title, value, UI.toggleStyle, options);
            if (changed)
            {
                Main.Log($"Changed: {!value}");
                action.Invoke(!value);
            }
            return changed;
        }

        public static void Label(string title)
        {
            GUILayout.Label(title, GUILayout.ExpandWidth(false));
        }

        public static IEnumerable<string> SplitOnCapitals(this string text)
        {
            Regex regex = new Regex(@"\p{Lu}\p{Ll}*");
            foreach (Match match in regex.Matches(text))
            {
                yield return match.Value;
            }
        }
    }
}
