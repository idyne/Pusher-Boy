using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FateGames
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager instance;
        private static GameObject UIObject;
        public Canvas UICanvas { get; set; }
        public static UIManager Instance { get => instance; }
        [Header("Prefabs")]
        [SerializeField] private GameObject uiPrefab = null;
        [SerializeField] private GameObject uiLoadingScreenPrefab = null;
        [SerializeField] private GameObject uiCompleteScreenPrefab = null;
        [SerializeField] private GameObject uiLevelTextPrefab = null;
        [SerializeField] private GameObject uiStartTextPrefab = null;
        private void Awake()
        {
            if (!instance)
            {
                instance = this;

            }
            else
            {
                Destroy(gameObject);
                return;
            }

        }

        private void OnLevelWasLoaded(int level)
        {
            CreateUI();
        }

        public void CreateUI()
        {
            if (!UIObject)
            {
                UIObject = Instantiate(uiPrefab);
                UICanvas = UIObject.GetComponentInChildren<Canvas>();
                DontDestroyOnLoad(UIObject);
            }
            else
            {
                ClearUI();
            }
        }

        private void ClearUI()
        {
            while (UICanvas.transform.childCount > 0)
                DestroyImmediate(UICanvas.transform.GetChild(0).gameObject);
        }

        public void CreateUILevelText()
        {
            GameObject go = Instantiate(uiLevelTextPrefab, UICanvas.transform);
            TextMeshProUGUI levelText = go.GetComponentInChildren<TextMeshProUGUI>();
            levelText.text = "Level " + PlayerProgression.CurrentLevel;
        }

        public void CreateUIStartText() => Instantiate(uiStartTextPrefab, UICanvas.transform);
        public void CreateUICompleteScreen(bool success)
        {
            GameObject go = Instantiate(uiCompleteScreenPrefab, UICanvas.transform);
            UICompleteScreen uiCompleteScreen = go.GetComponent<UICompleteScreen>();
            uiCompleteScreen.SetScreen(success, PlayerProgression.CurrentLevel);
        }
        public void CreateLoadingScreen() => Instantiate(uiLoadingScreenPrefab, UICanvas.transform);
    }
}
