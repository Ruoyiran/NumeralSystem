using UnityEngine;

namespace NumeralSystem
{
    public class NumeralSystemManager : MonoBehaviour
    {
        [SerializeField]
        private NumeralEnvironmentManager _numeralEnvMgr;
        void Start()
        {
        }

        private void OnEnable()
        {
            Messenger.AddListener<int>(MessageConstant.MSG_NUMERIC_INPUT_OK, OnNumericInputOkHandler);
            Messenger.AddListener<int>(MessageConstant.MSG_NUMERIC_INPUT_VALUE_CHANGED, OnNumericInputValueChanged);
        }

        private void OnDisable()
        {
            Messenger.RemoveListener<int>(MessageConstant.MSG_NUMERIC_INPUT_OK, OnNumericInputOkHandler);
            Messenger.RemoveListener<int>(MessageConstant.MSG_NUMERIC_INPUT_VALUE_CHANGED, OnNumericInputValueChanged);
        }

        private void OnNumericInputValueChanged(int number)
        {
            Logger.Print("OnNumericInputValueChanged - number: {0}", number);
        }

        private void OnNumericInputOkHandler(int number)
        {
            if (_numeralEnvMgr != null)
                _numeralEnvMgr.Reset(number);
        }
    }
}