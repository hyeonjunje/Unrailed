using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Village : MonoBehaviour
{
    public Transform spawnPoint;
    [SerializeField] GameObject VillagerPrefab;
    List<GOAPBrain> Villagers = new List<GOAPBrain>();

    [SerializeField] int GatherPickRange = 10;
    [SerializeField] float PerfectKnowledgeRange = 100f;
    List<GOAPBrain> Gatherers = new List<GOAPBrain>();
    public int NumAvailableResources { get; private set; } = 0;

    [SerializeField] WorldResource.EType DefaultResource = WorldResource.EType.Wood;
    Dictionary<GOAPBrain, WorldResource.EType> GathererAssignments = new Dictionary<GOAPBrain, WorldResource.EType>();
    Dictionary<WorldResource.EType, List<WorldResource>> TrackedResources = null;

    void Awake()
    {
        // nothing stored to begin with
        var resourceTypes = System.Enum.GetValues(typeof(WorldResource.EType));
    }

    private void Start()
    {
        var villager = Instantiate(VillagerPrefab, spawnPoint.position, spawnPoint.rotation);
        villager.name = $"{gameObject.name}_Villager";
        Villagers.Add(villager.GetComponent<GOAPBrain>());

        villager.GetComponent<AIState>().SetHome(this);
    }



    public WorldResource GetGatherTarget(GOAPBrain brain)
    {
        int lowestNumGatherers = int.MaxValue;
        WorldResource.EType targetResource = DefaultResource;

        // find the resource most in need
        var resourceTypes = System.Enum.GetValues(typeof(WorldResource.EType));
        foreach (var typeValue in resourceTypes)
        {
            var resourceType = (WorldResource.EType)typeValue;

            // if there are none of this resource type then skip
            if (TrackedResources[resourceType].Count == 0)
                Debug.Log("자원 없음");
                continue;

            // determine how many are gathering this resource
            int numGatherers = 0;
            foreach (var gatheredResource in GathererAssignments.Values)
            {
                if (gatheredResource == resourceType)
                    ++numGatherers;
            }

            // found a new best target?
            if (numGatherers < lowestNumGatherers)
            {
                lowestNumGatherers = numGatherers;
                targetResource = resourceType;
            }
        }

        if (TrackedResources[targetResource].Count == 0)
            return null;

        GathererAssignments[brain] = targetResource;

        var sortedResources = TrackedResources[targetResource].OrderBy(resource => Vector3.Distance(brain.transform.position, resource.transform.position)).ToList();
        return sortedResources[Random.Range(0, Mathf.Min(GatherPickRange, sortedResources.Count))];
    }

    private void Update()
    {
        if (TrackedResources == null)
            PopulateResources();
    }



    public void SawResource(WorldResource resource)
    {
        if (!TrackedResources[resource.Type].Contains(resource))
        {
            TrackedResources[resource.Type].Add(resource);
            ++NumAvailableResources;
        }
    }

    public void AddGatherer(GOAPBrain brain)
    {
        Gatherers.Add(brain);
    }

    public void RemoveGatherer(GOAPBrain brain)
    {
        Gatherers.Remove(brain);
        GathererAssignments.Remove(brain);
    }
    void PopulateResources()
    {
        // build up the resource knowledge
        var resourceTypes = System.Enum.GetValues(typeof(WorldResource.EType));
        TrackedResources = new Dictionary<WorldResource.EType, List<WorldResource>>();
        foreach (var value in resourceTypes)
        {
            var type = (WorldResource.EType)value;
            TrackedResources[type] = ResourceTracker.Instance.GetResourcesInRange(type, transform.position, PerfectKnowledgeRange);
            Debug.Log($"{TrackedResources[type].Count}, {type}");
            NumAvailableResources += TrackedResources[type].Count;
            Debug.Log(NumAvailableResources);
        }
    }

    public void StoreResource(WorldResource.EType type, int amount)
    {
        //ResourcesStored[type] += amount;
    }



}
