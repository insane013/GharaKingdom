using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "newAtlas", menuName = "Custom/Create Atlas")]
public class AtlasScriptableObject : ScriptableObject
{
    [SerializeField] string resoursePath = "Sprites/FieldAtlas/Atlas 0";

    private GameObject[] loadedCells;

    public List<GameObject> GetAtlas()
    {
        List<GameObject> list = new List<GameObject>();

        list = loadedCells.Select(item => item).ToList();
        return list;
    }

    public void LoadResourses()
    {
        loadedCells = Resources.LoadAll<GameObject>(resoursePath);

        if (loadedCells.Length == 0) throw new Exception("No resourses founded. Please check the resourses' path.");
    }
}
