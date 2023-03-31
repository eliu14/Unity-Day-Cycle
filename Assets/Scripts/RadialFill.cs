using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RadialFill : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    public Action<float> OnValueChange;
    public Image img;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetRatio(float ratio, float duration = 0f)
    {
        StartCoroutine(UpdateCircleGraphics(img.fillAmount, ratio, duration));
        OnValueChange.Invoke(ratio);
    }
    private IEnumerator UpdateCircleGraphics(float currentRatio, float desiredRatio, float animationDuration)
    {
        for (float t = 0; t < animationDuration; t+= Time.deltaTime)
        {
            img.fillAmount = Mathf.Lerp(currentRatio, desiredRatio, t / animationDuration);
            yield return null;
        }

        img.fillAmount = desiredRatio;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("On Drag Invoked");
        Vector2 pos = default(Vector2);
        
        if(RectTransformUtility.ScreenPointToLocalPointInRectangle(img.rectTransform, eventData.position, eventData.pressEventCamera, out pos))
        {
            // The position of the pointer is given in pos
            // Get the position of the pivot
            Vector2 pivot = img.rectTransform.pivot;

            // Calculate the vector from pivot to pointer
            Vector2 vectFromPivotToPointer = new Vector3(pos.x - pivot.x, pos.y - pivot.y);
            // Normalize the vector
            Vector3 angle = new Vector3(vectFromPivotToPointer.x, vectFromPivotToPointer.y, 0).normalized;

            // Assuming radial fill starts from down angle
            // Get the quaternion rotation from radial fill start angle to desired angle
            Quaternion r = Quaternion.FromToRotation(Vector3.down, angle);

            // Calculate fill ratio
            float ratio = (360f - (r.eulerAngles.z)) / 360;
            
            SetRatio(ratio);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    
}
