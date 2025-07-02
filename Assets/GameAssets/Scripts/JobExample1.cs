using Unity.Burst;
using Unity.Jobs;
using UnityEngine;

public class JobExample1 : MonoBehaviour
{
    [BurstCompile]
    struct HelloJob : IJob
    {
        public void Execute()
        {
        }
    }

    void Start()
    {
        HelloJob job = new HelloJob();
        JobHandle handle = job.Schedule();
        handle.Complete();

        Debug.Log("Job complete!");
    }
}