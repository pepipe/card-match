using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CardMatch.Card
{
    [RequireComponent(typeof(CardFlip))]
    public class CardView : MonoBehaviour, IPointerClickHandler
    {
        public int CardValue { get; set; }
        public event Action<CardView> OnCardClicked;

        CardFlip _cardFlip;
        bool _cardClicked;

        void Awake()
        {
            _cardFlip = GetComponent<CardFlip>();
        }

        void OnEnable()
        {
            _cardFlip.OnCardShow += CardShowHandler;
        }
        
        void OnDisable()
        {
            _cardFlip.OnCardShow -= CardShowHandler;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_cardClicked) return;
            _cardClicked = true;
            _cardFlip.FlipCard();
        }

        public void Missmatch()
        {
            _cardClicked = false;
            _cardFlip.FlipCard();
        }

        void CardShowHandler()
        {
            OnCardClicked?.Invoke(this);
        }
    }
}