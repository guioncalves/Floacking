using UnityEngine;

public class Flock : MonoBehaviour
{
    float speed;

    void Start()
    {
        speed = Random.Range(
            FlockManager.FM.minSpeed,
            FlockManager.FM.maxSpeed
        );
    }

    void Update()
    {
        ApplyRules();

        // Faz o peixe avançar na direção para onde está virado
        transform.Translate(
            Vector3.forward * speed * Time.deltaTime
        );
    }

    void ApplyRules()
    {
        GameObject[] gos = FlockManager.FM.allFish;

        Vector3 centre = Vector3.zero;
        Vector3 avoid = Vector3.zero;
        Vector3 alignment = Vector3.zero;

        float groupSpeed = 0f;
        int groupSize = 0;

        foreach (GameObject go in gos)
        {
            if (go == null || go == gameObject)
                continue;

            float distance = Vector3.Distance(
                go.transform.position,
                transform.position
            );

            // Só considera peixes próximos
            if (distance <= FlockManager.FM.neighbourDistance)
            {
                centre += go.transform.position;
                alignment += go.transform.forward;

                groupSize++;

                // Evita que os peixes fiquem em cima uns dos outros
                if (distance < 1.0f)
                {
                    avoid += transform.position - go.transform.position;
                }

                Flock otherFish = go.GetComponent<Flock>();

                if (otherFish != null)
                {
                    groupSpeed += otherFish.speed;
                }
            }
        }

        if (groupSize > 0)
        {
            // Centro médio dos vizinhos
            centre /= groupSize;

            // Direção média dos vizinhos
            alignment /= groupSize;

            // Velocidade média
            speed = groupSpeed / groupSize;

            // COESÃO + SEPARAÇÃO + ALINHAMENTO
            Vector3 direction =
                (centre - transform.position)
                + avoid
                + alignment;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation =
                    Quaternion.LookRotation(direction);

                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    FlockManager.FM.rotationSpeed * Time.deltaTime
                );
            }
        }
    }
}