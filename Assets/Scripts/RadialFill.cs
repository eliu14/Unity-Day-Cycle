using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RadialFill : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler
{
    public Action<float> OnValueChange;

    public Image colorOuterRingImage;
    public Image colorInnerRingImage;
    [SerializeField]
    private DragRotate clockhandRotate;

    float lastRatio;
    // Start is called before the first frame update
    void Start()
    {
        clockhandRotate.OnAngleChanged += ClockHandSetRatio;
    }

    public void ClockHandSetRatio(float rotation)
    {
        float endRatio = (rotation / 360f);

        SetRatio(endRatio);
    }
    public void SetRatio(float ratio, float duration = 0f)
    {
        lastRatio = ratio;
        StartCoroutine(UpdateCircleGraphics(ratio, duration));
        //OnValueChange.Invoke(ratio);
    }
    private IEnumerator UpdateCircleGraphics(float desiredRatio, float animationDuration)
    {
        float currentOuterRatio = colorOuterRingImage.fillAmount;
        float currentInnerRatio = colorInnerRingImage.fillAmount;
        float outerRatio, innerRatio;
        if (desiredRatio > 1.0f)
        {
            outerRatio = 1.0f;
            innerRatio = desiredRatio - 1.0f;
            for (float t = 0; t < animationDuration/2; t += Time.deltaTime)
            {
                colorOuterRingImage.fillAmount = Mathf.Lerp(currentOuterRatio, outerRatio, t / animationDuration);
                yield return null;
            }
            for (float t = 0; t < animationDuration/2; t += Time.deltaTime)
            {
                colorInnerRingImage.fillAmount = Mathf.Lerp(currentInnerRatio, outerRatio, t / animationDuration);
                yield return null;
            }
            colorOuterRingImage.fillAmount = outerRatio;
            colorInnerRingImage.fillAmount = innerRatio;
            
        }
        else
        {
            outerRatio = desiredRatio;
            innerRatio = 0f;
            for (float t = 0; t < animationDuration/2; t += Time.deltaTime)
            {
                colorInnerRingImage.fillAmount = Mathf.Lerp(currentInnerRatio, outerRatio, t / animationDuration);
                yield return null;
            }
            for (float t = 0; t < animationDuration / 2; t += Time.deltaTime)
            {
                colorOuterRingImage.fillAmount = Mathf.Lerp(currentOuterRatio, outerRatio, t / animationDuration);
                yield return null;
            }
            

            colorInnerRingImage.fillAmount = innerRatio;
            colorOuterRingImage.fillAmount = outerRatio;
        }
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Vector2 pos = default(Vector2);
        
        //if(RectTransformUtility.ScreenPointToLocalPointInRectangle(colorOuterRingImage.rectTransform, eventData.position, eventData.pressEventCamera, out pos)) //&& RectTransformUtility.RectangleContainsScreenPoint(clockHand.rectTransform, eventData.position, eventData.pressEventCamera))
        //{
        //    //Debug.Log("On Drag Invoked in RadialFill");
        //    // The position of the pointer is given in pos
        //    // Get the position of the pivots
        //    Vector2 ringPivot = colorOuterRingImage.rectTransform.pivot;

        //    // Calculate the vector from pivot to pointer
        //    Vector2 vectFromRingPivotToPointer = new Vector2(pos.x - ringPivot.x, pos.y - ringPivot.y);
        //    // Normalize the vector
        //    Vector3 angle = new Vector3(vectFromRingPivotToPointer.x, vectFromRingPivotToPointer.y, 0).normalized;

        //    // Assuming radial fill starts from down angle
        //    // Get the quaternion rotation from radial fill start angle to desired angle
        //    Quaternion r = Quaternion.FromToRotation(Vector3.up, angle);

        //    // Calculate fill ratio
        //    float ratio = (360f - (r.eulerAngles.z)) / 360;
            
        //    SetRatio(ratio);
            
        //}
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
       
    }

    private void OnDestroy()
    {
        clockhandRotate.OnAngleChanged -= ClockHandSetRatio;
    }
}
