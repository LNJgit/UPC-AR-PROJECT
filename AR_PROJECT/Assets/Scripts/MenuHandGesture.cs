using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using UnityEngine;

public class LeftHandMenuTrigger : MonoBehaviour
{
    public GameObject menuCanvas; // Assign the Canvas menu in the Inspector
    public OVRHand leftHand; // Assign the OVRHand object for the left hand
    private bool isMenuVisible = false;

    public HandGrabInteractable grabbable;  // Object that can be grabbed
    public OVRGrabber leftGrabber;          // The grabber for the left hand

    void Update()
    {
        if (leftHand == null || !leftHand.IsTracked)
        {
            // If the left hand is not assigned or not tracked, do nothing
            return;
        }

        // Detect if the hand is closed (i.e., pinch gesture)
        if (IsHandClosed(leftHand) && !isMenuVisible && !IsHandHoldingObject())
        {
            ShowMenu();
        }
        else if (!IsHandClosed(leftHand) && isMenuVisible)
        {
            HideMenu();
        }
        else if (IsHandHoldingObject()) // When holding an object, ensure the menu is disabled
        {
            DisableMenu();
        }
    }

    private bool IsHandClosed(OVRHand hand)
    {
        // Check if the hand is closed (fingers pinching)
        if (hand == null) return false;

        bool isClosed = hand.GetFingerPinchStrength(OVRHand.HandFinger.Index) > 0.3f &&
                        hand.GetFingerPinchStrength(OVRHand.HandFinger.Middle) > 0.3f;

        return isClosed;
    }

    private bool IsHandHoldingObject()
    {
        // Check if the left hand is holding an object
        // This uses the grabbedObject property to check if the hand is holding something
        bool isHoldingObject = leftGrabber.grabbedObject != null;

        return isHoldingObject;
    }

    private void ShowMenu()
    {
        if (isMenuVisible || IsHandHoldingObject()) return;

        isMenuVisible = true;
        menuCanvas.SetActive(true);
        //Debug.Log("Menu shown.");
    }

    private void HideMenu()
    {
        if (!isMenuVisible) return;

        isMenuVisible = false;
        menuCanvas.SetActive(false);
        //Debug.Log("Menu hidden.");
    }

    private void DisableMenu()
    {
        // Disable the menu visually when holding an object
        if (isMenuVisible)
        {
            isMenuVisible = false;
            menuCanvas.SetActive(false);
        }
        // Optionally, add other visual cues here to indicate the menu is disabled
        //Debug.Log("Menu disabled while holding an object.");
    }
}
