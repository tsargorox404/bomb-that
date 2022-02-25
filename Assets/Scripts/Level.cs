using System;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] private LevelConfig _levelConfig;

    private Ground _ground;
    private List<Border> _bordersPool;
    private List<Obstacle> _obstaclesPool;
    private List<SpawnPointer> _spawnPointersPool;
    private List<Character> _charactersPool;
    
    private LevelAssembler _levelAssembler;
    private CharactersSpawner _charactersSpawner;
    
    public Border LeftBorder { get; set; }
    public Border RightBorder { get; set; }
    public Border BottomBorder { get; set; }
    public Border TopBorder { get; set; }
    public List<SpawnPointer> SpawnPointers { get; set; }
    public List<Character> CharactersPool => _charactersPool;

    public event Action LevelStarted;

    private void Awake()
    {
        LoadLevelItems();
        LoadCharacters();
        
        gameObject.SetActive(false);
    }

    public void StartLevel()
    {
        gameObject.SetActive(true);
        
        _levelAssembler = new LevelAssembler();
        _charactersSpawner = new CharactersSpawner();
        
        _levelAssembler.Assemble(this, _levelConfig, _ground, _bordersPool, _obstaclesPool, _spawnPointersPool);
        _charactersSpawner.Spawn(_charactersPool, SpawnPointers);
        
        LevelStarted?.Invoke();
    }

    public void ClearLevel()
    {
        _ground.gameObject.SetActive(false);
        _ground.transform.SetParent(transform);

        RefreshLevelItemsPool(_bordersPool);
        RefreshLevelItemsPool(_obstaclesPool);
        RefreshLevelItemsPool(_spawnPointersPool);

        foreach (var character in _charactersPool)
            character.gameObject.SetActive(false);
    }

    private void LoadLevelItems()
    {
        _ground = Instantiate(_levelConfig.GroundPrefab, transform);
        _ground.gameObject.SetActive(false);
        
        _bordersPool = GetLevelItemsPool(_levelConfig.BorderPrefab, _levelConfig.BordersCount);
        _obstaclesPool = GetLevelItemsPool(_levelConfig.ObstaclePrefab, 
            _levelConfig.MaxObstaclesInLineCount * (_levelConfig.MaxObstaclesInLineCount / 2));
        _spawnPointersPool = GetLevelItemsPool(_levelConfig.SpawnPointerPrefab, _levelConfig.MaxObstaclesInLineCount);
    }

    private void LoadCharacters()
    {
        _charactersPool = new List<Character>();
        
        var player = Instantiate(_levelConfig.PlayerPrefab, transform);
        
        player.gameObject.SetActive(false);
        _charactersPool.Add(player);

        for (int i = 0; i < _levelConfig.MaxObstaclesInLineCount / 2; i++)
        {
            var enemy = Instantiate(_levelConfig.EnemyPrefab, transform);
            
            enemy.gameObject.SetActive(false);
            _charactersPool.Add(enemy);
        }
    }

    private List<T> GetLevelItemsPool<T>(T prefab, int count) where T : LevelItem
    {
        var items = new List<T>();

        for (int i = 0; i < count; i++)
        {
            items.Add(Instantiate(prefab, transform));
            items[i].gameObject.SetActive(false);
        }

        return items;
    }

    private void RefreshLevelItemsPool<T>(List<T> itemsPool) where T : LevelItem
    {
        foreach (var item in itemsPool)
        {
            item.gameObject.SetActive(false);
            item.transform.SetParent(transform);
        }
    }
}