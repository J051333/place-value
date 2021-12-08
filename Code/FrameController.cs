using UnityEngine;

/**
 * <summary>
 * FrameController controls the target frames per second.
 * The target frames per second is held in the float constant <c>FramesPerSecond</c>.
 * <br/>
 * <b>Author:</b> Josi Scarlett Whitlock
 * <br/>
 * <b>Version:</b> 1.0
 * </summary>
 */
public class FrameController : MonoBehaviour {
    #region Fields
    /// <summary>Target FPS. 0 indicates no frame cap.</summary>
    public const float FramesPerSecond = 0;
    #endregion

    #region Methods
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake() {
        // Disable vSync.
        QualitySettings.vSyncCount = 0;
        // Apply the frames per second.
        Application.targetFrameRate = (int) FramesPerSecond;
    }
    #endregion
}
