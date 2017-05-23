using Memory.Sound;
using UnityEngine;

namespace Memory.Game
{
    public class Box: MonoBehaviour
    {
        public delegate void OnBoxAnimateEvent(Box box1, Box box2);
        public const float AnimateTime = 0.35f;

        public bool IsAnimated { get; private set; }
        public Sprite Symbol { get { return image.sprite; } }
        public bool IsShirt {get { return border.sprite == borderShirt; } }

        [SerializeField] private Animation anim;
        [SerializeField] private string showAnimation;
        [SerializeField] private string dropAnimation;
        [SerializeField] private SpriteRenderer border;
        [SerializeField] private Sprite borderShirt;
        [SerializeField] private Sprite borderSym;
        [SerializeField] private SpriteRenderer image;        
        [GameSound][SerializeField] private string hideSound;
        private event OnBoxAnimateEvent onAnimateComplete;

        public Box Init(Sprite symbol, Vector2 position, float showDelay)
        {
            transform.localPosition = position;
            transform.localEulerAngles = Vector3.zero;
            transform.localScale = Vector3.zero;            
            image.sprite = symbol;
            var c = image.color;
            c.a = 1;
            image.color = c;
            image.gameObject.SetActive(false);
            border.sprite = borderShirt;
            gameObject.SetActive(true);
            IsAnimated = false;
            onAnimateComplete = null;

            if (showDelay > 0) LeanTween.delayedCall(gameObject, showDelay, PlayShowAnimation);
            else PlayShowAnimation();

            return this;
        }
        
        public bool ShowSymbol(OnBoxAnimateEvent callback, object param)
        {
            if (IsAnimated) return false;
            IsAnimated = true;
            onAnimateComplete = callback;
            LeanTween.rotateY(gameObject, 180, AnimateTime).setOnComplete(OnRotateComplete, param);
            LeanTween.delayedCall(gameObject, AnimateTime / 2f, SwapShirtToSym);  
            return true;
        }

        public bool ShowShirt(float delay)
        {
            if (IsAnimated) return false;
            IsAnimated = true;
            LeanTween.rotateY(gameObject, 0, AnimateTime).setOnComplete(OnRotateComplete, null).setDelay(delay);
            LeanTween.delayedCall(gameObject, delay + AnimateTime / 2f, SwapSymToShirt);
            return true;
        }

        public void Hide(float delay)
        {
            LeanTween.delayedCall(delay, DoHide);
        }

        public void Drop()
        {
            //anim.Play(dropAnimation);
            //LeanTween.delayedCall(1f, OnHide);
            LeanTween.alpha(gameObject, 0f, AnimateTime).setOnComplete(OnHide);
        }

        #region private
        // ======================================

        private void SwapSymToShirt()
        {
            image.gameObject.SetActive(false);
            border.sprite = borderShirt;
        }

        private void SwapShirtToSym()
        {
            image.gameObject.SetActive(true);
            border.sprite = borderSym;
        }

        private void OnRotateComplete(object param)
        {
            IsAnimated = false;
            if (onAnimateComplete != null) onAnimateComplete(this, (Box)param);
            onAnimateComplete = null;
        }

        private void DoHide()
        {
            LeanTween.alpha(gameObject, 0f, AnimateTime).setOnComplete(OnHide);
            GameSound.Play(hideSound);
        }

        private void OnHide()
        {
            gameObject.SetActive(false);
        }
        
        private void PlayShowAnimation()
        {
            anim.Play(showAnimation);
        }
        // ======================================
        #endregion
    }
}