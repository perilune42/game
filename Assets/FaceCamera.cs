using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    [SerializeField] Transform camTransform;
    public Vector3 offset = Vector2.up * 50;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //transform.rotation = Quaternion.LookRotation(transform.position - camTransform.position);
        transform.position = Camera.main.WorldToScreenPoint(transform.parent.parent.position) + offset;
    }
}
