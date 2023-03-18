using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;

namespace Content.Shared.Audio
{
	// Token: 0x02000682 RID: 1666
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedAmbientSoundSystem : EntitySystem
	{
		// Token: 0x06001473 RID: 5235 RVA: 0x00044242 File Offset: 0x00042442
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<AmbientSoundComponent, ComponentGetState>(new ComponentEventRefHandler<AmbientSoundComponent, ComponentGetState>(this.GetCompState), null, null);
			base.SubscribeLocalEvent<AmbientSoundComponent, ComponentHandleState>(new ComponentEventRefHandler<AmbientSoundComponent, ComponentHandleState>(this.HandleCompState), null, null);
		}

		// Token: 0x06001474 RID: 5236 RVA: 0x00044272 File Offset: 0x00042472
		[NullableContext(2)]
		public virtual void SetAmbience(EntityUid uid, bool value, AmbientSoundComponent ambience = null)
		{
			if (!base.Resolve<AmbientSoundComponent>(uid, ref ambience, false) || ambience.Enabled == value)
			{
				return;
			}
			ambience.Enabled = value;
			this.QueueUpdate(uid, ambience);
			base.Dirty(ambience, null);
		}

		// Token: 0x06001475 RID: 5237 RVA: 0x000442A1 File Offset: 0x000424A1
		[NullableContext(2)]
		public virtual void SetRange(EntityUid uid, float value, AmbientSoundComponent ambience = null)
		{
			if (!base.Resolve<AmbientSoundComponent>(uid, ref ambience, false) || MathHelper.CloseToPercent(ambience.Range, value, 1E-05))
			{
				return;
			}
			ambience.Range = value;
			this.QueueUpdate(uid, ambience);
			base.Dirty(ambience, null);
		}

		// Token: 0x06001476 RID: 5238 RVA: 0x000442DE File Offset: 0x000424DE
		protected virtual void QueueUpdate(EntityUid uid, AmbientSoundComponent ambience)
		{
		}

		// Token: 0x06001477 RID: 5239 RVA: 0x000442E0 File Offset: 0x000424E0
		[NullableContext(2)]
		public virtual void SetVolume(EntityUid uid, float value, AmbientSoundComponent ambience = null)
		{
			if (!base.Resolve<AmbientSoundComponent>(uid, ref ambience, false) || MathHelper.CloseToPercent(ambience.Volume, value, 1E-05))
			{
				return;
			}
			ambience.Volume = value;
			base.Dirty(ambience, null);
		}

		// Token: 0x06001478 RID: 5240 RVA: 0x00044318 File Offset: 0x00042518
		private void HandleCompState(EntityUid uid, AmbientSoundComponent component, ref ComponentHandleState args)
		{
			AmbientSoundComponentState state = args.Current as AmbientSoundComponentState;
			if (state == null)
			{
				return;
			}
			this.SetAmbience(uid, state.Enabled, component);
			this.SetRange(uid, state.Range, component);
			this.SetVolume(uid, state.Volume, component);
		}

		// Token: 0x06001479 RID: 5241 RVA: 0x0004435F File Offset: 0x0004255F
		private void GetCompState(EntityUid uid, AmbientSoundComponent component, ref ComponentGetState args)
		{
			args.State = new AmbientSoundComponentState
			{
				Enabled = component.Enabled,
				Range = component.Range,
				Volume = component.Volume
			};
		}
	}
}
