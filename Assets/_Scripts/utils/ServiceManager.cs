using System.Collections.Generic;
using UnityEngine;

public class ServiceManager : MonoBehaviour
{
    #region Singleton
    static public ServiceManager instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject instanceObj = new GameObject("ServiceManager");
                _instance = instanceObj.AddComponent<ServiceManager>();
                DontDestroyOnLoad(instanceObj);
            }
            return _instance;
        }
    }
    static private ServiceManager _instance;
    #endregion Singleton
    protected void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
    public void Add(string in_name, object in_managed)
    {
        if (_managed.ContainsKey(in_name))
        {
            Debug.LogError($"[ServiceManager] {in_name} already has a registered service.");
            return;
        }
        _managed.Add(in_name, in_managed);
    }
    public object Get(string in_name)
    {
        if (!_managed.ContainsKey(in_name))
        {
            Debug.LogError($"[ServiceManager] {in_name} is not a registered service.");
            return null;
        }
        return _managed[in_name];
    }
    public void Remove(string in_name)
    {
        _managed.Remove(in_name);
    }
    protected Dictionary<string, object> _managed = new();
}
