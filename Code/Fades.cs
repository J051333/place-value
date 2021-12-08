using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/**
 * <summary>
 * <see cref="Fades"/> can fade in and fade out a scene. This component should be 
 * loaded with the scene for proper fade in.
 * <br/>
 * <b>Author:</b> Josi Scarlett Whitlock
 * <br/>
 * <b>Version:</b> 1.0
 * </summary>
 */
public class Fades : MonoBehaviour {
    #region Fields
    /// <summary>Reference to the panel displaying the fade.</summary>
    private GameObject fadePanel;
    /// <summary>Should the scene fade in on <see cref="Start"/>. Set to false
    /// to prevent fading in on load.</summary>
    public bool startFade = true;
    /// <summary>Reference to the scene's canvas.</summary>
    private Canvas canvas;
    /// <summary>Reference to <see cref="fadePanel"/>'s image component.</summary>
    private Image image;
    /// <summary>Speed at which the fade should occur.</summary>
    private static readonly float fadeSpeed = 1.25f;
    #endregion

    #region Methods
    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start() {
        // Fade in as the scene is loaded.
        if (startFade)
            FadeIn();
    }

    /// <summary>
    /// Prepares and triggers a fade out. It then loads the given scene, should any scene be provided.
    /// </summary>
    /// <param name="sceneNumber">The index number of the scene to load.</param>
    public void FadeOut(int sceneNumber = -1) {
        canvas = FindObjectOfType<Canvas>();
        fadePanel = new GameObject("Panel");
        fadePanel.transform.SetAsLastSibling();
        fadePanel.AddComponent<CanvasRenderer>();
        image = fadePanel.AddComponent<Image>();
        fadePanel.GetComponent<RectTransform>().sizeDelta = canvas.GetComponent<RectTransform>().sizeDelta;
        fadePanel.transform.SetParent(canvas.transform, false);
        image.color = Color.clear;
        StartCoroutine(FadingOut(sceneNumber));
    }

    /// <summary>
    /// Prepares and triggers a fade in. It then loads the given scene, should any scene be provided.
    /// </summary>
    /// <param name="sceneNumber">The index number of the scene to load.</param>
    public void FadeIn(int sceneNumber = -1) {
        canvas = FindObjectOfType<Canvas>();
        fadePanel = new GameObject("Panel");
        fadePanel.transform.SetAsLastSibling();
        fadePanel.AddComponent<CanvasRenderer>();
        image = fadePanel.AddComponent<Image>();
        fadePanel.GetComponent<RectTransform>().sizeDelta = canvas.GetComponent<RectTransform>().sizeDelta;
        fadePanel.transform.SetParent(canvas.transform, false);
        image.color = Color.black;
        StartCoroutine(FadingIn(sceneNumber));
    }
    #endregion

    #region Coroutines
    /// <summary>
    /// Fades the <see cref="fadePanel"/> alpha channel from 0 to 1.
    /// </summary>
    /// <param name="sceneNumber">Index number of the scene to load.</param>
    /// <returns><c>null</c></returns>
    private IEnumerator FadingOut(int sceneNumber) {
        float fade;
        while (image.color.a < 1) {
            fade = image.color.a + (fadeSpeed * Time.deltaTime);
            image.color = new Color(0, 0, 0, fade);
            fadePanel.transform.SetAsLastSibling();
            yield return null;
        }
        if (sceneNumber != -1)
            SceneManager.LoadScene(sceneNumber);
    }

    /// <summary>
    /// Fades the <see cref="fadePanel"/> alpha channel from 1 to 0.
    /// </summary>
    /// <param name="sceneNumber">Index number of the scene to load.</param>
    /// <returns><c>null</c></returns>
    private IEnumerator FadingIn(int sceneNumber) {
        float fade;
        while (image.color.a > 0) {
            fade = image.color.a - (fadeSpeed * Time.deltaTime);
            image.color = new Color(0, 0, 0, fade);
            fadePanel.transform.SetAsLastSibling();
            yield return null;
        }
        if (sceneNumber != -1)
            SceneManager.LoadScene(sceneNumber);
        Destroy(fadePanel);
    }
    #endregion
}
