using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace FateGames
{
    public class UICompleteScreen : UIElement
    {
        [SerializeField] private GameObject winScreen, loseScreen;
        [SerializeField] private Text levelText, coinText, gainText;
        [SerializeField] private RectTransform spreadCoinFrom, spreadCoinTo;
        private float totalCoin = 0;

        public void SetScreen(bool success, int level)
        {
            winScreen.SetActive(success);
            loseScreen.SetActive(!success);
            if (!success)
            {
                levelText.text = "LEVEL " + PlayerProgression.CurrentLevel;
            }
            else
            {
                /*if (MainLevelManager.Instance.Coin > 0)
                    SpreadCoin();*/
            }
        }

        // Called by ContinueButton onClick
        public void Continue()
        {
            SceneManager.LoadCurrentLevel();
        }
    }
}