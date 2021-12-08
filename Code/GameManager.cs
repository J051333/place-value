using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/**
 * <summary>
 * GameManagers manage the scene and do most abstract/miscellaneous processing.
 * They also hold a lot of references.
 * <br/>
 * <b>Author:</b> Josi Scarlett Whitlock
 * <br/>
 * <b>Version:</b> 1.0
 * </summary>
 */
public class GameManager : MonoBehaviour {
    #region Fields
    // May change to TextMeshPro in the future.
    /// <summary>Displays the sum of all of the units on screen.</summary>
    public Text sumText;
    /// <summary>Public reference to the images used by the units.</summary>
    public UnitImages unitImgs;
    /// <summary>The corners of the screen in sc units.</summary>
    public Vector3 topRight, bottomLeft;
    /// <summary>The size of the bounding lines.</summary>
    public Vector3 boundScale;
    /// <summary>Reference to the main camera in the main scene.</summary>
    public Camera cam;
    // TODO: Change SelectedUnits to be Units not GOs
    /// <summary>Reference to the top panel that contains the unit buttons.</summary>
    public GameObject topPanel;
    /// <summary>Reference to the mid panel that performs all of the selection logic.</summary>
    public MidPanel midPanel;
    /// <summary>Reference to the bottom panel that contains the bottom buttons and the sumText.</summary>
    public GameObject bottomPanel;
    /// <summary>Reference to the prefab of the unitButtons.</summary>
    public GameObject unitButtonPrefab;
    /// <summary>Reference to the prefab of the bounding lines.</summary>
    public GameObject boundPrefab;
    /// <summary>Reference to the prefab of the basic unit.</summary>
    public GameObject unitPrefab;
    /// <summary>Reference to the canvas that holds the UI panels.</summary>
    public GameObject uiCanvas;
    /// <summary>Reference to the pop-up menu that holds miscelaneous controls.</summary>
    public GameObject menu;
    /// <summary>Array that contains references to all of the bounding lines.</summary>
    public GameObject[] bounds;
    /// <summary>Array that contains references to all of the UnitButtons.</summary>
    public GameObject[] unitButtons;
    /// <summary>Length in seconds of the merge animation.</summary>
    public AudioManager audioManager;
    /// <summary>Array containing all of the possible values units should be able to hold.</summary>
    public static readonly int[] PossibleValues = { 1, 10, 100, 1000 };
    /// <summary>List of units currently on the screen.</summary>
    public List<Unit> activeUnits;
    /// <summary>Array containing the colors of the different units. Should correspond to the current theme.</summary>
    public Color32[] unitColors;
    /// <summary>Controls the Z position of new units.</summary>
    public float ZController = 2000;
    #endregion

    #region Properties
    /// <summary>Semi-dynamic list on units considered to be selected.</summary>
    public List<GameObject> SelectedUnits { get; set; }
    /// <summary>Time that merges should take.</summary>
    public float FinishTime { get; set; } = 0.5f;
    #endregion

    #region Methods
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake() {
        unitColors = new Color32[4];
    }

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    void Start() {
        midPanel = FindObjectOfType<MidPanel>();
        cam = FindObjectOfType<Camera>();
        bounds = new GameObject[5];
        topPanel.GetComponent<UIPanel>().level = 't';
        bottomPanel.GetComponent<UIPanel>().level = 'b';
        activeUnits = new List<Unit>();
        SelectedUnits = new List<GameObject>();

        for (int i = 0; i < bounds.Length; i++) {
            bounds[i] = Instantiate(boundPrefab);

            bounds[i].AddComponent<InnerBound>();
            bounds[i].GetComponent<InnerBound>().SetMult(i);
            bounds[i].transform.localScale = boundScale;

            // Hide the outer two edge bounds.
            if (i == 0 || i == bounds.Length - 1) {
                bounds[i].GetComponent<SpriteRenderer>().enabled = false;
            }
        }

        // Unit buttons creation
        unitButtons = new GameObject[4];
        for (int i = 0, j = 1; i < unitButtons.Length; i++, j += 2) {
            unitButtons[i] = Instantiate(unitButtonPrefab, uiCanvas.transform);
            unitButtons[i].AddComponent<UnitButton>();
            unitButtons[i].GetComponent<UnitButton>().SetMult(j);
            unitButtons[i].GetComponent<UnitButton>().Gamemanager = this;
            unitButtons[i].GetComponent<UnitButton>().unitImages = unitImgs;
            unitButtons[i].GetComponent<UnitButton>().unitVal = i;
            // This is to prevent the value from changing before the delegated method gets called.
            // Since this method captures the variable, whatever is stored in the variable at the time
            // of execution is used rather than the value stored when the method was called.
            int a = i;
            unitButtons[i].GetComponent<UnitButton>().createUnit = () => CreateUnitWithButton(a);
        }

        menu.transform.SetAsLastSibling();
        GetComponent<ThemeHandler>().UseDefault();
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    void Update() {
        AddAndUpdateText();
        RefreshBounds();
        topPanel.transform.SetPositionAndRotation(new Vector3(cam.pixelWidth / 2, cam.pixelHeight - topPanel.transform.localScale.y), topPanel.transform.rotation);
    }

    /// <summary>
    /// Creates a unit set to the passed in value.
    /// </summary>
    /// <param name="value">The unit's initial value.</param>
    /// <returns>Returns the created unit.</returns>
    GameObject CreateUnit(int value) {
        GameObject u = Instantiate(unitPrefab);
        u.GetComponent<Unit>().SetValue(value);
        u.GetComponent<Unit>().Gamemanager = this;
        u.GetComponent<UnitPosChecker>().Gamemanager = this;

        u.transform.position = new Vector3(u.transform.position.x, u.transform.position.y, ZController);
        ZController -= 0.01f;

        activeUnits.Add(u.GetComponent<Unit>());

        return u;
    }

    /// <summary>
    /// Used by the <see cref="UnitButton"/>s. Creates a unit and sets its value
    /// with predefined values corresponding to the <c>i</c> found when
    /// creating the <see cref="UnitButton"/>s.
    /// <br/>See also:<br/>
    /// <seealso cref="CreateUnit(int)"/>
    /// </summary>
    /// <param name="setting">UnitButton number to be converted to a value.</param>
    /// <returns>The created unit.</returns>
    GameObject CreateUnitWithButton(int setting) {
        var value = setting switch {
            0 => 1000,
            1 => 100,
            2 => 10,
            3 => 1,
            _ => -1,
        };
        return CreateUnit(value);
    }

    /// <summary>
    /// Drags all units in <see cref="SelectedUnits"/> by the passed in <c>deltaSpace</c>.
    /// </summary>
    /// <param name="deltaSpace">The amount to translate the selected units by.</param>
    /// <param name="cycle">The number of iterations of this drag that have passed.</param>
    /// <param name="selectionRect">The <see cref="RectTransform"/> that represents the rectangle selection.</param>
    /// <param name="selectionRectDeltaSpace">The amount to translate the selection rectangle by.</param>
#nullable enable
    internal void DragSelected(Vector2 deltaSpace, int cycle, RectTransform? selectionRect = null, Vector2? selectionRectDeltaSpace = null) {
        if (cycle != 0) {
            foreach (GameObject u in SelectedUnits) {
                u.transform.Translate(deltaSpace);
            }
            if (selectionRect != null && selectionRectDeltaSpace != null)
                selectionRect.transform.Translate((Vector3)selectionRectDeltaSpace);
        }
    }
#nullable disable

    /// <summary>
    /// Checks to see if the screen size has been changed.
    /// </summary>
    private void RefreshBounds() {
        // TODO: Optimize with checking.
        topRight = new Vector3(cam.pixelWidth, cam.pixelHeight);
        bottomLeft = Vector3.zero;

    }

    /// <summary>
    /// Takes the sum of all living units and returns it to the
    /// text object (<see cref="sumText"/>) representing the current value.
    /// </summary>
    private void AddAndUpdateText() {
        int sum = 0;
        foreach (Unit u in activeUnits) {
            sum += u.GetValue();
        }
        if (sum == 0) ZController = 2000;

        sumText.text = sum.ToString();
    }

    /// <summary>
    /// Regenerates <see cref="SelectedUnits"/>, which contains all units currently enclosed in the rectangle selection.
    /// </summary>
    /// <param name="r"><see cref="RectTransform"/> of the rectangle selection to check against.</param>
    public void CheckUnits(RectTransform r) {
        foreach (Unit u in activeUnits) {
            if (RectTransformUtility.RectangleContainsScreenPoint(r, cam.WorldToScreenPoint(u.gameObject.transform.position)) && !SelectedUnits.Contains(u.gameObject))
                SelectedUnits.Add(u.gameObject);
        }
    }

    /// <summary>
    /// Runs <see cref="MidPanel.CheckUnitsStandAlone()"/> and deletes the units selected by it.
    /// </summary>
    public void DeleteSelected() {
        midPanel.CheckUnitsStandAlone();
        List<GameObject> toRemove = new List<GameObject>();
        foreach (GameObject currentUnit in SelectedUnits) {
            toRemove.Add(currentUnit);
        }
        foreach (GameObject currentUnit in toRemove) {
            currentUnit.GetComponent<Unit>().KillUnit();
        }
    }

    /// <summary>
    /// Merges all groups of ten units into their next level unit.
    /// </summary>
    public void MergeSelected() {
        midPanel.CheckUnitsStandAlone();
        // REMEMBER TO SWAP THE ARRAY FOR A NEW ONE THAT CHECKS VALUE BEFORE MERGING //
        foreach (int o in PossibleValues) {
            List<GameObject> MergeGroup = new List<GameObject>();

            foreach (GameObject currentUnit in SelectedUnits) {
                if (currentUnit.GetComponent<Unit>().GetValue() == o) {
                    MergeGroup.Add(currentUnit);
                }
            }

            for (int i = 0; i < MergeGroup.Count / 10; i++) {
                // Get the Object that is the start of the next ten merging units.
                GameObject parent = MergeGroup[i * 10];
                parent.GetComponent<Unit>().parentalStatus = Unit.UnitStatus.ParentalMerging;
                // Give each unit it
                for (int j = 0; j < 10; j++) {
                    MergeGroup[i * 10 + j].transform.SetParent(parent.transform, true);
                    MergeGroup[i * 10 + j].GetComponent<Unit>().Merge(j);
                }
            }
        }
    }

    /// <summary>
    /// Controls whether the bounds between value columns are being displayed.
    /// </summary>
    /// <param name="_show">Should the borders be displayed?</param>
    public void ShowBounds(bool _show) {
        if (_show) {
            for (int i = 0; i < bounds.Length; i++) {
                bounds[i].GetComponent<SpriteRenderer>().enabled = true;
                if (i == 0 || i == bounds.Length - 1) {
                    bounds[i].GetComponent<SpriteRenderer>().enabled = false;
                }
            }
        } else {
            for (int i = 0; i < bounds.Length; i++) {
                bounds[i].GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }
    
    /// <summary>
    /// Loads the main menu screen.
    /// </summary>
    public void MainMenu() {
        FindObjectOfType<Fades>().FadeOut(0);
    }

    /// <summary>
    /// Starts the <see cref="SplitSelected"/> coroutine.
    /// </summary>
    public void TriggerSplit() {
        StartCoroutine(SplitSelected());
    }

    /// <summary>
    /// Returns what color units of the given value should be.
    /// </summary>
    /// <param name="_value"></param>
    /// <returns></returns>
    public Color32 GetUnitColor(int _value) {
        return _value switch {
            1000 => unitColors[3],
            100 => unitColors[2],
            10 => unitColors[1],
            _ => unitColors[0],
        };
    }

    /// <summary>
    /// Returns the size of the screen in world coordinates.
    /// </summary>
    /// <returns>Size of screen in world coordinates.</returns>
    public float WorldScreenSizeX => topRight.x + bottomLeft.x;

    /// <summary>
    /// Returns the size of the screen in world coordinates.
    /// </summary>
    /// <returns>Size of screen in world coordinates.</returns>
    public float WorldScreenSizeY => topRight.y + bottomLeft.y;
    #endregion

    #region Coroutines
    /// <summary>
    /// Takes all units with values greater than one and splits them into
    /// lesser value units.
    /// </summary>
    private IEnumerator SplitSelected() {
        midPanel.CheckUnitsStandAlone();

        foreach (int currentVal in PossibleValues) {
            List<GameObject> SplitGroup = new List<GameObject>();

            foreach (GameObject currentUnit in SelectedUnits) {
                // We skip 1 because we can't split it any further
                if (currentUnit.GetComponent<Unit>().GetValue() == currentVal && currentVal != 1) {
                    SplitGroup.Add(currentUnit);
                }
            }

            // Loop through each unit nine times to create nine more units and set their
            // Split positions
            foreach (GameObject currentParent in SplitGroup) {
                for (int j = 0; j < 9; j++) {
                    // Create the unit and assign it's value as 10% of the original value
                    GameObject currentUnit = CreateUnit(currentVal / 10);

                    currentUnit.transform.SetParent(currentParent.transform, true);

                    // The + 1 is because we don't instantiate a 0th unit so we skip past it when we're setting locations.
                    Unit currentUnitComponent = currentUnit.GetComponent<Unit>();
                    currentUnitComponent.AssignImage();
                    currentUnitComponent.Split(j + 1);
                }

                // Set the parent to the new value and run it's image assignment method
                Unit currentParentComponent = currentParent.GetComponent<Unit>();
                currentParentComponent.SetValue(currentVal / 10);
                currentParentComponent.parentalStatus = Unit.UnitStatus.ParentalSplitting;
                currentParentComponent.AssignImage(true, true);

                // Unparent all of the new units so they can move independently now that they're in their spots
                currentParent.transform.DetachChildren();
                //yield return new WaitForSecondsRealtime(0.0001f);
            }
        }
        yield return null;
        FindObjectOfType<MidPanel>().CheckUnitsStandAlone();
    }
    #endregion
}
