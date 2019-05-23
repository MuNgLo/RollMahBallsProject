using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Car
{
    /// <summary>
    /// Make sure Unity Input has Horizontal2 and Vertical2 for the left stick. Try 4th and 5th Axis first.
    /// </summary>
    [RequireComponent(typeof(CarMover))]
    public class CarTwinstickInput : MonoBehaviour
    {
        private float _R_horizontal = 0.0f, _R_vertical = 0.0f;
        private float _L_horizontal = 0.0f, _L_vertical = 0.0f;
        public float _deadZone = 0.02f;
        public bool _useRaw = false;
        //public AnimationCurve _defaultInputCurve;
        public Vector3 LeftStickVector { get { return GetLeftStickVector(); } private set { } }
        public Vector3 RightStickVector { get { return GetRightStickVector(); } private set { } }
        public float LHorizontalValue { get { return _L_horizontal; } private set { } }
        public float LVerticalValue { get { return _L_vertical; } private set { } }

        // Update is called once per frame
        void Update()
        {
            if (_useRaw)
            {
                _L_horizontal = ClampDeadZone(Input.GetAxisRaw("Horizontal"));
                _L_vertical = ClampDeadZone(Input.GetAxisRaw("Vertical"));
                _R_horizontal = ClampDeadZone(Input.GetAxisRaw("Horizontal2"));
                _R_vertical = ClampDeadZone(Input.GetAxisRaw("Vertical2"));
            }
            else
            {
                _L_horizontal = ClampDeadZone(Input.GetAxis("Horizontal"));
                _L_vertical = ClampDeadZone(Input.GetAxis("Vertical"));
                _R_horizontal = ClampDeadZone(Input.GetAxis("Horizontal2"));
                _R_vertical = ClampDeadZone(Input.GetAxis("Vertical2"));
            }
        }

        private Vector3 GetLeftStickVector()
        {
            return new Vector3(_L_horizontal, 0.0f, _L_vertical);
        }
        private Vector3 GetRightStickVector()
        {
            return new Vector3(_R_horizontal, 0.0f, _R_vertical);
        }

        private float ClampDeadZone(float value)
        {
            if (value <= _deadZone && value >= -_deadZone)
            {
                return 0.0f;
            }
            else
            {
                return value;
            }
        }
    }
}