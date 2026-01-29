using UnityEngine;
using UnityEngine.InputSystem;

public class DemoCameraSwitching : MonoBehaviour
{
    [SerializeField] GameObject _motorCamera;
    [SerializeField] GameObject _walkCamera;
    [SerializeField] InputActionReference ToggleCamera;

    private bool _motorCamActive = true;

    private void Start()
    {
        SetCamera(_motorCamActive);
    }
    void Update()
    {
        if (ToggleCamera.action.WasPerformedThisFrame())
        {
            _motorCamActive = !_motorCamActive;
            SetCamera(_motorCamActive);
        }

    }
    void SetCamera(bool motorCam)
    {
        _motorCamera.gameObject.SetActive(motorCam);
        _walkCamera.gameObject.SetActive(!motorCam);
    }
    private void OnEnable()
    {
        ToggleCamera.action.Enable();
    }

    private void OnDisable()
    {
        ToggleCamera.action.Disable();
    }


}
