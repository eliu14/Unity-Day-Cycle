using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragRotate : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler
{
    // This event echoes the new local angle to which we have been dragged
    public event Action<Quaternion> OnAngleChanged;

    Quaternion dragStartRotation;
    Quaternion dragStartInverseRotation;

    [SerializeField]
    private RadialFill colorRadialFill;

    private void Awake()
    {
        // As an example: rotate the attached object
        OnAngleChanged += rotateAttachedObject;
        OnAngleChanged += SetRadialColors;
    }

    // Rotates the attached object when the OnAngleChanged action is triggered
    private void rotateAttachedObject (Quaternion rotation)
    {
        transform.localRotation = rotation;
    }
    
    private void SetRadialColors(Quaternion rotation)
    {
        colorRadialFill.ClockHandSetRatio(dragStartRotation, rotation);
    }
    // This detects the starting point of the drag more accurately than OnBeginDrag,
    // because OnBeginDrag won't fire until the mouse has moved from the point of mousedown
    public void OnPointerDown(PointerEventData eventData)
    {
        // Gets the current local rotation (Quanternion) of the object
        dragStartRotation = transform.localRotation;

        // the world point of the Pointer 
        Vector3 worldPoint;
        if (DragWorldPoint(eventData, out worldPoint))
        {
            // We use Vector3.forward as the "up" vector because we assume we're working in a Canvas
            // and so mostly care about rotation around the Z axis
            dragStartInverseRotation = Quaternion.Inverse(Quaternion.LookRotation(worldPoint - transform.position, Vector3.forward));
        }
        else
        {
            Debug.LogWarning("Couldn't get drag start world point");
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Do nothing (but this has to exist or OnDrag won't work)
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        // Do nothing (but this has to exist or OnDrag won't work)
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 worldPoint;
        if (DragWorldPoint(eventData, out worldPoint))
        {
            Quaternion currentDragAngle = Quaternion.LookRotation(worldPoint - transform.position, Vector3.forward);
            //Debug.Log("currentDragAngle: " + currentDragAngle.eulerAngles);
            if (OnAngleChanged != null)
            {
                OnAngleChanged(currentDragAngle * dragStartInverseRotation * dragStartRotation); // Combined rotation
            }
        }
    }


    // Gets the point in worldspace corresponding to where the mouse is
    private bool DragWorldPoint(PointerEventData eventData, out Vector3 worldPoint)
    {
        return RectTransformUtility.ScreenPointToWorldPointInRectangle(
            GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera,
            out worldPoint);
    }

    private void OnDestroy()
    {
        OnAngleChanged -= rotateAttachedObject;
    }
}
