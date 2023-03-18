using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Light.Components;
using Content.Shared.Audio;
using Content.Shared.Interaction;
using Content.Shared.Item;
using Content.Shared.Smoking;
using Content.Shared.Temperature;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;

namespace Content.Server.Light.EntitySystems
{
	// Token: 0x02000411 RID: 1041
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MatchstickSystem : EntitySystem
	{
		// Token: 0x06001525 RID: 5413 RVA: 0x0006EDDC File Offset: 0x0006CFDC
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<MatchstickComponent, InteractUsingEvent>(new ComponentEventHandler<MatchstickComponent, InteractUsingEvent>(this.OnInteractUsing), null, null);
			base.SubscribeLocalEvent<MatchstickComponent, IsHotEvent>(new ComponentEventHandler<MatchstickComponent, IsHotEvent>(this.OnIsHotEvent), null, null);
			base.SubscribeLocalEvent<MatchstickComponent, ComponentShutdown>(new ComponentEventHandler<MatchstickComponent, ComponentShutdown>(this.OnShutdown), null, null);
		}

		// Token: 0x06001526 RID: 5414 RVA: 0x0006EE2B File Offset: 0x0006D02B
		private void OnShutdown(EntityUid uid, MatchstickComponent component, ComponentShutdown args)
		{
			this._litMatches.Remove(component);
		}

		// Token: 0x06001527 RID: 5415 RVA: 0x0006EE3C File Offset: 0x0006D03C
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (MatchstickComponent match in this._litMatches)
			{
				if (match.CurrentState == SmokableState.Lit && !base.Paused(match.Owner, null) && !match.Deleted)
				{
					TransformComponent xform = base.Transform(match.Owner);
					EntityUid? gridUid2 = xform.GridUid;
					if (gridUid2 == null)
					{
						break;
					}
					EntityUid gridUid = gridUid2.GetValueOrDefault();
					Vector2i position = this._transformSystem.GetGridOrMapTilePosition(match.Owner, xform);
					this._atmosphereSystem.HotspotExpose(gridUid, position, 400f, 50f, true);
				}
			}
		}

		// Token: 0x06001528 RID: 5416 RVA: 0x0006EF0C File Offset: 0x0006D10C
		private void OnInteractUsing(EntityUid uid, MatchstickComponent component, InteractUsingEvent args)
		{
			if (args.Handled || component.CurrentState != SmokableState.Unlit)
			{
				return;
			}
			IsHotEvent isHotEvent = new IsHotEvent();
			base.RaiseLocalEvent<IsHotEvent>(args.Used, isHotEvent, false);
			if (!isHotEvent.IsHot)
			{
				return;
			}
			this.Ignite(uid, component, args.User);
			args.Handled = true;
		}

		// Token: 0x06001529 RID: 5417 RVA: 0x0006EF5C File Offset: 0x0006D15C
		private void OnIsHotEvent(EntityUid uid, MatchstickComponent component, IsHotEvent args)
		{
			args.IsHot = (component.CurrentState == SmokableState.Lit);
		}

		// Token: 0x0600152A RID: 5418 RVA: 0x0006EF70 File Offset: 0x0006D170
		public void Ignite(EntityUid uid, MatchstickComponent component, EntityUid user)
		{
			SoundSystem.Play(component.IgniteSound.GetSound(null, null), Filter.Pvs(component.Owner, 2f, null, null, null), component.Owner, new AudioParams?(AudioHelpers.WithVariation(0.125f).WithVolume(-0.125f)));
			this.SetState(uid, component, SmokableState.Lit);
			this._litMatches.Add(component);
			TimerExtensions.SpawnTimer(component.Owner, component.Duration * 1000, delegate()
			{
				this.SetState(uid, component, SmokableState.Burnt);
				this._litMatches.Remove(component);
			}, default(CancellationToken));
		}

		// Token: 0x0600152B RID: 5419 RVA: 0x0006F04C File Offset: 0x0006D24C
		private void SetState(EntityUid uid, MatchstickComponent component, SmokableState value)
		{
			component.CurrentState = value;
			PointLightComponent pointLightComponent;
			if (base.TryComp<PointLightComponent>(component.Owner, ref pointLightComponent))
			{
				pointLightComponent.Enabled = (component.CurrentState == SmokableState.Lit);
			}
			ItemComponent item;
			if (this.EntityManager.TryGetComponent<ItemComponent>(component.Owner, ref item))
			{
				if (component.CurrentState == SmokableState.Lit)
				{
					this._item.SetHeldPrefix(component.Owner, "lit", item);
				}
				else
				{
					this._item.SetHeldPrefix(component.Owner, "unlit", item);
				}
			}
			AppearanceComponent appearance;
			if (this.EntityManager.TryGetComponent<AppearanceComponent>(component.Owner, ref appearance))
			{
				this._appearance.SetData(uid, SmokingVisuals.Smoking, component.CurrentState, appearance);
			}
		}

		// Token: 0x04000D11 RID: 3345
		[Dependency]
		private readonly AtmosphereSystem _atmosphereSystem;

		// Token: 0x04000D12 RID: 3346
		[Dependency]
		private readonly TransformSystem _transformSystem;

		// Token: 0x04000D13 RID: 3347
		[Dependency]
		private readonly SharedItemSystem _item;

		// Token: 0x04000D14 RID: 3348
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x04000D15 RID: 3349
		private HashSet<MatchstickComponent> _litMatches = new HashSet<MatchstickComponent>();
	}
}
