using UnityEngine;

public class EngineDecelerationState : IEngineState
{
    public void Enter(EngineSound engineSound) { }
    public void Exit() { }
    public void UpdateState(EngineStateMachine stateMachine, MotorMovement motor, EngineSound engineSound)
    {
        float normalizedSpeed = Mathf.InverseLerp(
            IEngineState.IdleThreshold,
            IEngineState.FullSpeedThreshold,
            motor.CurrentSpeed
        );
        engineSound.UpdatePitch(normalizedSpeed);

        float speed = motor.CurrentSpeed;
        float prevSpeed = motor.PreviousSpeed;
        if (speed < 0.1f)
        {
            stateMachine.SetState(new EngineIdleState());
        }
        else if (speed > prevSpeed)
        {
            stateMachine.SetState(new EngineAccelerationState());
            Debug.Log($"Transitioning to Acceleration State. Current Speed: {speed}, Previous Speed: {prevSpeed}");
        }
        else if (speed >= IEngineState.FullSpeedThreshold)
        {
            stateMachine.SetState(new EngineFullSpeedState());
        }
    }
}
