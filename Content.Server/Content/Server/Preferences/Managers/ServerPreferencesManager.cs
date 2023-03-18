using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Content.Server.Database;
using Content.Server.White.Sponsors;
using Content.Shared.CCVar;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Preferences;
using Content.Shared.Roles;
using Content.Shared.White.Sponsors;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Server.Preferences.Managers
{
	// Token: 0x0200026E RID: 622
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ServerPreferencesManager : IServerPreferencesManager
	{
		// Token: 0x06000C66 RID: 3174 RVA: 0x00040DA8 File Offset: 0x0003EFA8
		public void Init()
		{
			this._netManager.RegisterNetMessage<MsgPreferencesAndSettings>(null, 3);
			this._netManager.RegisterNetMessage<MsgSelectCharacter>(new ProcessMessage<MsgSelectCharacter>(this.HandleSelectCharacterMessage), 3);
			this._netManager.RegisterNetMessage<MsgUpdateCharacter>(new ProcessMessage<MsgUpdateCharacter>(this.HandleUpdateCharacterMessage), 3);
			this._netManager.RegisterNetMessage<MsgDeleteCharacter>(new ProcessMessage<MsgDeleteCharacter>(this.HandleDeleteCharacterMessage), 3);
		}

		// Token: 0x06000C67 RID: 3175 RVA: 0x00040E0C File Offset: 0x0003F00C
		private void HandleSelectCharacterMessage(MsgSelectCharacter message)
		{
			ServerPreferencesManager.<HandleSelectCharacterMessage>d__7 <HandleSelectCharacterMessage>d__;
			<HandleSelectCharacterMessage>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<HandleSelectCharacterMessage>d__.<>4__this = this;
			<HandleSelectCharacterMessage>d__.message = message;
			<HandleSelectCharacterMessage>d__.<>1__state = -1;
			<HandleSelectCharacterMessage>d__.<>t__builder.Start<ServerPreferencesManager.<HandleSelectCharacterMessage>d__7>(ref <HandleSelectCharacterMessage>d__);
		}

		// Token: 0x06000C68 RID: 3176 RVA: 0x00040E4C File Offset: 0x0003F04C
		private void HandleUpdateCharacterMessage(MsgUpdateCharacter message)
		{
			ServerPreferencesManager.<HandleUpdateCharacterMessage>d__8 <HandleUpdateCharacterMessage>d__;
			<HandleUpdateCharacterMessage>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<HandleUpdateCharacterMessage>d__.<>4__this = this;
			<HandleUpdateCharacterMessage>d__.message = message;
			<HandleUpdateCharacterMessage>d__.<>1__state = -1;
			<HandleUpdateCharacterMessage>d__.<>t__builder.Start<ServerPreferencesManager.<HandleUpdateCharacterMessage>d__8>(ref <HandleUpdateCharacterMessage>d__);
		}

		// Token: 0x06000C69 RID: 3177 RVA: 0x00040E8C File Offset: 0x0003F08C
		private void HandleDeleteCharacterMessage(MsgDeleteCharacter message)
		{
			ServerPreferencesManager.<HandleDeleteCharacterMessage>d__9 <HandleDeleteCharacterMessage>d__;
			<HandleDeleteCharacterMessage>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<HandleDeleteCharacterMessage>d__.<>4__this = this;
			<HandleDeleteCharacterMessage>d__.message = message;
			<HandleDeleteCharacterMessage>d__.<>1__state = -1;
			<HandleDeleteCharacterMessage>d__.<>t__builder.Start<ServerPreferencesManager.<HandleDeleteCharacterMessage>d__9>(ref <HandleDeleteCharacterMessage>d__);
		}

		// Token: 0x06000C6A RID: 3178 RVA: 0x00040ECC File Offset: 0x0003F0CC
		public Task LoadData(IPlayerSession session, CancellationToken cancel)
		{
			ServerPreferencesManager.<LoadData>d__10 <LoadData>d__;
			<LoadData>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<LoadData>d__.<>4__this = this;
			<LoadData>d__.session = session;
			<LoadData>d__.<>1__state = -1;
			<LoadData>d__.<>t__builder.Start<ServerPreferencesManager.<LoadData>d__10>(ref <LoadData>d__);
			return <LoadData>d__.<>t__builder.Task;
		}

		// Token: 0x06000C6B RID: 3179 RVA: 0x00040F17 File Offset: 0x0003F117
		public void OnClientDisconnected(IPlayerSession session)
		{
			this._cachedPlayerPrefs.Remove(session.UserId);
		}

		// Token: 0x06000C6C RID: 3180 RVA: 0x00040F2B File Offset: 0x0003F12B
		public bool HavePreferencesLoaded(IPlayerSession session)
		{
			return this._cachedPlayerPrefs.ContainsKey(session.UserId);
		}

		// Token: 0x06000C6D RID: 3181 RVA: 0x00040F40 File Offset: 0x0003F140
		private int GetMaxUserCharacterSlots(NetUserId userId)
		{
			int cvar = this._cfg.GetCVar<int>(CCVars.GameMaxCharacterSlots);
			SponsorInfo sponsor;
			int extraSlots = this._sponsors.TryGetInfo(userId, out sponsor) ? sponsor.ExtraSlots : 0;
			return cvar + extraSlots;
		}

		// Token: 0x06000C6E RID: 3182 RVA: 0x00040F7C File Offset: 0x0003F17C
		[NullableContext(2)]
		public bool TryGetCachedPreferences(NetUserId userId, [NotNullWhen(true)] out PlayerPreferences playerPreferences)
		{
			ServerPreferencesManager.PlayerPrefData prefs;
			if (this._cachedPlayerPrefs.TryGetValue(userId, out prefs))
			{
				playerPreferences = prefs.Prefs;
				return prefs.Prefs != null;
			}
			playerPreferences = null;
			return false;
		}

		// Token: 0x06000C6F RID: 3183 RVA: 0x00040FAF File Offset: 0x0003F1AF
		public PlayerPreferences GetPreferences(NetUserId userId)
		{
			PlayerPreferences prefs = this._cachedPlayerPrefs[userId].Prefs;
			if (prefs == null)
			{
				throw new InvalidOperationException("Preferences for this player have not loaded yet.");
			}
			return prefs;
		}

		// Token: 0x06000C70 RID: 3184 RVA: 0x00040FD0 File Offset: 0x0003F1D0
		private Task<PlayerPreferences> GetOrCreatePreferencesAsync(NetUserId userId)
		{
			ServerPreferencesManager.<GetOrCreatePreferencesAsync>d__16 <GetOrCreatePreferencesAsync>d__;
			<GetOrCreatePreferencesAsync>d__.<>t__builder = AsyncTaskMethodBuilder<PlayerPreferences>.Create();
			<GetOrCreatePreferencesAsync>d__.<>4__this = this;
			<GetOrCreatePreferencesAsync>d__.userId = userId;
			<GetOrCreatePreferencesAsync>d__.<>1__state = -1;
			<GetOrCreatePreferencesAsync>d__.<>t__builder.Start<ServerPreferencesManager.<GetOrCreatePreferencesAsync>d__16>(ref <GetOrCreatePreferencesAsync>d__);
			return <GetOrCreatePreferencesAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06000C71 RID: 3185 RVA: 0x0004101B File Offset: 0x0003F21B
		private PlayerPreferences SanitizePreferences(PlayerPreferences prefs)
		{
			return new PlayerPreferences(prefs.Characters.Select(delegate(KeyValuePair<int, ICharacterProfile> p)
			{
				HumanoidCharacterProfile hp = p.Value as HumanoidCharacterProfile;
				if (hp != null)
				{
					IPrototypeManager prototypeManager = IoCManager.Resolve<IPrototypeManager>();
					string selectedSpecies = "Human";
					SpeciesPrototype species;
					if (prototypeManager.TryIndex<SpeciesPrototype>(hp.Species, ref species) && species.RoundStart)
					{
						selectedSpecies = hp.Species;
					}
					ICharacterProfile newProf = hp.WithJobPriorities(from job in hp.JobPriorities
					where this._protos.HasIndex<JobPrototype>(job.Key)
					select job).WithAntagPreferences(from antag in hp.AntagPreferences
					where this._protos.HasIndex<AntagPrototype>(antag)
					select antag).WithSpecies(selectedSpecies);
					return new KeyValuePair<int, ICharacterProfile>(p.Key, newProf);
				}
				throw new NotSupportedException();
			}), prefs.SelectedCharacterIndex, prefs.AdminOOCColor);
		}

		// Token: 0x06000C72 RID: 3186 RVA: 0x00041048 File Offset: 0x0003F248
		[return: Nullable(new byte[]
		{
			1,
			0,
			1
		})]
		public IEnumerable<KeyValuePair<NetUserId, ICharacterProfile>> GetSelectedProfilesForPlayers(List<NetUserId> usernames)
		{
			return (from p in usernames
			select new ValueTuple<PlayerPreferences, NetUserId>(this._cachedPlayerPrefs[p].Prefs, p) into p
			where p.Item1 != null
			select p).Select(delegate([TupleElementNames(new string[]
			{
				"Prefs",
				"p"
			})] ValueTuple<PlayerPreferences, NetUserId> p)
			{
				int idx = p.Item1.SelectedCharacterIndex;
				return new KeyValuePair<NetUserId, ICharacterProfile>(p.Item2, p.Item1.GetProfile(idx));
			});
		}

		// Token: 0x06000C73 RID: 3187 RVA: 0x000410AF File Offset: 0x0003F2AF
		internal static bool ShouldStorePrefs(LoginType loginType)
		{
			return LoginTypeExt.HasStaticUserId(loginType);
		}

		// Token: 0x040007A6 RID: 1958
		[Dependency]
		private readonly IServerNetManager _netManager;

		// Token: 0x040007A7 RID: 1959
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x040007A8 RID: 1960
		[Dependency]
		private readonly IServerDbManager _db;

		// Token: 0x040007A9 RID: 1961
		[Dependency]
		private readonly IPrototypeManager _protos;

		// Token: 0x040007AA RID: 1962
		[Dependency]
		private readonly SponsorsManager _sponsors;

		// Token: 0x040007AB RID: 1963
		private readonly Dictionary<NetUserId, ServerPreferencesManager.PlayerPrefData> _cachedPlayerPrefs = new Dictionary<NetUserId, ServerPreferencesManager.PlayerPrefData>();

		// Token: 0x02000925 RID: 2341
		[NullableContext(0)]
		private sealed class PlayerPrefData
		{
			// Token: 0x04001EF0 RID: 7920
			public bool PrefsLoaded;

			// Token: 0x04001EF1 RID: 7921
			[Nullable(2)]
			public PlayerPreferences Prefs;
		}
	}
}
