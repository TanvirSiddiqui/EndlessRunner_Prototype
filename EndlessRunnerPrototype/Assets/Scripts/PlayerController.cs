using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 direction;
    public float forwardSpeed;
    public float rotationSpeed;
    private int desiredLane = 1;
    private bool isSliding = false;
    public float laneDistance = 4;
    public float jumpForce;
    public float gravity = -20;

    public Animator playerAnim;

    private bool isRunning = false; // Add a flag for running animation control

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
       
    }

    // Update is called once per frame
    void Update()
    {
        if (!PlayerManager.isGameStarted)
        {
            transform.rotation = Quaternion.identity; // Fix the rotation when not started
            isRunning = true;
            return;
        }

        direction.z = forwardSpeed;
        playerAnim.SetBool("IsRunning", isRunning);
      
        if (controller.isGrounded)
        {
            
            if (SwipeManager.swipeUp)
            {
                transform.rotation = Quaternion.identity;
                playerAnim.SetBool("IsJumping", true);
                Jump();
                transform.rotation = Quaternion.identity;
            }
        }
        else
        {
            isRunning = false;
            direction.y += gravity * Time.deltaTime;
            playerAnim.SetBool("IsJumping", false);
        }

        if (SwipeManager.swipeDown&&!isSliding)
        {
            StartCoroutine(Slide());
        }

        if (SwipeManager.swipeRight)
        {
            transform.rotation = Quaternion.identity;
            desiredLane++;
            if (desiredLane >= 3)
            {
                desiredLane = 2;
            }
            transform.rotation = Quaternion.identity;
        }

        if (SwipeManager.swipeLeft)
        {
            desiredLane--;
            if (desiredLane <= 0)
            {
                desiredLane = 0;
            }
            transform.rotation = Quaternion.identity;
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

        /* transform.position = Vector3.Lerp(transform.position, targetPosition, 80 * Time.deltaTime);*/
      //  float targetX = desiredLane * laneDistance;
      /*   targetPosition = new Vector3(targetPosition.x,transform.position.y, transform.position.z);
        transform.position = targetPosition;*/
        if (transform.position == targetPosition)
            return;
        Vector3 diff = targetPosition - transform.position;
        Vector3 moveDir = diff.normalized * 15 * Time.deltaTime;
        if (moveDir.sqrMagnitude < diff.sqrMagnitude)
        {
            transform.rotation = Quaternion.identity;
            controller.Move(moveDir);
            transform.rotation = Quaternion.identity;
        }
        else
        {
            controller.Move(diff);
        }
    }

    private IEnumerator Slide()
    {
        transform.rotation = Quaternion.identity;
        isSliding = true;
        playerAnim.SetBool("IsSliding",true);
        controller.center = new Vector3(0,0.60f,0);
        controller.height = 0.5f;
        yield return new WaitForSeconds(1.3f);
        playerAnim.SetBool("IsSliding", false);
        controller.center = new Vector3(0, 0.79f, 0);
        controller.height = 1.45f;
        isSliding = false;
        transform.rotation = Quaternion.identity;

    }

    private void Jump()
    {
        transform.rotation = Quaternion.identity;
        direction.y = jumpForce;
        transform.rotation = Quaternion.identity;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "JunkFood")
        {
            PlayerManager.totalPoints -= 1;
            Destroy(other.gameObject);
            Debug.Log("Junk food: " + PlayerManager.totalPoints);
        }
        if (other.transform.tag == "GoodFood")
        {
            PlayerManager.totalPoints += 1;
            Destroy(other.gameObject);
            Debug.Log("Good food: " + PlayerManager.totalPoints);
        }
    }

    private void GameOver()
    {
        PlayerManager.gameOver = true;
        Debug.Log("Game Over");
    }
}