using System.Collections.Generic;
using CardMatch.Card;
using UnityEngine;

namespace CardMatch.Board
{
    [System.Serializable]
    public struct CardBoardPosition
    {
        public int Row;
        public int Col;
    }
    
    [System.Serializable]
    public struct BoardConfiguration
    {
        public int CardsCount;
        public int RowsCount;
        public int ColsCount;
        public CardBoardPosition[] InvisibleCards;
    }

    [CreateAssetMenu(fileName = "BoardConfig", menuName = "CardMatch/BoardConfig")]
    public class BoardConfig : ScriptableObject
    {
        public BoardConfiguration[] Configurations;
        public List<CardView> Cards = new ();
    }
}