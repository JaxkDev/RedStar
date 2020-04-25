/******************************************
 * License - OSL-3.0                      *
 * Created by JaxkDev (JaxkDev@gmail.com) *
 ******************************************/
 
using System;
using System.Diagnostics;
using System.Collections.Generic;

// Serializing:
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

public class World : IXmlSerializable {

	Tile[,] tiles;

    List<Character> characters;

    public Path_TileGraph tileGraph;

    Dictionary<string, Furniture> furniturePrototypes;

	public int Width { get; protected set; }
	public int Height { get; protected set; }

    Action<Character> cbCharacterCreated;
    Action<Furniture> cbFurnitureCreated;
    Action<Tile> cbTileChanged;

    public JobQueue jobQueue;

	public World(int width, int height){
        this.SetupWorld(width, height);
    }

    void SetupWorld(int width, int height) {
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

        UnityEngine.Debug.Log("World created with " + width * height + " tiles in " + stopwatch.ElapsedMilliseconds + "ms");

    }


    public void Update(float deltaTime) {

        // Todo possibly pause, 2x, 4x etc. (world time speed)
        // deltaTime *= multiplier;

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
            this.InvalidateTileGraph();
        }
    }

    public void InvalidateTileGraph() {
        this.tileGraph = null;
    }

    public void SetupPathfinding() {
        int l = Width / 2 - 5;
        int b = Height / 2 - 5;

        for(int x = l - 5; x < l + 15; x++) {
            for(int y = b - 5; y < b + 15; y++) {
                tiles[x, y].Type = TileType.Floor;


                if(x == l || x == (l + 9) || y == b || y == (b + 9)) {
                    if(x != (l + 9) && y != (b + 4)) {
                        PlaceFurniture("Wall", tiles[x, y]);
                    }
                }



            }
        }

    }

    public Tile GetTileAt(int x, int y) {
        if(x >= this.Width || x < 0 || y >= this.Height || y < 0) {
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
        if(this.cbTileChanged != null) {
            this.cbTileChanged(t);
            this.InvalidateTileGraph();
        }
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



    ////////////////////////////////////////////////////////////////////////////////////
    ///
    ///
    ///                             SAVING & LOADING
    ///
    ///
    ////////////////////////////////////////////////////////////////////////////////////



    public World() {
        // Empty public constructor needed for XmlSerializer.
    }

    public XmlSchema GetSchema() {
        return null;
    }

    public void WriteXml(XmlWriter writer) {
        // Save info here.
        UnityEngine.Debug.Log("SaveXml");
        writer.WriteAttributeString("Width", this.Width.ToString());
        writer.WriteAttributeString("Height", this.Height.ToString());

        /// -- Tiles --
        writer.WriteStartElement("Tiles");
        for(int x = 0; x < this.Width; x++) {
            for(int y = 0; y < this.Height; y++) {
                writer.WriteStartElement("Tile");
                tiles[x, y].WriteXml(writer);
                writer.WriteEndElement();
            }
        }
        writer.WriteEndElement();
        /// -----------
        

        /// -- Furniture --
        
        /// ---------------

    }

    public void ReadXml(XmlReader reader) {
        // Read info here.
        UnityEngine.Debug.Log("ReadXml");

        int width = int.Parse(reader.GetAttribute("Width"));
        int height = int.Parse(reader.GetAttribute("Height"));

        this.SetupWorld(width, height);

        reader.ReadToDescendant("Tiles");
        reader.ReadToDescendant("Tile");
        while(reader.IsStartElement("Tile")) {
            int x = int.Parse(reader.GetAttribute("X"));
            int y = int.Parse(reader.GetAttribute("Y"));

            this.tiles[x, y].ReadXml(reader);

            reader.ReadToNextSibling("Tile");
        }
    }
}
