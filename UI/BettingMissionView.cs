using System;
using System.Linq;
using BettingExpanded.BettingLogic;
using SandBox.Tournaments.MissionLogics;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.ScreenSystem;

namespace BettingExpanded.UI
{
    public class BettingMissionView : MissionView
    {

        public BettingMissionView()
        {
            ViewOrderPriority = 100;
        }
        
        public override void AfterStart()
        {
            base.AfterStart();
            _tournamentBehavior = Mission.GetMissionBehavior<TournamentBehavior>();
            _lastMatchState = _tournamentBehavior.CurrentMatch.State;
            _bettingHandler = new BettingHandler(_tournamentBehavior.CurrentMatch);
            _dataSource = new BettingVM(this);
            _dataSource.Init(_bettingHandler);

        }

        protected override void OnEndMission()
        {
            base.OnEndMission();
            _tournamentBehavior = null;
            _bettingHandler = null;
            _dataSource = null;
            _gauntletLayer = null;
            _movie = null;
        }

        public override void OnMissionTick(float dt)
        {
            base.OnMissionTick(dt);
            if (_tournamentBehavior == null || _bettingHandler == null || _bettingHandler.GetHandledMatch() == null) 
            {
                return;
            }

            TournamentMatch match = _bettingHandler.GetHandledMatch();

            TournamentMatch.MatchState currentMatchState = match.State;

            if (currentMatchState == _lastMatchState) return;

            if (_bettingHandler.IsBettingEnabled)
            {
                switch (currentMatchState)
                {
                    case TournamentMatch.MatchState.Started:
                        ToggleBettingMenu(true);
                        break;
                    case TournamentMatch.MatchState.Finished:
                    {
                        _bettingHandler.ProcessBetResult();
                        if (!BettingExpandedSettings.Instance.HideResultIfNotBetting || _bettingHandler.IsPlayerPutAnyBet())
                        {
                            ToggleBettingMenu(true);    
                        }

                        break;
                    }
                }
            }


            _lastMatchState = currentMatchState;

            if (_bettingHandler != null && _tournamentBehavior.CurrentMatch != _bettingHandler.GetHandledMatch() && _tournamentBehavior.CurrentMatch != null)
            {
                _bettingHandler = new BettingHandler(_tournamentBehavior.CurrentMatch);
                _dataSource.Init(_bettingHandler);
                _lastMatchState = _tournamentBehavior.CurrentMatch.State;
            }
            
        }

        public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
        {
            base.OnAgentRemoved(affectedAgent, affectorAgent, agentState, blow);
            if (agentState == AgentState.Killed || agentState == AgentState.Unconscious)
            {
                _bettingHandler.AddDeadParticipant(affectedAgent.Character);    
            }
        }

        public void ToggleBettingMenu(bool show)
        {
            if (_isActive == show)
            {
                return;
            }
            _isActive = show;
            if (show)
            {
                
                MBCommon.PauseGameEngine();
                Game.Current.GameStateManager.RegisterActiveStateDisableRequest(this);
                _gauntletLayer = new GauntletLayer("BettingMenu", ViewOrderPriority);
                _gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
                _gauntletLayer.InputRestrictions.SetInputRestrictions(true, InputUsageMask.All);
                _movie = _gauntletLayer.LoadMovie("BettingExpandedBettingWidget", _dataSource);
                
                _gauntletLayer.IsFocusLayer = true;
                
                ScreenManager.TopScreen.AddLayer(_gauntletLayer);
                ScreenManager.TrySetFocus(_gauntletLayer);
            }
            else
            {

                MBCommon.UnPauseGameEngine();
                Game.Current.GameStateManager.UnregisterActiveStateDisableRequest(this);
                
                _gauntletLayer.InputRestrictions.ResetInputRestrictions();
                ScreenManager.TopScreen.RemoveLayer(_gauntletLayer);
                _gauntletLayer = null;
                _movie = null;
            }
        }
        
        
        private TournamentBehavior _tournamentBehavior;
        private BettingHandler _bettingHandler;
        private TournamentMatch.MatchState _lastMatchState;
        private BettingVM _dataSource;
        private GauntletLayer _gauntletLayer;
        private GauntletMovieIdentifier _movie;
        private bool _isActive;
    }
}