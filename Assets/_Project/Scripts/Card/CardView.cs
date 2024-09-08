using System;
using CardMatch.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CardMatch.Card
{
    [RequireComponent(typeof(CardFlip))]
    public class CardView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] SoundClip FlipSound;

        public int CardId { get; set; }
        public int CardIndex { get; set; }
        public event Action<CardView> OnCardShow;

        CardFlip _cardFlip;
        bool _isClickable = true;
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
            if (_cardClicked || !_isClickable) return;
            FlipSound.PlayOneShot();
            _cardClicked = true;
            _cardFlip.FlipCard();
        }

        public async UniTaskVoid InitialShowCard(float showDuration)
        {
            _isClickable = false;
            await _cardFlip.InitialShowCard(showDuration);
            _isClickable = true;
        }

        public void MakeCardFaceUp()
        {
            _cardFlip.MakeCardFace(true);
        }

        public void MakeCardFaceDown()
        {
            _cardFlip.MakeCardFace(false);
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