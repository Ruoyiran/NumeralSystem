using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace NumeralSystem
{
    public class NumericInputManager : MonoBehaviour
    {
        public static NumericInputManager Instance
        {
            get
            {
                return _instance;
            }
        }
        public int PrevInputNumber
        {
            get
            {
                return _prevInputNumber;
            }
        }
        public Text textNumericType;
        public Button btnOne;
        public Button btnTwo;
        public Button btnThree;
        public Button btnFour;
        public Button btnFive;
        public Button btnSix;
        public Button btnSeven;
        public Button btnEight;
        public Button btnNine;
        public Button btnZero;
        public Button btnDel;
        public Button btnOk;
        public const int kMaxNumberBit = 16; // 最大计算的进制数位
        private int _inputNumber; // 当前输入的数字
        private int _prevInputNumber; // 点击确定按钮后保存的输入数值
        private static NumericInputManager _instance;

        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            _inputNumber = 0;
            _prevInputNumber = 0;
            SetNumberText("");
            CheckOkButtonInteractable();
            AddAllButtonsClickedListener();
        }

        private void AddAllButtonsClickedListener()
        {
            AddAllNumericButtonsClickedListener();
            AddButtonClickedListener(btnDel, OnDelButtonClicked);
            AddButtonClickedListener(btnOk, OnOkButtonClicked);
        }

        private void SetNumberText(string text)
        {
            if (textNumericType != null)
                textNumericType.text = text;
        }

        private void AddButtonClickedListener(Button button, UnityAction action)
        {
            if (button == null)
                return;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(action);
        }

        private void AddAllNumericButtonsClickedListener()
        {
            List<Button> buttons = new List<Button>{ btnZero, btnOne, btnTwo, btnThree, btnFour,
                                                 btnFive, btnSix, btnSeven, btnEight, btnNine};
            for (int i = 0; i < buttons.Count; i++)
            {
                int num = i;
                AddButtonClickedListener(buttons[i], () =>
                {
                    OnNumericButtonClicked(num);
                });
            }
        }

        private void CheckOkButtonInteractable()
        {
            if(_inputNumber >= 2 && _inputNumber <= kMaxNumberBit &&
                _prevInputNumber != _inputNumber)
                SetButtonInteractable(btnOk, true);
            else
                SetButtonInteractable(btnOk, false);
        }

        private void SetButtonInteractable(Button button, bool interactable)
        {
            if (button == null)
                return;
            button.interactable = interactable;
        }

        private void OnNumericButtonClicked(int num)
        {
            CheckInput(num);
            SetNumberText(NumericConfig.GetChineseNumber(_inputNumber));
            CheckOkButtonInteractable();
            Messenger.Broadcast<int>(MessageConstant.MSG_NUMERIC_INPUT_VALUE_CHANGED, _inputNumber);
        }

        private void OnOkButtonClicked()
        {
            Logger.Print("OnOkButtonClicked - _inputNumber: {0}", _inputNumber);
            _prevInputNumber = _inputNumber;
            CheckOkButtonInteractable();
            Messenger.Broadcast<int>(MessageConstant.MSG_NUMERIC_INPUT_OK, _inputNumber);
        }

        private void OnDelButtonClicked()
        {
            if (_inputNumber > 0)
            {
                _inputNumber /= 10;
                SetNumberText(NumericConfig.GetChineseNumber(_inputNumber));
            }
            CheckOkButtonInteractable();
        }

        private void CheckInput(int num)
        {
            if (_inputNumber > 0)
            {
                int curInputNumber = _inputNumber * 10 + num;
                if (curInputNumber <= kMaxNumberBit)
                {
                    _inputNumber = curInputNumber;
                }
            }
            else
            {
                _inputNumber = num;
            }

        }
    }
}