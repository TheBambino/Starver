﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace Starvers.PlayerBoosts
{
	public class SkillManager : IEnumerable<StarverSkill>
	{
		#region Fields
		private StarverSkill[] skills;
		#endregion
		#region Properties
		public int Count => StarverSkill.Count;

		public StarverSkill this[int id] => skills[id];
		public StarverSkill this[SkillIDs id] => this[(int)id];

		public string[] SkillLists { get; private set; }
		#endregion
		#region Ctor
		public SkillManager()
		{
			Load();
		}
		#endregion
		#region Load
		public void Load()
		{
			#region LoadSkillTypes
			var types = typeof(SkillManager).Assembly.GetTypes();
			var skillTypes = types.Where(type => type.IsSubclassOf(typeof(StarverSkill)) && !type.IsAbstract).ToArray();
			#endregion
			#region LoadSkills
			skills = new StarverSkill[skillTypes.Count()];
			foreach (var type in skillTypes)
			{
				var skill = (StarverSkill)Activator.CreateInstance(type);
				skill.Load();
				skills[skill.ID] = skill;
			}
			#endregion
			#region LoadSkillList
			{
				SkillLists = new string[(int)Math.Ceiling(Count / 8.0)];
				int page = 0;
				var sb = new StringBuilder(skills.Length * 10);
				for (int i = 0; i < skills.Length; i++)
				{
					var skill = skills[i];
					sb.AppendFormat("{0}:  {1}", skill, skill.Summary);
					sb.AppendLine();
					if (i % (8) == 8 - 1 && i != Count - 1)
					{
						SkillLists[page] = sb.ToString();
						sb.Clear();
						page++;
					}
				}
				SkillLists[page] = sb.ToString();
			}
			#endregion
		}
		#endregion
		#region GetSkill
		public SkillT GetSkill<SkillT>() where SkillT : StarverSkill, new()
		{
			return SkillInstance<SkillT>.Value;
		}
		public StarverSkill GetSkill(string nameOrId)
		{
			if (int.TryParse(nameOrId, out int index) && index.InRange(0, Count - 1))
			{
				return skills[index];
			}
			return skills.FirstOrDefault(skill => skill.Name.StartsWith(nameOrId, StringComparison.OrdinalIgnoreCase));
		}
		#endregion
		#region Update
		public void Update()
		{
			for (int i = 0; i < Count; i++)
			{
				skills[i].UpdateState();
			}
		}
		#endregion
		#region GetEnumerator
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public IEnumerator<StarverSkill> GetEnumerator() => ((IEnumerable<StarverSkill>)skills).GetEnumerator();
		#endregion
	}

	public static class SkillInstance<SkillT> where SkillT : StarverSkill
	{
		public static SkillT Value { get; private set; }
		public static byte ID { get; private set; }
		public static void Load(SkillT skill)
		{
			if (Value != null)
			{
				throw new InvalidOperationException();
			}
			Value = skill;
			ID = skill.ID;
		}
	}

	public enum SkillIDs : byte
	{
		FireBall,
		MirrorMana,
		GaeBolg,
		LimitBreak,
		MagnetStorm,
		Musket,
		FlameBurning,
		Sacrifice,
		EnderWand,
		SpiritStrike,
		Whirlwind,
		UnstableTele,
		FishingRod,
		FrozenCraze,
		FromHell,
		AlcoholFeast,
		NatureGuard,
		WindRealm,
		Avalon,
		TrackingMissile,
		NightMana,
		LimitlessSpark,
		PosionFog,
		JusticeFromSky,
		Cosmos,
		MiracleMana,
		GreenWind,
		AvalonGradation,
		StarEruption,
		NatureRage,
		ChordMana,
		LawAias,
		CDLess,
		Chaos,
		RealmOfDefense,
		ExCalibur,
		NStrike,
		UltimateSlash,
		UniverseBlast,
		NatureStorm
	}
}
