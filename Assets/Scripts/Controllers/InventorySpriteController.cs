/******************************************
 * License - OSL-3.0                      *
 * Created by JaxkDev (JaxkDev@gmail.com) *
 ******************************************/

using UnityEngine;
using System.Collections.Generic;

public class InventorySpriteController : MonoBehaviour {
     
    Dictionary<Inventory, GameObject> inventoryGameObjectMap;

    Dictionary<string, Sprite> inventorySprites;

    void Start() {
        this.inventoryGameObjectMap = new Dictionary<Inventory, GameObject>();

        this.LoadSprites();

        WorldController.Instance.world.RegisterInventoryCreatedCallback(this.OnInventoryCreated);

        // Handle created inventories before start eg loading from save.
        foreach(string objectType in WorldController.Instance.world.inventoryManager.inventories.Keys) {
            foreach(Inventory inv in WorldController.Instance.world.inventoryManager.inventories[objectType]) {
                this.OnInventoryCreated(inv);
            }
        }
    }

    void LoadSprites() {
        this.inventorySprites = new Dictionary<string, Sprite>();

        //Load Resources:
        Sprite[] sprites = Resources.LoadAll<Sprite>("Images/Inventory/");
        Debug.Log("Loading inventory sprites from resources...");
        foreach(Sprite s in sprites) {
            Debug.Log("'" + s.name + "' Loaded.");
            this.inventorySprites[s.name] = s;
        }
        Debug.Log("Done.");
    }

    void OnInventoryCreated(Inventory inv) {
        GameObject inv_go = new GameObject();

        this.inventoryGameObjectMap.Add(inv, inv_go);

        inv_go.name = inv.objectType;
        inv_go.transform.position = new Vector3(inv.tile.X, inv.tile.Y, 0);
        inv_go.transform.SetParent(this.transform, true);

        SpriteRenderer inv_sr = inv_go.AddComponent<SpriteRenderer>();
        inv_sr.sprite = this.inventorySprites[inv.objectType];
        inv_sr.sortingLayerName = "Inventory";

        //TODO Add on changed cb's
        //inv.RegisterCharacterChangedCallback(this.OnCharacterChanged);
    }

    void OnInventoryChanged(Inventory inv) {
        if(this.inventoryGameObjectMap.ContainsKey(inv) == false) {
            Debug.LogError("Cannot update inventory.");
            return;
        }

        GameObject inv_go = this.inventoryGameObjectMap[inv];

        inv_go.transform.position = new Vector3(inv.tile.X, inv.tile.Y, 0);
    }
}
