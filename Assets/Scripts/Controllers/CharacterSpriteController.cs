/******************************************
 * License - OSL-3.0                      *
 * Created by JaxkDev (JaxkDev@gmail.com) *
 ******************************************/

using UnityEngine;
using System.Collections.Generic;

public class CharacterSpriteController : MonoBehaviour {

    Dictionary<Character, GameObject> characterGameObjectMap;

    Dictionary<string, Sprite> characterSprites;

    void Start() {
        this.characterGameObjectMap = new Dictionary<Character, GameObject>();
        this.characterSprites = new Dictionary<string, Sprite>();

        this.LoadSprites();

        WorldController.Instance.world.RegisterCharacterCreatedCallback(this.OnCharacterCreated);

        Character c = WorldController.Instance.world.CreateCharacter(WorldController.Instance.world.GetTileAt(WorldController.Instance.world.Width / 2, WorldController.Instance.world.Height / 2));

        c.SetDestinationTile(WorldController.Instance.world.GetTileAt((WorldController.Instance.world.Width / 2) + 5, WorldController.Instance.world.Height / 2));
    }

    void LoadSprites() {
        this.characterSprites = new Dictionary<string, Sprite>();

        //Load Resources:
        Sprite[] sprites = Resources.LoadAll<Sprite>("Images/Characters/");
        Debug.Log("Loading character sprites from resources...");
        foreach(Sprite s in sprites) {
            Debug.Log("'" + s.name + "' Loaded.");
            this.characterSprites[s.name] = s;
        }
    }

    void OnCharacterCreated(Character character) {
        GameObject char_go = new GameObject(); //Char_Go 'cargo' lol.

        this.characterGameObjectMap.Add(character, char_go);

        char_go.name = "Character 1";
        char_go.transform.position = new Vector3(character.X, character.Y, 0);
        char_go.transform.SetParent(this.transform, true);

        SpriteRenderer char_sr = char_go.AddComponent<SpriteRenderer>();
        char_sr.sprite = this.characterSprites["p1_front"];
        char_sr.sortingLayerName = "Characters";

        character.RegisterCharacterChangedCallback(this.OnCharacterChanged);
    }

    void OnCharacterChanged(Character c) {
        if(this.characterGameObjectMap.ContainsKey(c) == false) {
            Debug.LogError("Cannot update character.");
            return;
        }

        GameObject char_go = this.characterGameObjectMap[c];

        char_go.transform.position = new Vector3(c.X, c.Y, 0);
    }
}
