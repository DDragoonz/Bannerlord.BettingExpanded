using System.Linq;
using HarmonyLib;
using SandBox.Tournaments.MissionLogics;
using TaleWorlds.CampaignSystem.TournamentGames;

namespace BettingExpanded
{

    [HarmonyPatch(typeof(TournamentBehavior))]
    [HarmonyPatch("CreateTournamentTree")]
    class TournamentBehaviorCreateTournamentTreePatch
    {
        static void Postfix(TournamentBehavior __instance)
        {

            TournamentGame.QualificationMode qualificationMode = (TournamentGame.QualificationMode)BettingExpandedSettings.Instance.QualificationMode.SelectedIndex;

            if (BettingExpandedSettings.Instance.UseSimpleRounds)
            {
                __instance.Rounds[0] = new TournamentRound(16, 1, 4, 8, qualificationMode);
                __instance.Rounds[1] = new TournamentRound(8, 1, 4, 4, qualificationMode);
                __instance.Rounds[2] = new TournamentRound(4, 1, 4, 2, qualificationMode);
                __instance.Rounds[3] = new TournamentRound(2, 1, 2, 1, qualificationMode);
                
            }
            else
            {
                foreach (TournamentRound tournamentRound in __instance.Rounds)
                {
                    TournamentMatch[] matches = tournamentRound.Matches;
                    foreach (TournamentMatch tournamentMatch in matches)
                    {
                        Traverse.Create(tournamentMatch).Field("QualificationMode").SetValue(qualificationMode).GetValue();
                    }
                }
            }

        }
    }

}