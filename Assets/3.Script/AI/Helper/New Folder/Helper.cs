using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper : MonoBehaviour
{
    public Resource Home { get; private set; } = null;
    public Resource dd;
    [SerializeField] public WorldResource.EType TargetResource = WorldResource.EType.Wood;
    private void Awake()
    {
        Home = dd;
    }

    public void SetHome(Resource _Home)
    {
        Debug.Log("¿ì¸®Áý");
        Home = _Home;
    }

}
