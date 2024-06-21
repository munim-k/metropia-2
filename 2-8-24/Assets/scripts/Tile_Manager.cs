using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Tile_Manager : MonoBehaviour
{
    [SerializeField] public GameObject tiles;
    [SerializeField] public Transform parent;
    public int height, width;
    private Node[,] nodes;
    public LayerMask ortoplanelayermask;
    private Plane plane;
    private Transform cube;
    private Transform onMouseprefab;
    public GameObject cube_placement;
    private Vector3 mousePosition;
    private Vector3 tilemouseposition;
    public int highest_block_value;
    public bool onGrid = false;
    public Swiping swiping;
    private block_node nextBlock;
    private Vector2[,] arrayyy =
    {
        { new Vector2(0, -4), new Vector2(0, -3), new Vector2(0, -2), new Vector2(0, -1), new Vector2(0, 0) },
        { new Vector2(1, -4), new Vector2(1, -3), new Vector2(1, -2), new Vector2(1, -1), new Vector2(1, 0) },
        { new Vector2(2, -4), new Vector2(2, -3), new Vector2(2, -2), new Vector2(2, -1), new Vector2(2, 0) },
        { new Vector2(3, -4), new Vector2(3, -3), new Vector2(3, -2), new Vector2(3, -1), new Vector2(3, 0) },
        { new Vector2(4, -4), new Vector2(4, -3), new Vector2(4, -2), new Vector2(4, -1), new Vector2(4, 0) }
    };
    private float xoffset=0f;
    private float yoffset=-0.4f;
    private Vector2 returnindex(Vector2 tile_index)
    {
        if (tile_index.x < 0 || tile_index.y < 0 || tile_index.x > 4 || tile_index.y > 4)
        {
            return new Vector2(-1, -1);
        }
        return arrayyy[(int)(tile_index.x), (int)(tile_index.y)];
    }

    public BlockRandomizer blockRandomizer;

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
                onGrid = true;
              //  Debug.Log("onGrid: true");
                if (!swiping.isSwiping)
                {
                    tilemouseposition = hit.transform.position;
                    tilemouseposition.x = Mathf.RoundToInt(tilemouseposition.x);
                    tilemouseposition.y = Mathf.RoundToInt(tilemouseposition.y);
                    tilemouseposition.z = Mathf.RoundToInt(tilemouseposition.z);
                    foreach (var node in nodes)
                    {
                        Vector2 cell_index = new Vector2(node.cellposition.x, node.cellposition.y);

                        Vector2 tile_index = returnindex(cell_index);

                        if (tile_index.x == -1 && tile_index.y == -1)
                        {
                            break;
                        }
                        else if (tilemouseposition.x == tile_index.x && tilemouseposition.z == tile_index.y &&
                                 node.isplaceable)
                        {
                            if (onMouseprefab)
                            {
                                node.block_level = nextBlock.block_level;
                                BFS(node);
                                node.isplaceable = false;
                                onMouseprefab.position =
                                    new Vector3(tile_index.x + xoffset, 0.84f, tile_index.y + yoffset);
                                onMouseprefab = null;
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                onGrid = false;
                //Debug.Log("onGrid: false");
            }
        }
    }

    int getHighestBlock()
    {
        int max = 0;
            foreach (var node in nodes)
            {
                if (node.block_level > max)
                {   max = node.block_level;}
            }

            return max;
    }
    
    int BFS(Node node)
    {
        Queue<Node> Q = new Queue<Node>();
        List<Node> markedNodes = new List<Node>();
        node.parent.x = -1;
        node.parent.y = -1;
        Q.Enqueue(node);
        markedNodes.Add(node);

        int time = 0;
        Node current = null;
        int n = 0;

        while (!(Q.Count == 0))
        {
            current = Q.Dequeue();
            current.obj.GetComponent<Renderer>().material.color = Color.red;
            current.discTime = time;
            current.traversed = true;

            if (current.y > 0)
            {
                if (nodes[current.x, current.y - 1].block_level == nodes[current.x, current.y].block_level) //left
                {
                    if (!nodes[current.x, current.y - 1].traversed)
                    {
                        nodes[current.x, current.y - 1].parent.x = current.x;
                        nodes[current.x, current.y - 1].parent.y = current.y;
                        markedNodes.Add(nodes[current.x, current.y - 1]);
                        Q.Enqueue(nodes[current.x, current.y - 1]);
                    }
                }
            }

            if (current.y < 4)
            {
                if (nodes[current.x, current.y + 1].block_level == nodes[current.x, current.y].block_level) //right
                {
                    if (!nodes[current.x, current.y + 1].traversed)
                    {
                        nodes[current.x, current.y + 1].parent.x = current.x;
                        nodes[current.x, current.y + 1].parent.y = current.y;

                        markedNodes.Add(nodes[current.x, current.y + 1]);
                        Q.Enqueue(nodes[current.x, current.y + 1]);
                    }
                }
            }

            if (current.x < 4)
            {
                if (nodes[current.x + 1, current.y].block_level == nodes[current.x, current.y].block_level) //bottom
                {
                    if (!nodes[current.x + 1, current.y].traversed)
                    {
                        nodes[current.x + 1, current.y].parent.x = current.x;
                        nodes[current.x + 1, current.y].parent.y = current.y;

                        markedNodes.Add(nodes[current.x + 1, current.y]);
                        Q.Enqueue(nodes[current.x + 1, current.y]);
                    }
                }
            }

            if (current.x > 0)
            {
                if (nodes[current.x - 1, current.y].block_level == nodes[current.x, current.y].block_level) //top
                {
                    if (!nodes[current.x - 1, current.y].traversed)
                    {
                        nodes[current.x - 1, current.y].parent.x = current.x;
                        nodes[current.x - 1, current.y].parent.y = current.y;

                        markedNodes.Add(nodes[current.x - 1, current.y]);
                        Q.Enqueue(nodes[current.x - 1, current.y]);
                    }
                }
            }
            time++;
            n++;
        }
        for (int i = 0; i < markedNodes.Count; i++)
        {
            markedNodes[i].traversed = false;
        }
        return UniqueNodeCounter.CountUniqueNodes(markedNodes);
    }
    public class UniqueNodeCounter
    {
        public static int CountUniqueNodes(List<Node> markedNodes)
        {
            HashSet<(int, int)> uniqueCoordinates = new HashSet<(int, int)>();

            foreach (Node node in markedNodes)
            {
                uniqueCoordinates.Add((node.x, node.y));
            }

            return uniqueCoordinates.Count;
        }
    }
    private void Start()
    {
        if (blockRandomizer == null)
        {
            blockRandomizer = FindObjectOfType<BlockRandomizer>();
        }
        //        Debug.Log(blockRandomizer.blocks[0].block_level);
        createGrid();
        plane = new Plane(Vector3.up, transform.position);

    }

    private void Update()
    {
        if (!onMouseprefab)
        {
            highest_block_value = getHighestBlock();
            nextBlock = blockRandomizer.GetNextBlock();

            GameObject nextBlockObject = nextBlock.block;
            // cube = nextBlockObject.transform;
            onMouseprefab = Instantiate(nextBlockObject.transform, cube_placement.transform.position, Quaternion.identity);
        }
        getMousePositionOnGrid();
        if (onMouseprefab)
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
                nodes[x, y] = new Node(true, worldposition, spawned_tile.transform, x, y);


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
    public Vector2Int parent;
    public int discTime;
    public Transform obj;
    public bool traversed;
    public int block_level;
    public int x;
    public int y;
    public Node(bool _isplaceable, Vector3 _cellposition, Transform _obj, int xn = 0, int yn = 0)
    {
        x = xn;
        y = yn;
        block_level = -1;
        traversed = false;
        isplaceable = _isplaceable;
        cellposition = _cellposition;
        obj = _obj;
    }
};
