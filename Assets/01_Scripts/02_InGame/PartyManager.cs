using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PartyManager : MonoBehaviour
{
    [SerializeField] private List<Companion> partyMemberList = new();
    Dictionary<string, Companion> companionDictionary = new();
    private TrainingCamp trainingCamp = null;

    public void RegisterPartyMember(Companion newCompanion)
    {
        partyMemberList.Add(newCompanion);
        companionDictionary[newCompanion.GetName()] = newCompanion;
        if (trainingCamp == null)
            trainingCamp = FindAnyObjectByType<TrainingCamp>();
        trainingCamp.RegisterCompanionToTrainingCamp(newCompanion);
    }

    /// <summary>
    /// 동료 사망에 바인딩하기
    /// </summary>
    /// <param name="deadCompanion"></param>
    public void UnregisterPartyMember(Companion deadCompanion)
    {
        partyMemberList.Remove(deadCompanion);
        companionDictionary.Remove(deadCompanion.GetName());
        trainingCamp.UnregisterCompanionFromTrainingCamp(deadCompanion);
    }

    public int GetPartySize()
    {
        return partyMemberList.Count;
    }

    public Companion GetPartyMember(string name)
    {
        return companionDictionary[name];
    }

    public List<Companion> GetWholePartyMember()
    {
        return partyMemberList;
    }

    public Player GetPlayer()
    {
        return FindAnyObjectByType<Player>();
    }
}
