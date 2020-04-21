/******************************************
 * License - OSL-3.0                      *
 * Created by JaxkDev (JaxkDev@gmail.com) *
 ******************************************/

using System;
using UnityEngine;
using System.Collections;

public enum TileType { Empty, Floor };

public class Tile {

	TileType type = TileType.Empty;
	Action<Tile> cbTileTypeChange;

	public TileType Type {
		get { return this.type; }
		set {
			TileType oldType = type;
			this.type = value;
			//Update graphically.
			if (cbTileTypeChange != null && oldType != this.type) {
				cbTileTypeChange (this);
			}
		}
	}

    public Inventory inventory { get; protected set; }
	public Furniture furniture { get; protected set; }

	public World world { get; protected set; }
    
	public int Y { get; protected set; }
	public int X { get; protected set; }


	public Tile(World world, int x, int y) {
		this.world = world;
		this.X = x;
		this.Y = y;
	}

	public void RegisterTileTypeChangeCallBack(Action<Tile> callback) {
		this.cbTileTypeChange += callback;
	}

    public void UnRegisterTileTypeChangeCallBack(Action<Tile> callback) {
        this.cbTileTypeChange -= callback;
    }

    public bool PlaceObject(Furniture objectInstance){
        if(objectInstance == null) {
            this.furniture = null;
            return true;
        }

        if(this.furniture != null) {
            Debug.LogError("Trying to assign a piece of furniture on tile (" + this.X + "," + this.Y + ") that already has a piece of furniture assigned.");
            return false;
        }

        this.furniture = objectInstance;
        return true;
    }
}
