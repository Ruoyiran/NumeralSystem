using UnityEngine;
using NumeralSystem.Utils;
using System.Collections.Generic;
using System;
using System.Collections;

namespace NumeralSystem
{
    public class NumeralEnvironmentManager : MonoBehaviour
    {
        public GameObject onePoleObj;   // 单根弹珠杆子GameObject对象，用于多位数进位创建新实例
        public Transform poleParentTransform; // 弹珠杆子窗口对象，用于存放子onePoleObj创建出来的对象
        private int _numDigits = 5;    // 演示的位数
        private List<PinballManager> _allPinballManagers;
        private readonly List<string> pinballSpritePaths = new List<string>()
        { "Sprites/image6", "Sprites/image7", "Sprites/image8", "Sprites/image9", "Sprites/image10" };
        private bool _isPlayingAnimation = false; // 是否正在播放进位动画

        private void Start()
        {
            _allPinballManagers = new List<PinballManager>();
            InitEnvironment();
        }

        private void Update()
        {
            PinballManager.IsDisableTouch = _isPlayingAnimation; // 正在播放进位动画时不能点击弹珠
            if (_isPlayingAnimation)
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
            Logger.Print("Playing done.");
            _isPlayingAnimation = false;
            if (_allPinballManagers.Count > 0 && _allPinballManagers[0].CurPinballCount > 0)
            {
                Logger.Print("Game Over!");
                Reset(NumericInputManager.Instance.PrevInputNumber);
            }
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

        public void Reset(int number)
        {
            if (_allPinballManagers == null)
                return;
            for (int i = 0; i < _allPinballManagers.Count; i++)
            {
                bool showTopPinball = true;
                if (i == 0)
                    showTopPinball = false;
                _allPinballManagers[i].Reset(number, showTopPinball);
            }
            _isPlayingAnimation = false;
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
    }
}