using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngameUIManager : Singleton<IngameUIManager>
{
    [SerializeField] private GameObject companionHirePanel;
    [SerializeField] private GameObject trainingCampPanel;
    private ExpeditionManager expeditionManager;
    [SerializeField] private List<GameObject> trainingOptions;

    protected override void Awake()
    {
        base.Awake();
        expeditionManager = FindAnyObjectByType<ExpeditionManager>();
    }

    public void ActivateCompanionHireUI()
    {
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
        TextMeshProUGUI trainingTitle = targetTrainingOption.transform.Find("Text_Title").GetComponent<TextMeshProUGUI>();
        Image trainingIcon = targetTrainingOption.transform.Find("Image_Icon").GetComponent<Image>();
        TextMeshProUGUI trainingTarget = targetTrainingOption.transform.Find("Text_Target").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI trainingDescription = targetTrainingOption.transform.Find("Text_Desc").GetComponent<TextMeshProUGUI>();

        trainingTitle.text = skillInfo.skillName;
        trainingIcon.sprite = skillInfo.skillImg;
        trainingTarget.text = "´ë»ó: " + name + "(" + skillInfo.ownerClass + ")";
        trainingDescription.text = skillInfo.skillDescription;
    }
}
