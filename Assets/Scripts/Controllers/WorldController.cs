/******************************************
 * License - OSL-3.0                      *
 * Created by JaxkDev (JaxkDev@gmail.com) *
 ******************************************/
using UnityEngine;

public class WorldController : MonoBehaviour {

	public static WorldController Instance { get; protected set; }

    public World world { get; protected set; }

    void OnEnable() {
        if(WorldController.Instance != null) {
            Debug.LogError("More then one WorldController was initialised !");
        }

        WorldController.Instance = this;

        //Generate World:
        this.world = new World();

        //Center the Camera.
        Camera.main.transform.position = new Vector3(this.world.Width / 2, this.world.Height / 2, Camera.main.transform.position.z);

        //this.world.RandomizeTiles();
    }

    void Update() {
        // UPDATES THE WORLD HERE !
        // Whole world simulation...

        world.Update(Time.deltaTime); // Todo possibly pause, 2x, 4x etc. (world time speed)
    }

	public Tile GetTileAtWorldPosition(Vector3 position){
		int x = Mathf.FloorToInt (position.x);
		int y = Mathf.FloorToInt (position.y);
		return WorldController.Instance.world.GetTileAt(x, y);
	}
}
