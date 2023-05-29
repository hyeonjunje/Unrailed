using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    public float hPos;
    public float vPos;
    public Transform pos;
    public GameObject obj;
    public bool space;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        hPos = Input.GetAxis("Horizontal");
        vPos = Input.GetAxis("Vertical");
        space = Input.GetKey(KeyCode.Space);

        transform.Translate(new Vector3(hPos, 0, vPos)* speed * Time.deltaTime);

        if(obj != null)
        {
            if (space)
            {
                obj.transform.position = pos.position;
            }
            else
            {
                obj.transform.position = new Vector3(obj.transform.position.x, 0, obj.transform.position.z);
            }
        }
  
    }
    private void OnTriggerStay(Collider other)
    {
        if (space)
        {
            if (other.CompareTag("ShopItem"))
            {
                obj = other.gameObject;
            }
        }
        
    }
}
