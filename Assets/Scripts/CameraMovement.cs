using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] GameObject player;

    float defaultZ = 0f;

    void Start()
    {
        defaultZ = transform.position.z;
    }

    void Update()
    {
        Vector3 position = player.transform.position + Vector3.up * 2f;
        position.z = defaultZ;
        position.y = Mathf.Max(0f, position.y);
        transform.position = position;
    }
}
