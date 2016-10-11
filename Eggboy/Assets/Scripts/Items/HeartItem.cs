using UnityEngine;
using System.Collections;

public class HeartItem : Item {

	public int heartGains;

	public override void GainLoot (Player player)
	{
		if (isShopItem) {
			if (player.getGolds () >= price) {
				player.gainHps (heartGains);
				player.loseGolds (this.price);
				Destroy (this.gameObject);
			} else {
				return;
			}
		} else {
			player.gainHps(heartGains);
			Destroy (this.gameObject);
		}
	}
}
