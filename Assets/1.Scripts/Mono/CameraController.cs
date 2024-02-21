using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraController : MonoBehaviour
{
    //카메라 사이즈 변경을 통한 줌인 줌아웃 구현 (참고: https://error37.tistory.com/3)
    private const float ZoomSpeed = 1.0f; //한번에 줌 되는 정도
    private const float MinZoomSize = 3.0f; //최소 카메라 사이즈
    private const float MaxZoomSize = 10.0f; //최대 카메라 사이즈

    private float targetZoomSize; //목표 카메라 크기

    private Camera _camera; //카메라 컴포넌트 받아오기

    // Start is called before the first frame update
    private void Start()
    {
        _camera = GetComponent<Camera>();
        targetZoomSize = _camera.orthographicSize;
    }

    // Update is called once per frame
    private void Update()
    {
        ControllerZoom();

        UpdateZoom();
    }

    private void ControllerZoom()
    {

        var scrollInput = Input.GetAxis("Mouse ScrollWheel"); //마우스 스크롤 입력받기
        var hasScrollInput = Mathf.Abs(scrollInput) > Mathf.Epsilon;
        if (!hasScrollInput)
        {
            return;
        }
        var newSize = _camera.orthographicSize - scrollInput * ZoomSpeed; //기존 카메라 사이즈 - 스크롤 입력값 * ZoomSpeed
        targetZoomSize = Mathf.Clamp(newSize, MinZoomSize, MaxZoomSize); //카메라 크기 값을 최소값과 최대값 사이로 유지
    }

    private void UpdateZoom()
    {
        if (Mathf.Abs(targetZoomSize - _camera.orthographicSize) < Mathf.Epsilon)
        {
            return;
        }

        var mouseWorldPos = _camera.ScreenToWorldPoint(Input.mousePosition);
        var cameraTransform = transform;
        var currentCameraPosition = cameraTransform.position;
        var offsetCamera = mouseWorldPos - currentCameraPosition - (mouseWorldPos - currentCameraPosition) / (_camera.orthographicSize / targetZoomSize);

        _camera.orthographicSize = targetZoomSize; // 카메라 크기 업데이트
        currentCameraPosition += offsetCamera; // 카메라 위치 조정
        cameraTransform.position = currentCameraPosition;

    }
}
