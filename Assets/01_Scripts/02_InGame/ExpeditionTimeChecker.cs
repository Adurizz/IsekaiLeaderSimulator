using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ExpeditionTimeChecker : MonoBehaviour
{
    [HideInInspector] public UnityEvent<int> informNewPhase = new();

    [SerializeField]
    private bool[] informed = new bool[6]
    {
        true, true, false, false, false, false
    };

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

            if (passedTime >= 10f && passedTime < 20f)
            {
                if (!informed[2])
                {
                    informed[2] = true;
                    informNewPhase.Invoke(2);
                }
            }
            else if (passedTime >= 20f && passedTime < 30f)
            {
                if (!informed[3])
                {
                    informed[3] = true;
                    informNewPhase.Invoke(3);
                }
            }
            else if (passedTime >= 30f && passedTime < 40f)
            {
                if (!informed[4])
                {
                    informed[4] = true;
                    informNewPhase.Invoke(4);
                }
            }
            else if (passedTime >= 40f)
            {
                if (!informed[5])
                {
                    informed[5] = true;
                    informNewPhase.Invoke(5);
                }
            }
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
