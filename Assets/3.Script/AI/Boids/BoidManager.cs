using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    public BoidSettings Settings;
    public ComputeShader Compute;
    public Transform Target;

    const int threadGroupSize = 1024;
    private Boid[] boids;

    void Start()
    {
        boids = FindObjectsOfType<Boid>();
        foreach (Boid b in boids)
        {
            b.Init(Settings, Target);
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
                boidData[i].Position = boids[i].Position;
                boidData[i].Direction = boids[i].Forward;
            }


            //CPU의 작업을 GPU로 전환하는 과정
            var boidBuffer = new ComputeBuffer(numBoids, BoidData.Size);
            boidBuffer.SetData(boidData);
            
            //메모리 공간 설정 (커널 0번에 boids 라는 이름으로 boidBuffer을 넣는다)
            Compute.SetBuffer(0, "boids", boidBuffer);

            //boid의 수
            Compute.SetInt("numBoids", boids.Length);
            
            //무리를 인식하는 범위
            Compute.SetFloat("viewRadius", Settings.PerceptionRadius);

            // 충돌을 피하기 위해 인식하는 범위
            Compute.SetFloat("avoidRadius", Settings.AvoidanceRadius);

            //boid의 수 / 스레드 그룹 사이즈
            //numBoids * numBoids 한 값을 스레드 수로 나눠서 병렬처리 할 수 있게 함
            //나눈 값을 올림한 값 = threadGroups

            int threadGroups = Mathf.CeilToInt(numBoids / (float)threadGroupSize);
            Compute.Dispatch(0, threadGroups, 1, 1);

            // Compute shader 실행 메서드
            // 커널을 하나만 선언했으므로 0
            // 스레드 그룹의 개수는 1024, 1, 1

            boidBuffer.GetData(boidData);

            for (int i = 0; i < boids.Length; i++)
            {
                boids[i].AvgFlockHeading = boidData[i].FlockHeading;
                boids[i].CentreOfFlockmates = boidData[i].FlockCentre;
                boids[i].AvgAvoidanceHeading = boidData[i].AvoidanceHeading;
                boids[i].NumPerceivedFlockmates = boidData[i].NumFlockmates;

                boids[i].UpdateBoid();
            }

            boidBuffer.Release();
        }
    }

    public struct BoidData
    {
        public Vector3 Position;
        public Vector3 Direction;

        public Vector3 FlockHeading;
        public Vector3 FlockCentre;
        public Vector3 AvoidanceHeading;
        public int NumFlockmates;

        public static int Size
        {
            get
            {
                return sizeof(float) * 3 * 5 + sizeof(int);
            }
        }
    }
}
