using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetVisibility : MonoBehaviour
{
    public GameObject player; // Assign your player object in the Inspector
    private Renderer targetRenderer;
    public float visibilityDistance = 1f;
    public bool forceVisible = false;


    void Start()
    {
        // Get the Renderer component to control visibility
        targetRenderer = GetComponent<Renderer>();

        // Hide the target at the start
        targetRenderer.enabled = false;
    }

    void Update()
    {

            if (forceVisible)
    {
        targetRenderer.enabled = true; // Force visibility
        return;
    }
        // Calculate distance between the player and the target
        float distance = Vector3.Distance(player.transform.position, transform.position);

        // If the player is within the specified distance, show the target
        if (GetComponent<Collider>().enabled && distance <= visibilityDistance)
        {
            targetRenderer.enabled = true; // Show the target
            forceVisible=true;
        }
        else {
            targetRenderer.enabled = false;
        }

    }
}
