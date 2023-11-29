using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;

    [Header("Player Status")]
    // PlayerData Data;
    public float speed;
    float xVelocity;
    float yVelocity;
    public bool isMove;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        ButtonPressedSeting();
    }

    private void FixedUpdate()
    {
        FlipDireaction();
        Movement();
    }

    private void ButtonPressedSeting()
    {
        xVelocity = Input.GetAxis("Horizontal");
        yVelocity = Input.GetAxis("Vertical");
    }

    private void FlipDireaction()
    {
        if (xVelocity < 0)
            transform.localScale = new Vector3(-1, 1, 1);
        if (xVelocity > 0)
            transform.localScale = new Vector3(1, 1, 1);
    }

    private void Movement()
    {
        if (Mathf.Abs(xVelocity + yVelocity) > 0)
            isMove = true;
        else
            isMove = false;

        var dir = new Vector2(xVelocity, yVelocity);
        dir.Normalize();
        rb.velocity = speed * dir * Time.fixedDeltaTime;
    }

}
