using UnityEngine;
using UnityEngine.EventSystems;

public class VRCarButtonController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	public CarController carController;
	public bool isForwardButton; // true = forward, false = backward

	public void OnPointerDown(PointerEventData eventData)
	{
		// Button pressed
		if (carController == null) return;

		if (isForwardButton)
			carController.StartMovingForward();
		else
			carController.StartMovingBackward();

		Debug.Log("Button pressed");
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		// Button released
		if (carController == null) return;

		carController.StopMoving();
		Debug.Log("Button released");
	}
}