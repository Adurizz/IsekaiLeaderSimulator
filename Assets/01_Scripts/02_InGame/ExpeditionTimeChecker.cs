using TMPro;
using UnityEngine;

public class ExpeditionTimeChecker : MonoBehaviour
{
    [SerializeField] private float passedTime = 0;
    public float PassedTime
    {
        private get
        { return passedTime; }
        set
        {
            passedTime = value;
            string min = ((int)passedTime / 60).ToString();
            string sec = ((int)passedTime % 60 < 10) ? "0" + ((int)passedTime % 60).ToString() : ((int)passedTime % 60).ToString();
            passedTimeText.text = min + ":" + sec;
        }
    }
    [SerializeField] private TextMeshProUGUI passedTimeText;

    private void Update()
    {
        PassedTime += Time.deltaTime;
    }

    public float GetCurPassedTime()
    {
        return PassedTime;
    }
}
