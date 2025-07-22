using System.Collections.Generic;
using UnityEngine;

public class GuardianSkill : Skill
{
    [SerializeField] private List<int> skillAdjustAmount = new();
    private Soldier soldierScript;
    [SerializeField] private float guardRange = 8f;
    [SerializeField] private float healAmount = 5f;
    [SerializeField] private bool mastered;
    private float curTime;
    private float healCoolTime = 1f;
    [SerializeField] private ParticleSystem areaHealEffect;

    protected override void Awake()
    {
        base.Awake();
        mastered = false;
        soldierScript = companionScript as Soldier;
    }

    protected override void SetSkillID()
    {
        skillID = (int)ESoldierSkill.Guardian;
    }

    protected override void AdjsustSkillLevel(List<int> skillLevels)
    {
        base.AdjsustSkillLevel(skillLevels);

        if (level >= 1)
        {
            companionScript.UpgradeMaxHP(10);
        }
        if (level >= 2)
        {
            companionScript.UpgradeMaxHP(20);
        }
        if (level >= 3)
        {
            companionScript.UpgradeMaxHP(30);
            mastered = true;
            ActivateAreaHealEffect();
        }
    }

    private void Update()
    {
        if (!mastered)
            return;
        GiveAreaHealWithInterval();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, guardRange);
    }

    private void ActivateAreaHealEffect()
    {
        areaHealEffect.gameObject.SetActive(true);
        areaHealEffect.Play();
    }

    public void DeactivateAreaHealEffect()
    {
        areaHealEffect.gameObject.SetActive(false);
    }

    private void GiveAreaHealWithInterval()
    {
        curTime += Time.deltaTime;
        if (curTime >= healCoolTime)
        {
            AreaHeal();
            curTime = 0;
        }
    }

    private void AreaHeal()
    {
        List<Hero> nearbyHeroList = FindAllNearbyHero();

        foreach (Hero hero in nearbyHeroList)
        {
            hero.GetHeal(healAmount);
            hero.GotHealedEffect.Play();
        }
    }

    private List<Hero> FindAllNearbyHero()
    {
        List<Hero> temp = new();
        Collider[] colliders = Physics.OverlapSphere(transform.position, guardRange, heroLayer);
        if (colliders.Length == 0)
        {
            return null;
        }

        foreach (Collider col in colliders)
        {
            Hero nearbyHero = col.GetComponent<Hero>();
            if (nearbyHero.IsDead)
                continue;

            temp.Add(nearbyHero);
        }

        return temp;
    }

    /*
    private void ChooseGuardTarget()
    {

    }

    private void GetMouseInput()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {

        }
#endif
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                RaycastHit hit;
                var ray = Camera.main.ScreenPointToRay(touch.position);

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject.GetComponent<Actor>() != null 
                        && hit.collider.gameObject.GetComponent<Actor>() is Soldier)
                    {

                    }
                }
            }
        }
    }
    */
}
