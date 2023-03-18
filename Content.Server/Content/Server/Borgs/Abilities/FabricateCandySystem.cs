using System;
using System.Runtime.CompilerServices;
using Content.Shared.Actions;
using Content.Shared.Actions.ActionTypes;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Server.Borgs.Abilities
{
	// Token: 0x020000A8 RID: 168
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class FabricateCandySystem : EntitySystem
	{
		// Token: 0x060002A3 RID: 675 RVA: 0x0000DD30 File Offset: 0x0000BF30
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<FabricateCandyComponent, ComponentInit>(new ComponentEventHandler<FabricateCandyComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<FabricateLollipopActionEvent>(new EntityEventHandler<FabricateLollipopActionEvent>(this.OnLollipop), null, null);
			base.SubscribeLocalEvent<FabricateGumballActionEvent>(new EntityEventHandler<FabricateGumballActionEvent>(this.OnGumball), null, null);
		}

		// Token: 0x060002A4 RID: 676 RVA: 0x0000DD80 File Offset: 0x0000BF80
		private void OnInit(EntityUid uid, FabricateCandyComponent component, ComponentInit args)
		{
			InstantActionPrototype lollipop;
			if (this._prototypeManager.TryIndex<InstantActionPrototype>("FabricateLollipop", ref lollipop))
			{
				this._actions.AddAction(uid, new InstantAction(lollipop), null, null, true);
			}
			InstantActionPrototype gumball;
			if (this._prototypeManager.TryIndex<InstantActionPrototype>("FabricateGumball", ref gumball))
			{
				this._actions.AddAction(uid, new InstantAction(gumball), null, null, true);
			}
		}

		// Token: 0x060002A5 RID: 677 RVA: 0x0000DDEF File Offset: 0x0000BFEF
		private void OnLollipop(FabricateLollipopActionEvent args)
		{
			base.Spawn("FoodLollipop", base.Transform(args.Performer).Coordinates);
			args.Handled = true;
		}

		// Token: 0x060002A6 RID: 678 RVA: 0x0000DE15 File Offset: 0x0000C015
		private void OnGumball(FabricateGumballActionEvent args)
		{
			base.Spawn("FoodGumball", base.Transform(args.Performer).Coordinates);
			args.Handled = true;
		}

		// Token: 0x040001D9 RID: 473
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x040001DA RID: 474
		[Dependency]
		private readonly SharedActionsSystem _actions;
	}
}
