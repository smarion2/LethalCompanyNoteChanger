using System.Globalization;
using HarmonyLib;

namespace PlayerNoteChanger
{
    [HarmonyPatch(typeof(Terminal))]
    internal class TerminalPatcher
    {
        [HarmonyPostfix]
        [HarmonyPatch("ParsePlayerSentence")]
        // If you're going to read any of these comments make sure its this one
        // The parameters in these method calls are actually really important to format this way with the underscores. It took me awhile to figure out the errors so hopefully this link will save someone sometime
        // https://harmony.pardeike.net/articles/patching-injections.html
        static void CustomParser(ref Terminal __instance, ref TerminalNode __result)
        {
            // Read what was typed in the terminal and make all characters lowercase
            // format will be: [0]{Command} [1]{PlayerNumber} [2]{PlayerNoteOverRide}
            var terminalText = __instance.screenText.text.Substring(__instance.screenText.text.Length - __instance.textAdded).ToLower(CultureInfo.InvariantCulture);

            // Split text on all spaces
            var splitText = terminalText.Split();

            if (splitText.Length > 0)
            {
                if (splitText[0].Contains("note"))
                {
                    if (splitText.Length > 1)
                    {
                        // Find player
                        var userPlayerSelection = splitText[1];

                        // check to see if 2nd param is a valid number
                        var success = int.TryParse(userPlayerSelection, out var playerNumber);
                        var playerName = "";

                        // If not found
                        if (!success || playerNumber >= StartOfRound.Instance.mapScreen.radarTargets.Count)
                        {
                            var terminalPLayerNotFound = CreateTerminal("Player number not a valid number");
                            __result = terminalPLayerNotFound;
                            return;
                        }
                        else // Make it nicer for the user to see the username instead of the number they input to make them feel warm and fuzzy
                        {
                            playerName = StartOfRound.Instance.mapScreen.radarTargets[playerNumber].name;
                        }

                        // Check if message selection is valid.
                        if (splitText.Length > 2)
                        {
                            int.TryParse(splitText[2], out var result);
                            var response = "";
                            switch (result)
                            {
                                case 0:
                                    response = "Selection not a valid number please try again.";
                                    break;
                                case 1:
                                    response = playerName + " note changed to The Laziest employee.";
                                    PlayerNoteOverrides.Overrides[playerNumber] = "The Laziest employee.";
                                    break;
                                case 2:
                                    response = playerName + " note changed to The most paranoid employee.";
                                    PlayerNoteOverrides.Overrides[playerNumber] = "The most paranoid employee.";
                                    break;
                                case 3:
                                    response = playerName + " note changed to Sustained the most injuries.";
                                    PlayerNoteOverrides.Overrides[playerNumber] = "Sustained the most injuries.";
                                    break;
                                case 4:
                                    response = playerName + " note changed to Most profitable.";
                                    PlayerNoteOverrides.Overrides[playerNumber] = "Most profitable.";
                                    break;
                                case 5:
                                    response = playerName + " note changed to The most worthless~.";
                                    PlayerNoteOverrides.Overrides[playerNumber] = "The most worthless.";
                                    break;
                                case 6:
                                    response = playerName + " note changed to Is a lazy sack of shit.";
                                    PlayerNoteOverrides.Overrides[playerNumber] = "Is a lazy sack of shit.";
                                    break;
                                default:
                                    PlayerNoteOverrides.Overrides.Remove(playerNumber);
                                    break;
                            }

                            var terminalSucess = CreateTerminal(response);
                            __result = terminalSucess;
                        }
                    }
                    else
                    {
                        // Create player list
                        string helpMessage = "Player Selections: \n------------------\n";
                        for (var i = 0; i < StartOfRound.Instance.mapScreen.radarTargets.Count; i++)
                        {
                            helpMessage += $"#{i}. {(StartOfRound.Instance.mapScreen.radarTargets[i].name + "\n")}";
                        }

                        // Create message list
                        helpMessage += "\nMessage Selections:\n------------------\n1.The laziest employee.\n2.The most paranoid employee.\n3.Sustained the most injuries.\n4.Most profitable\n5.The most worthless\n6.Is a lazy sack of shit.\n\n";
                        helpMessage += "Example usage: note 0 3\n";
                        var terminalHelp = CreateTerminal(helpMessage);
                        __result = terminalHelp;
                        return;
                    }
                    
                }
            }
        }

        static TerminalNode CreateTerminal(string message)
        {
            var terminal = new TerminalNode();
            terminal.displayText = message + "\n";
            terminal.clearPreviousText = true;

            return terminal;
        }
    }
}
