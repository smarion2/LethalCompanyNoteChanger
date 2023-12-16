using HarmonyLib;
using Unity;

namespace PlayerNoteChanger
{
    [HarmonyPatch(typeof(StartOfRound))]
    internal class StartOfRoundPatcher
    {
        [HarmonyPostfix]
        [HarmonyPatch("WritePlayerNotes")]
        static void CustomWritePlayerNotes(ref EndOfGameStats ___gameStats)
        {
            foreach(var note in PlayerNoteOverrides.Overrides)
            {
                if (note.Key > ___gameStats.allPlayerStats.Length)
                {
                    continue;
                }
                // check to see if any player has a current overriden note. this is pretty lazy at the moment and is going to clear all notes from the player if they have one we can do better but *effort*
                for (var i = 0; i < ___gameStats.allPlayerStats.Length; i++)
                {
                    if (___gameStats.allPlayerStats[i].playerNotes.Contains(note.Value))
                    {
                        ___gameStats.allPlayerStats[i].playerNotes.Clear();
                        continue;
                    }

                }
                // Set note once its all cleared.
                ___gameStats.allPlayerStats[note.Key].playerNotes.Add(note.Value);
            }
        }
    }
}
