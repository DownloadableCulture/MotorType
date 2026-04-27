using UnityEngine;

public class EngineFullSpeedState : IEngineState
{
    public void Enter(EngineSound engineSound) 
    {
        engineSound.PlayFullSpeed();
    }
    public void Exit() { }
    public void UpdateState(EngineStateMachine stateMachine, MotorMovement motor, EngineSound engineSound)
    {
        float speed = motor.CurrentSpeed;
        if (speed < IEngineState.FullSpeedThreshold)
        {
            stateMachine.SetState(new EngineDecelerationState());
        }
    }
}
