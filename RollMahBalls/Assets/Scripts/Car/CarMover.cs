using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Car
{
    /// <summary>
    /// This is using leftstick vector from twinstickinput to move the transform it is attached to
    /// Check the AImer class for rightstick bit
    /// </summary>
    [RequireComponent(typeof(CarTwinstickInput))]
    public class CarMover : MonoBehaviour
    {
        //public float _maxAngularSpeed = 90.0f;
        public float _StreeringSpeed = 10000.0f;
        public float _Acceleration = 10000.0f;
        public ForceMode _mode = ForceMode.Force;
        public bool _useDelta = true;

        public Transform _translator;

        public Rigidbody _Car;

        private CarTwinstickInput tw;
        private void Start()
        {
            tw = GetComponent<CarTwinstickInput>();
        }
        // Update is called once per frame
        void Update()
        {
            if (!_translator) { return; }
            if (_useDelta)
            {
                _Car.GetComponent<Rigidbody>().AddForce(_translator.TransformVector(Vector3.forward * tw.LVerticalValue * _Acceleration * Time.deltaTime), _mode);
                _Car.GetComponent<Rigidbody>().AddTorque(
                    _translator.TransformVector(Quaternion.Euler(0.0f, 0.0f, 90.0f) * Vector3.right * tw.LHorizontalValue * _StreeringSpeed * Time.deltaTime),
                    ForceMode.Force
                    );

            }
            else
            {
                _Car.GetComponent<Rigidbody>().AddForce(_translator.TransformVector(Vector3.forward * tw.LVerticalValue * _Acceleration), _mode);
                _Car.GetComponent<Rigidbody>().AddTorque(
                    _translator.TransformVector(Quaternion.Euler(0.0f, 0.0f, 90.0f) * Vector3.right  * tw.LHorizontalValue * _StreeringSpeed), 
                    ForceMode.Force                   
                    );
            }
        }

        public void SetTranslationTransform(Transform tr)
        {
            _translator = tr;
        }
    }
}