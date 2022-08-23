using System;
using UnityEngine;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Weather;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Guilds;

public class PlayerHitRegister : WeaponManager
{

    WeaponManager weapon = GameManager.Instance.WeaponManager;
    GameObject mainCamera;
    GameObject player;
    DaggerfallMissile missile = null;
    public DaggerfallEntityBehaviour hitNPC { get; private set; }
    public MobilePersonNPC villagerNpc { get; private set; }

        int playerLayerMask = 0;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        playerLayerMask = ~(1 << LayerMask.NameToLayer("Player"));
        player = transform.gameObject;
        villagerNpc = null;

    }

    // Update is called once per frame
    void Update()
    {

        villagerNpc = !weapon.ScreenWeapon.IsAttacking() ? null : villagerNpc;

        missile = FindObjectOfType<DaggerfallMissile>();

        if (missile != null && missile.Caster == GameManager.Instance.PlayerEntityBehaviour)
        {
            MissileCheck(missile);

        }
        if (weapon.ScreenWeapon.WeaponType != WeaponTypes.Bow && weapon.ScreenWeapon.WeaponType != WeaponTypes.None && weapon.ScreenWeapon.IsAttacking())
        {
            PhysCheck();
        }

    }

    public void PhysCheck()
    {

        RaycastHit hit;
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);

        //check for physical attacks
        if (Physics.SphereCast(ray, weapon.SphereCastRadius, out hit, weapon.ScreenWeapon.Reach, playerLayerMask) && !WeaponEnvCheck(hit))
        {
            hitNPC = missile != null && missile.Caster != GameManager.Instance.PlayerEntityBehaviour && hitNPC ? hitNPC : hit.transform.GetComponent<DaggerfallEntityBehaviour>();
            //grabs the EntityBehaviour of the enemy in the location that was attacked


            MobilePersonNPC mobileNpc = hit.transform.GetComponent<MobilePersonNPC>();
            villagerNpc = mobileNpc && !mobileNpc.IsGuard && mobileNpc.gameObject.activeSelf ? mobileNpc : null;

            if (!villagerNpc && weapon.ScreenWeapon.GetCurrentFrame() == GameManager.Instance.WeaponManager.ScreenWeapon.GetHitFrame() && hitNPC)
                EnemyCheck();
        }
    }

    //Mostly vanilla code used from the WeaponManager, checks to see if the Player is hitting the environment.
    public bool WeaponEnvCheck(RaycastHit hit)
    {
        // Check if hit has an DaggerfallAction component
        DaggerfallAction action = hit.transform.gameObject.GetComponent<DaggerfallAction>();
        if (action)
        {
            return true;
        }

        // Check if hit has an DaggerfallActionDoor component
        DaggerfallActionDoor actionDoor = hit.transform.gameObject.GetComponent<DaggerfallActionDoor>();
        if (actionDoor)
        {
            return true;
        }

        // Check if player hit a static exterior door
        if (GameManager.Instance.PlayerActivate.AttemptExteriorDoorBash(hit))
        {
            return true;
        }

        // Make hitting walls do a thud or clinging sound (not in classic)
        if (GameObjectHelper.IsStaticGeometry(hit.transform.gameObject))
        {
            return true;
        }

        return false;
    }


    public void MissileCheck(DaggerfallMissile missile)
    {
        if (missile.Targets.Length == 0)
            return;
        else
            hitNPC = missile.Targets[missile.Targets.Length - 1];

        if (hitNPC.EntityType == EntityTypes.CivilianNPC && hitNPC.Entity.Team == MobileTeams.PlayerEnemy)
        {
            return;
        }
        else
            EnemyCheck();

    }

    public void EnemyCheck()
    {
        if (hitNPC != null)
        {
            //make an event

        }
    }

}
