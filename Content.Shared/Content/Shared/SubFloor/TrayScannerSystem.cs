using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Interaction;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared.SubFloor
{
	// Token: 0x0200010A RID: 266
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TrayScannerSystem : EntitySystem
	{
		// Token: 0x060002F3 RID: 755 RVA: 0x0000D250 File Offset: 0x0000B450
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<TrayScannerComponent, ComponentShutdown>(new ComponentEventHandler<TrayScannerComponent, ComponentShutdown>(this.OnComponentShutdown), null, null);
			base.SubscribeLocalEvent<TrayScannerComponent, ComponentGetState>(new ComponentEventRefHandler<TrayScannerComponent, ComponentGetState>(this.OnTrayScannerGetState), null, null);
			base.SubscribeLocalEvent<TrayScannerComponent, ComponentHandleState>(new ComponentEventRefHandler<TrayScannerComponent, ComponentHandleState>(this.OnTrayScannerHandleState), null, null);
			base.SubscribeLocalEvent<TrayScannerComponent, ActivateInWorldEvent>(new ComponentEventHandler<TrayScannerComponent, ActivateInWorldEvent>(this.OnTrayScannerActivate), null, null);
		}

		// Token: 0x060002F4 RID: 756 RVA: 0x0000D2B3 File Offset: 0x0000B4B3
		private void OnTrayScannerActivate(EntityUid uid, TrayScannerComponent scanner, ActivateInWorldEvent args)
		{
			this.SetScannerEnabled(uid, !scanner.Enabled, scanner);
		}

		// Token: 0x060002F5 RID: 757 RVA: 0x0000D2C8 File Offset: 0x0000B4C8
		[NullableContext(2)]
		private void SetScannerEnabled(EntityUid uid, bool enabled, TrayScannerComponent scanner = null)
		{
			if (!base.Resolve<TrayScannerComponent>(uid, ref scanner, true))
			{
				return;
			}
			scanner.Enabled = enabled;
			base.Dirty(scanner, null);
			if (scanner.Enabled)
			{
				this._activeScanners.Add(uid);
			}
			AppearanceComponent appearance;
			if (this.EntityManager.TryGetComponent<AppearanceComponent>(uid, ref appearance))
			{
				this._appearance.SetData(uid, TrayScannerVisual.Visual, scanner.Enabled ? TrayScannerVisual.On : TrayScannerVisual.Off, appearance);
			}
		}

		// Token: 0x060002F6 RID: 758 RVA: 0x0000D33A File Offset: 0x0000B53A
		private void OnTrayScannerGetState(EntityUid uid, TrayScannerComponent scanner, ref ComponentGetState args)
		{
			args.State = new TrayScannerState(scanner.Enabled);
		}

		// Token: 0x060002F7 RID: 759 RVA: 0x0000D350 File Offset: 0x0000B550
		private void OnTrayScannerHandleState(EntityUid uid, TrayScannerComponent scanner, ref ComponentHandleState args)
		{
			TrayScannerState state = args.Current as TrayScannerState;
			if (state == null)
			{
				return;
			}
			this.SetScannerEnabled(uid, state.Enabled, scanner);
			scanner.LastLocation = null;
		}

		// Token: 0x060002F8 RID: 760 RVA: 0x0000D38A File Offset: 0x0000B58A
		public void OnComponentShutdown(EntityUid uid, TrayScannerComponent scanner, ComponentShutdown args)
		{
			this._subfloorSystem.SetEntitiesRevealed(scanner.RevealedSubfloors, uid, false);
			this._activeScanners.Remove(uid);
		}

		// Token: 0x060002F9 RID: 761 RVA: 0x0000D3AC File Offset: 0x0000B5AC
		public override void Update(float frameTime)
		{
			if (!this._gameTiming.IsFirstTimePredicted)
			{
				return;
			}
			if (!this._activeScanners.Any<EntityUid>())
			{
				return;
			}
			foreach (EntityUid scanner in this._activeScanners)
			{
				if ((this._invalidScanners.List == null || !this._invalidScanners.List.Contains(scanner)) && !this.UpdateTrayScanner(scanner, null, null))
				{
					this._invalidScanners.Add(scanner);
				}
			}
			foreach (EntityUid invalidScanner in this._invalidScanners)
			{
				this._activeScanners.Remove(invalidScanner);
			}
			List<EntityUid> list = this._invalidScanners.List;
			if (list == null)
			{
				return;
			}
			list.Clear();
		}

		// Token: 0x060002FA RID: 762 RVA: 0x0000D4A8 File Offset: 0x0000B6A8
		[NullableContext(2)]
		public void OnSubfloorAnchored(EntityUid uid, SubFloorHideComponent hideComp = null, TransformComponent xform = null)
		{
			if (!base.Resolve<SubFloorHideComponent, TransformComponent>(uid, ref hideComp, ref xform, true))
			{
				return;
			}
			MapCoordinates pos = xform.MapPosition;
			foreach (EntityUid entity in this._activeScanners)
			{
				TrayScannerComponent scanner;
				if (base.TryComp<TrayScannerComponent>(entity, ref scanner) && base.Transform(entity).MapPosition.InRange(pos, scanner.Range))
				{
					hideComp.RevealedBy.Add(entity);
					scanner.RevealedSubfloors.Add(uid);
				}
			}
		}

		// Token: 0x060002FB RID: 763 RVA: 0x0000D550 File Offset: 0x0000B750
		[NullableContext(2)]
		private bool UpdateTrayScanner(EntityUid uid, TrayScannerComponent scanner = null, TransformComponent transform = null)
		{
			if (!base.Resolve<TrayScannerComponent, TransformComponent>(uid, ref scanner, ref transform, true))
			{
				return false;
			}
			if (!scanner.Enabled || transform.MapID == MapId.Nullspace)
			{
				this._subfloorSystem.SetEntitiesRevealed(scanner.RevealedSubfloors, uid, false);
				scanner.LastLocation = null;
				scanner.RevealedSubfloors.Clear();
				return false;
			}
			Vector2 pos = transform.LocalPosition;
			TransformComponent parent = this._transform.GetParent(transform);
			if (pos == Vector2.Zero && parent != null && this._containerSystem.ContainsEntity(transform.ParentUid, uid, null))
			{
				pos = parent.LocalPosition;
				if (pos == Vector2.Zero)
				{
					TransformComponent gpTransform = this._transform.GetParent(parent);
					if (gpTransform != null && this._containerSystem.ContainsEntity(gpTransform.Owner, transform.ParentUid, null))
					{
						pos = gpTransform.LocalPosition;
					}
				}
			}
			if (pos == Vector2.Zero)
			{
				this._subfloorSystem.SetEntitiesRevealed(scanner.RevealedSubfloors, uid, false);
				scanner.RevealedSubfloors.Clear();
				return true;
			}
			Vector2i flooredPos = (Vector2i)pos;
			if (flooredPos == scanner.LastLocation || (float.IsNaN((float)flooredPos.X) && float.IsNaN((float)flooredPos.Y)))
			{
				return true;
			}
			scanner.LastLocation = new Vector2i?(flooredPos);
			HashSet<EntityUid> nearby = new HashSet<EntityUid>();
			MapCoordinates coords = transform.MapPosition;
			Box2 worldBox = Box2.CenteredAround(coords.Position, new ValueTuple<float, float>(scanner.Range * 2f, scanner.Range * 2f));
			MapGridComponent grid;
			if (this._mapManager.TryGetGrid(transform.GridUid, ref grid))
			{
				foreach (EntityUid entity in grid.GetAnchoredEntities(worldBox))
				{
					SubFloorHideComponent hideComp;
					if (base.Transform(entity).MapPosition.InRange(coords, scanner.Range) && base.TryComp<SubFloorHideComponent>(entity, ref hideComp))
					{
						nearby.Add(entity);
						if (scanner.RevealedSubfloors.Add(entity))
						{
							this._subfloorSystem.SetEntityRevealed(entity, uid, true, hideComp);
						}
					}
				}
			}
			HashSet<EntityUid> missing = new HashSet<EntityUid>(scanner.RevealedSubfloors.Except(nearby));
			scanner.RevealedSubfloors.ExceptWith(missing);
			this._subfloorSystem.SetEntitiesRevealed(missing, uid, false);
			return true;
		}

		// Token: 0x0400033D RID: 829
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x0400033E RID: 830
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x0400033F RID: 831
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x04000340 RID: 832
		[Dependency]
		private readonly SharedSubFloorHideSystem _subfloorSystem;

		// Token: 0x04000341 RID: 833
		[Dependency]
		private readonly SharedContainerSystem _containerSystem;

		// Token: 0x04000342 RID: 834
		[Dependency]
		private readonly SharedTransformSystem _transform;

		// Token: 0x04000343 RID: 835
		private HashSet<EntityUid> _activeScanners = new HashSet<EntityUid>();

		// Token: 0x04000344 RID: 836
		[Nullable(0)]
		private RemQueue<EntityUid> _invalidScanners;
	}
}
