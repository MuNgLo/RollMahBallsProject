using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingPongVertical : MonoBehaviour
{
    public float _speed = 1.0f;
    public float _distance = 4.0f;
    private float _startY = 0.0f;
    public bool _rising = true;
    // Start is called before the first frame update
    void Start()
    {
        _startY = this.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        float frameDistance = _speed * Time.deltaTime;
        if(_rising == false) { frameDistance *= -1; }
        this.transform.position += Vector3.up * frameDistance;
        if(this.transform.position.y > _startY + _distance * 0.5f)
        {
            _rising = false;
        }
        if (this.transform.position.y < _startY - _distance * 0.5f)
        {
            _rising = true;
        }
    }
}
