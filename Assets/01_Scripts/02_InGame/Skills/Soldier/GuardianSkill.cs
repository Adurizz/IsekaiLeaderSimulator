using System.Collections.Generic;
using UnityEngine;

public class GuardianSkill : Skill
{
    [SerializeField] private List<int> skillAdjustAmount = new();
    private Soldier soldierScript;
    private float guardRange = 0.3f;
    [SerializeField] private bool mastered;

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
        }
    }

    private void Update()
    {
        if (!mastered)
            return;

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
