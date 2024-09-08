using UnityEngine;
using UnityEngine.EventSystems;

namespace CardMatch.Card
{
    [RequireComponent(typeof(CardFlip))]
    public class CardView : MonoBehaviour, IPointerClickHandler
    {
        CardFlip _cardFlip;

        void Awake()
        {
            _cardFlip = GetComponent<CardFlip>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _cardFlip.FlipCard();
        }
    }
}