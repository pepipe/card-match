namespace CardMatch.SaveGame
{
    [System.Serializable]
    public class CardState
    {
        public int CardId;
        public int CardIndex;
        public bool IsFaceUp;
        public bool IsActive;
    }
}