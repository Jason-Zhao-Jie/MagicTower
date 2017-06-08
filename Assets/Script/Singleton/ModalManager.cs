using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ModalManager : MonoBehaviour
{
    public static ModalManager instance = null;
	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
			
	}

    public static bool AddMod(long uuid, Modal mod)
    {
        if (!Contains(uuid))
            return false;
            modals.Add(uuid, mod);
        return true;
    }

    public static void RemoveMod(long uuid){
        modals.Remove(uuid);
    }

	public static bool ChangeMod(long uuid, Modal mod)
	{
		if (!Contains(uuid))
			return false;
        RemoveMod(uuid);
        return AddMod(uuid, mod);
	}

	public static bool ChangeUuid(long newUuid, long oldUuid)
	{
        if (!Contains(oldUuid))
			return false;
        var obj = modals[oldUuid];
        RemoveMod(oldUuid);
        return AddMod(newUuid, obj);
    }

    public static bool Contains(long uuid){
        return modals.ContainsKey(uuid);
    }

    public static GameObject GetObjectByUuid(long uuid){
        if (Contains(uuid))
            return modals[uuid].gameObject;
        return null;
    }

	public static Modal GetModalByUuid(long uuid)
	{
		if (Contains(uuid))
			return modals[uuid];
		return null;
    }

    private static Dictionary<long, Modal> modals = new Dictionary<long, Modal>();
}
