using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CardMatch.Card
{
    [RequireComponent(typeof(CardFlip))]
    public class CardView : MonoBehaviour, IPointerClickHandler
    {
        public int CardValue { get; set; }
        public event Action<CardView> OnCardShow;

        CardFlip _cardFlip;
        bool _cardClicked;

        void Awake()
        {
            _cardFlip = GetComponent<CardFlip>();
        }

        void OnEnable()
        {
            _cardFlip.OnCardShow += CardShowHandler;
            _cardFlip.OnCardFlipBack += CardFlipBackHandler;
        }
        
        void OnDisable()
        {
            _cardFlip.OnCardShow -= CardShowHandler;
            _cardFlip.OnCardFlipBack -= CardFlipBackHandler;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_cardClicked) return;
            _cardClicked = true;
            _cardFlip.FlipCard();
        }

        public void InitialShowCard(float showDuration)
        {
            _cardFlip.InitialShowCard(showDuration);
        }
        
        public void Flip()
        {
            _cardFlip.FlipCard();
        }

        void CardShowHandler()
        {
            OnCardShow?.Invoke(this);
        }

        void CardFlipBackHandler()
        {
            _cardClicked = false;
        }
    }
}