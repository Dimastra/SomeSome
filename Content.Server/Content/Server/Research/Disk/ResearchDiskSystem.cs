using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Popups;
using Content.Server.Research.Systems;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Research.Components;
using Content.Shared.Research.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;

namespace Content.Server.Research.Disk
{
	// Token: 0x02000241 RID: 577
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ResearchDiskSystem : EntitySystem
	{
		// Token: 0x06000B9F RID: 2975 RVA: 0x0003D250 File Offset: 0x0003B450
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ResearchDiskComponent, AfterInteractEvent>(new ComponentEventHandler<ResearchDiskComponent, AfterInteractEvent>(this.OnAfterInteract), null, null);
			base.SubscribeLocalEvent<ResearchDiskComponent, MapInitEvent>(new ComponentEventHandler<ResearchDiskComponent, MapInitEvent>(this.OnMapInit), null, null);
		}

		// Token: 0x06000BA0 RID: 2976 RVA: 0x0003D280 File Offset: 0x0003B480
		private void OnAfterInteract(EntityUid uid, ResearchDiskComponent component, AfterInteractEvent args)
		{
			if (!args.CanReach)
			{
				return;
			}
			ResearchServerComponent server;
			if (!base.TryComp<ResearchServerComponent>(args.Target, ref server))
			{
				return;
			}
			this._research.AddPointsToServer(server.Owner, component.Points, server);
			this._popupSystem.PopupEntity(Loc.GetString("research-disk-inserted", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("points", component.Points)
			}), args.Target.Value, args.User, PopupType.Small);
			this.EntityManager.QueueDeleteEntity(uid);
		}

		// Token: 0x06000BA1 RID: 2977 RVA: 0x0003D317 File Offset: 0x0003B517
		private void OnMapInit(EntityUid uid, ResearchDiskComponent component, MapInitEvent args)
		{
			if (!component.UnlockAllTech)
			{
				return;
			}
			component.Points = this._prototype.EnumeratePrototypes<TechnologyPrototype>().Sum((TechnologyPrototype tech) => tech.RequiredPoints);
		}

		// Token: 0x04000718 RID: 1816
		[Dependency]
		private readonly IPrototypeManager _prototype;

		// Token: 0x04000719 RID: 1817
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x0400071A RID: 1818
		[Dependency]
		private readonly ResearchSystem _research;
	}
}
