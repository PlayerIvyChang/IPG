using UnityEngine;
using UnityEngine.InputSystem.Android;
using Unity.Cinemachine;

public class CameraController : MonoBehaviour
{
    private const float MIN_FOLLOW_Y_OFFSET = 2f;
    private const float MAX_FOLLOW_Y_OFFSET = 12f;
    
    [SerializeField] private CinemachineCamera followCamera;

    private CinemachineFollow followComponent;
    private Vector3 targetFollowOffset;

    private void Start()
    {
        followComponent = followCamera.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachineFollow;
        
        if (followComponent != null)
        {
            targetFollowOffset = followComponent.FollowOffset;
            
            if (followCamera.Follow != null)
            {
                transform.position = followCamera.Follow.position;
            }
        }
    }
    
    private void Update()
    {
        HandleMovement(Time.deltaTime);
        HandleRotation(Time.deltaTime);
        HandleZoom(Time.deltaTime);
    }
    
    private void HandleMovement(float deltaTime)
    {
        Vector3 inputMoveDirection = new Vector3(0, 0, 0);
        if(Input.GetKey(KeyCode.W))
        {
            inputMoveDirection.z = -1f;
        }
        if(Input.GetKey(KeyCode.S))
        {
            inputMoveDirection.z = +1f;
        }
        if(Input.GetKey(KeyCode.A))
        {
            inputMoveDirection.x = +1f;
        }
        if(Input.GetKey(KeyCode.D))
        {
            inputMoveDirection.x = -1f;
        }

        float moveSpeed = 10f;
        Vector3 moveDirection = transform.forward * inputMoveDirection.z + transform.right * inputMoveDirection.x;
        transform.position += moveDirection * moveSpeed * deltaTime;
    }

    private void HandleRotation(float deltaTime)
    {
        Vector3 rotationVector = Vector3.zero;
        if (Input.GetKey(KeyCode.Q))
        {
            rotationVector.y = -1f;
        }
        if (Input.GetKey(KeyCode.E))
        {
            rotationVector.y = 1f;
        }

        float rotationSpeed = 80f;
        transform.eulerAngles += Vector3.Lerp(Vector3.zero, rotationVector, rotationSpeed * deltaTime);
    }
    
    private void HandleZoom(float deltaTime)
    {
        if (followComponent == null) return;

        float zoomAmount = 1f;
        
        if (Input.mouseScrollDelta.y > 0)
        {
            targetFollowOffset.y -= zoomAmount;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            targetFollowOffset.y += zoomAmount;
        }
        
        // 限制 Y 轴范围(相机高度)
        targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y, MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET);
        
        // 快速平滑插值
        float zoomSpeed = 5f;
        followComponent.FollowOffset = Vector3.Lerp(
            followComponent.FollowOffset, 
            targetFollowOffset, 
            deltaTime * zoomSpeed
        );
    }
}
