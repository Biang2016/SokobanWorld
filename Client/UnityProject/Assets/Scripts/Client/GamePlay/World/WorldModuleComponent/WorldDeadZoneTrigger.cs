﻿using BiangLibrary.GameDataFormat.Grid;
using BiangLibrary.ObjectPool;
using UnityEngine;

public class WorldDeadZoneTrigger : PoolObject
{
    private BoxCollider BoxCollider;

    public override void OnUsed()
    {
        base.OnUsed();
        BoxCollider.enabled = true;
        gameObject.SetActive(true);
    }

    public override void OnRecycled()
    {
        base.OnRecycled();
        BoxCollider.enabled = false;
        gameObject.SetActive(false);
    }

    void Awake()
    {
        BoxCollider = GetComponent<BoxCollider>();
    }

    public void Initialize(GridPos3D gp)
    {
        transform.position = gp * WorldModule.MODULE_SIZE;
        BoxCollider.size = Vector3.one * (WorldModule.MODULE_SIZE - 0.5f);
        BoxCollider.center = 0.5f * Vector3.one * (WorldModule.MODULE_SIZE - 1);
    }

    void OnTriggerEnter(Collider collider)
    {
        CheckObject(collider);
    }

    void OnTriggerStay(Collider collider)
    {
        //CheckObject(collider);
    }

    private void CheckObject(Collider collider)
    {
        if (collider.gameObject.layer == LayerManager.Instance.Layer_Box || collider.gameObject.layer == LayerManager.Instance.Layer_BoxOnlyDynamicCollider)
        {
            Box box = collider.gameObject.GetComponentInParent<Box>();
            if (box)
            {
                box.DestroySelf();
            }
        }

        if (collider.gameObject.layer == LayerManager.Instance.Layer_ActorIndicator_Player || collider.gameObject.layer == LayerManager.Instance.Layer_ActorIndicator_Enemy)
        {
            ActorFaceHelper actorFaceHelper = collider.gameObject.GetComponent<ActorFaceHelper>();
            Actor actor = collider.gameObject.GetComponentInParent<Actor>();
            if (actor.IsNotNullAndAlive() && !actorFaceHelper)
            {
                Debug.Log("Actor die in WorldDeadZone:" + name);
                actor.DestroySelf();
            }
        }
    }
}