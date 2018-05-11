using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float _baseSpeed = 10.0f;
    [SerializeField] float _shiftMultiplier = 1.3f;

	void Update()
    {
        Vector3 movement = new Vector3()
        {
            x = Input.GetAxisRaw("Horizontal"),
            z = Input.GetAxisRaw("Vertical")
        };

        if (Input.GetKey(KeyCode.LeftShift))
            movement *= _shiftMultiplier;

        transform.position += movement * _baseSpeed * Time.deltaTime;
    }
}