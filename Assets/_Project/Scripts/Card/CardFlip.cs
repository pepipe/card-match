using UnityEngine;
using UnityEngine.UI;

namespace CardMatch.Card
{
    public class CardFlip : MonoBehaviour
    {
        [Tooltip("Card flip animation duration in seconds.")] [SerializeField]
        float FlipDuration = 1.0f;

        [Tooltip("Image displayed when the card is facing back.")] [SerializeField]
        Image BackImage;

        [Tooltip("Image displayed when the card is facing up.")] [SerializeField]
        Image UpImage;

        bool _isShowingCard;

        public void FlipCard()
        {
            float rotatingY = !_isShowingCard ? -90f : 90f;
            PrimeTween.Tween.Rotation(gameObject.transform, new Vector3(0, rotatingY, 0), FlipDuration / 2f,
                    PrimeTween.Ease.InQuint)
                .OnComplete(ChangeImage);
        }

        void FinishFlip()
        {
            PrimeTween.Tween.Rotation(gameObject.transform, Vector3.zero, FlipDuration / 2f,
                PrimeTween.Ease.OutQuint);
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