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
    }
}