using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class JobMover : MonoBehaviour
{
    [Header("Настройки спавна")]
    public GameObject cubePrefab;
    public int cubeCount = 100;
    public Vector2 areaSize = new Vector2(20, 20);

    private Transform[] objects;
    private NativeArray<Vector3> positions;

    void Start()
    {
        // Спавн кубов
        objects = new Transform[cubeCount];
        positions = new NativeArray<Vector3>(cubeCount, Allocator.Persistent);

        for (int i = 0; i < cubeCount; i++)
        {
            Vector3 spawnPos = new Vector3(
                Random.Range(-areaSize.x / 2f, areaSize.x / 2f),
                0f,
                Random.Range(-areaSize.y / 2f, areaSize.y / 2f)
            );

            GameObject obj = Instantiate(cubePrefab, spawnPos, Quaternion.identity);
            objects[i] = obj.transform;
            positions[i] = spawnPos;
        }
    }

    void Update()
    {
        var moveJob = new MoveJob
        {
            deltaTime = Time.deltaTime,
            speed = 2f,
            positions = positions
        };

        JobHandle handle = moveJob.Schedule(cubeCount, 64);
        handle.Complete();

        for (int i = 0; i < cubeCount; i++)
        {
            objects[i].position = positions[i];
        }
    }

    void OnDestroy()
    {
        if (positions.IsCreated)
            positions.Dispose();
    }

    [BurstCompile]
    struct MoveJob : IJobParallelFor
    {
        public float deltaTime;
        public float speed;
        public NativeArray<Vector3> positions;

        public void Execute(int index)
        {
            Vector3 pos = positions[index];
            pos.x += speed * deltaTime;
            positions[index] = pos;
        }
    }
}
