using Harmony;
using MM2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameUtility;

namespace MMMQOL
{
    class Patches
    {
        static string version = " +MMMQOL-0.14";
        [HarmonyPatch(typeof(SetUITextToVersionNumber), "Awake")]
        public static class SetUITextToVersionNumber_Awake_Patch
        {
            public static void Postfix(SetUITextToVersionNumber __instance) {
                Text component = __instance.GetComponent<Text>();
                if (component != null) {
                    component.text += version;
                }
                TextMeshProUGUI component2 = __instance.GetComponent<TextMeshProUGUI>();
                if (component2 != null) {
                    component2.text += version;
                }
            }
        }

        [HarmonyPatch(typeof(Team), "SelectDriverForSession")]
        public static class Team_SelectDriverForSession_Patch
        {
            public static void Prefix(Team __instance, ref Driver inDriver) {
                if (inDriver.IsReserveDriver()) {
                    inDriver.SetCarID(-1);
                }
            }
        }

        [HarmonyPatch(typeof(PersonCareerWidget), "SetGrid")]
        public static class PersonCareerWidget_SetGrid_Patch
        {
            public static void Prefix(PersonCareerWidget __instance, Person ___mPerson) {
                List<CareerHistoryEntry> removals = new List<CareerHistoryEntry>();
                foreach(CareerHistoryEntry history in ___mPerson.careerHistory.career) {
                    if(history.races <= 0) {
                        removals.Add(history);
                    }
                }
                foreach (CareerHistoryEntry history in removals) {
                    ___mPerson.careerHistory.RemoveHistory(history);
                }
            }
        }

      /*  [HarmonyPatch(typeof(Championship), "GetChampionshipName")]
        public static class Championship_GetChampionshipName_Patch
        {
            public static void Prefix(ref Championship __instance) {
                if(__instance.series == Championship.Series.SingleSeaterSeries) {
                    if(__instance.championshipOrderRelative == 0) {
                        __instance.customChampionshipName = "World Motorsport Championship";
                    }
                    if (__instance.championshipOrderRelative == 1) {
                        __instance.customChampionshipName = "Asia Pacific Cup";
                    }
                    if (__instance.championshipOrderRelative == 2) {
                        __instance.customChampionshipName = "European Racing Series";
                    }
                }
                if (__instance.series == Championship.Series.GTSeries) {
                    if (__instance.championshipOrderRelative == 0) {
                        __instance.customChampionshipName = "International GT Championship";
                    }
                    if (__instance.championshipOrderRelative == 1) {
                        __instance.customChampionshipName = "GT Challenger Series";
                    }
                }
                if (__instance.series == Championship.Series.EnduranceSeries) {
                    if (__instance.championshipOrderRelative == 0) {
                        __instance.customChampionshipName = "International Endurance Cup";
                    }
                    if (__instance.championshipOrderRelative == 1) {
                        __instance.customChampionshipName = "International Endurance Cup(Class B)";
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Championship), "GetAcronym")]
        public static class Championship_GetAcronym_Patch
        {
            public static void Prefix(ref Championship __instance) {
                if (__instance.series == Championship.Series.SingleSeaterSeries) {
                    if (__instance.championshipOrderRelative == 0) {
                        __instance.customAcronym = "WMC";
                    }
                    if (__instance.championshipOrderRelative == 1) {
                        __instance.customAcronym = "APC";
                    }
                    if (__instance.championshipOrderRelative == 2) {
                        __instance.customAcronym = "ERS";
                    }
                }
                if (__instance.series == Championship.Series.GTSeries) {
                    if (__instance.championshipOrderRelative == 0) {
                        __instance.customAcronym = "IGTC";
                    }
                    if (__instance.championshipOrderRelative == 1) {
                        __instance.customAcronym = "GTCS";
                    }
                }
                if (__instance.series == Championship.Series.EnduranceSeries) {
                    if (__instance.championshipOrderRelative == 0) {
                        __instance.customAcronym = "IEC";
                    }
                    if (__instance.championshipOrderRelative == 1) {
                        __instance.customAcronym = "IEC-B";
                    }
                }
            }
        }*/

        [HarmonyPatch(typeof(UIStat), "SetStat")]
        public static class UIStat_SetStat_Patch
        {
            public static void Postfix(UIStat __instance, string inHeading, ref TextMeshProUGUI ___headingLabel) {
                if (inHeading.Equals("Braking") || inHeading.Equals("Cornering") || inHeading.Equals("Overtaking")) {
                    ___headingLabel.text += "\n(Race Stat)";
                }
                else if (inHeading.Equals("Consistency")) {
                    ___headingLabel.text += "\n(Form)";
                }
                else if (inHeading.Equals("Smoothness")) {
                    ___headingLabel.text += "\n(Tyre Wear)";
                }
                else if (inHeading.Equals("Adaptability")) {
                    ___headingLabel.text += "\n(Wet Weather)";
                }
                else if (inHeading.Equals("Fitness")) {
                    ___headingLabel.text += "\n(Last 30% of Race)";
                }
                else if (inHeading.Equals("Feedback")) {
                    ___headingLabel.text += "\n(Practice)";
                }
                else if (inHeading.Equals("Focus")) {
                    ___headingLabel.text += "\n(Crash, Spin, Lockup)";
                }
            }
        }

        [HarmonyPatch(typeof(StringBuilderPool), "GetBuilder")]
        public static class StringBuilderPool_GetBuilder_Patch
        {
            public static bool Prefix(StringBuilderPool __instance, ref StringBuilder __result, ref Stack<StringBuilder> ___builders) {
                if (___builders.Count == 0) {
                    __result = new StringBuilder(640);
                    return false;
                }
                __result = ___builders.Pop();
                return false;
            }
        }

        [HarmonyPatch(typeof(StringBuilderPool), "ReturnBuilder")]
        public static class StringBuilderPool_ReturnBuilder_Patch
        {
            public static bool Prefix(StringBuilderPool __instance, StringBuilder builder, ref Stack<StringBuilder> ___builders) {
                builder.Length = 0;
                ___builders.Push(builder);
                return false;
            }
        }

        [HarmonyPatch(typeof(UITeamScreenTeamInfoWidget), "Setup")]
        public static class UITeamScreenTeamInfoWidget_Setup_Patch
        {
            public static void Postfix(UITeamScreenTeamInfoWidget __instance, Team inTeam) {
                __instance.nameLabel.text += "(" + Math.Round(inTeam.GetStarsStat(), 2) + ")";
            }
        }

        [HarmonyPatch(typeof(AIBlueFlagBehaviour), "IsInBlueFlagZoneOf")]
        public static class AIBlueFlagBehaviour_IsInBlueFlagZoneOf_Patch
        {
            public static bool Prefix(UITeamScreenTeamInfoWidget __instance, Vehicle inVehicleBehind, Vehicle inVehicleAhead, ref bool __result) {
                float minTimeAhead = 1f;
                if (inVehicleBehind.pathController.IsOnComparablePath(inVehicleAhead)) {
                    float pathDistanceToVehicle = inVehicleBehind.pathController.GetPathDistanceToVehicle(inVehicleAhead);
                    SessionManager sessionManager = Game.instance.sessionManager;
                    if (pathDistanceToVehicle > 0 && !sessionManager.hasSessionEnded) {
                        if (sessionManager.sessionType == SessionDetails.SessionType.Race) {
                            GateInfo gateTimer = sessionManager.GetGateTimer(inVehicleBehind.pathController.GetPreviousGate().id);
                            float timeGapBetweenVehicles = gateTimer.GetTimeGapBetweenVehicles(inVehicleAhead, inVehicleBehind);
                            if (timeGapBetweenVehicles < minTimeAhead || inVehicleAhead.performance.IsExperiencingCriticalIssue()) {
                                __result = true;
                                return false;
                            }
                        }
                        else {
                            if (inVehicleAhead.pathController.IsOnStraight() && inVehicleAhead.speed < inVehicleBehind.speed) {
                                float num = inVehicleBehind.speed - inVehicleAhead.speed;
                                float num2 = (pathDistanceToVehicle - VehicleConstants.vehicleLength) / num;
                                if (num2 < 4f) {
                                    __result = true;
                                    return false;
                                }
                            }
                            bool flag = pathDistanceToVehicle - VehicleConstants.vehicleLength * 6f < 0f;
                            if (flag) {
                                __result = true;
                                return false;
                            }
                        }
                    }
                }
                __result = false;
                return false;
            }
        }

        [HarmonyPatch(typeof(ChampionshipManager), "OnStart")]
        public static class ChampionshipManager_OnStart_Patch
        {
            public static void Postfix(ref ChampionshipManager __instance) {
                List<Championship> list = __instance.GetChampionshipsForSeries(Championship.Series.GTSeries);
                foreach (Championship championship in list) {
                    if (championship.championshipOrderRelative == 0) {
                        championship.prizeFund += 125000000;
                    }
                    else if (championship.championshipOrderRelative == 1) {
                        championship.prizeFund += 75000000;
                    }
                }

                list = __instance.GetChampionshipsForSeries(Championship.Series.EnduranceSeries);
                foreach (Championship championship in list) {
                    if (championship.championshipOrderRelative == 0) {
                        championship.prizeFund += 100000000;
                    }
                    else if (championship.championshipOrderRelative == 1) {
                        championship.prizeFund += 50000000;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(TyreInfoRollover), "Show")]
        public static class TyreInfoRollover_Show_Patch
        {
            public static void Postfix(TyreInfoRollover __instance, TyreSet inTyreSet, Circuit inCircuit) {
                float num = TyreSet.CalculateLapRangeOfTyre(inTyreSet, GameUtility.MilesToMeters(inCircuit.trackLengthMiles)) * inTyreSet.GetCondition();
                int num2 = Mathf.FloorToInt(num);
                int num3 = num2 - 2;
                num3 = ((num3 > 0 || num2 <= 1) ? num3 : 1);
                if (num3 > 0) {
                    __instance.estimatedLapsLabel.SetText(num3.ToString() + " - " + num2.ToString() + " (" + Mathf.FloorToInt((num - 2f) / 4f * 3f) + ")");
                }
            }
        }

        [HarmonyPatch(typeof(PracticeResultsScreen), "OnEnter")]
        public static class PracticeResultsScreen_OnEnter_Patch
        {
            public static void Postfix(PracticeResultsScreen __instance) {
                if (Game.instance.gameType == Game.GameType.Career && (!Game.instance.challengeManager.IsAttemptingChallenge() ||
                    Game.instance.challengeManager.currentChallenge.type != Challenge.ChallengeType.SingleRace)) {
                    Game.instance.queuedAutosave = true;
                    App.instance.saveSystem.TryDispatchDelayedAutoSave();
                }
            }
        }

        [HarmonyPatch(typeof(QualifyingResultsScreen), "OnEnter")]
        public static class QualifyingResultsScreen_OnEnter_Patch
        {
            public static void Postfix(QualifyingResultsScreen __instance) {
                Game.instance.queuedAutosave = true;
                if (Game.instance.gameType == Game.GameType.Career && (!Game.instance.challengeManager.IsAttemptingChallenge() ||
                    Game.instance.challengeManager.currentChallenge.type != Challenge.ChallengeType.SingleRace)) {
                    Game.instance.queuedAutosave = true;
                    App.instance.saveSystem.TryDispatchDelayedAutoSave();
                }
            }
        }

        [HarmonyPatch(typeof(UIFinanceDetailsWidget), "OnEnter")]
        public static class UIFinanceDetailsWidget_OnEnter_Patch
        {
            public static void Postfix(UIFinanceDetailsWidget __instance) {
                List<Transaction> allEventTransactions = Game.instance.player.team.financeController.GetAllEventTransactions();
                long num2 = 0L;
                for (int j = 0; j < allEventTransactions.Count; j++) {
                    num2 += allEventTransactions[j].amountWithSign;
                }
                int racesLeft = 0;
                List<RaceEventDetails> calendar = Game.instance.player.team.championship.calendar;
                foreach (RaceEventDetails details in calendar)
                    if (!details.hasEventEnded) {
                        racesLeft++;
                    }
                __instance.overallCostPerRace.text += "\n(Rest Of Season: " + GameUtility.GetCurrencyStringWithSign(num2 * (long)racesLeft, 0) + ")";
                Finance finance = Game.instance.player.team.financeController.finance;
                __instance.overallBalance[0].text += "\n(Budget Left: " + GameUtility.GetCurrencyStringWithSign(finance.currentBudget + num2 * (long)racesLeft, 0) + ")";
            }
        }

        [HarmonyPatch(typeof(SessionStrategy), "GetSlickTyre")]
        public static class SessionStrategy_GetSlickTyre_Patch
        {
            public static bool Prefix(SessionStrategy __instance, SessionDetails.SessionType inSessionType, ref TyreSet __result) {
                if (inSessionType == SessionDetails.SessionType.Practice) {
                    __result = __instance.GetTyreInBestCondition(SessionStrategy.TyreOption.First, null);
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(UIPanelDriverInfo), "SetHappinessData")]
        public static class UIPanelDriverInfo_SetHappinessData_Patch
        {

            public static void Prefix(ref UIPanelDriverInfo __instance, ref Driver inDriver, Car inCar, bool ___mIsEnduranceSeries) {
                inDriver.carOpinion.CalculateDriverOpinions(inDriver);
            }

            public static void Postfix(ref UIPanelDriverInfo __instance, Driver inDriver, Car inCar, bool ___mIsEnduranceSeries) {
                CarOpinion.Happiness happiness;
                if (___mIsEnduranceSeries) {
                    happiness = inCar.GetAverageCarHappiness();
                }
                else {
                    happiness = inDriver.carOpinion.GetDriverAverageHappiness();
                }
                if (happiness == CarOpinion.Happiness.Happy) {
                    __instance.emoji.sprite = App.instance.atlasManager.GetSprite(AtlasManager.Atlas.Shared1, "Smileys-HappySmileyLarge2");
                }
            }
        }

        [HarmonyPatch(typeof(CarOpinion), "GetColor")]
        public static class CarOpinion_GetColor_Patch
        {

            public static void Postfix(CarOpinion __instance, ref Color __result, CarOpinion.Happiness inHappiness) {
                if (inHappiness == CarOpinion.Happiness.Happy) {
                    __result = GameUtility.ColorFromInts(192, 223, 49);
                }
            }
        }


        [HarmonyPatch(typeof(CarHappinessOverviewRollover), "AssignReaction")]
        public static class CarHappinessOverviewRollover_SetHappinessData_Patch
        {

            public static void Postfix(CarHappinessOverviewRollover __instance, Image inImage, CarOpinion.Happiness inHappiness) {

                if (inHappiness == CarOpinion.Happiness.Happy) {
                    inImage.sprite = App.instance.atlasManager.GetSprite(AtlasManager.Atlas.Shared1, "Smileys-HappySmileyLarge2");
                }
            }
        }

        [HarmonyPatch(typeof(CarHappinessEnduranceEntry), "AssignReaction")]
        public static class CarHappinessEnduranceEntry_SetHappinessData_Patch
        {

            public static void Postfix(CarHappinessEnduranceEntry __instance, Image inImage, CarOpinion.Happiness inHappiness) {
                if (inHappiness == CarOpinion.Happiness.Happy) {
                    inImage.sprite = App.instance.atlasManager.GetSprite(AtlasManager.Atlas.Shared1, "Smileys-HappySmileyLarge2");
                }
            }
        }

        [HarmonyPatch(typeof(SessionSetup), "SetTargetTrim", new Type[0])]
        public static class SessionSetup_SetTargetTrim_Patch
        {
            public static bool Prefix(SessionSetup __instance, ref SessionPitstop ___mSessionPitStop, ref Driver ___mDriver, ref RacingVehicle ___mVehicle) {
                SessionDetails.SessionType sessionType = Game.instance.sessionManager.eventDetails.currentSession.sessionType;
                bool qualifyingBasedActive = ___mDriver.contract.GetTeam().championship.rules.qualifyingBasedActive;
                bool flag = false;
                bool flag2 = false;
                if (___mVehicle.practiceKnowledge.practiceReport != null) {
                    PracticeReportKnowledgeData knowledgeOfType = ___mVehicle.practiceKnowledge.practiceReport.GetKnowledgeOfType(PracticeReportSessionData.KnowledgeType.QualifyingTrim);
                    PracticeReportKnowledgeData knowledgeOfType2 = ___mVehicle.practiceKnowledge.practiceReport.GetKnowledgeOfType(PracticeReportSessionData.KnowledgeType.RaceTrim);
                    flag = (knowledgeOfType.normalizedKnowledge >= 1f);
                    flag2 = (knowledgeOfType2.normalizedKnowledge >= 1f);
                }
                switch (sessionType) {
                    case SessionDetails.SessionType.Practice:
                        if (qualifyingBasedActive && !flag && flag2) {
                            ___mSessionPitStop.currentSetup.trim = SessionSetup.Trim.Qualifying;
                            ___mSessionPitStop.targetSetup.trim = SessionSetup.Trim.Qualifying;
                            ___mVehicle.practiceKnowledge.knowledgeType = PracticeReportSessionData.KnowledgeType.QualifyingTrim;
                        }
                        else {
                            ___mSessionPitStop.currentSetup.trim = SessionSetup.Trim.Race;
                            ___mSessionPitStop.targetSetup.trim = SessionSetup.Trim.Race;
                            ___mVehicle.practiceKnowledge.knowledgeType = PracticeReportSessionData.KnowledgeType.RaceTrim;
                        }
                        break;
                    case SessionDetails.SessionType.Qualifying:
                        ___mSessionPitStop.currentSetup.trim = SessionSetup.Trim.Qualifying;
                        ___mSessionPitStop.targetSetup.trim = SessionSetup.Trim.Qualifying;
                        break;
                    case SessionDetails.SessionType.Race:
                        ___mSessionPitStop.currentSetup.trim = SessionSetup.Trim.Race;
                        ___mSessionPitStop.targetSetup.trim = SessionSetup.Trim.Race;
                        break;
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(UISponsorsOfferEntry), "SetDetails")]
        public static class UISponsorsOfferEntry_SetDetails_Patch
        {
            public static void Postfix(UISponsorsOfferEntry __instance, ContractSponsor ___mSponsorContract) {
                __instance.upfrontPayment.text += "\nGuaranteed (Race): " +
                    GameUtility.GetCurrencyString((long)((___mSponsorContract.upfrontValue + ___mSponsorContract.perRacePayment * ___mSponsorContract.contractTotalRaces) / ___mSponsorContract.contractTotalRaces), 0) +
                    "  ";
                __instance.raceBonus.text += "\n  Possible (Race): " +
                    GameUtility.GetCurrencyString((long)(___mSponsorContract.upfrontValue / ___mSponsorContract.contractTotalRaces) + (long)___mSponsorContract.bonusValuePerRace, 0);
            }
        }

        [HarmonyPatch(typeof(UIScoutingSearchResultsWidget), "ApplyFilterView")]
        public static class UIScoutingSearchResultsWidget_ApplyFilterView_Patch
        {
            public static void Postfix(UIScoutingSearchResultsWidget __instance, Person inPerson, ref bool __result) {
                UIScoutingFilterView.Filter filter = __instance.filterView;
                if (filter == UIScoutingFilterView.Filter.Favourites) {
                    __result = inPerson.GetInterestedToTalkReaction(Game.instance.player.team) == Person.InterestedToTalkResponseType.InterestedToTalk;
                }
            }
        }

        [HarmonyPatch(typeof(UIScoutingSearchResultsWidget), "AddPersonNotifications")]
        public static class UIScoutingSearchResultsWidget_AddPersonNotifications_Patch
        {
            public static bool Prefix(UIScoutingSearchResultsWidget __instance, Person inPerson) {
                Notification mNotificationAll = (Notification)AccessTools.Field(typeof(UIScoutingSearchResultsWidget), "mNotificationAll").GetValue(__instance);
                mNotificationAll.IncrementCount();
                if (inPerson.GetInterestedToTalkReaction(Game.instance.player.team) == Person.InterestedToTalkResponseType.InterestedToTalk) {
                    Notification mNotificationFavourites = (Notification)AccessTools.Field(typeof(UIScoutingSearchResultsWidget), "mNotificationFavourites").GetValue(__instance);
                    mNotificationFavourites.IncrementCount();
                }
                if (inPerson is Driver) {
                    Driver driver = inPerson as Driver;
                    if (driver.hasBeenScouted) {
                        Notification mNotificationScouted = (Notification)AccessTools.Field(typeof(UIScoutingSearchResultsWidget), "mNotificationScouted").GetValue(__instance);
                        mNotificationScouted.IncrementCount();
                    }
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(UIScoutingSearchResultsWidget), "UpdateFavouriteNotification")]
        public static class UIScoutingSearchResultsWidget_UpdateFavouriteNotification_Patch
        {
            public static bool Prefix(UIScoutingSearchResultsWidget __instance) {
                Notification mNotificationFavourites = (Notification)AccessTools.Field(typeof(UIScoutingSearchResultsWidget), "mNotificationFavourites").GetValue(__instance);
                if (mNotificationFavourites == null) {
                    mNotificationFavourites = Game.instance.notificationManager.GetNotification("ScoutingScreenFavourites");
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(UIScoutingSearchResultsWidget), "SortByFilter")]
        public static class UIScoutingSearchResultsWidget_SortByFilter_Patch
        {
            public static void Postfix(UIScoutingSearchResultsWidget __instance) {
                List<Person> mList = (List<Person>)AccessTools.Field(typeof(UIScoutingSearchResultsWidget), "mList").GetValue(__instance);
                List<Person> favorites = new List<Person>();
                for (int i = 0; i < mList.Count; i++) {
                    if (mList[i].isShortlisted) {
                        favorites.Add(mList[i]);
                    }
                }
                for (int i = favorites.Count - 1; i >= 0; i--) {
                    mList.Remove(favorites[i]);
                    mList.Insert(0, favorites[i]);
                }

                AccessTools.Field(typeof(UIScoutingSearchResultsWidget), "mList").SetValue(__instance, mList);
            }
        }

        [HarmonyPatch(typeof(StartUpState), "Update")]
        public static class StartUpState_Update_Patch
        {
            public static bool Prefix(StartUpState __instance, LegalScreen ___mLegalScreen, ref StartUpState.State ___mState) {
                if (___mState != StartUpState.State.SplashSequence) {
                    return false;
                }
                if (UIManager.instance.IsScreenSetLoaded(UIManager.ScreenSet.StartUp) && SceneManager.instance.HasSceneSetLoaded(SceneSet.SceneSetType.Core)) {
                    App.instance.StartCoroutine(Initialise(___mLegalScreen));
                    ___mState = StartUpState.State.LoadNavigationAndDialogs;
                }
                return false;

            }

            private static IEnumerator Initialise(LegalScreen mLegalScreen) {
                if (App.instance.database == null) {
                    App.instance.Initialise();
                }
                while (!App.instance.isInitialised) {
                    yield return null;
                }
                yield return UIManager.instance.LoadNavigationOnly();
                if (PlayerPrefs.GetInt("AcceptedPrivacyPolicy", 0) == 0) {
                    mLegalScreen.privacyPolicy.SetActive(true);
                }
                else {
                    App.instance.gameStateManager.LoadToTitleScreen(GameStateManager.StateChangeType.CheckForFadedScreenChange);
                }
                yield break;
            }
        }
    }
}
