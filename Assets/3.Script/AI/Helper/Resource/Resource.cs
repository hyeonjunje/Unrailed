using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Resource : MonoBehaviour
{
    public Transform spawnPoint;
    Helper Robot;

    [SerializeField] WorldResource.EType DefaultResource = WorldResource.EType.Wood;
    Dictionary<WorldResource.EType, List<WorldResource>> TrackedResources = null;

    [SerializeField] float PerfectKnowledgeRange = 100f;
    [SerializeField] int GatherPickRange = 10;

    public int NumAvailableResources { get; private set; } = 0;

    void Awake()
    {
        //나중에 로봇 생성 여기로 바꾸기
        Robot = FindObjectOfType<Helper>();
        Robot.SetHome(this);
    }

    private void Start()
    {
        PopulateResources();
    }

    private void Update()
    {
        if (TrackedResources == null)
            PopulateResources();
    }



    void PopulateResources()
    {
        //자원 세팅
        var resourceTypes = System.Enum.GetValues(typeof(WorldResource.EType));
        TrackedResources = new Dictionary<WorldResource.EType, List<WorldResource>>();
        foreach (var value in resourceTypes)
        {
            var type = (WorldResource.EType)value;
            TrackedResources[type] = ResourceTracker.Instance.GetResourcesInRange(type, transform.position, PerfectKnowledgeRange);
            //Debug.Log($"{TrackedResources[type].Count}, {type}");
            //총 자원수
            NumAvailableResources += TrackedResources[type].Count;
        }
    }

    public WorldResource GetGatherTarget(Helper brain)
    {
        WorldResource.EType targetResource = DefaultResource;
        var resourceTypes = System.Enum.GetValues(typeof(WorldResource.EType));
        foreach (var typeValue in resourceTypes)
        {
            var resourceType = (WorldResource.EType)typeValue;
            //명령이랑 같은지 확인
            if(resourceType==brain.TargetResource)
            {
                targetResource = resourceType;
                break;
            }
        }
        //자원이 있는지 확인
        if (TrackedResources[targetResource].Count == 0)
        {
            Debug.Log($"{targetResource} :  없어요");
        }
        var sortedResources = TrackedResources[targetResource].OrderBy(resource => Vector3.Distance(brain.transform.position, resource.transform.position)).ToList();
        //return sortedResources[Random.Range(0, Mathf.Min(GatherPickRange, sortedResources.Count))];
        return sortedResources[0];

    }

}
