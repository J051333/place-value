using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/**
 * <summary>
 * Panel where all unit handling happens.
 * <br/>
 * <b>Author:</b> Josi Scarlett Whitlock
 * <br/>
 * <b>Version:</b> 1.0
 * </summary>
 */
public class MidPanel : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler {
    #region Fields
    /// <summary>Reference to the main camera in the scene.</summary>
    [SerializeField]
    private Camera cam;
    /// <summary>Reference to the selection rectangle's RectTransform.</summary>
    [SerializeField]
    RectTransform selectionRect;
    /// <summary>Panels that might overlap the midpanel that should not be considered the midpanel.</summary>
    [SerializeField]
    private Transform[] avoidPanels;
    /// <summary>Where the mouse is in screen space at the beginning of a drag.</summary>
    private Vector2 startMousePosSS;
    /// <summary>Where the mouse was on the last iteration of OnDrag().</summary>
    private Vector2 lastDragPos;
    /// <summary>Hits representing the objects below the cursor.</summary>
    private RaycastHit2D[] hits;
    /// <summary>Should the MidPanel being dragging the selected units?</summary>
    private bool dragSelected;
    /// <summary>Should the MidPanel be dragging a single unit?</summary>
    private bool dragSingleUnit;
    /// <summary>Is the MidPanel currently in a drag event?</summary>
    private bool isDraggingUI = false;
    /// <summary>What iteration of OnDrag() are we on?</summary>
    private int cycle = 0;
    /// <summary>The buttons that should be affected by the selection rectangle's activity.</summary>
    [SerializeField]
    private GameObject[] bottomButtons;
    #endregion

    #region Properties
    /// <summary>Reference to the game manager.</summary>
    private GameManager Gamemanager { get; set; }
    #endregion

    #region Methods
    /// <summary>
    /// Called before the first frame update.
    /// </summary>
    public void Start() {
        cam = FindObjectOfType<Camera>();
        dragSelected = false;
        dragSingleUnit = false;
        Gamemanager = FindObjectOfType<GameManager>();

        // Make sure the selection rectangle is hidden.
        DisableSelectionRect();
    }

    /// <summary>
    /// Called once per frame.
    /// </summary>
    private void Update() {
        // If the selection rectangle is on the screen, show the bottomButtons.
        if (selectionRect.position.x > 0 && selectionRect.position.y > 0) {
            foreach (GameObject o in bottomButtons) {
                o.SetActive(true);
            }
        }
    }

    /// <summary>
    /// This method is called when a drag is registered before the first <see cref="OnDrag(PointerEventData)"/> execution.
    /// </summary>
    /// <param name="eventData">OnBeginDrag event data.</param>
    public void OnBeginDrag(PointerEventData eventData) {
        isDraggingUI = true;
        startMousePosSS = Input.mousePosition;

        // If the position clicked is in the rectangle, drag the selected units.
        if (RectTransformUtility.RectangleContainsScreenPoint(selectionRect, startMousePosSS)) {
            dragSelected = true;
        }

        // Refresh the objects in hits.
        hits = Physics2D.RaycastAll(new Vector2(cam.ScreenToWorldPoint(Input.mousePosition).x, cam.ScreenToWorldPoint(Input.mousePosition).y), Vector2.zero);

        // Touching a panel to avoid?
        bool touchingNonEmpty = false;
        // Single unit to drag
        GameObject single = null;
        foreach (RaycastHit2D r in hits) {

            foreach (Transform panel in avoidPanels) {
                if (r.transform.Equals(panel)) {
                    touchingNonEmpty = true;
                    CheckUnitsStandAlone();
                }
            }
            foreach (Unit u in Gamemanager.activeUnits) {
                if (r.collider.gameObject.Equals(u.gameObject) && !Gamemanager.SelectedUnits.Contains(u.gameObject)) {
                    if (single == null || u.transform.position.z < single.transform.position.z) {
                        single = u.gameObject;
                    }
                    dragSingleUnit = true;
                }
            }
        }
        if (touchingNonEmpty || dragSelected) {
            if (dragSelected) CheckUnitsStandAlone();
            return;
        }

        // Only called when the rectangle selected units are not being dragged.
        if (dragSingleUnit) {
            // We are adding the unit to the list here to prevent unnecessary writing to it.
            Gamemanager.SelectedUnits.Clear();
            Gamemanager.SelectedUnits.Add(single);
            return;
        }


        // Anything past this point will only get called if the selection rectangle is being redrawn.

        selectionRect.position = Vector3.zero;
        selectionRect.sizeDelta = Vector2.zero;
        ResetLDP();
    }

    /// <summary>
    /// Called each time the pointer moves while a drag is being registered.
    /// </summary>
    /// <param name="eventData">OnDrag event data.</param>
    public void OnDrag(PointerEventData eventData) {
        if (dragSelected) {
            DragSelectedUnits();
        } else if (dragSingleUnit) {
            Gamemanager.SelectedUnits[0].GetComponent<Unit>().OnDrag(eventData);
        } else {
            DrawRect(eventData);
        }

        lastDragPos = Input.mousePosition;
        cycle++;
    }

    /// <summary>
    /// Called when a pointer drag event has stopped registering.
    /// </summary>
    /// <param name="eventData">OnDrag event data.</param>
    public void OnEndDrag(PointerEventData eventData) {
        //FindObjectOfType<GameManager>().CheckUnits(selectionRect);

        if (dragSingleUnit) {
            Gamemanager.SelectedUnits[0].GetComponent<Unit>().ResetCycle();
        }
        if (selectionRect.rect.width < 50 || selectionRect.rect.height < 50) {
            DisableSelectionRect();
        }

        List<GameObject> rem = new List<GameObject>();
        foreach (GameObject u in Gamemanager.SelectedUnits) {
            // Check if the rectangle selection no longer contains any units and removes them.
            if (!RectTransformUtility.RectangleContainsScreenPoint(selectionRect, cam.WorldToScreenPoint(u.gameObject.transform.position))) {
                rem.Add(u);
            }
        }
        foreach (GameObject u in rem) {
            Gamemanager.SelectedUnits.Remove(u);
        }
        CheckUnitsStandAlone();

        // Reset for next drag.
        ResetLDP();
        dragSelected = false;
        dragSingleUnit = false;
        cycle = 0;
        isDraggingUI = false;
    }

    /// <summary>
    /// No-params version of <see cref="GameManager.CheckUnits(RectTransform)"/>. Uses this <see cref="MidPanel"/>'s <see cref="selectionRect"/>.
    /// </summary>
    public void CheckUnitsStandAlone() {
        Gamemanager.CheckUnits(selectionRect);
    }

    /// <summary>
    /// Resets <c>lastDragPos</c> to the screen space equivalent of <see cref="Vector2.zero"/>.
    /// </summary>
    private void ResetLDP() {
        lastDragPos = cam.WorldToScreenPoint(Vector2.zero);
    }

    /// <summary>
    /// Drags the units in <see cref="GameManager.SelectedUnits"/> a distance calculated through current
    /// and previous mouse positions.
    /// </summary>
    private void DragSelectedUnits() {
        Vector2 deltaSpace = new Vector2();
        Vector2 w_ldp = cam.ScreenToWorldPoint(lastDragPos);
        Vector2 w_cmp = cam.ScreenToWorldPoint(Input.mousePosition);
        deltaSpace.x = w_cmp.x - w_ldp.x;
        deltaSpace.y = w_cmp.y - w_ldp.y;

        Vector2 srds = new Vector2();
        srds.x = Input.mousePosition.x - lastDragPos.x;
        srds.y = Input.mousePosition.y - lastDragPos.y;

        Gamemanager.DragSelected(deltaSpace, cycle, selectionRect, srds);
    }

    /// <summary>
    /// Redraws the selection rectangle based on the saved <c>startMousePositionSS</c> 
    /// from the <see cref="OnBeginDrag(PointerEventData)"/> method. This is then used in conjuction with 
    /// the position from the <see cref="PointerEventData"/> passed from <see cref="OnDrag(PointerEventData)"/>.
    /// </summary>
    /// <param name="eventData">Data regarding the <c>OnDrag()</c> event.</param>
    private void DrawRect(PointerEventData eventData) {
        float width = eventData.position.x - startMousePosSS.x;
        float height = eventData.position.y - startMousePosSS.y;

        selectionRect.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        selectionRect.position = startMousePosSS + new Vector2(width / 2, height / 2);
    }

    /// <summary>
    /// Called when the close selection button is pressed.
    /// Resets the rectangle and hides the buttons pertaining to the selection.
    /// Clears the selection.
    /// </summary>
    public void DisableSelectionRect() {
        selectionRect.position = new Vector3(-1000, -1000);
        foreach (GameObject o in bottomButtons) {
            o.SetActive(false);
        }
        // Empty the SelectedUnits.
        Gamemanager.SelectedUnits.Clear();
    }

    /// <summary>
    /// Called when the midpanel is clicked. Hides the <see cref="selectionRect"/> if not clicking on it or a unit.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData) {
        // If we didn't click the selection rectangle
        if (!RectTransformUtility.RectangleContainsScreenPoint(selectionRect, eventData.position) && !isDraggingUI) {
            //Debug.Log("asd");
            DisableSelectionRect();
        }
    }

    /// <summary>
    /// Returns the <see cref="selectionRect"/>'s <see cref="GameObject"/>.
    /// </summary>
    /// <returns>The <see cref="GameObject"/> that the <see cref="selectionRect"/> belongs to.</returns>
    public GameObject GetRect() => selectionRect.gameObject;
    #endregion
}
