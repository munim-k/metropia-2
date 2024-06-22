using UnityEngine;

public class BlockRandomizer : MonoBehaviour
{
    public GameObject[] block_objects;
    public block_node[] blocks;
    private float[] probabilities = { 0.5f, 0.3f, 0.1f, 0.07f, 0.03f };
    public Tile_Manager tileManager;

    private void Start()
    {
        blocks = new block_node[block_objects.Length];
        for (int i = 0; i < block_objects.Length; i++)
        {
            blocks[i] = new block_node(block_objects[i], i+1);
        }
    }

    public block_node GetNextBlock()
    {
        int highestBlock = tileManager.highest_block_value;
        float[] adjustedProbabilities = probabilities;

        // Adjust probabilities based on the highest block
        if (highestBlock >= blocks[4].block_level)
        {
            adjustedProbabilities = new float[] { 0.5f, 0.3f, 0.1f, 0.07f, 0.03f };
        }
        else if (highestBlock >= blocks[3].block_level)
        {
            adjustedProbabilities = new float[] { 0.5f, 0.3f, 0.1f, 0.1f, 0f };
        }
        else if (highestBlock >= blocks[2].block_level)
        {
            adjustedProbabilities = new float[] { 0.5f, 0.3f, 0.2f, 0f, 0f };
        }
        else if (highestBlock >= blocks[1].block_level)
        {
            adjustedProbabilities = new float[] { 0.7f, 0.3f, 0f, 0f, 0f };
        }
        else
        {
            adjustedProbabilities = new float[] { 0.8f, 0.2f, 0f, 0f, 0f };
        }

        return GetRandomBlock(adjustedProbabilities);
    }

    public block_node GetHigherBlock(block_node block)
    {
        int level = block.block_level;
        return blocks[level];
    }
    private block_node GetRandomBlock(float[] adjustedProbabilities)
    {
        float total = 0;
        float randomValue = UnityEngine.Random.value;
        for (int i = 0; i < adjustedProbabilities.Length; i++)
        {
            total += adjustedProbabilities[i];
            if (randomValue <= total)
            {
                return blocks[i];
            }
        }

        // Fallback in case of floating-point precision issues
        return blocks[0];
    }
}

public class block_node
{
    public GameObject block;
    public int block_level;

    public block_node(GameObject _block, int _level)
    {
        block = _block;
        block_level = _level;
    }
}
