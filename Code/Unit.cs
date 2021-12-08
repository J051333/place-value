using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/**
 * <summary>
 * Units are the objects that represent data and hold/return a value.
 * <br/>
 * <b>Author:</b> Josi Scarlett Whitlock
 * <br/>
 * <b>Version:</b> 1.0
 * </summary>
 */
public class Unit : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler {
    #region Fields
    /// <summary>Reference to the object containing the images for the units.</summary>
    public UnitImages unitImages;
    /// <summary>Value of this unit.</summary>
    private int value;
    /// <summary>Reference to the midpanel.</summary>
    private MidPanel midPanel;
    /// <summary>This unit's status.</summary>
    [SerializeField]
    private UnitStatus status;
    /// <summary>Should this unit be a parent in merge or split, this is its status.</summary>
    internal UnitStatus parentalStatus;
    /// <summary>The position this unit needs to be in to finish merging/splitting.</summary>
    private Vector3 assignedPos;
    /// <summary>Time that has passed while merging.</summary>
    [SerializeField]
    private float elapsedTime = 0;
    /// <summary>This unit's polygon collider.</summary>
    private PolygonCollider2D myPolCol;
    /// <summary>List used by UpdatePolygonCollider2D() to draw the collider.</summary>
    private List<Vector2> points = new List<Vector2>();
    /// <summary>List used by UpdatePolygonCollider2D() to draw the collider.</summary>
    private List<Vector2> simplifiedPoints = new List<Vector2>();
    /// <summary>The position on the last iteration of OnDrag().</summary>
    private Vector3 lastDragPos;
    /// <summary>The iteration counter of OnDrag().</summary>
    private int cycle = 0;
    /// <summary>Should this unit refresh its image?</summary>
    private bool finishedMerge = false;
    #endregion

    #region Properites
    /// <summary>Reference to the GameManager.</summary>
    public GameManager Gamemanager { get; set; }
    /// <summary>This unit's renderer.</summary>
    public SpriteRenderer UnitRenderer { get; private set; }
    #endregion

    #region Methods
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake() {
        midPanel = FindObjectOfType<MidPanel>();
        UnitRenderer = GetComponent<SpriteRenderer>();
        myPolCol = GetComponent<PolygonCollider2D>();
    }

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start() {
        parentalStatus = UnitStatus.Idling;
        status = UnitStatus.Idling;
        SetLDP(Input.mousePosition);

        StartCoroutine(CheckMergeFinish());
        AssignImage();
        CheckColors();
    }

    /// <summary>
    /// Called on every frame update.
    /// </summary>
    private void Update() {
        CheckColors();
        if (status == UnitStatus.Merging) {
            CommenceMerge();
        }

        if (finishedMerge == true) {
            finishedMerge = false;
            midPanel.CheckUnitsStandAlone();
        }
    }

    /// <summary>
    /// Verifies that the current unit color matches the currently applied theme.
    /// If it does not, the color will be shifted to match.
    /// </summary>
    internal void CheckColors() {
        if (!UnitRenderer.color.Equals(Gamemanager.GetUnitColor(value)))
            UnitRenderer.color = Gamemanager.GetUnitColor(value);
    }

    /// <summary>
    /// Checks if the merge has finished and if so, deletes child objects, changes image, and detaches children.
    /// </summary>
    /// <returns><c>null</c></returns>
    private IEnumerator CheckMergeFinish() {
        while (true) {
            // If all child objects (should be 9) are in their spots,
            // they will have added themselves to the list, and this loop will destroy them.
            if (elapsedTime / Gamemanager.FinishTime >= 1.0f) {
                if (transform.childCount >= 9) {
                    // Only runs when merge has completed
                    value *= 10;
                    AssignImage(true);
                    Transform[] transforms = GetComponentsInChildren<Transform>();
                    foreach (Transform child in transforms) {
                        if (transform != child)
                            child.GetComponent<Unit>().KillUnit();
                    }
                    transform.DetachChildren();
                    Gamemanager.audioManager.Merged();
                    finishedMerge = true;
                }
                status = UnitStatus.Idling;
                elapsedTime = 0;
            }

            yield return new WaitForSeconds(0.1f);
        }

    }

    /// <summary>
    /// Checks the unit's current value and ensures the sprite attached to the unit correctly
    /// corresponds to the value. If <c>changeFrom</c> is true, the method will move the 
    /// unit in a way to seem as if the previous units didn't move. If <c>idleNext</c> is true, 
    /// the unit's <see cref="parentalStatus"/> will be reset to <see cref="UnitStatus.Idling"/>.
    /// Must be called before detaching children.
    /// </summary>
    /// <param name="changeFrom">Should the position change with the image?</param>
    /// <param name="idleNext">Should the unit idle after changing images?</param>
    public void AssignImage(bool changeFrom = false, bool idleNext = false) {
        // Store the size of the old sprite for use in location management
        Vector3 oldSpriteSize = Vector3.zero;
        if (UnitRenderer.sprite != null)
            oldSpriteSize = UnitRenderer.sprite.bounds.size;

        // Switch the current value to find the matching sprite to assign.
        GetComponent<SpriteRenderer>().sprite = value switch {
            10 => unitImages.Sprites[1],
            100 => unitImages.Sprites[2],
            1000 => unitImages.Sprites[3],
            _ => unitImages.Sprites[0],
        };

        // Make sure the current collider matches the shape of the sprite.
        UpdatePolygonCollider2D();

        // Check if we're supposed to be altering position.
        // If so, check if we're also not an idling parent. An idling parent does not need to change.
        if (changeFrom && parentalStatus != UnitStatus.Idling) {

            // Are we a new unit that was created through a merge? If so, parentalStatus should be ParentalMerging
            if (parentalStatus == UnitStatus.ParentalMerging) {
                // If the parental unit is merging, we switch the value it merged to
                switch (value) {
                    case 1000:
                        transform.Translate(new Vector3(0.36f, 0.36f));
                        // Debug.Log("Merging hundreds to a thousand.");
                        break;

                    case 100:
                        transform.Translate(new Vector3(-1.37f, 0f));
                        // Debug.Log("Merging tens to a hundred.");
                        break;

                    case 10:
                        transform.Translate(new Vector3(0, (-UnitRenderer.sprite.bounds.size.y + oldSpriteSize.y) / 2f));
                        // Debug.Log("Merging ones to a ten.");
                        break;

                    default:
                        Debug.LogWarning(string.Format("Invalid value {0} for parental merging.", value));
                        break;
                }

                // Or, is this a unit that is the parent to 9 children from a newly split unit?
            } else if (changeFrom && parentalStatus == UnitStatus.ParentalSplitting) {
                // If the parental unit is splitting
                switch (value) {
                    case 100:
                        transform.Translate(new Vector3(-.38f, -0.37f));
                        // Debug.Log("Splitting a thousand to hundreds.");
                        break;

                    case 10:
                        transform.Translate(new Vector3(1.37f, 0));
                        // Debug.Log("Splitting a hundred to tens.");
                        break;

                    case 1:
                        transform.Translate(new Vector3(0, (-UnitRenderer.sprite.bounds.size.y + oldSpriteSize.y) / 2f));
                        // Debug.Log("Splitting a ten to ones.");
                        break;

                    default:
                        Debug.LogWarning(string.Format("Invalid value {0} for parental splitting.", value));
                        break;
                }
            }

            if (idleNext) parentalStatus = UnitStatus.Idling;
        }
    }

    /// <summary>
    /// <b>---DON'T USE---</b>
    /// <br/>
    /// Sets the last drag position to the middle of the screen.
    /// </summary>
    private void ResetLDP() {
        lastDragPos = Gamemanager.cam.WorldToScreenPoint(Vector2.zero);
    }

    /// <summary>
    /// Determines timing and position of the unit and eventually sends the unit to be 
    /// destroyed once the position has been reached.
    /// </summary>
    private void CommenceMerge() {
        // Add the time since the last frame update to the elapsed time of the merge.
        elapsedTime += Time.deltaTime;
        // Interpolate the position it should be at now (linearly).
        if (transform.parent)
            transform.localPosition = Vector3.Lerp(transform.parent.InverseTransformPoint(transform.position), assignedPos, elapsedTime / Gamemanager.FinishTime);
    }

    /// <summary>
    /// Moves the unit to the mouse cursor.
    /// </summary>
    public void Move() {
        Vector3 q = Gamemanager.cam.ScreenToWorldPoint(Input.mousePosition);
        gameObject.transform.SetPositionAndRotation(new Vector3(q.x, q.y, 10), gameObject.transform.rotation);
    }

    /// <summary>
    /// Called each time the pointer moves while a drag is being registered.
    /// </summary>
    /// <param name="eventData">OnDrag event data.</param>
    public void OnDrag(PointerEventData eventData) {
        // There should not be an LDP if this is the first OnDrag iteration.
        if (cycle == 0) {
            SetLDP(Input.mousePosition);
        }

        // Calculate the change in position and move the unit by that much.
        Vector2 deltaSpace = new Vector2();
        Vector2 w_ldp = Gamemanager.cam.ScreenToWorldPoint(lastDragPos);
        Vector2 w_cmp = Gamemanager.cam.ScreenToWorldPoint(Input.mousePosition);
        deltaSpace.x = w_cmp.x - w_ldp.x;
        deltaSpace.y = w_cmp.y - w_ldp.y;

        Gamemanager.DragSelected(deltaSpace, cycle);

        lastDragPos = Input.mousePosition;
        cycle++;
    }

    /// <summary>
    /// This method is called when a drag is registered before the first <see cref="OnDrag(PointerEventData)"/> execution.
    /// </summary>
    /// <param name="eventData">OnBeginDrag event data.</param>
    public void OnBeginDrag(PointerEventData eventData) {
        Gamemanager.SelectedUnits.Add(gameObject);
    }

    /// <summary>
    /// Called when a pointer drag event has stopped registering.
    /// </summary>
    /// <param name="eventData">OnEndDrag event data.</param>
    public void OnEndDrag(PointerEventData eventData) {
        Gamemanager.SelectedUnits.Remove(gameObject);
        cycle = 0;
    }

    /// <summary>
    /// Performs necessary logic to move the unit based on its <see cref="assignedPos"/> in the new unit.
    /// </summary>
    /// <param name="assignment">The position or unit number that the unit must move to to complete its merge.</param>
    internal void Merge(int assignment) {
        if (assignment != 0) {

            assignedPos = GeneratePosition(assignment);
            if (!assignedPos.Equals(new Vector3(-1000, -1000))) {
                status = UnitStatus.Merging;
                assignedPos.z = assignment / 2f;
            }
        } else if (assignment == 0) {
            status = UnitStatus.Merging;
        }
    }

    /// <summary>
    /// Passes the given <c>assignment</c> to <see cref="GeneratePosition(int)"/>.
    /// It assigns the local position to this <see cref="Vector3"/>.
    /// </summary>
    /// <param name="assignment">The number unit this will be when splitting.</param>
    internal void Split(int assignment) {
        assignedPos = GeneratePosition(assignment);

        transform.localPosition = assignedPos;
    }

    /// <summary>
    /// Ensures that the polygon collider shape matches the shape of the sprite.
    /// </summary>
    /// <param name="tolerance">How accurate the shape should be.</param>
    public void UpdatePolygonCollider2D(float tolerance = 0.05f) {
        myPolCol.pathCount = UnitRenderer.sprite.GetPhysicsShapeCount();
        for (int i = 0; i < myPolCol.pathCount; i++) {
            UnitRenderer.sprite.GetPhysicsShape(i, points);
            LineUtility.Simplify(points, tolerance, simplifiedPoints);
            myPolCol.SetPath(i, simplifiedPoints);
        }
    }

    /// <summary>
    /// Returns a position based on this unit's value and the passed in assignment.
    /// </summary>
    /// <param name="assignment">The number in the sorting group this unit is.</param>
    /// <returns>A <see cref="Vector3"/> corresponding to the assignment passed in.</returns>
    public Vector3 GeneratePosition(int assignment) {
        // Hard coded values because of the weird shape of the units.
        // TODO: Finish posgen for 1000.
        return value switch {
            // 1000 => Nothing because there is no 10000
            100 => new Vector3(0.1f * assignment, 0.1f * assignment, 0.001f * assignment),
            10 => new Vector3(-0.305f * assignment, 0, 0.001f * assignment),
            _ => new Vector3(0, -0.3f * assignment, 0.001f * assignment),
        };
    }

    /// <summary>
    /// Destroys this unit and removes it from all lists.
    /// </summary>
    public void KillUnit() {
        Gamemanager.activeUnits.Remove(this);
        Gamemanager.SelectedUnits.Remove(gameObject);
        Destroy(gameObject);
    }

    /// <summary>
    /// Reassigns the value of this unit instance to the specified number.
    /// </summary>
    /// <param name="_value">New unit value.</param>
    public void SetValue(int _value) => value = _value;

    /// <summary>
    /// Returns the current unit value.
    /// </summary>
    /// <returns>Current unit value.</returns>
    public int GetValue() => value;

    /// <summary>
    /// Sets the <see cref="lastDragPos"/> to the provided <see cref="Vector3"/>.
    /// </summary>
    /// <param name="_ldp">New value for <see cref="lastDragPos"/></param>
    public void SetLDP(Vector3 _ldp) => lastDragPos = _ldp;

    /// <summary>
    /// Resets the <see cref="cycle"/> of this unit to 0.
    /// </summary>
    public void ResetCycle() => cycle = 0;
    #endregion

    #region Enumerations
    /// <summary>
    /// Possible unit statuses.
    /// </summary>
    public enum UnitStatus {
        Idling,
        Merging,
        Splitting,
        ParentalMerging,
        ParentalSplitting,
    }
    #endregion
}
