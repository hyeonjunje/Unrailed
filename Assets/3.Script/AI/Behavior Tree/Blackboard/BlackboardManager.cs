using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackboard<BlackboardKeyType>
{
    //저장공간
    Dictionary<BlackboardKeyType, int> IntValues = new Dictionary<BlackboardKeyType, int>();
    Dictionary<BlackboardKeyType, float> FloatValues = new Dictionary<BlackboardKeyType, float>();
    Dictionary<BlackboardKeyType, bool> BoolValues = new Dictionary<BlackboardKeyType, bool>();
    Dictionary<BlackboardKeyType, string> StringValues = new Dictionary<BlackboardKeyType, string>();
    Dictionary<BlackboardKeyType, Vector3> Vector3Values = new Dictionary<BlackboardKeyType, Vector3>();
    Dictionary<BlackboardKeyType, GameObject> GameObjectValues = new Dictionary<BlackboardKeyType, GameObject>();



    Dictionary<BlackboardKeyType, object> GenericValues = new Dictionary<BlackboardKeyType, object>();


    //Helper

    Dictionary<AIStat, float> AIStatValues = new Dictionary<AIStat, float>();


    public void SetStat(AIStat linkedStat, float value)
    {
        AIStatValues[linkedStat] = value;
    }

    public float GetStat(AIStat linkedStat)
    {
        if (!AIStatValues.ContainsKey(linkedStat))
            throw new System.ArgumentException($"Could not find value for {linkedStat.DisplayName} in AIStats");

        return AIStatValues[linkedStat];
    }




    //키 설정하기
    public void SetGeneric<T>(BlackboardKeyType key, T value)
    {
        GenericValues[key] = value;
    }
    // 키 가져오기
    public T GetGeneric<T>(BlackboardKeyType key)
    {
        object value;
        if (GenericValues.TryGetValue(key, out value))
            return (T)value;

        throw new System.ArgumentException($"해당 키에 해당하는 값이 없습니다");
    }

    public bool TryGetGeneric<T>(BlackboardKeyType key, out T value, T defaultValue)
    {
        object localValue;
        if (GenericValues.TryGetValue(key, out localValue))
        {
            value = (T)localValue;
            return true;
        }

        value = defaultValue;
        return false;
    }

    private T Get<T>(Dictionary<BlackboardKeyType, T> keySet, BlackboardKeyType key)
    {
        T value;
        if (keySet.TryGetValue(key, out value))
            return value;

        throw new System.ArgumentException($"Could not find value for {key} in {typeof(T).Name}Values");
    }

    private bool TryGet<T>(Dictionary<BlackboardKeyType, T> keySet, BlackboardKeyType key, out T value, T defaultValue = default)
    {
        if (keySet.TryGetValue(key, out value))
            return true;

        value = default;
        return false;
    }

    public void Set(BlackboardKeyType key, int value)
    {
        IntValues[key] = value;
    }

    public int GetInt(BlackboardKeyType key)
    {
        return Get(IntValues, key);
    }

    public bool TryGet(BlackboardKeyType key, out int value, int defaultValue = 0)
    {
        return TryGet(IntValues, key, out value, defaultValue);
    }

    public void Set(BlackboardKeyType key, float value)
    {
        FloatValues[key] = value;
    }

    public float GetFloat(BlackboardKeyType key)
    {
        return Get(FloatValues, key);
    }

    public bool TryGet(BlackboardKeyType key, out float value, float defaultValue = 0)
    {
        return TryGet(FloatValues, key, out value, defaultValue);
    }

    public void Set(BlackboardKeyType key, bool value)
    {
        BoolValues[key] = value;
    }

    public bool GetBool(BlackboardKeyType key)
    {
        return Get(BoolValues, key);
    }

    public bool TryGet(BlackboardKeyType key, out bool value, bool defaultValue = false)
    {
        return TryGet(BoolValues, key, out value, defaultValue);
    }

    public void Set(BlackboardKeyType key, string value)
    {
        StringValues[key] = value;
    }

    public string GetString(BlackboardKeyType key)
    {
        return Get(StringValues, key);
    }

    public bool TryGet(BlackboardKeyType key, out string value, string defaultValue = "")
    {
        return TryGet(StringValues, key, out value, defaultValue);
    }

    public void Set(BlackboardKeyType key, Vector3 value)
    {
        Vector3Values[key] = value;
    }

    public Vector3 GetVector3(BlackboardKeyType key)
    {
        return Get(Vector3Values, key);
    }

    public bool TryGet(BlackboardKeyType key, out Vector3 value, Vector3 defaultValue)
    {
        return TryGet(Vector3Values, key, out value, defaultValue);
    }

    public void Set(BlackboardKeyType key, GameObject value)
    {
        GameObjectValues[key] = value;
    }

    public GameObject GetGameObject(BlackboardKeyType key)
    {
        return Get(GameObjectValues, key);
    }

    public bool TryGet(BlackboardKeyType key, out GameObject value, GameObject defaultValue = null)
    {
        return TryGet(GameObjectValues, key, out value, defaultValue);
    }





}

public abstract class BlackboardKeyBase
{ }
public class BlackboardManager : MonoBehaviour
{
    public static BlackboardManager Instance { get; private set; } = null;

    //개인 정보
    private Dictionary<MonoBehaviour, object> _individualBlackboards = new Dictionary<MonoBehaviour, object>();
    //공유 정보
    private Dictionary<int, object> _sharedBlackboards = new Dictionary<int, object>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public Blackboard<T> GetIndividualBlackboard<T>(MonoBehaviour requestor) where T : BlackboardKeyBase, new()
    {
        if (!_individualBlackboards.ContainsKey(requestor))
            _individualBlackboards[requestor] = new Blackboard<T>();

        return _individualBlackboards[requestor] as Blackboard<T>;
    }

    public Blackboard<T> GetSharedBlackboard<T>(int uniqueID) where T : BlackboardKeyBase, new()
    {
        if (!_sharedBlackboards.ContainsKey(uniqueID))
            _sharedBlackboards[uniqueID] = new Blackboard<T>();

        return _sharedBlackboards[uniqueID] as Blackboard<T>;
    }


}
