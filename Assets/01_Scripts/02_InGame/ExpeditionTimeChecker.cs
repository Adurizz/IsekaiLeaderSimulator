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

    private const float secondPhaseTime = 120f;
    private const float thirdPhaseTime = 240f;
    private const float fourthPhaseTime = 360f;
    private const float fifthPhaseTime = 480f;




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

            if (passedTime >= secondPhaseTime && passedTime < thirdPhaseTime) // 2th Phase
            {
                if (!informed[2])
                {
                    informed[2] = true;
                    informNewPhase.Invoke(2);
                }
            }
            else if (passedTime >= thirdPhaseTime && passedTime < fourthPhaseTime) // 3th Phase
            {
                if (!informed[3])
                {
                    informed[3] = true;
                    informNewPhase.Invoke(3);
                }
            }
            else if (passedTime >= fourthPhaseTime && passedTime < fifthPhaseTime) // 4th Phase
            {
                if (!informed[4])
                {
                    informed[4] = true;
                    informNewPhase.Invoke(4);
                }
            }
            else if (passedTime >= fifthPhaseTime) // 5th Phase
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
