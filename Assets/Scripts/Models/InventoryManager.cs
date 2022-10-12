/******************************************
 * License - OSL-3.0                      *
 * Created by JaxkDev (JaxkDev@gmail.com) *
 ******************************************/

using System.Collections.Generic;

public class InventoryManager {

    // List of all LIVE inventories in the world.
    public Dictionary<string, List<Inventory>> inventories;

    public InventoryManager() {
        this.inventories = new Dictionary<string, List<Inventory>>();
    }

    public bool PlaceInventory(Tile tile, Inventory inventory) {

        bool tileEmpty = (tile.inventory == null);

        if(tile.PlaceInventory(inventory) == false) {
            // Tils is not allowing this inventory to be placed there.
            return false;
        }

        if(inventory.stackSize == 0) {
            if(this.inventories.ContainsKey(tile.inventory.objectType)) {
                this.inventories[inventory.objectType].Remove(inventory);
            }
        }

        // If tile was empty we need to register its new inventory.
        if(tileEmpty) {

            if(this.inventories.ContainsKey(tile.inventory.objectType) == false) {
                // Check list exists for this object type.
                this.inventories[tile.inventory.objectType] = new List<Inventory>();
            }

            this.inventories[tile.inventory.objectType].Add(tile.inventory);
        }

        return true;
    }
}
