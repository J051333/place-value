using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/**
 * <summary>
 * Controls the main start menu.
 * <br/>
 * <b>Author:</b> Josi Scarlett Whitlock
 * <br/>
 * <b>Version:</b> 1.0
 * </summary>
 */
public class MainMenuManager : MonoBehaviour {

    #region Fields
    /// <summary>Reference to the title text object.</summary>
    [SerializeField]
    private Text titleText;
    /// <summary>Reference to the little text object where I credit myself :3.</summary>
    [SerializeField]
    private Text creditText; // :)
    /// <summary>Reference to the fading object.</summary>
    private Fades fades;
    #endregion

    #region Methods
    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    void Start() {
        titleText.text = "Place Values";
        creditText.text = "made by Josi at RC3";
    }

    /// <summary>
    /// Called by the start button, loads the next scene.
    /// </summary>
    public void Proceed() {
        FindObjectOfType<Fades>().FadeOut(1);
    }
    /// <summary>
    /// Called by the help button, loads the help screen.
    /// </summary>
    public void Help() {
        FindObjectOfType<Fades>().FadeOut(2);
    }

    #endregion
}
