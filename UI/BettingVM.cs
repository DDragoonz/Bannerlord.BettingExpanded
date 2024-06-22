
using System.Collections.Generic;
using BettingExpanded.BettingLogic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;


namespace BettingExpanded.UI
{
    public class BettingVM : ViewModel
    {

        public BettingVM(BettingMissionView missionView)
        {
            _missionView = missionView;
        }

        public BettingVM()
        {
            
        }

        public void Init(BettingHandler bettingHandler)
        {
            if (_bettingHandler != null)
            {
                _bettingHandler.OnBettingDataChanged -= OnBettingDataChanged;
                _bettingHandler.OnBettingWinnerUpdated -= OnBettingWinnerUpdated;
            }

            if (!bettingHandler.IsBettingEnabled)
            {
                _bettingHandler = null;
                return;
            }
            _bettingHandler = bettingHandler;
            _bettingHandler.OnBettingDataChanged += OnBettingDataChanged;
            _bettingHandler.OnBettingWinnerUpdated += OnBettingWinnerUpdated;
            _participantBetList = new MBBindingList<BettingParticipantVM>();
            _bettingLogList = new MBBindingList<BettingSimpleTextVM>();
            List<TournamentParticipant> participants = _bettingHandler.GetParticipant();

            foreach (TournamentParticipant participant in participants)
            {
                _participantBetList.Add(new BettingParticipantVM(participant,_bettingHandler));
            }
            
            _bettingLogList.Add(new BettingSimpleTextVM(new TextObject("{=BetEx_00134}Place your bet to any participants. Hold shift key to add / remove 5. Hold ctrl to add / remove max.").ToString()));
        }
        
        public void Close()
        {
            if (_bettingHandler != null && _bettingHandler.CanPlaceBetting())
            {
                _bettingHandler.PurchaseBet();
            }
            _missionView.ToggleBettingMenu(false);
        }

        [DataSourceProperty] 
        public string BettingTitle => new TextObject(_bettingHandler.CanPlaceBetting() ? "{=BetEx_00100}Betting Time" : "{=BetEx_00101}Betting Result").ToString();
        
        [DataSourceProperty] 
        public string Participants => new TextObject("{=BetEx_00102}Participant").ToString();
        
        [DataSourceProperty] 
        public string Score10 => new TextObject("{=BetEx_00103}10 - 19 pts").ToString();
        
        [DataSourceProperty] 
        public string Score20 => new TextObject("{=BetEx_00104}20 - 29 pts").ToString();
        
        [DataSourceProperty] 
        public string Score30 => new TextObject("{=BetEx_00105}30 - 39 pts").ToString();
        
        [DataSourceProperty] 
        public string Score40 => new TextObject("{=BetEx_00106}40 - 49 pts").ToString();
        
        [DataSourceProperty] 
        public string Score50 => new TextObject("{=BetEx_00107}50 - 59 pts").ToString();
        
        [DataSourceProperty] 
        public string Score60 => new TextObject("{=BetEx_00108}60 - 69 pts").ToString();
        
        [DataSourceProperty] 
        public string Score70 => new TextObject("{=BetEx_00109}70 - 79 pts").ToString();
        
        [DataSourceProperty] 
        public string Score80 => new TextObject("{=BetEx_00110}80 - 89 pts").ToString();
        
        [DataSourceProperty] 
        public string Score90 => new TextObject("{=BetEx_00111}90 - 99 pts").ToString();
        
        [DataSourceProperty] 
        public string Score100 => new TextObject("{=BetEx_00112}100+ pts").ToString();
        
        [DataSourceProperty] 
        public string LocBettingMode => new TextObject("{=BetEx_00113}Betting Mode").ToString();
        
        [DataSourceProperty] 
        public string LocBetLeft => new TextObject("{=BetEx_00114}Bet Left").ToString();
        
        [DataSourceProperty] 
        public string LocBetPrice => new TextObject("{=BetEx_00115}Bet Price").ToString();
        
        [DataSourceProperty] 
        public string LocTax => new TextObject("{=BetEx_00116}Tax").ToString();
        
        [DataSourceProperty] 
        public string LocPrize => new TextObject("{=BetEx_00117}Prize Pool (after tax)").ToString();
        
        [DataSourceProperty] 
        public string YourGold => new TextObject("{=BetEx_00118}Your Gold").ToString();
        
        [DataSourceProperty] 
        public string Expected => new TextObject("{=BetEx_00119}Expected").ToString();

        

        [DataSourceProperty]
        public string BettingMode => _bettingHandler.GetCurrentBettingModeString();
        
        [DataSourceProperty]
        public int BettingLeft => _bettingHandler.BetLeft;

        [DataSourceProperty]
        public int BettingPrice => BettingExpandedSettings.Instance.BetPrice;
        
        [DataSourceProperty]
        public int PrizePool => _bettingHandler.GetPrizePoolAfterTax(out int prizePoolTax);

        [DataSourceProperty] 
        public string Tax => "" + (BettingExpandedSettings.Instance.PrizePoolTax * 100) + "%";

        [DataSourceProperty]
        public int PlayerGold => _bettingHandler.CanPlaceBetting() ? Hero.MainHero.Gold : Hero.MainHero.Gold - _bettingHandler.GetPlayerWinningPrize();
        
        [DataSourceProperty]
        public string PayOrGetText => new TextObject(_bettingHandler.CanPlaceBetting() ? "{=BetEx_00120}You Pay" : "{=BetEx_00121}You Get").ToString();
        
        [DataSourceProperty]
        public string PayOrGetValue => _bettingHandler.CanPlaceBetting() ? "-" + _bettingHandler.GetPlayerBetPrice() : "+" + _bettingHandler.GetPlayerWinningPrize();

        [DataSourceProperty]
        public int TotalGold => _bettingHandler.CanPlaceBetting() ? Hero.MainHero.Gold - _bettingHandler.GetPlayerBetPrice() : Hero.MainHero.Gold;

        [DataSourceProperty]
        public string MatchButtonText => new TextObject(_bettingHandler.CanPlaceBetting() ? "{=BetEx_00122}Start Match" : "{=BetEx_00123}Next Match").ToString();

        [DataSourceProperty]
        public Color BettingModeColor
        {
            get
            {
                switch (_bettingHandler.CurrentBettingMode)
                {
                    case BettingLogic.BettingMode.AllScore:
                        return Colors.White;
                    case BettingLogic.BettingMode.HighestScore:
                        return Colors.Blue;
                    case BettingLogic.BettingMode.HighestScoreRounded:
                        return Colors.Cyan;
                    case BettingLogic.BettingMode.AliveOnly:
                        return Colors.Green;
                    case BettingLogic.BettingMode.TeamWinner:
                        return Colors.Magenta;
                    case BettingLogic.BettingMode.Qualified:
                        return Colors.Red;
                    case BettingLogic.BettingMode.BestOf:
                        return Colors.Yellow;
                }
                return Colors.White;
            }
        }

        [DataSourceProperty]
        public MBBindingList<BettingParticipantVM> ParticipantBetList
        {
            get
            {
                return _participantBetList;
            }
            set
            {
                bool flag = value != _participantBetList;
                if (flag)
                {
                    _participantBetList = value;
                    OnPropertyChangedWithValue(value);
                }
            }
        }

        [DataSourceProperty]
        public MBBindingList<BettingSimpleTextVM> BettingLog
        {
            get
            {
                return _bettingLogList;
            }
            set
            {
                bool flag = value != _bettingLogList;
                if (flag)
                {
                    _bettingLogList = value;
                    OnPropertyChangedWithValue(value);
                }
            }
        }

        private void OnBettingDataChanged()
        {
            OnPropertyChanged("BettingLeft");
            OnPropertyChanged("PrizePool");
            OnPropertyChanged("PayOrGetValue");
            OnPropertyChanged("TotalGold");
        }

        private void OnBettingWinnerUpdated()
        {
            _bettingLogList.Clear();
            
            if (!_bettingHandler.IsBettingEnabled) return;
            
            List<TournamentParticipant> betWinners = _bettingHandler.GetBetWinners();

            if (betWinners.IsEmpty()) return;
            
            int betRewardDivided = PrizePool / betWinners.Count;
             TextObject betResult = new TextObject("{=BetEx_00130}Betting Mode is {BettingMode}. Prize Pool Split to {betWinnersCount} winner")
                 .SetTextVariable("BettingMode", BettingMode)
                 .SetTextVariable("betWinnersCount", betWinners.Count);
             _bettingLogList.Add(new BettingSimpleTextVM(betResult.ToString(),Colors.Magenta));
             TextObject prizePool = new TextObject("{=BetEx_00131}Prize Pool after split : {betRewardDivided}")
                 .SetTextVariable("betRewardDivided", betRewardDivided);
            _bettingLogList.Add(new BettingSimpleTextVM(prizePool.ToString(),Colors.Magenta));

            foreach (TournamentParticipant participant in betWinners)
            {
                int scoreIndex = (int)(participant.Score/10)-1;
                scoreIndex = MBMath.ClampInt(scoreIndex, 0, _bettingHandler.MaxPoint - 1);
                BettingData bettingData = _bettingHandler.GetBettingData(participant, scoreIndex);
                if (bettingData.IsPlayerHasBet())
                {

                    TextObject winBet = new TextObject("{=BetEx_00132}You Win Bet for {CharacterName} {participantScore} pts. Your Share : ({playerBet}/{totalBet}) X {betRewardDivided} = {betReward} Denars")
                        .SetTextVariable("CharacterName", participant.Character.Name.ToString())
                        .SetTextVariable("participantScore", participant.Score)
                        .SetTextVariable("playerBet", bettingData.GetPlayerBet())
                        .SetTextVariable("totalBet", bettingData.GetTotalBet())
                        .SetTextVariable("betRewardDivided", betRewardDivided)
                        .SetTextVariable("betReward", (int)(bettingData.GetPlayerBetPercent() * betRewardDivided));
                    
                    _bettingLogList.Add(new BettingSimpleTextVM(winBet.ToString(),Colors.Yellow));    
                }
                else
                {
                    TextObject loseBet = new TextObject("{=BetEx_00133}{CharacterName}  is Winning Bet with {participantScore} pts. You don't put any bet here")
                        .SetTextVariable("CharacterName", participant.Character.Name.ToString())
                        .SetTextVariable("participantScore", participant.Score);
                    _bettingLogList.Add(new BettingSimpleTextVM(loseBet.ToString()));
                }
                
            }
            OnPropertyChanged("BettingLog");
            
        }

        private MBBindingList<BettingParticipantVM> _participantBetList;
        private BettingHandler _bettingHandler;
        private BettingMissionView _missionView;
        private MBBindingList<BettingSimpleTextVM> _bettingLogList;
    }
}