using Cinemachine;
using UnityEngine;

public class Swiping : MonoBehaviour
{
    public GameObject camcontroller;
    public GameObject grid;
    public float movspeed = 1.5f;
    private Vector2 previousTouchPosition;
    public bool isSwiping;
    public Tile_Manager tile_Manager;
    private void Start()
    {
        if (camcontroller == null || grid == null)
        {
            Debug.LogError("Camcontroller or Grid is not assigned.");
        }
    }
    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Ended)
            {
                //ending the swiping event
                Debug.Log("in touch end");
                isSwiping = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!tile_Manager.onGrid)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    Debug.Log("in touch begin");
                    previousTouchPosition = touch.position;
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    //beginning the swiping
                    Debug.Log("in touch move");
                    isSwiping = true;
                    Vector2 touchDeltaPosition = touch.deltaPosition;

                    float x = touchDeltaPosition.x;
                    if (x > 0)
                    {
                        camcontroller.transform.RotateAround(grid.transform.position, Vector3.up, Mathf.Abs(x * movspeed));
                    }
                    else if (x < 0)
                    {
                        camcontroller.transform.RotateAround(grid.transform.position, Vector3.down, Mathf.Abs(x * movspeed));
                    }

                    previousTouchPosition = touch.position;
                }
            }
        }
    }
}
