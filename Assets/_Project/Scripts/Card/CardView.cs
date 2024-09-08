using System;
using CardMatch.SaveGame;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CardMatch.Card
{
    [RequireComponent(typeof(CardFlip))]
    public class CardView : MonoBehaviour, IPointerClickHandler
    {
        public int CardId { get; set; }
        public int CardIndex { get; set; }
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

        public void LoadState(bool isActive, bool isFaceUp)
        {
            gameObject.SetActive(isActive);
            _cardFlip.LoadState(isFaceUp);
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

        public bool IsCardFlipped()
        {
            return _cardFlip.IsCardFlipped();
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