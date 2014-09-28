using System;
using UnityEngine;


/// <summary>
/// Handles the player looking around using the mouse or a hydra joystick.
/// </summary>
public class PlayerLooking : MonoBehaviour
{
    public Vector2 LookMovement { get; private set; }

    public float MouseLookXSpeed = 100.0f;
	public float MouseLookYSpeed = 120.0f;
	public float HydraLookXSpeed = 130.0f;
	public float HydraLookYSpeed = 150.0f;
    private Transform trans;


    void Awake()
    {
        trans = transform;
    }

    private void UpdateInput()
    {
        // Mouse input.
        LookMovement = new Vector2(Input.GetAxis("Mouse X") * MouseLookXSpeed,
                                   -Input.GetAxis("Mouse Y") * MouseLookYSpeed);
    }
    void FixedUpdate()
    {
        UpdateInput();

        Vector3 ea = trans.localEulerAngles;
        ea += new Vector3(LookMovement.y * Time.fixedDeltaTime, LookMovement.x * Time.fixedDeltaTime, 0.0f);
        ea.x = PushToEnds(90.0f, 270.0f, ea.x, 5.0f);
        trans.localEulerAngles = ea;
    }

    private static float PushToEnds(float start, float end, float value, float feather)
    {
		if (value < start || value > end)
		{
			return value;
		}

        float distS = Mathf.Abs(value - start),
              distE = Mathf.Abs(value - end);

        if (distS < distE)
        {
            return start - feather;
        }
        else
        {
            return end + feather;
        }
    }
}