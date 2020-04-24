/******************************************
 * License - OSL-3.0                      *
 * Created by JaxkDev (JaxkDev@gmail.com) *
 ******************************************/
using UnityEngine;
using System.Xml.Serialization;
using UnityEngine.SceneManagement;
using System.IO;

public class WorldController : MonoBehaviour {

	public static WorldController Instance { get; protected set; }

    public World world { get; protected set; }

    static bool loadWorld = false; // Stays even after reload of scene.

    void OnEnable() {
        if(WorldController.Instance != null) {
            Debug.LogError("More then one WorldController was initialised !");
        }

        WorldController.Instance = this;

        this.CreateEmptyWorld();
    }

    void Update() {
        // UPDATES THE WORLD HERE !
        // Whole world simulation...

        world.Update(Time.deltaTime);
    }

	public Tile GetTileAtWorldPosition(Vector3 position){
		int x = Mathf.FloorToInt (position.x);
		int y = Mathf.FloorToInt (position.y);
		return WorldController.Instance.world.GetTileAt(x, y);
	}





    ////////////////////////////////////////////////////////////////////////////////////
    ///
    ///
    ///                             SAVING & LOADING
    ///
    ///
    ////////////////////////////////////////////////////////////////////////////////////





    public void NewWorld() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SaveWorld() {
        Debug.Log("Starting to save the world...");

        XmlSerializer serializer = new XmlSerializer(typeof(World));
        TextWriter writer = new StringWriter();
        serializer.Serialize(writer, world);
        writer.Close();

        Debug.Log(writer.ToString());
    }

    public void LoadWorld() {
        WorldController.loadWorld = true;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void CreateEmptyWorld(){
        //Generate World:
        this.world = new World(100,100);

        //Center the Camera.
        Camera.main.transform.position = new Vector3(this.world.Width / 2, this.world.Height / 2, Camera.main.transform.position.z);
    }

    void CreateWorldFromSave() {

        Debug.Log("create world from save");

        //Generate World:
        this.world = new World(100,100);

        //Center the Camera.
        Camera.main.transform.position = new Vector3(this.world.Width / 2, this.world.Height / 2, Camera.main.transform.position.z);
    }
}
