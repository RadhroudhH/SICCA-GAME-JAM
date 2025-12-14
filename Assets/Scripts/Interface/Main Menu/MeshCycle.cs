using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCycle : MonoBehaviour
{
    [Header("Mesh Settings")]
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private List<Mesh> meshes = new List<Mesh>();

    [Header("Timing")]
    [SerializeField] private float changeInterval = 4f;

    private int currentIndex = 0;

    private void Start()
    {
        if (meshFilter == null)
            meshFilter = GetComponent<MeshFilter>();

        if (meshes.Count > 0)
        {
            meshFilter.mesh = meshes[0];
            StartCoroutine(MeshCycleRoutine());
        }
    }

    private IEnumerator MeshCycleRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(changeInterval);

            currentIndex++;
            if (currentIndex >= meshes.Count)
                currentIndex = 0;

            meshFilter.mesh = meshes[currentIndex];
        }
    }
}
