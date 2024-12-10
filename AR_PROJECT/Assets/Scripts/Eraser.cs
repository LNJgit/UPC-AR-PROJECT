using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using System.Linq;

public class Eraser : MonoBehaviour
{
    [Header("Eraser Settings")]
    public float eraseDistance = 0.1f; // Distance to detect points to erase
    public HandGrabInteractable grabbable;
    public OVRGrabber rightHand;
    public OVRGrabber leftHand;
    public string drawingObjectTag = "DrawingObject"; // Tag for objects to erase
    private bool isGrabbed;

    private void Update()
    {
        // Update grab status
        isGrabbed = grabbable.Interactors.Count > 0;

        bool isRightHandGrabbing = isGrabbed && grabbable.Interactors.Any(interactor => interactor.gameObject.CompareTag("RightHand"));
        bool isLeftHandGrabbing = isGrabbed && grabbable.Interactors.Any(interactor => interactor.gameObject.CompareTag("LeftHand"));

        // Only check for erasing if the eraser is actively grabbed
        if (isRightHandGrabbing || isLeftHandGrabbing)
        {
            CheckForPointsToErase();
        }
    }

    private void CheckForPointsToErase()
    {
        // Get all objects with the tag "DrawingObject"
        GameObject[] drawingObjects = GameObject.FindGameObjectsWithTag(drawingObjectTag);
        float eraseDistance = 0.1f; // Distance to detect points to erase
        foreach (GameObject drawingObject in drawingObjects)
        {
            LineRenderer lineRenderer = drawingObject.GetComponent<LineRenderer>();
            if (lineRenderer == null) continue;

            List<Vector3> updatedPositions = new List<Vector3>();
            bool isErasing = false;

            for (int i = 0; i < lineRenderer.positionCount; i++)
            {
                Vector3 pointPosition = lineRenderer.GetPosition(i);
                float distance = Vector3.Distance(transform.position, pointPosition);

                if (distance < eraseDistance)
                {
                    Debug.Log($"Erasing point {i} from DrawingObject '{drawingObject.name}'");
                    isErasing = true;
                }
                else
                {
                    updatedPositions.Add(pointPosition);
                }

                // If erasing, split the line at this point
                if (isErasing && updatedPositions.Count > 0)
                {
                    CreateNewLineSegment(updatedPositions, lineRenderer);
                    updatedPositions.Clear();
                    isErasing = false;
                }
            }

            // Handle the remaining segment after the loop
            if (updatedPositions.Count > 0)
            {
                lineRenderer.positionCount = updatedPositions.Count;
                lineRenderer.SetPositions(updatedPositions.ToArray());
            }
            else
            {
                Destroy(drawingObject); // If no points remain, destroy the object
            }
        }
    }

    private void CreateNewLineSegment(List<Vector3> positions, LineRenderer originalLineRenderer)
    {
        GameObject newLineObject = new GameObject("DrawingObject_Segment");
        LineRenderer newLineRenderer = newLineObject.AddComponent<LineRenderer>();

        // Configure the new LineRenderer
        newLineRenderer.positionCount = positions.Count;
        newLineRenderer.SetPositions(positions.ToArray());
        newLineRenderer.material = originalLineRenderer.material;
        newLineRenderer.startColor = originalLineRenderer.startColor;
        newLineRenderer.endColor = originalLineRenderer.endColor;
        newLineRenderer.startWidth = originalLineRenderer.startWidth;
        newLineRenderer.endWidth = originalLineRenderer.endWidth;
        newLineRenderer.tag = originalLineRenderer.tag;

        Debug.Log("Created new line segment.");
    }
}
