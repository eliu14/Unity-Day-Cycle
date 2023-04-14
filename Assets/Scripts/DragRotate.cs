using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragRotate : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler
{
    // This event echoes the new local angle to which we have been dragged
    public event Action<float> OnAngleChanged;
    public event Action<Quaternion> OnDraggingFinished;
    Quaternion dragStartRotation;
    Quaternion dragStartInverseRotation;

    private RectTransform rect;
    float currentAngle;

    private void Awake()
    {
        currentAngle = 0f;
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
    private void rotateAttachedObject(Quaternion rotation)
    {
        float newRotationAngle, currentRotationAngle = 0f;
        Vector3 newRotationAxis, currentRotationAxis;

        transform.localRotation.ToAngleAxis(out currentRotationAngle, out currentRotationAxis);
        rotation.ToAngleAxis(out newRotationAngle, out newRotationAxis);

        //Debug.Log($"<color=#00FF00>Current Rotation Angle: {currentRotationAngle}</color>");
        //Debug.Log($"<color=#42BBEB>New Rotation Angle: {newRotationAngle}</color>");

        //Debug.Log($"<color=#00FF00>Current Rotation Axis: {currentRotationAxis}</color>");
        //Debug.Log($"<color=#42BBEB>New Rotation Axis: {newRotationAxis}</color>");

        var forwardA = transform.localRotation * Vector3.forward;
        var forwardB = rotation * Vector3.forward;
        //Debug.Log($"Current forward: <color=#00FF00>{forwardA} </color>");
        //Debug.Log($"New forward: <color=#42BBEB>{forwardB} </color>");

        var angleA = Mathf.Atan2(forwardA.y, forwardA.x) * Mathf.Rad2Deg;
        var angleB = Mathf.Atan2(forwardB.y, forwardB.x) * Mathf.Rad2Deg;

        var angleDiff = Mathf.DeltaAngle(angleB, angleA);
        //Debug.Log($"Current Angle: <color=#00FF00>{angleA} </color>");
        //Debug.Log($"New Angle: <color=#42BBEB>{angleB} </color>");
        //Debug.Log($"DeltaAngle: <color=#EEEA9B>{angleDiff} </color>");
        //Debug.Log($"Angle Manual diff: {angleB - angleA}");
        
        // The axis of the transform starts off as (1.00, 0, 0) usually
        if (currentRotationAxis.z == 0)
        {
            if ((newRotationAngle * newRotationAxis.z) > 0)
            {
                return;
            }
            transform.localRotation = rotation;
            currentAngle = (newRotationAngle * newRotationAxis.z) < 0 ? -newRotationAngle : newRotationAngle;
        }

        // Clamp rotation to be between 0 and 720 degrees
        if (currentAngle == 0)
        {
            if ((newRotationAngle * newRotationAxis.z) > 0)
            {
                return;
            }
        }

        if (currentAngle == 720)
        {
            if ((newRotationAngle * newRotationAxis.z) < 0)
            {
                return;
            }
        }

        if (angleDiff > 0)
        {
            if ((currentAngle + angleDiff) > 720)
            {
                if (currentAngle == 720)
                    return;

                Quaternion endClampAngle = Quaternion.LookRotation((transform.position + 10* Vector3.up) -transform.position, Vector3.forward);
                Quaternion customRotation = endClampAngle * dragStartInverseRotation * dragStartRotation; // Combined rotation
                transform.localRotation = customRotation;
                currentAngle = 720f;
            }
            else
            {
                transform.localRotation = rotation;
                currentAngle += angleDiff;
            }   
        }

        else if (angleDiff < 0)
        { 
            if ((currentAngle + angleDiff) < 0)
            {
                if (currentAngle == 0)
                    return;
                
                Quaternion begginningClampAngle = Quaternion.LookRotation((transform.position + 10 * Vector3.up) - transform.position, Vector3.forward);
                Quaternion customRotation = begginningClampAngle * dragStartInverseRotation * dragStartRotation; // Combined rotation
                transform.localRotation = customRotation;
                currentAngle = 0f;
            }
            else
            {
                transform.localRotation = rotation;
                currentAngle += angleDiff;
            }
        }

        //Debug.Log($"Current angle: {currentAngle}");

        // Correct currentAngle according to the actual angle of the transform
        // 0-270 comes out to be an angle between 0 and 270 in the negative Z axis
        // 270-360 comes out to be an angle between 0 and 90 in the positive Z axis
        if (currentAngle != 0 && currentAngle != 720)
        {
            float actualAngle;
            Vector3 actualAxis;
            transform.localRotation.ToAngleAxis(out actualAngle, out actualAxis);

            //Debug.Log($"<color=#FFFF00>Actual Rotation Axis: {actualAxis}</color>");
            //Debug.Log($"<color=#FFFF00>Actual Rotation Angle: {actualAngle}</color>");

            float effectiveActualAngle = actualAxis.z * actualAngle;
            if (effectiveActualAngle < 0)
            {
                if (currentAngle > 360)
                {
                    currentAngle = (-effectiveActualAngle) + 360;

                }
                else
                {
                    currentAngle = (-effectiveActualAngle);
                }
            }
            else if (effectiveActualAngle > 0)
            {
                if (currentAngle > 360)
                {
                    currentAngle = 720 - effectiveActualAngle;

                }
                else
                {
                    currentAngle = 360 - effectiveActualAngle;
                }
            }
            //Debug.Log($"Current angle after correction: {currentAngle}");
        }
        if (OnAngleChanged != null)
        {
            OnAngleChanged(currentAngle);
        }
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
        if (DragWorldPoint(eventData, out worldPoint) )//&& RectTransformUtility.RectangleContainsScreenPoint(rect, eventData.position, eventData.pressEventCamera))
        {
            Quaternion currentDragAngle = Quaternion.LookRotation(worldPoint - transform.position, Vector3.forward);

            rotateAttachedObject(currentDragAngle * dragStartInverseRotation * dragStartRotation); // Combined rotation
            
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

}
