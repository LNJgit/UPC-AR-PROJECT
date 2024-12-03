using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using System.Linq;

public class Eraser : MonoBehaviour
{
    [Header("Eraser Settings")]
    public float eraseDistance = 0.001f; // Distancia para detectar si el DrawingObject está cerca
    public HandGrabInteractable grabbable;
    public OVRGrabber rightHand; // Para verificar si el cubo está siendo agarrado
    public OVRGrabber leftHand;  // Para verificar si el cubo está siendo agarrado
    public string drawingObjectTag = "DrawingObject"; // Tag para los objetos que serán borrados

    private void Update()
    {
        // Verificar si el cubo está siendo agarrado por alguna de las manos
        bool isGrabbed = grabbable.Interactors.Count > 0;

        bool isRightHandGrabbing = isGrabbed && grabbable.Interactors.Any(interactor => interactor.gameObject.CompareTag("RightHand"));
        bool isLeftHandGrabbing = isGrabbed && grabbable.Interactors.Any(interactor => interactor.gameObject.CompareTag("LeftHand"));

        // Si está siendo agarrado, buscar objetos cercanos que sean "DrawingObject"
        if (isRightHandGrabbing || isLeftHandGrabbing)
        {
            Debug.Log(" ---------- Goma Agarrada");
            CheckForObjectsToErase();
        }
    }

    private void CheckForObjectsToErase()
    {
        // Obtener todos los objetos con el tag "DrawingObject"
        GameObject[] drawingObjects = GameObject.FindGameObjectsWithTag("DrawingObject");

        Debug.Log("Size drawingObjects" + drawingObjects.Length );

        foreach (GameObject drawingObject in drawingObjects)
        {
            // Comprobar la distancia entre el cubo y el DrawingObject
            if (Vector3.Distance(transform.position, drawingObject.transform.position) < eraseDistance)
            {
                Debug.Log("Borrando");
                // Si está cerca, borrar el objeto (en este caso, se destruye el objeto)
                Destroy(drawingObject);
            }
        }
    }
}
