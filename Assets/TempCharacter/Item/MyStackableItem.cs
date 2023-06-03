using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyStackableItem : MyItem
{
    public override Pair<Stack<MyItem>, Stack<MyItem>> AutoGain(Stack<MyItem> handItem, Stack<MyItem> detectedItem)
    {
        // 종류가 같다면 줍는다.
        if(handItem.Peek().CheckItemType(detectedItem.Peek()))
        {
            while(handItem.Count < 3)
            {
                if (detectedItem.Count == 0)
                    break;

                handItem.Push(detectedItem.Pop());
                handItem.Peek().RePosition(handItem.Peek().equipment, Vector3.up * (handItem.Count - 1) * stackInterval);
            }
        }

        return new Pair<Stack<MyItem>, Stack<MyItem>>(handItem, detectedItem);
    }

    public override Pair<Stack<MyItem>, Stack<MyItem>> Change(Stack<MyItem> handItem, Stack<MyItem> detectedItem)
    {
        // handItem은 무조건 StackableItem

        EItemType detectedItemType = detectedItem.Peek().ItemType;
        if (detectedItemType == EItemType.axe || detectedItemType == EItemType.pick || detectedItemType == EItemType.bucket)
        {
            MyItem temp = detectedItem.Pop();
            while(handItem.Count != 0)
            {
                detectedItem.Push(handItem.Pop());
                detectedItem.Peek().RePosition(player.CurrentBlockTransform, Vector3.up * 0.5f + Vector3.up * (detectedItem.Count - 1) * stackInterval);
            }
            handItem.Push(temp);
            handItem.Peek().RePosition(handItem.Peek().equipment, Vector3.zero);
        }
        else if (detectedItemType == EItemType.wood || detectedItemType == EItemType.steel)
        {
            if (handItem.Peek().CheckItemType(detectedItem.Peek()))
            {
                // 그냥 쌓는다.
                while (handItem.Count != 0)
                {
                    detectedItem.Push(handItem.Pop());
                    detectedItem.Peek().RePosition(player.CurrentBlockTransform, Vector3.up * 0.5f + Vector3.up * (detectedItem.Count - 1) * stackInterval);
                }
            }
            else
            {
                Stack<MyItem> temp = new Stack<MyItem>(handItem);
                handItem.Clear();

                // 한계까지 든다
                while(handItem.Count < 3)
                {
                    if (detectedItem.Count == 0)
                        break;
                    handItem.Push(detectedItem.Pop());
                    handItem.Peek().RePosition(handItem.Peek().equipment, Vector3.up * (handItem.Count - 1) * stackInterval);
                }

                // 다른 타입이니까 내 손에 있는거는 다른곳에 놓는다.
                Transform aroundTransform = player.AroundEmptyBlockTranform;
                int count = 0;
                while(temp.Count != 0)
                {
                    temp.Pop().RePosition(aroundTransform, Vector3.up * 0.5f + Vector3.up * count++ * stackInterval);
                }
            }
        }
        else if (detectedItemType == EItemType.rail)
        {
            if (IsRailInteractive(detectedItem.Peek().GetComponent<RailController>()))
            {
                Stack<MyItem> temp = new Stack<MyItem>(handItem);
                handItem.Clear();

                // 한계까지 든다
                while (handItem.Count < 3)
                {
                    if (detectedItem.Count == 0)
                        break;
                    handItem.Push(detectedItem.Pop());
                    handItem.Peek().RePosition(handItem.Peek().equipment, Vector3.up * (handItem.Count - 1) * stackInterval);
                }

                if(detectedItem.Count == 0)
                {
                    while(temp.Count != 0)
                    {
                        detectedItem.Push(temp.Pop());
                        detectedItem.Peek().RePosition(player.CurrentBlockTransform, Vector3.up * 0.5f + Vector3.up * (detectedItem.Count - 1) * stackInterval);
                    }
                }
                else
                {
                    Transform aroundTransform = player.AroundEmptyBlockTranform;
                    int count = 0;
                    while (temp.Count != 0)
                    {
                        temp.Pop().RePosition(aroundTransform, Vector3.up * 0.5f + Vector3.up * count++ * stackInterval);
                    }
                }
            }
        }

        return new Pair<Stack<MyItem>, Stack<MyItem>>(handItem, detectedItem);
    }

    public override Pair<Stack<MyItem>, Stack<MyItem>> PickUp(Stack<MyItem> handItem, Stack<MyItem> detectedItem)
    {
        for(int i = 0; i < 3; i++)
        {
            if (detectedItem.Count == 0)
                break;
            handItem.Push(detectedItem.Pop());
            handItem.Peek().RePosition(handItem.Peek().equipment, Vector3.up * (handItem.Count - 1) * stackInterval);
        }

        return new Pair<Stack<MyItem>, Stack<MyItem>>(handItem, detectedItem);
    }

    public override Pair<Stack<MyItem>, Stack<MyItem>> PutDown(Stack<MyItem> handItem, Stack<MyItem> detectedItem)
    {
        Transform aroundTransform = player.AroundEmptyBlockTranform;
        while(handItem.Count != 0)
        {
            detectedItem.Push(handItem.Pop());
            detectedItem.Peek().RePosition(aroundTransform, Vector3.up * 0.5f + Vector3.up * (detectedItem.Count - 1) * stackInterval);
        }

        return new Pair<Stack<MyItem>, Stack<MyItem>>(handItem, detectedItem);
    }
}
