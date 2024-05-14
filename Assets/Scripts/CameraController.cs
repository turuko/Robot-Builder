using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    public float cameraMoveSpeed = 10f;
    public float cameraRotationSpeed = 3f;

    private float rotX;
    private float rotY;
    
    public Texture2D cursorTexture;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance != null)
        {
            Debug.LogError("Two Camera Controllers?");
        }

        Instance = this;
        
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * (cameraMoveSpeed * Time.fixedDeltaTime));
        }
        
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(-Vector3.forward * (cameraMoveSpeed * Time.fixedDeltaTime));
        }
        
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * (cameraMoveSpeed * Time.fixedDeltaTime));
        }
        
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(-Vector3.right * (cameraMoveSpeed * Time.fixedDeltaTime));
        }
        
        if (Input.GetKey(KeyCode.Space))
        {
            transform.Translate(Vector3.up * (cameraMoveSpeed * Time.fixedDeltaTime), Space.World);
        }
        
        if (Input.GetKey(KeyCode.LeftShift))
        {
            transform.Translate(Vector3.down * (cameraMoveSpeed * Time.fixedDeltaTime), Space.World);
        }

        transform.eulerAngles = new Vector3(-rotX, transform.eulerAngles.y + rotY, 0);
    }

    public void GetMousePos(float x, float y)
    {
        rotY = x * cameraRotationSpeed * Time.fixedDeltaTime;
        rotX += y * cameraRotationSpeed * Time.fixedDeltaTime;
        rotX = Mathf.Clamp(rotX, -90f, 90f);
    }
}
