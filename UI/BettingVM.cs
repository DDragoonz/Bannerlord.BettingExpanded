using TaleWorlds.Library;
using TaleWorlds.ScreenSystem;

namespace BettingExpanded.UI
{
    public class BettingExpandedBettingVM : ViewModel
    {

        public BettingExpandedBettingVM()
        {
            
            _participantBetList = new MBBindingList<BettingExpandedBettingParticipantVM>();
            for (int i = 0; i < 16; i++)
            {
                _participantBetList.Add(new BettingExpandedBettingParticipantVM());    
            }
            
        }
        
        public void Close()
        {
            ScreenManager.PopScreen();
            
        }
        
        [DataSourceProperty]
        public MBBindingList<BettingExpandedBettingParticipantVM> ParticipantBetList
        {
            get
            {
                return this._participantBetList;
            }
            set
            {
                bool flag = value != this._participantBetList;
                if (flag)
                {
                    this._participantBetList = value;
                    base.OnPropertyChangedWithValue<MBBindingList<BettingExpandedBettingParticipantVM>>(value, "ParticipantBetList");
                }
            }
        }

        private MBBindingList<BettingExpandedBettingParticipantVM> _participantBetList;
    }
}