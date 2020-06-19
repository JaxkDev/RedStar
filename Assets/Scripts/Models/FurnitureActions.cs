/******************************************
 * License - OSL-3.0                      *
 * Created by JaxkDev (JaxkDev@gmail.com) *
 ******************************************/

using UnityEngine;

public static class FurnitureActions {

    public static void Door_UpdateAction(Furniture furn, float deltaTime) {
        //Debug.Log("Update door.");

        if(furn.furnParameters["State"] == 1) {
            //Open door.
            furn.furnParameters["OpenPercent"] += deltaTime * 0.5f;
        }
        if(furn.furnParameters["State"] == 2) {
            //Close door.
            furn.furnParameters["OpenPercent"] -= deltaTime * 0.5f;
        }

        furn.furnParameters["OpenPercent"] = Mathf.Clamp01(furn.furnParameters["OpenPercent"]);

        if(furn.furnParameters["OpenPercent"] == 1) {
            furn.furnParameters["State"] = 0; //IF SET TO 2, Door opens and closes constantly while player walking through.
        }
    }

    public static Enterability Door_IsEnterable(Furniture furn) {
        if(furn.furnParameters["OpenPercent"] == 1) {
            // Door is open.
            return Enterability.Yes;
        }

        furn.furnParameters["State"] = 1; //0=idle,1=open,2=close

        return Enterability.Soon;
    }
}
