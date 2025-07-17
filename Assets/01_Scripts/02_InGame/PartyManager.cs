using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    [SerializeField] List<Companion> partyMemberList = new();
    Dictionary<string, Companion> companionDictionary = new();

    public void RegisterPartyMember(Companion newCompanion)
    {
        partyMemberList.Add(newCompanion);
        companionDictionary[newCompanion.GetName()] = newCompanion;
    }

    public int GetPartySize()
    {
        return partyMemberList.Count;
    }

    public Companion GetPartyMember(string name)
    {
        return companionDictionary[name];
    }

    public Player GetPlayer()
    {
        return FindAnyObjectByType<Player>();
    }
}
