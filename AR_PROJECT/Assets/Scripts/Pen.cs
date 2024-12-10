using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.HandGrab;
using System.Linq;

public class Pen : MonoBehaviour
{
    [Header("Pen properties")]
    public Transform tip;
    public Material drawingMaterial;
    public Material tipMaterial;
    [Range(0f, 0.1f)]
    public float penWidth = 0.01f;
    public Color[] penColors;
    public float maxDrawDistance = 0.05f; // Maximum distance to detect surface for drawing
    public LayerMask drawingSurfaceMask; // Layer mask for surfaces to draw on

    [Header("Hands and grabbable")]
    public HandGrabInteractable grabbable;

    [Header("Drawing Mode")]
    public bool drawOnSurface = true; // Toggle for drawing on surfaces or freely in the scene

    private LineRenderer currentDrawing;
    private List<Vector3> currentLinePoints = new();
    private int currentColorIndex;

    private void Start()
    {
        currentColorIndex = 0;
        tipMaterial.color = penColors[currentColorIndex];
    }

    private void Update()
    {
        bool isGrabbed = grabbable.Interactors.Count > 0;

        if (isGrabbed)
        {
            if (drawOnSurface)
            {
                if (IsNearSurface(out Vector3 hitPosition))
                {
                    Draw(hitPosition); // Pass the position on the surface
                }
            }
            else
            {
                Draw(tip.position); // Use the pen's tip position for free drawing
            }
        }
        else if (currentDrawing != null)
        {
            FinalizeDrawing();
        }

        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            SwitchColor();
        }

        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            ToggleDrawingMode();
        }
    }

    private bool IsNearSurface(out Vector3 hitPosition)
    {
        // Raycast from the tip position in the downward direction to detect a surface
        if (Physics.Raycast(tip.position, -tip.up, out RaycastHit hit, maxDrawDistance, drawingSurfaceMask))
        {
            hitPosition = hit.point; // Get the exact hit position on the surface
            return true;
        }
        hitPosition = Vector3.zero;
        return false;
    }

    private void Draw(Vector3 position)
    {
        if (currentDrawing == null)
        {
            StartNewLine();
        }

        // Add points only if they are sufficiently far from the last point
        if (currentLinePoints.Count == 0 || Vector3.Distance(currentLinePoints[^1], position) > 0.01f)
        {
            currentLinePoints.Add(position);
            currentDrawing.positionCount = currentLinePoints.Count;
            currentDrawing.SetPosition(currentLinePoints.Count - 1, position);
        }
    }

    private void StartNewLine()
    {
        GameObject lineObject = new GameObject($"DrawingObject_{Time.time}");
        currentDrawing = lineObject.AddComponent<LineRenderer>();
        currentDrawing.material = drawingMaterial;
        currentDrawing.startColor = currentDrawing.endColor = penColors[currentColorIndex];
        currentDrawing.startWidth = currentDrawing.endWidth = penWidth;
        currentDrawing.useWorldSpace = true; // Ensure the line sticks to world positions
        currentDrawing.tag = "DrawingObject";

        currentLinePoints.Clear(); // Reset the list of points for the new line
    }

    private void FinalizeDrawing()
    {
        currentDrawing = null; // Stop updating the current line
    }

    private void SwitchColor()
    {
        currentColorIndex = (currentColorIndex + 1) % penColors.Length;
        tipMaterial.color = penColors[currentColorIndex];
    }

    private void ToggleDrawingMode()
    {
        drawOnSurface = !drawOnSurface;
        Debug.Log($"Drawing mode switched to: {(drawOnSurface ? "Surface" : "Free")}");
    }
}