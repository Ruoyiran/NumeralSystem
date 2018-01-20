using System;
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
        public Toggle toggleOne;
        public Toggle toggleTwo;
        public Toggle toggleThree;
        public Toggle toggleFour;
        public Toggle toggleFive;
        public Toggle toggleSix;
        public Toggle toggleSeven;
        public Toggle toggleEight;
        public Toggle toggleNine;
        public Toggle toggleZero;
        public Button btnDel;
        public Button btnOk;
        public const int kMaxNumberBit = 16; // 最大计算的进制数位
        private int _inputNumber; // 当前输入的数字
        private int _prevInputNumber; // 点击确定按钮后保存的输入数值
        private static NumericInputManager _instance;
        private List<Toggle> _allToggles;
        private void Awake()
        {
            _instance = this;

        }

        private void Start()
        {
            _inputNumber = 0;
            _prevInputNumber = 0;
            
            InitToggleList();
            SetNumberText("");
            CheckOkButtonInteractable();
            AddAllButtonsClickedListener();
        }

        private void InitToggleList()
        {
            _allToggles = new List<Toggle>{ toggleZero, toggleOne, toggleTwo, toggleThree, toggleFour,
                                            toggleFive, toggleSix, toggleSeven, toggleEight, toggleNine};
        }

        private void AddAllButtonsClickedListener()
        {
            AddAllNumericTogglesClickedListener();
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

        private void AddAllNumericTogglesClickedListener()
        {

            for (int i = 0; i < _allToggles.Count; i++)
            {
                int num = i;
                Toggle toggle = _allToggles[i];
                toggle.onValueChanged.RemoveAllListeners();
                toggle.onValueChanged.AddListener((isOn) => {
                    OnNumericToggleClicked(num, toggle);
                });
            }
        }

        private void OnNumericToggleClicked(int num, Toggle toggle)
        {
            Logger.Print("OnNumericToggleClicked - name: {0}, num: {1}", toggle.name, num);
            if(toggle.isOn && _inputNumber == 0 && num == 0)
            {
                toggle.isOn = false;
                return;
            }
            CheckInput(num, toggle);
            SetNumberText(NumericConfig.GetChineseNumber(_inputNumber));
            CheckOkButtonInteractable();
        }

        private void CheckInput(int num, Toggle toggle)
        {
            if (toggle.isOn)
            {
                int curInputNumber = _inputNumber * 10 + num;
                if (curInputNumber <= kMaxNumberBit)
                {
                    _inputNumber = curInputNumber;
                }
                else
                {
                    toggle.isOn = false;
                }
            }
            else
            {
                string tenPlace = "";
                string singlePlace = "";
                if (_inputNumber < 10)
                    singlePlace = _inputNumber.ToString();
                else
                {
                    singlePlace = (_inputNumber % 10).ToString();
                    tenPlace = (_inputNumber / 10).ToString();
                }
                if (num.ToString() == tenPlace)
                {
                    tenPlace = "";
                }
                else if (num.ToString() == singlePlace)
                {
                    singlePlace = "";
                }
                string newValue = tenPlace + singlePlace;
                if (newValue == "")
                    _inputNumber = 0;
                else
                    _inputNumber = int.Parse(tenPlace + singlePlace);
            }
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
            string singlePlace = "0";
            if (_inputNumber < 10)
                singlePlace = _inputNumber.ToString();
            else
            {
                singlePlace = (_inputNumber % 10).ToString();
            }
            int singleValue = int.Parse(singlePlace);
            _allToggles[singleValue].isOn = false;
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

        private bool CheckInput(int num)
        {
            if (_inputNumber > 0)
            {
                int curInputNumber = _inputNumber * 10 + num;
                if (curInputNumber <= kMaxNumberBit)
                {
                    _inputNumber = curInputNumber;
                    return true;
                }
            }
            else
            {
                _inputNumber = num;
                return true;
            }
            return false;

        }
    }
}