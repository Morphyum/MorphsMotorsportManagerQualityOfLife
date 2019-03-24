using Harmony;
using MM2;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MMMQOL {
    class Patches {
        static string version = " +MMMQOL-0.4";
        [HarmonyPatch(typeof(SetUITextToVersionNumber), "Awake")]
        public static class SetUITextToVersionNumber_Awake_Patch {
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

        [HarmonyPatch(typeof(TyreInfoRollover), "Show")]
        public static class TyreInfoRollover_Show_Patch {
            public static void Postfix(TyreInfoRollover __instance, TyreSet inTyreSet, Circuit inCircuit) {
                float num = TyreSet.CalculateLapRangeOfTyre(inTyreSet, GameUtility.MilesToMeters(inCircuit.trackLengthMiles)) * inTyreSet.GetCondition();
                int num2 = Mathf.FloorToInt(num);
                int num3 = num2 - 2;
                num3 = ((num3 > 0 || num2 <= 1) ? num3 : 1);
                if (num3 > 0) {
                    __instance.estimatedLapsLabel.SetText(num3.ToString() + " - " + num2.ToString() + " (" + Mathf.FloorToInt((num-2f) / 4f * 3f) + ")");
                }
            }
        }

        [HarmonyPatch(typeof(PracticeResultsScreen), "OnEnter")]
        public static class PracticeResultsScreen_OnEnter_Patch {
            public static void Postfix(PracticeResultsScreen __instance) {            
                if (Game.instance.gameType == Game.GameType.Career && (!Game.instance.challengeManager.IsAttemptingChallenge() || 
                    Game.instance.challengeManager.currentChallenge.type != Challenge.ChallengeType.SingleRace)) {
                    Game.instance.queuedAutosave = true;
                    App.instance.saveSystem.TryDispatchDelayedAutoSave();
                }
            }
        }

        [HarmonyPatch(typeof(QualifyingResultsScreen), "OnEnter")]
        public static class QualifyingResultsScreen_OnEnter_Patch {
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
        public static class UIFinanceDetailsWidget_OnEnter_Patch {
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

        [HarmonyPatch(typeof(UISponsorsOfferEntry), "SetDetails")]
        public static class UISponsorsOfferEntry_SetDetails_Patch {
            public static void Postfix(UISponsorsOfferEntry __instance, ContractSponsor ___mSponsorContract) {
                __instance.upfrontPayment.text += "\nGuaranteed (Race): " + 
                    GameUtility.GetCurrencyString((long)((___mSponsorContract.upfrontValue + ___mSponsorContract.perRacePayment * ___mSponsorContract.contractTotalRaces) / ___mSponsorContract.contractTotalRaces), 0) + 
                    "  ";
                __instance.raceBonus.text += "\n  Possible (Race): " + 
                    GameUtility.GetCurrencyString((long)(___mSponsorContract.upfrontValue / ___mSponsorContract.contractTotalRaces) + (long)___mSponsorContract.bonusValuePerRace, 0);
            }
        }

        [HarmonyPatch(typeof(UIScoutingSearchResultsWidget), "ApplyFilterView")]
        public static class UIScoutingSearchResultsWidget_ApplyFilterView_Patch {
            public static void Postfix(UIScoutingSearchResultsWidget __instance, Person inPerson, ref bool __result) {
                UIScoutingFilterView.Filter filter = __instance.filterView;
                if (filter == UIScoutingFilterView.Filter.Favourites) {
                    __result = inPerson.GetInterestedToTalkReaction(Game.instance.player.team) == Person.InterestedToTalkResponseType.InterestedToTalk;
                }
            }
        }

        [HarmonyPatch(typeof(StartUpState), "Update")]
        public static class StartUpState_Update_Patch {
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
