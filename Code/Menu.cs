using UnityEngine;

/**
 * <summary>
 * Controls the mini-menu in the main scene.
 * <br/>
 * <b>Author:</b> Josi Scarlett Whitlock
 * <br/>
 * <b>Version:</b> 1.0
 * </summary>
 */
public class Menu : MonoBehaviour {
    #region Fields
    /// <summary>Reference to the menu panel.</summary>
    [SerializeField]
    private GameObject menuPanel;
    #endregion

    #region Methods
    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    void Start() {
        HideMenuPanel();
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (!menuPanel.activeInHierarchy) ShowMenuPanel();
            else HideMenuPanel();
        }
    }

    /// <summary>
    /// Displays the menu panel.
    /// </summary>
    public void ShowMenuPanel() {
        menuPanel.SetActive(true);
    }

    /// <summary>
    /// Hides the menu panel.
    /// </summary>
    public void HideMenuPanel() {
        menuPanel.SetActive(false);
    }
    #endregion
}
