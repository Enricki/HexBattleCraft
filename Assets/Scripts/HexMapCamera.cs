using UnityEngine;
using UnityEngine.EventSystems;


public class HexMapCamera : MonoBehaviour
{
    public float stickMinZoom, stickMaxZoom;
    public float swivelMinZoom, swivelMaxZoom;
    public float moveSpeedMinZoom, moveSpeedMaxZoom;
    public float rotationSpeed;
    public HexGrid grid;


    float rotationAngle;
    float zoom = 1;
    float delta;

    public static bool MovedCamera, MultyTouch, TouchedUI, BlockedCam = false;

    Transform swivel, stick;
    Vector3 startTPos;
    Vector3 targetTPos;

    static HexMapCamera instance;
    void Awake()
    {
        instance = this;
        startTPos = transform.position;
        swivel = transform.GetChild(0);
        stick = swivel.GetChild(0);

    }
    void Update()
    {


        float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
        if (zoomDelta != 0f)
        {
            AdjustZoom(zoomDelta);
        }


        float rotationDelta = Input.GetAxis("Rotation");
        if (rotationDelta != 0f)
        {
            AdjustRotation(rotationDelta);
        }

        float xDelta = Input.GetAxis("Horizontal");
        float zDelta = Input.GetAxis("Vertical");
        if (xDelta != 0f || zDelta != 0f)
        {
            moveSpeedMinZoom = 400f;
            moveSpeedMaxZoom = 100f;
            AdjustPosition(xDelta, zDelta);
        }
        if (Input.touchCount > 0)
        {
            if (Input.touchCount <= 1)
            {
                MultyTouch = false;
            }

            if (Input.touchCount > 1)
            {
                MultyTouch = true;
            }
        }
    }
    private void LateUpdate()
    {
        if (Input.touchCount > 0)
        {
            if (Input.touchCount <= 1)
            {
                TouchMove();
            }

            if (Input.touchCount > 1)
            {
                TouchZoom();
            }
        }
    }

    private void FixedUpdate()
    {
        if (needImpulse)
        {
            needImpulse = false;
        }
    }

    private void OnEnable()
    {
        instance = this;
        ValidatePosition();
    }

    public static bool Locked
    {
        set
        {
            instance.enabled = !value;
        }
    }
    public static Vector3 ValidPosition;
    public static void ValidatePosition()
    {
        instance.transform.position = new Vector3(ValidPosition.x,
            0, ValidPosition.z);
        //        instance.AdjustPosition(ValidPosition.x, ValidPosition.z);
    }

    void AdjustZoom(float delta)
    {

        zoom = Mathf.Clamp01(zoom + delta);

        float distance = Mathf.Lerp(stickMinZoom, stickMaxZoom, zoom);
        stick.localPosition = new Vector3(0f, 0f, distance);

        float angle = Mathf.Lerp(swivelMinZoom, swivelMaxZoom, zoom);
        swivel.localRotation = Quaternion.Euler(angle, 0f, 0f);
    }

    void AdjustPosition(float xDelta, float zDelta)
    {
        MovedCamera = true;
        Vector3 direction = new Vector3(xDelta, 0f, zDelta).normalized;
        float damping = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta));
        float distance =
            Mathf.Lerp(moveSpeedMinZoom, moveSpeedMaxZoom, zoom)
            * damping * Time.deltaTime;
        Vector3 position = transform.localPosition;
        position += -direction * distance;
        transform.localPosition =
            grid.wrapping ? WrapPosition(position) : ClampPosition(position);
    }

    Vector3 ClampPosition(Vector3 position)
    {
        float xMax =
            (grid.cellCountX - 0.5f) * HexMetrics.innerDiameter;
        position.x = Mathf.Clamp(position.x, 0f, xMax);

        float zMax =
            (grid.cellCountZ * HexMetrics.chunkSizeZ - 1) *
            (2f * HexMetrics.outerRadius);
        position.z = Mathf.Clamp(position.z, 0f, zMax);

        return position;
    }

    void AdjustRotation(float delta)
    {
        MovedCamera = true;
        rotationAngle += delta * rotationSpeed * Time.deltaTime;
        if (rotationAngle < 0f)
        {
            rotationAngle += 360f;
        }
        else if (rotationAngle >= 360f)
        {
            rotationAngle -= 360f;
        }
        transform.localRotation = Quaternion.Euler(0f, rotationAngle, 0f);
    }


    Vector3 WrapPosition(Vector3 position)
    {
        float width = grid.cellCountX * HexMetrics.innerDiameter;
        while (position.x < 0f)
        {
            position.x += width;
        }
        while (position.x > width)
        {
            position.x -= width;
        }
        float zMax = (grid.cellCountZ - 1) * (1.5f * HexMetrics.outerRadius);
        position.z = Mathf.Clamp(position.z, 0f, zMax);

        grid.CenterMap(position.x);
        return position;
    }


    public void MultyTouchDetect()
    {
        //checkTouches();
        //Vector2 firstTouch = new Vector2(0, 0);
        //Vector2 secondTouch = new Vector2(0, 0);
        //Vector2 startPosition = new Vector2(0, 0);
        //Vector2 lastPosition = new Vector2(0, 0);
        //if (Input.touchCount == 1)
        //{
        //    firstTouch = Input.GetTouch(0).position;
        //}
        //if (Input.touchCount == 2)
        //{
        //    MultyTouch = true;

        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        secondTouch = Input.GetTouch(1).position;
        //        startPosition = secondTouch - firstTouch;
        //    }

        //    lastPosition = secondTouch - firstTouch;

        //    if (lastPosition.x > startPosition.x || lastPosition.y > startPosition.y)
        //    {
        //        delta = 0.002f;
        //    }
        //    else if (lastPosition.x < startPosition.x || lastPosition.y < startPosition.y)
        //    {
        //        delta = -0.002f;
        //    }
        //    else
        //    {
        //        delta = 0;
        //    }

        //    delta = -0.002f;
        //}
        ////else if (Input.touchCount == 3)
        ////{
        ////    delta = -0.002f;
        ////    MultyTouch = true;
        ////}
        //else
        //{
        //    MultyTouch = false;
        //    delta = 0;
        //}
    }


    void checkTouchUI()
    {
        if (Input.touchCount > 0)
        {
            // Check if finger is over a UI element
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                TouchedUI = true;
            }
            else TouchedUI = false;
        }

    }

    //void TouchMove()
    //{
    //    moveSpeedMinZoom = 0.8f;
    //    moveSpeedMaxZoom = 0.2f;

    //    if (BlockedCam)
    //    {
    //        //      Locked = true;
    //        targetTPos = startTPos;
    //    }
    //    else
    //    {
    //        //      Locked = false;
    //        if (Input.GetMouseButtonDown(0))
    //        {
    //            //                startTPos = new Vector3(Input.GetTouch(0).position.x, 0, Input.GetTouch(0).position.y);
    //            startTPos = Camera.main.ScreenToWorldPoint(
    //                     new Vector3(Input.mousePosition.x, Input.mousePosition.y,
    //                     Camera.main.farClipPlane));
    //        }
    //        targetTPos = Camera.main.ScreenToWorldPoint
    //            (new Vector3(Input.mousePosition.x, Input.mousePosition.y,
    //            Camera.main.farClipPlane));
    //    }
    //    if (targetTPos != startTPos)
    //    {

    //        targetTPos.z -= startTPos.z;
    //        targetTPos.x -= startTPos.x;

    //        AdjustPosition(targetTPos.x, targetTPos.z);
    //    }
    //}

    Vector2 startTPos1;
    Vector2 startTPos2;
    Vector2 sumVector;
    bool needImpulse;

    void TouchMove()
    {
        moveSpeedMinZoom = 400f;
        moveSpeedMaxZoom = 100f;

        if (BlockedCam)
        {
            sumVector = startTPos;
        }
        else
        {
            if (Input.touches[0].phase == TouchPhase.Began)
            {
                startTPos1 = Input.touches[0].position;
            }
            if (Input.touches[0].phase == TouchPhase.Ended)
            {
                sumVector = Input.touches[0].position - startTPos1;
                needImpulse = true;
                MovedCamera = false;
            }
            if (Input.touches[0].phase == TouchPhase.Moved)
            {
                sumVector = (Input.touches[0].position - startTPos1).normalized;
                AdjustPosition(sumVector.x, sumVector.y);
            }
            if (Input.touches[0].phase == TouchPhase.Stationary)
            {
                sumVector = (Input.touches[0].position - startTPos1).normalized;
                AdjustPosition(sumVector.x, sumVector.y);
            }
            
        }
    }
    void TouchZoom()
    {
        if (Input.touches[1].phase == TouchPhase.Began)
        {
            startTPos1 = Input.touches[0].position;
            startTPos2 = Input.touches[1].position;
            if (startTPos1.x > startTPos2.x)
            {
                sumVector.x = startTPos1.x - startTPos2.x;
            }
            else if (startTPos2.x > startTPos1.x)
            {
                sumVector.x = startTPos2.x - startTPos1.x;
            }
        }
        if (Input.touches[0].phase == TouchPhase.Moved && Input.touches[1].phase == TouchPhase.Moved)
        {
            Vector2 deltaVector = sumVector;
            Vector2 deltaPos1 = Input.touches[0].position;
            Vector2 deltaPos2 = Input.touches[1].position;
            if (deltaPos1.x > deltaPos2.x)
            {
                deltaVector.x = deltaPos1.x - deltaPos2.x;
            }
            else if (deltaPos2.x > deltaPos1.x)
            {
                deltaVector.x = deltaPos2.x - deltaPos1.x;
            }

            if (deltaVector.x > sumVector.x)
            {
                delta = 0.008f;
            }
            if (deltaVector.x < sumVector.x)
            {
                delta = -0.008f;
            }
            AdjustZoom(delta);
        }
        if (Input.touches[0].phase == TouchPhase.Stationary && Input.touches[1].phase == TouchPhase.Stationary)
        {
            Vector2 deltaVector = sumVector;
            Vector2 deltaPos1 = Input.touches[0].position;
            Vector2 deltaPos2 = Input.touches[1].position;
            if (deltaPos1.x > deltaPos2.x)
            {
                deltaVector.x = deltaPos1.x - deltaPos2.x;
            }
            else if (deltaPos2.x > deltaPos1.x)
            {
                deltaVector.x = deltaPos2.x - deltaPos1.x;
            }

            if (deltaVector.x > sumVector.x)
            {
                delta = 0.008f;
            }
            if (deltaVector.x < sumVector.x)
            {
                delta = -0.008f;
            }
            AdjustZoom(delta);
        }
        if (Input.touches[0].phase == TouchPhase.Ended && Input.touches[1].phase == TouchPhase.Ended)
        {
            MovedCamera = false;
        }
    }
}
