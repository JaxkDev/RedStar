using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MouseOverTileTypeText : MonoBehaviour {

    TMP_Text myText;

    MouseController mouseController;

    // Start is called before the first frame update
    void Start() {
        this.myText = this.GetComponent<TMP_Text>();
        if(this.myText == null) {
            Debug.LogError("MouseOverTileTypeText: No 'TextMesh Pro' UI component on this object.");
            this.enabled = false;
            return;
        }

        this.mouseController = GameObject.FindObjectOfType<MouseController>();
        if(this.mouseController == null) {
            Debug.LogError("MouseOverTileTypeText: No instance of mouse controller ???");
            this.enabled = false;
            return;
        }
    }

    // Update is called once per frame
    void Update() {
        Tile t = this.mouseController.GetMouseOverTile();

        if(t != null) {
            this.myText.text = "Tile Type: " + t.Type.ToString();
        } else {
            this.myText.text = "Tile Type: NULL";
        }
    }
}