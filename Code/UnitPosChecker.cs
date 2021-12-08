using UnityEngine;
using System.Collections;

/**
 * <summary>
 * <see cref="UnitPosChecker"/>s ensure that after creation, the unit they are attached to stays on the screen.
 * <br/>
 * <b>Author:</b> Josi Scarlett Whitlock
 * <br/>
 * <b>Version:</b> 1.0
 * </summary>
 */
public class UnitPosChecker : MonoBehaviour {
    #region Fields
    /// <summary>Reference to the GameManager.</summary>
    public GameManager Gamemanager { get; set; }
    /// <summary>Has the user passed the unit below the top panel yet?</summary>
    [SerializeField]
    internal bool postCreation = false;
    /// <summary>The unit's position last update.</summary>
    private Vector3 prevPos;
    #endregion

    #region Methods
    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    void Start() {
        prevPos = PosScreenSpace;
        StartCoroutine(CheckPosLoop());
    }

    /// <summary>
    /// Makes sure that once the unit has passed below the top panel,
    /// the unit stays within the bounds of the screen.
    /// </summary>
    private void CheckPos() {
        // Check if the unit has been dragged within the bounds of the screen.
        if (postCreation) {
            // Check if the position has changed since the last update iteration.
            if (prevPos != PosScreenSpace) {
                Vector3 temp = PosScreenSpace;

                if (temp.x < 0) {
                    temp.x += 0 - temp.x;
                } else if (temp.x > Screen.width) {
                    temp.x += Screen.width - temp.x;
                }

                if (temp.y < GetSizeOfRT(Gamemanager.bottomPanel.GetComponent<RectTransform>()).y) {
                    temp.y += GetSizeOfRT(Gamemanager.bottomPanel.GetComponent<RectTransform>()).y - temp.y;
                } else if (temp.y > Screen.height - GetSizeOfRT(Gamemanager.topPanel.GetComponent<RectTransform>()).y) {
                    temp.y += Screen.height - GetSizeOfRT(Gamemanager.topPanel.GetComponent<RectTransform>()).y - temp.y;
                }

                ApplyTemp(temp);
            }
            prevPos = PosScreenSpace;
        } else {
            if (PosScreenSpace.y < Screen.height - GetSizeOfRT(Gamemanager.topPanel.GetComponent<RectTransform>()).y) {
                postCreation = true;
            }
        }
    }

    /// <summary>
    /// Calculates the size in pixels of the given <see cref="RectTransform"/>.
    /// </summary>
    /// <param name="rt">Given <see cref="RectTransform"/>.</param>
    /// <returns>Height and width of the given <see cref="RectTransform"/>.</returns>
    private Vector2 GetSizeOfRT(RectTransform rt) {
        return new Vector2(rt.sizeDelta.x * rt.localScale.x, rt.sizeDelta.y * rt.localScale.y);
    }

    /// <summary>
    /// Dynamic <see cref="Camera"/> that returns <see cref="GameManager.cam"/>.
    /// </summary>
    private Camera Cam => Gamemanager.cam;

    /// <summary>
    /// Dynamic <see cref="Vector3"/> that returns the current position of the unit in screen space.
    /// </summary>
    private Vector3 PosScreenSpace => Cam.WorldToScreenPoint(transform.position);

    /// <summary>
    /// Call once the unit is within the bounds of the screen. Sets <see cref="postCreation"/> to true.
    /// </summary>
    public void Created() => postCreation = true;

    /// <summary>
    /// Applies the given temporary <see cref="Vector3"/>.
    /// </summary>
    /// <param name="temp">New position for the unit.</param>
    private void ApplyTemp(Vector3 temp) {
        transform.position = Cam.ScreenToWorldPoint(temp);
    }
    #endregion

    #region Coroutines
    /// <summary>
    /// Runs <see cref="CheckPos"/> ten times per second.
    /// </summary>
    /// <returns><c>null</c></returns>
    private IEnumerator CheckPosLoop() {
        while (true) {
            CheckPos();
            yield return new WaitForSeconds(0.1f);
        }
    }
    #endregion
}
