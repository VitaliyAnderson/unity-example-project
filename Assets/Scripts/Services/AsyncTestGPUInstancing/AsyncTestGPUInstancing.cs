using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class AsyncTestGPUInstancing : IServiceScoped, IDisposable
{
    private readonly UnityEventsService _unityEventsService;

    private readonly System.Random _random = new();

    private readonly CancellationTokenSource _cts = new();

    private readonly Task _task;

    // Settings
    private const int _count = 500000;

    // Models
    private Mesh _instanceMesh;
    private Material _instanceMaterial;

    // Args
    private readonly uint[] _args = { 0, 0, 0, 0, 0 };
    private readonly ComputeBuffer _argsBuffer;

    // Buffers
    private (Vector3[] Pos, Color[] Col) _renderBuf = (new Vector3[_count], new Color[_count]);
    private (Vector3[] Pos, Color[] Col) _tempBuf = (new Vector3[_count], new Color[_count]);

    // Box and move
    private readonly Vector3 _boxMin = new(0f, 5f, 0f);
    private readonly Vector3 _boxMax = new(100f, 55f, 100f);
    private readonly Vector3[] _velocities = new Vector3[_count];

    private volatile bool _readyToSwitch = false;

    private readonly ComputeBuffer _posBuffer;
    private readonly ComputeBuffer _colBuffer;

    // Stuff
    private readonly float[] _hue = new float[_count];
    private bool _initialized;
    private double _currentTime;
    private float _deltaTime;
    private double _lastUpdateTime;
    private int _i;

    public AsyncTestGPUInstancing(
        UnityEventsService unityEventsService)
    {
        _unityEventsService = unityEventsService;

        _task = Task.Run(() => TaskJob(_cts.Token));
        
        _argsBuffer = new(1, sizeof(uint) * 5, ComputeBufferType.IndirectArguments);
        _posBuffer = new(_count, sizeof(float) * 3, ComputeBufferType.Default);
        _colBuffer = new(_count, sizeof(float) * 4, ComputeBufferType.Default);

        _unityEventsService.Events.OnStart += OnStart;
        _unityEventsService.Events.OnUpdate += OnUpdate;
    }

    public void Dispose()
    {
        _cts.Cancel();

        _task.Wait();

        _argsBuffer.Dispose();
        _posBuffer.Dispose();
        _colBuffer.Dispose();

        _unityEventsService.Events.OnStart-= OnStart;
        _unityEventsService.Events.OnUpdate -= OnUpdate;
    }

    private void OnStart()
    {
        _instanceMesh = Resources.Load<Mesh>("GPUInstancing/Mesh");
        _instanceMaterial = Resources.Load<Material>("GPUInstancing/Material");
        _instanceMaterial.SetBuffer("_InstancePositions", _posBuffer);
        _instanceMaterial.SetBuffer("_InstanceColors", _colBuffer);
        _args[0] = _instanceMesh.GetIndexCount(0);
        _args[1] = _count;
        _args[2] = _instanceMesh.GetIndexStart(0);
        _args[3] = _instanceMesh.GetBaseVertex(0);
        _args[4] = 0;
        _argsBuffer.SetData(_args);
    }

    private void OnUpdate(float deltaTime)
    {
        if (_readyToSwitch)
        {
            (_renderBuf, _tempBuf) = (_tempBuf, _renderBuf);

            _posBuffer.SetData(_renderBuf.Pos);
            _colBuffer.SetData(_renderBuf.Col);

            _readyToSwitch = false;
        }

        Graphics.DrawMeshInstancedIndirect(_instanceMesh, 0, _instanceMaterial, new Bounds(Vector3.zero, Vector3.one * 1000), _argsBuffer);
    }

    public async Task TaskJob(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(10, cancellationToken);

                UpdateBuffers();
            }
            catch
            {
            }
        }
    }

    private void UpdateBuffers()
    {
        if (!_readyToSwitch)
        {
            _currentTime = DateTime.UtcNow.Ticks * 1e-7;
            _deltaTime = _lastUpdateTime > 0 ? (float)(_currentTime - _lastUpdateTime) : 0.016f;
            _lastUpdateTime = _currentTime;

            if (!_initialized)
            {
                Init();

                _initialized = true;
            }
            else
            {
                //CalculatePositionAndColorTest1();
                //CalculatePositionAndColorTest2();
                CalculatePositionAndColorTest3();
            }

            _readyToSwitch = true;
        }
    }

    private void Init()
    {
        for (_i = 0; _i < _count; _i++)
        {
            _tempBuf.Pos[_i].x = (float)(_random.NextDouble() * (_boxMax.x - _boxMin.x) + _boxMin.x);
            _tempBuf.Pos[_i].y = (float)(_random.NextDouble() * (_boxMax.y - _boxMin.y) + _boxMin.y);
            _tempBuf.Pos[_i].z = (float)(_random.NextDouble() * (_boxMax.z - _boxMin.z) + _boxMin.z);
            _velocities[_i].x = (float)(_random.NextDouble() * 4.0f - 2.0f);
            _velocities[_i].y = (float)(_random.NextDouble() * 4.0f - 2.0f);
            _velocities[_i].z = (float)(_random.NextDouble() * 4.0f - 2.0f);
            _hue[_i] = (float)_random.NextDouble();
            HSVToRGBFast(ref _tempBuf.Col[_i], _hue[_i]);
        }
    }

    private void CalculatePositionAndColorTest1()
    {
        //unsafe
        //{
        //    fixed (Vector3* _renderBufPos = _renderBuf.Pos)
        //    fixed (Vector3* _tempBufPos = _tempBuf.Pos)
        //    fixed (Vector3* velocities = _velocities)
        //    fixed (float* hues = _hue)
        //    fixed (Color* colors = _tempBuf.Col)
        //    {
        //        Vector3 boxMin = _boxMin;
        //        Vector3 boxMax = _boxMax;

        //        for (int i = 0; i < _count; i++)
        //        {
        //            // Интеграция
        //            _tempBufPos[i].x = _renderBuf.Pos[i].x + velocities[i].x * _deltaTime;
        //            _tempBufPos[i].y = _renderBuf.Pos[i].y + velocities[i].y * _deltaTime;
        //            _tempBufPos[i].z = _renderBuf.Pos[i].z + velocities[i].z * _deltaTime;

        //            // Отскок по X
        //            if (_tempBufPos[i].x < boxMin.x || _tempBufPos[i].x > boxMax.x)
        //            {
        //                velocities[i].x = -velocities[i].x;
        //                _tempBufPos[i].x = _tempBufPos[i].x < boxMin.x ? boxMin.x : boxMax.x;
        //            }

        //            // Отскок по Y
        //            if (_tempBufPos[i].y < boxMin.y || _tempBufPos[i].y > boxMax.y)
        //            {
        //                velocities[i].y = -velocities[i].y;
        //                _tempBufPos[i].y = _tempBufPos[i].y < boxMin.y ? boxMin.y : boxMax.y;
        //            }

        //            // Отскок по Z
        //            if (_tempBufPos[i].z < boxMin.z || _tempBufPos[i].z > boxMax.z)
        //            {
        //                velocities[i].z = -velocities[i].z;
        //                _tempBufPos[i].z = _tempBufPos[i].z < boxMin.z ? boxMin.z : boxMax.z;
        //            }

        //            // Плавное изменение hue
        //            float h = hues[i] + 0.005f;
        //            hues[i] = h >= 1.0f ? h - 1.0f : h;

        //            // Быстрый HSV → RGB (s=1, v=1)
        //            HSVToRGBFast(ref colors[i], hues[i]);
        //        }
        //    }
        //}
    }
    private void CalculatePositionAndColorTest2()
    {
        Parallel.For(0, _count, i =>
        {
            _tempBuf.Pos[i].x = _renderBuf.Pos[i].x + _velocities[i].x * _deltaTime;
            _tempBuf.Pos[i].y = _renderBuf.Pos[i].y + _velocities[i].y * _deltaTime;
            _tempBuf.Pos[i].z = _renderBuf.Pos[i].z + _velocities[i].z * _deltaTime;

            if (_tempBuf.Pos[i].x < _boxMin.x || _tempBuf.Pos[i].x > _boxMax.x)
            {
                _velocities[i].x *= -1;
                _tempBuf.Pos[i].x = Mathf.Clamp(_tempBuf.Pos[i].x, _boxMin.x, _boxMax.x);
            }

            if (_tempBuf.Pos[i].y < _boxMin.y || _tempBuf.Pos[i].y > _boxMax.y)
            {
                _velocities[i].y *= -1;
                _tempBuf.Pos[i].y = Mathf.Clamp(_tempBuf.Pos[i].y, _boxMin.y, _boxMax.y);
            }

            if (_tempBuf.Pos[i].z < _boxMin.z || _tempBuf.Pos[i].z > _boxMax.z)
            {
                _velocities[i].z *= -1;
                _tempBuf.Pos[i].z = Mathf.Clamp(_tempBuf.Pos[i].z, _boxMin.z, _boxMax.z);
            }

            _hue[i] = _hue[i] > 1.0f ? _hue[i] - 1.0f : _hue[i] + 0.005f;
            HSVToRGBFast(ref _tempBuf.Col[i], _hue[i]);
        });
    }
    private void CalculatePositionAndColorTest3()
    {
        for (_i = 0; _i < _count; _i++)
        {
            _tempBuf.Pos[_i].x = _renderBuf.Pos[_i].x + _velocities[_i].x * _deltaTime;
            _tempBuf.Pos[_i].y = _renderBuf.Pos[_i].y + _velocities[_i].y * _deltaTime;
            _tempBuf.Pos[_i].z = _renderBuf.Pos[_i].z + _velocities[_i].z * _deltaTime;

            if (_tempBuf.Pos[_i].x < _boxMin.x || _tempBuf.Pos[_i].x > _boxMax.x)
            {
                _velocities[_i].x *= -1;
                _tempBuf.Pos[_i].x = Mathf.Clamp(_tempBuf.Pos[_i].x, _boxMin.x, _boxMax.x);
            }

            if (_tempBuf.Pos[_i].y < _boxMin.y || _tempBuf.Pos[_i].y > _boxMax.y)
            {
                _velocities[_i].y *= -1;
                _tempBuf.Pos[_i].y = Mathf.Clamp(_tempBuf.Pos[_i].y, _boxMin.y, _boxMax.y);
            }

            if (_tempBuf.Pos[_i].z < _boxMin.z || _tempBuf.Pos[_i].z > _boxMax.z)
            {
                _velocities[_i].z *= -1;
                _tempBuf.Pos[_i].z = Mathf.Clamp(_tempBuf.Pos[_i].z, _boxMin.z, _boxMax.z);
            }

            _hue[_i] = _hue[_i] > 1.0f ? _hue[_i] - 1.0f : _hue[_i] + 0.005f;
            HSVToRGBFast(ref _tempBuf.Col[_i], _hue[_i]);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void HSVToRGBFast(ref Color color, float h)
    {
        h -= (int)h;
        float hh = h * 6.0f;
        int i = (int)hh;
        float f = hh - i;
        float p = 0.0f;
        float q = 1.0f - f;
        float t = f;
        switch (i)
        {
            case 0: color.r = 1; color.g = t; color.b = p; break;
            case 1: color.r = q; color.g = 1; color.b = p; break;
            case 2: color.r = p; color.g = 1; color.b = t; break;
            case 3: color.r = p; color.g = q; color.b = 1; break;
            case 4: color.r = t; color.g = p; color.b = 1; break;
            case 5: color.r = 1; color.g = p; color.b = q; break;
            default: color.r = 1; color.g = 0; color.b = 0; break;
        }
    }
}
