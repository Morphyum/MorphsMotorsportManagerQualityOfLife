using Harmony;
using MM2;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MMMQOL
{
    class UI
    {
        static string _version = " +MMMQOL-1.6";

        [HarmonyPatch(typeof(SetUITextToVersionNumber), "Awake")]
        public static class SetUITextToVersionNumber_Awake_Patch
        {
            public static void Postfix(SetUITextToVersionNumber __instance)
            {
                Text component = __instance.GetComponent<Text>();
                if (component != null)
                {
                    component.text += _version;
                }

                TextMeshProUGUI component2 = __instance.GetComponent<TextMeshProUGUI>();
                if (component2 != null)
                {
                    component2.text += _version;
                }
            }
        }

        [HarmonyPatch(typeof(UIStat), "SetStat")]
        public static class UIStat_SetStat_Patch
        {
            public static void Postfix(UIStat __instance, string inHeading, ref TextMeshProUGUI ___headingLabel)
            {
                if (inHeading.Equals("Braking") || inHeading.Equals("Cornering") || inHeading.Equals("Overtaking"))
                {
                    ___headingLabel.text += "\n(Race Stat)";
                }
                else if (inHeading.Equals("Consistency"))
                {
                    ___headingLabel.text += "\n(Form)";
                }
                else if (inHeading.Equals("Smoothness"))
                {
                    ___headingLabel.text += "\n(Tyre Wear)";
                }
                else if (inHeading.Equals("Adaptability"))
                {
                    ___headingLabel.text += "\n(Wet Weather)";
                }
                else if (inHeading.Equals("Fitness"))
                {
                    ___headingLabel.text += "\n(Last 30% of Race)";
                }
                else if (inHeading.Equals("Feedback"))
                {
                    ___headingLabel.text += "\n(Practice)";
                }
                else if (inHeading.Equals("Focus"))
                {
                    ___headingLabel.text += "\n(Crash, Spin, Lockup)";
                }
            }
        }

        [HarmonyPatch(typeof(UITeamScreenTeamInfoWidget), "Setup")]
        public static class UITeamScreenTeamInfoWidget_Setup_Patch
        {
            public static void Postfix(UITeamScreenTeamInfoWidget __instance, Team inTeam)
            {
                __instance.nameLabel.text += "(" + Math.Round(inTeam.GetStarsStat(), 2) + ")";
            }
        }

        [HarmonyPatch(typeof(TyreInfoRollover), "Show")]
        public static class TyreInfoRollover_Show_Patch
        {
            public static void Postfix(TyreInfoRollover __instance, TyreSet inTyreSet, Circuit inCircuit)
            {
                float num = TyreSet.CalculateLapRangeOfTyre(inTyreSet,
                    GameUtility.MilesToMeters(inCircuit.trackLengthMiles)) * inTyreSet.GetCondition();
                int calculatedMax = Mathf.FloorToInt(num);
                int calculatedMin = calculatedMax - 2;
                calculatedMin = ((calculatedMin > 0 || calculatedMax <= 1) ? calculatedMin : 1);

                if (calculatedMin > 0)
                {
                    __instance.estimatedLapsLabel.SetText(calculatedMin + " - " + calculatedMax + " (" +
                                                          CalculateExpectedTyreRange(inTyreSet, inCircuit) + ")");
                }
            }
        }

        [HarmonyPatch(typeof(UIFinanceDetailsWidget), "OnEnter")]
        public static class UIFinanceDetailsWidget_OnEnter_Patch
        {
            public static void Postfix(UIFinanceDetailsWidget __instance)
            {
                List<Transaction> allEventTransactions =
                    Game.instance.player.team.financeController.GetAllEventTransactions();
                long num2 = 0L;
                for (int j = 0; j < allEventTransactions.Count; j++)
                {
                    num2 += allEventTransactions[j].amountWithSign;
                }

                int racesLeft = 0;
                List<RaceEventDetails> calendar = Game.instance.player.team.championship.calendar;
                foreach (RaceEventDetails details in calendar)
                    if (!details.hasEventEnded)
                    {
                        racesLeft++;
                    }

                __instance.overallCostPerRace.text += "\n(Rest Of Season: " +
                                                      GameUtility.GetCurrencyStringWithSign(num2 * racesLeft) + ")";
                Finance finance = Game.instance.player.team.financeController.finance;
                __instance.overallBalance[0].text += "\n(Budget Left: " +
                                                     GameUtility.GetCurrencyStringWithSign(finance.currentBudget +
                                                         num2 * racesLeft) + ")";
            }
        }

        [HarmonyPatch(typeof(UIPanelDriverInfo), "SetHappinessData")]
        public static class UIPanelDriverInfo_SetHappinessData_Patch
        {
            public static void Prefix(ref UIPanelDriverInfo __instance, ref Driver inDriver, Car inCar,
                bool ___mIsEnduranceSeries)
            {
                inDriver.carOpinion.CalculateDriverOpinions(inDriver);
            }

            public static void Postfix(ref UIPanelDriverInfo __instance, Driver inDriver, Car inCar,
                bool ___mIsEnduranceSeries)
            {
                CarOpinion.Happiness happiness;
                if (___mIsEnduranceSeries)
                {
                    happiness = inCar.GetAverageCarHappiness();
                }
                else
                {
                    happiness = inDriver.carOpinion.GetDriverAverageHappiness();
                }

                if (happiness == CarOpinion.Happiness.Happy)
                {
                    __instance.emoji.sprite =
                        App.instance.atlasManager.GetSprite(AtlasManager.Atlas.Shared1, "Smileys-HappySmileyLarge2");
                }
            }
        }

        [HarmonyPatch(typeof(CarHappinessOverviewRollover), "AssignReaction")]
        public static class CarHappinessOverviewRollover_SetHappinessData_Patch
        {
            public static void Postfix(CarHappinessOverviewRollover __instance, Image inImage,
                CarOpinion.Happiness inHappiness)
            {
                if (inHappiness == CarOpinion.Happiness.Happy)
                {
                    inImage.sprite =
                        App.instance.atlasManager.GetSprite(AtlasManager.Atlas.Shared1, "Smileys-HappySmileyLarge2");
                }
            }
        }

        [HarmonyPatch(typeof(CarOpinion), "GetColor")]
        public static class CarOpinion_GetColor_Patch
        {
            public static void Postfix(CarOpinion __instance, ref Color __result, CarOpinion.Happiness inHappiness)
            {
                if (inHappiness == CarOpinion.Happiness.Happy)
                {
                    __result = GameUtility.ColorFromInts(192, 223, 49);
                }
            }
        }

        [HarmonyPatch(typeof(CarHappinessEnduranceEntry), "AssignReaction")]
        public static class CarHappinessEnduranceEntry_SetHappinessData_Patch
        {
            public static void Postfix(CarHappinessEnduranceEntry __instance, Image inImage,
                CarOpinion.Happiness inHappiness)
            {
                if (inHappiness == CarOpinion.Happiness.Happy)
                {
                    inImage.sprite =
                        App.instance.atlasManager.GetSprite(AtlasManager.Atlas.Shared1, "Smileys-HappySmileyLarge2");
                }
            }
        }

        [HarmonyPatch(typeof(UISponsorsOfferEntry), "SetDetails")]
        public static class UISponsorsOfferEntry_SetDetails_Patch
        {
            public static void Postfix(UISponsorsOfferEntry __instance, ContractSponsor ___mSponsorContract)
            {
                __instance.upfrontPayment.text += "\nGuaranteed (Race): " +
                                                  GameUtility.GetCurrencyString(
                                                      (___mSponsorContract.upfrontValue +
                                                       ___mSponsorContract.perRacePayment *
                                                       ___mSponsorContract.contractTotalRaces) /
                                                      ___mSponsorContract.contractTotalRaces) +
                                                  "  ";
                __instance.raceBonus.text += "\n  Possible (Race): " +
                                             GameUtility.GetCurrencyString(
                                                 (long)(___mSponsorContract.upfrontValue /
                                                        ___mSponsorContract.contractTotalRaces) +
                                                 ___mSponsorContract.bonusValuePerRace);
            }
        }

        [HarmonyPatch(typeof(DriverWidget), "UpdateTyreWear")]
        public static class DriverWidget_UpdateTyreWear_Patch
        {
            public static void Postfix(DriverWidget __instance, RacingVehicle ___mVehicle)
            {
                if (__instance.tyreWearLabel != null)
                {
                    if (___mVehicle.setup.tyreSet.GetCompound() == TyreSet.Compound.UltraSoft)
                    {
                        if (___mVehicle.setup.tyreSet.GetCondition() <= 0.20f)
                        {
                            __instance.tyreWearLabel.color = UIConstants.negativeColor;
                        }
                        else if (___mVehicle.setup.tyreSet.GetCondition() <= 0.30f)
                        {
                            __instance.tyreWearLabel.color = UIConstants.colorBandYellow;
                        }
                        else
                        {
                            __instance.tyreWearLabel.color = UIConstants.positiveColor;
                        }
                    }

                    else if (___mVehicle.setup.tyreSet.GetCompound() == TyreSet.Compound.SuperSoft)
                    {
                        if (___mVehicle.setup.tyreSet.GetCondition() <= 0.15f)
                        {
                            __instance.tyreWearLabel.color = UIConstants.negativeColor;
                        }
                        else if (___mVehicle.setup.tyreSet.GetCondition() <= 0.25f)
                        {
                            __instance.tyreWearLabel.color = UIConstants.colorBandYellow;
                        }
                        else
                        {
                            __instance.tyreWearLabel.color = UIConstants.positiveColor;
                        }
                    }

                    else if (___mVehicle.setup.tyreSet.GetCompound() == TyreSet.Compound.Soft)
                    {
                        if (___mVehicle.setup.tyreSet.GetCondition() <= 0.10f)
                        {
                            __instance.tyreWearLabel.color = UIConstants.negativeColor;
                        }
                        else if (___mVehicle.setup.tyreSet.GetCondition() <= 0.20f)
                        {
                            __instance.tyreWearLabel.color = UIConstants.colorBandYellow;
                        }
                        else
                        {
                            __instance.tyreWearLabel.color = UIConstants.positiveColor;
                        }
                    }

                    else if (___mVehicle.setup.tyreSet.GetCompound() == TyreSet.Compound.Medium)
                    {
                        if (___mVehicle.setup.tyreSet.GetCondition() <= 0.05f)
                        {
                            __instance.tyreWearLabel.color = UIConstants.negativeColor;
                        }
                        else if (___mVehicle.setup.tyreSet.GetCondition() <= 0.15f)
                        {
                            __instance.tyreWearLabel.color = UIConstants.colorBandYellow;
                        }
                        else
                        {
                            __instance.tyreWearLabel.color = UIConstants.positiveColor;
                        }
                    }
                    else
                    {
                        if (___mVehicle.setup.tyreSet.GetCondition() <= 0.00f)
                        {
                            __instance.tyreWearLabel.color = UIConstants.negativeColor;
                        }
                        else if (___mVehicle.setup.tyreSet.GetCondition() <= 0.10f)
                        {
                            __instance.tyreWearLabel.color = UIConstants.colorBandYellow;
                        }
                        else
                        {
                            __instance.tyreWearLabel.color = UIConstants.positiveColor;
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(UIDriverInfo), "Update")]
        public static class UIDriverInfo_Update_Patch
        {
            public static void Postfix(UIDriverInfo __instance, RacingVehicle ___mVehicle)
            {
                if (__instance.tyreWearLabel != null)
                {
                    if (___mVehicle.setup.tyreSet.GetCompound() == TyreSet.Compound.UltraSoft)
                    {
                        if (___mVehicle.setup.tyreSet.GetCondition() <= 0.30f)
                        {
                            __instance.tyreWearLabel.color = UIConstants.colorBandYellow;
                        }
                        else if (___mVehicle.setup.tyreSet.GetCondition() <= 0.20f)
                        {
                            __instance.tyreWearLabel.color = UIConstants.negativeColor;
                        }
                        else
                        {
                            __instance.tyreWearLabel.color = UIConstants.positiveColor;
                        }
                    }
                    else if (___mVehicle.setup.tyreSet.GetCompound() == TyreSet.Compound.SuperSoft)
                    {
                        if (___mVehicle.setup.tyreSet.GetCondition() <= 0.25f)
                        {
                            __instance.tyreWearLabel.color = UIConstants.colorBandYellow;
                        }
                        else if (___mVehicle.setup.tyreSet.GetCondition() <= 0.15f)
                        {
                            __instance.tyreWearLabel.color = UIConstants.negativeColor;
                        }
                        else
                        {
                            __instance.tyreWearLabel.color = UIConstants.positiveColor;
                        }
                    }
                    else if (___mVehicle.setup.tyreSet.GetCompound() == TyreSet.Compound.Soft)
                    {
                        if (___mVehicle.setup.tyreSet.GetCondition() <= 0.20f)
                        {
                            __instance.tyreWearLabel.color = UIConstants.colorBandYellow;
                        }
                        else if (___mVehicle.setup.tyreSet.GetCondition() <= 0.10f)
                        {
                            __instance.tyreWearLabel.color = UIConstants.negativeColor;
                        }
                        else
                        {
                            __instance.tyreWearLabel.color = UIConstants.positiveColor;
                        }
                    }
                    else if (___mVehicle.setup.tyreSet.GetCompound() == TyreSet.Compound.Medium)
                    {
                        if (___mVehicle.setup.tyreSet.GetCondition() <= 0.15f)
                        {
                            __instance.tyreWearLabel.color = UIConstants.colorBandYellow;
                        }
                        else if (___mVehicle.setup.tyreSet.GetCondition() <= 0.05f)
                        {
                            __instance.tyreWearLabel.color = UIConstants.negativeColor;
                        }
                        else
                        {
                            __instance.tyreWearLabel.color = UIConstants.positiveColor;
                        }
                    }
                    else
                    {
                        if (___mVehicle.setup.tyreSet.GetCondition() <= 0.10f)
                        {
                            __instance.tyreWearLabel.color = UIConstants.colorBandYellow;
                        }
                        else if (___mVehicle.setup.tyreSet.GetCondition() <= 0.00f)
                        {
                            __instance.tyreWearLabel.color = UIConstants.negativeColor;
                        }
                        else
                        {
                            __instance.tyreWearLabel.color = UIConstants.positiveColor;
                        }
                    }
                }
            }
        }

        private static int CalculateExpectedTyreRange(TyreSet inTyreSet, Circuit inCircuit)
        {
            float num = TyreSet.CalculateLapRangeOfTyre(inTyreSet,
                GameUtility.MilesToMeters(inCircuit.trackLengthMiles)) * inTyreSet.GetCondition();
            int calculatedMax = Mathf.FloorToInt(num);
            int calculatedMin = calculatedMax - 2;
            calculatedMin = ((calculatedMin > 0 || calculatedMax <= 1) ? calculatedMin : 1);

            float multiplier;
            switch (inTyreSet.GetCompound())
            {
                case TyreSet.Compound.UltraSoft: multiplier = 1 - 0.3f; break;
                case TyreSet.Compound.SuperSoft: multiplier = 1 - 0.25f; break;
                case TyreSet.Compound.Soft: multiplier = 1 - 0.2f; break;
                case TyreSet.Compound.Medium: multiplier = 1 - 0.15f; break;
                default: multiplier = 1 - 0.1f; break;
            }
            return Mathf.FloorToInt(calculatedMin * multiplier);
        }
    }
}