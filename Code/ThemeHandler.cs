using UnityEngine;
using UnityEngine.UI;

/**
 * <summary>
 * Handles <see cref="Theme"/>s.
 * <br/>
 * <b>Author:</b> Josi Scarlett Whitlock
 * <br/>
 * <b>Version:</b> 1.0
 * </summary>
 */
public class ThemeHandler : MonoBehaviour {
    #region Fields
    /// <summary>Reference to the GameManager.</summary>
    [SerializeField]
    private GameManager gameManager;
    /// <summary>Array of all the available themes.</summary>
    [SerializeField]
    private Theme[] Themes;
    #endregion

    #region Methods
    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start() {
        gameManager = GetComponent<GameManager>();
    }

    /// <summary>
    /// Applies the given theme.
    /// </summary>
    /// <param name="_theme">The theme that should be applied.</param>
    private void SetTheme(Theme _theme) {
        gameManager.topPanel.GetComponent<Image>().color = _theme.TopPanelColor;
        gameManager.bottomPanel.GetComponent<Image>().color = _theme.BottomPanelColor;
        gameManager.sumText.color = _theme.SumTextColor;
        gameManager.cam.backgroundColor = _theme.BGColor;
        gameManager.unitColors[0] = _theme.OnesColor;
        gameManager.unitColors[1] = _theme.TensColor;
        gameManager.unitColors[2] = _theme.HundredsColor;
        gameManager.unitColors[3] = _theme.ThousandsColor;
        gameManager.midPanel.GetRect().GetComponent<Image>().color = _theme.RectColor;

    }

    /// <summary>
    /// Determined which <see cref="Theme"/> has been selected by the user
    /// </summary>
    /// <param name="_value">Number of the <see cref="Theme"/> selected.</param>
    public void ThemeDropdownValueChanged(int _value) {
        switch (_value) {
            case 4:
                UseDark();
                break;
            case 3:
                UseLight();
                break;
            case 2:
                UseTaffy();
                break;
            case 1:
                UseLilac();
                break;
            default:
                UseDefault();
                break;
        }
    }

    /// <summary>
    /// Applies the default theme: Bounce.
    /// </summary>
    public void UseDefault() => SetTheme(Themes[0]);

    /// <summary>
    /// Applies the Lilac theme.
    /// </summary>
    public void UseLilac() => SetTheme(Themes[1]);

    /// <summary>
    /// Applies the Taffy theme.
    /// </summary>
    public void UseTaffy() => SetTheme(Themes[2]);

    /// <summary>
    /// Applies the Light theme.
    /// </summary>
    public void UseLight() => SetTheme(Themes[3]);

    /// <summary>
    /// Applies the Dark theme.
    /// </summary>
    public void UseDark() => SetTheme(Themes[4]);
    #endregion
}
