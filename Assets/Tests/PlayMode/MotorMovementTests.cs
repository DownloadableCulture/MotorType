using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Reflection;

public class MotorMovementTests
{
    [UnityTest]
    public IEnumerator Rigidbody_HasZeroVelocity_OnStartup()
    {
        var go = new GameObject("Motor");
        var rb = go.AddComponent<Rigidbody>();
        rb.useGravity = false;

        go.AddComponent<MotorMovement>();

        yield return new WaitForFixedUpdate();

        Assert.AreEqual(Vector3.zero, rb.linearVelocity,
            "Rigidbody should have zero velocity on startup");

        Object.Destroy(go);
    }

    [UnityTest]
    public IEnumerator Rigidbody_Velocity_Increases()
    {
        var go = new GameObject("Motor");
        var rb = go.AddComponent<Rigidbody>();
        rb.useGravity = false;

        var movement = go.AddComponent<MotorMovement>();

        var mockInput = new MockMotorInput
        {
            AccelerationInput = 1f,
            BrakeInput = 0f,
            SteerInput = 0f
        };

        typeof(MotorMovement)
            .GetField("_motorInput", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(movement, mockInput);

        rb.linearVelocity = Vector3.zero;

        yield return new WaitForFixedUpdate();

        Assert.Greater(rb.linearVelocity.magnitude, 0.01f, "Rigidbody should accelerate when AccelerationInput > 0");

        Object.Destroy(go);
    }

    [UnityTest]
    public IEnumerator Rigidbody_Velocity_Decreases_WithBrake()
    {
        var go = new GameObject("Motor");
        var rb = go.AddComponent<Rigidbody>();
        rb.useGravity = false;
        var movement = go.AddComponent<MotorMovement>();
        var mockInput = new MockMotorInput
        {
            AccelerationInput = 0f,
            BrakeInput = 1f,
            SteerInput = 0f
        };
        typeof(MotorMovement)
            .GetField("_motorInput", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(movement, mockInput);
        rb.linearVelocity = Vector3.forward * 10f;
        yield return new WaitForFixedUpdate();
        Assert.Less(rb.linearVelocity.magnitude, 9.5f, "Rigidbody should decelerate when BrakeInput > 0");
        Object.Destroy(go);
    }

    // Mock class for IMotorInput
    private class MockMotorInput : IMotorInput
    {
        public float AccelerationInput { get; set; }
        public float BrakeInput { get; set; }
        public float SteerInput { get; set; }
    }
}