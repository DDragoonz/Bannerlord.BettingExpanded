using System.Diagnostics.CodeAnalysis;
using TaleWorlds.Library;

namespace BettingExpanded.UI
{
    
    public class BettingExpandedBettingParticipantVM : ViewModel
    {
        public BettingExpandedBettingParticipantVM()
        {
            _participantBetAmount = new MBBindingList<BettingExpandedBettingInputVM>();
            for (int i = 0; i < 10; i++)
            {
                _participantBetAmount.Add(new BettingExpandedBettingInputVM());    
            }
        }
        
        [DataSourceProperty]
        public MBBindingList<BettingExpandedBettingInputVM> ParticipantBetAmount
        {
            get
            {
                return _participantBetAmount;
            }
            set
            {
                bool flag = value != _participantBetAmount;
                if (flag)
                {
                    _participantBetAmount = value;
                    OnPropertyChangedWithValue(value);
                }
            }
        }

        private MBBindingList<BettingExpandedBettingInputVM> _participantBetAmount;
    }
}