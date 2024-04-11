using System;
using UnityEngine;

public class Tile_Manager : MonoBehaviour
{
    [SerializeField] public GameObject tiles;
    [SerializeField] public Transform parent;
    public int height, width;
    private Node[,] nodes;
    public LayerMask ortoplanelayermask;
    private Plane plane;
    public Transform cube;
    public Transform onMouseprefab;
    public GameObject cube_placement;
    private Vector3 mousePosition;
    public Vector3 tilemouseposition;
    private Vector2[,] arrayyy =
    {
        {new Vector2(4,0),new Vector2(3,0),new Vector2(2,0),new Vector2(1,0),new Vector2(0,0)},
        {new Vector2(4,-1),new Vector2(3,-1),new Vector2(2,-1),new Vector2(1,-1),new Vector2(0,-1)},
        {new Vector2(4,-2),new Vector2(3,-2),new Vector2(2,-2),new Vector2(1,-2),new Vector2(0,-2)},
        {new Vector2(4,-3),new Vector2(3,-3),new Vector2(2,-3),new Vector2(1,-3),new Vector2(0,-3)},
        {new Vector2(4,-4),new Vector2(3,-4),new Vector2(2,-4),new Vector2(1,-4),new Vector2(0,-4)}
    };

    public float xoffset;
    public float yoffset;
    private Vector2 returnindex(Vector2 tile_index)
    {
        if (tile_index.x < 0 || tile_index.y < 0 || tile_index.x > 4 || tile_index.y > 4)
        {
            return new Vector2(-1, -1);
        }
        return arrayyy[(int)(tile_index.x), (int)(tile_index.y)];
    }

    void getMousePositionOnGrid()
    {
        if (Input.GetButtonDown("Fire1") || Input.GetButton("Fire1"))
        {
            Touch touch = Input.touches[0];
            mousePosition = touch.position;
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, float.MaxValue, ortoplanelayermask))
            {
                tilemouseposition = hit.transform.position;
                tilemouseposition.x = Mathf.RoundToInt(tilemouseposition.x);
                tilemouseposition.y = Mathf.RoundToInt(tilemouseposition.y);
                tilemouseposition.z = Mathf.RoundToInt(tilemouseposition.z);

                foreach (var node in nodes)
                {
                    Vector2 cell_index = new Vector2(node.cellposition.x, node.cellposition.y);

                    Vector2 tile_index = returnindex(cell_index);
                    // Debug.Log(tilemouseposition);
                    //  Debug.Log(cell_index);
                    //Debug.Log(tile_index);

                    if (tile_index.x == -1 && tile_index.y == -1)
                    {
                        break;
                    }
                    else if (tilemouseposition.x == tile_index.x && tilemouseposition.z == tile_index.y && node.isplaceable)
                    {
                        // Debug.Log("match");
                        if (onMouseprefab)
                        {
                            node.block_level = 1;
                           // Debug.Log("up");
                            match_checker(node);
                            node.isplaceable = false;
                            onMouseprefab.GetComponent<FollowMouse>().IsOnGrid = true;
                            onMouseprefab.position = new Vector3(tile_index.x + xoffset, 0.84f, tile_index.y + yoffset);
                            onMouseprefab = null;

                        }
                    }
                }
            }
        }


    }

    bool match_checker(Node block)
    {
        int n = check_match(block, 1);
        Debug.Log("N value: " + n);
        foreach (var node in nodes)
        {
            node.traversed = false;
        }
        return n > 3;
    }

    int check_match(Node block, int n)
    {
        block.traversed = true;
        if (block.x < 4 && nodes[block.x + 1, block.y].traversed == false &&
            block.block_level == nodes[block.x + 1, block.y].block_level) //right
        {Debug.Log("right");
           // Debug.Log(n);
            check_match(nodes[block.x + 1, block.y], n + 1);
            
        }
        if (block.x > 0 && nodes[block.x - 1, block.y].traversed == false && block.block_level == nodes[block.x - 1, block.y].block_level) //left
        {
            Debug.Log("left");
          //  Debug.Log(n);
            check_match(nodes[block.x - 1, block.y], n + 1);
        }
        
        if (block.y < 4 && nodes[block.x, block.y + 1].traversed == false &&
            block.block_level == nodes[block.x, block.y + 1].block_level) //up
        { Debug.Log("down");
          //  Debug.Log(n);
            check_match(nodes[block.x, block.y + 1], n + 1);
           
        }

        if (block.y > 0 && nodes[block.x, block.y - 1].traversed == false &&
            block.block_level == nodes[block.x, block.y - 1].block_level) //down
        {Debug.Log("up");
           // Debug.Log(n);
            check_match(nodes[block.x, block.y - 1], n + 1);
            
        }
        return n;
    }





    private void Start()
    {
        createGrid();
        plane = new Plane(Vector3.up, transform.position);

    }

    private void Update()
    {
        if (!onMouseprefab)
        {
            onMouseprefab = Instantiate(cube, cube_placement.transform.position, Quaternion.identity);
        }
        getMousePositionOnGrid();
        if (onMouseprefab && !onMouseprefab.GetComponent<FollowMouse>().IsOnGrid)
        {
            onMouseprefab.position = cube_placement.transform.position;
        }
    }

    void createGrid()
    {
        nodes = new Node[height, width];
        for (int x = 0; x < height; x++)
        {
            for (int y = 0; y < width; y++)
            {
                Vector3 worldposition = new Vector3(x, y, 0);
                var spawned_tile = Instantiate(tiles, worldposition, Quaternion.identity, parent);
                spawned_tile.name = "Tile " + x + " " + y;

                var isoffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                nodes[x, y] = new Node(true, worldposition, spawned_tile.transform,x,y);


                if (x % 2 == 1)
                {
                    if (y % 2 == 1)
                    {
                        spawned_tile.GetComponent<Renderer>().material.color = Color.white;
                    }
                    if (y % 2 == 0)
                    {
                        spawned_tile.GetComponent<Renderer>().material.color = Color.black;
                    }
                }

                if (x % 2 == 0)
                {
                    if (y % 2 == 1)
                    {
                        spawned_tile.GetComponent<Renderer>().material.color = Color.black;
                    }
                    if (y % 2 == 0)
                    {
                        spawned_tile.GetComponent<Renderer>().material.color = Color.white;
                    }
                }

                if (x == 0 && y == 0)
                {
                    spawned_tile.GetComponent<Renderer>().material.color = Color.green;
                }
                if (x == 4 && y == 4)
                {
                    spawned_tile.GetComponent<Renderer>().material.color = Color.blue;
                }
                
            }
        }

        parent.Rotate(90, 0, 0);
    }
}

public class Node                     //class for each block
{
    public bool isplaceable;
    public Vector3 cellposition;
    public Transform obj;
    public bool traversed;
    public int block_level;
    public int x;
    public int y;
    public Node(bool _isplaceable, Vector3 _cellposition, Transform _obj, int xn = 0,int yn=0)
    {
        x=xn;
        y=yn;
        block_level = -1;
        traversed = false;
        isplaceable = _isplaceable;
        cellposition = _cellposition;
        obj = _obj;
    }
};