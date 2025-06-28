using System.Linq;
using Shokuho.CustomCampaign.Tournaments;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace BettingExpanded
{
 public class BettingExpandedMission : MissionLogic
    {
        private ShokuhoTournamentBehavior _tournamentBehavior;

        public override void AfterStart()
        {
            base.AfterStart();
            _tournamentBehavior = Mission.GetMissionBehavior<ShokuhoTournamentBehavior>();
        }

        public override void OnAgentRemoved(
            Agent affectedAgent,
            Agent affectorAgent,
            AgentState agentState,
            KillingBlow blow)
        {

            if (affectedAgent == null || affectorAgent == null)
            {
                return;
            }

            // skip for mount, or if agent is on same team
            if (affectedAgent.Team == affectorAgent.Team || affectedAgent.IsMount || affectorAgent.IsMount)
            {
                return;
            }

            if (agentState == AgentState.Killed || agentState == AgentState.Unconscious )
            {

                TournamentParticipant killer = _tournamentBehavior.CurrentMatch.Participants.First(x => x.Character == affectorAgent.Character);
                killer.AddScore(10);

                if (BettingExpandedSettings.Instance.HeadshotScoreEnabled && blow.IsHeadShot())
                {
                    killer.AddScore(10);
                }

                float distance = 0;
                float minimumDistance = 0;
                if (BettingExpandedSettings.Instance.UseRangedScoring)
                {
                    distance = affectedAgent.Position.AsVec2.Distance(affectorAgent.Position.AsVec2);

                    minimumDistance = affectorAgent.MountAgent != null ? BettingExpandedSettings.Instance.MinRangedKillDistanceOnHorse : BettingExpandedSettings.Instance.MinRangedKillDistanceOnFoot;

                    if (distance >= minimumDistance)
                    {
                        killer.AddScore(10);
                    }    
                }
                
                if (BettingExpandedSettings.Instance.EnableDebug)
                {
                    InformationManager.DisplayMessage(new InformationMessage(affectedAgent.Character.Name + " defeated by "+affectorAgent.Character.Name));
                    if (BettingExpandedSettings.Instance.UseRangedScoring) InformationManager.DisplayMessage(new InformationMessage("distance : "+distance +" / "+minimumDistance));
                    if (BettingExpandedSettings.Instance.HeadshotScoreEnabled) InformationManager.DisplayMessage(new InformationMessage("headshot : "+blow.IsHeadShot()));
                    InformationManager.DisplayMessage(new InformationMessage("killer is mounted : "+(affectorAgent.MountAgent != null)));    
                }
                
            }
            
            
        }

        
    }
}