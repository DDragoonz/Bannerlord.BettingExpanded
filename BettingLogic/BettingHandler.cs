using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace BettingExpanded.BettingLogic
{
	public class BettingHandler
	{
		public int BetLeft = 0;
		public int MaxPoint { get; private set; }
		public bool IsBettingEnabled { get; private set; }
		public BettingMode CurrentBettingMode = BettingMode.AllScore;
		
		public Action OnBettingDataChanged;
		public Action OnBettingWinnerUpdated;

		public int MaxBet
		{
			get
			{
				int maxByGold = Hero.MainHero.Gold / BettingExpandedSettings.Instance.BetPrice;
				int multiplier = 1; 
				if (Hero.MainHero.GetPerkValue(DefaultPerks.Roguery.DeepPockets))
				{
					multiplier = (int)DefaultPerks.Roguery.DeepPockets.PrimaryBonus;
				}
				return Math.Min(maxByGold,BettingExpandedSettings.Instance.MaxBet * multiplier); 
			}
		}

		public BettingHandler(TournamentMatch matchData)
		{
			_currentMatch = matchData;
			_deadparticipant = new List<BasicCharacterObject>();

			int participantCount = _currentMatch.Participants.Count();
			int teamCount = _currentMatch.Teams.Count();
			MaxPoint = GetMaxPossibleScore(participantCount, teamCount);
			ResetBetting();

			int totalNpcBet = GetTotalNpcBet(); 
			
			DistributeBet(totalNpcBet);

		}

		public List<TournamentParticipant> GetParticipant()
		{
			return _betting.IsEmpty() ? new List<TournamentParticipant>() : new List<TournamentParticipant>(_betting.Keys);
		}

		public List<BettingData> GetBettingDataList(TournamentParticipant participant)
		{
			return _betting.ContainsKey(participant) ? _betting[participant] : new List<BettingData>();
		}

		public BettingData GetBettingData(TournamentParticipant participant, int scoreIndex)
		{
			List<BettingData> data = GetBettingDataList(participant);
			if (data.Count == 0 || scoreIndex >= data.Count)
			{
				return null;
			}

			return data[scoreIndex];
		}
		public TournamentMatch GetHandledMatch()
		{
			return _currentMatch;
		}

		public bool CanPlaceBetting()
		{
			return _currentMatch.State != TournamentMatch.MatchState.Finished;
		}
		
		public bool IsPlayerPutAnyBet()
		{
			return BetLeft < MaxBet;
		}

		public void ProcessBetResult()
		{

			if (!IsBettingEnabled)
			{
				return;
			}

			_playerWinningPrize = 0;
			List<TournamentParticipant> betWinners = GetBetWinners(CurrentBettingMode);
			if (betWinners.IsEmpty()) return;
			
			int prizePool = GetPrizePoolAfterTax(out int prizePoolTax);
			
			int dividedPrizePool = prizePool / betWinners.Count;
			
			foreach (TournamentParticipant participant in betWinners)
			{
				int scoreIndex = (participant.Score / 10) - 1;
				scoreIndex = MBMath.ClampInt(scoreIndex, 0, _betting[participant].Count - 1 );
				BettingData bet = _betting[participant][scoreIndex];
				bet.IsBetWinner = true;
				if (!bet.IsPlayerHasBet()) continue;
				int playerPrize = (int)(bet.GetPlayerBetPercent() * dividedPrizePool);
				if (playerPrize != 0)
				{
					GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, playerPrize);
					_playerWinningPrize += playerPrize;
				}
			}

			if (BettingExpandedSettings.Instance.GiveTaxToSettlement && prizePoolTax != 0)
			{
				GiveGoldAction.ApplyForCharacterToSettlement(null, Settlement.CurrentSettlement, prizePoolTax);	
			}
			
		}
		
		
		
		public void AddDeadParticipant(BasicCharacterObject affectedAgentCharacter)
		{
			_deadparticipant.Add(affectedAgentCharacter);
		}

		public bool IsWinner(TournamentParticipant participant)
		{
			if (!IsBettingEnabled)
			{
				return false;
			}
			
			return GetBetWinners(CurrentBettingMode).Contains(participant);
		}

		public int GetPlayerBetPrice()
		{
			int playerBet = MaxBet - BetLeft;
			return playerBet * BettingExpandedSettings.Instance.BetPrice;
		}
		
		public void PurchaseBet()
		{
			int playerBetPrice = GetPlayerBetPrice();
			if (playerBetPrice <= 0)
			{
				return;
			}
			GiveGoldAction.ApplyBetweenCharacters(Hero.MainHero, null, playerBetPrice);
		}
		
		public string GetCurrentBettingModeString()
		{
			switch (CurrentBettingMode)
			{
				case BettingMode.AllScore:
					return new TextObject("{=BetEx_00201}All Score").ToString(); ;
				case BettingMode.HighestScore:
					return new TextObject("{=BetEx_00202}Highest Score").ToString(); ;
				case BettingMode.HighestScoreRounded:
					return new TextObject("{=BetEx_00203}Highest Score (Rounded)").ToString();
				case BettingMode.AliveOnly:
					return new TextObject("{=BetEx_00204}Alive Only").ToString();
				case BettingMode.TeamWinner:
					return new TextObject("{=BetEx_00205}Winning Team").ToString();
				case BettingMode.Qualified:
					return new TextObject("{=BetEx_00206}Qualified Participant").ToString();
				case BettingMode.BestOf:
					int num = (int)Math.Log(_currentMatch.Participants.Count(), 2f);
					TextObject text = new TextObject("{=BetEx_00207}Best of {Num}");
					// TextObject text = new TextObject("{=BetEx_00208}Best of X");
					return  text.SetTextVariable("Num", num).ToString();
			}

			return new TextObject("{=BetEx_00200}Invalid Mode").ToString(); ;
		}

		public int GetTotalNpcBet()
		{
			return (int)(Settlement.CurrentSettlement != null && Settlement.CurrentSettlement.IsTown ?
				Settlement.CurrentSettlement.Town.Prosperity * BettingExpandedSettings.Instance.MaxNpcBetRatio : 500);
		}

		public int GetPrizePool()
		{
			int totalNpcBet = GetTotalNpcBet(); 
			int ticketPrice = BettingExpandedSettings.Instance.BetPrice;
			
			return ((MaxBet - BetLeft) + totalNpcBet) * ticketPrice;
		}

		public int GetPrizePoolAfterTax(out int prizePoolTax)
		{
			int prizePool = GetPrizePool();
			float tax = BettingExpandedSettings.Instance.PrizePoolTax;
			prizePoolTax = (int)(prizePool * tax);
			return prizePool - prizePoolTax;
		}

		public int GetPlayerWinningPrize()
		{
			return _playerWinningPrize;
		}

		public List<TournamentParticipant> GetBetWinners()
		{
			if (!IsBettingEnabled)
			{
				return new List<TournamentParticipant>();
			}
			return GetBetWinners(CurrentBettingMode);
		}

		private List<TournamentParticipant> GetBetWinners(BettingMode bettingMode)
		{

			if (_winnerCache != null)
			{
				return _winnerCache;
			}
			
			_winnerCache = new List<TournamentParticipant>();

			IEnumerable<TournamentParticipant> sortedParticipants = (from x in _currentMatch.Participants
				orderby x.Score descending
				where x.Score >= 10
				select x);

			if (sortedParticipants.IsEmpty())
			{
				OnBettingWinnerUpdated();
				return _winnerCache;
			} 
				

			IEnumerable<TournamentParticipant> sortedParticipantsList = sortedParticipants.ToList();
			int highestScore = sortedParticipantsList.First().Score;
			int highestScoreRounded = (highestScore / 10) * 10;

			switch (bettingMode)
			{
				case BettingMode.AllScore:
					_winnerCache = sortedParticipantsList.ToList();
					break;
				case BettingMode.HighestScore:
					_winnerCache = (from x in sortedParticipantsList
						where x.Score == highestScore
						select x).ToList();
					break;
				case BettingMode.HighestScoreRounded:
					_winnerCache = (from x in sortedParticipantsList
						where (x.Score / 10) * 10 == highestScoreRounded
						select x).ToList();
					break;
				case BettingMode.AliveOnly:
					_winnerCache = (from x in sortedParticipantsList
						where !_deadparticipant.Contains(x.Character)
						select x).ToList();
					break;
				case BettingMode.TeamWinner:
					_winnerCache = (from x in sortedParticipantsList
						where x.Score % 10 >= (_currentMatch.Teams.Count() - 1)
						select x).ToList();

					// rare case where for some reason tournament winner not getting the last point (in 4 teams battle, team winner have 2 score instead 3 score)
					if (_winnerCache.IsEmpty())
					{
						// in that case, we get alive participant and use that team to determine winner
						List<TournamentParticipant> aliveParticipant = (from x in sortedParticipantsList
							where !_deadparticipant.Contains(x.Character)
							select x).ToList();

						// TODO : handle very rare occurence where both participants is dying at the same time. example : both throwing javelin and killed at the same time 
						_winnerCache = (from x in sortedParticipantsList
							where x.Team == aliveParticipant.First().Team
							select x).ToList();
					}
					break;
				case BettingMode.Qualified:
					_winnerCache = _currentMatch.Winners.ToList(); // check if we have winner here?
					break;
				case BettingMode.BestOf:
					_winnerCache = sortedParticipantsList.Take((int)Math.Log(_currentMatch.Participants.Count(),2f)).ToList();
					break;
				default:
					break;
			}

			OnBettingWinnerUpdated();
			return _winnerCache;
		}

		

		private void ResetBetting()
		{
			if (_currentMatch == null)
			{
				return;
			}

			List<BettingMode> modes = BettingExpandedSettings.Instance.GetBettingMode(_currentMatch.IsPlayerParticipating());

			IsBettingEnabled = !modes.IsEmpty();

			if (!IsBettingEnabled) return;

			CurrentBettingMode = modes.GetRandomElement();

			_winnerCache = null;
			BetLeft = MaxBet;
			

			_betting = new Dictionary<TournamentParticipant, List<BettingData>>();
			
			foreach (TournamentParticipant participant in _currentMatch.Participants)
			{
				_betting.Add(participant,new List<BettingData>(Enumerable.Range(1,MaxPoint).Select(x => new BettingData())));
			}
		}

		private void DistributeBet(int totalNpcBet)
		{
			if (!IsBettingEnabled)
			{
				return;
			}
			
			int participantCount = _currentMatch.Participants.Count();
			int teamCount = _currentMatch.Teams.Count();
			string matchID = GetModeString(CurrentBettingMode, participantCount,teamCount );
			

			for (int i = 0; i < totalNpcBet; i++)
			{
				int participantIndex = MBRandom.RandomInt(participantCount);
				TournamentParticipant randomParticipant = _currentMatch.Participants.ElementAt(participantIndex);
				// TODO : implement participant weight based on skill
				IReadOnlyList<float> betWeight = GetPresetBetWeight(matchID);
				int randomBetIndex = GetRandomBetWeight(ref betWeight, MaxPoint); // RandomRange(0,maxPoint);
				
				_betting[randomParticipant][randomBetIndex].AddNpcBet(1);
			}

		}

		private int GetRandomBetWeight(ref IReadOnlyList<float> weightList, int maxPoint)
		{
			if (weightList.Count == 0)
			{
				return MBRandom.RandomInt(maxPoint);
			}
			
			float num = MBRandom.RandomFloat * weightList.Sum();
			for (int index = 0; index < weightList.Count; ++index)
			{
				num -= weightList[index];
				if (num <= 0.0)
				{
					
					return Math.Min(index,maxPoint-1);
				}
			}
			
			return MBRandom.RandomInt(maxPoint);
		}


		
		private readonly TournamentMatch _currentMatch;
		private Dictionary<TournamentParticipant, List<BettingData>> _betting;
		private List<TournamentParticipant> _winnerCache;
		private List<BasicCharacterObject> _deadparticipant;
		private int _playerWinningPrize;

		#region Statics

		public static int GetMaxPossibleScore(int participantCount, int teamCount)
		{
			int maxPoint = 10;
			int maxParticipantPerTeam = participantCount / teamCount;
			int maxPossibleKill = (participantCount - maxParticipantPerTeam);
			int maxScoreMultiplier = 2; // if kill by headshot
			return MathF.Min(maxPoint, maxPossibleKill * maxScoreMultiplier);
		}

		public static IReadOnlyList<float> GetPresetBetWeight(BettingMode mode, int participantCount, int teamCount)
		{
			string modeString = GetModeString(mode, participantCount, teamCount);
			return GetPresetBetWeight(modeString);
		}

		public static IReadOnlyList<float> GetPresetBetWeight(string modeString)
		{
			return PresetBetWeight.ContainsKey(modeString) ? PresetBetWeight[modeString] : new List<float>();
		}

		public static string GetModeString(BettingMode mode, int participantCount, int teamCount)
		{
			return "mode_" + (int)mode + "#" + participantCount + "_" + teamCount;
		}


		// pre generated bet weight
		private static readonly Dictionary<string, IReadOnlyList<float>> PresetBetWeight = new Dictionary<string, IReadOnlyList<float>>
		{
			{ "mode_0#2_2", new List<float> { 0.79f, 0.21f } },
			{ "mode_0#4_2", new List<float> { 0.31f, 0.462f, 0.158f, 0.071f } },
			{ "mode_0#4_4", new List<float> { 0.324f, 0.357f, 0.179f, 0.118f, 0.023f, 0f } },
			{ "mode_0#8_2", new List<float> { 0.3f, 0.345f, 0.16f, 0.122f, 0.047f, 0.016f, 0.004f, 0.006f } },
			{ "mode_0#8_4", new List<float> { 0.264f, 0.329f, 0.157f, 0.126f, 0.069f, 0.038f, 0.013f, 0.003f, 0f, 0f } },
			{ "mode_0#16_4", new List<float> { 0.255f, 0.328f, 0.147f, 0.114f, 0.073f, 0.046f, 0.02f, 0.012f, 0.003f, 0f } },
			{ "mode_1#2_2", new List<float> { 0.481f, 0.519f } },
			{ "mode_1#4_2", new List<float> { 0.132f, 0.34f, 0.33f, 0.198f } },
			{ "mode_1#4_4", new List<float> { 0.013f, 0.336f, 0.352f, 0.208f, 0.072f, 0.019f } },
			{ "mode_1#8_2", new List<float> { 0f, 0.152f, 0.275f, 0.312f, 0.152f, 0.065f, 0.029f, 0.014f } },
			{ "mode_1#8_4", new List<float> { 0f, 0.01f, 0.235f, 0.332f, 0.238f, 0.114f, 0.037f, 0.027f, 0.003f, 0.003f } },
			{ "mode_1#16_4", new List<float> { 0f, 0f, 0.033f, 0.167f, 0.282f, 0.272f, 0.141f, 0.079f, 0.016f, 0.01f } },
			{ "mode_2#2_2", new List<float> { 0.517f, 0.483f } },
			{ "mode_2#4_2", new List<float> { 0.161f, 0.435f, 0.242f, 0.161f } },
			{ "mode_2#4_4", new List<float> { 0.06f, 0.391f, 0.286f, 0.189f, 0.065f, 0.01f } },
			{ "mode_2#8_2", new List<float> { 0f, 0.133f, 0.272f, 0.348f, 0.177f, 0.063f, 0.006f, 0f } },
			{ "mode_2#8_4", new List<float> { 0f, 0.133f, 0.193f, 0.316f, 0.185f, 0.117f, 0.039f, 0.01f, 0.003f, 0.003f } },
			{ "mode_2#16_4", new List<float> { 0f, 0f, 0.026f, 0.206f, 0.243f, 0.237f, 0.129f, 0.09f, 0.045f, 0.013f, 0.005f, 0.005f } },
			{ "mode_3#2_2", new List<float> { 0.509f, 0.491f } },
			{ "mode_3#4_2", new List<float> { 0.163f, 0.365f, 0.25f, 0.221f } },
			{ "mode_3#4_4", new List<float> { 0.171f, 0.255f, 0.284f, 0.244f, 0.033f, 0.015f } },
			{ "mode_3#8_2", new List<float> { 0.123f, 0.282f, 0.258f, 0.16f, 0.08f, 0.067f, 0.031f, 0f } },
			{ "mode_3#8_4", new List<float> { 0.055f, 0.199f, 0.246f, 0.213f, 0.14f, 0.077f, 0.037f, 0.029f, 0.004f, 0f } },
			{ "mode_3#16_4", new List<float> { 0.058f, 0.116f, 0.168f, 0.187f, 0.184f, 0.11f, 0.081f, 0.039f, 0.035f, 0.019f, 0.003f } },
			{ "mode_4#2_2", new List<float> { 0.493f, 0.507f } },
			{ "mode_4#4_2", new List<float> { 0.24f, 0.331f, 0.314f, 0.116f } },
			{ "mode_4#4_4", new List<float> { 0.152f, 0.296f, 0.276f, 0.188f, 0.056f, 0.032f } },
			{ "mode_4#8_2", new List<float> { 0.239f, 0.293f, 0.181f, 0.152f, 0.08f, 0.04f, 0.011f, 0.004f } },
			{ "mode_4#8_4", new List<float> { 0.178f, 0.221f, 0.168f, 0.212f, 0.103f, 0.072f, 0.028f, 0.009f, 0.006f, 0.003f } },
			{ "mode_4#16_4", new List<float> { 0.19f, 0.26f, 0.157f, 0.148f, 0.085f, 0.076f, 0.035f, 0.026f, 0.018f, 0.004f, 0.002f } },
			{ "mode_5#2_2", new List<float> { 0.519f, 0.481f } },
			{ "mode_5#4_2", new List<float> { 0.256f, 0.417f, 0.22f, 0.107f } },
			{ "mode_5#4_4", new List<float> { 0.241f, 0.367f, 0.213f, 0.13f, 0.039f, 0.009f } },
			{ "mode_5#8_2", new List<float> { 0.242f, 0.362f, 0.193f, 0.122f, 0.051f, 0.02f, 0.007f, 0.002f } },
			{ "mode_5#8_4", new List<float> { 0.23f, 0.374f, 0.148f, 0.137f, 0.059f, 0.03f, 0.017f, 0.004f, 0.002f, 0f } },
			{ "mode_5#16_4", new List<float> { 0.195f, 0.365f, 0.159f, 0.12f, 0.076f, 0.038f, 0.029f, 0.009f, 0.005f, 0.002f, 0.002f } },
			{ "mode_6#2_2", new List<float> { 0.494f, 0.506f } },
			{ "mode_6#4_2", new List<float> { 0.259f, 0.435f, 0.212f, 0.094f } },
			{ "mode_6#4_4", new List<float> { 0.231f, 0.431f, 0.176f, 0.122f, 0.024f, 0.016f } },
			{ "mode_6#8_2", new List<float> { 0.123f, 0.393f, 0.198f, 0.162f, 0.067f, 0.036f, 0.014f, 0.008f } },
			{ "mode_6#8_4", new List<float> { 0.087f, 0.382f, 0.22f, 0.15f, 0.101f, 0.04f, 0.014f, 0.003f, 0.002f, 0.002f } },
			{ "mode_6#16_4", new List<float> { 0.005f, 0.183f, 0.268f, 0.231f, 0.13f, 0.101f, 0.037f, 0.032f, 0.009f, 0.003f, 0.001f } },
		};


		#endregion



	}
}