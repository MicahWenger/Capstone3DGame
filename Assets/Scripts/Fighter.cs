using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using Photon.Pun;

public class Fighter : MonoBehaviour
{
    // Stats
    [Serializable]
    public class Stats
    {
        //public GameObject hitMarkerPrefab;

        public float maxHP;
        public float hp;
        public float maxMana;
        public float mana;
        public float manaRegenRate;
        public float strength;
        public float defense;
        public float spellPower;
    }

    public Stats stats = new Stats();
    //public EnemyAiMovement enemyAiMovement
    //public float pushRecoverySpeed = 0.2f;

    //immunity
    protected float immuneTime = 0.5f;
    protected float lastImmune;

    //push ?? ?? ? ?? ? ?
    protected Vector3 pushDirection;


    //all fighters can receivedamage/die
    public void ReceiveDamage(float dmg){
        /*
                //Debug.Log(EquipmentManager.instance.currentEquipment[0].name);
                Debug.Log("Damage: " + dmg);
                Debug.Log("Enemy Health: " + GameManager.instance.enemyAI.stats.hp);
                if(Time.time - lastImmune > immuneTime){
                    lastImmune = Time.time;
                    //Debug.Log("DMG BIG DAMAGE: " + dmg + (EquipmentManager.instance.currentEquipment[0].strengthModifier * GameManager.instance.player.strength));


                    //display damage above the enemy head and in red
                    // dmg + (EquipmentManager.instance.currentEquipment[0].strengthModifier * GameManager.instance.player.strength);  <- used for the damage amount


                    GameManager.instance.enemyAI.enemyDamage.SetValue(dmg + (EquipmentManager.instance.currentEquipment[0].strengthModifier * GameManager.instance.player.stats.strength));
                    GameManager.instance.enemyAI.enemyDamage.showDamage();


                    GameManager.instance.enemyAI.stats.hp -= dmg + (EquipmentManager.instance.currentEquipment[0].strengthModifier * GameManager.instance.player.stats.strength);
                    //pushDirection = (transform.position - dmg.origin).normalized * dmg.pushForce;

                    //GameManager.instance.ShowText((dmg.damageAmount + (GameManager.instance.inventoryUI.weaponSlot.getCurrentWeapon().percentModifier * GameManager.instance.player.strength)).ToString(), 25, Color.red, transform.position, Vector3.zero,0.5f);


                    if(GameManager.instance.enemyAI.stats.hp <= 0){
                        GameManager.instance.enemyAI.stats.hp = 0;
                        Death();

                    }
                }*/
        if (GameManager.instance.isMultiplayer)
        {
            GetComponent<PhotonView>().RPC("_ReceiveDamage", RpcTarget.All, dmg + (GameManager.instance.player.GetComponent<EquipmentManager>().currentEquipment[0].strengthModifier * GameManager.instance.player.stats.strength));
        } else
        {
            _ReceiveDamage(dmg + (EquipmentManager.instance.currentEquipment[0].strengthModifier * GameManager.instance.player.stats.strength));
        }
    }


    public void ReceiveMagicDamage(float dmg){
        /*//if(Time.time - lastImmune > immuneTime){


            lastImmune = Time.time;

            GameManager.instance.enemyAI.enemyDamage.SetValue(dmg);
            GameManager.instance.enemyAI.enemyDamage.showDamage();

            GameManager.instance.enemyAI.stats.hp -= dmg;
            // + (GameManager.instance.inventoryUI.weaponSlot.getCurrentWeapon().percentModifier * GameManager.instance.player.strength);
            //pushDirection = (transform.position - dmg.origin).normalized * dmg.pushForce;

            //GameManager.instance.ShowText((dmg.damageAmount + (GameManager.instance.inventoryUI.weaponSlot.getCurrentWeapon().percentModifier * GameManager.instance.player.strength)).ToString(), 25, Color.red, transform.position, Vector3.zero,0.5f);

            GameManager.instance.enemyAI.healthBar.healthBarUI.SetActive(true);
            GameManager.instance.enemyAI.healthBar.SetValue(GameManager.instance.enemyAI.stats.hp);


            //enemyAiMovement.healthBar.healthBarUI.SetActive(true);
            //enemyAiMovement.healthBar.SetHealth(hitPoint);

            if(GameManager.instance.enemyAI.stats.hp <= 0){
                GameManager.instance.enemyAI.stats.hp = 0;
                // GameManager.instance.enemyAI.Death();
                Death();

            }*/
        if (GameManager.instance.isMultiplayer)
        {
            GetComponent<PhotonView>().RPC("_ReceiveDamage", RpcTarget.All, dmg);
        }
        else
        {
            _ReceiveDamage(dmg);
        }
    }
    [PunRPC]
    public void _ReceiveDamage(float dmg)
    {
        // Debug.Log("Fighter: " + this);
        if (Time.time - lastImmune > immuneTime)
        {
            lastImmune = Time.time;

            gameObject.GetComponent<EnemyAI>().enemyDamage.SetValue(dmg);
            gameObject.GetComponent<EnemyAI>().enemyDamage.showDamage();
            gameObject.GetComponent<EnemyAI>().stats.hp -= dmg;
            gameObject.GetComponent<EnemyAI>().healthBar.healthBarUI.SetActive(true);
            gameObject.GetComponent<EnemyAI>().healthBar.SetValue(gameObject.GetComponent<EnemyAI>().stats.hp);

            if (gameObject.GetComponent<EnemyAI>().stats.hp <= 0) {
                gameObject.GetComponent<EnemyAI>().stats.hp = 0;
                Death();
            }
        }
    }


    /*public void PlayerReceiveMagicDamage(float dmg)
    {
        //double dmgReduction = 0.2 * GameManager.instance.player.defense;

        if(Time.time - lastImmune > immuneTime){
            lastImmune = Time.time;

            gameObject.GetComponent<BetterPlayerMovement>().stats.hp -= dmg;

            //GameManager.instance.player.stats.hp -= dmg;
            //pushDirection = (transform.position - dmg.origin).normalized * dmg.pushForce;

            //GameManager.instance.ShowText((dmg.damageAmount - dmgReduction).ToString(), 25, Color.red, transform.position, Vector3.zero,0.5f);

            gameObject.GetComponent<BetterPlayerMovement>().hudSettings.healthBar.SetValue(gameObject.GetComponent<BetterPlayerMovement>().stats.hp);
            //GameManager.instance.player.hudSettings.healthBar.SetValue(GameManager.instance.player.stats.hp);


            if(gameObject.GetComponent<BetterPlayerMovement>().stats.hp <= 0){
                gameObject.GetComponent<BetterPlayerMovement>().stats.hp = 0;
                Death();
            }
        }
    }*/
    



    protected virtual void Death(){

        // deathScreen.setActive(true);

        Debug.Log("you died...");
        GameManager.instance.Death();
    }


}
