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
	Action<Tile> cbTileChanged;

	public TileType Type {
		get { return this.type; }
		set {
			TileType oldType = type;
			this.type = value;
			//Update graphically.
			if (this.cbTileChanged != null && oldType != this.type) {
				this.cbTileChanged (this);
			}
		}
	}

    public Inventory inventory { get; protected set; }
	public Furniture furniture { get; protected set; }

    public Job pendingFurnitureJob;

	public World world { get; protected set; }
    
	public int Y { get; protected set; }
	public int X { get; protected set; }


	public Tile(World world, int x, int y) {
		this.world = world;
		this.X = x;
		this.Y = y;
	}

	public void RegisterTileTypeChangeCallBack(Action<Tile> callback) {
		this.cbTileChanged += callback;
	}

    public void UnRegisterTileTypeChangeCallBack(Action<Tile> callback) {
        this.cbTileChanged -= callback;
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

    public bool IsNeighbour(Tile tile, bool checkDiagonal = false) {
        if(this.X == tile.X && (this.Y == tile.Y+1 || this.Y == tile.Y - 1)) return true;
        if(this.Y == tile.Y && (this.X == tile.X+1 || this.X == tile.X - 1)) return true;

        if(checkDiagonal == true) {
            if(this.X == tile.X + 1 && this.Y == tile.Y + 1 || this.Y == tile.Y - 1) return true;
            if(this.X == tile.X - 1 && this.Y == tile.Y + 1 || this.Y == tile.Y - 1) return true;
        }

        return false;
    }
}
