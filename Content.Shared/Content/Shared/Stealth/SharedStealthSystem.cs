using System;
using System.Runtime.CompilerServices;
using Content.Shared.Examine;
using Content.Shared.Stealth.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Timing;

namespace Content.Shared.Stealth
{
	// Token: 0x02000151 RID: 337
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedStealthSystem : EntitySystem
	{
		// Token: 0x0600040A RID: 1034 RVA: 0x000101AC File Offset: 0x0000E3AC
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<StealthComponent, ComponentGetState>(new ComponentEventRefHandler<StealthComponent, ComponentGetState>(this.OnStealthGetState), null, null);
			base.SubscribeLocalEvent<StealthComponent, ComponentHandleState>(new ComponentEventRefHandler<StealthComponent, ComponentHandleState>(this.OnStealthHandleState), null, null);
			base.SubscribeLocalEvent<StealthOnMoveComponent, MoveEvent>(new ComponentEventRefHandler<StealthOnMoveComponent, MoveEvent>(this.OnMove), null, null);
			base.SubscribeLocalEvent<StealthOnMoveComponent, SharedStealthSystem.GetVisibilityModifiersEvent>(new ComponentEventHandler<StealthOnMoveComponent, SharedStealthSystem.GetVisibilityModifiersEvent>(this.OnGetVisibilityModifiers), null, null);
			base.SubscribeLocalEvent<StealthComponent, EntityPausedEvent>(new ComponentEventRefHandler<StealthComponent, EntityPausedEvent>(this.OnPaused), null, null);
			base.SubscribeLocalEvent<StealthComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<StealthComponent, EntityUnpausedEvent>(this.OnUnpaused), null, null);
			base.SubscribeLocalEvent<StealthComponent, ComponentInit>(new ComponentEventHandler<StealthComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<StealthComponent, ExamineAttemptEvent>(new ComponentEventHandler<StealthComponent, ExamineAttemptEvent>(this.OnExamineAttempt), null, null);
			base.SubscribeLocalEvent<StealthComponent, ExaminedEvent>(new ComponentEventHandler<StealthComponent, ExaminedEvent>(this.OnExamined), null, null);
		}

		// Token: 0x0600040B RID: 1035 RVA: 0x00010274 File Offset: 0x0000E474
		private void OnExamineAttempt(EntityUid uid, StealthComponent component, ExamineAttemptEvent args)
		{
			if (!component.Enabled || this.GetVisibility(uid, component) > component.ExamineThreshold)
			{
				return;
			}
			EntityUid source = args.Examiner;
			while (!(source == uid))
			{
				source = base.Transform(source).ParentUid;
				if (!source.IsValid())
				{
					args.Cancel();
					return;
				}
			}
		}

		// Token: 0x0600040C RID: 1036 RVA: 0x000102C7 File Offset: 0x0000E4C7
		private void OnExamined(EntityUid uid, StealthComponent component, ExaminedEvent args)
		{
			if (component.Enabled)
			{
				args.PushMarkup(Loc.GetString(component.ExaminedDesc, new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("target", uid)
				}));
			}
		}

		// Token: 0x0600040D RID: 1037 RVA: 0x000102FF File Offset: 0x0000E4FF
		[NullableContext(2)]
		public virtual void SetEnabled(EntityUid uid, bool value, StealthComponent component = null)
		{
			if (!base.Resolve<StealthComponent>(uid, ref component, false) || component.Enabled == value)
			{
				return;
			}
			component.Enabled = value;
			base.Dirty(component, null);
		}

		// Token: 0x0600040E RID: 1038 RVA: 0x00010326 File Offset: 0x0000E526
		private void OnPaused(EntityUid uid, StealthComponent component, ref EntityPausedEvent args)
		{
			component.LastVisibility = this.GetVisibility(uid, component);
			component.LastUpdated = null;
			base.Dirty(component, null);
		}

		// Token: 0x0600040F RID: 1039 RVA: 0x0001034A File Offset: 0x0000E54A
		private void OnUnpaused(EntityUid uid, StealthComponent component, ref EntityUnpausedEvent args)
		{
			component.LastUpdated = new TimeSpan?(this._timing.CurTime);
			base.Dirty(component, null);
		}

		// Token: 0x06000410 RID: 1040 RVA: 0x0001036A File Offset: 0x0000E56A
		protected virtual void OnInit(EntityUid uid, StealthComponent component, ComponentInit args)
		{
			if (component.LastUpdated != null || base.Paused(uid, null))
			{
				return;
			}
			component.LastUpdated = new TimeSpan?(this._timing.CurTime);
		}

		// Token: 0x06000411 RID: 1041 RVA: 0x0001039A File Offset: 0x0000E59A
		private void OnStealthGetState(EntityUid uid, StealthComponent component, ref ComponentGetState args)
		{
			args.State = new StealthComponentState(component.LastVisibility, component.LastUpdated, component.Enabled);
		}

		// Token: 0x06000412 RID: 1042 RVA: 0x000103BC File Offset: 0x0000E5BC
		private void OnStealthHandleState(EntityUid uid, StealthComponent component, ref ComponentHandleState args)
		{
			StealthComponentState cast = args.Current as StealthComponentState;
			if (cast == null)
			{
				return;
			}
			this.SetEnabled(uid, cast.Enabled, component);
			component.LastVisibility = cast.Visibility;
			component.LastUpdated = cast.LastUpdated;
		}

		// Token: 0x06000413 RID: 1043 RVA: 0x00010400 File Offset: 0x0000E600
		private void OnMove(EntityUid uid, StealthOnMoveComponent component, ref MoveEvent args)
		{
			if (args.FromStateHandling)
			{
				return;
			}
			if (args.NewPosition.EntityId != args.OldPosition.EntityId)
			{
				return;
			}
			float delta = component.MovementVisibilityRate * (args.NewPosition.Position - args.OldPosition.Position).Length;
			this.ModifyVisibility(uid, delta, null);
		}

		// Token: 0x06000414 RID: 1044 RVA: 0x00010468 File Offset: 0x0000E668
		private void OnGetVisibilityModifiers(EntityUid uid, StealthOnMoveComponent component, SharedStealthSystem.GetVisibilityModifiersEvent args)
		{
			float mod = args.SecondsSinceUpdate * component.PassiveVisibilityRate;
			args.FlatModifier += mod;
		}

		// Token: 0x06000415 RID: 1045 RVA: 0x00010494 File Offset: 0x0000E694
		[NullableContext(2)]
		public void ModifyVisibility(EntityUid uid, float delta, StealthComponent component = null)
		{
			if (delta == 0f || !base.Resolve<StealthComponent>(uid, ref component, true))
			{
				return;
			}
			if (component.LastUpdated != null)
			{
				component.LastVisibility = this.GetVisibility(uid, component);
				component.LastUpdated = new TimeSpan?(this._timing.CurTime);
			}
			component.LastVisibility = Math.Clamp(component.LastVisibility + delta, component.MinVisibility, component.MaxVisibility);
			base.Dirty(component, null);
		}

		// Token: 0x06000416 RID: 1046 RVA: 0x00010510 File Offset: 0x0000E710
		[NullableContext(2)]
		public void SetVisibility(EntityUid uid, float value, StealthComponent component = null)
		{
			if (!base.Resolve<StealthComponent>(uid, ref component, true))
			{
				return;
			}
			component.LastVisibility = Math.Clamp(value, component.MinVisibility, component.MaxVisibility);
			if (component.LastUpdated != null)
			{
				component.LastUpdated = new TimeSpan?(this._timing.CurTime);
			}
			base.Dirty(component, null);
		}

		// Token: 0x06000417 RID: 1047 RVA: 0x00010570 File Offset: 0x0000E770
		[NullableContext(2)]
		public float GetVisibility(EntityUid uid, StealthComponent component = null)
		{
			if (!base.Resolve<StealthComponent>(uid, ref component, true) || !component.Enabled)
			{
				return 1f;
			}
			if (component.LastUpdated == null)
			{
				return component.LastVisibility;
			}
			TimeSpan deltaTime = this._timing.CurTime - component.LastUpdated.Value;
			SharedStealthSystem.GetVisibilityModifiersEvent ev = new SharedStealthSystem.GetVisibilityModifiersEvent(uid, component, (float)deltaTime.TotalSeconds, 0f);
			base.RaiseLocalEvent<SharedStealthSystem.GetVisibilityModifiersEvent>(uid, ev, false);
			return Math.Clamp(component.LastVisibility + ev.FlatModifier, component.MinVisibility, component.MaxVisibility);
		}

		// Token: 0x040003E3 RID: 995
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x020007A2 RID: 1954
		[Nullable(0)]
		private sealed class GetVisibilityModifiersEvent : EntityEventArgs
		{
			// Token: 0x060017E4 RID: 6116 RVA: 0x0004D2AC File Offset: 0x0004B4AC
			public GetVisibilityModifiersEvent(EntityUid uid, StealthComponent stealth, float secondsSinceUpdate, float flatModifier)
			{
				this.Stealth = stealth;
				this.SecondsSinceUpdate = secondsSinceUpdate;
				this.FlatModifier = flatModifier;
			}

			// Token: 0x040017BD RID: 6077
			public readonly StealthComponent Stealth;

			// Token: 0x040017BE RID: 6078
			public readonly float SecondsSinceUpdate;

			// Token: 0x040017BF RID: 6079
			public float FlatModifier;
		}
	}
}
