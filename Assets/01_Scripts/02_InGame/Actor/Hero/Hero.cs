using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class Hero : Actor
{
    [SerializeField] protected EHeroClass heroClass;
    public EHeroClass HeroClass { get { return heroClass; } }

    [SerializeField] protected ParticleSystem gotHealedEffect;
    public ParticleSystem GotHealedEffect { get { return gotHealedEffect; } }

    #region HP¹Ù
    protected Transform target;
    protected RectTransform canvas;
    protected RectTransform hpBarRectTransform;
    protected Camera mainCam;

    [SerializeField] protected GameObject hpBarPrefab;
    [SerializeField] protected GameObject hpBarGO;
    protected Slider hpSlider;
    protected TextMeshProUGUI nameText;

    protected virtual void Awake()
    {
        hpBarGO = Instantiate(hpBarPrefab);
        target = transform;
        canvas = GameObject.Find("Canvas").GetComponent<RectTransform>();
        hpBarGO.transform.SetParent(canvas);
        hpBarGO.transform.SetSiblingIndex(0);

        hpBarRectTransform = hpBarGO.GetComponent<RectTransform>();
        hpBarRectTransform.localPosition = Vector3.zero;
        hpBarRectTransform.localRotation = Quaternion.Euler(0, 0, 0);
        hpBarRectTransform.localScale = Vector3.one;
        hpSlider = hpBarRectTransform.GetComponent<Slider>();
        mainCam = Camera.main;
        nameText = hpBarGO.GetComponentInChildren<TextMeshProUGUI>();
    }

    protected virtual void Update()
    {
        // Vector3 curPos = target.transform.position;
        
    }

    protected virtual void LateUpdate()
    {
        if (hpBarRectTransform == null)
            return;
        Vector3 curPos = target.transform.position + Vector3.up * 4;
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(curPos);
        hpBarRectTransform.position = Vector3.Lerp(hpBarRectTransform.position, screenPoint, 0.3f);
    }

    protected void DestroyHPBar()
    {
        if (hpBarRectTransform != null)
            Destroy(hpBarRectTransform.gameObject);
    }

    protected void UpdateHPBar()
    {
        hpSlider.value = curHealth / maxHealth;
    }

    public void SetNameText(string name)
    {
        nameText.text = name;
    }
    #endregion

    public override void GetHeal(float healAmount)
    {
        base.GetHeal(healAmount);
        UpdateHPBar();
    }
}
