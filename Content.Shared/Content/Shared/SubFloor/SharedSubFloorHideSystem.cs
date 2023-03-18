using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Audio;
using Content.Shared.Interaction.Events;
using Content.Shared.Maps;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

namespace Content.Shared.SubFloor
{
	// Token: 0x02000104 RID: 260
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedSubFloorHideSystem : EntitySystem
	{
		// Token: 0x060002D3 RID: 723 RVA: 0x0000CCD4 File Offset: 0x0000AED4
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<GridModifiedEvent>(new EntityEventHandler<GridModifiedEvent>(this.OnGridChanged), null, null);
			base.SubscribeLocalEvent<TileChangedEvent>(new EntityEventRefHandler<TileChangedEvent>(this.OnTileChanged), null, null);
			base.SubscribeLocalEvent<SubFloorHideComponent, ComponentStartup>(new ComponentEventHandler<SubFloorHideComponent, ComponentStartup>(this.OnSubFloorStarted), null, null);
			base.SubscribeLocalEvent<SubFloorHideComponent, ComponentShutdown>(new ComponentEventHandler<SubFloorHideComponent, ComponentShutdown>(this.OnSubFloorTerminating), null, null);
			base.SubscribeLocalEvent<SubFloorHideComponent, AnchorStateChangedEvent>(new ComponentEventRefHandler<SubFloorHideComponent, AnchorStateChangedEvent>(this.HandleAnchorChanged), null, null);
			base.SubscribeLocalEvent<SubFloorHideComponent, GettingInteractedWithAttemptEvent>(new ComponentEventHandler<SubFloorHideComponent, GettingInteractedWithAttemptEvent>(this.OnInteractionAttempt), null, null);
			base.SubscribeLocalEvent<SubFloorHideComponent, GettingAttackedAttemptEvent>(new ComponentEventRefHandler<SubFloorHideComponent, GettingAttackedAttemptEvent>(this.OnAttackAttempt), null, null);
		}

		// Token: 0x060002D4 RID: 724 RVA: 0x0000CD73 File Offset: 0x0000AF73
		private void OnAttackAttempt(EntityUid uid, SubFloorHideComponent component, ref GettingAttackedAttemptEvent args)
		{
			if (component.BlockInteractions && component.IsUnderCover)
			{
				args.Cancelled = true;
			}
		}

		// Token: 0x060002D5 RID: 725 RVA: 0x0000CD8C File Offset: 0x0000AF8C
		private void OnInteractionAttempt(EntityUid uid, SubFloorHideComponent component, GettingInteractedWithAttemptEvent args)
		{
			if (component.BlockInteractions && component.IsUnderCover)
			{
				args.Cancel();
			}
		}

		// Token: 0x060002D6 RID: 726 RVA: 0x0000CDA4 File Offset: 0x0000AFA4
		private void OnSubFloorStarted(EntityUid uid, SubFloorHideComponent component, ComponentStartup _)
		{
			this.UpdateFloorCover(uid, component, null);
			this.UpdateAppearance(uid, component, null);
			this.EntityManager.EnsureComponent<CollideOnAnchorComponent>(uid);
		}

		// Token: 0x060002D7 RID: 727 RVA: 0x0000CDC5 File Offset: 0x0000AFC5
		private void OnSubFloorTerminating(EntityUid uid, SubFloorHideComponent component, ComponentShutdown _)
		{
			if (this.EntityManager.GetComponent<MetaDataComponent>(uid).EntityLifeStage >= 4)
			{
				return;
			}
			component.IsUnderCover = false;
			this.UpdateAppearance(uid, component, null);
		}

		// Token: 0x060002D8 RID: 728 RVA: 0x0000CDEC File Offset: 0x0000AFEC
		private void HandleAnchorChanged(EntityUid uid, SubFloorHideComponent component, ref AnchorStateChangedEvent args)
		{
			if (args.Anchored)
			{
				TransformComponent xform = base.Transform(uid);
				this._trayScannerSystem.OnSubfloorAnchored(uid, component, xform);
				this.UpdateFloorCover(uid, component, xform);
				return;
			}
			if (component.IsUnderCover)
			{
				component.IsUnderCover = false;
				this.UpdateAppearance(uid, component, null);
			}
		}

		// Token: 0x060002D9 RID: 729 RVA: 0x0000CE3C File Offset: 0x0000B03C
		private void OnTileChanged(ref TileChangedEvent args)
		{
			if (args.OldTile.IsEmpty)
			{
				return;
			}
			if (args.NewTile.Tile.IsEmpty)
			{
				return;
			}
			this.UpdateTile(this.MapManager.GetGrid(args.NewTile.GridUid), args.NewTile.GridIndices);
		}

		// Token: 0x060002DA RID: 730 RVA: 0x0000CE94 File Offset: 0x0000B094
		private void OnGridChanged(GridModifiedEvent args)
		{
			foreach (ValueTuple<Vector2i, Tile> modified in args.Modified)
			{
				this.UpdateTile(args.Grid, modified.Item1);
			}
		}

		// Token: 0x060002DB RID: 731 RVA: 0x0000CEEC File Offset: 0x0000B0EC
		[NullableContext(2)]
		private void UpdateFloorCover(EntityUid uid, SubFloorHideComponent component = null, TransformComponent xform = null)
		{
			if (!base.Resolve<SubFloorHideComponent, TransformComponent>(uid, ref component, ref xform, true))
			{
				return;
			}
			MapGridComponent grid;
			if (xform.Anchored && this.MapManager.TryGetGrid(xform.GridUid, ref grid))
			{
				component.IsUnderCover = this.HasFloorCover(grid, grid.TileIndicesFor(xform.Coordinates));
			}
			else
			{
				component.IsUnderCover = false;
			}
			this.UpdateAppearance(uid, component, null);
		}

		// Token: 0x060002DC RID: 732 RVA: 0x0000CF50 File Offset: 0x0000B150
		public bool HasFloorCover(MapGridComponent grid, Vector2i position)
		{
			return !((ContentTileDefinition)this._tileDefinitionManager[(int)grid.GetTileRef(position).Tile.TypeId]).IsSubFloor;
		}

		// Token: 0x060002DD RID: 733 RVA: 0x0000CF7C File Offset: 0x0000B17C
		private void UpdateTile(MapGridComponent grid, Vector2i position)
		{
			bool covered = this.HasFloorCover(grid, position);
			foreach (EntityUid uid in grid.GetAnchoredEntities(position))
			{
				SubFloorHideComponent hideComp;
				if (base.TryComp<SubFloorHideComponent>(uid, ref hideComp) && hideComp.IsUnderCover != covered)
				{
					hideComp.IsUnderCover = covered;
					this.UpdateAppearance(uid, hideComp, null);
				}
			}
		}

		// Token: 0x060002DE RID: 734 RVA: 0x0000CFF0 File Offset: 0x0000B1F0
		public void SetEntitiesRevealed(IEnumerable<EntityUid> entities, EntityUid revealer, bool visible)
		{
			foreach (EntityUid uid in entities)
			{
				this.SetEntityRevealed(uid, revealer, visible, null);
			}
		}

		// Token: 0x060002DF RID: 735 RVA: 0x0000D03C File Offset: 0x0000B23C
		[NullableContext(2)]
		public void SetEntityRevealed(EntityUid uid, EntityUid revealer, bool visible, SubFloorHideComponent hideComp = null)
		{
			if (!base.Resolve<SubFloorHideComponent>(uid, ref hideComp, false))
			{
				return;
			}
			if (visible)
			{
				if (hideComp.RevealedBy.Add(revealer) && hideComp.RevealedBy.Count == 1)
				{
					this.UpdateAppearance(uid, hideComp, null);
				}
				return;
			}
			if (hideComp.RevealedBy.Remove(revealer) && hideComp.RevealedBy.Count == 0)
			{
				this.UpdateAppearance(uid, hideComp, null);
			}
		}

		// Token: 0x060002E0 RID: 736 RVA: 0x0000D0AC File Offset: 0x0000B2AC
		[NullableContext(2)]
		public void UpdateAppearance(EntityUid uid, SubFloorHideComponent hideComp = null, AppearanceComponent appearance = null)
		{
			if (!base.Resolve<SubFloorHideComponent>(uid, ref hideComp, false))
			{
				return;
			}
			if (hideComp.BlockAmbience && hideComp.IsUnderCover)
			{
				this._ambientSoundSystem.SetAmbience(uid, false, null);
			}
			else if (hideComp.BlockAmbience && !hideComp.IsUnderCover)
			{
				this._ambientSoundSystem.SetAmbience(uid, true, null);
			}
			if (base.Resolve<AppearanceComponent>(uid, ref appearance, false))
			{
				this.Appearance.SetData(uid, SubFloorVisuals.Covered, hideComp.IsUnderCover, appearance);
				this.Appearance.SetData(uid, SubFloorVisuals.ScannerRevealed, hideComp.RevealedBy.Count != 0, appearance);
			}
		}

		// Token: 0x04000328 RID: 808
		[Dependency]
		protected readonly IMapManager MapManager;

		// Token: 0x04000329 RID: 809
		[Dependency]
		private readonly ITileDefinitionManager _tileDefinitionManager;

		// Token: 0x0400032A RID: 810
		[Dependency]
		private readonly TrayScannerSystem _trayScannerSystem;

		// Token: 0x0400032B RID: 811
		[Dependency]
		private readonly SharedAmbientSoundSystem _ambientSoundSystem;

		// Token: 0x0400032C RID: 812
		[Dependency]
		protected readonly SharedAppearanceSystem Appearance;
	}
}
