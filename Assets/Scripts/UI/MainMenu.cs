using Memory.Game;
using UnityEngine;
using UnityEngine.UI;

namespace Momory.UI
{
    public class MainMenu: MonoBehaviour
    {
        [SerializeField] private Animator anim;
        [SerializeField] private string hideState;
        [SerializeField] private string settingState;        
        [SerializeField] private Toggle gameTime;
        [SerializeField] private Desk desk;
        [SerializeField] private Vector2[] presets;        

        public void OnGameStart(int preset)
        {
            anim.SetTrigger(hideState);
            Destroy(gameObject, 1f);
            LeanTween.delayedCall(2f, ()=>Resources.UnloadUnusedAssets());
            LeanTween.delayedCall(0.65f, () => desk.Init((int) presets[preset].x, (int) presets[preset].y, gameTime.isOn));
        }

        public void OnShowSettings()
        {
            anim.SetTrigger(settingState);
        }        
    }
}