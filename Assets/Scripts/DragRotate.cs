using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragRotate : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler
{
    // This event echoes the new local angle to which we have been dragged
    public event Action<Quaternion> OnAngleChanged;
    public event Action<Quaternion> OnDraggingFinished;
    Quaternion dragStartRotation;
    Quaternion dragStartInverseRotation;

    [SerializeField]
    private RadialFill colorRadialFill;

    private RectTransform rect;
    float currentAngle;
    private void Awake()
    {
        currentAngle = 0f;
        // As an example: rotate the attached object
        OnAngleChanged += rotateAttachedObject;
        OnAngleChanged += SetRadialColors;

        OnDraggingFinished += convertAngle;
    }

    private void Start()
    {
        rect = GetComponent<RectTransform>();
    }
    private void convertAngle(Quaternion rotation)
    {

    }
    // Rotates the attached object when the OnAngleChanged action is triggered
    private void rotateAttachedObject (Quaternion rotation)
    {
        float newRotationAngle, currentRotationAngle = 0f;
        Vector3 newRotationAxis, currentRotationAxis;

        transform.localRotation.ToAngleAxis(out currentRotationAngle, out currentRotationAxis);
        rotation.ToAngleAxis(out newRotationAngle, out newRotationAxis);

        Debug.Log($"<color=#00FF00>Current Rotation Angle: {currentRotationAngle}</color>");
        Debug.Log($"<color=#42BBEB>New Rotation Angle: {newRotationAngle}</color>");

        Debug.Log($"<color=#00FF00>Current Rotation Axis: {currentRotationAxis}</color>");
        Debug.Log($"<color=#42BBEB>New Rotation Axis: {newRotationAxis}</color>");

        var forwardA = transform.localRotation * Vector3.forward;
        var forwardB = rotation * Vector3.forward;

        var angleA = Mathf.Atan2(forwardA.y, forwardA.x) * Mathf.Rad2Deg;
        var angleB = Mathf.Atan2(forwardB.y, forwardB.x) * Mathf.Rad2Deg;

        var angleDiff = Mathf.DeltaAngle(angleB, angleA);

        Debug.Log($"Signed angle diff: <color=#EEEA9B>{angleDiff} </color>");

        // Clamp rotation to be between 0 and 720 degrees
        if (angleDiff > 0)
        {
            if (currentAngle == 0)
            {
                if (currentRotationAxis != Vector3.right)
                {
                    Debug.Log("Current Angle 0 but axis not right");
                    currentAngle += angleDiff;
                }
                else
                {
                    Debug.Log("Current Angle 0 but axis is right");
                    currentAngle = newRotationAngle;
                }
                transform.localRotation = rotation;
            }
            else if (currentAngle + angleDiff <= 720)
            {
                transform.localRotation = rotation;
                currentAngle += angleDiff;
            }

        }
        else if (angleDiff < 0)
        {
            if (currentAngle + angleDiff >= 0)
            {
                transform.localRotation = rotation;
                currentAngle += angleDiff;
            }
        }

        Debug.Log($"Current angle: {currentAngle}");


        //transform.localRotation = rotation;
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

        if (OnDraggingFinished != null)
        {
            //OnDraggingFinished();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 worldPoint;
        if (DragWorldPoint(eventData, out worldPoint) && RectTransformUtility.RectangleContainsScreenPoint(rect, eventData.position, eventData.pressEventCamera))
        {
            Quaternion currentDragAngle = Quaternion.LookRotation(worldPoint - transform.position, Vector3.forward);

            if (OnAngleChanged != null)
            {
                OnAngleChanged(currentDragAngle * dragStartInverseRotation * dragStartRotation); // Combined rotation
            }
        }
        else
        {
            OnEndDrag(eventData);
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
