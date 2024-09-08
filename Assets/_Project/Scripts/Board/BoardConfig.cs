using System.Collections.Generic;
using CardMatch.Card;
using UnityEngine;

namespace CardMatch.Board
{
    [CreateAssetMenu(fileName = "BoardConfig", menuName = "CardMatch/BoardConfig")]
    public class BoardConfig : ScriptableObject
    {
        public List<CardView> Cards = new ();
    }
}