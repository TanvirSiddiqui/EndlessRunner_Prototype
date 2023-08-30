using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 direction;
    public float forwardSpeed;
    private int desiredLane = 1;
    public float laneDistance = 4;
    public float jumpForce;
    public float gravity = -20;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        Debug.Log(desiredLane);
    }

    // Update is called once per frame
    void Update()
    {
        direction.z = forwardSpeed;
      
        if (controller.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Jump();
            }
        }
        else
        {
            direction.y += gravity * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Debug.Log("Right Arrow Pressed");
            desiredLane++;
            if (desiredLane >= 3)
            {
                desiredLane = 2;
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Debug.Log("Left Arrow Pressed");
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
         targetPosition = new Vector3(targetPosition.x,transform.position.y, transform.position.z);
        transform.position = targetPosition;
    }

    private void Jump()
    {
        direction.y = jumpForce;
    }

    private void FixedUpdate()
    {
        controller.Move(direction * Time.fixedDeltaTime);
    }
}