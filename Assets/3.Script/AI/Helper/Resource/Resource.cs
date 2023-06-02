using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Resource : MonoBehaviour
{
    public bool NonethisResourceType { get; private set; } = false;
    
    Helper Robot;

    [SerializeField] WorldResource.EType DefaultResource = WorldResource.EType.Wood;
    Dictionary<WorldResource.EType, List<WorldResource>> TrackedResources = null;


    [SerializeField] float PerfectKnowledgeRange = 30f;
    //[SerializeField] int GatherPickRange = 10;

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



    private void PopulateResources()
    {
        //자원 세팅
        var resourceTypes = System.Enum.GetValues(typeof(WorldResource.EType));
        TrackedResources = new Dictionary<WorldResource.EType, List<WorldResource>>();
        foreach (var value in resourceTypes)
        {
            var type = (WorldResource.EType)value;
            TrackedResources[type] = ResourceTracker.Instance.GetResourcesInRange(type, transform.position, PerfectKnowledgeRange);
            //총 자원수
            NumAvailableResources += TrackedResources[type].Count;
        }
    }

    public WorldResource GetGatherTarget(Helper brain)
    {
        //자원 업데이트
        PopulateResources();
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
        if (TrackedResources[targetResource].Count <= 1)
        {
            NonethisResourceType = true;
            Debug.Log($"{targetResource} :  거의 다 캤어요");
        }

        var sortedResources = TrackedResources[targetResource]
            //갈 수 있는 곳에 있는 자원만 추리기
            .Where(resource => Vector3.Distance(resource.transform.position, brain.Agent.
                                                FindCloestAroundEndPosition(resource.transform.position)) <= 1.5f)
            //가까운 순으로 정렬
            .OrderBy(resource => Vector3.Distance(brain.transform.position, resource.transform.position))
            //가장 가까운 자원 반환
            .FirstOrDefault();
        return sortedResources;



    }

}
