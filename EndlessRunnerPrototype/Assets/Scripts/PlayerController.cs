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

    private bool isRunning = false;
    private bool slidingFix = true;
    private Quaternion targetRotation; // Variable to store the desired rotation
    private float rotationLerpSpeed = 5.0f; // Adjust the rotation lerp speed

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        transform.rotation = Quaternion.identity;
        targetRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (!PlayerManager.isGameStarted)
        {
            isRunning = true;
            return;
        }

        direction.z = forwardSpeed;
        playerAnim.SetBool("IsRunning", isRunning);

        // Interpolate the rotation smoothly
        if (slidingFix)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationLerpSpeed);
        }
       
        if (controller.isGrounded)
        {
            if (SwipeManager.swipeUp)
            {
                StartCoroutine(Jump());
            }
        }
        else
        {
            isRunning = false;
            direction.y += gravity * Time.deltaTime;
        }

        if (SwipeManager.swipeDown && !isSliding)
        {
            slidingFix = false;
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
        else
        {
            targetPosition.x = 0;
        }

        if (transform.position == targetPosition)
            return;

        Vector3 diff = targetPosition - transform.position;
        Vector3 moveDir = diff.normalized * 15 * Time.deltaTime;

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
        playerAnim.SetBool("IsSliding", true);
        controller.center = new Vector3(0, 0.60f, 0);
        controller.height = 0.5f;
        yield return new WaitForSeconds(1.3f);
        playerAnim.SetBool("IsSliding", false);
        controller.center = new Vector3(0, 0.79f, 0);
        controller.height = 1.45f;
        isSliding = false;
        slidingFix = true;
    }

    private IEnumerator Jump()
    {
        playerAnim.SetBool("IsJumping", true);
        direction.y = jumpForce;
        yield return new WaitForSeconds(1f);
        playerAnim.SetBool("IsJumping", false);
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
            PlayerManager.gameOver = true;
            FindObjectOfType<AudioManager>().PlaySound("GameOver");
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
            FindObjectOfType<AudioManager>().PlaySound("PickUpCoin");
            PlayerManager.totalPoints += 1;
            Destroy(other.gameObject);
            Debug.Log("Good food: " + other.gameObject.name + " " + PlayerManager.totalPoints);
        }
    }

    private void GameOver()
    {
        PlayerManager.gameOver = true;
        Debug.Log("Game Over");
    }
}
