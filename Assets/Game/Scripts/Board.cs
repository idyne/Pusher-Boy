using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private Texture2D boardMap;
    [Header("Prefabs")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject hamburgerPrefab, pizzaPrefab;

    public int BoardWidth { get => boardMap.width; }
    public int BoardHeight { get => boardMap.height; }

    private void Awake()
    {
        Clear();
        Build();
    }


    public void Build()
    {
        int[] playerCoordinates = new int[2];
        ReadMap(out List<int[]> enemyCoordinates, out List<int[]> blockCoordinates, out List<int[]> foodCoordinates, ref playerCoordinates);
        BuildFloor();
        SpawnPlayer(playerCoordinates);
        SpawnFoods(foodCoordinates);
        SpawnEnemies(enemyCoordinates);
        BuildBlocks(blockCoordinates);
    }

    private void ReadMap(out List<int[]> enemyCoordinates, out List<int[]> blockCoordinates, out List<int[]> foodCoordinates, ref int[] playerCoordinates)
    {
        enemyCoordinates = new List<int[]>();
        blockCoordinates = new List<int[]>();
        foodCoordinates = new List<int[]>();
        Color[] pixels = boardMap.GetPixels(0);
        for (int i = 0; i < boardMap.height; i++)
            for (int j = 0; j < boardMap.width; j++)
            {
                Color pixel = pixels[i * boardMap.width + j];
                int[] coordinates = { i, j };
                if (pixel.g == 0 && pixel.b == 0 && pixel.r * 255 >= 145)
                {
                    int fatness = ((int)(pixel.r * 255) - 145) / 10;
                    int[] coordinatesWithFatness = { i, j, fatness };
                    enemyCoordinates.Add(coordinatesWithFatness);
                }
                else if (pixel == new Color(1, 1, 0) || pixel == new Color(0, 0, 0)) blockCoordinates.Add(coordinates);
                else if (pixel == new Color(0, 1, 0)) playerCoordinates = coordinates;
                else if (pixel == new Color(0, 0, 1))
                {
                    int[] coordinatesWithFat = { i, j, 1 };
                    foodCoordinates.Add(coordinatesWithFat);
                }
                else if (pixel == new Color(200 / 255f, 0, 200 / 255f))
                {
                    int[] coordinatesWithFat = { i, j, 2 };
                    foodCoordinates.Add(coordinatesWithFat);
                }
            }
    }

    private void SpawnEnemies(List<int[]> enemyCoordinates)
    {
        Transform enemyContainer = new GameObject("Enemies").transform;
        enemyContainer.parent = transform;
        float xAnchor = (boardMap.width - 1) * -0.5f;
        float yAnchor = (boardMap.height - 1) * -0.5f;
        for (int i = 0; i < enemyCoordinates.Count; i++)
        {
            Vector3 position = new Vector3(xAnchor + enemyCoordinates[i][1], 0, yAnchor + enemyCoordinates[i][0]);
            Vector3 rotation;
            if (enemyCoordinates[i][1] == 0)
                rotation = Vector3.right;
            else if (enemyCoordinates[i][1] == boardMap.width - 1)
                rotation = Vector3.left;
            else if (enemyCoordinates[i][0] == 0)
                rotation = Vector3.forward;
            else
                rotation = Vector3.back;
            Enemy enemy = Instantiate(enemyPrefab, position, Quaternion.LookRotation(rotation), enemyContainer).GetComponent<Enemy>();
            enemy.SetFatness(enemyCoordinates[i][2]);
        }
    }

    private void SpawnFoods(List<int[]> foodCoordinates)
    {
        Transform foodContainer = new GameObject("Foods").transform;
        foodContainer.parent = transform;
        float xAnchor = (boardMap.width - 1) * -0.5f;
        float yAnchor = (boardMap.height - 1) * -0.5f;
        for (int i = 0; i < foodCoordinates.Count; i++)
        {
            Vector3 position = new Vector3(xAnchor + foodCoordinates[i][1], 0, yAnchor + foodCoordinates[i][0]);
            Instantiate(foodCoordinates[i][2] == 1 ? pizzaPrefab : hamburgerPrefab, position, Quaternion.identity, foodContainer);
        }
    }


    private void SpawnPlayer(int[] playerCoordinates)
    {
        float xAnchor = (boardMap.width - 1) * -0.5f;
        float yAnchor = (boardMap.height - 1) * -0.5f;
        Vector3 position = new Vector3(xAnchor + playerCoordinates[1], 0, yAnchor + playerCoordinates[0]);
        Vector3 rotation;
        if (playerCoordinates[1] == 1)
            rotation = Vector3.right;
        else if (playerCoordinates[1] == boardMap.width - 2)
            rotation = Vector3.left;
        else if (playerCoordinates[0] == 1)
            rotation = Vector3.forward;
        else
            rotation = Vector3.back;
        Instantiate(playerPrefab, position, Quaternion.LookRotation(rotation), transform);

    }

    private void BuildBlocks(List<int[]> blockCoordinates)
    {
        Transform blockContainer = new GameObject("Blocks").transform;
        blockContainer.parent = transform;
        float xAnchor = (boardMap.width - 1) * -0.5f;
        float yAnchor = (boardMap.height - 1) * -0.5f;
        for (int i = 0; i < blockCoordinates.Count; i++)
        {
            Vector3 position = new Vector3(xAnchor + blockCoordinates[i][1], 0, yAnchor + blockCoordinates[i][0]);
            Instantiate(wallPrefab, position, Quaternion.identity, blockContainer);
        }
    }

    private void BuildFloor()
    {
        Transform floorContainer = new GameObject("Floor").transform;
        floorContainer.parent = transform;
        float xAnchor = (boardMap.width - 1) * -0.5f;
        for (int i = 0; i < boardMap.width; i++)
        {
            float yAnchor = (boardMap.height - 1) * -0.5f;
            for (int j = 0; j < boardMap.height; j++)
            {
                Vector3 position = new Vector3(xAnchor + i, 0, yAnchor + j);
                Instantiate(floorPrefab, position, Quaternion.identity, floorContainer);
            }
        }
    }

    public void FillGap(Vector3 position)
    {
        Transform wall = Instantiate(wallPrefab, position + Vector3.down, Quaternion.identity).transform;
        wall.LeanMoveY(0, 0.4f);
    }

    public void Clear()
    {
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);
    }
}
