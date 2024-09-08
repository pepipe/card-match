using System;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace CardMatch.Card
{
    public class CardFlip : MonoBehaviour
    {
        [Tooltip("Card flip animation duration in seconds.")]
        [SerializeField] float FlipDuration = 1.0f;
        [Tooltip("Image displayed when the card is facing back.")]
        [SerializeField] Image BackImage;
        [Tooltip("Image displayed when the card is facing up.")] 
        [SerializeField] Image UpImage;
        
        public event Action OnCardShow;
        public event Action OnCardFlipBack;
        
        bool _isShowingCard;
        
        public void InitialShowCard(float showDuration)
        {
            Sequence.Create()
                .Chain(Tween.Rotation(gameObject.transform, new Vector3(0, -90f, 0), FlipDuration / 2f, Ease.InQuint)
                    .OnComplete(() =>
                    {
                        UpImage.gameObject.SetActive(true);
                        BackImage.gameObject.SetActive(false);
                    }))
                .Chain(Tween.Rotation(gameObject.transform, Vector3.zero, FlipDuration / 2f, Ease.OutQuint))
                .ChainDelay(showDuration)
                .Chain(Tween.Rotation(gameObject.transform, new Vector3(0, 90f, 0), FlipDuration / 2f, Ease.InQuint)
                    .OnComplete(() =>
                    {
                        UpImage.gameObject.SetActive(false);
                        BackImage.gameObject.SetActive(true);
                    }))
                .Chain(Tween.Rotation(gameObject.transform, Vector3.zero, FlipDuration / 2f, Ease.OutQuint));
        }
        
        public void FlipCard()
        {
            float rotatingY = !_isShowingCard ? -90f : 90f;
            Tween.Rotation(gameObject.transform, new Vector3(0, rotatingY, 0), FlipDuration / 2f, Ease.InQuint)
                .OnComplete(ChangeImage);
        }

        public bool IsCardFlipped()
        {
            return _isShowingCard;
        }

        void FinishFlip()
        {
            Tween.Rotation(gameObject.transform, Vector3.zero, FlipDuration / 2f, Ease.OutQuint).OnComplete(() =>
            {
                if(_isShowingCard) OnCardShow?.Invoke();
                else OnCardFlipBack?.Invoke();
            });
        }

        void ChangeImage()
        {
            _isShowingCard = !_isShowingCard;
            UpImage.gameObject.SetActive(_isShowingCard);
            BackImage.gameObject.SetActive(!_isShowingCard);
            FinishFlip();
        }
    }
}