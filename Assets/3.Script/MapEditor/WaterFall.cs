using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterFall : MonoBehaviour
{
    [Header("Transform")]
    [SerializeField] private Transform _waterFallUpTransform;
    [SerializeField] private Transform _waterFallSideTransform;
    [SerializeField] private Transform _waterFallTransform;

    [Header("WaveWater")]
    [SerializeField] private Transform _waveWater;
    [SerializeField] private int _waveWaterCount = 10;
    [SerializeField] private float _minPosX = -4.5f;
    [SerializeField] private float _maxPosX = 4.5f;
    [SerializeField] private float _minSclY = 2f;
    [SerializeField] private float _maxSclY = 4f;
    [SerializeField] private float _minSpeed = 1f;
    [SerializeField] private float _maxSpeed = 2f;

    [Header("BrightWater")]
    [SerializeField] private Transform _brightWater;
    [SerializeField] private float _brightWaterCount = 3;
    [SerializeField] private float _minSideValue = -0.45f;
    [SerializeField] private float _maxSideValue = 0.45f;
    [SerializeField] private float _spawnPos = 0;
    [SerializeField] private float _initYPos = -20;
    [SerializeField] private float _waterFallSpeed = -5;
    [SerializeField] private float _minSpawnTime;
    [SerializeField] private float _maxSpawnTime;
    [SerializeField] private float _biggerSpeed = 2f;
    [SerializeField] private Vector3 _originBrightWaterScl;
    [SerializeField] private Vector3 _biggerBrightWaterScl;

    private List<Transform> _waveWaterList = new List<Transform>();
    private List<int> _waveDirs = new List<int>();
    private List<float> _waveSpeeds = new List<float>();

    private Queue<Transform> _brightWaterQueue = new Queue<Transform>();
    private List<Transform> _brightWaterList = new List<Transform>();

    private float _currentTime = 0f;

    private void Awake()
    {
        // 위쪽 폭포 파도 부분 초기화
        for(int i = 0; i < _waveWaterCount; i++)
        {
            Transform waveWater = Instantiate(_waveWater, _waterFallUpTransform);
            waveWater.localPosition += Vector3.right * i;
            waveWater.localScale = new Vector3(1, 10, Random.Range(_minSclY, _maxSclY));
            _waveWaterList.Add(waveWater);
            int dir = Random.Range(0, 2) == 0 ? -1 : 1;
            _waveDirs.Add(dir);
            _waveSpeeds.Add(Random.Range(_minSpeed, _maxSpeed));
        }

        // 사이드 폭포 파도 부분 초기화
        for (int i = 0; i < _waveWaterCount; i++)
        {
            Transform waveWater = Instantiate(_waveWater, _waterFallSideTransform);
            waveWater.localPosition += Vector3.right * i;
            waveWater.localScale = new Vector3(1, 10, Random.Range(_minSclY, _maxSclY));
            _waveWaterList.Add(waveWater);
            int dir = Random.Range(0, 2) == 0 ? -1 : 1;
            _waveDirs.Add(dir);
            _waveSpeeds.Add(Random.Range(_minSpeed, _maxSpeed));
        }

        // 폭포 밝은 부분 풀 초기화
        for (int i = 0; i < _brightWaterCount; i++)
        {
            Transform brightWater = Instantiate(_brightWater, _waterFallTransform);
            brightWater.gameObject.SetActive(false);
            _brightWaterQueue.Enqueue(brightWater);
        }
        _originBrightWaterScl = _brightWaterQueue.Peek().localScale;
        _biggerBrightWaterScl = new Vector3(_originBrightWaterScl.x * 5, _originBrightWaterScl.y, _originBrightWaterScl.z);
    }

    private void Update()
    {
        // 폭포 블럭 일렁이기
        ManageWave();

        // 폭포 밝은 부분 스폰 
        SpawnBrightWater();

        // 폭포 밝은 부분 내려오기
        ManageBrightWater();
    }

    private void ManageWave()
    {
        // 이동
        for(int i = 0; i < _waveWaterList.Count; i++)
        {
            _waveWaterList[i].localScale += Vector3.forward * _waveDirs[i] * _waveSpeeds[i] * Time.deltaTime; 
        }

        // 체크
        for (int i = 0; i < _waveWaterList.Count; i++)
        {
            if(_waveWaterList[i].localScale.z < _minSclY)
            {
                _waveDirs[i] = 1;
                _waveSpeeds[i] = Random.Range(_minSpeed, _maxSpeed);
            }
            else if(_waveWaterList[i].localScale.z > _maxSclY)
            {
                _waveDirs[i] = -1;
                _waveSpeeds[i] = Random.Range(_minSpeed, _maxSpeed);
            }
        }
    }

    private void SpawnBrightWater()
    {
        _currentTime += Time.deltaTime;

        if (_currentTime > Random.Range(_minSpawnTime, _maxSpawnTime))
        {
            if(_brightWaterQueue.Count != 0)
            {
                // 생성
                Transform brightWater = _brightWaterQueue.Dequeue();
                brightWater.gameObject.SetActive(true);
                _brightWaterList.Add(brightWater);

                // Transform 초기화
                float x = Random.Range(_minSideValue, _maxSideValue);
                float z = brightWater.localPosition.z;
                brightWater.localPosition = new Vector3(x, _spawnPos, z);
                brightWater.localScale = _originBrightWaterScl;
            }
            _currentTime = 0;
        }
    }

    private void ManageBrightWater()
    {
        // 위치 체크하기
        for(int i = 0; i < _brightWaterList.Count;)
        {
            if(_brightWaterList[i].localPosition.y < _initYPos)
            {
                Transform brightWater = _brightWaterList[i];
                _brightWaterList.RemoveAt(i);
                _brightWaterQueue.Enqueue(brightWater);
                brightWater.gameObject.SetActive(false);
            }
            else
            {
                i++;
            }
        }

        // 계속 내려가기
        for(int i = 0; i < _brightWaterList.Count; i++)
        {
            Transform brightWater = _brightWaterList[i];
            brightWater.localPosition += Vector3.up * _waterFallSpeed * Time.deltaTime;

            if(brightWater.localScale.x < _biggerBrightWaterScl.x)
            {
                brightWater.localScale = Vector3.Lerp(_originBrightWaterScl, _biggerBrightWaterScl, _currentTime * _biggerSpeed);
            }
        }
    }
}
