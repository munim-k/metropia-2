using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    private Tile_Manager tile;
    public bool IsOnGrid;
    void Start()
    {
        tile = FindObjectOfType<Tile_Manager>();
    }
    void FixedUpdate()
    {
        // if (!IsOnGrid)
        // {
        //     transform.position = tile.smoothmouseposition + new Vector3(0, 0.5f, 0);
        // }
    }
}
