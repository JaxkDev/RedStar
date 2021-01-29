/******************************************
 * License - OSL-3.0                      *
 * Created by JaxkDev (JaxkDev@gmail.com) *
 ******************************************/

using System;
using UnityEngine;

public class DiscordController : MonoBehaviour {

    public long discordId;
    public bool online = false;
    public int players = 1;
    public long startTime;
    Discord.Discord discord;

    // Use this for initialization
    void Start() {
        World world = WorldController.Instance.world;
        world.RegisterCharacterCreatedCallback(this.characterCreated);
        try{
            this.discord = new Discord.Discord(this.discordId, (System.UInt64)Discord.CreateFlags.NoRequireDiscord);
        } catch(Discord.ResultException result){
            Debug.LogError("Failed to initialise discord, "+result.ToString());
            return;
        }
        this.online = true;
        this.startTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        var activityManager = discord.GetActivityManager();
        activityManager.UpdateActivity(this.generateActivity(), (res) => {
            if(res == Discord.Result.Ok) {
                Debug.Log("Updated discord activity.");
            } else {
                this.StopDiscord();
                Debug.LogError("Discord failed to update activity, " + res);
            }
        });
    }

    Discord.Activity generateActivity() {
        return new Discord.Activity {
            Details = "In galaxy: TestGalaxy",
            State = this.players+" Lifeform"+(this.players == 1 ? "" : "s")+" present.",
            Timestamps =
            {
                Start = this.startTime,
            },
            Assets =
            {
                LargeImage = "red-star", // Larger Image Asset Key
            },
            Instance = false,
        };
    }
    void characterCreated(Character character) {
        if(!this.online) return;
        Debug.Log("Character created updating discord.");
        this.discord.GetActivityManager().UpdateActivity(this.generateActivity(), (res) => {
            if(res == Discord.Result.Ok) {
                Debug.Log("Updated discord activity.");
            } else {
                Debug.LogError("Discord failed to update activity, " + res);
                this.StopDiscord();
            }
        });
    }

    // Update is called once per frame
    void Update() {
        try {
            if(this.online) this.discord.RunCallbacks();
        } catch(Discord.ResultException e) {
            this.StopDiscord();
            Debug.LogError(e);
        }
    }


    void OnApplicationQuit() {
        this.StopDiscord();
    }

    void StopDiscord() {
        if(this.online){
            this.discord.Dispose();
            this.online = false;
            Debug.Log("Discord activity stopped/cleared.");
        }
    }
}
