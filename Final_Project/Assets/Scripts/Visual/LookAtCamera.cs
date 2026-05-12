using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] bool invert;
    private Transform cameraTransform;
    
    private void Awake()
    {
        cameraTransform = Camera.main.transform;
    }

    private void LateUpdate()
    {
        if (invert)
        {
            transform.LookAt(transform.position - cameraTransform.forward);
        }
        else
        {
            transform.LookAt(transform.position + cameraTransform.forward);
        }
    }
}

