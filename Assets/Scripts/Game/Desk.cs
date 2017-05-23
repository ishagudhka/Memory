using System.Collections.Generic;
using Memory.Sound;
using UnityEngine;
using UnityEngine.Events;

namespace Memory.Game
{
    public class Desk: MonoBehaviour
    {
        public const float ShowBoxTime = 1f;
        public const float ActivateBoxAnimationDelay = 0.125f;

        #region UnityEritor property
        // ======================================

        public static bool IsTimeGame;
        public float SizeBox;            
        [SerializeField] private Sprite[] Symbols;
        [SerializeField] private GameObjectPool BoxPool;
        [SerializeField] private GameObjectPool MatchEffectPool;
        [GameSound][SerializeField] private string matchSound;
        [GameSound][SerializeField] private string boxRotateSound;
        [GameSound][SerializeField] private string gameStartSound;
        [GameSound][SerializeField] private string gameWinSound;
        [GameSound][SerializeField] private string gameOverSound;

        public UnityEvent StartGame;
        public UnityEvent EndGame;
        // ======================================
        #endregion


        #region private property   
        // ======================================

        private Box[] boxes;
        private int rowSize;
        private int colSize;        
        private Vector2 deltaCentPos;
        private float halfSizeBox;
        private Vector2 halfSizeBoxVector;
        private Box lastBox;
        private Box currentBox;
        private float swapToShirtTime;
        private int score;

        // ======================================
        #endregion

        public void Init()
        {
            Init(rowSize, colSize, IsTimeGame);
        }

        public void Init(int row, int col, bool timer)
        {
            LeanTween.cancelAll(false);
            enabled = true;
            IsTimeGame = timer;            
            BoxPool.HideAll();
            MatchEffectPool.HideAll();
            rowSize = row;
            colSize = col;
            halfSizeBox = SizeBox / 2;
            halfSizeBoxVector = new Vector2(halfSizeBox, halfSizeBox);
            deltaCentPos = new Vector2(col * halfSizeBox - halfSizeBox, row * halfSizeBox - halfSizeBox);
            lastBox = null;
            currentBox = null;
            swapToShirtTime = -1;

            transform.localScale = new Vector3(4f / row, 4f / row);
            
            GameSound.Play(gameStartSound);

            if (boxes != null && boxes.Length != row*col) boxes = null;
            if (boxes == null) boxes = new Box[row * col];
            
            for (int i = 0; i < boxes.Length; i++)
            {
                boxes[i] = BoxPool.Get().GetComponent<Box>();
            }

            List<Sprite> sprites = new List<Sprite>(row * col);
            
            for (int i = 0; i < row * col / 2; i++)
            {
                sprites.Add(Symbols[i]);
                sprites.Add(Symbols[i]);
            }

            for (int r = 0; r < row; r++)
            {
                for (int c = 0; c < col; c++)
                {
                    boxes[r*col + c].Init(GetRandomAndRemove(sprites), GetPosition(r, c), (r + c) * ActivateBoxAnimationDelay);
                }
            }
            score = 0;
            if (StartGame != null) StartGame.Invoke();
        }

        public void GameOver(float restartDelay)
        {
            for (int i = 0; i < boxes.Length; i++)
            {
                if (boxes[i].gameObject.activeSelf) 
                    LeanTween.delayedCall(Random.Range(0, 1f), boxes[i].Drop);
            }

            LeanTween.delayedCall(restartDelay, Init);
            if (EndGame != null) EndGame.Invoke();
            GameSound.Play(gameOverSound);
            enabled = false;
        }

        #region private methods
        // ======================================

        private Sprite GetRandomAndRemove(IList<Sprite> list)
        {
            if (list.Count == 0) return null;
            var i = Random.Range(0, list.Count);
            var s = list[i];
            list.RemoveAt(i);
            return s;
        }

        private Vector2 GetPosition(int r, int c)
        {
            return new Vector2(c * SizeBox, r * SizeBox) - deltaCentPos;
        }

        private int GetIndex(Vector2 pos)
        {
            pos = pos + deltaCentPos + halfSizeBoxVector;

            var c = (int) (pos.x/SizeBox);
            var r = (int) (pos.y/SizeBox);
            return r*colSize + c;
        }

        private void Update()
        {
            if (!enabled) return;

            if (lastBox != null && currentBox != null && (lastBox.IsAnimated || currentBox.IsAnimated)) return;

            if (Input.GetMouseButtonUp(0))
            {
                var i = GetIndex(transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
                if (i >= 0 && i < boxes.Length && !boxes[i].IsAnimated)
                {                   
                    if (boxes[i].IsShirt)
                    {
                        if (lastBox != null && currentBox != null) SwapBoxes();

                        if (lastBox == null)
                        {
                            lastBox = boxes[i];
                            boxes[i].ShowSymbol(null, null);
                        }
                        else
                        {
                            currentBox = boxes[i];
                            boxes[i].ShowSymbol(OnCheckSym, lastBox);
                        }
                        GameSound.Play(boxRotateSound);
                    }
                }
            }

            if (swapToShirtTime > 0 && swapToShirtTime < Time.time)
            {                
                if (lastBox != null && !lastBox.IsAnimated && currentBox != null && !currentBox.IsAnimated)
                    SwapBoxes();
            }
        }

        private void SwapBoxes()
        {
            swapToShirtTime = -1;
            lastBox.ShowShirt(Random.Range(0.1f, 0.4f));
            currentBox.ShowShirt(Random.Range(0.1f, 0.4f));
            lastBox = null;
            currentBox = null;
        }

        private void OnCheckSym(Box box1, Box box2)
        {
            if (box1.Symbol == box2.Symbol)
            {
                box1.Hide(1.5f);
                box2.Hide(1.5f);
                lastBox = null;
                currentBox = null;
                swapToShirtTime = -1;
                score++;

                var effect = MatchEffectPool.Get();
                effect.transform.localPosition = box1.transform.localPosition;
                effect.SetActive(true);
                LeanTween.delayedCall(2f, HideEffects).setOnCompleteParam(effect);

                effect = MatchEffectPool.Get();
                effect.transform.localPosition = box2.transform.localPosition;
                effect.SetActive(true);
                LeanTween.delayedCall(2f, HideEffects).setOnCompleteParam(effect);

                GameSound.Play(matchSound);

                if (score == (rowSize*colSize)/2)
                {
                    if (EndGame != null) EndGame.Invoke();
                    GameSound.Play(gameWinSound);
                    LeanTween.delayedCall(3f, Init);                    
                }
            }
            else
            {
                swapToShirtTime = Time.time + ShowBoxTime;
            }
        }

        private void HideEffects(object obj)
        {
            (obj as GameObject).SetActive(false);
        }

        // ======================================
        #endregion
    }
}