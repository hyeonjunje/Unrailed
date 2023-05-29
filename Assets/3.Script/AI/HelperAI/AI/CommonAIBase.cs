using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AIStatConfiguration
{
    [field: SerializeField] public AIStat LinkedStat { get; private set; }

    [field: SerializeField] public bool OverrideDefaults { get; private set; } = false;
    [field: SerializeField, Range(0f, 1f)] public float Override_InitialValue { get; protected set; } = 0.5f;
    [field: SerializeField, Range(0f, 1f)] public float Override_DecayRate { get; protected set; } = 0.005f;
}

[RequireComponent(typeof(BaseNavigation))]
public class CommonAIBase : MonoBehaviour
{
    public class BlackBoardKey : BlackboardKeyBase
    {
        public static readonly BlackBoardKey Memories_ShortTerm = new BlackBoardKey() { Name = "Memories_ShortTerm" };
        public static readonly BlackBoardKey Memories_LongTerm = new BlackBoardKey() { Name = "Memories_LongTerm" };
        public static readonly BlackBoardKey Household_ObjectsInUse = new BlackBoardKey() { Name = "Household_ObjectsInUse" };
        public static readonly BlackBoardKey Character_FocusObject = new BlackBoardKey() { Name = "Character_FocusObject" };

        public string Name;


    }


    [Header("General")]
    [SerializeField] int HouseholdID = 1;
    [field: SerializeField] AIStatConfiguration[] Stats;
    [SerializeField] protected FeedbackUIPanel LinkedUI;

    [Header("Traits")]
    [SerializeField] protected List<Trait> Traits;

    [Header("Memories")]
    [SerializeField] int LongTermMemoryThreshold = 2;

    protected BaseNavigation Navigation;
    protected bool StartedPerforming = false;

    public Blackboard<BlackBoardKey> IndividualBlackboard { get; protected set; }
    public Blackboard<BlackBoardKey> HouseholdBlackboard { get; protected set; }

    protected Dictionary<AIStat, float> DecayRates = new Dictionary<AIStat, float>();
    protected Dictionary<AIStat, AIStatPanel> StatUIPanels = new Dictionary<AIStat, AIStatPanel>();

    protected BaseInteraction CurrentInteraction
    {
        get
        {
            BaseInteraction interaction = null;
            IndividualBlackboard.TryGetGeneric(BlackBoardKey.Character_FocusObject, out interaction, null);
            return interaction;
        }
        set
        {
            BaseInteraction previousInteraction = null;
            IndividualBlackboard.TryGetGeneric(BlackBoardKey.Character_FocusObject, out previousInteraction, null);

            IndividualBlackboard.SetGeneric(BlackBoardKey.Character_FocusObject, value);

            List<GameObject> objectsInUse = null;
            HouseholdBlackboard.TryGetGeneric(BlackBoardKey.Household_ObjectsInUse, out objectsInUse, null);

            // are we starting to use something?
            if (value != null)
            {
                // need to create list?
                if (objectsInUse == null)
                    objectsInUse = new List<GameObject>();

                // not already in list? add and update the blackboard
                if (!objectsInUse.Contains(value.gameObject))
                {
                    objectsInUse.Add(value.gameObject);
                    HouseholdBlackboard.SetGeneric(BlackBoardKey.Household_ObjectsInUse, objectsInUse);
                }
            } // we've stopped using something
            else if (objectsInUse != null)
            {
                // attempt to remove and update the blackboard if changed
                if (objectsInUse.Remove(previousInteraction.gameObject))
                    HouseholdBlackboard.SetGeneric(BlackBoardKey.Household_ObjectsInUse, objectsInUse);
            }
        }
    }

    protected virtual void Awake()
    {
        Navigation = GetComponent<BaseNavigation>();
    }

    protected virtual void Start()
    {
        IndividualBlackboard = BlackboardManager.Instance.GetIndividualBlackboard<BlackBoardKey>(this);
        HouseholdBlackboard = BlackboardManager.Instance.GetSharedBlackboard<BlackBoardKey>(HouseholdID);

        IndividualBlackboard.SetGeneric(BlackBoardKey.Memories_ShortTerm, new List<MemoryFragment>());
        IndividualBlackboard.SetGeneric(BlackBoardKey.Memories_LongTerm, new List<MemoryFragment>());

        // setup the stats
        foreach (var statConfig in Stats)
        {
            var linkedStat = statConfig.LinkedStat;
            float initialValue = statConfig.OverrideDefaults ? statConfig.Override_InitialValue : linkedStat.InitialValue;
            float decayRate = statConfig.OverrideDefaults ? statConfig.Override_DecayRate : linkedStat.DecayRate;

            DecayRates[linkedStat] = decayRate;
            IndividualBlackboard.SetStat(linkedStat, initialValue);

            if (linkedStat.IsVisible)
                StatUIPanels[linkedStat] = LinkedUI.AddStat(linkedStat, initialValue);
        }
    }

    protected float ApplyTraitsTo(AIStat targetStat, Trait.ETargetType targetType, float currentValue)
    {
        foreach (var trait in Traits)
        {
            currentValue = trait.Apply(targetStat, targetType, currentValue);
        }

        return currentValue;
    }

    protected virtual void Update()
    {
        if (CurrentInteraction != null)
        {
            if (Navigation.IsAtDestination && !StartedPerforming)
            {
                StartedPerforming = true;
                CurrentInteraction.Perform(this, OnInteractionFinished);
            }
        }

        // apply the decay rate
        foreach (var statConfig in Stats)
        {
            UpdateIndividualStat(statConfig.LinkedStat, -DecayRates[statConfig.LinkedStat] * Time.deltaTime, Trait.ETargetType.DecayRate);
        }

        // tick recent memories
        List<MemoryFragment> recentMemories = IndividualBlackboard.GetGeneric<List<MemoryFragment>>(BlackBoardKey.Memories_ShortTerm);
        bool memoriesChanged = false;

        for (int index = recentMemories.Count - 1; index >= 0; index--)
        {
            if (!recentMemories[index].Tick(Time.deltaTime))
            {
                recentMemories.RemoveAt(index);
                memoriesChanged = true;
            }
        }

        if (memoriesChanged)
            IndividualBlackboard.SetGeneric(BlackBoardKey.Memories_ShortTerm, recentMemories);
    }

    protected virtual void OnInteractionFinished(BaseInteraction interaction)
    {
        interaction.UnlockInteraction(this);
        CurrentInteraction = null;
        Debug.Log($"{interaction.DisplayName} | 상호 작용 완료");
    }

    public void UpdateIndividualStat(AIStat linkedStat, float amount, Trait.ETargetType targetType)
    {
        float adjustedAmount = ApplyTraitsTo(linkedStat, targetType, amount);
        float newValue = Mathf.Clamp01(GetStatValue(linkedStat) + adjustedAmount);

        IndividualBlackboard.SetStat(linkedStat, newValue);

        if (linkedStat.IsVisible)
            StatUIPanels[linkedStat].OnStatChanged(newValue);
    }

    public float GetStatValue(AIStat linkedStat)
    {
        return IndividualBlackboard.GetStat(linkedStat);
    }

    public void AddMemories(MemoryFragment[] memoriesToAdd)
    {
        foreach (var memory in memoriesToAdd)
            AddMemory(memory);
    }

    //기억 추가하기
    protected void AddMemory(MemoryFragment memoryToAdd)
    {
        List<MemoryFragment> permanentMemories = IndividualBlackboard.GetGeneric<List<MemoryFragment>>(BlackBoardKey.Memories_LongTerm);

        // 이미 장기 기억에 있는지 확인하기
        MemoryFragment memoryToCancel = null;

        //기억들 다 가져오기
        foreach (var memory in permanentMemories)
        {
            if (memoryToAdd.IsSimilarTo(memory))
                return;
            if (memory.IsCancelledBy(memoryToAdd))
                memoryToCancel = memory;
        }

        // does this cancel a long term memory?
        if (memoryToCancel != null)
        {
            permanentMemories.Remove(memoryToCancel);
            IndividualBlackboard.SetGeneric(BlackBoardKey.Memories_LongTerm, permanentMemories);
        }

        List<MemoryFragment> recentMemories = IndividualBlackboard.GetGeneric<List<MemoryFragment>>(BlackBoardKey.Memories_ShortTerm);

        // does this cancel a recent memory?
        MemoryFragment existingRecentMemory = null;
        foreach (var memory in recentMemories)
        {
            if (memoryToAdd.IsSimilarTo(memory))
                existingRecentMemory = memory;
            if (memory.IsCancelledBy(memoryToAdd))
                memoryToCancel = memory;
        }

        // does this cancel a short term memory?
        if (memoryToCancel != null)
        {
            recentMemories.Remove(memoryToCancel);
            IndividualBlackboard.SetGeneric(BlackBoardKey.Memories_ShortTerm, recentMemories);
        }

        if (existingRecentMemory == null)
        {
            Debug.Log($"단기 기억 추가 : {memoryToAdd.Name}");

            recentMemories.Add(memoryToAdd.Duplicate());
            IndividualBlackboard.SetGeneric(BlackBoardKey.Memories_ShortTerm, recentMemories);
        }
        else
        {
            Debug.Log($"기억 강화 : {memoryToAdd.Name}");

            existingRecentMemory.Reinforce(memoryToAdd);

            if (existingRecentMemory.Occurrences >= LongTermMemoryThreshold)
            {
                permanentMemories.Add(existingRecentMemory);
                recentMemories.Remove(existingRecentMemory);

                IndividualBlackboard.SetGeneric(BlackBoardKey.Memories_ShortTerm, recentMemories);
                IndividualBlackboard.SetGeneric(BlackBoardKey.Memories_LongTerm, permanentMemories);

                Debug.Log($"{existingRecentMemory.Name} 기억은 영구적이에용");
            }
        }
    }
}
