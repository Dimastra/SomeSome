using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Botany.Components;
using Content.Server.DoAfter;
using Content.Server.Popups;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Botany.Systems
{
	// Token: 0x020006FB RID: 1787
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BotanySwabSystem : EntitySystem
	{
		// Token: 0x06002567 RID: 9575 RVA: 0x000C4264 File Offset: 0x000C2464
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<BotanySwabComponent, ExaminedEvent>(new ComponentEventHandler<BotanySwabComponent, ExaminedEvent>(this.OnExamined), null, null);
			base.SubscribeLocalEvent<BotanySwabComponent, AfterInteractEvent>(new ComponentEventHandler<BotanySwabComponent, AfterInteractEvent>(this.OnAfterInteract), null, null);
			base.SubscribeLocalEvent<BotanySwabComponent, DoAfterEvent>(new ComponentEventHandler<BotanySwabComponent, DoAfterEvent>(this.OnDoAfter), null, null);
		}

		// Token: 0x06002568 RID: 9576 RVA: 0x000C42B3 File Offset: 0x000C24B3
		private void OnExamined(EntityUid uid, BotanySwabComponent swab, ExaminedEvent args)
		{
			if (args.IsInDetailsRange)
			{
				if (swab.SeedData != null)
				{
					args.PushMarkup(Loc.GetString("swab-used"));
					return;
				}
				args.PushMarkup(Loc.GetString("swab-unused"));
			}
		}

		// Token: 0x06002569 RID: 9577 RVA: 0x000C42E8 File Offset: 0x000C24E8
		private void OnAfterInteract(EntityUid uid, BotanySwabComponent swab, AfterInteractEvent args)
		{
			if (args.Target == null || !args.CanReach || !base.HasComp<PlantHolderComponent>(args.Target))
			{
				return;
			}
			SharedDoAfterSystem doAfterSystem = this._doAfterSystem;
			EntityUid user = args.User;
			float swabDelay = swab.SwabDelay;
			EntityUid? target = args.Target;
			EntityUid? used = new EntityUid?(uid);
			doAfterSystem.DoAfter(new DoAfterEventArgs(user, swabDelay, default(CancellationToken), target, used)
			{
				Broadcast = true,
				BreakOnTargetMove = true,
				BreakOnUserMove = true,
				BreakOnStun = true,
				NeedHand = true
			});
		}

		// Token: 0x0600256A RID: 9578 RVA: 0x000C4378 File Offset: 0x000C2578
		private void OnDoAfter(EntityUid uid, BotanySwabComponent component, DoAfterEvent args)
		{
			PlantHolderComponent plant;
			BotanySwabComponent swab;
			if (args.Cancelled || args.Handled || !base.TryComp<PlantHolderComponent>(args.Args.Target, ref plant) || !base.TryComp<BotanySwabComponent>(args.Args.Used, ref swab))
			{
				return;
			}
			if (swab.SeedData == null)
			{
				swab.SeedData = plant.Seed;
				this._popupSystem.PopupEntity(Loc.GetString("botany-swab-from"), args.Args.Target.Value, args.Args.User, PopupType.Small);
			}
			else
			{
				SeedData old = plant.Seed;
				if (old == null)
				{
					return;
				}
				plant.Seed = this._mutationSystem.Cross(swab.SeedData, old);
				swab.SeedData = old;
				this._popupSystem.PopupEntity(Loc.GetString("botany-swab-to"), args.Args.Target.Value, args.Args.User, PopupType.Small);
			}
			args.Handled = true;
		}

		// Token: 0x04001713 RID: 5907
		[Dependency]
		private readonly DoAfterSystem _doAfterSystem;

		// Token: 0x04001714 RID: 5908
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x04001715 RID: 5909
		[Dependency]
		private readonly MutationSystem _mutationSystem;
	}
}
