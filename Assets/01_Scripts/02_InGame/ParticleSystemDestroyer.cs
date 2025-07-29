using UnityEngine;

public class ParticleSystemDestroyer : MonoBehaviour
{
    ParticleSystem ps;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (!ps.IsAlive())
        {
            Destroy(gameObject);
        }
    }
}
