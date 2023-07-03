using System;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace DefaultNamespace.UI
{
    public class ResultPanelUI : MonoBehaviour
    {
        [SerializeField] private Text _resultText;
        [SerializeField] private Text _replayText;
        [SerializeField] private Button _replayButton;
        [SerializeField] private GameObject _winImg;
        [SerializeField] private GameObject _loseImg;
        [SerializeField] private CanvasGroup _canvasGroup;

        public Action OnReplay{ get; set; }
        
        private void Awake()
        {
            _replayButton.onClick.AddListener(Replay);
            Hide();
        }

        public void Show(bool isWin)
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;
            _winImg.gameObject.SetActive(isWin);
            _loseImg.gameObject.SetActive(!isWin);
            _resultText.text = isWin ? "What A Pizza!" : "You can do it better";
            _replayText.text = isWin ? "Tap to more challenge" : "Alright! Tap to replay";
        }

        public void Replay()
        {
            Hide();
            OnReplay?.Invoke();
        }

        private void Hide()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
        }
    }
}