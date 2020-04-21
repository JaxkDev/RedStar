/******************************************
 * License - OSL-3.0                      *
 * Created by JaxkDev (JaxkDev@gmail.com) *
 ******************************************/

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class MouseController : MonoBehaviour {

	public GameObject circleCursorPrefab;

	bool buildModeIsObjects = false;
    string buildModeObject;
	TileType buildModeTile = TileType.Floor;

	//Position in world NOT screen.
	private Vector3 lastFramePosition;
	private Vector3 currentFramePosition;

	private Vector3 dragStartPosition;
	private List<GameObject> dragPreviewGameObjects;

	void Start () {
		this.dragPreviewGameObjects = new List<GameObject>();

		SimplePool.Preload(circleCursorPrefab, 1000);
	}
		
	void Update () {

		this.currentFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		this.currentFramePosition.z = 0;

		//Update circle cursor.
		//updateCursor();

		//Multi-Select by click n'drag
		this.UpdateDrag();

		//Screen movement by click n' drag.
		this.UpdateCameraDrag();

		this.lastFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); //Cannot be currentFramePosition due to movement.
		this.lastFramePosition.z = 0;
	}

	public void SetMode_BuildInstalledObject(string objectType){
		//Not a tile.
		this.buildModeIsObjects = true;
        this.buildModeObject = objectType;
	}

	public void SetMode_BuildFloor(){
		this.buildModeIsObjects = false;
		this.buildModeTile = TileType.Floor;
	}

	public void SetMode_Bulldoze(){
		this.buildModeIsObjects = false;
		this.buildModeTile = TileType.Empty;
	}

	void UpdateDrag(){
		//Ignore UI interactions.

		if (EventSystem.current.IsPointerOverGameObject()) {
			return;
		}

		//Start Drag:
		if (Input.GetMouseButtonDown(0)) {
			this.dragStartPosition = this.currentFramePosition;
		}


		int start_x = Mathf.FloorToInt (this.dragStartPosition.x);
		int end_x = Mathf.FloorToInt (this.currentFramePosition.x);

		if (end_x < start_x) {
			int tmp = end_x;
			end_x = start_x;
			start_x = tmp;
		}

		int start_y = Mathf.FloorToInt (this.dragStartPosition.y);
		int end_y = Mathf.FloorToInt (this.currentFramePosition.y);

		if (end_y < start_y) {
			int tmp = end_y;
			end_y = start_y;
			start_y = tmp;
		}

		//Clean Old previews:
		while (this.dragPreviewGameObjects.Count > 0) {
			GameObject go = this.dragPreviewGameObjects[0];
			this.dragPreviewGameObjects.RemoveAt(0);
			SimplePool.Despawn(go);
		}

		//Currently Dragging:
		if (Input.GetMouseButton (0)) {
			// DISPLAY PREVIEW (TODO BETTER GRAPHICS)
			for (int x = start_x; x <= end_x; x++) {
				for (int y = start_y; y <= end_y; y++) {
					Tile t = WorldController.Instance.world.GetTileAt (x, y);
					if (t != null) {
						//Display tile preview:
						GameObject go = SimplePool.Spawn(circleCursorPrefab, new Vector3(x,y,0), Quaternion.identity);
						go.transform.SetParent(this.transform, true);
						this.dragPreviewGameObjects.Add(go);
					}
				}
			}
		}


		//End Drag:
		if (Input.GetMouseButtonUp (0)) {

			for (int x = start_x; x <= end_x; x++) {
				for (int y = start_y; y <= end_y; y++) {
					Tile t = WorldController.Instance.world.GetTileAt (x, y);

					if (t != null) {
						if (this.buildModeIsObjects == true) {
                            //Create object
                            WorldController.Instance.world.PlaceFurniture(this.buildModeObject, t);
						} else {
							t.Type = this.buildModeTile;
						}
					}
				}
			}
		}
	}

	void UpdateCameraDrag(){
		if (Input.GetMouseButton(1) || Input.GetMouseButton(2)) {
			//Right or middle button.
			Vector3 diff = this.lastFramePosition - this.currentFramePosition;
			Camera.main.transform.Translate(diff);
		}

		Camera.main.orthographicSize -= Camera.main.orthographicSize * Input.GetAxis ("Mouse ScrollWheel");
		Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 3f, 24.275f);
	}

//	void UpdateCursor(){
//		Tile tileUnderMouse = WorldController.Instance.getTileAtWorldPosition(currentFramePosition);
//		if (tileUnderMouse != null) {
//			this.circleCursor.SetActive(true);
//			Vector3 cursorPosition = new Vector3(tileUnderMouse.X, tileUnderMouse.Y, 0);
//			this.circleCursor.transform.position = cursorPosition;
//		} else {
//			this.circleCursor.SetActive(false);
//		}
//	}
}
