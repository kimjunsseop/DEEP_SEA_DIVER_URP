using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new GameManager();
            }
            return _instance;
        }
    }
    
    public List<bool> found = new List<bool>();
    public float endTime;
    void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
    }
    void Update()
    {
        
    }
    public void Change(int index)
    {
        found[index] = true;
    }
    public bool Check()
    {
        for(int i = 0; i < found.Count; i++)
        {
            if(found[i] == false)
            {
                return false;
            }
        }
        return true;
    }
}
