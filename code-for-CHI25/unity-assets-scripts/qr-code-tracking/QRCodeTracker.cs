using Microsoft.MixedReality.OpenXR;
using Microsoft.MixedReality.QR;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class QRCodeTracker : MonoBehaviour
{
    [SerializeField] private ARMarkerManager markerManager;
    ARMarker frontQR = null;
    // Start is called before the first frame update
    async Task StartAsync()
    {
        await QRCodeWatcher.RequestAccessAsync();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
