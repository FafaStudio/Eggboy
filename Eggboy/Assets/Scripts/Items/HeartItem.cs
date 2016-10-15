﻿using UnityEngine;
using System.Collections;

public class HeartItem : Item {

	public int heartGains;
	private BoardManager board;

	protected override void Start ()
	{
		board = GameManager.instance.getCurrentBoard ();
		board.setObjectOnGrid((int)this.transform.position.x, (int)this.transform.position.y, -1, this.gameObject);
		base.Start ();
	}

	public override void GainLoot (Player player)
	{
		if (isShopItem) {
			if (player.getGolds () >= price) {
				if (player.getHp () == 6)
					return;
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
