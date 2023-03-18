using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Client.Atmos.Overlays;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.EntitySystems;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client.Atmos.EntitySystems
{
	// Token: 0x0200045D RID: 1117
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GasTileOverlaySystem : SharedGasTileOverlaySystem
	{
		// Token: 0x06001BCA RID: 7114 RVA: 0x000A0B6C File Offset: 0x0009ED6C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeNetworkEvent<SharedGasTileOverlaySystem.GasOverlayUpdateEvent>(new EntityEventHandler<SharedGasTileOverlaySystem.GasOverlayUpdateEvent>(this.HandleGasOverlayUpdate), null, null);
			base.SubscribeLocalEvent<GasTileOverlayComponent, ComponentHandleState>(new ComponentEventRefHandler<GasTileOverlayComponent, ComponentHandleState>(this.OnHandleState), null, null);
			this._overlay = new GasTileOverlay(this, this.EntityManager, this._resourceCache, this.ProtoMan, this._spriteSys);
			this._overlayMan.AddOverlay(this._overlay);
		}

		// Token: 0x06001BCB RID: 7115 RVA: 0x000A0BDD File Offset: 0x0009EDDD
		public override void Shutdown()
		{
			base.Shutdown();
			this._overlayMan.RemoveOverlay(this._overlay);
		}

		// Token: 0x06001BCC RID: 7116 RVA: 0x000A0BF8 File Offset: 0x0009EDF8
		private void OnHandleState(EntityUid gridUid, GasTileOverlayComponent comp, ref ComponentHandleState args)
		{
			GasTileOverlayState gasTileOverlayState = args.Current as GasTileOverlayState;
			if (gasTileOverlayState == null)
			{
				return;
			}
			if (!gasTileOverlayState.FullState)
			{
				using (Dictionary<Vector2i, GasOverlayChunk>.KeyCollection.Enumerator enumerator = comp.Chunks.Keys.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Vector2i vector2i = enumerator.Current;
						if (!gasTileOverlayState.AllChunks.Contains(vector2i))
						{
							comp.Chunks.Remove(vector2i);
						}
					}
					goto IL_B6;
				}
			}
			foreach (Vector2i key in comp.Chunks.Keys)
			{
				if (!gasTileOverlayState.Chunks.ContainsKey(key))
				{
					comp.Chunks.Remove(key);
				}
			}
			IL_B6:
			foreach (KeyValuePair<Vector2i, GasOverlayChunk> keyValuePair in gasTileOverlayState.Chunks)
			{
				Vector2i vector2i2;
				GasOverlayChunk gasOverlayChunk;
				keyValuePair.Deconstruct(out vector2i2, out gasOverlayChunk);
				Vector2i key2 = vector2i2;
				GasOverlayChunk value = gasOverlayChunk;
				comp.Chunks[key2] = value;
			}
		}

		// Token: 0x06001BCD RID: 7117 RVA: 0x000A0D38 File Offset: 0x0009EF38
		private void HandleGasOverlayUpdate(SharedGasTileOverlaySystem.GasOverlayUpdateEvent ev)
		{
			foreach (KeyValuePair<EntityUid, HashSet<Vector2i>> keyValuePair in ev.RemovedChunks)
			{
				EntityUid entityUid;
				HashSet<Vector2i> hashSet;
				keyValuePair.Deconstruct(out entityUid, out hashSet);
				EntityUid entityUid2 = entityUid;
				HashSet<Vector2i> hashSet2 = hashSet;
				GasTileOverlayComponent gasTileOverlayComponent;
				if (base.TryComp<GasTileOverlayComponent>(entityUid2, ref gasTileOverlayComponent))
				{
					foreach (Vector2i key in hashSet2)
					{
						gasTileOverlayComponent.Chunks.Remove(key);
					}
				}
			}
			foreach (KeyValuePair<EntityUid, List<GasOverlayChunk>> keyValuePair2 in ev.UpdatedChunks)
			{
				EntityUid entityUid;
				List<GasOverlayChunk> list;
				keyValuePair2.Deconstruct(out entityUid, out list);
				EntityUid entityUid3 = entityUid;
				List<GasOverlayChunk> list2 = list;
				GasTileOverlayComponent gasTileOverlayComponent2;
				if (base.TryComp<GasTileOverlayComponent>(entityUid3, ref gasTileOverlayComponent2))
				{
					foreach (GasOverlayChunk gasOverlayChunk in list2)
					{
						gasTileOverlayComponent2.Chunks[gasOverlayChunk.Index] = gasOverlayChunk;
					}
				}
			}
		}

		// Token: 0x04000DDE RID: 3550
		[Dependency]
		private readonly IResourceCache _resourceCache;

		// Token: 0x04000DDF RID: 3551
		[Dependency]
		private readonly IOverlayManager _overlayMan;

		// Token: 0x04000DE0 RID: 3552
		[Dependency]
		private readonly SpriteSystem _spriteSys;

		// Token: 0x04000DE1 RID: 3553
		private GasTileOverlay _overlay;
	}
}
