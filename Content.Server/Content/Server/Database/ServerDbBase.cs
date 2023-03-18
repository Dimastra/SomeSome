using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Content.Server.Administration.Logs;
using Content.Shared.Administration.Logs;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Markings;
using Content.Shared.Preferences;
using Robust.Shared.Enums;
using Robust.Shared.Maths;
using Robust.Shared.Network;

namespace Content.Server.Database
{
	// Token: 0x020005B6 RID: 1462
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class ServerDbBase
	{
		// Token: 0x06001E66 RID: 7782 RVA: 0x000A133C File Offset: 0x0009F53C
		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		public Task<PlayerPreferences> GetPlayerPreferencesAsync(NetUserId userId)
		{
			ServerDbBase.<GetPlayerPreferencesAsync>d__0 <GetPlayerPreferencesAsync>d__;
			<GetPlayerPreferencesAsync>d__.<>t__builder = AsyncTaskMethodBuilder<PlayerPreferences>.Create();
			<GetPlayerPreferencesAsync>d__.<>4__this = this;
			<GetPlayerPreferencesAsync>d__.userId = userId;
			<GetPlayerPreferencesAsync>d__.<>1__state = -1;
			<GetPlayerPreferencesAsync>d__.<>t__builder.Start<ServerDbBase.<GetPlayerPreferencesAsync>d__0>(ref <GetPlayerPreferencesAsync>d__);
			return <GetPlayerPreferencesAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001E67 RID: 7783 RVA: 0x000A1388 File Offset: 0x0009F588
		public Task SaveSelectedCharacterIndexAsync(NetUserId userId, int index)
		{
			ServerDbBase.<SaveSelectedCharacterIndexAsync>d__1 <SaveSelectedCharacterIndexAsync>d__;
			<SaveSelectedCharacterIndexAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SaveSelectedCharacterIndexAsync>d__.<>4__this = this;
			<SaveSelectedCharacterIndexAsync>d__.userId = userId;
			<SaveSelectedCharacterIndexAsync>d__.index = index;
			<SaveSelectedCharacterIndexAsync>d__.<>1__state = -1;
			<SaveSelectedCharacterIndexAsync>d__.<>t__builder.Start<ServerDbBase.<SaveSelectedCharacterIndexAsync>d__1>(ref <SaveSelectedCharacterIndexAsync>d__);
			return <SaveSelectedCharacterIndexAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001E68 RID: 7784 RVA: 0x000A13DC File Offset: 0x0009F5DC
		public Task SaveCharacterSlotAsync(NetUserId userId, [Nullable(2)] ICharacterProfile profile, int slot)
		{
			ServerDbBase.<SaveCharacterSlotAsync>d__2 <SaveCharacterSlotAsync>d__;
			<SaveCharacterSlotAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SaveCharacterSlotAsync>d__.<>4__this = this;
			<SaveCharacterSlotAsync>d__.userId = userId;
			<SaveCharacterSlotAsync>d__.profile = profile;
			<SaveCharacterSlotAsync>d__.slot = slot;
			<SaveCharacterSlotAsync>d__.<>1__state = -1;
			<SaveCharacterSlotAsync>d__.<>t__builder.Start<ServerDbBase.<SaveCharacterSlotAsync>d__2>(ref <SaveCharacterSlotAsync>d__);
			return <SaveCharacterSlotAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001E69 RID: 7785 RVA: 0x000A1438 File Offset: 0x0009F638
		private static Task DeleteCharacterSlot(ServerDbContext db, NetUserId userId, int slot)
		{
			ServerDbBase.<DeleteCharacterSlot>d__3 <DeleteCharacterSlot>d__;
			<DeleteCharacterSlot>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<DeleteCharacterSlot>d__.db = db;
			<DeleteCharacterSlot>d__.userId = userId;
			<DeleteCharacterSlot>d__.slot = slot;
			<DeleteCharacterSlot>d__.<>1__state = -1;
			<DeleteCharacterSlot>d__.<>t__builder.Start<ServerDbBase.<DeleteCharacterSlot>d__3>(ref <DeleteCharacterSlot>d__);
			return <DeleteCharacterSlot>d__.<>t__builder.Task;
		}

		// Token: 0x06001E6A RID: 7786 RVA: 0x000A148C File Offset: 0x0009F68C
		public Task<PlayerPreferences> InitPrefsAsync(NetUserId userId, ICharacterProfile defaultProfile)
		{
			ServerDbBase.<InitPrefsAsync>d__4 <InitPrefsAsync>d__;
			<InitPrefsAsync>d__.<>t__builder = AsyncTaskMethodBuilder<PlayerPreferences>.Create();
			<InitPrefsAsync>d__.<>4__this = this;
			<InitPrefsAsync>d__.userId = userId;
			<InitPrefsAsync>d__.defaultProfile = defaultProfile;
			<InitPrefsAsync>d__.<>1__state = -1;
			<InitPrefsAsync>d__.<>t__builder.Start<ServerDbBase.<InitPrefsAsync>d__4>(ref <InitPrefsAsync>d__);
			return <InitPrefsAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001E6B RID: 7787 RVA: 0x000A14E0 File Offset: 0x0009F6E0
		public Task DeleteSlotAndSetSelectedIndex(NetUserId userId, int deleteSlot, int newSlot)
		{
			ServerDbBase.<DeleteSlotAndSetSelectedIndex>d__5 <DeleteSlotAndSetSelectedIndex>d__;
			<DeleteSlotAndSetSelectedIndex>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<DeleteSlotAndSetSelectedIndex>d__.<>4__this = this;
			<DeleteSlotAndSetSelectedIndex>d__.userId = userId;
			<DeleteSlotAndSetSelectedIndex>d__.deleteSlot = deleteSlot;
			<DeleteSlotAndSetSelectedIndex>d__.newSlot = newSlot;
			<DeleteSlotAndSetSelectedIndex>d__.<>1__state = -1;
			<DeleteSlotAndSetSelectedIndex>d__.<>t__builder.Start<ServerDbBase.<DeleteSlotAndSetSelectedIndex>d__5>(ref <DeleteSlotAndSetSelectedIndex>d__);
			return <DeleteSlotAndSetSelectedIndex>d__.<>t__builder.Task;
		}

		// Token: 0x06001E6C RID: 7788 RVA: 0x000A153C File Offset: 0x0009F73C
		public Task SaveAdminOOCColorAsync(NetUserId userId, Color color)
		{
			ServerDbBase.<SaveAdminOOCColorAsync>d__6 <SaveAdminOOCColorAsync>d__;
			<SaveAdminOOCColorAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SaveAdminOOCColorAsync>d__.<>4__this = this;
			<SaveAdminOOCColorAsync>d__.userId = userId;
			<SaveAdminOOCColorAsync>d__.color = color;
			<SaveAdminOOCColorAsync>d__.<>1__state = -1;
			<SaveAdminOOCColorAsync>d__.<>t__builder.Start<ServerDbBase.<SaveAdminOOCColorAsync>d__6>(ref <SaveAdminOOCColorAsync>d__);
			return <SaveAdminOOCColorAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001E6D RID: 7789 RVA: 0x000A1590 File Offset: 0x0009F790
		private static Task SetSelectedCharacterSlotAsync(NetUserId userId, int newSlot, ServerDbContext db)
		{
			ServerDbBase.<SetSelectedCharacterSlotAsync>d__7 <SetSelectedCharacterSlotAsync>d__;
			<SetSelectedCharacterSlotAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SetSelectedCharacterSlotAsync>d__.userId = userId;
			<SetSelectedCharacterSlotAsync>d__.newSlot = newSlot;
			<SetSelectedCharacterSlotAsync>d__.db = db;
			<SetSelectedCharacterSlotAsync>d__.<>1__state = -1;
			<SetSelectedCharacterSlotAsync>d__.<>t__builder.Start<ServerDbBase.<SetSelectedCharacterSlotAsync>d__7>(ref <SetSelectedCharacterSlotAsync>d__);
			return <SetSelectedCharacterSlotAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001E6E RID: 7790 RVA: 0x000A15E4 File Offset: 0x0009F7E4
		private static HumanoidCharacterProfile ConvertProfiles(Profile profile)
		{
			Dictionary<string, JobPriority> jobs = profile.Jobs.ToDictionary((Job j) => j.JobName, (Job j) => (JobPriority)j.Priority);
			IEnumerable<string> antags = from a in profile.Antags
			select a.AntagName;
			IEnumerable<string> traits = from t in profile.Traits
			select t.TraitName;
			Sex sex = Sex.Male;
			Sex sexVal;
			if (Enum.TryParse<Sex>(profile.Sex, true, out sexVal))
			{
				sex = sexVal;
			}
			ClothingPreference clothing = ClothingPreference.Jumpsuit;
			ClothingPreference clothingVal;
			if (Enum.TryParse<ClothingPreference>(profile.Clothing, true, out clothingVal))
			{
				clothing = clothingVal;
			}
			BackpackPreference backpack = BackpackPreference.Backpack;
			BackpackPreference backpackVal;
			if (Enum.TryParse<BackpackPreference>(profile.Backpack, true, out backpackVal))
			{
				backpack = backpackVal;
			}
			Gender gender = (sex == Sex.Male) ? 3 : 2;
			Gender genderVal;
			if (Enum.TryParse<Gender>(profile.Gender, true, out genderVal))
			{
				gender = genderVal;
			}
			string voice = profile.Voice;
			if (voice == string.Empty)
			{
				voice = SharedHumanoidAppearanceSystem.DefaultSexVoice[sex];
			}
			JsonDocument markings2 = profile.Markings;
			List<string> markingsRaw = (markings2 != null) ? markings2.Deserialize(null) : null;
			List<Marking> markings = new List<Marking>();
			if (markingsRaw != null)
			{
				foreach (string input in markingsRaw)
				{
					Marking parsed = Marking.ParseFromDbString(input);
					if (parsed != null)
					{
						markings.Add(parsed);
					}
				}
			}
			return new HumanoidCharacterProfile(profile.CharacterName, profile.FlavorText, profile.Species, voice, profile.Age, sex, gender, new HumanoidCharacterAppearance(profile.HairName, Color.FromHex(profile.HairColor, null), profile.FacialHairName, Color.FromHex(profile.FacialHairColor, null), Color.FromHex(profile.EyeColor, null), Color.FromHex(profile.SkinColor, null), markings), clothing, backpack, jobs, (PreferenceUnavailableMode)profile.PreferenceUnavailable, antags.ToList<string>(), traits.ToList<string>());
		}

		// Token: 0x06001E6F RID: 7791 RVA: 0x000A1838 File Offset: 0x0009FA38
		private static Profile ConvertProfiles(HumanoidCharacterProfile humanoid, int slot)
		{
			HumanoidCharacterAppearance appearance = (HumanoidCharacterAppearance)humanoid.CharacterAppearance;
			List<string> markingStrings = new List<string>();
			foreach (Marking marking in appearance.Markings)
			{
				markingStrings.Add(marking.ToString());
			}
			JsonDocument markings = JsonSerializer.SerializeToDocument<List<string>>(markingStrings, null);
			Profile profile = new Profile();
			profile.CharacterName = humanoid.Name;
			profile.FlavorText = humanoid.FlavorText;
			profile.Species = humanoid.Species;
			profile.Voice = humanoid.Voice;
			profile.Age = humanoid.Age;
			profile.Sex = humanoid.Sex.ToString();
			profile.Gender = humanoid.Gender.ToString();
			profile.HairName = appearance.HairStyleId;
			profile.HairColor = appearance.HairColor.ToHex();
			profile.FacialHairName = appearance.FacialHairStyleId;
			profile.FacialHairColor = appearance.FacialHairColor.ToHex();
			profile.EyeColor = appearance.EyeColor.ToHex();
			profile.SkinColor = appearance.SkinColor.ToHex();
			profile.Clothing = humanoid.Clothing.ToString();
			profile.Backpack = humanoid.Backpack.ToString();
			profile.Markings = markings;
			profile.Slot = slot;
			profile.PreferenceUnavailable = (DbPreferenceUnavailableMode)humanoid.PreferenceUnavailable;
			profile.Jobs.AddRange(from j in humanoid.JobPriorities
			where j.Value > JobPriority.Never
			select new Job
			{
				JobName = j.Key,
				Priority = (DbJobPriority)j.Value
			});
			profile.Antags.AddRange(from a in humanoid.AntagPreferences
			select new Antag
			{
				AntagName = a
			});
			profile.Traits.AddRange(from t in humanoid.TraitPreferences
			select new Trait
			{
				TraitName = t
			});
			return profile;
		}

		// Token: 0x06001E70 RID: 7792 RVA: 0x000A1AA4 File Offset: 0x0009FCA4
		public Task<NetUserId?> GetAssignedUserIdAsync(string name)
		{
			ServerDbBase.<GetAssignedUserIdAsync>d__10 <GetAssignedUserIdAsync>d__;
			<GetAssignedUserIdAsync>d__.<>t__builder = AsyncTaskMethodBuilder<NetUserId?>.Create();
			<GetAssignedUserIdAsync>d__.<>4__this = this;
			<GetAssignedUserIdAsync>d__.name = name;
			<GetAssignedUserIdAsync>d__.<>1__state = -1;
			<GetAssignedUserIdAsync>d__.<>t__builder.Start<ServerDbBase.<GetAssignedUserIdAsync>d__10>(ref <GetAssignedUserIdAsync>d__);
			return <GetAssignedUserIdAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001E71 RID: 7793 RVA: 0x000A1AF0 File Offset: 0x0009FCF0
		public Task AssignUserIdAsync(string name, NetUserId netUserId)
		{
			ServerDbBase.<AssignUserIdAsync>d__11 <AssignUserIdAsync>d__;
			<AssignUserIdAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<AssignUserIdAsync>d__.<>4__this = this;
			<AssignUserIdAsync>d__.name = name;
			<AssignUserIdAsync>d__.netUserId = netUserId;
			<AssignUserIdAsync>d__.<>1__state = -1;
			<AssignUserIdAsync>d__.<>t__builder.Start<ServerDbBase.<AssignUserIdAsync>d__11>(ref <AssignUserIdAsync>d__);
			return <AssignUserIdAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001E72 RID: 7794
		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		public abstract Task<ServerBanDef> GetServerBanAsync(int id);

		// Token: 0x06001E73 RID: 7795
		[NullableContext(0)]
		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		public abstract Task<ServerBanDef> GetServerBanAsync([Nullable(2)] IPAddress address, NetUserId? userId, ImmutableArray<byte>? hwId);

		// Token: 0x06001E74 RID: 7796
		[NullableContext(0)]
		[return: Nullable(1)]
		public abstract Task<List<ServerBanDef>> GetServerBansAsync([Nullable(2)] IPAddress address, NetUserId? userId, ImmutableArray<byte>? hwId, bool includeUnbanned);

		// Token: 0x06001E75 RID: 7797
		public abstract Task AddServerBanAsync(ServerBanDef serverBan);

		// Token: 0x06001E76 RID: 7798
		public abstract Task AddServerUnbanAsync(ServerUnbanDef serverUnban);

		// Token: 0x06001E77 RID: 7799
		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		public abstract Task<ServerRoleBanDef> GetServerRoleBanAsync(int id);

		// Token: 0x06001E78 RID: 7800
		[NullableContext(0)]
		[return: Nullable(1)]
		public abstract Task<List<ServerRoleBanDef>> GetServerRoleBansAsync([Nullable(2)] IPAddress address, NetUserId? userId, ImmutableArray<byte>? hwId, bool includeUnbanned);

		// Token: 0x06001E79 RID: 7801
		public abstract Task AddServerRoleBanAsync(ServerRoleBanDef serverRoleBan);

		// Token: 0x06001E7A RID: 7802
		public abstract Task AddServerRoleUnbanAsync(ServerRoleUnbanDef serverRoleUnban);

		// Token: 0x06001E7B RID: 7803 RVA: 0x000A1B44 File Offset: 0x0009FD44
		public Task<List<PlayTime>> GetPlayTimes(Guid player)
		{
			ServerDbBase.<GetPlayTimes>d__21 <GetPlayTimes>d__;
			<GetPlayTimes>d__.<>t__builder = AsyncTaskMethodBuilder<List<PlayTime>>.Create();
			<GetPlayTimes>d__.<>4__this = this;
			<GetPlayTimes>d__.player = player;
			<GetPlayTimes>d__.<>1__state = -1;
			<GetPlayTimes>d__.<>t__builder.Start<ServerDbBase.<GetPlayTimes>d__21>(ref <GetPlayTimes>d__);
			return <GetPlayTimes>d__.<>t__builder.Task;
		}

		// Token: 0x06001E7C RID: 7804 RVA: 0x000A1B90 File Offset: 0x0009FD90
		public Task UpdatePlayTimes(IReadOnlyCollection<PlayTimeUpdate> updates)
		{
			ServerDbBase.<UpdatePlayTimes>d__22 <UpdatePlayTimes>d__;
			<UpdatePlayTimes>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<UpdatePlayTimes>d__.<>4__this = this;
			<UpdatePlayTimes>d__.updates = updates;
			<UpdatePlayTimes>d__.<>1__state = -1;
			<UpdatePlayTimes>d__.<>t__builder.Start<ServerDbBase.<UpdatePlayTimes>d__22>(ref <UpdatePlayTimes>d__);
			return <UpdatePlayTimes>d__.<>t__builder.Task;
		}

		// Token: 0x06001E7D RID: 7805 RVA: 0x000A1BDC File Offset: 0x0009FDDC
		public Task UpdatePlayerRecord(NetUserId userId, string userName, IPAddress address, [Nullable(0)] ImmutableArray<byte> hwId)
		{
			ServerDbBase.<UpdatePlayerRecord>d__23 <UpdatePlayerRecord>d__;
			<UpdatePlayerRecord>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<UpdatePlayerRecord>d__.<>4__this = this;
			<UpdatePlayerRecord>d__.userId = userId;
			<UpdatePlayerRecord>d__.userName = userName;
			<UpdatePlayerRecord>d__.address = address;
			<UpdatePlayerRecord>d__.hwId = hwId;
			<UpdatePlayerRecord>d__.<>1__state = -1;
			<UpdatePlayerRecord>d__.<>t__builder.Start<ServerDbBase.<UpdatePlayerRecord>d__23>(ref <UpdatePlayerRecord>d__);
			return <UpdatePlayerRecord>d__.<>t__builder.Task;
		}

		// Token: 0x06001E7E RID: 7806 RVA: 0x000A1C40 File Offset: 0x0009FE40
		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		public Task<PlayerRecord> GetPlayerRecordByUserName(string userName, CancellationToken cancel)
		{
			ServerDbBase.<GetPlayerRecordByUserName>d__24 <GetPlayerRecordByUserName>d__;
			<GetPlayerRecordByUserName>d__.<>t__builder = AsyncTaskMethodBuilder<PlayerRecord>.Create();
			<GetPlayerRecordByUserName>d__.<>4__this = this;
			<GetPlayerRecordByUserName>d__.userName = userName;
			<GetPlayerRecordByUserName>d__.cancel = cancel;
			<GetPlayerRecordByUserName>d__.<>1__state = -1;
			<GetPlayerRecordByUserName>d__.<>t__builder.Start<ServerDbBase.<GetPlayerRecordByUserName>d__24>(ref <GetPlayerRecordByUserName>d__);
			return <GetPlayerRecordByUserName>d__.<>t__builder.Task;
		}

		// Token: 0x06001E7F RID: 7807 RVA: 0x000A1C94 File Offset: 0x0009FE94
		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		public Task<PlayerRecord> GetPlayerRecordByUserId(NetUserId userId, CancellationToken cancel)
		{
			ServerDbBase.<GetPlayerRecordByUserId>d__25 <GetPlayerRecordByUserId>d__;
			<GetPlayerRecordByUserId>d__.<>t__builder = AsyncTaskMethodBuilder<PlayerRecord>.Create();
			<GetPlayerRecordByUserId>d__.<>4__this = this;
			<GetPlayerRecordByUserId>d__.userId = userId;
			<GetPlayerRecordByUserId>d__.cancel = cancel;
			<GetPlayerRecordByUserId>d__.<>1__state = -1;
			<GetPlayerRecordByUserId>d__.<>t__builder.Start<ServerDbBase.<GetPlayerRecordByUserId>d__25>(ref <GetPlayerRecordByUserId>d__);
			return <GetPlayerRecordByUserId>d__.<>t__builder.Task;
		}

		// Token: 0x06001E80 RID: 7808
		protected abstract PlayerRecord MakePlayerRecord(Player player);

		// Token: 0x06001E81 RID: 7809
		public abstract Task<int> AddConnectionLogAsync(NetUserId userId, string userName, IPAddress address, [Nullable(0)] ImmutableArray<byte> hwId, ConnectionDenyReason? denied);

		// Token: 0x06001E82 RID: 7810 RVA: 0x000A1CE8 File Offset: 0x0009FEE8
		public Task AddServerBanHitsAsync(int connection, IEnumerable<ServerBanDef> bans)
		{
			ServerDbBase.<AddServerBanHitsAsync>d__28 <AddServerBanHitsAsync>d__;
			<AddServerBanHitsAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<AddServerBanHitsAsync>d__.<>4__this = this;
			<AddServerBanHitsAsync>d__.connection = connection;
			<AddServerBanHitsAsync>d__.bans = bans;
			<AddServerBanHitsAsync>d__.<>1__state = -1;
			<AddServerBanHitsAsync>d__.<>t__builder.Start<ServerDbBase.<AddServerBanHitsAsync>d__28>(ref <AddServerBanHitsAsync>d__);
			return <AddServerBanHitsAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001E83 RID: 7811 RVA: 0x000A1D3C File Offset: 0x0009FF3C
		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		public Task<Admin> GetAdminDataForAsync(NetUserId userId, CancellationToken cancel)
		{
			ServerDbBase.<GetAdminDataForAsync>d__29 <GetAdminDataForAsync>d__;
			<GetAdminDataForAsync>d__.<>t__builder = AsyncTaskMethodBuilder<Admin>.Create();
			<GetAdminDataForAsync>d__.<>4__this = this;
			<GetAdminDataForAsync>d__.userId = userId;
			<GetAdminDataForAsync>d__.cancel = cancel;
			<GetAdminDataForAsync>d__.<>1__state = -1;
			<GetAdminDataForAsync>d__.<>t__builder.Start<ServerDbBase.<GetAdminDataForAsync>d__29>(ref <GetAdminDataForAsync>d__);
			return <GetAdminDataForAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001E84 RID: 7812
		[return: TupleElementNames(new string[]
		{
			"admins",
			null,
			null,
			"lastUserName"
		})]
		[return: Nullable(new byte[]
		{
			1,
			0,
			1,
			0,
			1,
			2,
			1,
			1
		})]
		public abstract Task<ValueTuple<ValueTuple<Admin, string>[], AdminRank[]>> GetAllAdminAndRanksAsync(CancellationToken cancel);

		// Token: 0x06001E85 RID: 7813 RVA: 0x000A1D90 File Offset: 0x0009FF90
		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		public Task<AdminRank> GetAdminRankDataForAsync(int id, CancellationToken cancel = default(CancellationToken))
		{
			ServerDbBase.<GetAdminRankDataForAsync>d__31 <GetAdminRankDataForAsync>d__;
			<GetAdminRankDataForAsync>d__.<>t__builder = AsyncTaskMethodBuilder<AdminRank>.Create();
			<GetAdminRankDataForAsync>d__.<>4__this = this;
			<GetAdminRankDataForAsync>d__.id = id;
			<GetAdminRankDataForAsync>d__.cancel = cancel;
			<GetAdminRankDataForAsync>d__.<>1__state = -1;
			<GetAdminRankDataForAsync>d__.<>t__builder.Start<ServerDbBase.<GetAdminRankDataForAsync>d__31>(ref <GetAdminRankDataForAsync>d__);
			return <GetAdminRankDataForAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001E86 RID: 7814 RVA: 0x000A1DE4 File Offset: 0x0009FFE4
		public Task RemoveAdminAsync(NetUserId userId, CancellationToken cancel)
		{
			ServerDbBase.<RemoveAdminAsync>d__32 <RemoveAdminAsync>d__;
			<RemoveAdminAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<RemoveAdminAsync>d__.<>4__this = this;
			<RemoveAdminAsync>d__.userId = userId;
			<RemoveAdminAsync>d__.cancel = cancel;
			<RemoveAdminAsync>d__.<>1__state = -1;
			<RemoveAdminAsync>d__.<>t__builder.Start<ServerDbBase.<RemoveAdminAsync>d__32>(ref <RemoveAdminAsync>d__);
			return <RemoveAdminAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001E87 RID: 7815 RVA: 0x000A1E38 File Offset: 0x000A0038
		public Task AddAdminAsync(Admin admin, CancellationToken cancel)
		{
			ServerDbBase.<AddAdminAsync>d__33 <AddAdminAsync>d__;
			<AddAdminAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<AddAdminAsync>d__.<>4__this = this;
			<AddAdminAsync>d__.admin = admin;
			<AddAdminAsync>d__.cancel = cancel;
			<AddAdminAsync>d__.<>1__state = -1;
			<AddAdminAsync>d__.<>t__builder.Start<ServerDbBase.<AddAdminAsync>d__33>(ref <AddAdminAsync>d__);
			return <AddAdminAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001E88 RID: 7816 RVA: 0x000A1E8C File Offset: 0x000A008C
		public Task UpdateAdminAsync(Admin admin, CancellationToken cancel)
		{
			ServerDbBase.<UpdateAdminAsync>d__34 <UpdateAdminAsync>d__;
			<UpdateAdminAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<UpdateAdminAsync>d__.<>4__this = this;
			<UpdateAdminAsync>d__.admin = admin;
			<UpdateAdminAsync>d__.cancel = cancel;
			<UpdateAdminAsync>d__.<>1__state = -1;
			<UpdateAdminAsync>d__.<>t__builder.Start<ServerDbBase.<UpdateAdminAsync>d__34>(ref <UpdateAdminAsync>d__);
			return <UpdateAdminAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001E89 RID: 7817 RVA: 0x000A1EE0 File Offset: 0x000A00E0
		public Task RemoveAdminRankAsync(int rankId, CancellationToken cancel)
		{
			ServerDbBase.<RemoveAdminRankAsync>d__35 <RemoveAdminRankAsync>d__;
			<RemoveAdminRankAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<RemoveAdminRankAsync>d__.<>4__this = this;
			<RemoveAdminRankAsync>d__.rankId = rankId;
			<RemoveAdminRankAsync>d__.cancel = cancel;
			<RemoveAdminRankAsync>d__.<>1__state = -1;
			<RemoveAdminRankAsync>d__.<>t__builder.Start<ServerDbBase.<RemoveAdminRankAsync>d__35>(ref <RemoveAdminRankAsync>d__);
			return <RemoveAdminRankAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001E8A RID: 7818 RVA: 0x000A1F34 File Offset: 0x000A0134
		public Task AddAdminRankAsync(AdminRank rank, CancellationToken cancel)
		{
			ServerDbBase.<AddAdminRankAsync>d__36 <AddAdminRankAsync>d__;
			<AddAdminRankAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<AddAdminRankAsync>d__.<>4__this = this;
			<AddAdminRankAsync>d__.rank = rank;
			<AddAdminRankAsync>d__.cancel = cancel;
			<AddAdminRankAsync>d__.<>1__state = -1;
			<AddAdminRankAsync>d__.<>t__builder.Start<ServerDbBase.<AddAdminRankAsync>d__36>(ref <AddAdminRankAsync>d__);
			return <AddAdminRankAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001E8B RID: 7819 RVA: 0x000A1F88 File Offset: 0x000A0188
		public virtual Task<int> AddNewRound(Server server, params Guid[] playerIds)
		{
			ServerDbBase.<AddNewRound>d__37 <AddNewRound>d__;
			<AddNewRound>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
			<AddNewRound>d__.<>4__this = this;
			<AddNewRound>d__.server = server;
			<AddNewRound>d__.playerIds = playerIds;
			<AddNewRound>d__.<>1__state = -1;
			<AddNewRound>d__.<>t__builder.Start<ServerDbBase.<AddNewRound>d__37>(ref <AddNewRound>d__);
			return <AddNewRound>d__.<>t__builder.Task;
		}

		// Token: 0x06001E8C RID: 7820 RVA: 0x000A1FDC File Offset: 0x000A01DC
		public Task<Round> GetRound(int id)
		{
			ServerDbBase.<GetRound>d__38 <GetRound>d__;
			<GetRound>d__.<>t__builder = AsyncTaskMethodBuilder<Round>.Create();
			<GetRound>d__.<>4__this = this;
			<GetRound>d__.id = id;
			<GetRound>d__.<>1__state = -1;
			<GetRound>d__.<>t__builder.Start<ServerDbBase.<GetRound>d__38>(ref <GetRound>d__);
			return <GetRound>d__.<>t__builder.Task;
		}

		// Token: 0x06001E8D RID: 7821 RVA: 0x000A2028 File Offset: 0x000A0228
		public Task AddRoundPlayers(int id, Guid[] playerIds)
		{
			ServerDbBase.<AddRoundPlayers>d__39 <AddRoundPlayers>d__;
			<AddRoundPlayers>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<AddRoundPlayers>d__.<>4__this = this;
			<AddRoundPlayers>d__.id = id;
			<AddRoundPlayers>d__.playerIds = playerIds;
			<AddRoundPlayers>d__.<>1__state = -1;
			<AddRoundPlayers>d__.<>t__builder.Start<ServerDbBase.<AddRoundPlayers>d__39>(ref <AddRoundPlayers>d__);
			return <AddRoundPlayers>d__.<>t__builder.Task;
		}

		// Token: 0x06001E8E RID: 7822 RVA: 0x000A207C File Offset: 0x000A027C
		public Task UpdateAdminRankAsync(AdminRank rank, CancellationToken cancel)
		{
			ServerDbBase.<UpdateAdminRankAsync>d__40 <UpdateAdminRankAsync>d__;
			<UpdateAdminRankAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<UpdateAdminRankAsync>d__.<>4__this = this;
			<UpdateAdminRankAsync>d__.rank = rank;
			<UpdateAdminRankAsync>d__.cancel = cancel;
			<UpdateAdminRankAsync>d__.<>1__state = -1;
			<UpdateAdminRankAsync>d__.<>t__builder.Start<ServerDbBase.<UpdateAdminRankAsync>d__40>(ref <UpdateAdminRankAsync>d__);
			return <UpdateAdminRankAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001E8F RID: 7823 RVA: 0x000A20D0 File Offset: 0x000A02D0
		[return: TupleElementNames(new string[]
		{
			null,
			"existed"
		})]
		[return: Nullable(new byte[]
		{
			1,
			0,
			1
		})]
		public Task<ValueTuple<Server, bool>> AddOrGetServer(string serverName)
		{
			ServerDbBase.<AddOrGetServer>d__41 <AddOrGetServer>d__;
			<AddOrGetServer>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Server, bool>>.Create();
			<AddOrGetServer>d__.<>4__this = this;
			<AddOrGetServer>d__.serverName = serverName;
			<AddOrGetServer>d__.<>1__state = -1;
			<AddOrGetServer>d__.<>t__builder.Start<ServerDbBase.<AddOrGetServer>d__41>(ref <AddOrGetServer>d__);
			return <AddOrGetServer>d__.<>t__builder.Task;
		}

		// Token: 0x06001E90 RID: 7824 RVA: 0x000A211C File Offset: 0x000A031C
		public virtual Task AddAdminLogs(List<QueuedLog> logs)
		{
			ServerDbBase.<AddAdminLogs>d__42 <AddAdminLogs>d__;
			<AddAdminLogs>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<AddAdminLogs>d__.<>4__this = this;
			<AddAdminLogs>d__.logs = logs;
			<AddAdminLogs>d__.<>1__state = -1;
			<AddAdminLogs>d__.<>t__builder.Start<ServerDbBase.<AddAdminLogs>d__42>(ref <AddAdminLogs>d__);
			return <AddAdminLogs>d__.<>t__builder.Task;
		}

		// Token: 0x06001E91 RID: 7825 RVA: 0x000A2168 File Offset: 0x000A0368
		private Task<IQueryable<AdminLog>> GetAdminLogsQuery(ServerDbContext db, [Nullable(2)] LogFilter filter = null)
		{
			ServerDbBase.<GetAdminLogsQuery>d__43 <GetAdminLogsQuery>d__;
			<GetAdminLogsQuery>d__.<>t__builder = AsyncTaskMethodBuilder<IQueryable<AdminLog>>.Create();
			<GetAdminLogsQuery>d__.db = db;
			<GetAdminLogsQuery>d__.filter = filter;
			<GetAdminLogsQuery>d__.<>1__state = -1;
			<GetAdminLogsQuery>d__.<>t__builder.Start<ServerDbBase.<GetAdminLogsQuery>d__43>(ref <GetAdminLogsQuery>d__);
			return <GetAdminLogsQuery>d__.<>t__builder.Task;
		}

		// Token: 0x06001E92 RID: 7826 RVA: 0x000A21B3 File Offset: 0x000A03B3
		[AsyncIteratorStateMachine(typeof(ServerDbBase.<GetAdminLogMessages>d__44))]
		public IAsyncEnumerable<string> GetAdminLogMessages([Nullable(2)] LogFilter filter = null)
		{
			ServerDbBase.<GetAdminLogMessages>d__44 <GetAdminLogMessages>d__ = new ServerDbBase.<GetAdminLogMessages>d__44(-2);
			<GetAdminLogMessages>d__.<>4__this = this;
			<GetAdminLogMessages>d__.<>3__filter = filter;
			return <GetAdminLogMessages>d__;
		}

		// Token: 0x06001E93 RID: 7827 RVA: 0x000A21CA File Offset: 0x000A03CA
		[AsyncIteratorStateMachine(typeof(ServerDbBase.<GetAdminLogs>d__45))]
		public IAsyncEnumerable<SharedAdminLog> GetAdminLogs([Nullable(2)] LogFilter filter = null)
		{
			ServerDbBase.<GetAdminLogs>d__45 <GetAdminLogs>d__ = new ServerDbBase.<GetAdminLogs>d__45(-2);
			<GetAdminLogs>d__.<>4__this = this;
			<GetAdminLogs>d__.<>3__filter = filter;
			return <GetAdminLogs>d__;
		}

		// Token: 0x06001E94 RID: 7828 RVA: 0x000A21E1 File Offset: 0x000A03E1
		[AsyncIteratorStateMachine(typeof(ServerDbBase.<GetAdminLogsJson>d__46))]
		public IAsyncEnumerable<JsonDocument> GetAdminLogsJson([Nullable(2)] LogFilter filter = null)
		{
			ServerDbBase.<GetAdminLogsJson>d__46 <GetAdminLogsJson>d__ = new ServerDbBase.<GetAdminLogsJson>d__46(-2);
			<GetAdminLogsJson>d__.<>4__this = this;
			<GetAdminLogsJson>d__.<>3__filter = filter;
			return <GetAdminLogsJson>d__;
		}

		// Token: 0x06001E95 RID: 7829 RVA: 0x000A21F8 File Offset: 0x000A03F8
		public Task<bool> GetWhitelistStatusAsync(NetUserId player)
		{
			ServerDbBase.<GetWhitelistStatusAsync>d__47 <GetWhitelistStatusAsync>d__;
			<GetWhitelistStatusAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<GetWhitelistStatusAsync>d__.<>4__this = this;
			<GetWhitelistStatusAsync>d__.player = player;
			<GetWhitelistStatusAsync>d__.<>1__state = -1;
			<GetWhitelistStatusAsync>d__.<>t__builder.Start<ServerDbBase.<GetWhitelistStatusAsync>d__47>(ref <GetWhitelistStatusAsync>d__);
			return <GetWhitelistStatusAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001E96 RID: 7830 RVA: 0x000A2244 File Offset: 0x000A0444
		public Task AddToWhitelistAsync(NetUserId player)
		{
			ServerDbBase.<AddToWhitelistAsync>d__48 <AddToWhitelistAsync>d__;
			<AddToWhitelistAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<AddToWhitelistAsync>d__.<>4__this = this;
			<AddToWhitelistAsync>d__.player = player;
			<AddToWhitelistAsync>d__.<>1__state = -1;
			<AddToWhitelistAsync>d__.<>t__builder.Start<ServerDbBase.<AddToWhitelistAsync>d__48>(ref <AddToWhitelistAsync>d__);
			return <AddToWhitelistAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001E97 RID: 7831 RVA: 0x000A2290 File Offset: 0x000A0490
		public Task RemoveFromWhitelistAsync(NetUserId player)
		{
			ServerDbBase.<RemoveFromWhitelistAsync>d__49 <RemoveFromWhitelistAsync>d__;
			<RemoveFromWhitelistAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<RemoveFromWhitelistAsync>d__.<>4__this = this;
			<RemoveFromWhitelistAsync>d__.player = player;
			<RemoveFromWhitelistAsync>d__.<>1__state = -1;
			<RemoveFromWhitelistAsync>d__.<>t__builder.Start<ServerDbBase.<RemoveFromWhitelistAsync>d__49>(ref <RemoveFromWhitelistAsync>d__);
			return <RemoveFromWhitelistAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001E98 RID: 7832 RVA: 0x000A22DC File Offset: 0x000A04DC
		public Task<DateTime?> GetLastReadRules(NetUserId player)
		{
			ServerDbBase.<GetLastReadRules>d__50 <GetLastReadRules>d__;
			<GetLastReadRules>d__.<>t__builder = AsyncTaskMethodBuilder<DateTime?>.Create();
			<GetLastReadRules>d__.<>4__this = this;
			<GetLastReadRules>d__.player = player;
			<GetLastReadRules>d__.<>1__state = -1;
			<GetLastReadRules>d__.<>t__builder.Start<ServerDbBase.<GetLastReadRules>d__50>(ref <GetLastReadRules>d__);
			return <GetLastReadRules>d__.<>t__builder.Task;
		}

		// Token: 0x06001E99 RID: 7833 RVA: 0x000A2328 File Offset: 0x000A0528
		public Task SetLastReadRules(NetUserId player, DateTime date)
		{
			ServerDbBase.<SetLastReadRules>d__51 <SetLastReadRules>d__;
			<SetLastReadRules>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SetLastReadRules>d__.<>4__this = this;
			<SetLastReadRules>d__.player = player;
			<SetLastReadRules>d__.date = date;
			<SetLastReadRules>d__.<>1__state = -1;
			<SetLastReadRules>d__.<>t__builder.Start<ServerDbBase.<SetLastReadRules>d__51>(ref <SetLastReadRules>d__);
			return <SetLastReadRules>d__.<>t__builder.Task;
		}

		// Token: 0x06001E9A RID: 7834 RVA: 0x000A237C File Offset: 0x000A057C
		public Task AddUploadedResourceLogAsync(NetUserId user, DateTime date, string path, byte[] data)
		{
			ServerDbBase.<AddUploadedResourceLogAsync>d__52 <AddUploadedResourceLogAsync>d__;
			<AddUploadedResourceLogAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<AddUploadedResourceLogAsync>d__.<>4__this = this;
			<AddUploadedResourceLogAsync>d__.user = user;
			<AddUploadedResourceLogAsync>d__.date = date;
			<AddUploadedResourceLogAsync>d__.path = path;
			<AddUploadedResourceLogAsync>d__.data = data;
			<AddUploadedResourceLogAsync>d__.<>1__state = -1;
			<AddUploadedResourceLogAsync>d__.<>t__builder.Start<ServerDbBase.<AddUploadedResourceLogAsync>d__52>(ref <AddUploadedResourceLogAsync>d__);
			return <AddUploadedResourceLogAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001E9B RID: 7835 RVA: 0x000A23E0 File Offset: 0x000A05E0
		public Task PurgeUploadedResourceLogAsync(int days)
		{
			ServerDbBase.<PurgeUploadedResourceLogAsync>d__53 <PurgeUploadedResourceLogAsync>d__;
			<PurgeUploadedResourceLogAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<PurgeUploadedResourceLogAsync>d__.<>4__this = this;
			<PurgeUploadedResourceLogAsync>d__.days = days;
			<PurgeUploadedResourceLogAsync>d__.<>1__state = -1;
			<PurgeUploadedResourceLogAsync>d__.<>t__builder.Start<ServerDbBase.<PurgeUploadedResourceLogAsync>d__53>(ref <PurgeUploadedResourceLogAsync>d__);
			return <PurgeUploadedResourceLogAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001E9C RID: 7836 RVA: 0x000A242C File Offset: 0x000A062C
		public virtual Task<int> AddAdminNote(AdminNote note)
		{
			ServerDbBase.<AddAdminNote>d__54 <AddAdminNote>d__;
			<AddAdminNote>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
			<AddAdminNote>d__.<>4__this = this;
			<AddAdminNote>d__.note = note;
			<AddAdminNote>d__.<>1__state = -1;
			<AddAdminNote>d__.<>t__builder.Start<ServerDbBase.<AddAdminNote>d__54>(ref <AddAdminNote>d__);
			return <AddAdminNote>d__.<>t__builder.Task;
		}

		// Token: 0x06001E9D RID: 7837 RVA: 0x000A2478 File Offset: 0x000A0678
		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		public Task<AdminNote> GetAdminNote(int id)
		{
			ServerDbBase.<GetAdminNote>d__55 <GetAdminNote>d__;
			<GetAdminNote>d__.<>t__builder = AsyncTaskMethodBuilder<AdminNote>.Create();
			<GetAdminNote>d__.<>4__this = this;
			<GetAdminNote>d__.id = id;
			<GetAdminNote>d__.<>1__state = -1;
			<GetAdminNote>d__.<>t__builder.Start<ServerDbBase.<GetAdminNote>d__55>(ref <GetAdminNote>d__);
			return <GetAdminNote>d__.<>t__builder.Task;
		}

		// Token: 0x06001E9E RID: 7838 RVA: 0x000A24C4 File Offset: 0x000A06C4
		public Task<List<AdminNote>> GetAdminNotes(Guid player)
		{
			ServerDbBase.<GetAdminNotes>d__56 <GetAdminNotes>d__;
			<GetAdminNotes>d__.<>t__builder = AsyncTaskMethodBuilder<List<AdminNote>>.Create();
			<GetAdminNotes>d__.<>4__this = this;
			<GetAdminNotes>d__.player = player;
			<GetAdminNotes>d__.<>1__state = -1;
			<GetAdminNotes>d__.<>t__builder.Start<ServerDbBase.<GetAdminNotes>d__56>(ref <GetAdminNotes>d__);
			return <GetAdminNotes>d__.<>t__builder.Task;
		}

		// Token: 0x06001E9F RID: 7839 RVA: 0x000A2510 File Offset: 0x000A0710
		public Task DeleteAdminNote(int id, Guid deletedBy, DateTime deletedAt)
		{
			ServerDbBase.<DeleteAdminNote>d__57 <DeleteAdminNote>d__;
			<DeleteAdminNote>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<DeleteAdminNote>d__.<>4__this = this;
			<DeleteAdminNote>d__.id = id;
			<DeleteAdminNote>d__.deletedBy = deletedBy;
			<DeleteAdminNote>d__.deletedAt = deletedAt;
			<DeleteAdminNote>d__.<>1__state = -1;
			<DeleteAdminNote>d__.<>t__builder.Start<ServerDbBase.<DeleteAdminNote>d__57>(ref <DeleteAdminNote>d__);
			return <DeleteAdminNote>d__.<>t__builder.Task;
		}

		// Token: 0x06001EA0 RID: 7840 RVA: 0x000A256C File Offset: 0x000A076C
		public Task EditAdminNote(int id, string message, Guid editedBy, DateTime editedAt)
		{
			ServerDbBase.<EditAdminNote>d__58 <EditAdminNote>d__;
			<EditAdminNote>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<EditAdminNote>d__.<>4__this = this;
			<EditAdminNote>d__.id = id;
			<EditAdminNote>d__.message = message;
			<EditAdminNote>d__.editedBy = editedBy;
			<EditAdminNote>d__.editedAt = editedAt;
			<EditAdminNote>d__.<>1__state = -1;
			<EditAdminNote>d__.<>t__builder.Start<ServerDbBase.<EditAdminNote>d__58>(ref <EditAdminNote>d__);
			return <EditAdminNote>d__.<>t__builder.Task;
		}

		// Token: 0x06001EA1 RID: 7841
		protected abstract Task<ServerDbBase.DbGuard> GetDb();

		// Token: 0x02000A2C RID: 2604
		[NullableContext(0)]
		protected abstract class DbGuard : IAsyncDisposable
		{
			// Token: 0x17000835 RID: 2101
			// (get) Token: 0x06003466 RID: 13414
			[Nullable(1)]
			public abstract ServerDbContext DbContext { [NullableContext(1)] get; }

			// Token: 0x06003467 RID: 13415
			public abstract ValueTask DisposeAsync();
		}
	}
}
