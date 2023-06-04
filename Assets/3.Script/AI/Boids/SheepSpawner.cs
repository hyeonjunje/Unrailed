using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepSpawner : MonoBehaviour
{
    public enum GizmoType { Never, SelectedOnly, Always }

    [SerializeField] private FlockBT FlockPrefab;
    [SerializeField] private float _spawnRadius = 10;
    [SerializeField] private int _spawnCount = 10;
    public Color colour;
    public GizmoType showSpawnRegion;

    void Awake()
    {
        for (int i = 0; i < _spawnCount; i++)
        {
            Vector3 pos = transform.position + Random.insideUnitSphere * _spawnRadius;
            FlockBT flock = Instantiate(FlockPrefab);
            flock.transform.position = new Vector3(pos.x, 0.5f, pos.z);
        }
    }

    private void OnDrawGizmos()
    {
        if (showSpawnRegion == GizmoType.Always)
        {
            DrawGizmos();
        }
    }

    void OnDrawGizmosSelected()
    {
        if (showSpawnRegion == GizmoType.SelectedOnly)
        {
            DrawGizmos();
        }
    }

    void DrawGizmos()
    {

        Gizmos.color = new Color(colour.r, colour.g, colour.b, 0.3f);
        Gizmos.DrawSphere(transform.position, _spawnRadius);
    }



}
