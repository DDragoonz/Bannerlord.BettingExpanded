using System.Collections.Generic;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.Library;

namespace BettingExpanded.BettingLogic
{
    public class BettingData
    {
        
        public int GetTotalBet()
        {
            return _playerBet + _npcBet;
        }

        public float GetPlayerBetPercent()
        {
            return (float)_playerBet / GetTotalBet();
        }

        public bool IsPlayerHasBet()
        {
            return _playerBet > 0;
        }
        
        public int GetPlayerBet()
        {
            return _playerBet;
        }

        public void AddPlayerBet(int value)
        {
            _playerBet += value;
            if (_playerBet < 0)
            {
                _playerBet = 0;
            }
        }

        public void AddNpcBet(int value)
        {
            _npcBet += value;
        }

        public void SetNpcBet(int value)
        {
            _npcBet = value;
        }

        public int GetNpcBet()
        {
            return _npcBet;
        }

        public void Reset()
        {
            _playerBet = 0;
            _npcBet = 0;
        }

        public bool IsBetWinner;

        private int _playerBet;
        private int _npcBet;


    }
    
}