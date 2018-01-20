using UnityEngine;
using NumeralSystem.Utils;
using System.Collections.Generic;
using System.Collections;
using System;

namespace NumeralSystem
{
    public enum GameState
    {
        Playing,
        GameOver,
    }
    public class NumeralEnvironmentManager : MonoBehaviour
    {
        public GameObject onePoleObj;   // 单根弹珠杆子GameObject对象，用于多位数进位创建新实例
        public Transform poleParentTransform; // 弹珠杆子窗口对象，用于存放子onePoleObj创建出来的对象
        public static GameState State = GameState.GameOver;
        public static NumeralEnvironmentManager Instance
        {
            get
            {
                return _instance;
            }
        }

        public int NumberMaxValue
        {
            get
            {
                return _maxValue;
            }
        }

        public int CurrentNumberValue
        {
            get
            {
                int value = CalcCurrentNumberValue();
                return value;
            }
        }

        private static NumeralEnvironmentManager _instance;
        private int _numDigits = 5;    // 演示的位数
        private List<PinballManager> _allPinballManagers;
        private readonly List<string> pinballSpritePaths = new List<string>()
        { "Sprites/image6", "Sprites/image7", "Sprites/image8", "Sprites/image9", "Sprites/image10" };
        private bool _isPlayingAnimation = false; // 是否正在播放进位动画
        private Coroutine _envResetCoroutine;
        private int _maxValue;
        private int _numBits;

        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            _allPinballManagers = new List<PinballManager>();
            InitEnvironment();
        }

        private void OnEnable()
        {
            Messenger.AddListener<int>(MessageConstant.MSG_NUMERIC_INPUT_OK, OnNumericInputOkHandler);
            Messenger.AddListener(MessageConstant.MSG_NUMERIC_INPUT_CLEAR, OnNumericClearHandler);
        }

        private void OnDisable()
        {
            Messenger.RemoveListener<int>(MessageConstant.MSG_NUMERIC_INPUT_OK, OnNumericInputOkHandler);
            Messenger.RemoveListener(MessageConstant.MSG_NUMERIC_INPUT_CLEAR, OnNumericClearHandler);
        }

        private void Update()
        {
            PinballManager.IsDisableTouch = _isPlayingAnimation; // 正在播放进位动画时不能点击弹珠
            if (_isPlayingAnimation || State != GameState.Playing)
                return;
            bool isStatic = CheckAllPinballsStaticState(); // 所有弹珠都处于静止状态
            if (!isStatic) // 有正在下落的弹珠，等待下落结束
                return;
            bool canCarry = CarryChecking(); // 检查进位
            if (canCarry)
            {
                _isPlayingAnimation = true;
                StartCoroutine(PlayCarryAnimation()); // 播放进位动画
            }
        }

        private IEnumerator PlayCarryAnimation()
        {
            for (int i = _allPinballManagers.Count-1; i > 0; --i)
            {
                PinballManager pinballMgr = _allPinballManagers[i];
                if (pinballMgr.CanCarry)
                {
                    yield return pinballMgr.MoveToTop();
                    pinballMgr.MergeToOnePinball(pinballSpritePaths[i - 1]);
                    yield return pinballMgr.DoCarry(_allPinballManagers[i - 1], i == 0);
                }
            }
            _isPlayingAnimation = false;
            bool isGameOver = CheckGameOver();

            if (isGameOver)
            {
                Logger.Print("Game Over!");
                State = GameState.GameOver;
                Messenger.Broadcast(MessageConstant.MSG_NUMERIC_GAME_OVER);
            }
        }

        private bool CheckGameOver()
        {
            if (_allPinballManagers == null || _allPinballManagers.Count == 0)
                return true;
            if (_allPinballManagers[0].CurPinballCount > NumericInputManager.Instance.PrevInputNumber)
                return true;
            bool isGameOver = true;
            for (int i = 0; i < _allPinballManagers.Count; i++)
            {
                if(_allPinballManagers[i].CurPinballCount < NumericInputManager.Instance.PrevInputNumber)
                {
                    isGameOver = false;
                    break;
                }
            }
            return isGameOver;
        }

        private void InitEnvironment()
        {
            if (onePoleObj == null)
                return;
            for (int i = 0; i < _numDigits; i++)
            {
                GameObject poleObj = null;
                if (i == 0)
                    poleObj = onePoleObj;
                else
                    poleObj = GameObjectUtils.InstantiateGameObject(onePoleObj, poleParentTransform, "Pole" + (i + 1));
                PinballManager pinballMgr = GameObjectUtils.GetComponent<PinballManager>(poleObj, true);
                if (pinballMgr != null)
                {
                    if (pinballSpritePaths.Count > i)
                        pinballMgr.SetImageSprite(pinballSpritePaths[i]);
                    _allPinballManagers.Add(pinballMgr);
                }
            }
        }

        public IEnumerator Reset(int number)
        {
            _numBits = number;
            _isPlayingAnimation = true;
            _maxValue = (int)Mathf.Pow(number, _numDigits);
            for (int i = _allPinballManagers.Count - 1; i >= 0; --i)
            {
                _allPinballManagers[i].Reset(number, _allPinballManagers.Count - 1 - i, true);
                yield return new WaitForSeconds(0.1f);
            }
            _isPlayingAnimation = false;
            State = GameState.Playing;
        }

        public void Restart()
        {
            Clear();
            StartCoroutine(Reset(_numBits));
        }

        private void HideAllPinballs()
        {
            for (int i = 0; i < _allPinballManagers.Count; ++i)
            {
                _allPinballManagers[i].SetTopPinballVisible(false);
                _allPinballManagers[i].HideAllPinballs();
            }
        }

        private bool CheckAllPinballsStaticState()
        {
            bool isStaticState = true;
            for (int i = 0; i < _allPinballManagers.Count; i++)
            {
                PinballManager pinballMgr = _allPinballManagers[i];
                if (!pinballMgr.IsStaticPinball)
                {
                    isStaticState = false;
                    break;
                }
            }
            return isStaticState;
        }

        private int CalcCurrentNumberValue()
        {
            int value = 0;
            for (int i = 0; i < _allPinballManagers.Count; i++)
            {
                value += _allPinballManagers[i].CurPinballCount * (int)Mathf.Pow(_numBits, _allPinballManagers.Count - i - 1);
            }
            return value;
        }

        private bool CarryChecking()
        {
            bool canCarry = false;
            for (int i = 0; i < _allPinballManagers.Count; i++)
            {
                PinballManager pinballMgr = _allPinballManagers[i];
                if (pinballMgr.CanCarry)
                {
                    canCarry = true;
                    break;
                }
            }
            return canCarry;
        }

        private void OnNumericInputOkHandler(int number)
        {
            StopEnvResetCoroutine();
            _envResetCoroutine = StartCoroutine(Reset(number));
        }

        private void StopEnvResetCoroutine()
        {
            if(_envResetCoroutine != null)
            {
                StopCoroutine(_envResetCoroutine);
                _envResetCoroutine = null;
            }
        }

        private void OnNumericClearHandler()
        {
            Clear();
        }

        private void Clear()
        {
            State = GameState.GameOver;
            StopEnvResetCoroutine();
            HideAllPinballs();
        }
    }
}