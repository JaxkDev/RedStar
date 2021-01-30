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

        // Handle created furniture before start eg loading from save.
        foreach(Furniture furn in WorldController.Instance.world.furnitures) {
            this.OnFurnitureCreated(furn);
        }
    }

    void LoadSprites() {
        this.furnitureSprites = new Dictionary<string, Sprite>();

        //Load Resources:
        Sprite[] sprites = Resources.LoadAll<Sprite>("Images/Furniture/");
        //Debug.Log("Loading sprites from resources...");
        foreach(Sprite s in sprites) {
            //Debug.Log("'" + s.name + "' Loaded.");
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

    public Sprite GetSpriteForFurniture(string furnitureType) {
        if(this.furnitureSprites.ContainsKey(furnitureType)) return this.furnitureSprites[furnitureType];

        if(this.furnitureSprites.ContainsKey(furnitureType+"_")) return this.furnitureSprites[furnitureType+"_"];

        Debug.LogError("No sprite for furniture type: " + furnitureType);

        return null;
    }

    public Sprite GetSpriteForFurniture(Furniture furn) {
        string spriteName = furn.furnitureType + "_";

        if(furn.linksToNeighbour == false) {
            //Temp, Check door graphics here. (FIXME)
            if(furn.furnitureType == "Door"){
                float percent = furn.furnParameters["OpenPercent"];
                if(percent < 0.1f){
                    //Closed state.
                    spriteName = "Door_";
                } else if(percent < 0.5f){
                    //Mostly closed state.
                    spriteName = "Door_1";
                } else if(percent < 0.9f){
                    //Mostly open state.
                    spriteName = "Door_2";
                } else {
                    //Open state.
                    spriteName = "Door_3";
                }
                return this.furnitureSprites[spriteName];
            }
            return this.furnitureSprites[furn.furnitureType];
        }

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
