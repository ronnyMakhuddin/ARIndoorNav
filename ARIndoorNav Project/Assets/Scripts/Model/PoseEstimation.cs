﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoseEstimation : MonoBehaviour
{
    public Transform _ARCoreOriginTransform;
    public Transform _ARCoreFPSPosition;
    public MarkerDetection _MarkerDetection;
    public NavigationPresenter _NavigationPresenter;

    public Transform _TestWorldMarker;
    public Transform _TestVirtualMarker;
    public bool _CalculateRotation = true;
    public bool _CalculatePosition = true;

    private string lastWorldMarkerPos, lastVirtualMarkerPos, lastWorldMarkerRot, lastVirtualMarkerRot;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //TestUpdateLastMarkerPosition();

        // Check if 
        _NavigationPresenter.DisplayPositionInformation(
            "pos: " + lastWorldMarkerPos + " rot: " + lastWorldMarkerRot,
            "pos: " + lastVirtualMarkerPos + " rot: " + lastVirtualMarkerRot,
            "pos: " + _ARCoreOriginTransform.position.ToString() + " rot: " + _ARCoreOriginTransform.rotation.eulerAngles.ToString(),
            "pos: " + _ARCoreFPSPosition.position.ToString() + " rot: " + _ARCoreFPSPosition.rotation.eulerAngles.ToString());
    }

    /**
        Method to gather positional data to combine it and calculate the most likely user position
        ARCore is constantly updating the position in the background 
        This method is used to counteract drift by scanning marker
     */
    public void ReportMarkerPose(Transform virtualMarkerPosition, Pose worldMarkerPose)
    {
        _MarkerDetection.StopDetection();
        UpdateLastMarkerPosition(virtualMarkerPosition, worldMarkerPose);
        UpdateLastMarkerRotation(virtualMarkerPosition, worldMarkerPose);
    }

    public void ReportMarkerRotation(Transform virtualMarkerPosition, Pose worldMarkerPose)
    {
        _MarkerDetection.StopDetection();
        //UpdateLastMarkerRotation(virtualMarkerPosition, worldMarkerPose);
    }

    /**
        This method is called when the accumulated tracking error exceed a limit
        Tracking will be stopped and a user prompt initiated that asks the user to scan a marker
     */
    public void RequestNewPosition()
    {
        _MarkerDetection.StartDetection();
    }

    private void UpdateLastMarkerPosition(Transform virtualMarkerTransform, Pose worldMarkerPose)
    {
        lastWorldMarkerPos = worldMarkerPose.position.ToString();
        lastVirtualMarkerPos = virtualMarkerTransform.position.ToString();
        lastWorldMarkerRot = worldMarkerPose.rotation.eulerAngles.ToString();
        lastVirtualMarkerRot = virtualMarkerTransform.rotation.eulerAngles.ToString();

        var virtualMarkerPosition = virtualMarkerTransform.position;

        var worldMarkerPosition = worldMarkerPose.position; // - _LastMarkerTransform.position;

        var originPosition = _ARCoreOriginTransform.position;

        Vector3 targetPosDelta;
        targetPosDelta = virtualMarkerPosition - _ARCoreOriginTransform.position - worldMarkerPosition;
        _ARCoreOriginTransform.position += targetPosDelta;
    }

    private void UpdateLastMarkerRotation(Transform virtualMarkerTransform, Pose worldMarkerPose)
    {
        var originRotation = _ARCoreOriginTransform.rotation;
        var worldMarkerRotation = worldMarkerPose.rotation;
        var virtualMarkerRotation = virtualMarkerTransform.rotation;
        Quaternion targetRotation;

        targetRotation = virtualMarkerRotation * Quaternion.Inverse(_ARCoreOriginTransform.rotation) * Quaternion.Inverse(worldMarkerRotation);
        _ARCoreOriginTransform.rotation *= targetRotation;
    }

    private void UpdateLastMarkerRotationEuler(Transform virtualMarkerTransform, Pose worldMarkerPose)
    {
        var originRotation = _ARCoreOriginTransform.rotation;
        var worldMarkerRotation = worldMarkerPose.rotation;
        var virtualMarkerRotation = virtualMarkerTransform.rotation;


        var targetRotX = 0f; // virtualMarkerRotation.eulerAngles.x - _ARCoreOriginTransform.rotation.eulerAngles.x - worldMarkerRotation.eulerAngles.x + _ARCoreOriginTransform.rotation.eulerAngles.x;
        var targetRotY = 0f; //virtualMarkerRotation.eulerAngles.y - originRotation.eulerAngles.y - worldMarkerRotation.eulerAngles.y + originRotation.eulerAngles.y;
        targetRotY = originRotation.eulerAngles.y - virtualMarkerRotation.eulerAngles.y - worldMarkerRotation.eulerAngles.y + 180;

        var targetRotZ = 0f;// virtualMarkerRotation.eulerAngles.z - _ARCoreOriginTransform.rotation.eulerAngles.z - worldMarkerRotation.eulerAngles.z + _ARCoreOriginTransform.rotation.z;

        if (targetRotX < 0) targetRotX += 360;
        if (targetRotX >= 360) targetRotX -= 360;
        if (targetRotY < 0) targetRotX += 360;
        if (targetRotY >= 360) targetRotX -= 360;
        if (targetRotZ < 0) targetRotX += 360;
        if (targetRotZ >= 360) targetRotX -= 360;

        Quaternion targetRotDelta;
        targetRotDelta = Quaternion.Euler(targetRotX, targetRotY, targetRotZ);
        _ARCoreOriginTransform.rotation = targetRotDelta;
    }


}



