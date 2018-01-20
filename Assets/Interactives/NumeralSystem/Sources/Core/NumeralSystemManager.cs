using NumeralSystem.Utils;
using UnityEngine;

namespace NumeralSystem
{
    public class NumeralSystemManager : MonoBehaviour
    {
        public GameObject gameOverTipsPanel;
        private GameOverTips _gameOverTips;

        private void Start()
        {
            _gameOverTips = GameObjectUtils.GetComponent<GameOverTips>(gameOverTipsPanel, true);
            if (_gameOverTips != null)
                _gameOverTips.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            Messenger.AddListener(MessageConstant.MSG_NUMERIC_GAME_OVER, OnNumericGameOverHandler);
        }

        private void OnDisable()
        {
            Messenger.RemoveListener(MessageConstant.MSG_NUMERIC_GAME_OVER, OnNumericGameOverHandler);
        }

        private void OnNumericGameOverHandler()
        {
            if (_gameOverTips != null)
                _gameOverTips.gameObject.SetActive(true);
        }
    }
}