using BettingExpanded.UI;
using HarmonyLib;
using SandBox.Tournaments.MissionLogics;
using TaleWorlds.MountAndBlade;

namespace BettingExpanded
{
    public class BettingExpandedSubModule: MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            new Harmony("mod.bannerlord.bettingexpanded").PatchAll();
        }
        
        public override void OnBeforeMissionBehaviorInitialize(Mission mission)
        {
            base.OnBeforeMissionBehaviorInitialize(mission);
            if (mission.HasMissionBehavior<TournamentBehavior>())
            {
                mission.AddMissionBehavior(new BettingExpandedMission());
                if (BettingExpandedSettings.Instance.BettingModeToggle)
                {
                    mission.AddMissionBehavior(new BettingMissionView());    
                }
                    
            }
            
        }
    }
}