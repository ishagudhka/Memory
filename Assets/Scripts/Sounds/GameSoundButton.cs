using UnityEngine;
using UnityEngine.EventSystems;

namespace Memory.Sound
{
    public class GameSoundButton: MonoBehaviour, IPointerClickHandler
    {
        [GameSound]
        [SerializeField]
        private string sound;

        public void OnClick()
        {
            GameSound.Play(sound);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick();
        }
    }
}