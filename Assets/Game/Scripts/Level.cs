using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;

public class Level : MonoBehaviour
{
    private static Level instance = null;
    private List<Enemy> enemies;
    private LevelManager levelManager;
    private Board board;
    private DynamicCamera dynamicCamera;

    public static Level Instance { get => instance; }
    public Board Board { get => board;}

    private void Awake()
    {
        if (!instance)
            instance = this;
        else
            DestroyImmediate(gameObject);
        levelManager = FindObjectOfType<LevelManager>();
        board = FindObjectOfType<Board>();
        dynamicCamera = FindObjectOfType<DynamicCamera>();
    }

    private void Start()
    {
        enemies = new List<Enemy>(FindObjectsOfType<Enemy>());
        dynamicCamera.AdjustCameraZoom();
        
    }
    public void RemoveEnemy(Enemy enemy)
    {
        enemies.Remove(enemy);
        if (enemies.Count == 0)
            levelManager.FinishLevel(true);
    }

    public void Success()
    {
        GameManager.Instance.UpdateGameState(GameState.FINISHED);
        ConfettiManager.CreateConfettiDirectional(Vector3.zero + Vector3.up * 0.5f, Vector3.up, Vector3.one * 2);
        LeanTween.delayedCall(1, () =>
        {
            SceneManager.FinishLevel(true);
        });
    }
}
