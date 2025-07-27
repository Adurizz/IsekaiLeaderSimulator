using UnityEngine;

public class EnemyArrowController : ProjectileController
{
    [SerializeField] private EnemySpawnManager enemySpawnManager;
    private float curLife;
    private float lifeTime = 5f;

    public void SetEnemySpawnManager(EnemySpawnManager enemySpawnManager)
    {
        this.enemySpawnManager = enemySpawnManager;
    }

    private void Update()
    {
        curLife += Time.deltaTime;
        if (curLife >= lifeTime)
        {
            curLife = 0;
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Company"))
        {
            other.gameObject.GetComponent<Actor>().GetDamage(damage);
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        enemySpawnManager.EnqueueArrow(gameObject);
    }
}
