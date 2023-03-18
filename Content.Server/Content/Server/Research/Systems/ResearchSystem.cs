using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Power.EntitySystems;
using Content.Server.Research.Components;
using Content.Shared.Research.Components;
using Content.Shared.Research.Prototypes;
using Content.Shared.Research.Systems;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Server.Research.Systems
{
	// Token: 0x0200023F RID: 575
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ResearchSystem : SharedResearchSystem
	{
		// Token: 0x06000B6F RID: 2927 RVA: 0x0003C5BC File Offset: 0x0003A7BC
		private void InitializeClient()
		{
			base.SubscribeLocalEvent<ResearchClientComponent, MapInitEvent>(new ComponentEventHandler<ResearchClientComponent, MapInitEvent>(this.OnClientMapInit), null, null);
			base.SubscribeLocalEvent<ResearchClientComponent, ComponentShutdown>(new ComponentEventHandler<ResearchClientComponent, ComponentShutdown>(this.OnClientShutdown), null, null);
			base.SubscribeLocalEvent<ResearchClientComponent, BoundUIOpenedEvent>(new ComponentEventHandler<ResearchClientComponent, BoundUIOpenedEvent>(this.OnClientUIOpen), null, null);
			base.SubscribeLocalEvent<ResearchClientComponent, ConsoleServerSyncMessage>(new ComponentEventHandler<ResearchClientComponent, ConsoleServerSyncMessage>(this.OnConsoleSync), null, null);
			base.SubscribeLocalEvent<ResearchClientComponent, ConsoleServerSelectionMessage>(new ComponentEventHandler<ResearchClientComponent, ConsoleServerSelectionMessage>(this.OnConsoleSelect), null, null);
			base.SubscribeLocalEvent<ResearchClientComponent, ResearchClientSyncMessage>(new ComponentEventHandler<ResearchClientComponent, ResearchClientSyncMessage>(this.OnClientSyncMessage), null, null);
			base.SubscribeLocalEvent<ResearchClientComponent, ResearchClientServerSelectedMessage>(new ComponentEventHandler<ResearchClientComponent, ResearchClientServerSelectedMessage>(this.OnClientSelected), null, null);
			base.SubscribeLocalEvent<ResearchClientComponent, ResearchClientServerDeselectedMessage>(new ComponentEventHandler<ResearchClientComponent, ResearchClientServerDeselectedMessage>(this.OnClientDeselected), null, null);
			base.SubscribeLocalEvent<ResearchClientComponent, ResearchRegistrationChangedEvent>(new ComponentEventRefHandler<ResearchClientComponent, ResearchRegistrationChangedEvent>(this.OnClientRegistrationChanged), null, null);
		}

		// Token: 0x06000B70 RID: 2928 RVA: 0x0003C680 File Offset: 0x0003A880
		private void OnClientSelected(EntityUid uid, ResearchClientComponent component, ResearchClientServerSelectedMessage args)
		{
			EntityUid? serveruid;
			ResearchServerComponent serverComponent;
			if (!this.TryGetServerById(args.ServerId, out serveruid, out serverComponent))
			{
				return;
			}
			this.UnregisterClient(uid, component, true);
			this.RegisterClient(uid, serveruid.Value, component, serverComponent, true);
		}

		// Token: 0x06000B71 RID: 2929 RVA: 0x0003C6BB File Offset: 0x0003A8BB
		private void OnClientDeselected(EntityUid uid, ResearchClientComponent component, ResearchClientServerDeselectedMessage args)
		{
			this.UnregisterClient(uid, component, true);
		}

		// Token: 0x06000B72 RID: 2930 RVA: 0x0003C6C6 File Offset: 0x0003A8C6
		private void OnClientSyncMessage(EntityUid uid, ResearchClientComponent component, ResearchClientSyncMessage args)
		{
			this.UpdateClientInterface(uid, component);
		}

		// Token: 0x06000B73 RID: 2931 RVA: 0x0003C6D0 File Offset: 0x0003A8D0
		private void OnConsoleSelect(EntityUid uid, ResearchClientComponent component, ConsoleServerSelectionMessage args)
		{
			if (!this.IsPowered(uid, this.EntityManager, null))
			{
				return;
			}
			this._uiSystem.TryToggleUi(uid, ResearchClientUiKey.Key, (IPlayerSession)args.Session, null);
		}

		// Token: 0x06000B74 RID: 2932 RVA: 0x0003C702 File Offset: 0x0003A902
		private void OnConsoleSync(EntityUid uid, ResearchClientComponent component, ConsoleServerSyncMessage args)
		{
			if (!this.IsPowered(uid, this.EntityManager, null))
			{
				return;
			}
			this.SyncClientWithServer(uid, null, null);
		}

		// Token: 0x06000B75 RID: 2933 RVA: 0x0003C71F File Offset: 0x0003A91F
		private void OnClientRegistrationChanged(EntityUid uid, ResearchClientComponent component, ref ResearchRegistrationChangedEvent args)
		{
			this.UpdateClientInterface(uid, component);
		}

		// Token: 0x06000B76 RID: 2934 RVA: 0x0003C72C File Offset: 0x0003A92C
		private void OnClientMapInit(EntityUid uid, ResearchClientComponent component, MapInitEvent args)
		{
			ResearchServerComponent[] allServers = base.EntityQuery<ResearchServerComponent>(true).ToArray<ResearchServerComponent>();
			if (allServers.Length != 0)
			{
				this.RegisterClient(uid, allServers[0].Owner, component, allServers[0], true);
			}
		}

		// Token: 0x06000B77 RID: 2935 RVA: 0x0003C75F File Offset: 0x0003A95F
		private void OnClientShutdown(EntityUid uid, ResearchClientComponent component, ComponentShutdown args)
		{
			this.UnregisterClient(uid, component, true);
		}

		// Token: 0x06000B78 RID: 2936 RVA: 0x0003C76A File Offset: 0x0003A96A
		private void OnClientUIOpen(EntityUid uid, ResearchClientComponent component, BoundUIOpenedEvent args)
		{
			this.UpdateClientInterface(uid, component);
		}

		// Token: 0x06000B79 RID: 2937 RVA: 0x0003C774 File Offset: 0x0003A974
		[NullableContext(2)]
		private void UpdateClientInterface(EntityUid uid, ResearchClientComponent component = null)
		{
			if (!base.Resolve<ResearchClientComponent>(uid, ref component, false))
			{
				return;
			}
			EntityUid? entityUid;
			ResearchServerComponent serverComponent;
			if (!this.TryGetClientServer(uid, out entityUid, out serverComponent, component))
			{
				return;
			}
			string[] names = this.GetServerNames();
			ResearchClientBoundInterfaceState state = new ResearchClientBoundInterfaceState(names.Length, names, this.GetServerIds(), component.ConnectedToServer ? serverComponent.Id : -1);
			this._uiSystem.TrySetUiState(uid, ResearchClientUiKey.Key, state, null, null, true);
		}

		// Token: 0x06000B7A RID: 2938 RVA: 0x0003C7DC File Offset: 0x0003A9DC
		[NullableContext(2)]
		public bool TryGetClientServer(EntityUid uid, [NotNullWhen(true)] out EntityUid? server, [NotNullWhen(true)] out ResearchServerComponent serverComponent, ResearchClientComponent component = null)
		{
			server = null;
			serverComponent = null;
			if (!base.Resolve<ResearchClientComponent>(uid, ref component, false))
			{
				return false;
			}
			if (component.Server == null)
			{
				return false;
			}
			ResearchServerComponent sc;
			if (!base.TryComp<ResearchServerComponent>(component.Server, ref sc))
			{
				return false;
			}
			server = component.Server;
			serverComponent = sc;
			return true;
		}

		// Token: 0x06000B7B RID: 2939 RVA: 0x0003C838 File Offset: 0x0003AA38
		private void InitializeConsole()
		{
			base.SubscribeLocalEvent<ResearchConsoleComponent, ConsoleUnlockTechnologyMessage>(new ComponentEventHandler<ResearchConsoleComponent, ConsoleUnlockTechnologyMessage>(this.OnConsoleUnlock), null, null);
			base.SubscribeLocalEvent<ResearchConsoleComponent, ResearchServerPointsChangedEvent>(new ComponentEventRefHandler<ResearchConsoleComponent, ResearchServerPointsChangedEvent>(this.OnPointsChanged), null, null);
			base.SubscribeLocalEvent<ResearchConsoleComponent, ResearchRegistrationChangedEvent>(new ComponentEventRefHandler<ResearchConsoleComponent, ResearchRegistrationChangedEvent>(this.OnConsoleRegistrationChanged), null, null);
			base.SubscribeLocalEvent<ResearchConsoleComponent, TechnologyDatabaseModifiedEvent>(new ComponentEventRefHandler<ResearchConsoleComponent, TechnologyDatabaseModifiedEvent>(this.OnConsoleDatabaseModified), null, null);
		}

		// Token: 0x06000B7C RID: 2940 RVA: 0x0003C895 File Offset: 0x0003AA95
		private void OnConsoleUnlock(EntityUid uid, ResearchConsoleComponent component, ConsoleUnlockTechnologyMessage args)
		{
			if (!this.IsPowered(uid, this.EntityManager, null))
			{
				return;
			}
			if (!this.UnlockTechnology(uid, args.Id, null, null))
			{
				return;
			}
			this.SyncClientWithServer(uid, null, null);
			this.UpdateConsoleInterface(uid, component, null);
		}

		// Token: 0x06000B7D RID: 2941 RVA: 0x0003C8D0 File Offset: 0x0003AAD0
		[NullableContext(2)]
		private void UpdateConsoleInterface(EntityUid uid, ResearchConsoleComponent component = null, ResearchClientComponent clientComponent = null)
		{
			if (!base.Resolve<ResearchConsoleComponent, ResearchClientComponent>(uid, ref component, ref clientComponent, false))
			{
				return;
			}
			EntityUid? server;
			ResearchServerComponent serverComponent;
			ResearchConsoleBoundInterfaceState state;
			if (this.TryGetClientServer(uid, out server, out serverComponent, clientComponent))
			{
				int points = clientComponent.ConnectedToServer ? serverComponent.Points : 0;
				int pointsPerSecond = clientComponent.ConnectedToServer ? this.PointsPerSecond(server.Value, serverComponent) : 0;
				state = new ResearchConsoleBoundInterfaceState(points, pointsPerSecond);
			}
			else
			{
				state = new ResearchConsoleBoundInterfaceState(0, 0);
			}
			this._uiSystem.TrySetUiState(component.Owner, ResearchConsoleUiKey.Key, state, null, null, true);
		}

		// Token: 0x06000B7E RID: 2942 RVA: 0x0003C952 File Offset: 0x0003AB52
		private void OnPointsChanged(EntityUid uid, ResearchConsoleComponent component, ref ResearchServerPointsChangedEvent args)
		{
			if (!this._uiSystem.IsUiOpen(uid, ResearchConsoleUiKey.Key, null))
			{
				return;
			}
			this.UpdateConsoleInterface(uid, component, null);
		}

		// Token: 0x06000B7F RID: 2943 RVA: 0x0003C973 File Offset: 0x0003AB73
		private void OnConsoleRegistrationChanged(EntityUid uid, ResearchConsoleComponent component, ref ResearchRegistrationChangedEvent args)
		{
			this.SyncClientWithServer(uid, null, null);
			this.UpdateConsoleInterface(uid, component, null);
		}

		// Token: 0x06000B80 RID: 2944 RVA: 0x0003C988 File Offset: 0x0003AB88
		private void OnConsoleDatabaseModified(EntityUid uid, ResearchConsoleComponent component, ref TechnologyDatabaseModifiedEvent args)
		{
			this.UpdateConsoleInterface(uid, component, null);
		}

		// Token: 0x06000B81 RID: 2945 RVA: 0x0003C993 File Offset: 0x0003AB93
		public override void Initialize()
		{
			base.Initialize();
			this.InitializeClient();
			this.InitializeConsole();
			this.InitializeSource();
			this.InitializeServer();
		}

		// Token: 0x06000B82 RID: 2946 RVA: 0x0003C9B4 File Offset: 0x0003ABB4
		[NullableContext(2)]
		public bool TryGetServerById(int id, [NotNullWhen(true)] out EntityUid? serverUid, [NotNullWhen(true)] out ResearchServerComponent serverComponent)
		{
			serverUid = null;
			serverComponent = null;
			foreach (ResearchServerComponent server in base.EntityQuery<ResearchServerComponent>(false))
			{
				if (server.Id == id)
				{
					serverUid = new EntityUid?(server.Owner);
					serverComponent = server;
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000B83 RID: 2947 RVA: 0x0003CA2C File Offset: 0x0003AC2C
		public string[] GetServerNames()
		{
			ResearchServerComponent[] allServers = base.EntityQuery<ResearchServerComponent>(true).ToArray<ResearchServerComponent>();
			string[] list = new string[allServers.Length];
			for (int i = 0; i < allServers.Length; i++)
			{
				list[i] = allServers[i].ServerName;
			}
			return list;
		}

		// Token: 0x06000B84 RID: 2948 RVA: 0x0003CA6C File Offset: 0x0003AC6C
		public int[] GetServerIds()
		{
			ResearchServerComponent[] allServers = base.EntityQuery<ResearchServerComponent>(true).ToArray<ResearchServerComponent>();
			int[] list = new int[allServers.Length];
			for (int i = 0; i < allServers.Length; i++)
			{
				list[i] = allServers[i].Id;
			}
			return list;
		}

		// Token: 0x06000B85 RID: 2949 RVA: 0x0003CAAC File Offset: 0x0003ACAC
		public override void Update(float frameTime)
		{
			foreach (ResearchServerComponent server in base.EntityQuery<ResearchServerComponent>(false))
			{
				if (!(server.NextUpdateTime > this._timing.CurTime))
				{
					server.NextUpdateTime = this._timing.CurTime + server.ResearchConsoleUpdateTime;
					this.UpdateServer(server.Owner, (int)server.ResearchConsoleUpdateTime.TotalSeconds, server);
				}
			}
		}

		// Token: 0x06000B86 RID: 2950 RVA: 0x0003CB40 File Offset: 0x0003AD40
		private void InitializeSource()
		{
			base.SubscribeLocalEvent<ResearchPointSourceComponent, ResearchServerGetPointsPerSecondEvent>(new ComponentEventRefHandler<ResearchPointSourceComponent, ResearchServerGetPointsPerSecondEvent>(this.OnGetPointsPerSecond), null, null);
		}

		// Token: 0x06000B87 RID: 2951 RVA: 0x0003CB56 File Offset: 0x0003AD56
		private void OnGetPointsPerSecond(EntityUid uid, ResearchPointSourceComponent component, ref ResearchServerGetPointsPerSecondEvent args)
		{
			if (this.CanProduce(component))
			{
				args.Points += component.PointsPerSecond;
			}
		}

		// Token: 0x06000B88 RID: 2952 RVA: 0x0003CB74 File Offset: 0x0003AD74
		public bool CanProduce(ResearchPointSourceComponent component)
		{
			return component.Active && this.IsPowered(component.Owner, this.EntityManager, null);
		}

		// Token: 0x06000B89 RID: 2953 RVA: 0x0003CB93 File Offset: 0x0003AD93
		private void InitializeServer()
		{
			base.SubscribeLocalEvent<ResearchServerComponent, ComponentStartup>(new ComponentEventHandler<ResearchServerComponent, ComponentStartup>(this.OnServerStartup), null, null);
			base.SubscribeLocalEvent<ResearchServerComponent, ComponentShutdown>(new ComponentEventHandler<ResearchServerComponent, ComponentShutdown>(this.OnServerShutdown), null, null);
			base.SubscribeLocalEvent<ResearchServerComponent, TechnologyDatabaseModifiedEvent>(new ComponentEventRefHandler<ResearchServerComponent, TechnologyDatabaseModifiedEvent>(this.OnServerDatabaseModified), null, null);
		}

		// Token: 0x06000B8A RID: 2954 RVA: 0x0003CBD4 File Offset: 0x0003ADD4
		private void OnServerStartup(EntityUid uid, ResearchServerComponent component, ComponentStartup args)
		{
			int unusedId = base.EntityQuery<ResearchServerComponent>(true).Max((ResearchServerComponent s) => s.Id) + 1;
			component.Id = unusedId;
			base.Dirty(component, null);
		}

		// Token: 0x06000B8B RID: 2955 RVA: 0x0003CC20 File Offset: 0x0003AE20
		private void OnServerShutdown(EntityUid uid, ResearchServerComponent component, ComponentShutdown args)
		{
			foreach (EntityUid client in new List<EntityUid>(component.Clients))
			{
				this.UnregisterClient(client, uid, null, component, false);
			}
		}

		// Token: 0x06000B8C RID: 2956 RVA: 0x0003CC7C File Offset: 0x0003AE7C
		private void OnServerDatabaseModified(EntityUid uid, ResearchServerComponent component, ref TechnologyDatabaseModifiedEvent args)
		{
			foreach (EntityUid client in component.Clients)
			{
				base.RaiseLocalEvent<TechnologyDatabaseModifiedEvent>(client, ref args, false);
			}
		}

		// Token: 0x06000B8D RID: 2957 RVA: 0x0003CCD4 File Offset: 0x0003AED4
		private bool CanRun(EntityUid uid)
		{
			return this.IsPowered(uid, this.EntityManager, null);
		}

		// Token: 0x06000B8E RID: 2958 RVA: 0x0003CCE4 File Offset: 0x0003AEE4
		[NullableContext(2)]
		private void UpdateServer(EntityUid uid, int time, ResearchServerComponent component = null)
		{
			if (!base.Resolve<ResearchServerComponent>(uid, ref component, true))
			{
				return;
			}
			if (!this.CanRun(uid))
			{
				return;
			}
			this.AddPointsToServer(uid, this.PointsPerSecond(uid, component) * time, component);
		}

		// Token: 0x06000B8F RID: 2959 RVA: 0x0003CD10 File Offset: 0x0003AF10
		[NullableContext(2)]
		public bool RegisterClient(EntityUid client, EntityUid server, ResearchClientComponent clientComponent = null, ResearchServerComponent serverComponent = null, bool dirtyServer = true)
		{
			if (!base.Resolve<ResearchClientComponent>(client, ref clientComponent, true) || !base.Resolve<ResearchServerComponent>(server, ref serverComponent, true))
			{
				return false;
			}
			if (serverComponent.Clients.Contains(client))
			{
				return false;
			}
			serverComponent.Clients.Add(client);
			clientComponent.Server = new EntityUid?(server);
			if (dirtyServer)
			{
				base.Dirty(serverComponent, null);
			}
			ResearchRegistrationChangedEvent ev = new ResearchRegistrationChangedEvent(new EntityUid?(server));
			base.RaiseLocalEvent<ResearchRegistrationChangedEvent>(client, ref ev, false);
			return true;
		}

		// Token: 0x06000B90 RID: 2960 RVA: 0x0003CD88 File Offset: 0x0003AF88
		[NullableContext(2)]
		public void UnregisterClient(EntityUid client, ResearchClientComponent clientComponent = null, bool dirtyServer = true)
		{
			if (!base.Resolve<ResearchClientComponent>(client, ref clientComponent, true))
			{
				return;
			}
			EntityUid? server2 = clientComponent.Server;
			if (server2 != null)
			{
				EntityUid server = server2.GetValueOrDefault();
				this.UnregisterClient(client, server, clientComponent, null, dirtyServer);
				return;
			}
		}

		// Token: 0x06000B91 RID: 2961 RVA: 0x0003CDC8 File Offset: 0x0003AFC8
		[NullableContext(2)]
		public void UnregisterClient(EntityUid client, EntityUid server, ResearchClientComponent clientComponent = null, ResearchServerComponent serverComponent = null, bool dirtyServer = true)
		{
			if (!base.Resolve<ResearchClientComponent>(client, ref clientComponent, true) || !base.Resolve<ResearchServerComponent>(server, ref serverComponent, true))
			{
				return;
			}
			serverComponent.Clients.Remove(client);
			clientComponent.Server = null;
			if (dirtyServer)
			{
				base.Dirty(serverComponent, null);
			}
			ResearchRegistrationChangedEvent ev = new ResearchRegistrationChangedEvent(null);
			base.RaiseLocalEvent<ResearchRegistrationChangedEvent>(client, ref ev, false);
		}

		// Token: 0x06000B92 RID: 2962 RVA: 0x0003CE34 File Offset: 0x0003B034
		[NullableContext(2)]
		public int PointsPerSecond(EntityUid uid, ResearchServerComponent component = null)
		{
			int points = 0;
			if (!base.Resolve<ResearchServerComponent>(uid, ref component, true))
			{
				return points;
			}
			if (!this.CanRun(uid))
			{
				return points;
			}
			ResearchServerGetPointsPerSecondEvent ev = new ResearchServerGetPointsPerSecondEvent(uid, points);
			foreach (EntityUid client in component.Clients)
			{
				base.RaiseLocalEvent<ResearchServerGetPointsPerSecondEvent>(client, ref ev, false);
			}
			return ev.Points;
		}

		// Token: 0x06000B93 RID: 2963 RVA: 0x0003CEB8 File Offset: 0x0003B0B8
		[NullableContext(2)]
		public void AddPointsToServer(EntityUid uid, int points, ResearchServerComponent component = null)
		{
			if (points == 0)
			{
				return;
			}
			if (!base.Resolve<ResearchServerComponent>(uid, ref component, true))
			{
				return;
			}
			component.Points += points;
			ResearchServerPointsChangedEvent ev = new ResearchServerPointsChangedEvent(uid, component.Points, points);
			foreach (EntityUid client in component.Clients)
			{
				base.RaiseLocalEvent<ResearchServerPointsChangedEvent>(client, ref ev, false);
			}
			base.Dirty(component, null);
		}

		// Token: 0x06000B94 RID: 2964 RVA: 0x0003CF44 File Offset: 0x0003B144
		[NullableContext(2)]
		public void Sync(EntityUid primaryUid, EntityUid otherUid, TechnologyDatabaseComponent primaryDb = null, TechnologyDatabaseComponent otherDb = null)
		{
			if (!base.Resolve<TechnologyDatabaseComponent>(primaryUid, ref primaryDb, true) || !base.Resolve<TechnologyDatabaseComponent>(otherUid, ref otherDb, true))
			{
				return;
			}
			primaryDb.TechnologyIds = otherDb.TechnologyIds;
			primaryDb.RecipeIds = otherDb.RecipeIds;
			base.Dirty(primaryDb, null);
			TechnologyDatabaseModifiedEvent ev = default(TechnologyDatabaseModifiedEvent);
			base.RaiseLocalEvent<TechnologyDatabaseModifiedEvent>(primaryDb.Owner, ref ev, false);
		}

		// Token: 0x06000B95 RID: 2965 RVA: 0x0003CFA4 File Offset: 0x0003B1A4
		[NullableContext(2)]
		public bool SyncClientWithServer(EntityUid uid, TechnologyDatabaseComponent databaseComponent = null, ResearchClientComponent clientComponent = null)
		{
			if (!base.Resolve<TechnologyDatabaseComponent, ResearchClientComponent>(uid, ref databaseComponent, ref clientComponent, false))
			{
				return false;
			}
			TechnologyDatabaseComponent serverDatabase;
			if (!base.TryComp<TechnologyDatabaseComponent>(clientComponent.Server, ref serverDatabase))
			{
				return false;
			}
			this.Sync(uid, clientComponent.Server.Value, databaseComponent, serverDatabase);
			return true;
		}

		// Token: 0x06000B96 RID: 2966 RVA: 0x0003CFEC File Offset: 0x0003B1EC
		[NullableContext(2)]
		public bool UnlockTechnology(EntityUid client, [Nullable(1)] string prototypeid, ResearchClientComponent component = null, TechnologyDatabaseComponent databaseComponent = null)
		{
			TechnologyPrototype prototype;
			if (!this._prototypeManager.TryIndex<TechnologyPrototype>(prototypeid, ref prototype))
			{
				Logger.Error("invalid technology prototype");
				return false;
			}
			return this.UnlockTechnology(client, prototype, component, databaseComponent);
		}

		// Token: 0x06000B97 RID: 2967 RVA: 0x0003D020 File Offset: 0x0003B220
		[NullableContext(2)]
		public bool UnlockTechnology(EntityUid client, [Nullable(1)] TechnologyPrototype prototype, ResearchClientComponent component = null, TechnologyDatabaseComponent databaseComponent = null)
		{
			if (!base.Resolve<ResearchClientComponent, TechnologyDatabaseComponent>(client, ref component, ref databaseComponent, false))
			{
				return false;
			}
			if (!this.CanUnlockTechnology(client, prototype, databaseComponent, null))
			{
				return false;
			}
			EntityUid? server2 = component.Server;
			if (server2 != null)
			{
				EntityUid server = server2.GetValueOrDefault();
				this.AddTechnology(server, prototype.ID, null);
				this.AddPointsToServer(server, -prototype.RequiredPoints, null);
				return true;
			}
			return false;
		}

		// Token: 0x06000B98 RID: 2968 RVA: 0x0003D088 File Offset: 0x0003B288
		public void AddTechnology(EntityUid uid, string technology, [Nullable(2)] TechnologyDatabaseComponent component = null)
		{
			if (!base.Resolve<TechnologyDatabaseComponent>(uid, ref component, true))
			{
				return;
			}
			TechnologyPrototype prototype;
			if (!this._prototypeManager.TryIndex<TechnologyPrototype>(technology, ref prototype))
			{
				return;
			}
			this.AddTechnology(uid, prototype, component);
		}

		// Token: 0x06000B99 RID: 2969 RVA: 0x0003D0BC File Offset: 0x0003B2BC
		public void AddTechnology(EntityUid uid, TechnologyPrototype technology, [Nullable(2)] TechnologyDatabaseComponent component = null)
		{
			if (!base.Resolve<TechnologyDatabaseComponent>(uid, ref component, true))
			{
				return;
			}
			component.TechnologyIds.Add(technology.ID);
			foreach (string unlock in technology.UnlockedRecipes)
			{
				if (!component.RecipeIds.Contains(unlock))
				{
					component.RecipeIds.Add(unlock);
				}
			}
			base.Dirty(component, null);
			TechnologyDatabaseModifiedEvent ev = default(TechnologyDatabaseModifiedEvent);
			base.RaiseLocalEvent<TechnologyDatabaseModifiedEvent>(uid, ref ev, false);
		}

		// Token: 0x06000B9A RID: 2970 RVA: 0x0003D15C File Offset: 0x0003B35C
		public void AddLatheRecipe(EntityUid uid, string recipe, [Nullable(2)] TechnologyDatabaseComponent component = null, bool dirty = true)
		{
			if (!base.Resolve<TechnologyDatabaseComponent>(uid, ref component, true))
			{
				return;
			}
			if (component.RecipeIds.Contains(recipe))
			{
				return;
			}
			component.RecipeIds.Add(recipe);
			if (dirty)
			{
				base.Dirty(component, null);
			}
			TechnologyDatabaseModifiedEvent ev = default(TechnologyDatabaseModifiedEvent);
			base.RaiseLocalEvent<TechnologyDatabaseModifiedEvent>(uid, ref ev, false);
		}

		// Token: 0x06000B9B RID: 2971 RVA: 0x0003D1B0 File Offset: 0x0003B3B0
		[NullableContext(2)]
		public bool CanUnlockTechnology(EntityUid uid, [Nullable(1)] string technology, TechnologyDatabaseComponent database = null, ResearchClientComponent client = null)
		{
			TechnologyPrototype prototype;
			return this._prototypeManager.TryIndex<TechnologyPrototype>(technology, ref prototype) && this.CanUnlockTechnology(uid, prototype, database, client);
		}

		// Token: 0x06000B9C RID: 2972 RVA: 0x0003D1DC File Offset: 0x0003B3DC
		[NullableContext(2)]
		public bool CanUnlockTechnology(EntityUid uid, [Nullable(1)] TechnologyPrototype technology, TechnologyDatabaseComponent database = null, ResearchClientComponent client = null)
		{
			EntityUid? entityUid;
			ResearchServerComponent serverComponent;
			return base.Resolve<TechnologyDatabaseComponent, ResearchClientComponent>(uid, ref database, ref client, true) && this.TryGetClientServer(uid, out entityUid, out serverComponent, client) && serverComponent.Points >= technology.RequiredPoints && !base.IsTechnologyUnlocked(uid, technology, database) && base.ArePrerequesitesUnlocked(uid, technology, database);
		}

		// Token: 0x04000713 RID: 1811
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x04000714 RID: 1812
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000715 RID: 1813
		[Dependency]
		private readonly UserInterfaceSystem _uiSystem;
	}
}
