using UnityEngine;

using MixedReality.Toolkit.UX;
using UnityEngine.XR.Interaction.Toolkit;

public class UpdateCameraFarClipPlane : MonoBehaviour
{
    public Slider slider; // Reference to the MRTK slider

    void Start()
    {
        if (slider != null)
        {
            slider.OnValueUpdated.AddListener(UpdateFarClipPlane);
        }
        else
        {
            Debug.LogError("Slider is not assigned.");
        }
    }

    void UpdateFarClipPlane(SliderEventData eventData)
    {
        Camera.main.farClipPlane = eventData.NewValue;
        Debug.Log("Updated far clipping plane to: " + eventData.NewValue);
    }

    void OnDestroy()
    {
        if (slider != null)
        {
            slider.OnValueUpdated.RemoveListener(UpdateFarClipPlane);
        }
    }
}