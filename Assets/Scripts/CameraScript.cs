using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public float maxZoom = 300f,
        minZoom = 150f,
        panSpeed = 6f;

    Vector3 bottomLeft, topRight;
    float cameraMaxX, cameraMaxY, cameraMinX, cameraMinY, x, y;
    public Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
        topRight = cam.ScreenToWorldPoint(
            new Vector3(cam.pixelWidth, cam.pixelHeight, -transform.position.z));
        bottomLeft = cam.ScreenToWorldPoint(
            new Vector3(0, 0, -transform.position.z));
        cameraMaxX = topRight.x;
        cameraMinX = bottomLeft.x;
        cameraMaxY = topRight.y;
        cameraMinY = bottomLeft.y;
    }

    void Update()
    {
        x = Input.GetAxis("Mouse X") * panSpeed;
        y = Input.GetAxis("Mouse Y") * panSpeed;
        transform.Translate(x, y, 0);

        if ((Input.GetAxis("Mouse ScrollWheel") > 0) && cam.orthographicSize > minZoom)
        {
            cam.orthographicSize = cam.orthographicSize - 50f;
        }

        if ((Input.GetAxis("Mouse ScrollWheel") < 0) && cam.orthographicSize < maxZoom)
        {
            cam.orthographicSize = cam.orthographicSize + 50f;
        }
    }
}
