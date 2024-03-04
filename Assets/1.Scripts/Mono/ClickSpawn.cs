using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using _1.Scripts.DOTS.Authoring_baker_; //SampleSpawnData 컴포넌트를 불러오기 위함
using _1.Scripts.DOTS.Components___Tags;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

public class ClickSpawn : MonoBehaviour
{
    public float distance = 0.1f;

    private Entity _targetEntity;
    private Camera MainCamera;
    private EntityManager _entityManager;
    private Entity _spawnerEntity;
    private Entity _sampleUnitEntity;
    private int _spawnNum;
    // private int _toggleValue;

    private void Awake()
    {
        MainCamera = Camera.main;
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    private void LateUpdate()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Vector3 mousePosition;
            mousePosition = MainCamera.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, 10f));

            _targetEntity = SelectEntity(mousePosition);
            //Debug.Log($"{_targetEntity.ToString()}");
        }

        if (_targetEntity == Entity.Null)
        {
            return;
        }

        _spawnerEntity = _entityManager.CreateEntityQuery(typeof(SampleSpawnData)).GetSingletonEntity();
        _sampleUnitEntity = _entityManager.GetComponentData<SampleSpawnData>(_spawnerEntity).SampleEntityPrefab;
        _spawnNum = _entityManager.GetComponentData<SampleSpawnData>(_spawnerEntity).number;
        //_toggleValue = _entityManager.GetComponentData<WhattoSpawn>(_spawnerEntity).ToggleValue;

        /*
        if (_toggleValue == 0)
        {
            return;
        }
        */

        var SampleUnits = new NativeArray<Entity>(_spawnNum, Allocator.Temp);
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        ecb.Instantiate(_sampleUnitEntity, SampleUnits);

        int x = 0;
        int y = (int)_entityManager.GetComponentData<LocalTransform>(_targetEntity).Position.y;
        int newteam = 0;
        foreach (var SampleUnit in SampleUnits)
        {
            ecb.SetComponent(SampleUnit, new SampleUnitComponentData
            {
                index = new int2(x, y),
                hp = 3,
                movementspeed = 0.2f,
                dmg = 1,
                team = newteam
            });
            ecb.SetComponentEnabled<MovingTag>(SampleUnit, false);
            ecb.SetComponentEnabled<AttackTag>(SampleUnit, false);
            ecb.SetComponentEnabled<LazyTag>(SampleUnit, false);

            ecb.SetComponent(SampleUnit, new LocalTransform()
            {
                Position = new float3(x, y, 0),
                Scale = 5
            });
            if (x != 100)
            {
                x++;
            }
            else
            {
                break;
            }
            if (x > 50)
            {
                newteam = 1;
            }

        }
        ecb.Playback(_entityManager);


    }

    private Entity SelectEntity(Vector3 mousePosition)
    {
        //if (MainCamera.Instance == null) return Entity.Null;
        //var mainCam = MainCamera.Instance.Camera;
        //Debug.Log(mousePosition);
        // var ray = MainCamera.ScreenToWorldPoint(mousePosition);//var ray = MainCamera.ScreenPointToRay(mousePosition);
        //Debug.Log($"{ray.ToString()}");
        Entity closestTile = Entity.Null;

        // RaycastHit2D hit = Physics2D.Raycast(ray, transform.forward, 50f);
        // Debug.DrawRay(mousePosition, transform.forward * 10, Color.red, 0.3f);
        // if (!hit)
        // {
        //     Debug.Log("분기 발동");
        //     return Entity.Null;
        // }
        // else
        //     Debug.Log("정상 클릭");


        var query = _entityManager.CreateEntityQuery(typeof(MapTileAuthoringComponentData));

        var minSqrDist = distance * distance;
        foreach (var entity in query.ToEntityArray(Allocator.Temp))
        {

            var entityPos = _entityManager.GetComponentData<LocalTransform>(entity).Position;

            var dist = (mousePosition - (Vector3)entityPos).sqrMagnitude;
            if (dist < minSqrDist)
            {
                //Debug.Log(entity);
                closestTile = entity;
                break;
            }
        }

        return closestTile;
    }
}
