using System;
using System.Runtime.CompilerServices;
using Content.Shared.Destructible;
using Content.Shared.Light.Component;
using Content.Shared.Throwing;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;

namespace Content.Server.Light.EntitySystems
{
	// Token: 0x0200040D RID: 1037
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class LightBulbSystem : EntitySystem
	{
		// Token: 0x0600150D RID: 5389 RVA: 0x0006E63C File Offset: 0x0006C83C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<LightBulbComponent, ComponentInit>(new ComponentEventHandler<LightBulbComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<LightBulbComponent, LandEvent>(new ComponentEventRefHandler<LightBulbComponent, LandEvent>(this.HandleLand), null, null);
			base.SubscribeLocalEvent<LightBulbComponent, BreakageEventArgs>(new ComponentEventHandler<LightBulbComponent, BreakageEventArgs>(this.OnBreak), null, null);
		}

		// Token: 0x0600150E RID: 5390 RVA: 0x0006E68B File Offset: 0x0006C88B
		private void OnInit(EntityUid uid, LightBulbComponent bulb, ComponentInit args)
		{
			this.SetColor(uid, bulb.Color, bulb);
			this.SetState(uid, bulb.State, bulb);
		}

		// Token: 0x0600150F RID: 5391 RVA: 0x0006E6A9 File Offset: 0x0006C8A9
		private void HandleLand(EntityUid uid, LightBulbComponent bulb, ref LandEvent args)
		{
			this.PlayBreakSound(uid, bulb);
			this.SetState(uid, LightBulbState.Broken, bulb);
		}

		// Token: 0x06001510 RID: 5392 RVA: 0x0006E6BC File Offset: 0x0006C8BC
		private void OnBreak(EntityUid uid, LightBulbComponent component, BreakageEventArgs args)
		{
			this.SetState(uid, LightBulbState.Broken, component);
		}

		// Token: 0x06001511 RID: 5393 RVA: 0x0006E6C7 File Offset: 0x0006C8C7
		[NullableContext(2)]
		public void SetColor(EntityUid uid, Color color, LightBulbComponent bulb = null)
		{
			if (!base.Resolve<LightBulbComponent>(uid, ref bulb, true))
			{
				return;
			}
			bulb.Color = color;
			this.UpdateAppearance(uid, bulb, null);
		}

		// Token: 0x06001512 RID: 5394 RVA: 0x0006E6E6 File Offset: 0x0006C8E6
		[NullableContext(2)]
		public void SetState(EntityUid uid, LightBulbState state, LightBulbComponent bulb = null)
		{
			if (!base.Resolve<LightBulbComponent>(uid, ref bulb, true))
			{
				return;
			}
			bulb.State = state;
			this.UpdateAppearance(uid, bulb, null);
		}

		// Token: 0x06001513 RID: 5395 RVA: 0x0006E708 File Offset: 0x0006C908
		[NullableContext(2)]
		public void PlayBreakSound(EntityUid uid, LightBulbComponent bulb = null)
		{
			if (!base.Resolve<LightBulbComponent>(uid, ref bulb, true))
			{
				return;
			}
			SoundSystem.Play(bulb.BreakSound.GetSound(null, null), Filter.Pvs(uid, 2f, null, null, null), uid, null);
		}

		// Token: 0x06001514 RID: 5396 RVA: 0x0006E750 File Offset: 0x0006C950
		[NullableContext(2)]
		private void UpdateAppearance(EntityUid uid, LightBulbComponent bulb = null, AppearanceComponent appearance = null)
		{
			if (!base.Resolve<LightBulbComponent, AppearanceComponent>(uid, ref bulb, ref appearance, false))
			{
				return;
			}
			this._appearance.SetData(uid, LightBulbVisuals.State, bulb.State, appearance);
			this._appearance.SetData(uid, LightBulbVisuals.Color, bulb.Color, appearance);
		}

		// Token: 0x04000D0D RID: 3341
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
