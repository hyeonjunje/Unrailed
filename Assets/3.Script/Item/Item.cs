using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EItem
{
    woodPiece,
    ironPiece,
    axe,
    pick,
    bucket,
    bolt,
    none
}

public class Item : MonoBehaviour
{ 
    

    [SerializeField] public EItem itemType;
  

    private void Awake()
    {
        //resourceNumber = ResourceItem.woodPiece;
    }



    void Update()
    {
        
    }
}
