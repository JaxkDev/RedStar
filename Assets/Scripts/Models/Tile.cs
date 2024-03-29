﻿/******************************************
 * License - OSL-3.0                      *
 * Created by JaxkDev (JaxkDev@gmail.com) *
 ******************************************/

using System;
using UnityEngine;

// Serializing:
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

public enum TileType { Empty, Floor };

public enum Enterability {  Yes, Never, Soon };

public class Tile : IXmlSerializable {

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

    public Room room;

    public Job pendingFurnitureJob;

	public World world { get; protected set; }
    
	public int Y { get; protected set; }
	public int X { get; protected set; }

    public float movementCost {
        get {
            if(this.type == TileType.Empty) return 0;
            if(this.furniture == null) return 1; //Possibly tile defaults ?
            return this.furniture.movementCost;
        }
    }


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

    public bool PlaceInventory(Inventory newInventory) {
        if(newInventory == null) {
            this.inventory = null;
            return true;
        }

        if(this.inventory != null) {
            if(this.inventory.objectType != newInventory.objectType) {
                Debug.LogError("Trying to assign inventory to a tile with different type of inventory (unable to stack)");
                return false;
            }

            int numToMove = newInventory.stackSize;
            if((this.inventory.stackSize + newInventory.stackSize) > newInventory.maxStackSize) {
                numToMove = this.inventory.maxStackSize - this.inventory.stackSize;
                //Debug.LogError("Trying to assign inventory to a tile that would exceed max stack size.");
                //return false;
            }

            this.inventory.stackSize += numToMove;
            newInventory.stackSize -= numToMove;

            return true;
        }

        //Place new inventory on tile.
        this.inventory = newInventory.Clone();
        this.inventory.tile = this;
        newInventory.stackSize = 0;
        return true;
    }

    public bool IsNeighbour(Tile tile, bool checkDiagonal = false) {
        return Mathf.Abs(this.X - tile.X) + Mathf.Abs(this.Y - tile.Y) == 1 || (checkDiagonal && (Mathf.Abs(this.X - tile.X) == 1 && Mathf.Abs(this.Y - tile.Y) == 1));
    }
    
    public Tile[] GetNeighbours(bool checkDiagonal = false) {
        Tile[] ns = new Tile[checkDiagonal ? 8 : 4]; //N,E,S,W , NE,SE,SW,NW

        ns[0] = this.world.GetTileAt(this.X, this.Y + 1);
        ns[1] = this.world.GetTileAt(this.X + 1, this.Y);
        ns[2] = this.world.GetTileAt(this.X, this.Y - 1);
        ns[3] = this.world.GetTileAt(this.X - 1, this.Y);

        if(checkDiagonal) {
            ns[4] = this.world.GetTileAt(this.X + 1, this.Y + 1);
            ns[5] = this.world.GetTileAt(this.X + 1, this.Y - 1);
            ns[6] = this.world.GetTileAt(this.X - 1, this.Y - 1);
            ns[7] = this.world.GetTileAt(this.X - 1, this.Y + 1);
        }

        return ns;
    }

    public Enterability IsEnterable() {
        if(this.movementCost == 0) {
            return Enterability.Never;
        }
        
        // Check furniture for enterability.
        if(this.furniture != null && this.furniture.IsEnterable != null) {
            return this.furniture.IsEnterable(this.furniture);
        }

        return Enterability.Yes;
    }

    public Tile NorthTile() {
        return world.GetTileAt(X, Y + 1);
    }

    public Tile EastTile() {
        return world.GetTileAt(X + 1, Y);
    }

    public Tile SouthTile() {
        return world.GetTileAt(X, Y - 1);
    }

    public Tile WestTile() {
        return world.GetTileAt(X - 1, Y);
    }


    ////////////////////////////////////////////////////////////////////////////////////
    ///
    ///                             SAVING & LOADING
    ///
    ////////////////////////////////////////////////////////////////////////////////////



    public Tile() {
        // DO NOT USE, XML Serialize ONLY.
    }

    public XmlSchema GetSchema() {
        return null;
    }

    public void WriteXml(XmlWriter writer) {
        writer.WriteAttributeString("X", this.X.ToString());
        writer.WriteAttributeString("Y", this.Y.ToString());

        writer.WriteAttributeString("Type", ((int)this.type).ToString());
    }

    public void ReadXml(XmlReader reader) {
        //this.X = int.Parse(reader.GetAttribute("X"));
        //this.Y = int.Parse(reader.GetAttribute("Y"));

        this.Type = (TileType)int.Parse(reader.GetAttribute("Type"));
    }
}
