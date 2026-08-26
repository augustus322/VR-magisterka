using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
	[Header("Movement")]
	public float speed = 5f;
	public Rigidbody rb;

	[Header("Input")]
	public InputActionReference forwardAction;
	public InputActionReference backwardAction;

	[Header("Crash Effects")]
	public GameObject[] wheels;
	public ParticleSystem hoodSmoke;

	private float moveInput = 0f;
	private bool hasFinished = false;
	private bool hasCrashed = false;

	void FixedUpdate()
	{
		if (hasFinished) return;

		// Move the car
		Vector3 movement = transform.right * moveInput * speed * Time.fixedDeltaTime;
		rb.MovePosition(rb.position + movement);
	}
	public void StartMovingForward()
	{
		Debug.Log("Moving Forward");
		moveInput = -1f;
	}

	public void StartMovingBackward()
	{
		Debug.Log("Moving Backward");
		moveInput = 1f;
	}

	public void StopMoving()
	{
		moveInput = 0f;
	}

	void CrashCar()
	{
		if (hasCrashed) return;
		hasCrashed = true;

		// Start smoke
		if (hoodSmoke != null)
		{
			hoodSmoke.gameObject.SetActive(true);
			hoodSmoke.Play();
		}
			
		// Detach wheels
		foreach (GameObject wheel in wheels)
		{
			wheel.transform.parent = null;
			Rigidbody wheelRb = wheel.AddComponent<Rigidbody>();
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Finish") && !hasFinished)
		{
			hasFinished = true;
			moveInput = 0f;
			Debug.Log("LEVEL COMPLETE!");
			PopupManager.Instance.ShowLevelComplete();
		}
		else if (other.CompareTag("Floor"))
		{
			hasFinished = true;
			moveInput = 0f;
			Debug.Log("TRY AGAIN");
			CrashCar();
			PopupManager.Instance.ShowFailure();
		}
	}
}