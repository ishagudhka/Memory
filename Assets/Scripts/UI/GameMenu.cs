using System.Collections;
using Memory.Game;
using UnityEngine;
using UnityEngine.UI;

namespace Momory.UI
{
    public class GameMenu: MonoBehaviour
    {
        [SerializeField] private int gameTime;
        [SerializeField] private Desk desk;
        [SerializeField] private Text time;
        [SerializeField] private CanvasGroup gameOver;

        public void OnGameStart()
        {
            StopAllCoroutines();
            if (Desk.IsTimeGame)
            {
                StartCoroutine(TimerProcess());
                time.gameObject.SetActive(true);
            }
        }

        public void OnGameEnd()
        {
            StopAllCoroutines();
            LeanTween.alphaCanvas(time.GetComponent<CanvasGroup>(), 0, 0.35f);
        }

        IEnumerator TimerProcess()
        {
            var ws = new WaitForSeconds(1);
            var currentTime = gameTime;
            LeanTween.alphaCanvas(time.GetComponent<CanvasGroup>(), 1, 0.35f);

            while (currentTime > 0)
            {
                time.text = string.Format("{0}:{1:00}", currentTime/60, currentTime%60);
                yield return ws;
                currentTime--;
            }

            desk.GameOver(3.5f);
            gameOver.gameObject.SetActive(true);
            gameOver.alpha = 0;
            LeanTween.alphaCanvas(gameOver, 1, 0.35f);
            LeanTween.alphaCanvas(gameOver, 0, 0.35f).setDelay(2.7f).setOnComplete(()=>gameOver.gameObject.SetActive(false));
        }        
    }
}