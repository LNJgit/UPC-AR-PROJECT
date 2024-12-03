using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LeftHandMenuTrigger : MonoBehaviour
{
    public GameObject menuCanvas; // Assign the Canvas menu in the Inspector
    public OVRHand leftHand; // Assign the OVRHand object for the left hand
    private bool isMenuVisible = false;

    void Update()
    {
        if (leftHand == null)
        {
            //Debug.LogWarning("Left hand not assigned or not being tracked.");
            return;
        }

        // Debug the tracking state
        if (!leftHand.IsTracked)
        {
            //Debug.LogWarning("Left hand is not currently tracked.");
            return;
        }

        // Debug each finger's pinch strength
        DebugFingerFlexion(leftHand);

        // Detect if the hand is closed
        if (IsHandClosed(leftHand) && !isMenuVisible)
        {
            ShowMenu();
        }
        else if (!IsHandClosed(leftHand) && isMenuVisible)
        {
            HideMenu();
        }
    }

    private bool IsHandClosed(OVRHand hand)
    {
        // Check if the hand is valid
        if (hand == null) return false;

        // Check if all fingers are flexed
        bool isClosed = hand.GetFingerPinchStrength(OVRHand.HandFinger.Index) > 0.3f &&
                        hand.GetFingerPinchStrength(OVRHand.HandFinger.Middle) > 0.3f;

        //Debug.Log($"Is Hand Closed: {isClosed}");
        return isClosed;
    }

    private void DebugFingerFlexion(OVRHand hand)
    {
        // Debug each finger's flexion value
        /*
        Debug.Log($"Index Finger Pinch Strength: {hand.GetFingerPinchStrength(OVRHand.HandFinger.Index):F2}");
        Debug.Log($"Middle Finger Pinch Strength: {hand.GetFingerPinchStrength(OVRHand.HandFinger.Middle):F2}");
        Debug.Log($"Ring Finger Pinch Strength: {hand.GetFingerPinchStrength(OVRHand.HandFinger.Ring):F2}");
        Debug.Log($"Pinky Finger Pinch Strength: {hand.GetFingerPinchStrength(OVRHand.HandFinger.Pinky):F2}");
        */
    }

    private void ShowMenu()
    {
        isMenuVisible = true;
        menuCanvas.SetActive(true);
        //Debug.Log("Menu shown.");
    }

    private void HideMenu()
    {
        isMenuVisible = false;
        menuCanvas.SetActive(false);
        //Debug.Log("Menu hidden.");
    }
}