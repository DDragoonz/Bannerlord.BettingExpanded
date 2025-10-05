using System.Collections.Generic;
using BettingExpanded.BettingLogic;
using SandBox.ViewModelCollection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.ImageIdentifiers;
using TaleWorlds.Library;

namespace BettingExpanded.UI
{
    
    public class BettingParticipantVM : ViewModel
    {
        public BettingParticipantVM(TournamentParticipant participant, BettingHandler bettingHandler)
        {
	        
	        _bettingData = bettingHandler.GetBettingDataList(participant);;
	        _isHovered = false;
	        _bettingHandler = bettingHandler;
            _participantBetAmount = new MBBindingList<BettingInputVM>();
            for (int i = 0; i < _bettingData.Count; i++)
            {
                _participantBetAmount.Add(new BettingInputVM(_bettingData[i],bettingHandler));    
            }
            Refresh(participant);
        }
        
        public override void RefreshValues()
        {
	        base.RefreshValues();
	        if (IsInitialized)
	        {
		        Refresh(_participant);
	        }
        }
        
        public void Refresh(TournamentParticipant participant)
        {
	        _participant = participant;
	        State = ((participant == null) ? 0 : ((participant.Character == CharacterObject.PlayerCharacter) ? 2 : 1));
	        IsInitialized = true;
	        if (participant != null)
	        {
		        Name = participant.Character.Name.ToString();
		        CharacterCode characterCode = SandBoxUIHelper.GetCharacterCode(participant.Character, false);
		        Visual = new CharacterImageIdentifierVM(characterCode);
		        IsValid = true;
	        }
        }
        
        public void ExecuteOpenEncyclopedia()
        {
	        TournamentParticipant participant = _participant;
	        if (((participant != null) ? participant.Character : null) != null)
	        {
		        Campaign.Current.EncyclopediaManager.GoToLink(_participant.Character.EncyclopediaLink);
	        }
        }
        
        public void Refresh()
        {
	        OnPropertyChanged("Name");
	        OnPropertyChanged("Visual");
	        OnPropertyChanged("Score");
	        OnPropertyChanged("State");
	        OnPropertyChanged("TeamColor");
	        OnPropertyChanged("IsDead");
        }

        public void OnHoverBegin()
        {
	        _isHovered = true;
	        OnPropertyChanged("BackgroundAlpha");
        }
        
        public void OnHoverEnd()
        {
	        _isHovered = false;
	        OnPropertyChanged("BackgroundAlpha");
        }

        [DataSourceProperty] 
        public string ParticipantName => _participant.Character.Name.ToString();

        public string ParticipantBrush => _participant.Character.IsPlayerCharacter ? "Tournament.MainHero.Participant.Text" : "Tournament.Participant.Text";

        [DataSourceProperty]
        public MBBindingList<BettingInputVM> ParticipantBetAmount
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

        [DataSourceProperty]
        public float BackgroundAlpha
        {
	        get
	        {
		        return _isHovered ? 1 : 0.5f;
	        }
        }

		[DataSourceProperty]
		public bool IsInitialized
		{
			get
			{
				return _isInitialized;
			}
			set
			{
				if (value != _isInitialized)
				{
					_isInitialized = value;
					OnPropertyChangedWithValue(value, "IsInitialized");
				}
			}
		}

		[DataSourceProperty]
		public bool IsValid
		{
			get
			{
				return _isValid;
			}
			set
			{
				if (value != _isValid)
				{
					_isValid = value;
					OnPropertyChangedWithValue(value, "IsValid");
				}
			}
		}


		[DataSourceProperty]
		public Color TeamColor
		{
			get
			{
				if (_bettingHandler.CanPlaceBetting())
				{
					return Color.FromUint(_participant.Team.TeamColor);
				}
				else
				{
					return _bettingHandler.IsWinner(_participant) ? Colors.Yellow : Colors.Gray;
				}
			}
		}
		
		[DataSourceProperty]
		public ImageIdentifierVM Visual
		{
			get
			{
				return _visual;
			}
			set
			{
				if (value != _visual)
				{
					_visual = value;
					OnPropertyChangedWithValue<ImageIdentifierVM>(value, "Visual");
				}
			}
		}
		
		[DataSourceProperty]
		public int State
		{
			get
			{
				return _state;
			}
			set
			{
				if (value != _state)
				{
					_state = value;
					OnPropertyChangedWithValue(value, "State");
				}
			}
		}
		
		[DataSourceProperty]
		public bool IsQualifiedForNextRound
		{
			get
			{
				return _isQualifiedForNextRound;
			}
			set
			{
				if (value != _isQualifiedForNextRound)
				{
					_isQualifiedForNextRound = value;
					OnPropertyChangedWithValue(value, "IsQualifiedForNextRound");
				}
			}
		}
		
		[DataSourceProperty]
		public int Score
		{
			get
			{
				return _participant.Score;
			}
		}
		
		[DataSourceProperty]
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				if (value != _name)
				{
					_name = value;
					OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}


		private bool _isInitialized;

		private bool _isValid;

		private string _name = "";

		private bool _isQualifiedForNextRound;

		private int _state = -1;

		private ImageIdentifierVM _visual;

		private bool _isHovered;


		private MBBindingList<BettingInputVM> _participantBetAmount;
        private TournamentParticipant _participant;
        private List<BettingData> _bettingData;
        private readonly BettingHandler _bettingHandler;
    }
}