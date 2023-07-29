using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragRotate : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler
{
    // This event echoes the new local angle to which we have been dragged
    public event Action<float> OnAngleChanged;
    public event Action<float> OnActualAngleChanged;
    Quaternion dragStartRotation;
    Quaternion dragStartInverseRotation;

    private RectTransform rect;
    public float currentAngle;

    private Quaternion currentOriginQuat;
    private void Awake()
    {
        currentAngle = 0f;
    }

    private void Start()
    {
        rect = GetComponent<RectTransform>();
    }
    public void rotateAttachedObjectByAngle(float newAngle)
    {
        Quaternion quatFromEulerAngle = Quaternion.Euler(new Vector3(0, 0, -newAngle));

        transform.localRotation = quatFromEulerAngle;
        currentOriginQuat = quatFromEulerAngle;
        currentAngle = 0f;
    }
    private void rotateAttachedObject(Quaternion rotation)
    {
        float newRotationAngle, localRotationAngle = 0f;
        Vector3 newRotationAxis, localRotationAxis;

        transform.localRotation.ToAngleAxis(out localRotationAngle, out localRotationAxis); // Current rotation
        rotation.ToAngleAxis(out newRotationAngle, out newRotationAxis);
        //Debug.Log($"Current Angle: {currentAngle}");
        //Debug.Log($"<color=#00FF00>Local Rotation Angle: {localRotationAngle}</color>");
        //Debug.Log($"<color=#42BBEB>New Rotation Angle: {newRotationAngle}</color>");

        //Debug.Log($"<color=#00FF00>Local Rotation Axis: {localRotationAxis}</color>");
        //Debug.Log($"<color=#42BBEB>New Rotation Axis: {newRotationAxis}</color>");

        var unsignedDiff = Quaternion.Angle(transform.localRotation, rotation);
        //Debug.Log($"Unsigned diff: {unsignedDiff}");

        var newAngle = newRotationAngle * newRotationAxis.z;
        var localAngle = localRotationAngle * localRotationAxis.z;
        if (newAngle > 0) 
            newAngle = 360 - newAngle;
        else 
            newAngle = -newAngle;
        if (localAngle > 0)
            localAngle = 360 - localAngle;
        else
            localAngle = -localAngle;

        var manualDiff = (newAngle - localAngle);
        //Debug.Log($"Angle Manual diff: {manualDiff}");
        var signedDiff = manualDiff >= 0 ? unsignedDiff : -unsignedDiff;
        if (signedDiff > 0)
        {
            if ((currentAngle + signedDiff) > 720)
            {
                if (currentAngle == 720f)
                    return;

                transform.localRotation = currentOriginQuat;
                currentAngle = 720f;
            }
            else
            {
                transform.localRotation = rotation;
                currentAngle += signedDiff;
            }   
        }

        else if (signedDiff < 0)
        {
            if ((currentAngle + signedDiff) < 0)
            {
                if (currentAngle == 0f)
                    return;

                transform.localRotation = currentOriginQuat;
                currentAngle = 0f;
            }
            else
            {
                transform.localRotation = rotation;
                currentAngle += signedDiff;
            }
        }

        if (OnAngleChanged != null)
        {
            OnAngleChanged(currentAngle);
        }
        if (OnActualAngleChanged != null)
        {
            OnActualAngleChanged(newAngle);
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

        
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 worldPoint;
        if (DragWorldPoint(eventData, out worldPoint) )//&& RectTransformUtility.RectangleContainsScreenPoint(rect, eventData.position, eventData.pressEventCamera))
        {
            //Debug Stuff
            float dragStartInverseAngle;
            Vector3 dragStartInverseAxis;
            dragStartInverseRotation.ToAngleAxis(out dragStartInverseAngle, out dragStartInverseAxis);
            //Debug.Log($"<color=#6CBACB>Drag Start Inverse Angle: {dragStartInverseAngle}</color>");
            //Debug.Log($"<color=#6CBACB>Drag Start Inverse Axis: {dragStartInverseAxis}</color>");

            Quaternion currentDragAngle = Quaternion.LookRotation(worldPoint - transform.position, Vector3.forward);

            //Debug Stuff
            float currentDrag;
            Vector3 currentDragAxis;
            currentDragAngle.ToAngleAxis(out currentDrag, out currentDragAxis);
            //Debug.Log($"<color=#6CBACB>Current Drag Angle: {currentDrag}</color>");
            //Debug.Log($"<color=#6CBACB>Current Drag Axis: {currentDragAxis}</color>");

            Quaternion newRotation = currentDragAngle * dragStartInverseRotation * dragStartRotation;

            //Debug Stuff
            float newAngle;
            Vector3 newAxis;
            newRotation.ToAngleAxis(out newAngle, out newAxis);
            //Debug.Log($"<color=#6CBACB>New Rotation Angle: {newAngle}</color>");
            //Debug.Log($"<color=#6CBACB>New Rotation Axis: {newAxis}</color>");

            rotateAttachedObject(newRotation); // Combined rotation
            
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
