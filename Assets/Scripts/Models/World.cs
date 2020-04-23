/******************************************
 * License - OSL-3.0                      *
 * Created by JaxkDev (JaxkDev@gmail.com) *
 ******************************************/

using System;
using UnityEngine;
using System.Diagnostics;
using System.Collections.Generic;

public class World {

	Tile[,] tiles;

    List<Character> characters;

    Dictionary<string, Furniture> furniturePrototypes;

	public int Width { get; protected set; }
	public int Height { get; protected set; }

    Action<Character> cbCharacterCreated;
    Action<Furniture> cbFurnitureCreated;
    Action<Tile> cbTileChanged;

    public JobQueue jobQueue;

	public World(int width = 100, int height = 100){
		this.Width = width;
		this.Height = height;

		this.tiles = new Tile[width, height];
        this.characters = new List<Character>();
        this.jobQueue = new JobQueue();

		Stopwatch stopwatch = new Stopwatch();

		stopwatch.Start();
        for(int x = 0; x < width; x++) {
            for(int y = 0; y < height; y++) {
                this.tiles[x, y] = new Tile(this, x, y);
                this.tiles[x, y].RegisterTileTypeChangeCallBack(this.OnTileChanged);
            }
        }

        this.CreateFurniturePrototypes();


        stopwatch.Stop();

		UnityEngine.Debug.Log ("World created with " + width * height + " tiles in " + stopwatch.ElapsedMilliseconds + "ms");
	}


    public void Update(float deltaTime) {
        foreach(Character c in this.characters) {
            c.Update(deltaTime);
        }
    }



    public Character CreateCharacter(Tile tile) {
        Character c = new Character(tile);
        this.characters.Add(c);

        if(this.cbCharacterCreated != null) this.cbCharacterCreated(c);

        return c;
    }

    void CreateFurniturePrototypes() {
        this.furniturePrototypes = new Dictionary<string, Furniture>();

        this.furniturePrototypes.Add("Wall", Furniture.CreatePrototype(
            "Wall", 
            0, 
            1, 
            1, 
            true //Links to neighbour.
            ));
    }

    public void PlaceFurniture(string furnitureType, Tile tile) {
        //TODO assumes 1x1 size.
        //UnityEngine.Debug.Log("Placing Furniture '" + objectType + "'");

        if(this.furniturePrototypes.ContainsKey(furnitureType) == false) {
            UnityEngine.Debug.LogError("furniturePrototypes doesn't contain a prototype for type '" + furnitureType + "'");
            return;
        }

        Furniture obj = Furniture.PlaceInstance(this.furniturePrototypes[furnitureType], tile);

        if(obj == null) {
            // Failed to place.
            return;
        }

        if(this.cbFurnitureCreated != null) {
            this.cbFurnitureCreated(obj);
        }
    }

    public void RandomizeTiles(){
		for (int x = 0; x < this.Width; x++) {
			for (int y = 0; y < this.Height; y++) {
				if (UnityEngine.Random.Range(0, 2) == 0) {
					this.tiles[x, y].Type = TileType.Empty;
				} else {
					this.tiles[x, y].Type = TileType.Floor;
				}
			}
		}
	}

    public Tile GetTileAt(int x, int y) {
        if(x >= this.Width || x < 0 || y >= this.Height || y < 0) {
            UnityEngine.Debug.LogError("Tile requested at (" + x + "," + y + ") is out of range.");
            return null;
        }
        return this.tiles[x, y];
    }

    public void RegisterFurnitureCreatedCallback(Action<Furniture> callbackFunc) {
        this.cbFurnitureCreated += callbackFunc;
    }

    public void UnRegisterInstalledObjectCreatedCallback(Action<Furniture> callbackFunc) {
        this.cbFurnitureCreated -= callbackFunc;
    }

    public void RegisterTileChangedCallback(Action<Tile> callbackFunc) {
        this.cbTileChanged += callbackFunc;
    }

    public void UnRegisterTileChangedCallback(Action<Tile> callbackFunc) {
        this.cbTileChanged -= callbackFunc;
    }

    public void RegisterCharacterCreatedCallback(Action<Character> callbackFunc) {
        this.cbCharacterCreated += callbackFunc;
    }

    public void UnRegisterCharacterCreatedCallback(Action<Character> callbackFunc) {
        this.cbCharacterCreated -= callbackFunc;
    }

    void OnTileChanged(Tile t) {
        if(this.cbTileChanged != null) this.cbTileChanged(t);
    }

    public bool IsFurniturePlacementValid(string furnitureType, Tile t) {
        return this.furniturePrototypes[furnitureType].IsValidPosition(t);
    }

    public Furniture GetFurniturePrototype(string furnitureType) {
        if(this.furniturePrototypes.ContainsKey(furnitureType) == false) {
            UnityEngine.Debug.LogError("No furniture prototype with name: " + furnitureType);
            return null;
        }

        return this.furniturePrototypes[furnitureType];
    }
}
