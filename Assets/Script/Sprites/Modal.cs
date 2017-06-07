﻿using UnityEngine;
using System.Collections;

public class Modal : MonoBehaviour
{
    public int TypeId
    {
        get
        {
            return typeId;
        }
        protected set
        {
            typeId = value;
        }
    }
    public long Uuid
    {
        get
        {
            return uuid;
        }
    }

    public void InitWithMapPos(int mapId, sbyte posx, sbyte posy, DataCenter.ModalData data)
    {
        this.modId = data.id;
        this.typeId = data.typeId;
        this.modName = data.name;
        this.eventId = data.eventId;
        this.mapId = mapId;
        this.posx = posx;
        this.posy = posy;
        uuid = mapId * 10000 + posy + posx * 100;
		ModalManager.AddMod(uuid, this);

		var animator = GetComponent<Animator>();
		if (animator != null)
		{
			var controller = animator.runtimeAnimatorController;
			controller.animationClips[0] = Resources.Load<AnimationClip>(data.keyAnimation);
		}
    }

    // Use this for initialization
    void Start(){}
    // Update is called once per frame
    void Update(){}


    private int eventId = 0;
    private sbyte posx = -1;
    private sbyte posy = -1;
    private int mapId = 0;
    private int typeId = 0;
    private string modName = "";
    private int modId = 0;
    private long uuid = 0;
}
