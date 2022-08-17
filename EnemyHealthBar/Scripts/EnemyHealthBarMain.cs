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


namespace HealthBarMod
{
    public class EnemyHealthBarMain : MonoBehaviour
    {

        public static EnemyHealthBarMain Instance { get; private set; }

        private static Mod mod;

        bool showingHealth = false;
        WeaponManager weapon = GameManager.Instance.WeaponManager;
        GameObject mainCamera;
        GameObject player;
        PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
        public DaggerfallEntityBehaviour hitNPC;

        public GameObject healthBarObj;


        public float SphereCastRadius = 0.25f;
        int playerLayerMask = 0;

       

        [Invoke(StateManager.StateTypes.Start, 0)]
        public static void Init(InitParams initParams)
        {
            mod = initParams.Mod;

            var go = new GameObject(mod.Title);
            Instance = go.AddComponent<EnemyHealthBarMain>();

            mod.IsReady = true;
        }

        private void Awake()
        {
            mod.IsReady = true;

            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            playerLayerMask = ~(1 << LayerMask.NameToLayer("Player"));
            player = transform.gameObject;
            healthBarObj = new GameObject("HealthBar");

        }

        private void Update()
        {
            
            Debug.Log(GameManager.Instance.PlayerEntity.CurrentRestMode);
            var missile = FindObjectOfType<DaggerfallMissile>();
            
            if (missile != null && missile.Caster.Entity == playerEntity)
            {
                MissileCheck(missile);
                
            }
            if (weapon.ScreenWeapon.WeaponType != WeaponTypes.Bow && weapon.ScreenWeapon.WeaponType != WeaponTypes.None && weapon.ScreenWeapon.IsAttacking())
            {
                PhysCheck();
            }


        }




        //This code checks to see if an enemy is hit by the Player.
        public void PhysCheck()
        {
            if (weapon.ScreenWeapon.GetCurrentFrame() == weapon.ScreenWeapon.GetHitFrame())
            {
                RaycastHit hit;
                Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);

                //check for physical attacks
                if (Physics.SphereCast(ray, weapon.SphereCastRadius, out hit, weapon.ScreenWeapon.Reach, playerLayerMask) && !WeaponEnvCheck(hit))
                {

                    hitNPC = hit.transform.GetComponent<DaggerfallEntityBehaviour>(); //grabs the component of the enemy in the location that was attacked
                    EnemyCheck();
                }
            }
        }
        
        //Mostly vanilla code used from the WeaponManager, checks to see if the Player is hitting the environment.
        public bool WeaponEnvCheck(RaycastHit hit)
        {
            // Check if hit has an DaggerfallAction component
            DaggerfallAction action = hit.transform.gameObject.GetComponent<DaggerfallAction>();
            if (action)
            {
                action.Receive(player, DaggerfallAction.TriggerTypes.Attack);
            }

            // Check if hit has an DaggerfallActionDoor component
            DaggerfallActionDoor actionDoor = hit.transform.gameObject.GetComponent<DaggerfallActionDoor>();
            if (actionDoor)
            {
                actionDoor.AttemptBash(true);
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
            EnemyCheck();
            
        }

        public void EnemyCheck()
        {
            if (hitNPC != null)
            {

                if (HealthBar.Instance == null)
                {
                    HealthBar healthbar = HealthBar.Instance;
                }
                else
                    HealthBar.Instance.reset = true;
            }
        }
    }
}
