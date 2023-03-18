using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Content.Server.Administration.Logs;
using Content.Server.Body.Systems;
using Content.Server.DoAfter;
using Content.Server.Medical.Components;
using Content.Server.Stack;
using Content.Shared.Administration.Logs;
using Content.Shared.Audio;
using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Physics;
using Content.Shared.Stacks;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;

namespace Content.Server.Medical
{
	// Token: 0x020003AF RID: 943
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class HealingSystem : EntitySystem
	{
		// Token: 0x06001360 RID: 4960 RVA: 0x00063D20 File Offset: 0x00061F20
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<HealingComponent, UseInHandEvent>(new ComponentEventHandler<HealingComponent, UseInHandEvent>(this.OnHealingUse), null, null);
			base.SubscribeLocalEvent<HealingComponent, AfterInteractEvent>(new ComponentEventHandler<HealingComponent, AfterInteractEvent>(this.OnHealingAfterInteract), null, null);
			base.SubscribeLocalEvent<DamageableComponent, DoAfterEvent<HealingSystem.HealingData>>(new ComponentEventHandler<DamageableComponent, DoAfterEvent<HealingSystem.HealingData>>(this.OnDoAfter), null, null);
		}

		// Token: 0x06001361 RID: 4961 RVA: 0x00063D70 File Offset: 0x00061F70
		private void OnDoAfter(EntityUid uid, DamageableComponent component, DoAfterEvent<HealingSystem.HealingData> args)
		{
			if (args.Cancelled)
			{
				args.AdditionalData.HealingComponent.CancelToken = null;
				return;
			}
			if (args.Handled || args.Cancelled || this._mobStateSystem.IsDead(uid, null) || args.Args.Used == null)
			{
				return;
			}
			if (component.DamageContainerID != null && !component.DamageContainerID.Equals(component.DamageContainerID))
			{
				return;
			}
			if (args.AdditionalData.HealingComponent.BloodlossModifier != 0f)
			{
				this._bloodstreamSystem.TryModifyBleedAmount(uid, args.AdditionalData.HealingComponent.BloodlossModifier, null);
			}
			DamageSpecifier healed = this._damageable.TryChangeDamage(new EntityUid?(uid), args.AdditionalData.HealingComponent.Damage, true, true, null, new EntityUid?(args.Args.User));
			if (healed == null)
			{
				return;
			}
			this._stacks.Use(args.Args.Used.Value, 1, args.AdditionalData.Stack);
			if (uid != args.Args.User)
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Healed;
				LogStringHandler logStringHandler = new LogStringHandler(20, 3);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(this.EntityManager.ToPrettyString(args.Args.User), "user", "EntityManager.ToPrettyString(args.Args.User)");
				logStringHandler.AppendLiteral(" healed ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(this.EntityManager.ToPrettyString(uid), "target", "EntityManager.ToPrettyString(uid)");
				logStringHandler.AppendLiteral(" for ");
				logStringHandler.AppendFormatted<FixedPoint2>(healed.Total, "damage", "healed.Total");
				logStringHandler.AppendLiteral(" damage");
				adminLogger.Add(type, ref logStringHandler);
			}
			else
			{
				ISharedAdminLogManager adminLogger2 = this._adminLogger;
				LogType type2 = LogType.Healed;
				LogStringHandler logStringHandler = new LogStringHandler(30, 2);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(this.EntityManager.ToPrettyString(args.Args.User), "user", "EntityManager.ToPrettyString(args.Args.User)");
				logStringHandler.AppendLiteral(" healed themselves for ");
				logStringHandler.AppendFormatted<FixedPoint2>(healed.Total, "damage", "healed.Total");
				logStringHandler.AppendLiteral(" damage");
				adminLogger2.Add(type2, ref logStringHandler);
			}
			if (args.AdditionalData.HealingComponent.HealingEndSound != null)
			{
				this._audio.PlayPvs(args.AdditionalData.HealingComponent.HealingEndSound, uid, new AudioParams?(AudioHelpers.WithVariation(0.125f, this._random).WithVolume(-5f)));
			}
			args.AdditionalData.HealingComponent.CancelToken = null;
			args.Handled = true;
		}

		// Token: 0x06001362 RID: 4962 RVA: 0x00064001 File Offset: 0x00062201
		private void OnHealingUse(EntityUid uid, HealingComponent component, UseInHandEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			if (this.TryHeal(uid, args.User, args.User, component))
			{
				args.Handled = true;
			}
		}

		// Token: 0x06001363 RID: 4963 RVA: 0x0006402C File Offset: 0x0006222C
		private void OnHealingAfterInteract(EntityUid uid, HealingComponent component, AfterInteractEvent args)
		{
			if (args.Handled || !args.CanReach || args.Target == null)
			{
				return;
			}
			if (this.TryHeal(uid, args.User, args.Target.Value, component))
			{
				args.Handled = true;
			}
		}

		// Token: 0x06001364 RID: 4964 RVA: 0x00064080 File Offset: 0x00062280
		private bool TryHeal(EntityUid uid, EntityUid user, EntityUid target, HealingComponent component)
		{
			DamageableComponent targetDamage;
			if (this._mobStateSystem.IsDead(target, null) || !base.TryComp<DamageableComponent>(target, ref targetDamage) || component.CancelToken != null)
			{
				return false;
			}
			if (targetDamage.TotalDamage == 0)
			{
				return false;
			}
			if (component.DamageContainerID != null && !component.DamageContainerID.Equals(targetDamage.DamageContainerID))
			{
				return false;
			}
			if (user != target && !this._interactionSystem.InRangeUnobstructed(user, target, 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, true))
			{
				return false;
			}
			StackComponent stack;
			if (!base.TryComp<StackComponent>(uid, ref stack) || stack.Count < 1)
			{
				return false;
			}
			if (component.HealingBeginSound != null)
			{
				this._audio.PlayPvs(component.HealingBeginSound, uid, new AudioParams?(AudioHelpers.WithVariation(0.125f, this._random).WithVolume(-5f)));
			}
			bool isNotSelf = user != target;
			float delay = isNotSelf ? component.Delay : (component.Delay * this.GetScaledHealingPenalty(user, component));
			component.CancelToken = new CancellationTokenSource();
			HealingSystem.HealingData healingData = new HealingSystem.HealingData(component, stack);
			DoAfterEventArgs doAfterEventArgs2 = new DoAfterEventArgs(user, delay, component.CancelToken.Token, new EntityUid?(target), new EntityUid?(uid));
			doAfterEventArgs2.RaiseOnTarget = isNotSelf;
			doAfterEventArgs2.RaiseOnUser = !isNotSelf;
			doAfterEventArgs2.BreakOnUserMove = true;
			doAfterEventArgs2.BreakOnTargetMove = true;
			doAfterEventArgs2.BreakOnStun = true;
			doAfterEventArgs2.NeedHand = true;
			doAfterEventArgs2.PostCheck = (() => true);
			DoAfterEventArgs doAfterEventArgs = doAfterEventArgs2;
			this._doAfter.DoAfter<HealingSystem.HealingData>(doAfterEventArgs, healingData);
			return true;
		}

		// Token: 0x06001365 RID: 4965 RVA: 0x0006421C File Offset: 0x0006241C
		public float GetScaledHealingPenalty(EntityUid uid, HealingComponent component)
		{
			float output = component.Delay;
			MobThresholdsComponent mobThreshold;
			DamageableComponent damageable;
			if (!base.TryComp<MobThresholdsComponent>(uid, ref mobThreshold) || !base.TryComp<DamageableComponent>(uid, ref damageable))
			{
				return output;
			}
			FixedPoint2? amount;
			if (!this._mobThresholdSystem.TryGetThresholdForState(uid, MobState.Critical, out amount, mobThreshold))
			{
				return 1f;
			}
			return Math.Max((float)(damageable.TotalDamage / amount).Value * (component.SelfHealPenaltyMultiplier - 1f) + 1f, 1f);
		}

		// Token: 0x04000BC6 RID: 3014
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x04000BC7 RID: 3015
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x04000BC8 RID: 3016
		[Dependency]
		private readonly DamageableSystem _damageable;

		// Token: 0x04000BC9 RID: 3017
		[Dependency]
		private readonly BloodstreamSystem _bloodstreamSystem;

		// Token: 0x04000BCA RID: 3018
		[Dependency]
		private readonly DoAfterSystem _doAfter;

		// Token: 0x04000BCB RID: 3019
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000BCC RID: 3020
		[Dependency]
		private readonly StackSystem _stacks;

		// Token: 0x04000BCD RID: 3021
		[Dependency]
		private readonly SharedInteractionSystem _interactionSystem;

		// Token: 0x04000BCE RID: 3022
		[Dependency]
		private readonly MobStateSystem _mobStateSystem;

		// Token: 0x04000BCF RID: 3023
		[Dependency]
		private readonly MobThresholdSystem _mobThresholdSystem;

		// Token: 0x020009A2 RID: 2466
		[NullableContext(0)]
		private struct HealingData : IEquatable<HealingSystem.HealingData>
		{
			// Token: 0x06003302 RID: 13058 RVA: 0x00106857 File Offset: 0x00104A57
			[NullableContext(1)]
			public HealingData(HealingComponent HealingComponent, StackComponent Stack)
			{
				this.HealingComponent = HealingComponent;
				this.Stack = Stack;
			}

			// Token: 0x06003303 RID: 13059 RVA: 0x00106868 File Offset: 0x00104A68
			[CompilerGenerated]
			public override readonly string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("HealingData");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			// Token: 0x06003304 RID: 13060 RVA: 0x001068B4 File Offset: 0x00104AB4
			[CompilerGenerated]
			private readonly bool PrintMembers(StringBuilder builder)
			{
				builder.Append("HealingComponent = ");
				builder.Append(this.HealingComponent);
				builder.Append(", Stack = ");
				builder.Append(this.Stack);
				return true;
			}

			// Token: 0x06003305 RID: 13061 RVA: 0x001068E9 File Offset: 0x00104AE9
			[CompilerGenerated]
			public static bool operator !=(HealingSystem.HealingData left, HealingSystem.HealingData right)
			{
				return !(left == right);
			}

			// Token: 0x06003306 RID: 13062 RVA: 0x001068F5 File Offset: 0x00104AF5
			[CompilerGenerated]
			public static bool operator ==(HealingSystem.HealingData left, HealingSystem.HealingData right)
			{
				return left.Equals(right);
			}

			// Token: 0x06003307 RID: 13063 RVA: 0x001068FF File Offset: 0x00104AFF
			[CompilerGenerated]
			public override readonly int GetHashCode()
			{
				return EqualityComparer<HealingComponent>.Default.GetHashCode(this.HealingComponent) * -1521134295 + EqualityComparer<StackComponent>.Default.GetHashCode(this.Stack);
			}

			// Token: 0x06003308 RID: 13064 RVA: 0x00106928 File Offset: 0x00104B28
			[CompilerGenerated]
			public override readonly bool Equals(object obj)
			{
				return obj is HealingSystem.HealingData && this.Equals((HealingSystem.HealingData)obj);
			}

			// Token: 0x06003309 RID: 13065 RVA: 0x00106940 File Offset: 0x00104B40
			[CompilerGenerated]
			public readonly bool Equals(HealingSystem.HealingData other)
			{
				return EqualityComparer<HealingComponent>.Default.Equals(this.HealingComponent, other.HealingComponent) && EqualityComparer<StackComponent>.Default.Equals(this.Stack, other.Stack);
			}

			// Token: 0x0600330A RID: 13066 RVA: 0x00106972 File Offset: 0x00104B72
			[NullableContext(1)]
			[CompilerGenerated]
			public readonly void Deconstruct(out HealingComponent HealingComponent, out StackComponent Stack)
			{
				HealingComponent = this.HealingComponent;
				Stack = this.Stack;
			}

			// Token: 0x040021AA RID: 8618
			[Nullable(1)]
			public HealingComponent HealingComponent;

			// Token: 0x040021AB RID: 8619
			[Nullable(1)]
			public StackComponent Stack;
		}
	}
}
