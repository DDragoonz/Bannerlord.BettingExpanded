using TaleWorlds.Library;

namespace BettingExpanded.BettingLogic
{
    public class BettingExpandedBettingHandler
    {
        public static int GetMaxPossibleScore(int participantCount, int teamNum)
        {
            int maxPoint = 10;
            int maxParticipantPerTeam = participantCount / teamNum;
            int maxPossibleKill = (participantCount - maxParticipantPerTeam);
            int maxScoreMultiplier = 2; // if kill by headshot
            return MathF.Min(maxPoint,maxPossibleKill * maxScoreMultiplier);
        }
    }
}