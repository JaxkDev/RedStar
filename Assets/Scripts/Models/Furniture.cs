﻿/******************************************
 * License - OSL-3.0                      *
 * Created by JaxkDev (JaxkDev@gmail.com) *
 ******************************************/

using System;
using System.Collections.Generic;

// Serializing:
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

//Furniture - a object that has been 'constructed' / 'fitted'

public class Furniture : IXmlSerializable {

    /// <summary>
    /// Custom parameters (data) for a particular piece of furniture.
    /// </summary>
    protected Dictionary<string, float> furnParameters;

    /// <summary>
    /// These actions are called every update.
    /// </summary>
    protected Action<Furniture, float> updateActions;

    public Func<Furniture, Enterability> IsEnterable;

    public void Update(float deltaTime) {
        if(this.updateActions != null) this.updateActions(this, deltaTime);
    }


    // REPRESENTS BASE TILE, BUT OBJECT MAY OCCUPY MULTIPLE TILES.
    public Tile tile { get; protected set; }

    // Type to be used by graphics to render sprite.
    public string furnitureType { get; protected set; }

	// So, this movementCost is a multiplier to slow down the entity moving through it (e.g. at cost 4, entity would move 1/4 of its speed through this tile.)
	// IF 0 then entity's cannot pass through.
	public float movementCost { get; protected set; }

    //Does this piece of furniture seperate rooms (eg walls/doors)?
    public bool roomEnclosure { get; protected set; }

	// The dimensions of the object in terms of code NOT visually.
	int width;
	int height;

    // Hints to visual layer that it may need alteration because of neighbours.
    public bool linksToNeighbour { get; protected set; }

    public Action<Furniture> cbOnChanged;

    Func<Tile, bool> funcPositionValidation;

    // TODO Larger options.
    // TODO Rotation.
    public Furniture() {
        // DO NOT USE, XML Serialize ONLY.
        this.furnParameters = new Dictionary<string, float>();
    }

    // Copy Constructor
    protected Furniture( Furniture other) {
        this.furnitureType = other.furnitureType;
        this.movementCost = other.movementCost;
        this.roomEnclosure = other.roomEnclosure;
        this.width = other.width;
        this.height = other.height;
        this.linksToNeighbour = other.linksToNeighbour;
        this.funcPositionValidation = other.DEFAULT__IsValidPosition;

        this.furnParameters = new Dictionary<string, float>(other.furnParameters);
        if(other.updateActions != null) this.updateActions = (Action<Furniture, float>)other.updateActions.Clone();

        this.IsEnterable = other.IsEnterable;
    }

    virtual public Furniture Clone() {
        return new Furniture(this);
    }

    // Constructor for furniture factory.
    public Furniture(string furnitureType, float movementCost = 1f, int width = 1, int height = 1, bool linksToNeighbour = false, bool roomEnclosure = false){
		this.furnitureType = furnitureType;
        this.movementCost = movementCost;
        this.roomEnclosure = roomEnclosure;
        this.width = width;
        this.height = height;
        this.linksToNeighbour = linksToNeighbour;
        this.funcPositionValidation = this.DEFAULT__IsValidPosition;
        this.furnParameters = new Dictionary<string, float>();
    }

	static public Furniture PlaceInstance(Furniture proto, Tile tile){

        if(proto.funcPositionValidation(tile) == false) {
            //Debug.LogError("Cannot place furniture here.");
            return null;
        }

        Furniture instance = proto.Clone();

        instance.tile = tile;

        //TODO Assumes 1x1 size.
        if(tile.PlaceObject(instance) == false) {
            //Not able to place, probably already occupied.
            return null;
        }

        if(instance.linksToNeighbour == true) {
            Tile t;
            int x = tile.X;
            int y = tile.Y;

            t = tile.world.GetTileAt(x, y + 1);
            if(t != null && t.furniture != null && t.furniture.cbOnChanged != null && t.furniture.furnitureType == instance.furnitureType) {
                t.furniture.cbOnChanged(t.furniture);
            }
            t = tile.world.GetTileAt(x + 1, y);
            if(t != null && t.furniture != null && t.furniture.cbOnChanged != null && t.furniture.furnitureType == instance.furnitureType) {
                t.furniture.cbOnChanged(t.furniture);
            }
            t = tile.world.GetTileAt(x, y - 1);
            if(t != null && t.furniture != null && t.furniture.cbOnChanged != null && t.furniture.furnitureType == instance.furnitureType) {
                t.furniture.cbOnChanged(t.furniture);
            }
            t = tile.world.GetTileAt(x - 1, y);
            if(t != null && t.furniture != null && t.furniture.cbOnChanged != null && t.furniture.furnitureType == instance.furnitureType) {
                t.furniture.cbOnChanged(t.furniture);
            }
        }

        return instance;
	}

    public void RegisterOnChangedCallback(Action<Furniture> callbackFunc) {
        this.cbOnChanged += callbackFunc;
    }

    public void UnRegisterOnChangedCallback(Action<Furniture> callbackFunc) {
        this.cbOnChanged -= callbackFunc;
    }

    public float GetParameter(string s, float defaultValue = 0) {
        if(this.furnParameters.ContainsKey(s) == false) {
            return defaultValue;
        }

        return this.furnParameters[s];
    }

    public void SetParameter(string key, float value) {
        this.furnParameters[key] = value;
    }

    /// <summary>
    /// ADDS value to parameter.
    /// </summary>
    public void ChangeParameter(string key, float value) {
        if(this.furnParameters.ContainsKey(key) == false) {
            this.furnParameters[key] = value;
            return;
        }
        this.furnParameters[key] += value;
    }

    public void RegisterUpdateAction(Action<Furniture, float> a) {
        this.updateActions += a;
    }

    public void UnregisterUpdateAction(Action<Furniture, float> a) {
        this.updateActions -= a;
    }

    public bool IsValidPosition(Tile t) {
        return this.funcPositionValidation(t);
    }

    // This will be replaced by custom checks from LUA for the furniture.
    protected bool DEFAULT__IsValidPosition(Tile t) {
        // Called pre-build to check environment.
        if(t.Type != TileType.Floor) {
            //Cannot build on top of nothing !
            return false;
        }

        if(t.furniture != null) {
            return false;
        }

        return true;
    }



    ////////////////////////////////////////////////////////////////////////////////////
    ///
    ///                             SAVING & LOADING
    ///
    ////////////////////////////////////////////////////////////////////////////////////



    public XmlSchema GetSchema() {
        return null;
    }

    public void WriteXml(XmlWriter writer) {
        writer.WriteAttributeString("X", this.tile.X.ToString());
        writer.WriteAttributeString("Y", this.tile.Y.ToString());
        writer.WriteAttributeString("FurnitureType", this.furnitureType);

        foreach(string k in this.furnParameters.Keys) {
            writer.WriteStartElement("Param");
            writer.WriteAttributeString("name", k);
            writer.WriteAttributeString("value", this.furnParameters[k].ToString());
            writer.WriteEndElement();
        }
    }

    public void ReadXml(XmlReader reader) {
        if(reader.ReadToDescendant("Param")) {
            do {
                string k = reader.GetAttribute("name");
                float v = float.Parse(reader.GetAttribute("value"));
                this.furnParameters[k] = v;
            } while(reader.ReadToNextSibling("Param"));
        }
    }
}
