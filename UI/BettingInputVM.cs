
using System;
using BettingExpanded.BettingLogic;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace BettingExpanded.UI
{
    public class BettingInputVM : ViewModel
    {

        public BettingInputVM(BettingData bettingData, BettingHandler bettingHandler)
        {
            _bettingData = bettingData;
            _bettingHandler = bettingHandler;
        }
        
        [DataSourceProperty]
        public int PlayerBetAmount => _bettingData.GetPlayerBet();

        [DataSourceProperty]
        public int NpcBetAmount => _bettingData.GetTotalBet();

        [DataSourceProperty] 
        public bool CanPlaceBet => _bettingHandler.CanPlaceBetting();

        [DataSourceProperty]
        public Color BetTextColor
        {
            get
            {
                if (_bettingHandler.CanPlaceBetting())
                {
                    return _bettingData.IsPlayerHasBet() ? Colors.Cyan : Colors.White;// Color.FromUint(0x8CDBB5FF) : Color.FromUint(0xF4E1C4FF);
                }
                else
                {
                    return _bettingData.IsBetWinner ? Colors.Yellow : Colors.Gray;
                }
            }
        }

        public void IncreasePlayerBet()
        {
            bool useFiveStackModifier = Input.IsKeyDown(InputKey.LeftShift) || Input.IsKeyDown(InputKey.RightShift);
            bool useAllModifier = Input.IsKeyDown(InputKey.LeftControl) || Input.IsKeyDown(InputKey.RightControl);

            int maxAllowedBet = _bettingHandler.BetLeft;
            if (maxAllowedBet == 0)
            {
                return;
            }
            
            int increaseAmount = useAllModifier ? maxAllowedBet : useFiveStackModifier ? 5 : 1;
            increaseAmount = Math.Min(maxAllowedBet, increaseAmount);

            _bettingData.AddPlayerBet(increaseAmount);
            _bettingHandler.BetLeft -= increaseAmount;
            _bettingHandler.OnBettingDataChanged();
            OnPropertyChanged("PlayerBetAmount");
            OnPropertyChanged("NpcBetAmount");
            OnPropertyChanged("BetTextColor");
        }
        
        public void DecreasePlayerBet()
        {
            bool useFiveStackModifier = Input.IsKeyDown(InputKey.LeftShift) || Input.IsKeyDown(InputKey.RightShift);
            bool useAllModifier = Input.IsKeyDown(InputKey.LeftControl) || Input.IsKeyDown(InputKey.RightControl);

            int currentBet = _bettingData.GetPlayerBet();
            if (currentBet == 0)
            {
                return;
            }
            
            int decreaseAmount = useAllModifier ? currentBet : useFiveStackModifier ? 5 : 1;
            decreaseAmount = Math.Min(currentBet, decreaseAmount);

            _bettingData.AddPlayerBet(decreaseAmount * -1);
            _bettingHandler.BetLeft += decreaseAmount;
            _bettingHandler.OnBettingDataChanged();
            OnPropertyChangedWithValue(PlayerBetAmount,"PlayerBetAmount");
            OnPropertyChangedWithValue(NpcBetAmount,"NpcBetAmount");
            OnPropertyChangedWithValue(BetTextColor,"BetTextColor");
        }
        
        

        private BettingData _bettingData;
        private BettingHandler _bettingHandler;
    }
}