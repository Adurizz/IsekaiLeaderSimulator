using UnityEngine;

public class PlayerDetectorForTrainingCamp : MonoBehaviour
{
    private TrainingCamp trainingCamp;

    private void Awake()
    {
        trainingCamp = GetComponentInParent<TrainingCamp>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            trainingCamp.OnReachTrainingCamp();
        }
    }
}
