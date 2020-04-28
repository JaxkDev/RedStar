/******************************************
 * License - OSL-3.0                      *
 * Created by JaxkDev (JaxkDev@gmail.com) *
 ******************************************/

using System;
using UnityEngine;

public class DiscordController : MonoBehaviour {

    public long discordId;
    public bool online = true;

    Discord.Discord discord;

    // Use this for initialization
    void Start() {
        this.discord = new Discord.Discord(this.discordId, (System.UInt64)Discord.CreateFlags.Default);
        var activityManager = discord.GetActivityManager();
        var activity = new Discord.Activity {
            State = "Building My Game In C#",
            Details = "No looking !",
            Timestamps =
            {
                Start = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds,
            },
            Assets =
            {
                LargeImage = "red-star", // Larger Image Asset Key
            },
            Instance = true,
        };
        activityManager.UpdateActivity(activity, (res) => {
            if(res == Discord.Result.Ok) {
                Debug.Log("Updated discord activity.");
            } else {
                Debug.LogError("Discord failed to update activity, " + res);
            }
        });
    }

    // Update is called once per frame
    void Update() {
        try {
            if(this.online) this.discord.RunCallbacks();
        } catch(Discord.ResultException e) {
            if(this.online) {
                this.online = false;
                Debug.LogError("Discord has gone offline.");
                Debug.LogError(e);
            }
        }
    }


    void OnApplicationQuit() {
        if(this.online) this.discord.GetActivityManager().ClearActivity((res) => {});
    }
}
