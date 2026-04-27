using UnityEngine;

public class EngineDecelerationState : IEngineState
{
    public void Enter() { }
    public void Exit() { }
    public void UpdateState(EngineStateMachine stateMachine, MotorMovement motor)
    {
        float speed = motor.CurrentSpeed;
        float prevSpeed = motor.PreviousSpeed;
        if (speed < 0.1f)
        {
            stateMachine.SetState(new EngineIdleState());
        }
        else if (speed > prevSpeed + IEngineState.IdleThreshold)
        {
            stateMachine.SetState(new EngineAccelerationState());
        }
        else if (speed >= IEngineState.FullSpeedThreshold)
        {
            stateMachine.SetState(new EngineFullSpeedState());
        }
    }
}
