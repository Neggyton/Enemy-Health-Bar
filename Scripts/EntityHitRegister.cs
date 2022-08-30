using System;
using UnityEngine;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Utility;

public class EntityHitRegister : WeaponManager
{

    GameObject mainCamera;
    public DaggerfallEntityBehaviour hitNPC { get; private set; }

    int playerLayerMask = 0;
    float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        playerLayerMask = ~(1 << LayerMask.NameToLayer("Player"));
        ScreenWeapon = GameManager.Instance.WeaponManager.ScreenWeapon;
        SphereCastRadius = 0.15f;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0)
        {
            Countdown();
            return;
        }

        DaggerfallMissile missile = null;
        missile = FindObjectOfType<DaggerfallMissile>();

        if (missile != null && missile.Caster == GameManager.Instance.PlayerEntityBehaviour)
        {
            MissileCheck(missile);
        }
        if (ScreenWeapon.WeaponType != WeaponTypes.Bow && ScreenWeapon.WeaponType != WeaponTypes.None && ScreenWeapon.IsAttacking())
        {
            PhysCheck(missile);
        }

    }

    public void PhysCheck(DaggerfallMissile missile)
    {

        RaycastHit hit;
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);

        //check for physical attacks
        if (Physics.SphereCast(ray, SphereCastRadius, out hit, ScreenWeapon.Reach, playerLayerMask))
        {
            hitNPC = hit.transform.GetComponent<DaggerfallEntityBehaviour>();

            EnemyCheck();
        }
    }

    //Mostly vanilla code used from the WeaponManager, checks to see if the Player is hitting the environment.



    public void MissileCheck(DaggerfallMissile missile)
    {
        if (missile.Targets.Length == 0)
            return;

        hitNPC = missile.Targets[missile.Targets.Length - 1];
        EnemyCheck();

    }

    private void Countdown()
    {
        timer -= Time.deltaTime;
    }

    public void EnemyCheck()
    {
        if (!hitNPC)
            return;

        float npcHealth = hitNPC.Entity.CurrentHealth / (float)hitNPC.Entity.MaxHealth;
        if (hitNPC.EntityType != EntityTypes.Player)
        {
            OnTargetNPC(hitNPC);
            timer = 0.25f;
        }
    }

    public delegate void TargetNPCEventHandler(DaggerfallEntityBehaviour target);
    public static event TargetNPCEventHandler TargetNPC;
    protected virtual void OnTargetNPC(DaggerfallEntityBehaviour target)
    {
        TargetNPC?.Invoke(target);
    }

 
}
