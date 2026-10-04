using UnityEngine;

public class FlockManager : MonoBehaviour
{
    public static FlockManager FM;

    [Header("Fish")]
    public GameObject fishPrefab;
    public int numFish = 20;
    public GameObject[] allFish;

    [Header("Swimming Area")]
    public Vector3 swimLimits = new Vector3(5, 5, 5);

    [Header("Fish Settings")]
    [Range(0.0f, 5.0f)]
    public float minSpeed = 1.0f;

    [Range(0.0f, 5.0f)]
    public float maxSpeed = 3.0f;

    [Range(1.0f, 10.0f)]
    public float neighbourDistance = 3.0f;

    [Range(0.0f, 10.0f)]
    public float rotationSpeed = 4.0f;

    void Awake()
    {
        FM = this;
    }

    void Start()
    {
        if (fishPrefab == null)
        {
            Debug.LogError("ERRO: Não existe nenhum Fish Prefab no FlockManager!");
            return;
        }

        Debug.Log("Vou criar " + numFish + " peixes.");

        allFish = new GameObject[numFish];

        for (int i = 0; i < numFish; i++)
        {
            Vector3 pos = transform.position + new Vector3(
                Random.Range(-swimLimits.x, swimLimits.x),
                Random.Range(-swimLimits.y, swimLimits.y),
                Random.Range(-swimLimits.z, swimLimits.z)
            );

            Quaternion rotation = Quaternion.Euler(
                0,
                Random.Range(0f, 360f),
                0
            );

            allFish[i] = Instantiate(
                fishPrefab,
                pos,
                rotation
            );

            Debug.Log("Peixe " + i + " criado em " + pos);
        }
    }
}