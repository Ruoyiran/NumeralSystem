using UnityEngine;
using NumeralSystem.Utils;

namespace NumeralSystem
{
    public class PinballObject : MonoBehaviour
    {
        public bool IsStatic
        {
            get
            {
                return _isStatic;
            }
        }

        private const int kMaxFramesForJudgingPinballStatic = 10; // 用于检测弹珠是否静止的帧数阈值
        private Vector3 _targetPosition;
        private int _nFramesCounter;
        public bool _isStatic;
        private bool _isLocal;

        void Update()
        {
            if(_isLocal && Vector3.Distance(transform.localPosition, _targetPosition) < 0.1f)
                ++_nFramesCounter;
            else if(!_isLocal && Vector3.Distance(transform.position, _targetPosition) < 0.1f)
                ++_nFramesCounter;
            _isStatic = _nFramesCounter >= kMaxFramesForJudgingPinballStatic;
        }

        public void Reset(Vector3 position, float colorAlpha)
        {
            _nFramesCounter = 0;
            _targetPosition = position;
            transform.localPosition = position;
            GameObjectUtils.SetImageColor(gameObject, new Color(1.0f, 1.0f, 1.0f, colorAlpha));
            GameObjectUtils.SetActive(gameObject, true);
        }

        public float MoveTo(Vector3 targetPos, float speed, bool isLocal = true)
        {
            Vector3 startPos;
            if (isLocal)
                startPos = transform.localPosition;
            else
                startPos = transform.position;
            _isStatic = false;
            _isLocal = isLocal;
            _nFramesCounter = 0;
            _targetPosition = targetPos;
            float time = CalcPinballMoveTime(startPos, targetPos, speed);
            GameObjectUtils.TweenMoveTo(gameObject, targetPos, time, isLocal);
            return time;
        }

        private float CalcPinballMoveTime(Vector3 pos1, Vector3 pos2, float speed)
        {
            if (speed <= 0)
                return 0;
            float distance = Vector3.Distance(pos1, pos2);
            return distance / (speed * Time.deltaTime * 1000);
        }
    }
}