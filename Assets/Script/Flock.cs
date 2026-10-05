using UnityEngine;

public class Flock : MonoBehaviour
{
    float speed;
    private float randomSpeedMultiplier;
    bool turning = false;

    void Start()
    {
        // Cada peixe começa com uma variação própria
        randomSpeedMultiplier = Random.Range(0.8f, 1.2f);

        speed = Random.Range(
            FlockManager.FM.minSpeed,
            FlockManager.FM.maxSpeed
        );

        speed *= randomSpeedMultiplier;
    }

    void Update()
    {
        Bounds b = new Bounds(FlockManager.FM.transform.position, FlockManager.FM.swimLimits * 2);
        // Ocasionalmente escolhe uma nova velocidade

        if (!b.Contains(transform.position))
        {
            turning = true;
        }
        else
        {
            turning = false;
        }

        if (turning)
        {
            Vector3 direction = FlockManager.FM.transform.position - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), FlockManager.FM.rotationSpeed * Time.deltaTime);
        }


        if (Random.Range(0, 100) < 10)
        {
            speed = Random.Range(
                FlockManager.FM.minSpeed,
                FlockManager.FM.maxSpeed
            );

            speed *= randomSpeedMultiplier;
        }

        // Segurança: nunca deixar a velocidade cair demasiado
        speed = Mathf.Clamp(
            speed,
            FlockManager.FM.minSpeed,
            FlockManager.FM.maxSpeed
        );

        ApplyRules();

        // Avança na direção atual
        transform.Translate(
            Vector3.forward * speed * Time.deltaTime
        );
    }

    void ApplyRules()
    {
        GameObject[] gos = FlockManager.FM.allFish;

        Vector3 vcentre = Vector3.zero;
        Vector3 vavoid = Vector3.zero;
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
                vcentre += go.transform.position;
                alignment += go.transform.forward;

                groupSize++;

                // Separação
                if (distance < 1.0f)
                {
                    vavoid +=
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
            vcentre /= groupSize;
            alignment /= groupSize;

            // Calcula a velocidade média dos vizinhos
            float averageGroupSpeed =
                groupSpeed / groupSize;

            // Aproxima suavemente a velocidade do peixe
            // da velocidade média do grupo.
            // NÃO multiplica aleatoriamente a cada frame.
            speed = Mathf.Lerp(
                speed,
                averageGroupSpeed,
                0.05f
            );

            // Garante que nunca fica abaixo/acima dos limites
            speed = Mathf.Clamp(
                speed,
                FlockManager.FM.minSpeed,
                FlockManager.FM.maxSpeed
            );

            // Coesão
            direction +=
                (vcentre - transform.position) * 0.5f;

            // Separação
            direction +=
                vavoid * 1.5f;

            // Alinhamento
            direction +=
                alignment * 0.5f;
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