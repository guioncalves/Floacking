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

        // Avança na direção atual
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

            if (distance <= FlockManager.FM.neighbourDistance)
            {
                centre += go.transform.position;
                alignment += go.transform.forward;

                groupSize++;

                // Evitar outros peixes
                if (distance < 1.0f)
                {
                    avoid +=
                        transform.position - go.transform.position;
                }

                Flock otherFish = go.GetComponent<Flock>();

                if (otherFish != null)
                {
                    groupSpeed += otherFish.speed;
                }
            }
        }

        Vector3 direction = transform.forward;

        if (groupSize > 0)
        {
            centre /= groupSize;
            alignment /= groupSize;

            speed = groupSpeed / groupSize;

            // Coesão
            direction +=
                (centre - transform.position) * 0.5f;

            // Separação
            direction += avoid * 1.5f;

            // Alinhamento
            direction += alignment * 0.5f;
        }

        // ----------------------------------
        // MANTER OS PEIXES NA ZONA
        // ----------------------------------

        Vector3 managerCentre =
            FlockManager.FM.transform.position;

        Vector3 limits =
            FlockManager.FM.swimLimits;

        Vector3 localPosition =
            transform.position - managerCentre;

        bool outside = false;

        if (Mathf.Abs(localPosition.x) > limits.x)
            outside = true;

        if (Mathf.Abs(localPosition.y) > limits.y)
            outside = true;

        if (Mathf.Abs(localPosition.z) > limits.z)
            outside = true;

        // Se estiver a sair da área, vira para o centro
        if (outside)
        {
            direction =
                managerCentre - transform.position;
        }

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