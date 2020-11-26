using UnityEngine;


public class MoveCamera : MonoBehaviour
{
    [SerializeField]
    private static float speed = 0.25f;

    private Vector3 startPos;
    private Vector3 targetPos;

    private void Start()
    {
        startPos = transform.position;
    }
    private void Update()
    {
        CameraMove();
        CameraScroll();
    }
    private void CameraMove()
    {
        if (Input.GetMouseButtonDown(1))
        {
            startPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane));
        }
        if (Input.GetMouseButton(1))
        {
            targetPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane));
            if (targetPos != startPos)
            {
                targetPos -= startPos;
                transform.position = new Vector3(Mathf.Lerp(transform.position.x, -targetPos.x, speed * Time.deltaTime), transform.position.y, Mathf.Lerp(transform.position.z, -targetPos.z, speed * Time.deltaTime));
            }
        }
    }

    private void CameraScroll()
    {
        float mw = Input.GetAxis("Mouse ScrollWheel");
        if (mw > 0.1)
        {
            transform.position += transform.forward * Time.deltaTime * 100;/*Приближение*/
        }
        if (mw < -0.1)
        {
            transform.position -= transform.forward * Time.deltaTime * 100;/*Отдаление*/
        }
    }
    

}
