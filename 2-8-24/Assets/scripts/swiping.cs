using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class swiping : MonoBehaviour
{
    public Touch _touch = new Touch();
    public GameObject camcontroller;
    public GameObject grid;
    private float movx = 0f;
    private float movy = 0f;
    private Vector3 originalpos;

    public float movspeed=0f;
    private void Start()
    {
        originalpos = camcontroller.transform.position;
        movx = originalpos.x;
        movy = originalpos.y;
    }
    private void FixedUpdate()
    {
        foreach(Touch touch in Input.touches)
        {
            if(touch.phase== TouchPhase.Began)
            {
                _touch = touch;
            }
            else if(touch.phase == TouchPhase.Moved)
            {
                float x = _touch.position.x - touch.position.x;
                _touch = touch;
                    if(x < 0)
                    {
                            camcontroller.transform.RotateAround(grid.transform.position,Vector3.up,Mathf.Abs(x*movspeed));
                    }
                    else if (x > 0)
                    {
                              camcontroller.transform.RotateAround(grid.transform.position,Vector3.down,Mathf.Abs(x*movspeed));
                    }
            }
            else if(touch.phase == TouchPhase.Ended)
            {
                _touch = new Touch();
            }
        }
    }
}
