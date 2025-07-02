using Unity.Burst;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Jobs;

public class TransformJobMover : MonoBehaviour
{
    [Header("Настройки спавна")]
    public GameObject cubePrefab;
    public int cubeCount = 100;
    public Vector2 areaSize = new Vector2(20, 20);
    public float speed = 2f;

    private TransformAccessArray transformAccessArray;

    void Start()
    {
        transformAccessArray = new TransformAccessArray(cubeCount);

        for (int i = 0; i < cubeCount; i++)
        {
            Vector3 spawnPos = new Vector3(
                Random.Range(-areaSize.x / 2f, areaSize.x / 2f),
                0f,
                Random.Range(-areaSize.y / 2f, areaSize.y / 2f)
            );

            GameObject obj = Instantiate(cubePrefab, spawnPos, Quaternion.identity);
            transformAccessArray.Add(obj.transform);
        }
    }

    void Update()
    {
        var job = new MoveTransformJob
        {
            deltaTime = Time.deltaTime,
            speed = speed
        };

        JobHandle handle = job.Schedule(transformAccessArray);
        handle.Complete();
    }

    void OnDestroy()
    {
        if (transformAccessArray.isCreated)
            transformAccessArray.Dispose();
    }

    [BurstCompile]
    struct MoveTransformJob : IJobParallelForTransform
    {
        public float deltaTime;
        public float speed;

        public void Execute(int index, TransformAccess transform)
        {
            Vector3 pos = transform.position;
            pos.x += speed * deltaTime;
            transform.position = pos;
        }
    }
}
