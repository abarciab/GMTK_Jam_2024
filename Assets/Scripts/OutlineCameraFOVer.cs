using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineCameraFOVer : MonoBehaviour
{
    public Camera mainCam, thisCam;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        thisCam.fieldOfView = mainCam.fieldOfView;
    }
}
