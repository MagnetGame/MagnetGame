using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTransform : MonoBehaviour
{
    public GameObject targetObject;
    public float yOffset = 0f; // Offset to the Y Axis
    private Vector3 initialPositionRelativeToTarget;
    private Vector3 offSet;

    void Start()
    {
        if (targetObject == null)
        {
            targetObject = this.gameObject;
            Debug.Log("Default target not specified");
        }

        initialPositionRelativeToTarget = transform.position - targetObject.transform.position;
        offSet = new Vector3(0, yOffset, 0);
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = initialPositionRelativeToTarget + targetObject.transform.position;
        transform.LookAt(targetObject.transform);
    }
}