using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    const int threadGroupSize = 1024;

    public BoidSettings settings;
    public ComputeShader compute;
    Boid[] boids;
    public Transform target;

    void Start()
    {
        boids = FindObjectsOfType<Boid>();
        foreach (Boid b in boids)
        {
            b.Initialize(settings, target);
        }

    }

    void Update()
    {
        if (boids != null)
        {

            
            int numBoids = boids.Length;

            //boid들의 위치값, 바라보는 방향을
            //boidData에 넣기
            var boidData = new BoidData[numBoids];

            for (int i = 0; i < boids.Length; i++)
            {
                boidData[i].position = boids[i].position;
                boidData[i].direction = boids[i].forward;
            }


            //CPU의 작업을 GPU로 전환하는 과정
            //대량의 병렬 알고리즘 처리에 특화
            var boidBuffer = new ComputeBuffer(numBoids, BoidData.Size);
            boidBuffer.SetData(boidData);
            
            //메모리 공간 설정 (커널 0번에 boids 라는 이름으로 boidBuffer을 넣는다)
            compute.SetBuffer(0, "boids", boidBuffer);

            //boid의 수
            compute.SetInt("numBoids", boids.Length);
            
            //무리를 인식하는 범위
            compute.SetFloat("viewRadius", settings.perceptionRadius);

            // 충돌을 피하기 위해 인식하는 범위
            compute.SetFloat("avoidRadius", settings.avoidanceRadius);

            //boid의 수 / 스레드 그룹 사이즈
            //numBoids * numBoids 한 값을 스레드 수로 나눠서 병렬처리 할 수 있게 함
            //나눈 값을 올림한 값 = threadGroups

            int threadGroups = Mathf.CeilToInt(numBoids / (float)threadGroupSize);
            compute.Dispatch(0, threadGroups, 1, 1);

            // compute shader 실행 메서드
            // 커널을 하나만 선언했으므로 0
            // 스레드 그룹의 개수는 1024, 1, 1

            boidBuffer.GetData(boidData);

            for (int i = 0; i < boids.Length; i++)
            {
                boids[i].avgFlockHeading = boidData[i].flockHeading;
                boids[i].centreOfFlockmates = boidData[i].flockCentre;
                boids[i].avgAvoidanceHeading = boidData[i].avoidanceHeading;
                boids[i].numPerceivedFlockmates = boidData[i].numFlockmates;

                boids[i].UpdateBoid();
            }

            boidBuffer.Release();
        }
    }

    public struct BoidData
    {
        public Vector3 position;
        public Vector3 direction;

        public Vector3 flockHeading;
        public Vector3 flockCentre;
        public Vector3 avoidanceHeading;
        public int numFlockmates;

        public static int Size
        {
            get
            {
                return sizeof(float) * 3 * 5 + sizeof(int);
            }
        }
    }
}
