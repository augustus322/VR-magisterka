using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System.Collections.Generic;
using System.Linq;

public class AttachmentDetector : MonoBehaviour
{
	[Header("Settings")]
	[Tooltip("Input action to attach/detach (e.g., Trigger button)")]
	public InputActionReference attachAction;

	[Tooltip("Material to highlight attachable objects")]
	public Material highlightMaterial;

	[Header("Joint Settings")]
	[Tooltip("Force needed to break the attachment (0 = unbreakable)")]
	public float jointBreakForce = 0f; // 0 = unbreakable

	private XRGrabInteractable grabInteractable;
	private GameObject currentTarget;
	private Renderer highlightedRenderer;
	private Material[] originalMaterials;

	// Track ALL joints and connected objects
	private List<FixedJoint> allJoints = new List<FixedJoint>();
	private List<GameObject> attachedObjects = new List<GameObject>();

	void Start()
	{
		grabInteractable = GetComponent<XRGrabInteractable>();

		if (attachAction != null)
			attachAction.action.Enable();

		// Setup grab events
		if (grabInteractable != null)
		{
			grabInteractable.selectEntered.AddListener(OnGrabbed);
			grabInteractable.selectExited.AddListener(OnReleased);
		}
	}

	void Update()
	{
		if (attachAction == null) return;

		// Check for button press
		if (attachAction.action.triggered && IsGrabbed())
		{
			// PRIORITY: If we have joints, DETACH first (regardless of currentTarget)
			if (allJoints.Count > 0)
			{
				DetachFromAll();
			}
			// Only attach if we have NO joints AND we have a current target
			else if (currentTarget != null && !IsAttachedTo(currentTarget))
			{
				// PREVENT MUTUAL ATTACHMENT: Check if target is already attached to THIS object
				AttachmentDetector targetDetector = currentTarget.GetComponent<AttachmentDetector>();
				if (targetDetector != null && targetDetector.IsAttachedTo(gameObject))
				{
					Debug.Log($"Prevented mutual attachment: {currentTarget.name} is already attached to {gameObject.name}");
					return;
				}

				AttachToTarget();
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (!IsGrabbed()) return;
		if (!other.CompareTag("Attachable")) return;
		if (other.gameObject == gameObject) return;
		if (IsAttachedTo(other.gameObject)) return;

		// PREVENT MUTUAL ATTACHMENT: Don't highlight if target is already attached to us
		AttachmentDetector targetDetector = other.gameObject.GetComponent<AttachmentDetector>();
		if (targetDetector != null && targetDetector.IsAttachedTo(gameObject))
		{
			return; // Skip highlighting to prevent confusion
		}

		// Remove old highlight, set new target
		RemoveHighlight();
		currentTarget = other.gameObject;
		ApplyHighlight(currentTarget);
	}

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject == currentTarget)
		{
			RemoveHighlight();
			currentTarget = null;
		}
	}

	private bool IsGrabbed()
	{
		return grabInteractable != null && grabInteractable.isSelected;
	}

	private bool IsAttachedTo(GameObject obj)
	{
		return attachedObjects.Contains(obj);
	}

	private void ApplyHighlight(GameObject obj)
	{
		if (obj == null || highlightMaterial == null) return;

		highlightedRenderer = obj.GetComponent<Renderer>();
		if (highlightedRenderer != null)
		{
			originalMaterials = highlightedRenderer.materials;

			Material[] highlightMats = new Material[originalMaterials.Length];
			for (int i = 0; i < highlightMats.Length; i++)
				highlightMats[i] = highlightMaterial;

			highlightedRenderer.materials = highlightMats;
		}
	}

	private void RemoveHighlight()
	{
		if (highlightedRenderer != null && originalMaterials != null)
		{
			highlightedRenderer.materials = originalMaterials;
			highlightedRenderer = null;
			originalMaterials = null;
		}
	}

	private void AttachToTarget()
	{
		if (currentTarget == null) return;

		RemoveHighlight();

		// Create FixedJoint
		Rigidbody targetRb = currentTarget.GetComponent<Rigidbody>();
		if (targetRb == null)
		{
			Debug.LogWarning($"Target {currentTarget.name} has no Rigidbody!");
			return;
		}

		// Create new joint
		FixedJoint newJoint = gameObject.AddComponent<FixedJoint>();
		newJoint.connectedBody = targetRb;
		newJoint.breakForce = jointBreakForce == 0f ? Mathf.Infinity : jointBreakForce;

		// Track the joint and connected object
		allJoints.Add(newJoint);
		attachedObjects.Add(currentTarget);

		Debug.Log($"Attached: {gameObject.name} → {currentTarget.name} (Total joints: {allJoints.Count})");

		// Release from hand
		if (grabInteractable.isSelected)
		{
			var interactor = grabInteractable.firstInteractorSelecting;
			if (interactor != null)
				grabInteractable.interactionManager.SelectExit(interactor, grabInteractable);
		}

		currentTarget = null;
	}

	private void DetachFromAll()
	{
		if (allJoints.Count == 0) return;

		Debug.Log($"Detaching from {allJoints.Count} objects");

		// Make a copy of attached objects to avoid modification during iteration
		List<GameObject> attachedObjectsCopy = new List<GameObject>(attachedObjects);

		// FIRST: Tell all attached objects to remove their joints to THIS object
		foreach (GameObject attachedObj in attachedObjectsCopy)
		{
			if (attachedObj != null)
			{
				AttachmentDetector otherDetector = attachedObj.GetComponent<AttachmentDetector>();
				if (otherDetector != null)
				{
					otherDetector.RemoveJointToObject(gameObject);
					Debug.Log($"Removed joint from {attachedObj.name} → {gameObject.name}");
				}
			}
		}

		// THEN: Destroy all joints from THIS object to others
		for (int i = allJoints.Count - 1; i >= 0; i--)
		{
			if (allJoints[i] != null)
			{
				Destroy(allJoints[i]);
				Debug.Log($"Destroyed joint from {gameObject.name} → {attachedObjects[i].name}");
			}
		}

		// Clear tracking lists
		allJoints.Clear();
		attachedObjects.Clear();

		// Clear any pending highlights
		RemoveHighlight();
		currentTarget = null;

		Debug.Log($"Detached from ALL {attachedObjectsCopy.Count} objects (both directions)");
	}

	// Helper method for other objects to remove joints pointing to a specific target
	public void RemoveJointToObject(GameObject targetObject)
	{
		int index = attachedObjects.IndexOf(targetObject);
		if (index >= 0 && index < allJoints.Count)
		{
			// Destroy the joint
			if (allJoints[index] != null)
				Destroy(allJoints[index]);

			// Remove from tracking lists
			allJoints.RemoveAt(index);
			attachedObjects.RemoveAt(index);

			Debug.Log($"Removed joint from {gameObject.name} → {targetObject.name} (by external request)");
		}
	}

	// Get all attached objects
	public List<GameObject> GetAttachedObjects()
	{
		return attachedObjects;
	}

	// Check if attached to anything
	public bool IsAttachedToAnything()
	{
		return allJoints.Count > 0;
	}

	private void OnGrabbed(SelectEnterEventArgs args)
	{
		// Do nothing on grab
	}

	private void OnReleased(SelectExitEventArgs args)
	{
		// Do nothing on release
	}

	void OnDisable()
	{
		RemoveHighlight();
	}

	void OnDestroy()
	{
		RemoveHighlight();

		// Clean up any remaining joints
		for (int i = allJoints.Count - 1; i >= 0; i--)
		{
			if (allJoints[i] != null)
				Destroy(allJoints[i]);
		}
		allJoints.Clear();
		attachedObjects.Clear();
	}
}