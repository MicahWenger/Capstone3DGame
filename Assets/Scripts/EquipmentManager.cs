using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    #region Singleton
    public static EquipmentManager instance;

    void Awake()
    {
        instance = this;
    }

    #endregion

    public Equipment[] currentEquipment;

    public delegate void OnEquipmentChanged (Equipment newItem, Equipment oldItem);
    public OnEquipmentChanged onEquipmentChanged;

    Inventory inventory;

    // Weapon Holder
    public Transform weaponHolder;

    void Start()
    {
        inventory = Inventory.instance;

        int numSlots = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
        currentEquipment = new Equipment[numSlots];

    }

    // Equip a new Item
    public void Equip (Equipment newItem)
    {
        // Find what slot the item fits
        int slotIndex = (int)newItem.equipSlot;

        Equipment oldItem = null;

        // If there's an item already in the slot
        // Swap with this item
        if (currentEquipment[slotIndex] != null)
        {
            oldItem = currentEquipment[slotIndex];
            inventory.Add(oldItem);
        }

        // An item has been equipped
        if (onEquipmentChanged != null)
        {
            onEquipmentChanged.Invoke(newItem, oldItem);
        }

        // Insert item into the slot
        currentEquipment[slotIndex] = newItem;

        // Equip Weapon
        if (slotIndex == 0)
        {
            Vector3 pos = weaponHolder.position;
            // Create new Item and place it in the weapon holder
            Instantiate(newItem.prefab, pos, Quaternion.identity, weaponHolder);
            //GameObject Sword = GameObject.Instantiate(newItem.prefab).gameObject;
            //Sword.transform.SetParent(weaponHolder);
            //weapon.transform.SetPositionAndRotation(pos, id.normalized);
            //weapon.transform.parent = weaponHolder;
            //weapon.transform.set
            
            

            // GameManager.instance.weapon.SwapWeapon(newItem);
        }

    }

    public void UnEquip(int slotIndex)
    {
        if (currentEquipment[slotIndex] != null)
        {
            if(weaponHolder.childCount > 0)
            {
                GameObject oldWeapon = weaponHolder.GetChild(0).gameObject;
                Destroy(oldWeapon);
            }


            Equipment oldItem = currentEquipment[slotIndex];
            inventory.Add(oldItem);

            currentEquipment[slotIndex] = null;
            
            if (onEquipmentChanged != null)
            {
                onEquipmentChanged.Invoke(null, oldItem);
            }

            ///GameManager.instance.weapon.SwapHand();
        }
    }

    public void UnEquipall()
    {
        for (int i = 0; i < currentEquipment.Length; i++)
        {
            UnEquip(i);
        }
    }

     void Update()
     {
        if (Input.GetKeyDown(KeyCode.U))
        {
            UnEquipall();
        }
     }


}
