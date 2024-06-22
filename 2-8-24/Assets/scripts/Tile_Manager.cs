using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Properties;

public class Tile_Manager : MonoBehaviour
{
    [SerializeField] public GameObject tiles;
    [SerializeField] public Transform parent;
    public int height, width;
    private Node[,] nodes; //2D matrix which forms the backend
    public LayerMask ortoplanelayermask;
    private Plane plane;
    private Transform cube;
    private Transform onMouseprefab;
    public GameObject cube_placement;
    private Vector3 mousePosition;          //unnecessary
    private Vector3 tilemouseposition;
    public int highest_block_value;     //the block with the highest level on the board
    public bool onGrid = false;         //used for smooth swiping
    public Swiping swiping;
    private GameObject PrefabInstance;
    private block_node nextBlock;       //contains the block level of the block which the user has currently selected
    private Vector2[,] arrayyy =        //contains predefined values which return correct index of block on tile
    {
        { new Vector2(0, -4), new Vector2(0, -3), new Vector2(0, -2), new Vector2(0, -1), new Vector2(0, 0) },
        { new Vector2(1, -4), new Vector2(1, -3), new Vector2(1, -2), new Vector2(1, -1), new Vector2(1, 0) },
        { new Vector2(2, -4), new Vector2(2, -3), new Vector2(2, -2), new Vector2(2, -1), new Vector2(2, 0) },
        { new Vector2(3, -4), new Vector2(3, -3), new Vector2(3, -2), new Vector2(3, -1), new Vector2(3, 0) },
        { new Vector2(4, -4), new Vector2(4, -3), new Vector2(4, -2), new Vector2(4, -1), new Vector2(4, 0) }
    };
    private float xoffset=0f;
    private float yoffset=-0.4f;
    private Vector2 returnindex(Vector2 tile_index)     //return predefined values which return correct index of block on tile
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
            Ray ray = Camera.main.ScreenPointToRay(mousePosition); //unnecessary
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, float.MaxValue, ortoplanelayermask)&&!swiping.isSwiping)  //if a touch occurs on top of board
            {
                onGrid = true;
              //  Debug.Log("onGrid: true");
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
                            if (onMouseprefab)  //will create a new block on top of the pressed postion assign it values from nextBlock and then run BFS
                            {
                                node.BlockObject = PrefabInstance;
                                node.block_level = nextBlock.block_level;
                                
                                node.isplaceable = false;
                                onMouseprefab.position =
                                    new Vector3(tile_index.x + xoffset, 0.84f, tile_index.y + yoffset);
                                bool IsMatchLoop= BFS(node);
                                while (IsMatchLoop)
                                {
                                    if (node.isplaceable == true)
                                    {
                                        nextBlock = blockRandomizer.GetHigherBlock(nextBlock);
                                        GameObject nextBlockObject = nextBlock.block;
                                        PrefabInstance = Instantiate(nextBlockObject, onMouseprefab.position,
                                            Quaternion.identity);
                                        node.isplaceable = false;
                                        node.BlockObject = PrefabInstance;
                                        node.block_level = nextBlock.block_level;
                                    }

                                    IsMatchLoop = BFS(node);
                                }

                                onMouseprefab = null;
                                break;
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

    int getHighestBlock() //returns max node level
    {
        int max = 0;
            foreach (var node in nodes)
            {
                if (node.block_level > max)
                {   max = node.block_level;}
            }

            return max;
    }
    bool compareNodes(Node A, Node B)
    {
        return (A.discTime < B.discTime);
    }
    bool BFS(Node node)
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

        while (!(Q.Count == 0))//until queue is not empty
        {
            current = Q.Dequeue();
            current.discTime = time;
            current.traversed = true;

            if (current.y > 0)
            {
                if (nodes[current.x, current.y - 1].block_level == nodes[current.x, current.y].block_level) //check left
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
                if (nodes[current.x, current.y + 1].block_level == nodes[current.x, current.y].block_level) //check right
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
                if (nodes[current.x + 1, current.y].block_level == nodes[current.x, current.y].block_level) //check bottom
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
                if (nodes[current.x - 1, current.y].block_level == nodes[current.x, current.y].block_level) //check top
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

        markedNodes = FilterDuplicates(markedNodes);    //removes duplicates
        for (int i = 0; i < markedNodes.Count; i++)
        {
            markedNodes[i].traversed = false;           //turn them back to false so next run is not affected
        }

        if (markedNodes.Count >= 3)
        {
            combineBlocks(markedNodes,node);
            return true;
        }

        return false;
    }

    void combineBlocks(List<Node> markedNodes, Node current)//Destroys blocks and combine them into a single block with increased level
    {
        foreach (var node in markedNodes)
        {
            node.block_level = 0;
            node.isplaceable = true;
            node.traversed = false;
            Destroy(node.BlockObject);
            node.BlockObject = null;
        }
        
    }
    
    List<Node> FilterDuplicates(List<Node> markedNodes) //changed this function to return markedNodes wihout duplicates
    {
        HashSet<(int, int)> uniqueCoordinates = new HashSet<(int, int)>();
        List<Node> temp = new List<Node>();
        foreach (Node node in markedNodes)
        {
            if (uniqueCoordinates.Add((node.x, node.y)))
            {
                temp.Add(node);
            }
        }
        return temp;
    }
    private void Start()
    {
        if (blockRandomizer == null)
        {
            blockRandomizer = FindObjectOfType<BlockRandomizer>();
        }
        createGrid();
        plane = new Plane(Vector3.up, transform.position);

    }

    private void Update()   //unnecessary not referenced anywhere
    {
        if (!onMouseprefab)
        {
            highest_block_value = getHighestBlock();
            nextBlock = blockRandomizer.GetNextBlock();

            GameObject nextBlockObject = nextBlock.block;
            // cube = nextBlockObject.transform;
            PrefabInstance = Instantiate(nextBlockObject, cube_placement.transform.position, Quaternion.identity);
            onMouseprefab = PrefabInstance.transform;
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
    public bool isplaceable;    //flag to check if it is possible to place a block 
    public Vector3 cellposition;
    public Vector2Int parent;   //tells form which node does the destroy function initiate from(used for animation)
    public GameObject BlockObject;
    public int discTime;        //the time taken to find this particular node when BFS is called
    public Transform obj;
    public bool traversed;      
    public int block_level;     //current level of the node
    public int x;               //position in the array 'nodes'
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

