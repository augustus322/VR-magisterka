using UnityEngine;

public class LevelCompletePopup : MonoBehaviour
{
	public static LevelCompletePopup Instance;

	[Header("References")]
	public GameObject popupCanvas;
	public GameObject levelOptions;

	void Awake()
	{
		// Set singleton
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}
	void Start()
	{
		if (popupCanvas != null)
			popupCanvas.SetActive(false);
	}

	public void ShowPopup()
	{
		if (popupCanvas != null)
		{
			popupCanvas.SetActive(true);
			Debug.Log("Popup shown");
		}
		if (levelOptions != null)
		{
			levelOptions.SetActive(false);
			Debug.Log("Options hidden");
		}
	}

	public void HidePopup()
	{
		if (popupCanvas != null)
		{
			popupCanvas.SetActive(false);
		}
	}
}