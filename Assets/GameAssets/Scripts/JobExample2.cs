using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class JobExample2 : MonoBehaviour
{
    [BurstCompile]
    struct SquareJob : IJobParallelFor
    {
        public NativeArray<float> numbers;

        public void Execute(int index)
        {
            numbers[index] *= numbers[index];
        }
    }

    void Start()
    {
        var array = new NativeArray<float>(5, Allocator.TempJob);
        for (int i = 0; i < array.Length; i++)
            array[i] = i + 1; // [1, 2, 3, 4, 5]

        var job = new SquareJob { numbers = array };
        var handle = job.Schedule(array.Length, 1); // 1 = batch size
        handle.Complete();

        for (int i = 0; i < array.Length; i++)
            Debug.Log(array[i]); // [1, 4, 9, 16, 25]

        array.Dispose();
    }
}