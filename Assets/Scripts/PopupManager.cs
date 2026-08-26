using UnityEngine;

public class PopupManager : MonoBehaviour
{
	public static PopupManager Instance;

	[Header("References")]
	public GameObject levelCompletePopup;
	public GameObject failurePopup;
	public GameObject levelOptions;
	public GameObject gameCompleted;
	public LevelManager levelManager;

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
		if (levelCompletePopup != null)
			levelCompletePopup.SetActive(false);
		if (failurePopup != null)
			failurePopup.SetActive(false);
		if (gameCompleted != null)
			gameCompleted.SetActive(false);
	}

	public void ShowLevelComplete()
	{
		if(levelManager != null && levelManager.IsLastLevel())
		{
			ShowGameCompleted();
			return;
		}
		if (levelCompletePopup != null)
		{
			levelCompletePopup.SetActive(true);
			Debug.Log("Level Complete Popup shown");
		}
		if (levelOptions != null)
		{
			levelOptions.SetActive(false);
			Debug.Log("Options hidden");
		}
	}

	public void ShowFailure()
	{
		if (failurePopup != null)
		{
			failurePopup.SetActive(true);
			Debug.Log("Failure Popup shown");
		}
	}

	public void ShowGameCompleted()
	{
		if (gameCompleted != null)
		{
			gameCompleted.SetActive(true);
			Debug.Log("Game Completed Popup shown");
		}
	}

	public void HideAllPopups()
	{
		if (levelCompletePopup != null)
			levelCompletePopup.SetActive(false);
		if (failurePopup != null)
			failurePopup.SetActive(false);
		if (levelOptions != null)
			levelOptions.SetActive(true);
		if (gameCompleted != null)
			gameCompleted.SetActive(true);
	}
}