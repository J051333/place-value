using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

/**
 * <summary>
 * UnitButtons are the different buttons that instantiate new units.
 * <br/>
 * <b>Author:</b> Josi Scarlett Whitlock
 * <br/>
 * <b>Version:</b> 1.0
 * </summary>
 */
public class UnitButton : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {
    #region Fields
    /// <summary>
    /// Contains a method that returns a gameobject.
    /// </summary>
    /// <returns>A <see cref="GameObject"/></returns>
    public delegate GameObject Del();
    /// <summary>Contains the images for the units.</summary>
    public UnitImages unitImages;
    /// <summary>Used to determine where the UnitButton should be positioned.</summary>
    public int mult;
    /// <summary>The value of unit that this button creates.</summary>
    public int unitVal = -1;
    /// <summary>
    /// Contains the method to be run when this <see cref="UnitButton"/> is pressed.
    /// </summary>
    /// <returns>The created unit.</returns>
    public Del createUnit;
    /// <summary>The image this button should display.</summary>
    public Image buttonImage;
    /// <summary>The latest unit this button has created.</summary>
    private Unit newUnit;
    #endregion

    #region Properties
    /// <summary>Reference to the GameManager.</summary>
    public GameManager Gamemanager { get; set; }
    #endregion

    #region Methods
    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    void Start() {
        buttonImage = GetComponent<Image>();
        LocationInstructions();
        unitVal = unitVal switch {
            0 => 1000,
            1 => 100,
            2 => 10,
            3 => 1,
            _ => -1,
        };
        GetComponent<Image>().sprite = unitVal switch {
            10 => unitImages.Sprites[1 + 4],
            100 => unitImages.Sprites[2 + 4],
            1000 => unitImages.Sprites[3 + 4],
            _ => unitImages.Sprites[0 + 4],
        };
        GetComponentInChildren<Text>().text = unitVal switch {
            10 => "Tens",
            100 => "Hundreds",
            1000 => "Thousands",
            _ => "Ones",
        };
        CheckColors();
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    void Update() {
        LocationInstructions();
        CheckColors();
    }

    /// <summary>
    /// Called each time the pointer moves while a drag is being registered.
    /// </summary>
    /// <param name="eventData">OnDrag event data.</param>
    public void OnDrag(PointerEventData eventData) {
        newUnit.OnDrag(eventData);
    }

    /// <summary>
    /// Called when a pointer drag event has stopped registering.
    /// </summary>
    /// <param name="eventData">OnEndDrag event data.</param>
    public void OnEndDrag(PointerEventData eventData) {
        newUnit.OnEndDrag(eventData);
        if (!newUnit.GetComponent<UnitPosChecker>().postCreation) { newUnit.KillUnit(); }
        newUnit = null;
    }

    /// <summary>
    /// This method is called when a drag is registered before the first <see cref="OnDrag(PointerEventData)"/> execution.
    /// </summary>
    /// <param name="eventData">OnBeginDrag event data.</param>
    public void OnBeginDrag(PointerEventData eventData) {
        if (createUnit != null) {
            Gamemanager.SelectedUnits.Clear();
            Gamemanager.midPanel.DisableSelectionRect();
            GameObject nu = createUnit();
            Gamemanager.SelectedUnits.Add(nu);
            newUnit = nu.GetComponent<Unit>();
            newUnit.SetLDP(Input.mousePosition);
            Vector2 newWorldPoint = (Vector2) Gamemanager.cam.ScreenToWorldPoint(gameObject.transform.position);
            newUnit.transform.position = new Vector3(newWorldPoint.x, newWorldPoint.y, nu.transform.position.z);
        }
    }

    /// <summary>
    /// Used to set the distance the button should be from the leftmost edge.
    /// The integer <see cref="mult"/> should be calculated with the formula 
    /// <c>{<see cref="Screen.width"/> / 8f * <see cref="mult"/>}</c>. This means that mult evaluates to
    /// the position in eighths of the screen width.
    /// </summary>
    /// <param name="_mult">New <c>mult</c> value.</param>
    public void SetMult(int _mult) {
        mult = _mult;
    }

    /// <summary>
    /// Performs the process of setting the location of this button using <see cref="mult"/>.
    /// </summary>
    void LocationInstructions() {
        gameObject.GetComponent<Transform>().SetPositionAndRotation(new Vector3(Screen.width / 8f * mult, FindObjectOfType<GameManager>().GetComponent<GameManager>().topPanel.GetComponent<Transform>().position.y, 0), gameObject.GetComponent<Transform>().rotation);
    }

    /// <summary>
    /// Makes sure that the color of the button matches the theme.
    /// </summary>
    internal void CheckColors() {
        if (!buttonImage.color.Equals(Gamemanager.GetUnitColor(unitVal)))
            buttonImage.color = Gamemanager.GetUnitColor(unitVal);
    }
    #endregion
}
