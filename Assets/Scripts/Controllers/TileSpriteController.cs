/******************************************
 * License - OSL-3.0                      *
 * Created by JaxkDev (JaxkDev@gmail.com) *
 ******************************************/
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class TileSpriteController: MonoBehaviour {

	Dictionary<Tile, GameObject> tileGameObjectMap;

    public Sprite emptySprite;
	public Sprite floorSprite;

	void Start() {

		this.tileGameObjectMap = new Dictionary<Tile, GameObject>();

        //Generate tilemap (Will take a few ms !):
        for(int x = 0; x < WorldController.Instance.world.Width; x++) {
            for(int y = 0; y < WorldController.Instance.world.Height; y++) {
                GameObject tile_go = new GameObject();
                Tile tile_data = WorldController.Instance.world.GetTileAt(x, y);

                this.tileGameObjectMap.Add(tile_data, tile_go);

                SpriteRenderer tile_sr = tile_go.AddComponent<SpriteRenderer>();
                tile_sr.sprite = emptySprite;
                tile_sr.sortingLayerName = "Tiles";
                tile_go.name = "Tile_" + x + "_" + y;
                tile_go.transform.position = new Vector3(tile_data.X, tile_data.Y, 0);
                tile_go.transform.SetParent(this.transform, true);
            }
        }

        WorldController.Instance.world.RegisterTileChangedCallback(this.OnTileChanged);
    }

    // DP - Not yet used could be for changing levels/floors....
    void DestroyAllTileGameObjects() {
        while(this.tileGameObjectMap.Count > 0) {
            Tile tile_data = this.tileGameObjectMap.Keys.First();
            GameObject tile_go = this.tileGameObjectMap[tile_data];

            //remove from map.
            this.tileGameObjectMap.Remove(tile_data);

            Destroy(tile_go);
        }
    }

	void OnTileChanged(Tile tile_data) {

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
			tile_go.GetComponent<SpriteRenderer> ().sprite = this.floorSprite;
		} else if (tile_data.Type == TileType.Empty) {
			tile_go.GetComponent<SpriteRenderer> ().sprite = this.emptySprite;
		} else {
			Debug.LogError ("onTileTypeChange - Unknown tile type '" + tile_data.Type + "'");
		}
	}
}
