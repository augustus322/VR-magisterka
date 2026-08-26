using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
	[Header("Slides")]
	public GameObject[] slides;
	public float slideDuration = 2f;
	public string mainSceneName = "MainScene";

	[Header("Skip")]
	public bool allowSkip = true;

	private int currentSlide = 0;

	void Start()
	{
		// Hide all slides except first
		for (int i = 0; i < slides.Length; i++)
		{
			if (slides[i] != null)
				slides[i].SetActive(i == 0);
		}
		// Start slideshow
		StartCoroutine(PlaySlides());
	}

	void Update()
	{
		if (allowSkip && Input.anyKeyDown)
		{
			StopAllCoroutines();
			LoadMainScene();
		}
	}

	IEnumerator PlaySlides()
	{
		for (currentSlide = 0; currentSlide < slides.Length; currentSlide++)
		{
			yield return new WaitForSeconds(slideDuration);

			// Hide current, show next
			if (slides[currentSlide] != null)
				slides[currentSlide].SetActive(false);

			if (currentSlide + 1 < slides.Length && slides[currentSlide + 1] != null)
				slides[currentSlide + 1].SetActive(true);
		}

		LoadMainScene();
	}

	void LoadMainScene()
	{
		SceneManager.LoadScene(mainSceneName);
	}
}