using System;
using UnityEngine;


/// <summary>
/// Handles player movement.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
	// TODO: Refactor so that it only handles the kinematic calculations, not actually APPLYING the kinematics.

    public float SixenseJoystickXDeadzone = 0.1f,
                 SixenseJoystickYDeadzone = 0.1f;

    public Vector2 MovementInput { get; private set; }

    public float Acceleration = 10.0f;
    public Vector3 Velocity = Vector3.zero;
    public float MaxSpeed = 20.0f;
	public float FrictionMultiplier = 0.92f;

	public Transform MyTransform { get; private set; }
	public CharacterController MyCharController { get; private set; }
    void Awake()
    {
        MyCharController = GetComponent<CharacterController>();
        MyTransform = transform;
    }


    void Update()
    {
        MovementInput = GetMovementInput();
    }

	void FixedUpdate()
	{
		// Interpret movement input.
		Vector3 worldMovement = (MyTransform.right * MovementInput.x) + (MyTransform.forward * MovementInput.y);
		Vector3 accel = worldMovement * Acceleration;

		// Update the velocity.
		Vector3 newVel = Velocity + (accel * Time.fixedDeltaTime);


		// Apply friction.
		// If the player is accelerating, apply friction to the part of his velocity perpendicular to his acceleration.
		// Otherwise, if the player is moving, apply it to his whole velocity.
		// Otherwise, the player is standing still, so don't apply any friction.
		float accelSqr = accel.sqrMagnitude;
		float velSqr = newVel.sqrMagnitude;

		// 'frictionDir' is the direction that gets NO friction.
		Vector3 frictionDir = new Vector3(float.NaN, float.NaN, float.NaN);
		if (accelSqr > 0.0f)
		{
			frictionDir = accel.normalized;
		}
		else if (velSqr > 0.0f)
		{
			frictionDir = Vector3.Cross(newVel.normalized, Vector3.up);
		}

		if (!float.IsNaN(frictionDir.x))
		{
			// Get the components of velocity parallel/perpendicular to the friction dir.
			Vector3 parallel = frictionDir * Vector3.Dot(newVel, frictionDir);
			Vector3 perp = newVel - parallel;
			// Leave the parallel component alone, but apply friction to the perpendicular one.
			perp *= FrictionMultiplier;

			newVel = parallel + perp;
		}

		Velocity = newVel;


		// Constrain the velocity.
		if (Velocity.sqrMagnitude > MaxSpeed * MaxSpeed)
		{
			Velocity = Velocity.normalized * MaxSpeed;
		}

		// Apply the velocity.
		MyCharController.SimpleMove(Velocity * Time.fixedDeltaTime);
	}


    /// <summary>
    /// Gets a vector in the range { -1, -1 } to { 1, 1 } (either normalized or length 0) representing movement input.
    /// </summary>
    private Vector2 GetMovementInput()
    {
        Vector2 input = Vector2.zero;

        // Keyboard input.
		if (Input.GetKey(KeyCode.W))
		{
			input.y = 1.0f;
		}
		if (Input.GetKey(KeyCode.S))
		{
			input.y = -1.0f;
		}
		if (Input.GetKey(KeyCode.A))
		{
			input.x = -1.0f;
		}
		if (Input.GetKey(KeyCode.D))
		{
			input.x = 1.0f;
		}

        //If the input isn't unused, normalize it.
        return (input == Vector2.zero ? input : input.normalized);
    }
}