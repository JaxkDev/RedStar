/******************************************
 * License - OSL-3.0                      *
 * Created by JaxkDev (JaxkDev@gmail.com) *
 ******************************************/
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class FurnitureSpriteController: MonoBehaviour {

    Dictionary<Furniture, GameObject> furnitureGameObjectMap;
    Dictionary<string, Sprite> furnitureSprites;

	void Start() {

        //Load Resources:
        this.LoadSprites();

        this.furnitureGameObjectMap = new Dictionary<Furniture, GameObject>();

        WorldController.Instance.world.RegisterFurnitureCreatedCallback(this.OnFurnitureCreated);
    }

    void LoadSprites() {
        this.furnitureSprites = new Dictionary<string, Sprite>();

        //Load Resources:
        Sprite[] sprites = Resources.LoadAll<Sprite>("Images/Furniture/");
        Debug.Log("Loading sprites from resources...");
        foreach(Sprite s in sprites) {
            Debug.Log("'" + s.name + "' Loaded.");
            this.furnitureSprites[s.name] = s;
        }
    }

    public void OnFurnitureCreated(Furniture furn) {
        GameObject furn_go = new GameObject();

        this.furnitureGameObjectMap.Add(furn, furn_go);

        furn_go.name = furn.furnitureType + "_" + furn.tile.X + "_" + furn.tile.Y;
        furn_go.transform.position = new Vector3(furn.tile.X, furn.tile.Y, 0);
        furn_go.transform.SetParent(this.transform, true);

        SpriteRenderer sr = furn_go.AddComponent<SpriteRenderer>();
        sr.sprite = this.GetSpriteForFurniture(furn);
        sr.sortingLayerName = "Furniture";

        furn.RegisterOnChangedCallback(this.OnFurnitureChanged);
    }

    void OnFurnitureChanged(Furniture furn) {
        if(this.furnitureGameObjectMap.ContainsKey(furn) == false) {
            Debug.LogError("Furniture game object not found in the internal map.");
            return;
        }

        GameObject furn_go = this.furnitureGameObjectMap[furn];
        furn_go.GetComponent<SpriteRenderer>().sprite = this.GetSpriteForFurniture(furn);
    }

    Sprite GetSpriteForFurniture(Furniture furn) {
        if(furn.linksToNeighbour == false) {
            return this.furnitureSprites[furn.furnitureType];
        }

        string spriteName = furn.furnitureType + "_";

        // NEIGHBOUR LOGIC. (TODO CHANGE WHOLE SYSTEM TO USE SUB CLASSES FOR INSTANCE CHECKING EG WALL CONNECT TO WALL BUT NOT TABLE)
        Tile t;
        int x = furn.tile.X;
        int y = furn.tile.Y;

        t = WorldController.Instance.world.GetTileAt(x, y + 1);
        if(t != null && t.furniture != null && t.furniture.furnitureType == furn.furnitureType) {
            spriteName += "N";
        }
        t = WorldController.Instance.world.GetTileAt(x + 1, y);
        if(t != null && t.furniture != null && t.furniture.furnitureType == furn.furnitureType) {
            spriteName += "E";
        }
        t = WorldController.Instance.world.GetTileAt(x, y - 1);
        if(t != null && t.furniture != null && t.furniture.furnitureType == furn.furnitureType) {
            spriteName += "S";
        }
        t = WorldController.Instance.world.GetTileAt(x - 1, y);
        if(t != null && t.furniture != null && t.furniture.furnitureType == furn.furnitureType) {
            spriteName += "W";
        }

        if(this.furnitureSprites.ContainsKey(spriteName) == false) {
            Debug.LogError("No sprite found with name '" + spriteName + "'.");
            return null;
        }

        return this.furnitureSprites[spriteName];
    }
}
