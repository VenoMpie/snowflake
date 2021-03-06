using Snowflake.Configuration;
using Snowflake.Configuration.Attributes;

//autogenerated using generate_retroarch.py
namespace Snowflake.Plugin.EmulatorAdapter.RetroArch.Configuration.Internal
{
    /// <summary>
    /// RetroAchievements is in beta.
    /// Perhaps enable this in a future update
    /// </summary>
    public class AchievementConfiguration : ConfigurationSection
    {
        [ConfigurationOption("cheevos_enable", DisplayName = "Enable RetroArchivements", Private = true)]
        public bool CheevosEnable { get; set; } = false;
        [ConfigurationOption("cheevos_hardcore_mode_enable", DisplayName = "Enable RetroArcheivements Hardcore Mode", Private = true)]
        public bool CheevosHardcoreModeEnable { get; set; } = false;
        [ConfigurationOption("cheevos_password", DisplayName = "RetroAchievements Password", Private = true)]
        public string CheevosPassword { get; set; } = "";
        [ConfigurationOption("cheevos_test_unofficial", DisplayName = "RetroAchievements Test", Private = true)]
        public bool CheevosTestUnofficial { get; set; } = false;
        [ConfigurationOption("cheevos_username", DisplayName = "RetroAchivements Username", Private = true)]
        public string CheevosUsername { get; set; } = "";

        public AchievementConfiguration() : base("cheevos", "Cheevos Options")
        {

        }

    }
}
