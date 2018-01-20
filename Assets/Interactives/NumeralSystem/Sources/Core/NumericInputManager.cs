using System;
using System.Collections.Generic;
using System.Linq;
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
        public Button btnClear;
        public Button btnOk;
        public const int kMaxNumberBit = 16; // 最大计算的进制数位
        private int _inputNumber; // 当前输入的数字
        private int _prevInputNumber; // 点击确定按钮后保存的输入数值
        private static NumericInputManager _instance;
        private List<Toggle> _allToggles;
        private HashSet<int> _inputNumberSet;
        private void Awake()
        {
            _instance = this;
            _inputNumberSet = new HashSet<int>();
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
            AddButtonClickedListener(btnClear, OnClearButtonClicked);
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
            if (toggle.isOn)
            {
                _inputNumberSet.Add(num);
            }
            else
            {
                if (_inputNumberSet.Contains(num))
                    _inputNumberSet.Remove(num);
            }
            CheckInput();
            CheckOkButtonInteractable();
        }

        private void OnOkButtonClicked()
        {
            Logger.Print("OnOkButtonClicked - _inputNumber: {0}", _inputNumber);
            _prevInputNumber = _inputNumber;
            CheckOkButtonInteractable();
            SetNumberText(NumericConfig.GetChineseNumber(_inputNumber));
            Messenger.Broadcast<int>(MessageConstant.MSG_NUMERIC_INPUT_OK, _inputNumber);
        }

        private void OnClearButtonClicked()
        {
            List<int> numbers = _inputNumberSet.ToList();
            for (int i = 0; i < numbers.Count; i++)
            {
                int number = numbers[i];
                _allToggles[number].isOn = false;
            }
            _inputNumber = 0;
            _prevInputNumber = 0;
            SetNumberText("");
            Messenger.Broadcast(MessageConstant.MSG_NUMERIC_INPUT_CLEAR);
        }

        private void CheckOkButtonInteractable()
        {
            if (_inputNumber >= 2 && _inputNumber <= kMaxNumberBit &&
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

        private void CheckInput()
        {
            List<int> numbers = _inputNumberSet.ToList();
            if (numbers.Count == 1)
            {
                _inputNumber = numbers[0];
            }
            else if(numbers.Count == 2)
            {
                int value1 = numbers[0] * 10 + numbers[1];
                int value2 = numbers[1] * 10 + numbers[0];
                if(value1 > kMaxNumberBit && value2 > kMaxNumberBit)
                {
                    _inputNumber = 0;
                }
                else
                {
                    if (value1 > kMaxNumberBit)
                        _inputNumber = value2;
                    else if (value2 > kMaxNumberBit)
                        _inputNumber = value1;
                    else
                    {
                        _inputNumber = value1 > value2 ? value1 : value2;
                    }
                }
            }
            else
            {
                _inputNumber = 0;
            }
        }
    }
}