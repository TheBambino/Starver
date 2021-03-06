﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.GameContent.NetModules;
using Terraria.ID;
using Terraria.Net;
using TShockAPI;

namespace Starvers.PlayerBoosts.Skills
{
	public class Avalon : StarverSkill
	{
		private static Random rand = new Random();
		public Avalon() 
		{
			MPCost = 200;
			CD = 60 * 45;
			Description = "幻想乡，这个技能可以给予你5s的无敌,\n随后附加多种回血buff,苟命专用";
			Author = "三叶草";
			LevelNeed = 1000;
			Summary = "[1000][击败骷髅王解锁]获得暂时的完美防御";
		}
		public override void Release(StarverPlayer player, Vector vel)
		{
			var power = CreativePowerManager.Instance.GetPower<CreativePowers.GodmodePower>();
			power.SetEnabledState(player.Index, true);

			var sound = new NetMessage.NetSoundInfo(player.Center, (ushort)rand.Next(47, 56));
			NetMessage.PlayNetSound(sound, player.Index);

			AsyncRelease(player);
		}
		private async void AsyncRelease(StarverPlayer player)
		{
			await Task.Run(() =>
			{
				try
				{
					var life = player.Life;
					Thread.Sleep(5000);
					var power = CreativePowerManager.Instance.GetPower<CreativePowers.GodmodePower>();
					power.SetEnabledState(player.Index, false);

					player.Life = life;
					player.SetBuff(BuffID.RapidHealing, 10 * 60);
					player.SetBuff(BuffID.NebulaUpLife3, 10 * 60);

					var sound = new NetMessage.NetSoundInfo(player.Center, 19);
					NetMessage.PlayNetSound(sound);
				}
				catch(Exception e)
				{
					TSPlayer.Server.SendErrorMessage(e.ToString());
				}
			});
		}
		public override bool CanSet(StarverPlayer player)
		{
			if (!NPC.downedBoss3)
			{
				player.SendText("该技能已被地牢的诅咒封印", 238, 232, 170);
				return false;
			}
			return base.CanSet(player);
		}
	}
}
