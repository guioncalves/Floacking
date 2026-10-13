
using UnityEngine;
using UnityEngine.AI;

public class AIControlSingle : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;
    private GameObject[] goalLocations;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        goalLocations = GameObject.FindGameObjectsWithTag("goal");

        if (agent == null || goalLocations.Length == 0)
        {
            Debug.LogWarning("NavMeshAgent ou destinos em falta!");
            enabled = false;
            return;
        }

        if (anim != null)
        {
            anim.SetTrigger("isWalking");
        }

        ChooseDestination();
    }

    void Update()
    {
        if (!agent.pathPending && agent.remainingDistance < 1f)
        {
            ChooseDestination();
        }
    }

    void ChooseDestination()
    {
        int i = Random.Range(0, goalLocations.Length);

        agent.SetDestination(
            goalLocations[i].transform.position
        );
    }
}
