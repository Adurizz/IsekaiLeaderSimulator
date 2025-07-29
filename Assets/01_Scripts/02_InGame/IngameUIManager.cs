using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngameUIManager : Singleton<IngameUIManager>
{
    [SerializeField] private Player player;
    [SerializeField] private GameObject companionHirePanel;
    [SerializeField] private GameObject trainingCampPanel;
    private ExpeditionManager expeditionManager;
    [SerializeField] private List<GameObject> trainingOptions;
    [SerializeField] private SkillInfo[] curSelectedSkillInfos = new SkillInfo[3];
    private TrainingCamp trainingCamp;
    [SerializeField] private TextMeshProUGUI haveGoldText;
    [SerializeField] private TextMeshProUGUI earnedGoldText;
    [SerializeField] private TextMeshProUGUI ownStoneText;
    [SerializeField] private TextMeshProUGUI ownStoneTextInTrainingCamp;
    private CompanionSpawnManager companionSpawnManager;
    [SerializeField] private TextMeshProUGUI spawnTargetClass;
    [SerializeField] private TextMeshProUGUI spawnCostText;
    [SerializeField] private Jun_TweenRuntime notEnoughStoneImg;
    [SerializeField] private Jun_TweenRuntime notEnoughStoneText;
    [SerializeField] private Jun_TweenRuntime notEnoughGoldImg;
    [SerializeField] private Jun_TweenRuntime notEnoughGoldText;

    public void SetCurHaveGoldText(int value)
    {
        haveGoldText.text = value.ToString();
    }

    public void SetCurEarnedGoldText(int value)
    {
        earnedGoldText.text = value.ToString();
    }

    public void SetStoneText(int value)
    {
        ownStoneText.text = value.ToString();
        ownStoneTextInTrainingCamp.text = value.ToString();
    }

    protected override void Awake()
    {
        base.Awake();
        expeditionManager = FindAnyObjectByType<ExpeditionManager>();
        companionSpawnManager = FindAnyObjectByType<CompanionSpawnManager>();
        // player = FindAnyObjectByType<Player>();
        SetCurHaveGoldText(player.GetCurHaveGoldAmount());
        SetCurEarnedGoldText(0);
        SetStoneText(0);
    }

    public void ActivateCompanionHireUI()
    {
        string targetClassText = "";
        switch (companionSpawnManager.CurSpawnTarget)
        {
            case EHeroClass.Soldier:
                targetClassText = "전사";
                break;
            case EHeroClass.Barbarian:
                targetClassText = "야만전사";
                break;
            case EHeroClass.Archor:
                targetClassText = "궁수";
                break;
            case EHeroClass.Wizard:
                targetClassText = "마법사";
                break;
            case EHeroClass.Mechanic:
                targetClassText = "메카닉";
                break;
            case EHeroClass.Druid:
                targetClassText = "드루이드";
                break;
            case EHeroClass.Ninja:
                targetClassText = "닌자";
                break;
        }
        spawnTargetClass.text = "직업: " + targetClassText;
        spawnCostText.text = "비용: " + GlobalValueHolder.companionHireCost + "G";
        companionHirePanel.SetActive(true);
        expeditionManager.StopExpedition();
    }

    public void ActivateTrainingCampUI()
    {
        trainingCampPanel.SetActive(true);
        expeditionManager.StopExpedition();
    }

    public void SetTrainingOptionPanel(int index, string name, SkillInfo skillInfo)
    {
        GameObject targetTrainingOption = trainingOptions[index];
        Image trainingSkillClassImg = targetTrainingOption.transform.Find("Group_Name/Image_Class").GetComponent<Image>();
        TextMeshProUGUI trainingTitle = targetTrainingOption.transform.Find("Group_Name/Text_Title").GetComponent<TextMeshProUGUI>();
        Image trainingIcon = targetTrainingOption.transform.Find("Image_IconBack/Image_Icon").GetComponent<Image>();
        TextMeshProUGUI trainingTarget = targetTrainingOption.transform.Find("Text_Target").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI trainingDescription = targetTrainingOption.transform.Find("Text_Desc").GetComponent<TextMeshProUGUI>();

        trainingSkillClassImg.sprite = skillInfo.classImg;
        trainingTitle.text = skillInfo.skillName;
        trainingIcon.sprite = skillInfo.skillImg;
        trainingTarget.text = "대상: " + name + "(" + skillInfo.ownerClass + ")";
        trainingDescription.text = skillInfo.skillDescription;
    }

    public void OnClickTrainingButton(int index)
    {
        if (trainingCamp == null)
            trainingCamp = FindAnyObjectByType<TrainingCamp>();
        trainingCamp.UpgradeTarget(index);
    }

    public void RevealNotEnoughStoneUI()
    {
        notEnoughStoneImg.gameObject.SetActive(true);
        notEnoughStoneImg.Play();
        notEnoughStoneText.Play();
        Invoke(nameof(UnableNotEnoughStoneUI), 1.2f);
    }

    private void UnableNotEnoughStoneUI()
    {
        notEnoughStoneImg.gameObject.SetActive(false);
    }

    public void RevealNotEnoughGoldUI()
    {
        notEnoughGoldImg.gameObject.SetActive(true);
        notEnoughGoldImg.Play();
        notEnoughGoldText.Play();
        Invoke(nameof(UnableNotEnoughGoldUI), 1.2f);
    }

    private void UnableNotEnoughGoldUI()
    {
        notEnoughGoldImg.gameObject.SetActive(false);
    }
}
