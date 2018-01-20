using UnityEngine;
using NumeralSystem.Utils;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;
using System.Collections;

namespace NumeralSystem
{
    public class PinballManager : MonoBehaviour
    {
        public bool IsStaticPinball
        {
            get
            {
                bool isStatic = true;
                for (int i = 0; i < _pinballObjs.Count; i++)
                {
                    if (!_pinballObjs[i].IsStatic)
                    {
                        isStatic = false;
                        break;
                    }
                }
                return isStatic;
            }
        }

        public bool CanCarry
        {
            get
            {
                return _numBits >= 2 && _curPinballCount >= _numBits;
            }
        }

        public static bool IsDisableTouch { get; set; }   // 是否禁止点击弹珠
        public int CurPinballCount
        {
            get
            {
                return _curPinballCount;
            }
            set
            {
                _curPinballCount = value;
            }
        }
        public GameObject topPinballObj;
        public GameObject bottomPinballObj;

        private List<PinballObject> _pinballObjs = new List<PinballObject>();
        private List<Vector3> _allAvailablePositions;

        private const float kColorAlpha = 0.3f;     // 弹珠初始透明度
        private float _pinballMovingSpeed = 40f;    // 弹珠下落速度
        private int _pinballIntervalY = 30;         // 每个弹珠的垂直间距
        private int _nextPinballPosition = 0;
        private int _numBits;
        private int _curPinballCount;
        private string _spritePath;
        private bool _showTopObj = true;

        private void Awake()
        {
            _nextPinballPosition = 0;
            GameObjectUtils.SetVisible(topPinballObj, false);
            GameObjectUtils.SetVisible(bottomPinballObj, false);
            GameObjectUtils.SetImageColor(topPinballObj, new Color(1.0f, 1.0f, 1.0f, kColorAlpha));
        }

        private void Start()
        {
            ResetAllAvailablePositions();
            AddTopPinballTouchEvent();
        }

        private void ResetAllAvailablePositions()
        {
            _allAvailablePositions = new List<Vector3>();
            if (bottomPinballObj == null)
                return;
            Vector3 startPos = bottomPinballObj.transform.localPosition;
            for (int i = 0; i < NumericInputManager.kMaxNumberBit+1; i++)
            {
                _allAvailablePositions.Add(startPos);
                startPos += new Vector3(0, _pinballIntervalY, 0);
            }
        }

        private void FixedUpdate()
        {
            if (NumeralEnvironmentManager.State != GameState.Playing)
                return;
            CheckTopPinballVisibleState();
        }

        private void CheckTopPinballVisibleState()
        {
            if (topPinballObj == null)
                return;
            if (!_showTopObj)
            {
                GameObjectUtils.SetVisible(topPinballObj, false);
                return;
            }

            bool isStatic = true;
            if (_curPinballCount == 0)
                GameObjectUtils.SetVisible(topPinballObj, true);
            else
            {
                for (int i = 0; i < _pinballObjs.Count; i++)
                {
                    if (!_pinballObjs[i].IsStatic)
                    {
                        isStatic = false;
                        break;
                    }
                }
            }
            bool active = isStatic && (_curPinballCount < _numBits);
            GameObjectUtils.SetVisible(topPinballObj, active);
        }

        public void SetImageSprite(string path)
        {
            _spritePath = path;
        }

        public void SetTopPinballVisible(bool visible)
        {
            Logger.Print("SetTopPinballVisible - visible: {0}, name: {1}", visible, topPinballObj.name);
            GameObjectUtils.SetVisible(topPinballObj, visible);
        }

        public void HideAllPinballs()
        {
            for (int i = 0; i < _pinballObjs.Count; i++)
            {
                GameObjectUtils.SetVisible(_pinballObjs[i].gameObject, false);
            }
        }

        public void Reset(int numBits, bool showTopPinball)
        {
            _numBits = Math.Min(numBits, NumericInputManager.kMaxNumberBit);
            _curPinballCount = 0;
            _nextPinballPosition = 0;
            _showTopObj = showTopPinball;
            GameObjectUtils.SetVisible(topPinballObj, showTopPinball);
            GameObjectUtils.SetImageSprite(topPinballObj, _spritePath);
            for (int i = 0; i < _pinballObjs.Count; i++)
            {
                GameObjectUtils.SetImageSprite(_pinballObjs[i].gameObject, _spritePath);
                GameObjectUtils.SetVisible(_pinballObjs[i].gameObject, false);
            }
        }

        public void InsertPinballObject(int index, PinballObject pinballObject)
        {
            if (index < 0)
                index = 0;
            else if (index > _pinballObjs.Count)
                index = _pinballObjs.Count;
            _pinballObjs.Insert(index, pinballObject);
        }

        public PinballObject GetPinballObject(int index)
        {
            if (index < 0 || index >= _pinballObjs.Count)
                return null;
            return _pinballObjs[index];
        }

        public void RemovePinballObject(int index)
        {
            if (index < 0 || index >= _pinballObjs.Count)
                return;
            _pinballObjs.RemoveAt(index);
        }

        private bool AddTopPinballTouchEvent()
        {
            UITouchEvent touchEvent = GameObjectUtils.GetComponent<UITouchEvent>(topPinballObj, true);
            if (touchEvent == null)
                return false;
            touchEvent.onTouchDown += OnTopPinballTouchDown;
            return true;
        }

        private void OnTopPinballTouchDown(PointerEventData eventData)
        {
            if (IsDisableTouch)
            {
                Logger.Print("OnTopPinballTouchDown - touch disabled.");
                return;
            }
            PinballObject pinballObj = null;
            if (_pinballObjs.Count > _nextPinballPosition)
                pinballObj = _pinballObjs[_nextPinballPosition];
            else
            {
                GameObject obj = GameObjectUtils.InstantiateGameObject(topPinballObj, transform, "Pinball" + _nextPinballPosition);
                pinballObj = GameObjectUtils.GetComponent<PinballObject>(obj, true);
                if (pinballObj != null)
                    _pinballObjs.Add(pinballObj);
            }
            if (pinballObj == null)
                return;
            pinballObj.Reset(topPinballObj.transform.localPosition, 1.0f);
            pinballObj.MoveTo(_allAvailablePositions[_nextPinballPosition], _pinballMovingSpeed, true);
            _curPinballCount += 1;
            _nextPinballPosition += 1;
        }

        public Vector3 MoveToNextAvaiablePostion()
        {
            if (_nextPinballPosition >= _allAvailablePositions.Count)
                _nextPinballPosition = _allAvailablePositions.Count - 1;
            Vector3 pos = _allAvailablePositions[_nextPinballPosition];
            _nextPinballPosition += 1;
            return pos;
        }

        public IEnumerator MoveToTop()
        {
            float totalMovingTime = 0;
            for (int i = _pinballObjs.Count - 1; i >= 0; --i)
            {
                totalMovingTime += _pinballObjs[i].MoveTo(topPinballObj.transform.localPosition, _pinballMovingSpeed, true);
            }
            int j = 0;
            while(j < _pinballObjs.Count)
            {
                if (!_pinballObjs[j].IsStatic)
                {
                    yield return null;
                }
                else
                {
                    ++j;
                }
            }
        }

        public void MergeToOnePinball(string spritePath)
        {
            if (_pinballObjs.Count == 0)
                return;
            for (int i = 0; i < _pinballObjs.Count-1; ++i)
            {
                GameObjectUtils.SetVisible(_pinballObjs[i].gameObject, false);
            }
            PinballObject onePinballObj = _pinballObjs[_pinballObjs.Count - 1];
            GameObjectUtils.SetImageSprite(onePinballObj.gameObject, spritePath);
        }

        public IEnumerator DoCarry(PinballManager nextPinballMgr, bool hasNext)
        {
            int carry = GetCarryCount();
            PinballObject carryPinballObj = null;
            if (carry > 0 && _pinballObjs.Count > 0)
            {
                _curPinballCount -= carry * _numBits;
                for (int i = 0; i < _curPinballCount; i++)
                {
                    PinballObject pinballObj = _pinballObjs[i];
                    pinballObj.Reset(topPinballObj.transform.localPosition, 1.0f);
                }
                nextPinballMgr.CurPinballCount += carry;
                carryPinballObj = _pinballObjs[_pinballObjs.Count - 1];
                // 进位，弹珠向左移动
                GameObjectUtils.SetVisible(carryPinballObj.gameObject, true);
                carryPinballObj.MoveTo(nextPinballMgr.topPinballObj.transform.position, _pinballMovingSpeed, false);
                while (!carryPinballObj.IsStatic)
                {
                    yield return null;
                }
                carryPinballObj.transform.SetParent(nextPinballMgr.transform);
                RemovePinballObject(_pinballObjs.Count - 1);
                nextPinballMgr.InsertPinballObject(_pinballObjs.Count, carryPinballObj);
                //if (carry > 0 && !nextPinballMgr.CanCarry && hasNext)
                //    GameObjectUtils.SetVisible(carryPinballObj.gameObject, false);
            }
            for (int i = 0; i < _curPinballCount; i++) // 剩余弹珠下落到底部
            {
                PinballObject pinballObj = _pinballObjs[i];
                pinballObj.MoveTo(_allAvailablePositions[i], _pinballMovingSpeed);
                while (!pinballObj.IsStatic)
                    yield return null;
            }
            _nextPinballPosition = _curPinballCount;
            if (carry > 0) // 进位弹珠下落到底部
            {
                //Logger.Print("Falling down. scale " + carryPinballObj.transform.localScale);
                //GameObjectUtils.SetVisible(carryPinballObj.gameObject, true);
                Vector3 targetPos = nextPinballMgr.MoveToNextAvaiablePostion();
                carryPinballObj.MoveTo(targetPos, _pinballMovingSpeed, true);
                while (!carryPinballObj.IsStatic)
                    yield return null;
            }
        }

        public int GetCarryCount()
        {
            if (_numBits <= 0)
                return 0;
            int carry = _curPinballCount / _numBits;
            return carry;
        }
    }
}