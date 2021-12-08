using UnityEngine;

/**
 * <summary>
 * Represents the boundaries separating the place values.
 * <br/>
 * <b>Author:</b> Josi Scarlett Whitlock
 * <br/>
 * <b>Version:</b> 1.0
 * </summary>
 */
public class InnerBound : MonoBehaviour {
    #region Fields
    /// <summary>Reference to the GameManager.</summary>
    public GameManager gameManager;
    /// <summary>Int that determines the position of the bounding line.</summary>
    private int mult;
    #endregion

    #region Methods
    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start() {
        gameManager = FindObjectOfType<GameManager>();
        LocationInstructions();
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    private void Update() {
        LocationInstructions();
    }

    /// <summary>
    /// Performs the process for setting the location of the bounds.
    /// </summary>
    private void LocationInstructions() {
        Vector3 newPos = gameManager.cam.ScreenToWorldPoint(new Vector3(gameManager.WorldScreenSizeX / 4 * mult, gameManager.cam.pixelHeight / 2, 11));

        gameObject.GetComponent<Transform>().SetPositionAndRotation(newPos, gameObject.GetComponent<Transform>().rotation);
    }

    /// <summary>
    /// Used to set the <c>mult</c> value.
    /// </summary>
    /// <param name="_mult">Value to assign to <c>mult</c>.</param>
    public void SetMult(int _mult) => mult = _mult;
    #endregion
}
