using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{   
    
    public float panSpeed = 30.0f;
    public float rotateSpeed = 30.0f;
    public float scrollSpeed = 1f;
    public int panMargin = 20;
    public float upperLimit = 420f;
    public float lowerLimit = 90f;

    public bool mousePanEnabled = true;
    Vector3 pos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()

    {
        pos = transform.position;

        if (Input.GetKeyDown(KeyCode.F)){
            mousePanEnabled = !mousePanEnabled;
        }
        
        if (Input.GetKey(KeyCode.W) || ((Input.mousePosition.y > Screen.height - panMargin) && mousePanEnabled)) {
            pos.z += Time.deltaTime * panSpeed * pos.y / lowerLimit;
        }
        if (Input.GetKey(KeyCode.S) || ((Input.mousePosition.y < panMargin) && mousePanEnabled)) {
            pos.z -= Time.deltaTime * panSpeed * pos.y / lowerLimit;
        }
        if (Input.GetKey(KeyCode.A) || ((Input.mousePosition.x < panMargin) && mousePanEnabled)) {
            pos.x -= Time.deltaTime * panSpeed * pos.y / lowerLimit;
        }
        if (Input.GetKey(KeyCode.D) || ((Input.mousePosition.x > Screen.width - panMargin) && mousePanEnabled)) {
            pos.x += Time.deltaTime * panSpeed * pos.y / lowerLimit;
        }
        if (Input.GetKey(KeyCode.Q)) {
            transform.Rotate(Vector3.up * Time.deltaTime * rotateSpeed);
        }
        if (Input.GetKey(KeyCode.E)) {
            transform.Rotate(-Vector3.up * Time.deltaTime * rotateSpeed);
        }

        //transform.Translate(Vector3.up * Input.mouseScrollDelta.y * scrollSpeed);
        pos.y = Mathf.Clamp(transform.position.y + Input.mouseScrollDelta.y * scrollSpeed, lowerLimit, upperLimit);

        transform.position = pos; 
    }
}
