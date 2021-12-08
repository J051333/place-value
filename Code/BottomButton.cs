using UnityEngine;

/**
 * <summary>
 * Represents a button that shows on the BottomPanel.
 * <br/>
 * <b>Author:</b> Josi Scarlett Whitlock
 * <br/>
 * <b>Version:</b> 1.0
 * </summary>
 */
public class BottomButton : MonoBehaviour {
    #region Fields
    /// <summary>Float determining the positioning of this button.</summary>
    [SerializeField]
    private float mult = 0;
    /// <summary>Reference to the panel this button belongs to.</summary>
    [SerializeField]
    private GameObject bottomPanel;
    #endregion

    #region Methods
    ///<summary>
    /// Update is called once per frame.
    ///</summary>
    void Update() {
        LocationSize();
    }

    /// <summary>
    /// Performs the process of setting the location of this button using <see cref="mult"/>.
    /// </summary>
    private void LocationSize() {
        GetComponent<RectTransform>().position = new Vector3(Screen.width / 16F * mult, GetComponent<RectTransform>().position.y, GetComponent<RectTransform>().position.z);
        // We are multplying the x value by .25f to match the distortion caused by the scale
        // of the bottomPanel.
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2((bottomPanel.GetComponent<RectTransform>().sizeDelta.y - 20) * bottomPanel.GetComponent<RectTransform>().localScale.y, bottomPanel.GetComponent<RectTransform>().sizeDelta.y - 20);

        // Previously used Screen.width / 6F, changed to make buttons square.
    }
    #endregion
}
