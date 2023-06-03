using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_StackableItem : AI_StackItem
{
    public override Pair<Stack<AI_StackItem>, Stack<AI_StackItem>> AutoGain(Stack<AI_StackItem> handItem, Stack<AI_StackItem> detectedItem)
    {
        Init();

        // ������ ���ٸ� �ݴ´�.
        if (handItem.Peek().CheckItemType(detectedItem.Peek()))
        {
            while (handItem.Count < 3)
            {
                if (detectedItem.Count == 0)
                    break;

                handItem.Push(detectedItem.Pop());
                handItem.Peek().RePosition(_ai.TwoHandTransform, Vector3.up * (handItem.Count) * stackInterval);
            }
        }

        else
        {
            //�ƴ϶�� �տ� �ִ°� ���ٳ��� �ٽ� �;���

            Debug.Log("Ÿ���� �޶��");
/*            if (handItem.Count <= 3 && detectedItem.Count <= 3)
            {
                Stack<AI_StackItem> temp = new Stack<AI_StackItem>(handItem);
                _aiStack.PutDown();

                while (detectedItem.Count != 0)
                {
                    handItem.Push(detectedItem.Pop());
                    handItem.Peek().RePosition(_ai.TwoHandTransform, Vector3.up * (handItem.Count - 1) * stackInterval);
                }
                while (temp.Count != 0)
                {
                    detectedItem.Push(temp.Pop());
                    detectedItem.Peek().RePosition(_ai.CurrentBlockTransform, Vector3.up * 0.5f + Vector3.up * (detectedItem.Count - 1) * stackInterval);
                }
            }*/
        }




        return new Pair<Stack<AI_StackItem>, Stack<AI_StackItem>>(handItem, detectedItem);
    }

    public override Pair<Stack<AI_StackItem>, Stack<AI_StackItem>> Change(Stack<AI_StackItem> handItem, Stack<AI_StackItem> detectedItem)
    {
        // handItem�� ������ StackableItem

        Init();

        EItemType detectedItemType = detectedItem.Peek().ItemType;
        if (detectedItemType == EItemType.axe || detectedItemType == EItemType.pick || detectedItemType == EItemType.bucket)
        {
            AI_StackItem temp = detectedItem.Pop();
            while (handItem.Count != 0)
            {
                detectedItem.Push(handItem.Pop());
                detectedItem.Peek().RePosition(_ai.CurrentBlockTransform, Vector3.up * 0.5f + Vector3.up * (detectedItem.Count - 1) * stackInterval);
            }
            handItem.Push(temp);
            handItem.Peek().RePosition(_ai.RightHandTransform, Vector3.zero);
        }
        else if (detectedItemType == EItemType.wood || detectedItemType == EItemType.steel)
        {
            if (handItem.Peek().CheckItemType(detectedItem.Peek()))
            {
                while (handItem.Count != 0)
                {
                    detectedItem.Push(handItem.Pop());
                    detectedItem.Peek().RePosition(_ai.CurrentBlockTransform, Vector3.up * 0.5f + Vector3.up * (detectedItem.Count - 1) * stackInterval);
                }
            }
            else
            {
                if (handItem.Count <= 3 && detectedItem.Count <= 3)
                {
                    Stack<AI_StackItem> temp = new Stack<AI_StackItem>(handItem);
                    _aiStack.PutDown();
                    //handItem.Peek().PutDownResource(_aiStack.AroundEmptyBlockTranform, Vector3.up * 0.5f + Vector3.up * (detectedItem.Count - 1) * stackInterval);
                    //handItem.Clear();
                    while (detectedItem.Count != 0)
                    {
                        handItem.Push(detectedItem.Pop());
                        handItem.Peek().RePosition(_ai.TwoHandTransform, Vector3.up * (handItem.Count - 1) * stackInterval);
                    }
                    while (temp.Count != 0)
                    {
                        detectedItem.Push(temp.Pop());
                        detectedItem.Peek().RePosition(_ai.CurrentBlockTransform, Vector3.up * 0.5f + Vector3.up * (detectedItem.Count - 1) * stackInterval);
                    }
                }
            }
        }
        else if (detectedItemType == EItemType.rail)
        {
            if (!CheckConnectedRail())
            {
                if (handItem.Count <= 3 && detectedItem.Count <= 3)
                {
                    Stack<AI_StackItem> temp = new Stack<AI_StackItem>(handItem);
                    handItem.Clear();
                    while (detectedItem.Count != 0)
                    {
                        handItem.Push(detectedItem.Pop());
                        handItem.Peek().RePosition(_ai.TwoHandTransform, Vector3.up * (handItem.Count - 1) * stackInterval);
                    }
                    while (temp.Count != 0)
                    {
                        detectedItem.Push(temp.Pop());
                        detectedItem.Peek().RePosition(_ai.CurrentBlockTransform, Vector3.up * 0.5f + Vector3.up * (detectedItem.Count - 1) * stackInterval);
                    }
                }
            }
        }

        return new Pair<Stack<AI_StackItem>, Stack<AI_StackItem>>(handItem, detectedItem);
    }

    public override Pair<Stack<AI_StackItem>, Stack<AI_StackItem>> PickUp(Stack<AI_StackItem> handItem, Stack<AI_StackItem> detectedItem)
    {
        Init();

        for (int i = 0; i < 3; i++)
        {
            if (detectedItem.Count == 0)
                break;
            handItem.Push(detectedItem.Pop());
            handItem.Peek().RePosition(_ai.TwoHandTransform, Vector3.up * stackInterval);
        }

        return new Pair<Stack<AI_StackItem>, Stack<AI_StackItem>>(handItem, detectedItem);
    }

    public override Pair<Stack<AI_StackItem>, Stack<AI_StackItem>> PutDown(Stack<AI_StackItem> handItem, Stack<AI_StackItem> detectedItem)
    {
        Init();

        while (handItem.Count != 0)
        {
            detectedItem.Push(handItem.Pop());
            detectedItem.Peek().PutDownResource(_ai.CurrentBlockTransform, Vector3.up * 0.5f + Vector3.up * stackInterval*handItem.Count);
        }


        return new Pair<Stack<AI_StackItem>, Stack<AI_StackItem>>(handItem, detectedItem);
    }

    public override Pair<Stack<AI_StackItem>, Stack<AI_StackItem>> EnemyPutDown(Stack<AI_StackItem> handItem, Stack<AI_StackItem> detectedItem)
    {
        Init();

        while (handItem.Count != 0)
        {
            detectedItem.Push(handItem.Pop());
            detectedItem.Peek().ThrowResource();
        }


        return new Pair<Stack<AI_StackItem>, Stack<AI_StackItem>>(handItem, detectedItem);
    }








}
