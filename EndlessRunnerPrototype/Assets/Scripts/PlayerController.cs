using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    public GameObject player;
    private Vector3 direction;
    public float forwardSpeed;
    private int desiredLane = 1;
    private bool isSliding=false;
    public float laneDistance = 4;
    public float jumpForce;
    public float gravity = -20;

    public Animator playerAnim;

    private Quaternion initialRotation;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        initialRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = initialRotation;
        if (!PlayerManager.isGameStarted)
            return;

        direction.z = forwardSpeed;
        playerAnim.SetBool("IsRunning", true);
      
        if (controller.isGrounded)
        {
            if (SwipeManager.swipeUp)
            {
                playerAnim.SetBool("IsJumping", true);
                Jump();
            }
        }
        else
        {
            direction.y += gravity * Time.deltaTime;
            playerAnim.SetBool("IsJumping", false);
        }

        if (SwipeManager.swipeDown&&!isSliding)
        {
            StartCoroutine(Slide());
        }

        if (SwipeManager.swipeRight)
        {
            desiredLane++;
            if (desiredLane >= 3)
            {
                desiredLane = 2;
            }
        }

        if (SwipeManager.swipeLeft)
        {
            desiredLane--;
            if (desiredLane <= 0)
            {
                desiredLane = 0;
            }
        }

         Vector3 targetPosition = transform.position;

          if (desiredLane == 0)
          {
              targetPosition.x = -laneDistance;
          }
          else if (desiredLane == 2)
          {
              targetPosition.x = laneDistance;
          }
          else  // This covers desiredLane == 1
          {
              targetPosition.x = 0;  // Center lane
          }

        /*  transform.position = Vector3.Lerp(transform.position, targetPosition, 80 * Time.deltaTime);*/
      //  float targetX = desiredLane * laneDistance;
      /*   targetPosition = new Vector3(targetPosition.x,transform.position.y, transform.position.z);
        transform.position = targetPosition;*/
        if (transform.position == targetPosition)
            return;
        Vector3 diff = targetPosition - transform.position;
        Vector3 moveDir = diff.normalized * 25 * Time.deltaTime;
        if (moveDir.sqrMagnitude < diff.sqrMagnitude)
        {
            controller.Move(moveDir);
        }
        else
        {
            controller.Move(diff);
        }
    }

    private IEnumerator Slide()
    {
        isSliding = true;
        playerAnim.SetBool("IsSliding",true);
        controller.center = new Vector3(0,0.60f,0);
        controller.height = 0.5f;
        yield return new WaitForSeconds(1.3f);
        playerAnim.SetBool("IsSliding", false);
        controller.center = new Vector3(0, 0.79f, 0);
        controller.height = 1.23f;
        isSliding = false;
    }

    private void Jump()
    {
        direction.y = jumpForce;
    }

    private void FixedUpdate()
    {
        if (!PlayerManager.isGameStarted)
            return;

        controller.Move(direction * Time.fixedDeltaTime);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.tag == "Obstacles")
        {
            playerAnim.SetBool("IsDead", true);
            Invoke("GameOver", 2f);
        }
    }

    private void GameOver()
    {
        PlayerManager.gameOver = true;
        Debug.Log("Game Over");
    }
}