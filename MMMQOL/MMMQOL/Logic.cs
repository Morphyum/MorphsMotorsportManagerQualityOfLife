using Harmony;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MMMQOL
{
    class Logic
    {
        
        [HarmonyPatch(typeof(Team), "SelectDriverForSession")]
        public static class Team_SelectDriverForSession_Patch
        {
            public static void Prefix(Team __instance, ref Driver inDriver) {
                if (inDriver.IsReserveDriver()) {
                    inDriver.SetCarID(-1);
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

        [HarmonyPatch(typeof(UIScoutingSearchResultsWidget), "ApplyFilterView")]
        public static class UIScoutingSearchResultsWidget_ApplyFilterView_Patch
        {
            public static void Postfix(UIScoutingSearchResultsWidget __instance, Person inPerson, ref bool __result) {
                UIScoutingFilterView.Filter filter = __instance.filterView;
                if (filter == UIScoutingFilterView.Filter.Favourites) {
                    __result = inPerson.GetInterestedToTalkReaction(Game.instance.player.team) == Person.InterestedToTalkResponseType.InterestedToTalk;
                }
                else if (filter == UIScoutingFilterView.Filter.Scouted)
                {
                    if (inPerson is Driver) {
                        Driver driver = inPerson as Driver;
                        __result = driver.personalityTraitController.IsPayDriver() && driver.hasBeenScouted && inPerson.GetInterestedToTalkReaction(Game.instance.player.team) == Person.InterestedToTalkResponseType.InterestedToTalk;
                    }
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
                    if (driver.personalityTraitController.IsPayDriver() && driver.hasBeenScouted && inPerson.GetInterestedToTalkReaction(Game.instance.player.team) == Person.InterestedToTalkResponseType.InterestedToTalk) {
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
