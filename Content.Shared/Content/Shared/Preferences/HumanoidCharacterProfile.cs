using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Content.Shared.CCVar;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Roles;
using Content.Shared.Traits;
using Content.Shared.White.TTS;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Shared.Preferences
{
	// Token: 0x02000246 RID: 582
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	[NetSerializable]
	[Serializable]
	public sealed class HumanoidCharacterProfile : ICharacterProfile
	{
		// Token: 0x06000690 RID: 1680 RVA: 0x00017374 File Offset: 0x00015574
		private HumanoidCharacterProfile(string name, string flavortext, string species, string voice, int age, Sex sex, Gender gender, HumanoidCharacterAppearance appearance, ClothingPreference clothing, BackpackPreference backpack, Dictionary<string, JobPriority> jobPriorities, PreferenceUnavailableMode preferenceUnavailable, List<string> antagPreferences, List<string> traitPreferences)
		{
			this.Name = name;
			this.FlavorText = flavortext;
			this.Species = species;
			this.Voice = voice;
			this.Age = age;
			this.Sex = sex;
			this.Gender = gender;
			this.Appearance = appearance;
			this.Clothing = clothing;
			this.Backpack = backpack;
			this._jobPriorities = jobPriorities;
			this.PreferenceUnavailable = preferenceUnavailable;
			this._antagPreferences = antagPreferences;
			this._traitPreferences = traitPreferences;
		}

		// Token: 0x06000691 RID: 1681 RVA: 0x000173F4 File Offset: 0x000155F4
		private HumanoidCharacterProfile(HumanoidCharacterProfile other, Dictionary<string, JobPriority> jobPriorities, List<string> antagPreferences, List<string> traitPreferences) : this(other.Name, other.FlavorText, other.Species, other.Voice, other.Age, other.Sex, other.Gender, other.Appearance, other.Clothing, other.Backpack, jobPriorities, other.PreferenceUnavailable, antagPreferences, traitPreferences)
		{
		}

		// Token: 0x06000692 RID: 1682 RVA: 0x0001744D File Offset: 0x0001564D
		private HumanoidCharacterProfile(HumanoidCharacterProfile other) : this(other, new Dictionary<string, JobPriority>(other.JobPriorities), new List<string>(other.AntagPreferences), new List<string>(other.TraitPreferences))
		{
		}

		// Token: 0x06000693 RID: 1683 RVA: 0x00017478 File Offset: 0x00015678
		public HumanoidCharacterProfile(string name, string flavortext, string species, string voice, int age, Sex sex, Gender gender, HumanoidCharacterAppearance appearance, ClothingPreference clothing, BackpackPreference backpack, IReadOnlyDictionary<string, JobPriority> jobPriorities, PreferenceUnavailableMode preferenceUnavailable, IReadOnlyList<string> antagPreferences, IReadOnlyList<string> traitPreferences) : this(name, flavortext, species, voice, age, sex, gender, appearance, clothing, backpack, new Dictionary<string, JobPriority>(jobPriorities), preferenceUnavailable, new List<string>(antagPreferences), new List<string>(traitPreferences))
		{
		}

		// Token: 0x06000694 RID: 1684 RVA: 0x000174B4 File Offset: 0x000156B4
		public static HumanoidCharacterProfile Default()
		{
			return new HumanoidCharacterProfile("John Doe", "", "Human", "Garithos", 18, Sex.Male, 3, HumanoidCharacterAppearance.Default(), ClothingPreference.Jumpsuit, BackpackPreference.Backpack, new Dictionary<string, JobPriority>
			{
				{
					"Passenger",
					JobPriority.High
				}
			}, PreferenceUnavailableMode.SpawnAsOverflow, new List<string>(), new List<string>());
		}

		// Token: 0x06000695 RID: 1685 RVA: 0x00017504 File Offset: 0x00015704
		public static HumanoidCharacterProfile DefaultWithSpecies(string species = "Human")
		{
			return new HumanoidCharacterProfile("John Doe", "", species, "Garithos", 18, Sex.Male, 3, HumanoidCharacterAppearance.DefaultWithSpecies(species), ClothingPreference.Jumpsuit, BackpackPreference.Backpack, new Dictionary<string, JobPriority>
			{
				{
					"Passenger",
					JobPriority.High
				}
			}, PreferenceUnavailableMode.SpawnAsOverflow, new List<string>(), new List<string>());
		}

		// Token: 0x06000696 RID: 1686 RVA: 0x00017550 File Offset: 0x00015750
		public static HumanoidCharacterProfile Random([Nullable(new byte[]
		{
			2,
			1
		})] HashSet<string> ignoredSpecies = null)
		{
			IPrototypeManager prototypeManager = IoCManager.Resolve<IPrototypeManager>();
			return HumanoidCharacterProfile.RandomWithSpecies(RandomExtensions.Pick<SpeciesPrototype>(IoCManager.Resolve<IRobustRandom>(), prototypeManager.EnumeratePrototypes<SpeciesPrototype>().Where(delegate(SpeciesPrototype x)
			{
				if (ignoredSpecies != null)
				{
					return x.RoundStart && !ignoredSpecies.Contains(x.ID);
				}
				return x.RoundStart;
			}).ToArray<SpeciesPrototype>()).ID);
		}

		// Token: 0x06000697 RID: 1687 RVA: 0x000175A0 File Offset: 0x000157A0
		public static HumanoidCharacterProfile RandomWithSpecies(string species = "Human")
		{
			IPrototypeManager prototypeManager = IoCManager.Resolve<IPrototypeManager>();
			IRobustRandom random = IoCManager.Resolve<IRobustRandom>();
			Sex sex = Sex.Unsexed;
			int age = 18;
			SpeciesPrototype speciesPrototype;
			if (prototypeManager.TryIndex<SpeciesPrototype>(species, ref speciesPrototype))
			{
				sex = RandomExtensions.Pick<Sex>(random, speciesPrototype.Sexes);
				age = random.Next(speciesPrototype.MinAge, speciesPrototype.OldAge);
			}
			string voiceId = RandomExtensions.Pick<TTSVoicePrototype>(random, (from o in prototypeManager.EnumeratePrototypes<TTSVoicePrototype>()
			where HumanoidCharacterProfile.CanHaveVoice(o, sex)
			select o).ToArray<TTSVoicePrototype>()).ID;
			Gender gender = (sex == Sex.Male) ? 3 : 2;
			return new HumanoidCharacterProfile(HumanoidCharacterProfile.GetName(species, gender), "", species, voiceId, age, sex, gender, HumanoidCharacterAppearance.Random(species, sex), ClothingPreference.Jumpsuit, BackpackPreference.Backpack, new Dictionary<string, JobPriority>
			{
				{
					"Passenger",
					JobPriority.High
				}
			}, PreferenceUnavailableMode.StayInLobby, new List<string>(), new List<string>());
		}

		// Token: 0x17000141 RID: 321
		// (get) Token: 0x06000698 RID: 1688 RVA: 0x0001767B File Offset: 0x0001587B
		// (set) Token: 0x06000699 RID: 1689 RVA: 0x00017683 File Offset: 0x00015883
		public string Name { get; private set; }

		// Token: 0x17000142 RID: 322
		// (get) Token: 0x0600069A RID: 1690 RVA: 0x0001768C File Offset: 0x0001588C
		// (set) Token: 0x0600069B RID: 1691 RVA: 0x00017694 File Offset: 0x00015894
		public string FlavorText { get; private set; }

		// Token: 0x17000143 RID: 323
		// (get) Token: 0x0600069C RID: 1692 RVA: 0x0001769D File Offset: 0x0001589D
		// (set) Token: 0x0600069D RID: 1693 RVA: 0x000176A5 File Offset: 0x000158A5
		public string Species { get; private set; }

		// Token: 0x17000144 RID: 324
		// (get) Token: 0x0600069E RID: 1694 RVA: 0x000176AE File Offset: 0x000158AE
		// (set) Token: 0x0600069F RID: 1695 RVA: 0x000176B6 File Offset: 0x000158B6
		public string Voice { get; private set; }

		// Token: 0x17000145 RID: 325
		// (get) Token: 0x060006A0 RID: 1696 RVA: 0x000176BF File Offset: 0x000158BF
		// (set) Token: 0x060006A1 RID: 1697 RVA: 0x000176C7 File Offset: 0x000158C7
		[DataField("age", false, 1, false, false, null)]
		public int Age { get; private set; }

		// Token: 0x17000146 RID: 326
		// (get) Token: 0x060006A2 RID: 1698 RVA: 0x000176D0 File Offset: 0x000158D0
		// (set) Token: 0x060006A3 RID: 1699 RVA: 0x000176D8 File Offset: 0x000158D8
		[DataField("sex", false, 1, false, false, null)]
		public Sex Sex { get; private set; }

		// Token: 0x17000147 RID: 327
		// (get) Token: 0x060006A4 RID: 1700 RVA: 0x000176E1 File Offset: 0x000158E1
		// (set) Token: 0x060006A5 RID: 1701 RVA: 0x000176E9 File Offset: 0x000158E9
		[DataField("gender", false, 1, false, false, null)]
		public Gender Gender { get; private set; }

		// Token: 0x17000148 RID: 328
		// (get) Token: 0x060006A6 RID: 1702 RVA: 0x000176F2 File Offset: 0x000158F2
		public ICharacterAppearance CharacterAppearance
		{
			get
			{
				return this.Appearance;
			}
		}

		// Token: 0x17000149 RID: 329
		// (get) Token: 0x060006A7 RID: 1703 RVA: 0x000176FA File Offset: 0x000158FA
		// (set) Token: 0x060006A8 RID: 1704 RVA: 0x00017702 File Offset: 0x00015902
		[DataField("appearance", false, 1, false, false, null)]
		public HumanoidCharacterAppearance Appearance { get; private set; }

		// Token: 0x1700014A RID: 330
		// (get) Token: 0x060006A9 RID: 1705 RVA: 0x0001770B File Offset: 0x0001590B
		// (set) Token: 0x060006AA RID: 1706 RVA: 0x00017713 File Offset: 0x00015913
		public ClothingPreference Clothing { get; private set; }

		// Token: 0x1700014B RID: 331
		// (get) Token: 0x060006AB RID: 1707 RVA: 0x0001771C File Offset: 0x0001591C
		// (set) Token: 0x060006AC RID: 1708 RVA: 0x00017724 File Offset: 0x00015924
		public BackpackPreference Backpack { get; private set; }

		// Token: 0x1700014C RID: 332
		// (get) Token: 0x060006AD RID: 1709 RVA: 0x0001772D File Offset: 0x0001592D
		public IReadOnlyDictionary<string, JobPriority> JobPriorities
		{
			get
			{
				return this._jobPriorities;
			}
		}

		// Token: 0x1700014D RID: 333
		// (get) Token: 0x060006AE RID: 1710 RVA: 0x00017735 File Offset: 0x00015935
		public IReadOnlyList<string> AntagPreferences
		{
			get
			{
				return this._antagPreferences;
			}
		}

		// Token: 0x1700014E RID: 334
		// (get) Token: 0x060006AF RID: 1711 RVA: 0x0001773D File Offset: 0x0001593D
		public IReadOnlyList<string> TraitPreferences
		{
			get
			{
				return this._traitPreferences;
			}
		}

		// Token: 0x1700014F RID: 335
		// (get) Token: 0x060006B0 RID: 1712 RVA: 0x00017745 File Offset: 0x00015945
		// (set) Token: 0x060006B1 RID: 1713 RVA: 0x0001774D File Offset: 0x0001594D
		public PreferenceUnavailableMode PreferenceUnavailable { get; private set; }

		// Token: 0x060006B2 RID: 1714 RVA: 0x00017756 File Offset: 0x00015956
		public HumanoidCharacterProfile WithName(string name)
		{
			return new HumanoidCharacterProfile(this)
			{
				Name = name
			};
		}

		// Token: 0x060006B3 RID: 1715 RVA: 0x00017765 File Offset: 0x00015965
		public HumanoidCharacterProfile WithFlavorText(string flavorText)
		{
			return new HumanoidCharacterProfile(this)
			{
				FlavorText = flavorText
			};
		}

		// Token: 0x060006B4 RID: 1716 RVA: 0x00017774 File Offset: 0x00015974
		public HumanoidCharacterProfile WithAge(int age)
		{
			return new HumanoidCharacterProfile(this)
			{
				Age = age
			};
		}

		// Token: 0x060006B5 RID: 1717 RVA: 0x00017783 File Offset: 0x00015983
		public HumanoidCharacterProfile WithSex(Sex sex)
		{
			return new HumanoidCharacterProfile(this)
			{
				Sex = sex
			};
		}

		// Token: 0x060006B6 RID: 1718 RVA: 0x00017792 File Offset: 0x00015992
		public HumanoidCharacterProfile WithGender(Gender gender)
		{
			return new HumanoidCharacterProfile(this)
			{
				Gender = gender
			};
		}

		// Token: 0x060006B7 RID: 1719 RVA: 0x000177A1 File Offset: 0x000159A1
		public HumanoidCharacterProfile WithSpecies(string species)
		{
			return new HumanoidCharacterProfile(this)
			{
				Species = species
			};
		}

		// Token: 0x060006B8 RID: 1720 RVA: 0x000177B0 File Offset: 0x000159B0
		public HumanoidCharacterProfile WithVoice(string voice)
		{
			return new HumanoidCharacterProfile(this)
			{
				Voice = voice
			};
		}

		// Token: 0x060006B9 RID: 1721 RVA: 0x000177BF File Offset: 0x000159BF
		public HumanoidCharacterProfile WithCharacterAppearance(HumanoidCharacterAppearance appearance)
		{
			return new HumanoidCharacterProfile(this)
			{
				Appearance = appearance
			};
		}

		// Token: 0x060006BA RID: 1722 RVA: 0x000177CE File Offset: 0x000159CE
		public HumanoidCharacterProfile WithClothingPreference(ClothingPreference clothing)
		{
			return new HumanoidCharacterProfile(this)
			{
				Clothing = clothing
			};
		}

		// Token: 0x060006BB RID: 1723 RVA: 0x000177DD File Offset: 0x000159DD
		public HumanoidCharacterProfile WithBackpackPreference(BackpackPreference backpack)
		{
			return new HumanoidCharacterProfile(this)
			{
				Backpack = backpack
			};
		}

		// Token: 0x060006BC RID: 1724 RVA: 0x000177EC File Offset: 0x000159EC
		public HumanoidCharacterProfile WithJobPriorities([Nullable(new byte[]
		{
			1,
			0,
			1
		})] IEnumerable<KeyValuePair<string, JobPriority>> jobPriorities)
		{
			return new HumanoidCharacterProfile(this, new Dictionary<string, JobPriority>(jobPriorities), this._antagPreferences, this._traitPreferences);
		}

		// Token: 0x060006BD RID: 1725 RVA: 0x00017808 File Offset: 0x00015A08
		public HumanoidCharacterProfile WithJobPriority(string jobId, JobPriority priority)
		{
			Dictionary<string, JobPriority> dictionary = new Dictionary<string, JobPriority>(this._jobPriorities);
			if (priority == JobPriority.Never)
			{
				dictionary.Remove(jobId);
			}
			else
			{
				dictionary[jobId] = priority;
			}
			return new HumanoidCharacterProfile(this, dictionary, this._antagPreferences, this._traitPreferences);
		}

		// Token: 0x060006BE RID: 1726 RVA: 0x00017849 File Offset: 0x00015A49
		public HumanoidCharacterProfile WithPreferenceUnavailable(PreferenceUnavailableMode mode)
		{
			return new HumanoidCharacterProfile(this)
			{
				PreferenceUnavailable = mode
			};
		}

		// Token: 0x060006BF RID: 1727 RVA: 0x00017858 File Offset: 0x00015A58
		public HumanoidCharacterProfile WithAntagPreferences(IEnumerable<string> antagPreferences)
		{
			return new HumanoidCharacterProfile(this, this._jobPriorities, new List<string>(antagPreferences), this._traitPreferences);
		}

		// Token: 0x060006C0 RID: 1728 RVA: 0x00017874 File Offset: 0x00015A74
		public HumanoidCharacterProfile WithAntagPreference(string antagId, bool pref)
		{
			List<string> list = new List<string>(this._antagPreferences);
			if (pref)
			{
				if (!list.Contains(antagId))
				{
					list.Add(antagId);
				}
			}
			else if (list.Contains(antagId))
			{
				list.Remove(antagId);
			}
			return new HumanoidCharacterProfile(this, this._jobPriorities, list, this._traitPreferences);
		}

		// Token: 0x060006C1 RID: 1729 RVA: 0x000178C8 File Offset: 0x00015AC8
		public HumanoidCharacterProfile WithTraitPreference(string traitId, bool pref)
		{
			List<string> list = new List<string>(this._traitPreferences);
			if (pref)
			{
				if (!list.Contains(traitId))
				{
					list.Add(traitId);
				}
			}
			else if (list.Contains(traitId))
			{
				list.Remove(traitId);
			}
			return new HumanoidCharacterProfile(this, this._jobPriorities, this._antagPreferences, list);
		}

		// Token: 0x17000150 RID: 336
		// (get) Token: 0x060006C2 RID: 1730 RVA: 0x0001791C File Offset: 0x00015B1C
		public string Summary
		{
			get
			{
				return Loc.GetString("humanoid-character-profile-summary", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("name", this.Name),
					new ValueTuple<string, object>("gender", this.Gender.ToString().ToLowerInvariant()),
					new ValueTuple<string, object>("age", this.Age)
				});
			}
		}

		// Token: 0x060006C3 RID: 1731 RVA: 0x00017998 File Offset: 0x00015B98
		public bool MemberwiseEquals(ICharacterProfile maybeOther)
		{
			HumanoidCharacterProfile other = maybeOther as HumanoidCharacterProfile;
			return other != null && !(this.Name != other.Name) && this.Age == other.Age && this.Sex == other.Sex && this.Gender == other.Gender && this.PreferenceUnavailable == other.PreferenceUnavailable && this.Clothing == other.Clothing && this.Backpack == other.Backpack && this._jobPriorities.SequenceEqual(other._jobPriorities) && this._antagPreferences.SequenceEqual(other._antagPreferences) && this._traitPreferences.SequenceEqual(other._traitPreferences) && this.Appearance.MemberwiseEquals(other.Appearance);
		}

		// Token: 0x060006C4 RID: 1732 RVA: 0x00017A78 File Offset: 0x00015C78
		public void EnsureValid(string[] sponsorMarkings)
		{
			IPrototypeManager prototypeManager = IoCManager.Resolve<IPrototypeManager>();
			SpeciesPrototype speciesPrototype;
			prototypeManager.TryIndex<SpeciesPrototype>(this.Species, ref speciesPrototype);
			Sex sex2;
			switch (this.Sex)
			{
			case Sex.Male:
				sex2 = Sex.Male;
				break;
			case Sex.Female:
				sex2 = Sex.Female;
				break;
			case Sex.Unsexed:
				sex2 = Sex.Unsexed;
				break;
			default:
				sex2 = Sex.Male;
				break;
			}
			Sex sex = sex2;
			int age = this.Age;
			if (speciesPrototype != null)
			{
				if (!speciesPrototype.Sexes.Contains(sex))
				{
					sex = speciesPrototype.Sexes[0];
				}
				age = Math.Clamp(this.Age, speciesPrototype.MinAge, speciesPrototype.MaxAge);
			}
			Gender gender2;
			switch (this.Gender)
			{
			case 0:
				gender2 = 0;
				break;
			case 1:
				gender2 = 1;
				break;
			case 2:
				gender2 = 2;
				break;
			case 3:
				gender2 = 3;
				break;
			default:
				gender2 = 1;
				break;
			}
			Gender gender = gender2;
			string name;
			if (string.IsNullOrEmpty(this.Name))
			{
				name = HumanoidCharacterProfile.GetName(this.Species, gender);
			}
			else if (this.Name.Length > 32)
			{
				name = this.Name.Substring(0, 32);
			}
			else
			{
				name = this.Name;
			}
			name = name.Trim();
			IConfigurationManager configurationManager = IoCManager.Resolve<IConfigurationManager>();
			if (configurationManager.GetCVar<bool>(CCVars.RestrictedNames))
			{
				name = Regex.Replace(name, "[^А-Я,а-я,0-9, -]", string.Empty);
			}
			if (configurationManager.GetCVar<bool>(CCVars.ICNameCase))
			{
				name = Regex.Replace(name, "^(?<word>\\w)|\\b(?<word>\\w)(?=\\w*$)", (Match m) => m.Groups["word"].Value.ToUpper());
			}
			if (string.IsNullOrEmpty(name))
			{
				name = HumanoidCharacterProfile.GetName(this.Species, gender);
			}
			string flavortext;
			if (this.FlavorText.Length > 512)
			{
				flavortext = FormattedMessage.RemoveMarkup(this.FlavorText).Substring(0, 512);
			}
			else
			{
				flavortext = FormattedMessage.RemoveMarkup(this.FlavorText);
			}
			HumanoidCharacterAppearance appearance = HumanoidCharacterAppearance.EnsureValid(this.Appearance, this.Species, sponsorMarkings);
			PreferenceUnavailableMode preferenceUnavailable = this.PreferenceUnavailable;
			PreferenceUnavailableMode preferenceUnavailableMode;
			if (preferenceUnavailable != PreferenceUnavailableMode.StayInLobby)
			{
				if (preferenceUnavailable != PreferenceUnavailableMode.SpawnAsOverflow)
				{
					preferenceUnavailableMode = PreferenceUnavailableMode.StayInLobby;
				}
				else
				{
					preferenceUnavailableMode = PreferenceUnavailableMode.SpawnAsOverflow;
				}
			}
			else
			{
				preferenceUnavailableMode = PreferenceUnavailableMode.StayInLobby;
			}
			PreferenceUnavailableMode prefsUnavailableMode = preferenceUnavailableMode;
			ClothingPreference clothing2 = this.Clothing;
			ClothingPreference clothingPreference;
			if (clothing2 != ClothingPreference.Jumpsuit)
			{
				if (clothing2 != ClothingPreference.Jumpskirt)
				{
					clothingPreference = ClothingPreference.Jumpsuit;
				}
				else
				{
					clothingPreference = ClothingPreference.Jumpskirt;
				}
			}
			else
			{
				clothingPreference = ClothingPreference.Jumpsuit;
			}
			ClothingPreference clothing = clothingPreference;
			BackpackPreference backpackPreference;
			switch (this.Backpack)
			{
			case BackpackPreference.Backpack:
				backpackPreference = BackpackPreference.Backpack;
				break;
			case BackpackPreference.Satchel:
				backpackPreference = BackpackPreference.Satchel;
				break;
			case BackpackPreference.Duffelbag:
				backpackPreference = BackpackPreference.Duffelbag;
				break;
			default:
				backpackPreference = BackpackPreference.Backpack;
				break;
			}
			BackpackPreference backpack = backpackPreference;
			Dictionary<string, JobPriority> dictionary = new Dictionary<string, JobPriority>(this.JobPriorities.Where(delegate(KeyValuePair<string, JobPriority> p)
			{
				bool flag = prototypeManager.HasIndex<JobPrototype>(p.Key);
				if (flag)
				{
					bool flag2;
					switch (p.Value)
					{
					case JobPriority.Never:
						flag2 = false;
						break;
					case JobPriority.Low:
						flag2 = true;
						break;
					case JobPriority.Medium:
						flag2 = true;
						break;
					case JobPriority.High:
						flag2 = true;
						break;
					default:
						flag2 = false;
						break;
					}
					flag = flag2;
				}
				return flag;
			}));
			List<string> antags = this.AntagPreferences.Where(new Func<string, bool>(prototypeManager.HasIndex<AntagPrototype>)).ToList<string>();
			List<string> traits = this.TraitPreferences.Where(new Func<string, bool>(prototypeManager.HasIndex<TraitPrototype>)).ToList<string>();
			this.Name = name;
			this.FlavorText = flavortext;
			this.Age = age;
			this.Sex = sex;
			this.Gender = gender;
			this.Appearance = appearance;
			this.Clothing = clothing;
			this.Backpack = backpack;
			this._jobPriorities.Clear();
			foreach (KeyValuePair<string, JobPriority> keyValuePair in dictionary)
			{
				string text;
				JobPriority jobPriority;
				keyValuePair.Deconstruct(out text, out jobPriority);
				string job = text;
				JobPriority priority = jobPriority;
				this._jobPriorities.Add(job, priority);
			}
			this.PreferenceUnavailable = prefsUnavailableMode;
			this._antagPreferences.Clear();
			this._antagPreferences.AddRange(antags);
			this._traitPreferences.Clear();
			this._traitPreferences.AddRange(traits);
			TTSVoicePrototype voice;
			prototypeManager.TryIndex<TTSVoicePrototype>(this.Voice, ref voice);
			if (voice == null || !HumanoidCharacterProfile.CanHaveVoice(voice, this.Sex))
			{
				this.Voice = SharedHumanoidAppearanceSystem.DefaultSexVoice[sex];
			}
		}

		// Token: 0x060006C5 RID: 1733 RVA: 0x00017E60 File Offset: 0x00016060
		public static bool CanHaveVoice(TTSVoicePrototype voice, Sex sex)
		{
			return (voice.RoundStart && sex == Sex.Unsexed) || voice.Sex == sex || voice.Sex == Sex.Unsexed;
		}

		// Token: 0x060006C6 RID: 1734 RVA: 0x00017E84 File Offset: 0x00016084
		public static string GetName(string species, Gender gender)
		{
			return IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<NamingSystem>().GetName(species, new Gender?(gender));
		}

		// Token: 0x060006C7 RID: 1735 RVA: 0x00017E9C File Offset: 0x0001609C
		[NullableContext(2)]
		public override bool Equals(object obj)
		{
			HumanoidCharacterProfile other = obj as HumanoidCharacterProfile;
			return other != null && this.MemberwiseEquals(other);
		}

		// Token: 0x060006C8 RID: 1736 RVA: 0x00017EBC File Offset: 0x000160BC
		public override int GetHashCode()
		{
			return HashCode.Combine<int, PreferenceUnavailableMode, Dictionary<string, JobPriority>, List<string>, List<string>>(HashCode.Combine<string, string, int, Sex, Gender, HumanoidCharacterAppearance, ClothingPreference, BackpackPreference>(this.Name, this.Species, this.Age, this.Sex, this.Gender, this.Appearance, this.Clothing, this.Backpack), this.PreferenceUnavailable, this._jobPriorities, this._antagPreferences, this._traitPreferences);
		}

		// Token: 0x04000685 RID: 1669
		public const int MaxNameLength = 32;

		// Token: 0x04000686 RID: 1670
		public const int MaxDescLength = 512;

		// Token: 0x04000687 RID: 1671
		private readonly Dictionary<string, JobPriority> _jobPriorities;

		// Token: 0x04000688 RID: 1672
		private readonly List<string> _antagPreferences;

		// Token: 0x04000689 RID: 1673
		private readonly List<string> _traitPreferences;
	}
}
