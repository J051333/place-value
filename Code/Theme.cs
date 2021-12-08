using UnityEngine;

/**
 * <summary>
 * Themes are sets of colors used by <see cref="ThemeHandler"/>s to color the scene.
 * <br/>
 * <b>Author:</b> Josi Scarlett Whitlock
 * <br/>
 * <b>Version:</b> 1.0
 * </summary>
 */
[CreateAssetMenu(fileName = "New Theme", menuName
= "2D/Theme")]
public class Theme : ScriptableObject {
    #region Fields
    /// <summary>Name of the theme.</summary>
    public string Name;

    #region Colors
    public Color32 TopPanelColor;
    public Color32 BottomPanelColor;
    public Color32 SumTextColor;
    public Color32 BGColor;
    public Color32 RectColor;
    public Color32 OnesColor;
    public Color32 TensColor;
    public Color32 HundredsColor;
    public Color32 ThousandsColor;
    #endregion
    #endregion
}
