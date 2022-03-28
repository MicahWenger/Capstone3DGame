using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;
using Photon.Pun;

public class NetworkManager : MonoBehaviourPunCallbacks
{
 
    // Instance
    public static NetworkManager instance;
    internal const int MAX_PLAYERS = 5;

   /* public GameObject ItemsPrefab;
    public GameObject EnemiesPrefab;*/

    // references
    public List<GameObject> items;
    public List<GameObject> enemies;
    public List<GameObject> players;

    /*public GameObject[] enemies;
    public GameObject[] items;*/

    public bool isMultiplayer;

    public int playerCount = 0;

    private void Start()
    {
        Debug.Log("NetworkManager waking");
        instance = this;
        if (players == null) {
            players = new List<GameObject>();
        }
        isMultiplayer = PlayerPrefs.GetInt("isMultiplayer") == 1;
        

        if ((isMultiplayer && PhotonNetwork.IsMasterClient) || !isMultiplayer) {
            InstantiateEntities();
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public void InstantiateEntities()
    {
        if (isMultiplayer)
        {
            // items
            foreach (GameObject item in items)
            {
                string itemName = item.name;
                // Debug.Log(itemName);
                GameObject itemObj = Resources.Load<GameObject>(itemName);
                PhotonNetwork.Instantiate(itemName, itemObj.transform.position, Quaternion.identity);
            }
            foreach (GameObject enemy in enemies)
            {
                string enemyName = enemy.name;
                GameObject enemyObj = Resources.Load<GameObject>(enemyName);
                PhotonNetwork.Instantiate(enemyName, enemyObj.transform.position, Quaternion.identity);
                //PhotonNetwork.Instantiate(EnemiesPrefab.name, EnemiesPrefab.transform.position, Quaternion.identity);
            }
        }
        else
        {
            // Instantiate(ItemsPrefab);
            // Instantiate(EnemiesPrefab);
            foreach (GameObject item in items)
                Instantiate(item);
            foreach (GameObject enemy in enemies)
                Instantiate(enemy);
        }
    }
    public void Destroy(GameObject g)
    {
        Debug.Log("destroy " + g.GetPhotonView().ViewID);
        GetComponent<PhotonView>().RPC("DestroyMaster", RpcTarget.MasterClient, g.GetPhotonView().ViewID);
    }
    [PunRPC]
    public void DestroyMaster(int gID)
    {
        Debug.Log("destroy master " + gID);
        PhotonNetwork.Destroy(PhotonView.Find(gID).gameObject);
    }

    public void AddPlayer(int pID)
    {
        Debug.Log("add player " + pID);
        GetComponent<PhotonView>().RPC("AddPlayerMaster", RpcTarget.MasterClient, pID);
    }
    [PunRPC]
    public void AddPlayerMaster(int newPlayerID)
    {
        if (instance == null)
        {
            players = new List<GameObject>();
        }
        if ((isMultiplayer && PhotonNetwork.IsMasterClient) || !isMultiplayer)
        {
            GameObject newPlayer = PhotonView.Find(newPlayerID).gameObject;
            Debug.Log("adding player master");
            // Debug.Log(instance);
            // Debug.Log(players);
            Debug.Log(playerCount);
            players.Add(newPlayer);
            newPlayer.name = "Player" + playerCount;
            playerCount++;
            //Debug.Log("OnPhotonPlayerConnected() " + other.name);
            
            int[] pIDs = new int[players.Count];
            for (int i = 0; i < players.Count; i++)
                pIDs[i] = players[i].GetPhotonView().ViewID;
            Debug.Log("update pList client please for " + players.Count);
            GetComponent<PhotonView>().RPC("UpdatePlayerListClient", RpcTarget.All, pIDs);
            
        }
    }
    [PunRPC]
    public void UpdatePlayerListClient(int[] pIDs)
    {
        if (instance == null)
        {
            Debug.Log("null instance");
            players = new List<GameObject>();
        }
        if (isMultiplayer && !PhotonNetwork.IsMasterClient)
        {
            Debug.Log("update pList client " + pIDs.Length);
            for (int i = 0; i < pIDs.Length; i++)
            {
                bool playerAdded = false;
                foreach (GameObject p in players)
                {
                    if (p.GetPhotonView().ViewID == pIDs[i]) {
                        playerAdded = true;
                        break;
                    }
                }
                if (!playerAdded)
                {
                    Debug.Log("adding player " + pIDs[i]);
                    GameObject newPlayer = PhotonView.Find(pIDs[i]).gameObject;
                    players.Add(newPlayer);
                    newPlayer.name = "Player" + i;
                    playerCount++;
                }
            }
        }
    }

}
