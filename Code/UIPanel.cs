using UnityEngine;

/**
 * <summary>
 * UIPanel represents the top and bottom panels of the scene.
 * <br/>
 * <b>Author:</b> Josi Scarlett Whitlock
 * <br/>
 * <b>Version:</b> 1.0
 * </summary>
 */
public class UIPanel : MonoBehaviour {
    #region Fields
    /// <summary>This UIPanel's transform.</summary>
    private RectTransform tf;
    /// <summary>Size factor of the top panel.</summary>
    private float sizeFactorT = 2.5f;
    /// <summary>Size factor of the bottom panel.</summary>
    private float sizeFactorB = 2f;
    /// <summary>T or B, top and bottom respectively.</summary>
    public char level;
    #endregion

    #region Properties
    /// <summary>Reference to the GameManager.</summary>
    private GameManager Gamemanager { get; set; }
    #endregion

    #region Methods
    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    void Start() {
        tf = GetComponent<RectTransform>();
        Gamemanager = FindObjectOfType<GameManager>().GetComponent<GameManager>();
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    void Update() {
        switch (level) {
            case 't':
                LocationInstructionsT();
                break;
            case 'b':
                LocationInstructionsB();
                break;
        }
    }

    /// <summary>
    /// Performs the process for setting the location
    /// of the bottom panel.
    /// </summary>
    private void LocationInstructionsB() {
        tf.localScale = new Vector3(tf.localScale.x, Gamemanager.uiCanvas.transform.localScale.y / sizeFactorB / 2f);
        tf.SetPositionAndRotation(new Vector3(Screen.width / 2f, tf.sizeDelta.y / (sizeFactorB * 4)), tf.rotation);
    }

    // In the future, should this project be maintained, this method might be abstracted into an interface like IHaveLocationInstructions or something
    /// <summary>
    /// Ferforms the process for setting the location
    /// of the top panel.
    /// </summary>
    void LocationInstructionsT() {
        tf.localScale = new Vector3(tf.localScale.x, Gamemanager.uiCanvas.transform.localScale.y / sizeFactorT);
        tf.SetPositionAndRotation(new Vector3(Screen.width / 2f, Screen.height - tf.sizeDelta.y / (2f * sizeFactorT)), tf.rotation);
    }
    #endregion
}
