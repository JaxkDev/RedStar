/******************************************
 * License - OSL-3.0                      *
 * Created by JaxkDev (JaxkDev@gmail.com) *
 ******************************************/

using UnityEngine;

public static class FurnitureActions {

    public static void Door_UpdateAction(Furniture furn, float deltaTime) {
        //Debug.Log("Update door.");

        if(furn.GetParameter("State") == 1) {
            //Open door.
            furn.ChangeParameter("OpenPercent", deltaTime * 0.5f);
        }
        if(furn.GetParameter("State") == 2) {
            //Close door.
            furn.ChangeParameter("OpenPercent", deltaTime * -0.5f);
        }

        furn.SetParameter("OpenPercent", Mathf.Clamp01(furn.GetParameter("OpenPercent")));

        if(furn.GetParameter("OpenPercent") == 1) {
            furn.SetParameter("State", 0); //IF SET TO 2, Door opens and closes constantly while player walking through.
        }

        if(furn.cbOnChanged != null){
            furn.cbOnChanged(furn);
        }
    }

    public static Enterability Door_IsEnterable(Furniture furn) {
        furn.SetParameter("State", 1); //0=idle,1=open,2=close

        if(furn.GetParameter("OpenPercent") == 1) {
            // Door is open.
            return Enterability.Yes;
        }

        return Enterability.Soon;
    }
}
