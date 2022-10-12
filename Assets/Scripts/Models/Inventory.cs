/******************************************
 * License - OSL-3.0                      *
 * Created by JaxkDev (JaxkDev@gmail.com) *
 ******************************************/

public class Inventory {

	public string objectType = "Steel Plate";
	public int maxStackSize = 50;
	public int stackSize = 1;

	public Tile tile;
	public Character character;

	public Inventory() {

    }

	protected Inventory(Inventory other) {
		this.objectType = other.objectType;
		this.maxStackSize = other.maxStackSize;
		this.stackSize = other.stackSize;
	}

	public virtual Inventory Clone() {
		return new Inventory(this);
    }
}