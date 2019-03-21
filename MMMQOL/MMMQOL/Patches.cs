using Harmony;
using MM2;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MMMQOL {
    class Patches {

        [HarmonyPatch(typeof(SetUITextToVersionNumber), "Awake")]
        public static class SetUITextToVersionNumber_Awake_Patch {
            public static void Postfix(SetUITextToVersionNumber __instance) {
                Text component = __instance.GetComponent<Text>();
                if (component != null) {
                    component.text = component.text + " +MMMQOL-0.1";
                }
                TextMeshProUGUI component2 = __instance.GetComponent<TextMeshProUGUI>();
                if (component2 != null) {
                    component2.text = component2.text + " +MMMQOL-0.1";
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
                    __instance.estimatedLapsLabel.SetText(num3.ToString() + " - " + num2.ToString() + " (" + num3 / 4 * 3 + ")");
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
                    if (details.hasEventEnded) {
                        racesLeft++;
                    }
                __instance.overallCostPerRace.text += "\n(Rest Of Season: " + GameUtility.GetCurrencyStringWithSign(num2 * (long)racesLeft, 0) + ")";
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

    }
}
