using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D playerRB;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    private Transform checkGroundPoint;
    [SerializeField] private LayerMask selectedLayerMask;
    private bool isGrounded;
    private bool isFlippedInX;
 
    private Animator animator;
    private int IdSpeed;
    private int IdIsGrounded;
    private int IdShootArrow;
    private int IdCanDoubleJump;

    private Transform transformArrowPoint;
    [SerializeField]
    private ArrowController arrowController;
    private Transform transformPlayerController;

    [SerializeField]
    private GameObject dustJump;
    private Transform transformDustPoint;
    private bool isIdle;
    private bool canDoubleJump;

    private void Awake()
    {
        playerRB = GetComponent<Rigidbody2D>();
        transformPlayerController = GetComponent<Transform>();
    }

    private void Start()
    {
        transformDustPoint = GameObject.Find("DustPoint").GetComponent<Transform>();
        transformArrowPoint = GameObject.Find("ArrowPoint").GetComponent<Transform>();
        checkGroundPoint = GameObject.Find("CheckGroundPoint").GetComponent<Transform>();
        animator =GameObject.Find("StandingPlayer").GetComponent<Animator>();
        IdSpeed = Animator.StringToHash("speed");
        IdIsGrounded = Animator.StringToHash("isGrounded");
        IdShootArrow = Animator.StringToHash("shootArrow");
        IdCanDoubleJump = Animator.StringToHash("canDoubleJump");
    }

    void Update()
    {
        Move();
        Jump();
        isFlippedInX = CheckAndSetDirection();
        ShootArrow();
        PlayDust();
    }

    private void ShootArrow()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            ArrowController tempArrowController = Instantiate(arrowController, transformArrowPoint.position, transformArrowPoint.rotation);
            if (isFlippedInX)
            {
                tempArrowController.ArrowDirection = new Vector2(-1, 0f);
                tempArrowController.GetComponent<SpriteRenderer>().flipX = true;
            }
            else
            {
                tempArrowController.ArrowDirection = new Vector2(1, 0f);
            }
            animator.SetTrigger(IdShootArrow);
        }
    }

    private void Move()
    {
        float inputX = Input.GetAxisRaw("Horizontal") * moveSpeed;
        playerRB.velocity = new Vector2(inputX, playerRB.velocity.y);
        animator.SetFloat(IdSpeed, Mathf.Abs(playerRB.velocity.x));
    }

    private void Jump()
    {
        isGrounded = Physics2D.OverlapCircle(checkGroundPoint.position, 0.2f, selectedLayerMask);
        if (Input.GetButtonDown("Jump") && (isGrounded || canDoubleJump))
        {
            if (isGrounded)
            {
                canDoubleJump = true;
                Instantiate(dustJump, transformDustPoint.position, Quaternion.identity);
            }
            else
            {
                canDoubleJump = false;
                animator.SetTrigger(IdCanDoubleJump);
            }
            
            playerRB.velocity = new Vector2(playerRB.velocity.x, jumpForce);
        }
        animator.SetBool(IdIsGrounded, isGrounded);
    }

    private bool CheckAndSetDirection()
    {
        if (playerRB.velocity.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            isFlippedInX = true;
        }
        else if (playerRB.velocity.x > 0)
        {
            transform.localScale = Vector3.one;
            isFlippedInX = false;
        }
        return isFlippedInX;
    }

    private void PlayDust()
    {
        if (playerRB.velocity.x != 0 && isIdle)
        {
            isIdle = false;
            if (isGrounded)
                Instantiate(dustJump, transformDustPoint.position, Quaternion.identity);
        }
        if (playerRB.velocity.x == 0)
        {
            isIdle = true;
        }
    }
}
