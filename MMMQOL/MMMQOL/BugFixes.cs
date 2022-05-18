using Harmony;
using System.Collections.Generic;

namespace MMMQOL {
    class BugFixes {
        [HarmonyPatch(typeof(Team), "IncreaseDriverHistoryStat")]
        public static class Team_IncreaseDriverHistoryStat_Patch {
            public static bool Prefix(Team __instance, Driver inDriver) {
                if(inDriver == null) {
                    Logger.LogLine("IncreaseDriverHistoryStat No Driver");
                    return false;
                }
                if(inDriver.careerHistory == null) {
                    Logger.LogLine("IncreaseDriverHistoryStat No career History");
                    inDriver.GenerateCareerHistory();
                }
                if(inDriver.careerHistory.currentEntry == null) {
                    Logger.LogLine("IncreaseDriverHistoryStat No currentEntry");
                    inDriver.careerHistory.AddHistory();
                }
                Logger.LogLine("IncreaseDriverHistoryStat Driver found");
                return true;
            }
        }

        [HarmonyPatch(typeof(Person), "GenerateCareerHistory")]
        public static class Person_GenerateCareerHistory_Patch {
            public static void Postfix(Person __instance) {
                if(__instance.careerHistory == null) {
                    __instance.careerHistory = new CareerHistory();
                }
            }
        }

        [HarmonyPatch(typeof(Championship), "GetChampionshipName")]
        public static class Championship_GetChampionshipName_Patch {
            public static void Prefix(ref Championship __instance) {
                if(__instance.series == Championship.Series.SingleSeaterSeries) {
                    if(__instance.championshipOrderRelative == 0) {
                        __instance.customChampionshipName = "World Motorsport Championship";
                    }
                    if(__instance.championshipOrderRelative == 1) {
                        __instance.customChampionshipName = "Asia Pacific Cup";
                    }
                    if(__instance.championshipOrderRelative == 2) {
                        __instance.customChampionshipName = "European Racing Series";
                    }
                }
                if(__instance.series == Championship.Series.GTSeries) {
                    if(__instance.championshipOrderRelative == 0) {
                        __instance.customChampionshipName = "International GT Championship";
                    }
                    if(__instance.championshipOrderRelative == 1) {
                        __instance.customChampionshipName = "GT Challenger Series";
                    }
                }
                if(__instance.series == Championship.Series.EnduranceSeries) {
                    if(__instance.championshipOrderRelative == 0) {
                        __instance.customChampionshipName = "International Endurance Cup";
                    }
                    if(__instance.championshipOrderRelative == 1) {
                        __instance.customChampionshipName = "International Endurance Cup(Class B)";
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Championship), "GetAcronym")]
        public static class Championship_GetAcronym_Patch {
            public static void Prefix(ref Championship __instance) {
                if(__instance.series == Championship.Series.SingleSeaterSeries) {
                    if(__instance.championshipOrderRelative == 0) {
                        __instance.customAcronym = "WMC";
                    }
                    if(__instance.championshipOrderRelative == 1) {
                        __instance.customAcronym = "APC";
                    }
                    if(__instance.championshipOrderRelative == 2) {
                        __instance.customAcronym = "ERS";
                    }
                }
                if(__instance.series == Championship.Series.GTSeries) {
                    if(__instance.championshipOrderRelative == 0) {
                        __instance.customAcronym = "IGTC";
                    }
                    if(__instance.championshipOrderRelative == 1) {
                        __instance.customAcronym = "GTCS";
                    }
                }
                if(__instance.series == Championship.Series.EnduranceSeries) {
                    if(__instance.championshipOrderRelative == 0) {
                        __instance.customAcronym = "IEC";
                    }
                    if(__instance.championshipOrderRelative == 1) {
                        __instance.customAcronym = "IEC-B";
                    }
                }
            }
        }

        [HarmonyPatch(typeof(PersonCareerWidget), "SetGrid")]
        public static class PersonCareerWidget_SetGrid_Patch {
            public static void Prefix(PersonCareerWidget __instance, Person ___mPerson) {
                List<CareerHistoryEntry> removals = new List<CareerHistoryEntry>();
                foreach(CareerHistoryEntry history in ___mPerson.careerHistory.career) {
                    if(history.races <= 0) {
                        removals.Add(history);
                    }
                }
                foreach(CareerHistoryEntry history in removals) {
                    ___mPerson.careerHistory.RemoveHistory(history);
                }
            }
        }
    }
}
