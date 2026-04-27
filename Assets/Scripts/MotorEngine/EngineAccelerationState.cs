using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class EngineAccelerationState : IEngineState
{
    public void Enter() { }
    public void Exit() { }
    public void UpdateState(EngineStateMachine stateMachine, MotorMovement motor)
    {
        float speed = motor.CurrentSpeed;
        float prevSpeed = motor.PreviousSpeed;
        if (speed >= IEngineState.FullSpeedThreshold)
        {
            stateMachine.SetState(new EngineFullSpeedState());
        }
        else if (speed < prevSpeed - IEngineState.IdleThreshold)
        {
            stateMachine.SetState(new EngineDecelerationState());
        }
        else if (speed < IEngineState.IdleThreshold)
        {
            stateMachine.SetState(new EngineIdleState());
        }
    }
}
