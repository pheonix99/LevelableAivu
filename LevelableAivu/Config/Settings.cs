using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelableAivu.Config
{
    public  class Settings : IUpdatableSettings
    {
        public bool NewSettingsOffByDefault = false;
        public SettingGroup settings = new SettingGroup();

        public  void  OverrideSettings(IUpdatableSettings userSettings)
        {
            var loaded = userSettings as Settings;
            NewSettingsOffByDefault = loaded.NewSettingsOffByDefault;

            settings.LoadSettingGroup(loaded.settings, NewSettingsOffByDefault);
        }

        public  void Init()
        {
           
        }

        
    }
}
