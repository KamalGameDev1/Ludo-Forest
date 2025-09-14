using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace LudoMGP
{
    public class LudoDiceController : MonoBehaviour
    {
        public float slidingpos;
        public float sidepos;
        public Image diceImage;
        public int diceNumber;
        private void OnEnable()
        {
            transform.DOLocalMoveX(slidingpos+ sidepos, 0.5f);
        }
        private void OnDisable()
        {
            transform.DOLocalMoveX(sidepos, 0.3f);
            if (GameManagerLudo.instance != null && GameManagerLudo.instance.diceAnimatingSprite != null && GameManagerLudo.instance.diceAnimatingSprite.Length > 0)
            {
                int random = UnityEngine.Random.Range(0, GameManagerLudo.instance.diceAnimatingSprite.Length);
                diceImage.sprite = GameManagerLudo.instance.diceAnimatingSprite[random];
            }

        }
        public void CloseDiceSlider()
        {
            transform.DOLocalMoveX(sidepos, 0.3f);

            if (GameManagerLudo.instance != null &&GameManagerLudo.instance.diceAnimatingSprite != null &&GameManagerLudo.instance.diceAnimatingSprite.Length > 0)
            {
                int random = UnityEngine.Random.Range(0, GameManagerLudo.instance.diceAnimatingSprite.Length);
                diceImage.sprite = GameManagerLudo.instance.diceAnimatingSprite[random];
            }
        }

        public IEnumerator DiceRollingAnimation(DiceResultResponse diceResult, string color)
        {
            foreach(Sprite r_dice in GameManagerLudo.instance.diceAnimatingSprite)
            {
                diceImage.sprite = r_dice;
                yield return new WaitForSeconds(0.025f);                
            }
            diceImage.sprite = GameManagerLudo.instance.diceMovingSprite[diceResult.diceNumber-1];

            //PawnManager.instance.AllSelectedPawnAnimtion(color);
            SoundManager.VibrateDevice?.Invoke();

        }

        public void DiceRolling(DiceResultResponse diceResult,string color)
        {
            diceNumber = diceResult.diceNumber;
            StartCoroutine(DiceRollingAnimation(diceResult, color));
            
            StopCoroutine(DiceRollingAnimation(diceResult, color));
        }
    }
}