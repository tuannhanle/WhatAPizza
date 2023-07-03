using System;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.UI
{
    public class IntroPanelUI : MonoBehaviour
    {
        [SerializeField] private Button _tapToPlay;
        [SerializeField] private CanvasGroup _canvasGroup;
        private void Awake()
        {
            _tapToPlay.onClick.AddListener(OnPlayGame);
            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;
        }

        private void OnPlayGame()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
        }
    }
}