using System;
using UnityEngine;
using System.Collections;
using Photon.Pun;
using UnityStandardAssets.Characters.FirstPerson;

[RequireComponent(typeof(Rigidbody))]
public class Fireball : MonoBehaviour {
    //public bool pushOnAwake = true;
    //public Vector3 startDirection;
    //public float startMagnitude;
    public ForceMode forceMode;
    //public Transform player;

    public GameObject fieryEffect;
    public GameObject smokeEffect;
    //public GameObject explodeEffect;

    public GameObject explosion;

    public LayerMask whatIsEnemies, whatIsPlayer;

    //private float spellPowerModifier = Mathf.Ceil(GameManager.instance.player.spellPower * 0.2f);
    public float fireballDamage;
    public float explosionRange;
    public float maxLifetime;
    public bool explodeOnTouch = true;

    protected Rigidbody rgbd;

    public void Awake()
    {
        rgbd = GetComponent<Rigidbody>();
    }
/*
    public void Start()
    {
        if (pushOnAwake)
        {
            Push(startDirection, startMagnitude);
        }
    }
*/
    public void Push(Vector3 direction, float magnitude)
    {
        
        // Helps the enemy shoot at the player more accurately 
        //transform.LookAt(player);
        // Might have to change for the player
        
        Vector3 dir = direction.normalized;
        rgbd.AddForce(dir * magnitude, forceMode);
    }

    public void OnCollisionEnter(Collision col)
    {
        rgbd.Sleep();
        if (fieryEffect != null)
        {
            StopParticleSystem(fieryEffect);
        }
        if (smokeEffect != null)
        {
            StopParticleSystem(smokeEffect);
        }
        /*
        if (explodeEffect != null)
            explodeEffect.SetActive(false);
*/
        if (col.collider.CompareTag("Player") && explodeOnTouch) ExplodePlayer();
        if (col.collider.CompareTag("Terrain") && explodeOnTouch) Explode();
        if (col.collider.CompareTag("Enemy") && explodeOnTouch)
        {
            //Debug.Log("CALLEDEXPLODEENEMY");
            GameManager.instance.enemyAI = col.collider.GetComponent<EnemyAI>();
            ExplodeEnemy();
        }
    }

    private void Update()
    {
        maxLifetime -= Time.deltaTime;
        if (maxLifetime <= 0)
            Explode();
    }

    private void ExplodePlayer()
    {
        //Debug.Log("CALLED EXPLODE PLAYER");
        if (explosion != null)
        {
            if (GameManager.instance.isMultiplayer)
            {
                PhotonNetwork.Instantiate(explosion.name, transform.position, Quaternion.identity);

            }
            else
            {
                Instantiate(explosion, transform.position, Quaternion.identity);

            }
        }

        Collider[] players = Physics.OverlapSphere(transform.position, explosionRange, whatIsPlayer);
        for (int i = 0; i < players.Length; i++)
        {
            //get component of enemy and call take damage
            
            //example
            players[i].GetComponent<BetterPlayerMovement>().ReceiveMagicDamage(fireballDamage);
            //players[i].GetComponent<Fighter>().PlayerReceiveMagicDamage(fireballDamage);


        }
        
        Invoke("Delay", 0.05f);
    }
    
    private void ExplodeEnemy()
    {


        if (explosion != null)
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
        }

        
        Collider[] enemies = Physics.OverlapSphere(transform.position, explosionRange, whatIsEnemies);
        for (int p = 0; p < enemies.Length; p++)
        {
            //get component of enemy and call take damage
            //example
            enemies[p].GetComponent<Fighter>().ReceiveMagicDamage(fireballDamage);


        }
        
        Invoke("Delay", 0.05f);
    }
    
    private void Explode()
    {

        //Debug.Log("CALLED EXPLODE ");
        if (explosion != null)
        {
            if (GameManager.instance.isMultiplayer)
            {
                PhotonNetwork.Instantiate(explosion.name, transform.position, Quaternion.identity);
            } else
            {
                Instantiate(explosion, transform.position, Quaternion.identity);
            }
        }
        
        Invoke("Delay", 0.05f);
        
    }
    
    
    

    private void Delay()
    {
        
        if (GameManager.instance.isMultiplayer)
        {
            // NetworkManager.instance.Destroy(gameObject);
            PhotonNetwork.Destroy(gameObject);
            PhotonNetwork.Destroy(explosion);
        }
        else
        {
            Destroy(gameObject);
            Destroy(explosion);
        }
        
    }


    public void StopParticleSystem(GameObject g)
    {
        ParticleSystem[] par;
        par = g.GetComponentsInChildren<ParticleSystem>();
        foreach(ParticleSystem p in par)
        {
            p.Stop();
        }
    }

    public void OnEnable()
    {
        if (fieryEffect != null)
            fieryEffect.SetActive(true);
        if (smokeEffect != null)
            smokeEffect.SetActive(true);
        /*
        if (explodeEffect != null)
            explodeEffect.SetActive(false);
            */
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}


