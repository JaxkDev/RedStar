using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MouseOverFurnitureTypeText : MonoBehaviour {

    TMP_Text myText;

    MouseController mouseController;

    // Start is called before the first frame update
    void Start() {
        this.myText = this.GetComponent<TMP_Text>();
        if(this.myText == null) {
            Debug.LogError("MouseOverFurnitureTypeText: No 'TextMesh Pro' UI component on this object.");
            this.enabled = false;
            return;
        }

        this.mouseController = GameObject.FindObjectOfType<MouseController>();
        if(this.mouseController == null) {
            Debug.LogError("MouseOverFurnitureTypeText: No instance of mouse controller ???");
            this.enabled = false;
            return;
        }
    }

    // Update is called once per frame
    void Update() {
        Tile t = this.mouseController.GetMouseOverTile();

        if(t != null && t.furniture != null) {
            this.myText.text = "Furniture Type: " + t.furniture.furnitureType;
        } else {
            this.myText.text = "Furniture Type: NULL";
        }
    }
}
