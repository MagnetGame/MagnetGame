using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTransform : MonoBehaviour
{
    public GameObject targetObject;
    public float yOffset = 0f; // Offset to the Y Axis
    private Vector3 initialPositionRelativeToTarger;
    // Start is called before the first frame update
    private Quaternion initialRotation;

    void Start()
    {
        if(targetObject == null){
            targetObject = this.gameObject;
            Debug.Log("Default target target not specified");
        }

        initialPositionRelativeToTarger = transform.position - targetObject.transform.position;
        initialPositionRelativeToTarger.y += yOffset;

        initialRotation = transform.rotation;
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = initialPositionRelativeToTarger + targetObject.transform.position;
        transform.LookAt(targetObject.transform);

        if (Input.GetKey(KeyCode.R)) //if press r we reset table position
        {
            ResetTableRotation();
        }
    }

    public void ResetTableRotation()
    {
        transform.rotation = initialRotation;
    }
}
