using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class LevelManager : MonoBehaviour
{
	[Header("Level Order")]
	public string[] levelOrder = { "Level1", "Level2", "Level3" };
	private int currentLevelIndex = -1;

	private string currentLevel = "";

	public void LoadLevel(string levelName)
	{
		// Find index of level being loaded
		currentLevelIndex = System.Array.IndexOf(levelOrder, levelName);

		// Unload current level if exists
		UnloadCurrentLevel();

		// Load new level additively
		AsyncOperation operation = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
		currentLevel = levelName;

		Debug.Log($"Loaded level: {levelName} (Index: {currentLevelIndex})");
	}
	public void LoadNextLevel()
	{
		if (levelOrder.Length == 0)
		{
			Debug.LogWarning("No levels defined in levelOrder!");
			return;
		}

		// Calculate next level index
		int nextIndex = currentLevelIndex + 1;

		// Check if there is a next level
		if (nextIndex < levelOrder.Length)
		{
			string nextLevelName = levelOrder[nextIndex];
			Debug.Log($"Loading next level: {nextLevelName}");
			LoadLevel(nextLevelName);
		}
		else
		{
			Debug.Log("Already at last level! No more levels.");
		}
	}

	public bool HasNextLevel()
	{
		return currentLevelIndex >= 0 && currentLevelIndex < levelOrder.Length - 1;
	}
	public bool IsLastLevel()
	{
		return currentLevelIndex >= 0 && currentLevelIndex >= levelOrder.Length - 1;
	}

	public void UnloadCurrentLevel()
	{
		if (!string.IsNullOrEmpty(currentLevel))
		{
			SceneManager.UnloadSceneAsync(currentLevel);
			Debug.Log($"Unloaded level: {currentLevel}");
			currentLevel = "";
		}
	}

	public void ReloadCurrentLevel()
	{
		if (!string.IsNullOrEmpty(currentLevel))
		{
			string levelToReload = currentLevel;
			UnloadCurrentLevel();
			LoadLevel(levelToReload);
		}
	}
}