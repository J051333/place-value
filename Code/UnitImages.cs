using UnityEngine;

/**
 * <summary>
 * UnitImages is a <c>ScriptableObject</c> that contains an array of type <c>Sprite</c>.
 * <br/>
 * <b>Author:</b> Josi Scarlett Whitlock
 * <br/>
 * <b>Version:</b> 1.0
 * </summary>
 */
[CreateAssetMenu(fileName = "New UnitImage", menuName
= "2D/Unit Images")]
public class UnitImages : ScriptableObject {
    #region Fields
    /// <summary>Array of sprites.</summary>
    public Sprite[] Sprites;
    #endregion
}
