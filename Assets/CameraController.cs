using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public float panSpeed = 100.0f;
    public float rotateSpeed = 100.0f;
    public float scrollSpeed = 30f;
    public int panMargin = 10;
    public float upperLimit = 420f;
    public float lowerLimit = 90f;

    public bool mousePanEnabled = true;

    Vector3 trackPos;
    Vector3 pos;
    Transform camTarget;
    CamMover camMover;

    private void Awake()
    {
        camTarget = transform.parent;
        camMover = GetComponentInParent<CamMover>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()

    {
        pos = transform.position;
        trackPos = camTarget.position;


        if (Input.GetKeyDown(KeyCode.F))
        {
            mousePanEnabled = !mousePanEnabled;
        }


        if (Input.GetKey(KeyCode.W) || ((Input.mousePosition.y > Screen.height - panMargin) && mousePanEnabled))
        {
            trackPos.z += Time.deltaTime * panSpeed * pos.y / lowerLimit;
            camMover.Stop();
        }
        if (Input.GetKey(KeyCode.S) || ((Input.mousePosition.y < panMargin) && mousePanEnabled))
        {
            trackPos.z -= Time.deltaTime * panSpeed * pos.y / lowerLimit;
            camMover.Stop();

        }
        if (Input.GetKey(KeyCode.A) || ((Input.mousePosition.x < panMargin) && mousePanEnabled))
        {
            trackPos.x -= Time.deltaTime * panSpeed * pos.y / lowerLimit;
            camMover.Stop();
        }
        if (Input.GetKey(KeyCode.D) || ((Input.mousePosition.x > Screen.width - panMargin) && mousePanEnabled))
        {
            trackPos.x += Time.deltaTime * panSpeed * pos.y / lowerLimit;
            camMover.Stop();
        }


        if (Input.GetKey(KeyCode.Q))
        {
            camTarget.Rotate(-Vector3.up * Time.deltaTime * rotateSpeed, Space.World);
        }
        if (Input.GetKey(KeyCode.E))
        {
            camTarget.Rotate(Vector3.up * Time.deltaTime * rotateSpeed, Space.World);
        }





        //transform.Translate(Vector3.up * Input.mouseScrollDelta.y * scrollSpeed);
        pos.y = Mathf.Clamp(transform.position.y + Input.mouseScrollDelta.y * scrollSpeed, lowerLimit, upperLimit);

        transform.position = pos;


        camTarget.position = trackPos;


        if (Input.GetKey(KeyCode.G))
        {
            camMover.MoveTo(Vector3.zero);
        }
    }
}
