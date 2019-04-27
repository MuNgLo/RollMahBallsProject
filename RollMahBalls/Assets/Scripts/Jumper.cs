using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumper : MonoBehaviour
{
    private Rigidbody rb;
    public bool isJumping;
    public float lastJump;
    [Range(0.05f, 2.0f)]
    public float jumpCooldown;
    [Range(1.0f, 100.0f)]
    public float jumpForce;

    public bool isGrounded;
    [Range(0.01f, 1.0f)]
    public float castLength;
    public LayerMask jumpable;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        isJumping = false;
        lastJump = 0.0f;
        
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = GroundCast();
        isJumping = Input.GetButton("Jump");
        if(isGrounded && isJumping && Time.time > jumpCooldown + lastJump)
        {
            // JUMP
            lastJump = Time.time;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private bool GroundCast()
    {
        Ray ray = new Ray(this.transform.position + Vector3.up * 0.2f, Vector3.down);
        return Physics.SphereCast(ray, 0.1f, castLength, jumpable);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Colliding with {collision.collider.name}!");
    }
}
