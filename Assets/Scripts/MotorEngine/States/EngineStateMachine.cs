using UnityEngine;

public class EngineStateMachine
{
    private IEngineState _currentState;
    private readonly MotorMovement _motor;
    private readonly EngineSound _engineSound;

    public EngineStateMachine(MotorMovement motor, EngineSound engineSound)
    {
        _motor = motor;
        _engineSound = engineSound;
        SetState(new EngineIdleState());
    }

    public void SetState(IEngineState newState)
    {
        Debug.Log($"[EngineStateMachine] Switching from {_currentState?.GetType().Name ?? "None"} to {newState.GetType().Name}");
        _currentState?.Exit();
        _currentState = newState;
        _currentState?.Enter(_engineSound);
    }

    public void Update()
    {
        _currentState?.UpdateState(this, _motor, _engineSound);
    }
}