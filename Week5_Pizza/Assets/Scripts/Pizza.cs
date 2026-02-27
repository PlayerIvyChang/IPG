using UnityEngine;

public class Pizza : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float floatAmplitude = 0.5f;
    [SerializeField] private float floatFrequency = 1f;

    [Header("Collection")]
    [SerializeField] private AudioClip collectSound;
    [SerializeField] private GameObject collectEffect;

    private Vector3 startPosition;
    private float timeOffset;

    private void Start()
    {
        startPosition = transform.position;
        timeOffset = Random.Range(0f, 2f * Mathf.PI);
    }

    private void Update()
    {
        RotatePizza();
        FloatPizza();
    }

    private void RotatePizza()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
    }

    private void FloatPizza()
    {
        float newY = startPosition.y + Mathf.Sin((Time.time + timeOffset) * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CollectPizza();
        }
    }

    private void CollectPizza()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddPizza(1);
        }

        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }

        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
