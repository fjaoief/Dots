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
    private Camera MainCamera; //마우스 클릭한 위치를 알아내기 위해 메인 카메라 호출
    private EntityManager _entityManager;
    private Entity _spawnerEntity; //소환할 유닛 엔티티 프리팹 정보가 스포너 엔티티의 SampleSpawnData 컴포넌트에 있기 때문에 불러옴
    private Entity _sampleUnitEntity;
    private int _spawnNum;
    // private int _toggleValue; //추후 여러가지 유닛 소환 기능을 구현할때 사용할 변수

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
            //Debug.Log($"{_targetEntity.ToString()}"); //디버깅을 위한 코드
        }

        if (_targetEntity == Entity.Null)
        {
            return;
        }

        _spawnerEntity = _entityManager.CreateEntityQuery(typeof(SampleSpawnData)).GetSingletonEntity();
        _sampleUnitEntity = _entityManager.GetComponentData<SampleSpawnData>(_spawnerEntity).SampleEntityPrefab;
        _spawnNum = _entityManager.GetComponentData<SampleSpawnData>(_spawnerEntity).number;
        //_toggleValue = _entityManager.GetComponentData<WhattoSpawn>(_spawnerEntity).ToggleValue;

        /* // 체크박스 체크 / 해제 작동 여부를 확인하기 위한 구문
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
        Entity closestTile = Entity.Null; //return할 타일 엔티티 변수 선언

        var query = _entityManager.CreateEntityQuery(typeof(MapTileAuthoringComponentData)); //맵 타일 엔티티 쿼링

        var minSqrDist = distance * distance; //거리 계산을 위해 미리 제곱해주기
        foreach (var entity in query.ToEntityArray(Allocator.Temp)) // 월드에 존재하는 모든 맵 타일 Entity를 가져와 전수조사
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
