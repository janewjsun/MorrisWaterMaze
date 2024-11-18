using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

// public class
using UnityEngine;
using UnityEngine.InputSystem;

using System.IO;

public class PlayerController : MonoBehaviour
{
    // Variables to store movement and rotation inputs

    public GameObject titleMsg;
    public GameObject bwBlockMsg;
    public GameObject endExperimentMsg;

    private float movementInput;
    private float rotationInput;
    private Rigidbody rb;
    private float distance;
    private Vector3 previousPosition;
    private Vector3 startPos;
    float curTime;
    public Transform[] spawnList;
    public List<GameObject> targetList;
    private int blockSize = 10;
    private int blockNumber = 0;

    // Speed settings for movement and rotation
    public float movementSpeed = 5f;
    public float rotationSpeed = 100f;
    private int turn = 0;

    IEnumerator Start()
    {
        // Get and store the Rigidbody component attached to the player

        rb = GetComponent<Rigidbody>();
        startPos = transform.position;
        previousPosition = transform.position;
        distance = 0;
        curTime = Time.time;

        bwBlockMsg.SetActive(false);
        endExperimentMsg.SetActive(false);

        titleMsg.SetActive(true);
        Debug.Log("Activating text: " + titleMsg.activeSelf);

        yield return new WaitForSeconds(5);
        titleMsg.SetActive(false);

        UpdateTargetVisibility();

    }

    // This function is called when a move input is detected (up/down)
    void OnMove(InputValue movementValue)
    {
        // Read the input for forward/backward movement
        movementInput = movementValue.Get<Vector2>().y;
    }

    // This function is called when a rotate input is detected (left/right)
    void OnRotate(InputValue rotationValue)
    {
        // Read the input for left/right rotation
        rotationInput = rotationValue.Get<float>();
    }

    void FixedUpdate()
    {
        // Handle player movement
        MovePlayer();

        // Handle player rotation
        RotatePlayer();

        float curDist = Vector3.Distance(previousPosition, transform.position);
        distance += curDist;
        previousPosition = transform.position;

    }

    IEnumerator OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("target")) {
    

            float timeElapsed = Time.time - curTime;
            float directVectorDist = new Vector3(transform.position.x - startPos.x, 0, transform.position.z - startPos.z).magnitude;
            
            
            Debug.Log(blockNumber);

            Debug.Log("Outputting status: " + Output.outputting);

            if (Output.outputting)
            {
                Debug.Log($"{blockNumber}\t{turn%10}\t{timeElapsed}\t{distance}\t{directVectorDist/distance*100}%");
                // Format and write data to excelBuffer
                Output.excelBuffer = $"{blockNumber}\t{turn%10+1}\t{timeElapsed}\t{distance}\t{directVectorDist/distance*100}%";
                
                // Append data to output file
                using (StreamWriter swe = File.AppendText(Output.accessibleExcelPath))
                {
                    swe.WriteLine(Output.excelBuffer);
                }

                // Clear buffer
                Output.excelBuffer = "";
            }
            resetGame();
            if (turn % blockSize == 0 && blockNumber < 3) {
                UpdateTargetVisibility(); // Switch target every 10 turns
                bwBlockMsg.SetActive(true);
                Debug.Log("Activating text: " + bwBlockMsg.activeSelf);

                yield return new WaitForSeconds(5);
                bwBlockMsg.SetActive(false);
            }
            if (turn % blockSize == 0 && blockNumber == 3) {
                UpdateTargetVisibility(); // Switch target every 10 turns
                endExperimentMsg.SetActive(true);
                Debug.Log("Activating text: " + endExperimentMsg.activeSelf);

                yield return new WaitForSeconds(5);
                endExperimentMsg.SetActive(false);
            }


        }
    }


    private void UpdateTargetVisibility()
    {
        // Disable all targets
        for (int i = 0; i < targetList.Count; i++)
        {
            targetList[i].GetComponent<Renderer>().enabled = false;
            targetList[i].GetComponent<Collider>().enabled = false; // Make it non-collidable
        }

        // Enable the current target
        targetList[blockNumber].GetComponent<Renderer>().enabled = false; // Initially hidden
        targetList[blockNumber].GetComponent<Collider>().enabled = true;  // Collidable

        blockNumber = blockNumber + 1;
    }

    private void resetGame() {
        turn = turn + 1;
        transform.position = spawnList[turn%blockSize].position;

        Vector3 newRotation = transform.eulerAngles;
        newRotation.y = spawnList[turn%blockSize].rotation.eulerAngles.y;
        transform.eulerAngles = newRotation;

        // transform.eulerAngles.y = spawnList[turn].eulerAngles.y;
        previousPosition = transform.position;
        startPos = transform.position;
        distance = 0;
        curTime = Time.time;
    }

    private void MovePlayer()
    {
        // Move the player forward and backward
        Vector3 movement = transform.forward * movementInput * movementSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + movement);
    }

    private void RotatePlayer()
    {
        // Rotate the player left and right
        float rotation = rotationInput * rotationSpeed * Time.deltaTime;
        transform.Rotate(0, rotation, 0);
    }
}
