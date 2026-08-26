using UnityEngine;
using Unity.XR.CoreUtils;
using UnityEngine.XR;

public class PlaceXROrigin : MonoBehaviour
{
	[Header("Starting Position")]
	public Vector3 startPosition = new Vector3(0f, 0f, 0f);

	private XROrigin xrOrigin;
	private bool hasBeenSet = false;

	void Start()
	{
		xrOrigin = GetComponent<XROrigin>();
	}

	void Update()
	{
		// Set position once after tracking is fully initialized
		if (!hasBeenSet && Time.frameCount > 10)
		{
			xrOrigin.MoveCameraToWorldLocation(startPosition);
			hasBeenSet = true;
			Debug.Log($"XR Origin camera positioned at: {startPosition}");
		}
	}
}