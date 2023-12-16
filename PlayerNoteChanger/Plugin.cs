using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace PlayerNoteChanger
{
    [BepInPlugin(guid, modName, modVersion)]
    public class Plugin : BaseUnityPlugin
    {
        const string guid = "LethalSlugs.PlayerNotePlugin";
        const string modName = "PlayerNoteChanger";
        const string modVersion = "1.0.0";

        private readonly Harmony harmony = new Harmony(guid);
        static Plugin Instance;
        internal ManualLogSource logSource;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            logSource = BepInEx.Logging.Logger.CreateLogSource(guid);
            logSource.LogInfo("Player Note Changer Loaded");
            harmony.PatchAll(typeof(Plugin));
            harmony.PatchAll(typeof(TerminalPatcher));
            harmony.PatchAll(typeof(PlayerNoteOverrides));
            harmony.PatchAll(typeof(StartOfRoundPatcher));
        }
    }
}
 