using System;
using System.Runtime.CompilerServices;
using Content.Shared.Examine;
using Content.Shared.Inventory;
using Content.Shared.Movement.Systems;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Shared.Clothing
{
	// Token: 0x020005A5 RID: 1445
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ClothingSpeedModifierSystem : EntitySystem
	{
		// Token: 0x0600119A RID: 4506 RVA: 0x00039630 File Offset: 0x00037830
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ClothingSpeedModifierComponent, ComponentGetState>(new ComponentEventRefHandler<ClothingSpeedModifierComponent, ComponentGetState>(this.OnGetState), null, null);
			base.SubscribeLocalEvent<ClothingSpeedModifierComponent, ComponentHandleState>(new ComponentEventRefHandler<ClothingSpeedModifierComponent, ComponentHandleState>(this.OnHandleState), null, null);
			base.SubscribeLocalEvent<ClothingSpeedModifierComponent, InventoryRelayedEvent<RefreshMovementSpeedModifiersEvent>>(new ComponentEventHandler<ClothingSpeedModifierComponent, InventoryRelayedEvent<RefreshMovementSpeedModifiersEvent>>(this.OnRefreshMoveSpeed), null, null);
			base.SubscribeLocalEvent<ClothingSpeedModifierComponent, GetVerbsEvent<ExamineVerb>>(new ComponentEventHandler<ClothingSpeedModifierComponent, GetVerbsEvent<ExamineVerb>>(this.OnClothingVerbExamine), null, null);
		}

		// Token: 0x0600119B RID: 4507 RVA: 0x00039694 File Offset: 0x00037894
		[NullableContext(2)]
		public void SetClothingSpeedModifierEnabled(EntityUid uid, bool enabled, ClothingSpeedModifierComponent component = null)
		{
			if (!base.Resolve<ClothingSpeedModifierComponent>(uid, ref component, false))
			{
				return;
			}
			if (component.Enabled != enabled)
			{
				component.Enabled = enabled;
				base.Dirty(component, null);
				IContainer container;
				if (this._container.TryGetContainingContainer(uid, ref container, null, null))
				{
					this._movementSpeed.RefreshMovementSpeedModifiers(container.Owner, null);
				}
			}
		}

		// Token: 0x0600119C RID: 4508 RVA: 0x000396EA File Offset: 0x000378EA
		private void OnGetState(EntityUid uid, ClothingSpeedModifierComponent component, ref ComponentGetState args)
		{
			args.State = new ClothingSpeedModifierComponentState(component.WalkModifier, component.SprintModifier, component.Enabled);
		}

		// Token: 0x0600119D RID: 4509 RVA: 0x0003970C File Offset: 0x0003790C
		private void OnHandleState(EntityUid uid, ClothingSpeedModifierComponent component, ref ComponentHandleState args)
		{
			ClothingSpeedModifierComponentState state = args.Current as ClothingSpeedModifierComponentState;
			if (state == null)
			{
				return;
			}
			bool flag = component.Enabled != state.Enabled || !MathHelper.CloseTo(component.SprintModifier, state.SprintModifier, 1E-07f) || !MathHelper.CloseTo(component.WalkModifier, state.WalkModifier, 1E-07f);
			component.WalkModifier = state.WalkModifier;
			component.SprintModifier = state.SprintModifier;
			component.Enabled = state.Enabled;
			IContainer container;
			if (flag && this._container.TryGetContainingContainer(uid, ref container, null, null))
			{
				this._movementSpeed.RefreshMovementSpeedModifiers(container.Owner, null);
			}
		}

		// Token: 0x0600119E RID: 4510 RVA: 0x000397B5 File Offset: 0x000379B5
		private void OnRefreshMoveSpeed(EntityUid uid, ClothingSpeedModifierComponent component, InventoryRelayedEvent<RefreshMovementSpeedModifiersEvent> args)
		{
			if (!component.Enabled)
			{
				return;
			}
			args.Args.ModifySpeed(component.WalkModifier, component.SprintModifier);
		}

		// Token: 0x0600119F RID: 4511 RVA: 0x000397D8 File Offset: 0x000379D8
		private void OnClothingVerbExamine(EntityUid uid, ClothingSpeedModifierComponent component, GetVerbsEvent<ExamineVerb> args)
		{
			if (!args.CanInteract || !args.CanAccess)
			{
				return;
			}
			float walkModifierPercentage = MathF.Round((1f - component.WalkModifier) * 100f, 1);
			float sprintModifierPercentage = MathF.Round((1f - component.SprintModifier) * 100f, 1);
			if (walkModifierPercentage == 0f && sprintModifierPercentage == 0f)
			{
				return;
			}
			FormattedMessage msg = new FormattedMessage();
			if (walkModifierPercentage == sprintModifierPercentage)
			{
				if (walkModifierPercentage < 0f)
				{
					msg.AddMarkup(Loc.GetString("clothing-speed-increase-equal-examine", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("walkSpeed", MathF.Abs(walkModifierPercentage)),
						new ValueTuple<string, object>("runSpeed", MathF.Abs(sprintModifierPercentage))
					}));
				}
				else
				{
					msg.AddMarkup(Loc.GetString("clothing-speed-decrease-equal-examine", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("walkSpeed", walkModifierPercentage),
						new ValueTuple<string, object>("runSpeed", sprintModifierPercentage)
					}));
				}
			}
			else
			{
				if (sprintModifierPercentage < 0f)
				{
					msg.AddMarkup(Loc.GetString("clothing-speed-increase-run-examine", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("runSpeed", MathF.Abs(sprintModifierPercentage))
					}));
				}
				else if (sprintModifierPercentage > 0f)
				{
					msg.AddMarkup(Loc.GetString("clothing-speed-decrease-run-examine", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("runSpeed", sprintModifierPercentage)
					}));
				}
				if (walkModifierPercentage != 0f && sprintModifierPercentage != 0f)
				{
					msg.PushNewline();
				}
				if (walkModifierPercentage < 0f)
				{
					msg.AddMarkup(Loc.GetString("clothing-speed-increase-walk-examine", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("walkSpeed", MathF.Abs(walkModifierPercentage))
					}));
				}
				else if (walkModifierPercentage > 0f)
				{
					msg.AddMarkup(Loc.GetString("clothing-speed-decrease-walk-examine", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("walkSpeed", walkModifierPercentage)
					}));
				}
			}
			this._examine.AddDetailedExamineVerb(args, component, msg, Loc.GetString("clothing-speed-examinable-verb-text"), "/Textures/Interface/VerbIcons/outfit.svg.192dpi.png", Loc.GetString("clothing-speed-examinable-verb-message"));
		}

		// Token: 0x04001044 RID: 4164
		[Dependency]
		private readonly MovementSpeedModifierSystem _movementSpeed;

		// Token: 0x04001045 RID: 4165
		[Dependency]
		private readonly SharedContainerSystem _container;

		// Token: 0x04001046 RID: 4166
		[Dependency]
		private readonly ExamineSystemShared _examine;
	}
}
