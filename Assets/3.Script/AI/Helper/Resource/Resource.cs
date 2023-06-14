using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Resource : MonoBehaviour
{
    public bool NonethisResourceTypeHelper { get; private set; } = false;
    public bool NonethisResourceTypeEnemy { get; private set; } = false;

    private WorldResource.EType _defaultResource = WorldResource.EType.Wood;
    private Dictionary<WorldResource.EType, List<WorldResource>> _trackedResources = null;
    private float _range = 60;

    private void Start()
    {
        PopulateResources();
    }

    public void PopulateResources()
    {
        //�ڿ� ����
        var resourceTypes = System.Enum.GetValues(typeof(WorldResource.EType));
        _trackedResources = new Dictionary<WorldResource.EType, List<WorldResource>>();
        foreach (var value in resourceTypes)
        {
            var type = (WorldResource.EType)value;
            _trackedResources[type] = ResourceTracker.Instance.GetResourcesInRange(type, transform.position, _range);
        }
    }

    public void ResetResources()
    {
        var resourceTypes = System.Enum.GetValues(typeof(WorldResource.EType));
        foreach (var value in resourceTypes)
        {
            var type = (WorldResource.EType)value;
            for(int i=0; i<_trackedResources[type].Count; i++)
            {
                ResourceTracker.Instance.DeRegisterResource(_trackedResources[type][i]);
            }
        }
    }




    public WorldResource GetGatherTarget(Helper brain)
    {
        //�ڿ� ������Ʈ
        PopulateResources();
        WorldResource.EType targetResource = _defaultResource;
        var resourceTypes = System.Enum.GetValues(typeof(WorldResource.EType));

        foreach (var typeValue in resourceTypes)
        {
            var resourceType = (WorldResource.EType)typeValue;
            //����̶� ������ Ȯ��
            if(resourceType==brain.TargetResource)
            {
                targetResource = resourceType;
                break;
            }
        }
        //�ڿ��� �ִ��� Ȯ��
        if (_trackedResources[targetResource].Count <1)
        {
            NonethisResourceTypeHelper = true;
            Debug.Log($"{targetResource} :  �ڿ��� ���� �����");
        }
        else
            NonethisResourceTypeHelper = false;

        var sortedResources = _trackedResources[targetResource]
            //�� �� �ִ� ���� �ִ� �ڿ��� �߸���
            .Where(resource => Vector3.Distance(resource.transform.position, brain.Agent.
                                                FindCloestAroundEndPosition(resource.transform.position)) <= 1.5f)
            //����� ������ ����
            .OrderBy(resource => Vector3.Distance(brain.transform.position, resource.transform.position))
            //���� ����� �ڿ� ��ȯ
            .FirstOrDefault();

        return sortedResources;
    }


    public WorldResource TargettoSteal(PathFindingAgent brain)
    {
        //�ڿ� ������Ʈ
        PopulateResources();
        WorldResource.EType targetResource = WorldResource.EType.Resource;
        //�ڿ��� �ִ��� Ȯ��
        if (_trackedResources[targetResource].Count < 1)
        {
            NonethisResourceTypeEnemy = true;
            Debug.Log($"{targetResource} :  ��ĥ �ڿ��� ���� �����");
        }
        else
            NonethisResourceTypeEnemy = false;

        var sortedResources = _trackedResources[targetResource]
            //�� �� �ִ� ���� �ִ� �ڿ��� �߸���
            .Where(resource => Vector3.Distance(resource.transform.position, brain.
                                                FindCloestAroundEndPosition(resource.transform.position)) <= 1.5f)
            //����� ������ ����
            .OrderBy(resource => Vector3.Distance(brain.transform.position, resource.transform.position))
            //���� ����� �ڿ� ��ȯ
            .FirstOrDefault();
        return sortedResources;
    }



    public WorldResource ResearchTarget(Helper brain)
    {
        //�� �� �ִ� ���� ���ٸ� ����
        WorldResource.EType targetResource = _defaultResource;
        var resourceTypes = System.Enum.GetValues(typeof(WorldResource.EType));

        foreach (var typeValue in resourceTypes)
        {
            var resourceType = (WorldResource.EType)typeValue;
            //����̶� ������ Ȯ��
            if (resourceType == brain.TargetResource)
            {
                targetResource = resourceType;
                break;
            }
        }
        //�ڿ��� �ִ��� Ȯ��
        
        var sortedResources = _trackedResources[targetResource]
            .OrderBy(resource => Vector3.Distance(brain.transform.position, resource.transform.position))
            .Take(20)
            .Where(resource => Vector3.Distance
            (brain.Agent.FindCloestAroundEndPosition(resource.transform.position), resource.transform.position)<1.5f)
            .Where(resource=> brain.Agent.MoveTo(brain.Agent.FindCloestAroundEndPosition(resource.transform.position)))
            //���� ����� �ڿ� ��ȯ
            .FirstOrDefault();

        return sortedResources;
    }

}
