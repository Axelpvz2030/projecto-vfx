using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine; 

public class CameraController : MonoBehaviour
{
    [Header("Cinemachine Cameras")]
    public CinemachineCamera topDownCamera;
    public CinemachineCamera orbitalCamera;

    public bool isOrbitalActive { get; private set; }

    private void Start()
    {
        isOrbitalActive = false;
        UpdateCameraPriorities();
    }

    private void Update()
    {
        if (Keyboard.current.shiftKey.wasPressedThisFrame)
        {
            isOrbitalActive = !isOrbitalActive;
            UpdateCameraPriorities();
        }
    }

    private void UpdateCameraPriorities()
    {
        orbitalCamera.Priority = isOrbitalActive ? 10 : 0;
        topDownCamera.Priority = !isOrbitalActive ? 10 : 0;
    }
}