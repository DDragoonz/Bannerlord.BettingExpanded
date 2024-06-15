using System;
using TaleWorlds.Library;

namespace BettingExpanded.UI
{
    public class BettingExpandedBettingInputVM : ViewModel
    {

        private static  Random _random = new Random();
        
        public BettingExpandedBettingInputVM()
        {
            _playerBetAmount = _random.Next(0, 999);
            _npcBetAmount = _random.Next(0, 999);
        }
        
        [DataSourceProperty]
        public int PlayerBetAmount
        {
            get => _playerBetAmount;
            set
            {
                if (value != _playerBetAmount)
                {
                    _playerBetAmount = value;
                    OnPropertyChangedWithValue(value);
                }
            }
        }
        
        [DataSourceProperty]
        public int NpcBetAmount
        {
            get => _npcBetAmount;
            set
            {
                if (value != _npcBetAmount)
                {
                    _npcBetAmount = value;
                    OnPropertyChangedWithValue(value);
                }
            }
        }

        private int _playerBetAmount = 0;
        private int _npcBetAmount = 0;
    }
}