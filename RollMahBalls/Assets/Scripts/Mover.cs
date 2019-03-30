using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Twinstick
{
    /// <summary>
    /// This is using leftstick vector from twinstickinput to move the transform it is attached to
    /// Check the AImer class for rightstick bit
    /// </summary>
    [RequireComponent(typeof(TwinstickInput))]
    public class Mover : MonoBehaviour
    {
        public float _maxAngularSpeed = 90.0f;
        public float _torqueStrength = 10000.0f;
        public ForceMode _mode = ForceMode.Force;
        public bool _useDelta = true;

        public Transform _translator;
        private TwinstickInput tw;
        private Rigidbody rb;
        private void Start()
        {
            tw = GetComponent<TwinstickInput>();
            rb = GetComponent<Rigidbody>();
        }
        // Update is called once per frame
        void Update()
        {
            if (!_translator) { return; }
            //transform.Translate(tw.LeftStickVector * _moveSpeed * Time.deltaTime);
            rb.maxAngularVelocity = _maxAngularSpeed;
            if (_useDelta)
            {
                rb.AddTorque(_translator.TransformVector(Quaternion.Euler(0.0f, 90.0f, 0.0f) * tw.LeftStickVector * _torqueStrength * Time.deltaTime), _mode);
            }
            else
            {
                rb.AddTorque(tw.LeftStickVector * _torqueStrength, _mode);
            }
        }

        public void SetTranslationTransform(Transform tr)
        {
            _translator = tr;
        }
    }
}