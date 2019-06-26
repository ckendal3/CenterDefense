
namespace WDIB.Core.States
{
    public class PlayerState
    {
        public PlayerState(uint newID)
        {
            ID = newID;
            score = new PlayerScore { Value = 0 };
        }
        public PlayerState(uint newID, int newScore)
        {
            ID = newID;
            score = new PlayerScore { Value = newScore };
        }

        public uint ID { get; private set; }
        private PlayerScore score { get; set; }

        // add input component

        public int GetScore()
        {
            return score.Value;
        }

        public void IncreaseScore(int increaseAmount)
        {
            score.IncreaseScore(increaseAmount);
        }

        public uint GetPlayerID()
        {
            return ID;
        }
    }

    public struct PlayerScore
    {
        // generic score that is wrote to by any game mode
        public int Value;

        public void IncreaseScore(int amount)
        {
            Value += amount;
        }
    }
}