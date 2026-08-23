using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System;
using System.Text;

namespace AmnesiaMead
{

    [BepInPlugin("com.archonite.amnesiamead", "Amnesia Mead", "1.0.0")]
    public class AmnesiaMeadPlugin : BaseUnityPlugin
    {
        public static BepInEx.Logging.ManualLogSource Log;
        private static AmnesiaMeadPlugin Instance;

        private readonly Harmony harmony = new Harmony("com.archonite.amnesiamead");

        private void Awake()
        {
            Log = Logger;
            
            Instance = this;
            harmony.PatchAll();
            Logger.LogInfo("AmnesiaMead has loaded!");
        }


    [HarmonyPatch(typeof(Terminal), "InitTerminal")]
        public static class TerminalInitPatch
        {
            public static void Postfix()
            {
                new Terminal.ConsoleCommand("amnesia-cheats", "Resets the Cheats stat, command history, and item stats to 0", args =>
                {
                    PlayerProfile profile = Game.instance?.GetPlayerProfile();
                    if (profile == null)
                    {
                        args.Context.AddString("Error: Load into a world first.");
                        return;
                    }

                    int clearedCount = 0;

                    // 1. Reset m_usedCheats boolean flags
                    var profileCheatsFlag = AccessTools.Field(typeof(PlayerProfile), "m_usedCheats");
                    if (profileCheatsFlag != null) profileCheatsFlag.SetValue(profile, false);

                    var terminalCheatsFlag = AccessTools.Field(typeof(Terminal), "m_usedCheats");
                    if (terminalCheatsFlag != null) terminalCheatsFlag.SetValue(null, false);

                    // 2. Clear Enum-based stat in m_playerStats.m_stats
                    var playerStatsField = AccessTools.Field(typeof(PlayerProfile), "m_playerStats");
                    if (playerStatsField != null)
                    {
                        object playerStatsObj = playerStatsField.GetValue(profile);
                        if (playerStatsObj != null)
                        {
                            var dictField = AccessTools.Field(playerStatsObj.GetType(), "m_stats");
                            if (dictField != null && dictField.GetValue(playerStatsObj) is IDictionary statsDict)
                            {
                                List<object> keysToZero = new List<object>();
                                foreach (object key in statsDict.Keys)
                                {
                                    if (key != null && key.ToString().Equals("Cheats", StringComparison.OrdinalIgnoreCase))
                                    {
                                        keysToZero.Add(key);
                                    }
                                }

                                foreach (object key in keysToZero)
                                {
                                    statsDict[key] = 0f;
                                    clearedCount++;
                                }
                            }
                        }
                    }

                    // 3. Clear m_knownCommands dictionary (removes 'god', 'spawn', etc.)
                    var knownCmdsField = AccessTools.Field(typeof(PlayerProfile), "m_knownCommands");
                    if (knownCmdsField != null && knownCmdsField.GetValue(profile) is IDictionary knownCmds)
                    {
                        knownCmds.Clear();
                        clearedCount++;
                    }

                    // 4. Remove cheat items from m_itemPickupStats
                    var itemPickupField = AccessTools.Field(typeof(PlayerProfile), "m_itemPickupStats");
                    if (itemPickupField != null && itemPickupField.GetValue(profile) is IDictionary itemStats)
                    {
                        List<object> cheatKeys = new List<object>();
                        foreach (object key in itemStats.Keys)
                        {
                            if (key != null && key.ToString().StartsWith("Cheat", StringComparison.OrdinalIgnoreCase))
                            {
                                cheatKeys.Add(key);
                            }
                        }

                        foreach (object key in cheatKeys)
                        {
                            itemStats.Remove(key);
                            clearedCount++;
                        }
                    }

                    // 5. Save changes to disk
                    Game.instance.SavePlayerProfile(true);

                    if (clearedCount > 0)
                    {
                        args.Context.AddString("Success! Reset Cheats stat to 0, cleared command history, and saved character profile.");
                    }
                    else
                    {
                        args.Context.AddString("No cheat entries were found to clear.");
                    }
                });
            }
        }
    }
}
