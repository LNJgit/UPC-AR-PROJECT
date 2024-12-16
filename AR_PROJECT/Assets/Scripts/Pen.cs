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
        if (penColors == null || penColors.Length == 0)
        {
            Debug.LogError("penColors no está configurado o está vacío. Por favor, añade colores en el Inspector.");
            return;
        }

        currentColorIndex = 0;
        tipMaterial.color = penColors[currentColorIndex];
        Debug.Log($"Colores inicializados correctamente: {penColors.Length} colores detectados.");
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

        /*if (OVRInput.GetDown(OVRInput.Button.One))
        {
            SwitchColor();
        }*/

        /*if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            ToggleDrawingMode();
        }*/
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

        // Asegura que la punta y la línea tengan siempre el mismo color
        Color currentColor = penColors[currentColorIndex];
        currentDrawing.startColor = currentDrawing.endColor = currentColor;
        tipMaterial.color = currentColor;

        currentDrawing.startWidth = currentDrawing.endWidth = penWidth;
        currentDrawing.useWorldSpace = true;
        currentDrawing.tag = "DrawingObject";

        currentLinePoints.Clear();
    }

    private void FinalizeDrawing()
    {
        currentDrawing = null; // Stop updating the current line
    }

    private void SwitchColor()
    {
        if (penColors == null || penColors.Length == 0)
        {
            Debug.LogError("penColors no está configurado. No se puede cambiar el color. PRIVATE");
            return;
        }

        // Este log ayuda a depurar si el método se llama inesperadamente
        Debug.Log($"SwitchColor llamado manualmente. Índice actual: {currentColorIndex}");

        currentColorIndex = (currentColorIndex + 1) % penColors.Length;
        tipMaterial.color = penColors[currentColorIndex];
        Debug.Log($"Color cambiado a índice {currentColorIndex}: {penColors[currentColorIndex]}");
    }

    public void SwitchColors()
    {
        if (penColors == null || penColors.Length == 0)
        {
            Debug.LogError("penColors no está configurado. No se puede cambiar el color. PUBLIC");
            return;
        }
        

        // Este log ayuda a depurar si el método se llama inesperadamente
        Debug.Log($"SwitchColor llamado manualmente. Índice actual: {currentColorIndex}");

        currentColorIndex = (currentColorIndex + 1) % penColors.Length;
        tipMaterial.color = penColors[currentColorIndex];
        Debug.Log($"Color cambiado a índice {currentColorIndex}: {penColors[currentColorIndex]}");
    }

    public void SwitchColorToBlue()
    {
        if (penColors == null || penColors.Length == 0)
        {
            Debug.LogError("penColors no está configurado. No se puede cambiar el color. PUBLIC");
            return;
        }
        

        // Este log ayuda a depurar si el método se llama inesperadamente
        Debug.Log($"SwitchColor llamado manualmente. Índice actual: {currentColorIndex}");

        currentColorIndex = 0;
        tipMaterial.color = penColors[currentColorIndex];
        Debug.Log($"Color cambiado a índice {currentColorIndex}: {penColors[currentColorIndex]}");
    }

    public void SwitchColorToBlack()
    {
        if (penColors == null || penColors.Length == 0)
        {
            Debug.LogError("penColors no está configurado. No se puede cambiar el color. PUBLIC");
            return;
        }
        

        // Este log ayuda a depurar si el método se llama inesperadamente
        Debug.Log($"SwitchColor llamado manualmente. Índice actual: {currentColorIndex}");

        currentColorIndex = 1;
        tipMaterial.color = penColors[currentColorIndex];
        Debug.Log($"Color cambiado a índice {currentColorIndex}: {penColors[currentColorIndex]}");
    }


    public void SwitchColorToRed()
    {
        if (penColors == null || penColors.Length == 0)
        {
            Debug.LogError("penColors no está configurado. No se puede cambiar el color. PUBLIC");
            return;
        }
        else
        {
            Debug.LogError("penColors está configurado. . PUBLIC" + penColors.Length);
        }

        // Este log ayuda a depurar si el método se llama inesperadamente
        Debug.Log($"SwitchColor llamado manualmente. Índice actual: {currentColorIndex}");

        currentColorIndex = 2;
        tipMaterial.color = penColors[currentColorIndex];
        Debug.Log($"Color cambiado a índice {currentColorIndex}: {penColors[currentColorIndex]}");
    }


    private void ToggleDrawingMode()
    {
        drawOnSurface = !drawOnSurface;
        Debug.Log($"Drawing mode switched to: {(drawOnSurface ? "Surface" : "Free")}");
    }
}