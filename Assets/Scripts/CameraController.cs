using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine; // Required for Cinemachine 3

public class CameraController : MonoBehaviour
{
    [Header("Cinemachine Cameras")]
    public CinemachineCamera topDownCamera;
    public CinemachineCamera orbitalCamera;

    // Public state for the PlayerMovement script to read
    public bool isOrbitalActive { get; private set; }

    private void Start()
    {
        // Start the game in Top-Down mode by default
        isOrbitalActive = false;
        UpdateCameraPriorities();
    }

    private void Update()
    {
        // Toggle camera when the Left Shift key is pressed
        if (Keyboard.current.shiftKey.wasPressedThisFrame)
        {
            isOrbitalActive = !isOrbitalActive;
            UpdateCameraPriorities();
        }
    }

    private void UpdateCameraPriorities()
    {
        // Cinemachine automatically blends to the camera with the highest Priority number.
        // We give the active camera a priority of 10, and the inactive one a priority of 0.
        orbitalCamera.Priority = isOrbitalActive ? 10 : 0;
        topDownCamera.Priority = !isOrbitalActive ? 10 : 0;
    }
}