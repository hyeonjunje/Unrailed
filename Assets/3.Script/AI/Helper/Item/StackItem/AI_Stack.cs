using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Stack : MonoBehaviour
{
    public Stack<AI_StackItem> _handItem = new Stack<AI_StackItem>();
    private Stack<AI_StackItem> _detectedItem = new Stack<AI_StackItem>();

    private Transform _currentblock;

    public LayerMask BlockLayer;
    public Transform AroundEmptyBlockTranform => BFS();


    private BaseAI _ai;


    private void Awake()
    {
        _ai = GetComponent<BaseAI>();

        _handItem = new Stack<AI_StackItem>();
        _detectedItem = new Stack<AI_StackItem>();
    }

    public void InteractiveItemSpace()
    {
        if (_handItem.Count == 0 && _detectedItem.Count != 0)  // 줍기
        {

            Pair<Stack<AI_StackItem>, Stack<AI_StackItem>> p = _detectedItem.Peek().PickUp(_handItem, _detectedItem);
            _handItem = p.first;
            _detectedItem = p.second;
        }

        else if (_handItem.Count != 0 && _detectedItem.Count == 0) // 버리기
        {
            Debug.Log("버리기");

            Pair<Stack<AI_StackItem>, Stack<AI_StackItem>> p = _handItem.Peek().PutDown(_handItem, _detectedItem);
            _handItem = p.first;
            _detectedItem = p.second;
        }
        else if (_handItem.Count != 0 && _detectedItem.Count != 0) // 교체
        {
            Debug.Log("교체");

            Pair<Stack<AI_StackItem>, Stack<AI_StackItem>> p = _handItem.Peek().Change(_handItem, _detectedItem);
            _handItem = p.first;
            _detectedItem = p.second;
        }



    }

    public void PutDown()
    {
        if (_handItem.Count != 0 && _detectedItem.Count == 0) // 내려놓기
        {

            Pair<Stack<AI_StackItem>, Stack<AI_StackItem>> p = _handItem.Peek().PutDown(_handItem, _detectedItem);
            _handItem = p.first;
            _detectedItem = p.second;

            _detectedItem.Clear();
        }

    }


    public void ThrowResource()
    {
        if (_handItem.Count != 0 && _detectedItem.Count == 0) // 버리기
        {

            Pair<Stack<AI_StackItem>, Stack<AI_StackItem>> p = _handItem.Peek().EnemyPutDown(_handItem, _detectedItem);
            _handItem = p.first;
            _detectedItem = p.second;

            _detectedItem.Clear();
        }

    }

    public void InteractiveItem()
    {

        if (_detectedItem.Count != 0&&_handItem.Count!=0)
        {
            Pair<Stack<AI_StackItem>, Stack<AI_StackItem>> p = _handItem.Peek().AutoGain(_handItem, _detectedItem);
            _handItem = p.first;
            _detectedItem = p.second;
        }




    }
    public void DetectGroundBlock(WorldResource resource)
    {
        _detectedItem.Push(resource.GetComponent<AI_StackItem>());
        ResourceTracker.Instance.DeRegisterResource(resource);
        Destroy(resource);
    }


    public Transform BFS()
    {
        if (Physics.Raycast(_ai.RayStartTransfrom.position, Vector3.down, out RaycastHit hit, 1, BlockLayer))
        {
            _currentblock = hit.transform;
        }

        Queue<Transform> queue = new Queue<Transform>();
        HashSet<Transform> hashSet = new HashSet<Transform>();

        if(_currentblock!=null)
        {
            queue.Enqueue(_currentblock);
            hashSet.Add(_currentblock);
        }

        Vector3[] dir = new Vector3[8] { Vector3.forward, Vector3.back, Vector3.right, Vector3.left,
        new Vector3(1, 0, 1), new Vector3(1, 0, -1), new Vector3(-1, 0, -1), new Vector3(-1, 0, 1)};

        while (queue.Count != 0)
        {
            Transform currentBlock = queue.Dequeue();

            if (currentBlock.childCount == 0)
                return currentBlock;

            for (int i = 0; i < 8; i++)
            {
                if (Physics.Raycast(currentBlock.position, dir[i], out RaycastHit hot, 1f, BlockLayer))
                {
                    if (hashSet.Add(hot.transform))
                    {
                        queue.Enqueue(hot.transform);
                    }
                }
            }
        }

        return null;
    }
}

