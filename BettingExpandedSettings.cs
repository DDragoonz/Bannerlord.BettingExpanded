using System.Collections.Generic;
using BettingExpanded.BettingLogic;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using MCM.Common;
using TaleWorlds.Core;

namespace BettingExpanded
{

    public class BettingExpandedSettings : AttributeGlobalSettings<BettingExpandedSettings>
    {
        public override string Id => "BettingExpanded";
        public override string DisplayName => "Betting Expanded";
        public override string FolderName => "BettingExpanded";
        public override string FormatType => "json2";
        
        [SettingPropertyBool("{=BetEx_00001}Enable Debug", Order = 0, RequireRestart = false, HintText = "{=BetEx_00002}Show debug Message")]
        [SettingPropertyGroup("{=BetEx_00030}Debug", GroupOrder = 0)]
        public bool EnableDebug{ get; set; } = false;
        
        [SettingPropertyBool("{=BetEx_00017}Enable Betting", Order = 0, RequireRestart = false, HintText = "{=BetEx_00018}Enable / Disable Betting system", IsToggle = true)]
        [SettingPropertyGroup("{=BetEx_00031}Betting", GroupOrder = 1)]
        public bool BettingModeToggle{ get; set; } = true;
        
        [SettingPropertyInteger("{=BetEx_00003}Betting Price", 10, 1000, "0 Denars", Order = 1, RequireRestart = false, HintText = "{=BetEx_00004}Betting buyout price")]
        [SettingPropertyGroup("{=BetEx_00031}Betting", GroupOrder = 1)]
        public int BetPrice{ get; set; } = 50;
        
        [SettingPropertyInteger("{=BetEx_00005}Max Bet", 1, 100, "0",Order = 1, RequireRestart = false, HintText = "{=BetEx_00006}Max Betting Allowed (double if having Deep Pocket Perk)")]
        [SettingPropertyGroup("{=BetEx_00031}Betting", GroupOrder = 1)]
        public int MaxBet{ get; set; } = 20;
        
        [SettingPropertyFloatingInteger("{=BetEx_00007}Prize Pool Tax", 0f, 1f, "#0%", Order = 1, RequireRestart = false, HintText = "{=BetEx_00008}Percentage of prize pool will be cut to this amount before split to betting winner")]
        [SettingPropertyGroup("{=BetEx_00031}Betting", GroupOrder = 1)]
        public float PrizePoolTax{ get; set; } = 0.01f;  
        
        [SettingPropertyFloatingInteger("{=BetEx_00009}Settlement Prosperity to NPC Betting Ratio", 0.01f, 1f, "#0%", Order = 1, RequireRestart = false, HintText = "{=BetEx_00010}Percentage of settlement prosperity will be converted as purchased bet from NPC. example : 5000 prosperity with 10% setting resulting in 500 total bet")]
        [SettingPropertyGroup("{=BetEx_00031}Betting", GroupOrder = 1)]
        public float MaxNpcBetRatio{ get; set; } = 0.1f;  
        
        [SettingPropertyBool("{=BetEx_00011}Give Prize Tax to Settlement", Order = 0, RequireRestart = false, HintText = "{=BetEx_00012}Should tax from total prize pool be added to settlement gold?")]
        [SettingPropertyGroup("{=BetEx_00031}Betting", GroupOrder = 1)]
        public bool GiveTaxToSettlement{ get; set; } = true;
        
        [SettingPropertyBool("{=BetEx_00025}Hide result if not betting", Order = 0, RequireRestart = false, HintText = "{=BetEx_00026}Hide betting result UI if you're not betting to anyone")]
        [SettingPropertyGroup("{=BetEx_00031}Betting", GroupOrder = 1)]
        public bool HideResultIfNotBetting{ get; set; } = false;

        [SettingPropertyDropdown("{=BetEx_00201}All Score", Order = 0, RequireRestart = false, HintText = "{=BetEx_00221}All betting score can be claimed")]
        [SettingPropertyGroup("{=BetEx_00031}Betting/{=BetEx_00033}Mode" , GroupOrder = 2)]
        public Dropdown<string> AllScore { get; set; } = new Dropdown<string>(BettingModeRuleString, selectedIndex: 2);
        
        [SettingPropertyDropdown("{=BetEx_00202}Highest Score", Order = 1, RequireRestart = false, HintText = "{=BetEx_00222}Only highest score can be claimed. if 2 participants score is 53 and 52, only score 53 is win")]
        [SettingPropertyGroup("{=BetEx_00031}Betting/{=BetEx_00033}Mode" , GroupOrder = 2)]
        public Dropdown<string> HighestScore { get; set; } = new Dropdown<string>(BettingModeRuleString, selectedIndex: 0);
        
        [SettingPropertyDropdown("{=BetEx_00203}Highest Score (Rounded)", Order = 1, RequireRestart = false, HintText = "{=BetEx_00223}Only highest score can be claimed. if 2 participants score is 53 and 52, both is win (both rounded to 50)")]
        [SettingPropertyGroup("{=BetEx_00031}Betting/{=BetEx_00033}Mode" , GroupOrder = 2)]
        public Dropdown<string> HighestScoreRounded { get; set; } = new Dropdown<string>(BettingModeRuleString, selectedIndex: 0);
        
        [SettingPropertyDropdown("{=BetEx_00204}Alive Only", Order = 1, RequireRestart = false, HintText = "{=BetEx_00224}Only score from participants whom alive in the end of battle can be claimed")]
        [SettingPropertyGroup("{=BetEx_00031}Betting/{=BetEx_00033}Mode" , GroupOrder = 2)]
        public Dropdown<string> AliveOnly { get; set; } = new Dropdown<string>(BettingModeRuleString, selectedIndex: 0);
        
        [SettingPropertyDropdown("{=BetEx_00205}Winning Team", Order = 1, RequireRestart = false, HintText = "{=BetEx_00225}Only score from the winning team can be claimed")]
        [SettingPropertyGroup("{=BetEx_00031}Betting/{=BetEx_00033}Mode" , GroupOrder = 2)]
        public Dropdown<string> TeamWinner { get; set; } = new Dropdown<string>(BettingModeRuleString, selectedIndex: 0);
        
        [SettingPropertyDropdown("{=BetEx_00206}Qualified Participant", Order = 1, RequireRestart = false, HintText = "{=BetEx_00226}Only score from qualified participant can be claimed")]
        [SettingPropertyGroup("{=BetEx_00031}Betting/{=BetEx_00033}Mode" , GroupOrder = 2)]
        public Dropdown<string> Qualified { get; set; } = new Dropdown<string>(BettingModeRuleString, selectedIndex: 0);
        
        [SettingPropertyDropdown("{=BetEx_00208}Best of X", Order = 1, RequireRestart = false, HintText = "{=BetEx_00228}Only score from top X position can be claimed. 16 participants = top 4, 8 participants = top 3. 4 participants = top 2. 2 participants = top 1")]
        [SettingPropertyGroup("{=BetEx_00031}Betting/{=BetEx_00033}Mode" , GroupOrder = 2)]
        public Dropdown<string> BestOf { get; set; } = new Dropdown<string>(BettingModeRuleString, selectedIndex: 0);

        [SettingPropertyDropdown("{=BetEx_00013}Qualification Mode", Order = 0, RequireRestart = false, HintText = "{=BetEx_00014}Individual : if you have high score but your team lose, you still can go to next round")]
        [SettingPropertyGroup("{=BetEx_00032}Match" , GroupOrder = 3)]
        public Dropdown<string> QualificationMode { get; set; } = new Dropdown<string>(new string[]{"{=BetEx_00043}Individual","{=BetEx_00044}Team (Vanilla)"}, selectedIndex: 0);
        
        [SettingPropertyBool("{=BetEx_00015}Simple Rounds", Order = 0, RequireRestart = false, HintText = "{=BetEx_00016}If true, round will divided by 16P(Participants)-4T(Teams), 8P-4T, 4P-4T, 2P-2T. Removing non player match")]
        [SettingPropertyGroup("{=BetEx_00032}Match", GroupOrder = 3)]
        public bool UseSimpleRounds{ get; set; } = false;

        [SettingPropertyBool("{=BetEx_00027}Headshot Bonus", Order = 0, RequireRestart = false, HintText = "{=BetEx_00027}Enable / Disable bonus +10 score for headshot")]
        [SettingPropertyGroup("{=BetEx_00034}Scoring", GroupOrder = 4)]
        public bool HeadshotScoreEnabled { get; set; } = true;
        
        [SettingPropertyBool("{=BetEx_00019}Bonus For Ranged Kill", Order = 1, RequireRestart = false, HintText = "{=BetEx_00020}Bonus +10 score for ranged kill. Note that betting odds is calculated without considering NPC ranged kill", IsToggle = true)]
        [SettingPropertyGroup("{=BetEx_00034}Scoring", GroupOrder = 4)]
        public bool UseRangedScoring{ get; set; } = true;
        
        [SettingPropertyFloatingInteger("{=BetEx_00021}Minimum Ranged Kill Distance", 5f, 200f, "0.0", Order = 2, RequireRestart = false, HintText = "{=BetEx_00022}Minimum distance for ranged kill to gain extra +10 score")]
        [SettingPropertyGroup("{=BetEx_00034}Scoring", GroupOrder = 4)]
        public float MinRangedKillDistanceOnFoot{ get; set; } = 20f;
        
        [SettingPropertyFloatingInteger("{=BetEx_00023}Minimum Ranged Kill Distance (On Horse)", 5f, 200f, "0.0", Order = 2, RequireRestart = false, HintText = "{=BetEx_00024}Minimum distance for ranged kill while on horse back to gain extra +10 score")]
        [SettingPropertyGroup("{=BetEx_00034}Scoring", GroupOrder = 4)]
        public float MinRangedKillDistanceOnHorse{ get; set; } = 10f;

        public List<BettingMode> GetBettingMode(bool isPlayerParticipating)
        {
            List <BettingMode> result = new List<BettingMode>();
            
            if (CompareBettingMode(Instance.AllScore.SelectedIndex,isPlayerParticipating)) result.Add(BettingMode.AllScore);
            if (CompareBettingMode(Instance.HighestScore.SelectedIndex,isPlayerParticipating)) result.Add(BettingMode.HighestScore);
            if (CompareBettingMode(Instance.HighestScoreRounded.SelectedIndex,isPlayerParticipating)) result.Add(BettingMode.HighestScoreRounded);
            if (CompareBettingMode(Instance.AliveOnly.SelectedIndex,isPlayerParticipating)) result.Add(BettingMode.AliveOnly);
            if (CompareBettingMode(Instance.TeamWinner.SelectedIndex,isPlayerParticipating)) result.Add(BettingMode.TeamWinner);
            if (CompareBettingMode(Instance.Qualified.SelectedIndex,isPlayerParticipating)) result.Add(BettingMode.Qualified);
            if (CompareBettingMode(Instance.BestOf.SelectedIndex,isPlayerParticipating)) result.Add(BettingMode.BestOf);
            
            return result;
        }

        private static bool CompareBettingMode(int selectedIndex, bool playerParticipating)
        {
            if (selectedIndex == 0) return false;
            if (selectedIndex == 1) return playerParticipating;
            return true;

        } 
        

        private static string[] BettingModeRuleString = new string[]
        {
            "{=BetEx_00040}Disabled",
            "{=BetEx_00041}Only if Player Participate",
            "{=BetEx_00042}Enabled"
        };
    }
}