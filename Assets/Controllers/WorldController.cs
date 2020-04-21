/******************************************
 * License - OSL-3.0                      *
 * Created by JaxkDev (JaxkDev@gmail.com) *
 ******************************************/
using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class WorldController : MonoBehaviour {

	public static WorldController Instance { get; protected set; }

	Dictionary<Tile, GameObject> tileGameObjectMap;
    Dictionary<Furniture, GameObject> furnitureGameObjectMap;

    Dictionary<string, Sprite> furnitureSprites;

	public Sprite floorSprite;

    public World world { get; protected set; }

	public int width;
	public int height;

	void Start () {
		if (WorldController.Instance != null) {
			Debug.LogError ("More then one WorldController was initialised !");
		}

		WorldController.Instance = this;

        //Load Resources:
        this.LoadSprites();

        //Generate World:
		this.world = new World(this.width, this.height);
        this.world.RegisterFurnitureCreatedCallback(this.OnFurnitureCreated);   

		this.tileGameObjectMap = new Dictionary<Tile, GameObject>();
        this.furnitureGameObjectMap = new Dictionary<Furniture, GameObject>();

        //Generate tilemap:
		for (int x = 0; x < this.world.Width; x++) {
			for (int y = 0; y < this.world.Height; y++) {
				GameObject tile_go = new GameObject();
				Tile tile_data = this.world.GetTileAt(x, y);

				this.tileGameObjectMap.Add(tile_data, tile_go);

				tile_go.AddComponent<SpriteRenderer>();
				tile_go.name = "Tile_" + x + "_" + y;
				tile_go.transform.position = new Vector3 (tile_data.X, tile_data.Y, 0);
				tile_go.transform.SetParent(this.transform, true);

				tile_data.RegisterTileTypeChangeCallBack(this.OnTileTypeChange);
			}
		}
		this.world.RandomizeTiles();
	}

	void Update(){}

    
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


	// DP - Not yet used could be for changing levels/floors....
	void DestroyAllTileGameObjects(){
		while (this.tileGameObjectMap.Count > 0) {
			Tile tile_data = this.tileGameObjectMap.Keys.First();
			GameObject tile_go = this.tileGameObjectMap[tile_data];

			//remove from map.
			this.tileGameObjectMap.Remove(tile_data);

			tile_data.UnRegisterTileTypeChangeCallBack(this.OnTileTypeChange);

			Destroy(tile_go);
		}
	}

	void OnTileTypeChange(Tile tile_data) {

		if (this.tileGameObjectMap.ContainsKey(tile_data) == false) {
			Debug.LogError ("tileGameObjectMap doesn't contain the tile_data for tile at (" + tile_data.X + "," + tile_data.Y + ")");
			return;
		}

		GameObject tile_go = this.tileGameObjectMap[tile_data];

		if (tile_go == null) {
			Debug.LogError ("tileGameObjectMap's returned GO is null for tile at (" + tile_data.X + "," + tile_data.Y + ")");
			return;
		}
			
		if (tile_data.Type == TileType.Floor) {
			tile_go.GetComponent<SpriteRenderer> ().sprite = floorSprite;
		} else if (tile_data.Type == TileType.Empty) {
			tile_go.GetComponent<SpriteRenderer> ().sprite = null;
		} else {
			Debug.LogError ("onTileTypeChange - Unknown tile type '" + tile_data.Type + "'");
		}
	}

	public Tile GetTileAtWorldPosition(Vector3 position){
		int x = Mathf.FloorToInt (position.x);
		int y = Mathf.FloorToInt (position.y);
		return WorldController.Instance.world.GetTileAt(x, y);
	}

	//debug.
	public void Randomize(){
		this.world.RandomizeTiles();
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

        t = this.world.GetTileAt(x, y + 1);
        if(t != null && t.furniture != null && t.furniture.furnitureType == furn.furnitureType) {
            spriteName += "N";
        }
        t = this.world.GetTileAt(x + 1, y);
        if(t != null && t.furniture != null && t.furniture.furnitureType == furn.furnitureType) {
            spriteName += "E";
        }
        t = this.world.GetTileAt(x, y - 1);
        if(t != null && t.furniture != null && t.furniture.furnitureType == furn.furnitureType) {
            spriteName += "S";
        }
        t = this.world.GetTileAt(x - 1, y);
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
