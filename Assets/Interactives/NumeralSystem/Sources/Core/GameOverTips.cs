using System;
using UnityEngine;
using UnityEngine.UI;
namespace NumeralSystem
{
    public class GameOverTips : MonoBehaviour
    {
        public Button btnOk;

        void Awake()
        {
            AddOkButtonClickListener();
        }

        private void OnEnable()
        {
            Messenger.AddListener(MessageConstant.MSG_NUMERIC_INPUT_CLEAR, OnNumericClearHandler);
        }

        private void OnDisable()
        {
            Messenger.RemoveListener(MessageConstant.MSG_NUMERIC_INPUT_CLEAR, OnNumericClearHandler);
        }

        private void AddOkButtonClickListener()
        {
            if (btnOk == null)
                return;
            btnOk.onClick.AddListener(OnOkButtonClicked);
        }

        private void OnOkButtonClicked()
        {
            gameObject.SetActive(false);
            if (NumeralEnvironmentManager.Instance != null)
                NumeralEnvironmentManager.Instance.Restart();
        }

        private void OnNumericClearHandler()
        {
            gameObject.SetActive(false);
        }
    }
}