using System;
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
    public void Add<T>(object in_managed)
    {
        Type type = typeof(T);
        if (_managed.ContainsKey(type))
        {
            Debug.LogError($"[ServiceManager] {type} already has a registered service.");
            return;
        }
        _managed.Add(type, in_managed);
    }
    public T Get<T>()
    {
        Type type = typeof(T);
        if (!_managed.ContainsKey(type))
        {
            Debug.LogError($"[ServiceManager] {type} is not a registered service.");
            return default;
        }
        return (T)_managed[type];
    }
    public void Remove<T>()
    {
        _managed.Remove(typeof(T));
    }
    protected Dictionary<Type, object> _managed = new();
}
