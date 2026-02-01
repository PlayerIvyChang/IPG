using UnityEngine;

public class Blade : MonoBehaviour
{
    private Collider bladeCollider;
    private bool slicing;
    private Camera mainCamera;
    private TrailRenderer bladeTrail;
    public float sliceForce = 8f;

    public Vector3 direction { get; private set; }

    private void Awake()
    {
        mainCamera = Camera.main;
        bladeCollider = GetComponent<Collider>();
        bladeCollider.enabled = false;
        bladeTrail = GetComponentInChildren<TrailRenderer>();
    }

    private void OnEnable()
    {
        StopSlicing();
    }
    private void OnDisable()
    {
        StopSlicing();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartSlicing();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopSlicing();
        }
        else if (slicing)
        {
            ContinueSlicing();
        }
    }
    private void StartSlicing()
    {
        Vector3 newposition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        newposition.z = 0f;

        transform.position = newposition;

        slicing = true;
        bladeCollider.enabled = true;
        bladeTrail.enabled = true;
    }
    private void StopSlicing()
    {
        slicing = false;
        bladeCollider.enabled = false;
        bladeTrail.enabled = false;
        bladeTrail.Clear();
    }

    private void ContinueSlicing()
    {
        // Get mouse position and direction
        Vector3 newposition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        newposition.z = 0f;
        direction = newposition - transform.position;

        // Get velocity
        float velocity = direction.magnitude / Time.deltaTime;
        bladeCollider.enabled = velocity > 0.01f;

        // In case the blade stays in the same position
        transform.position = newposition;
    }
}
