/******************************************
 * License - OSL-3.0                      *
 * Created by JaxkDev (JaxkDev@gmail.com) *
 ******************************************/

using UnityEngine;
using System.Collections;

public class SoundController: MonoBehaviour {
    //TODO RESOURCE PATHS.

    float soundCooldown = 0.1f;
    
    void Start() {
        WorldController.Instance.world.RegisterFurnitureCreatedCallback(this.OnFurnitureCreated);
        WorldController.Instance.world.RegisterTileChangedCallback(this.OnTileChanged);
    }
    
    void Update() {
        this.soundCooldown -= Time.deltaTime;
    }

    void OnTileChanged(Tile tile_data) {
        if(this.soundCooldown > 0) return;

        AudioClip ac = Resources.Load<AudioClip>("Sounds/Floor_OnCreated");
        AudioSource.PlayClipAtPoint(ac, Camera.main.transform.position);
        this.soundCooldown = 0.1f;
    }

    public void OnFurnitureCreated(Furniture furn) {
        if(this.soundCooldown > 0) return;

        AudioClip ac = Resources.Load<AudioClip>("Sounds/"+furn.furnitureType+"_OnCreated");

        if(ac == null) {
            //No set audio, assume default.
            ac = Resources.Load<AudioClip>("Sounds/Wall_OnCreated");
        }

        AudioSource.PlayClipAtPoint(ac, Camera.main.transform.position);
        this.soundCooldown = 0.1f;
    }
}