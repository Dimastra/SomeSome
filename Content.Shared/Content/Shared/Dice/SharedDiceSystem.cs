using System;
using System.Runtime.CompilerServices;
using Content.Shared.Examine;
using Content.Shared.Interaction.Events;
using Content.Shared.Throwing;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Log;

namespace Content.Shared.Dice
{
	// Token: 0x0200050C RID: 1292
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedDiceSystem : EntitySystem
	{
		// Token: 0x06000FB7 RID: 4023 RVA: 0x00032BB0 File Offset: 0x00030DB0
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<DiceComponent, UseInHandEvent>(new ComponentEventHandler<DiceComponent, UseInHandEvent>(this.OnUseInHand), null, null);
			base.SubscribeLocalEvent<DiceComponent, LandEvent>(new ComponentEventRefHandler<DiceComponent, LandEvent>(this.OnLand), null, null);
			base.SubscribeLocalEvent<DiceComponent, ExaminedEvent>(new ComponentEventHandler<DiceComponent, ExaminedEvent>(this.OnExamined), null, null);
			base.SubscribeLocalEvent<DiceComponent, ComponentGetState>(new ComponentEventRefHandler<DiceComponent, ComponentGetState>(this.OnGetState), null, null);
			base.SubscribeLocalEvent<DiceComponent, ComponentHandleState>(new ComponentEventRefHandler<DiceComponent, ComponentHandleState>(this.OnHandleState), null, null);
		}

		// Token: 0x06000FB8 RID: 4024 RVA: 0x00032C28 File Offset: 0x00030E28
		private void OnHandleState(EntityUid uid, DiceComponent component, ref ComponentHandleState args)
		{
			DiceComponent.DiceState state = args.Current as DiceComponent.DiceState;
			if (state != null)
			{
				component.CurrentValue = state.CurrentValue;
			}
			this.UpdateVisuals(uid, component);
		}

		// Token: 0x06000FB9 RID: 4025 RVA: 0x00032C58 File Offset: 0x00030E58
		private void OnGetState(EntityUid uid, DiceComponent component, ref ComponentGetState args)
		{
			args.State = new DiceComponent.DiceState(component.CurrentValue);
		}

		// Token: 0x06000FBA RID: 4026 RVA: 0x00032C6B File Offset: 0x00030E6B
		private void OnUseInHand(EntityUid uid, DiceComponent component, UseInHandEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			args.Handled = true;
			this.Roll(uid, component);
		}

		// Token: 0x06000FBB RID: 4027 RVA: 0x00032C85 File Offset: 0x00030E85
		private void OnLand(EntityUid uid, DiceComponent component, ref LandEvent args)
		{
			this.Roll(uid, component);
		}

		// Token: 0x06000FBC RID: 4028 RVA: 0x00032C90 File Offset: 0x00030E90
		private void OnExamined(EntityUid uid, DiceComponent dice, ExaminedEvent args)
		{
			args.PushMarkup(Loc.GetString("dice-component-on-examine-message-part-1", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("sidesAmount", dice.Sides)
			}));
			args.PushMarkup(Loc.GetString("dice-component-on-examine-message-part-2", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("currentSide", dice.CurrentValue)
			}));
		}

		// Token: 0x06000FBD RID: 4029 RVA: 0x00032D04 File Offset: 0x00030F04
		[NullableContext(2)]
		public void SetCurrentSide(EntityUid uid, int side, DiceComponent die = null)
		{
			if (!base.Resolve<DiceComponent>(uid, ref die, true))
			{
				return;
			}
			if (side < 1 || side > die.Sides)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(44, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Attempted to set die ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid));
				defaultInterpolatedStringHandler.AppendLiteral(" to an invalid side (");
				defaultInterpolatedStringHandler.AppendFormatted<int>(side);
				defaultInterpolatedStringHandler.AppendLiteral(").");
				Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			die.CurrentValue = (side - die.Offset) * die.Multiplier;
			base.Dirty(die, null);
			this.UpdateVisuals(uid, die);
		}

		// Token: 0x06000FBE RID: 4030 RVA: 0x00032DA4 File Offset: 0x00030FA4
		[NullableContext(2)]
		public void SetCurrentValue(EntityUid uid, int value, DiceComponent die = null)
		{
			if (!base.Resolve<DiceComponent>(uid, ref die, true))
			{
				return;
			}
			if (value % die.Multiplier != 0 || value / die.Multiplier + die.Offset < 1)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(45, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Attempted to set die ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid));
				defaultInterpolatedStringHandler.AppendLiteral(" to an invalid value (");
				defaultInterpolatedStringHandler.AppendFormatted<int>(value);
				defaultInterpolatedStringHandler.AppendLiteral(").");
				Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			this.SetCurrentSide(uid, value / die.Multiplier + die.Offset, die);
		}

		// Token: 0x06000FBF RID: 4031 RVA: 0x00032E42 File Offset: 0x00031042
		[NullableContext(2)]
		protected virtual void UpdateVisuals(EntityUid uid, DiceComponent die = null)
		{
		}

		// Token: 0x06000FC0 RID: 4032 RVA: 0x00032E44 File Offset: 0x00031044
		[NullableContext(2)]
		public virtual void Roll(EntityUid uid, DiceComponent die = null)
		{
		}
	}
}
