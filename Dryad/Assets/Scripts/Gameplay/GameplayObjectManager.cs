using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class GameplayObjectManager
{
    private static GameplayObjectManager instance = null;
    public static GameplayObjectManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new GameplayObjectManager();
            }
            return instance;
        }
    }

    private GameplayObjectManager() { }

    public List<HealthComponent> HealthComponentList = new List<HealthComponent>();
    public List<TerrainEditor2D> TerrainList = new List<TerrainEditor2D>();

    public void RegisterBehaviour(HealthComponent behaviour)
    {
        HealthComponentList.Add(behaviour);
    }

    public void RegisterBehaviour(TerrainEditor2D behaviour)
    {
        TerrainList.Add(behaviour);
    }

    public void UnregisterBehaviour(HealthComponent behaviour)
    {
        HealthComponentList.Remove(behaviour);
    }

    public void UnregisterBehaviour(TerrainEditor2D behaviour)
    {
        TerrainList.Remove(behaviour);
    }
}
