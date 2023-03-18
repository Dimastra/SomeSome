using System;
using System.Runtime.CompilerServices;
using Content.Shared.Hands;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Maps;
using Content.Shared.Popups;
using Content.Shared.Sound.Components;
using Content.Shared.Throwing;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Network;
using Robust.Shared.Random;

namespace Content.Shared.Sound
{
	// Token: 0x02000185 RID: 389
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedEmitSoundSystem : EntitySystem
	{
		// Token: 0x060004AF RID: 1199 RVA: 0x000122F4 File Offset: 0x000104F4
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<EmitSoundOnSpawnComponent, ComponentInit>(new ComponentEventHandler<EmitSoundOnSpawnComponent, ComponentInit>(this.HandleEmitSpawnOnInit), null, null);
			base.SubscribeLocalEvent<EmitSoundOnLandComponent, LandEvent>(new ComponentEventRefHandler<EmitSoundOnLandComponent, LandEvent>(this.OnEmitSoundOnLand), null, null);
			base.SubscribeLocalEvent<EmitSoundOnUseComponent, UseInHandEvent>(new ComponentEventHandler<EmitSoundOnUseComponent, UseInHandEvent>(this.HandleEmitSoundOnUseInHand), null, null);
			base.SubscribeLocalEvent<EmitSoundOnThrowComponent, ThrownEvent>(new ComponentEventHandler<EmitSoundOnThrowComponent, ThrownEvent>(this.HandleEmitSoundOnThrown), null, null);
			base.SubscribeLocalEvent<EmitSoundOnActivateComponent, ActivateInWorldEvent>(new ComponentEventHandler<EmitSoundOnActivateComponent, ActivateInWorldEvent>(this.HandleEmitSoundOnActivateInWorld), null, null);
			base.SubscribeLocalEvent<EmitSoundOnPickupComponent, GotEquippedHandEvent>(new ComponentEventHandler<EmitSoundOnPickupComponent, GotEquippedHandEvent>(this.HandleEmitSoundOnPickup), null, null);
			base.SubscribeLocalEvent<EmitSoundOnDropComponent, DroppedEvent>(new ComponentEventHandler<EmitSoundOnDropComponent, DroppedEvent>(this.HandleEmitSoundOnDrop), null, null);
		}

		// Token: 0x060004B0 RID: 1200 RVA: 0x00012394 File Offset: 0x00010594
		private void HandleEmitSpawnOnInit(EntityUid uid, EmitSoundOnSpawnComponent component, ComponentInit args)
		{
			this.TryEmitSound(component, null, false);
		}

		// Token: 0x060004B1 RID: 1201 RVA: 0x000123B4 File Offset: 0x000105B4
		private void OnEmitSoundOnLand(EntityUid uid, BaseEmitSoundComponent component, ref LandEvent args)
		{
			TransformComponent xform;
			MapGridComponent grid;
			if (!base.TryComp<TransformComponent>(uid, ref xform) || !this._mapManager.TryGetGrid(xform.GridUid, ref grid))
			{
				return;
			}
			TileRef tile = grid.GetTileRef(xform.Coordinates);
			if (xform.GridUid != xform.MapUid && tile.IsSpace(this._tileDefMan))
			{
				return;
			}
			this.TryEmitSound(component, args.User, false);
		}

		// Token: 0x060004B2 RID: 1202 RVA: 0x0001244F File Offset: 0x0001064F
		private void HandleEmitSoundOnUseInHand(EntityUid eUI, EmitSoundOnUseComponent component, UseInHandEvent args)
		{
			this.TryEmitSound(component, new EntityUid?(args.User), true);
			if (component.Handle)
			{
				args.Handled = true;
			}
		}

		// Token: 0x060004B3 RID: 1203 RVA: 0x00012473 File Offset: 0x00010673
		private void HandleEmitSoundOnThrown(EntityUid eUI, BaseEmitSoundComponent component, ThrownEvent args)
		{
			this.TryEmitSound(component, new EntityUid?(args.User), false);
		}

		// Token: 0x060004B4 RID: 1204 RVA: 0x00012488 File Offset: 0x00010688
		private void HandleEmitSoundOnActivateInWorld(EntityUid eUI, EmitSoundOnActivateComponent component, ActivateInWorldEvent args)
		{
			this.TryEmitSound(component, new EntityUid?(args.User), true);
			if (component.Handle)
			{
				args.Handled = true;
			}
		}

		// Token: 0x060004B5 RID: 1205 RVA: 0x000124AC File Offset: 0x000106AC
		private void HandleEmitSoundOnPickup(EntityUid uid, EmitSoundOnPickupComponent component, GotEquippedHandEvent args)
		{
			this.TryEmitSound(component, new EntityUid?(args.User), true);
		}

		// Token: 0x060004B6 RID: 1206 RVA: 0x000124C1 File Offset: 0x000106C1
		private void HandleEmitSoundOnDrop(EntityUid uid, EmitSoundOnDropComponent component, DroppedEvent args)
		{
			this.TryEmitSound(component, new EntityUid?(args.User), true);
		}

		// Token: 0x060004B7 RID: 1207 RVA: 0x000124D8 File Offset: 0x000106D8
		protected void TryEmitSound(BaseEmitSoundComponent component, EntityUid? user = null, bool predict = true)
		{
			if (component.Sound == null)
			{
				return;
			}
			if (predict)
			{
				this._audioSystem.PlayPredicted(component.Sound, component.Owner, user, new AudioParams?(component.Sound.Params.AddVolume(-2f)));
				return;
			}
			if (this._netMan.IsServer)
			{
				this._audioSystem.PlayPvs(component.Sound, component.Owner, new AudioParams?(component.Sound.Params.AddVolume(-2f)));
			}
		}

		// Token: 0x04000452 RID: 1106
		[Dependency]
		private readonly INetManager _netMan;

		// Token: 0x04000453 RID: 1107
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x04000454 RID: 1108
		[Dependency]
		private readonly ITileDefinitionManager _tileDefMan;

		// Token: 0x04000455 RID: 1109
		[Dependency]
		protected readonly IRobustRandom Random;

		// Token: 0x04000456 RID: 1110
		[Dependency]
		private readonly SharedAudioSystem _audioSystem;

		// Token: 0x04000457 RID: 1111
		[Dependency]
		protected readonly SharedPopupSystem Popup;
	}
}
