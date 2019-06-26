using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameramover : MonoBehaviour
{
    Vector3 move;
    public float movementSpeed = 5.0f;
    // Start is called before the first frame update
    void Start()
    {
        move = new Vector3();
    }

    // Update is called once per frame
    void Update()
    {
        move = Vector3.zero;
        move.x += Input.GetAxis("Horizontal");
        move.y += Input.GetAxis("Vertical");
        this.transform.position = this.transform.position + (move * movementSpeed * Time.deltaTime);
    }
}
