using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Client.Clickable;
using Content.Client.ContextMenu.UI;
using Robust.Client.ComponentTrees;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Players;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;

namespace Content.Client.Gameplay
{
	// Token: 0x02000309 RID: 777
	[NullableContext(1)]
	[Nullable(0)]
	[Virtual]
	public class GameplayStateBase : State, IEntityEventSubscriber
	{
		// Token: 0x060013A4 RID: 5028 RVA: 0x00073B6C File Offset: 0x00071D6C
		[return: TupleElementNames(new string[]
		{
			"path",
			"segments"
		})]
		[return: Nullable(new byte[]
		{
			0,
			2,
			1,
			1
		})]
		private ValueTuple<ViewVariablesPath, string[]> ResolveVVHoverObject(string path)
		{
			string[] item = path.Split('/', StringSplitOptions.None);
			EntityUid? entityUid = null;
			IViewportControl viewportControl = this.UserInterfaceManager.CurrentlyHovered as IViewportControl;
			if (viewportControl != null && this._inputManager.MouseScreenPosition.IsValid)
			{
				entityUid = this.GetClickedEntity(viewportControl.ScreenToMap(this._inputManager.MouseScreenPosition.Position));
			}
			else
			{
				EntityMenuElement entityMenuElement = this.UserInterfaceManager.CurrentlyHovered as EntityMenuElement;
				if (entityMenuElement != null)
				{
					entityUid = entityMenuElement.Entity;
				}
			}
			return new ValueTuple<ViewVariablesPath, string[]>((entityUid != null) ? new ViewVariablesInstancePath(entityUid) : null, item);
		}

		// Token: 0x060013A5 RID: 5029 RVA: 0x00073C0E File Offset: 0x00071E0E
		[return: Nullable(new byte[]
		{
			2,
			1
		})]
		private IEnumerable<string> ListVVHoverPaths(string[] segments)
		{
			return null;
		}

		// Token: 0x060013A6 RID: 5030 RVA: 0x00073C14 File Offset: 0x00071E14
		protected override void Startup()
		{
			this._vvm.RegisterDomain("enthover", new DomainResolveObject(this.ResolveVVHoverObject), new DomainListPaths(this.ListVVHoverPaths));
			this._inputManager.KeyBindStateChanged += this.OnKeyBindStateChanged;
			this._comparer = new GameplayStateBase.ClickableEntityComparer();
		}

		// Token: 0x060013A7 RID: 5031 RVA: 0x00073C6C File Offset: 0x00071E6C
		protected override void Shutdown()
		{
			this._vvm.UnregisterDomain("enthover");
			this._inputManager.KeyBindStateChanged -= this.OnKeyBindStateChanged;
		}

		// Token: 0x060013A8 RID: 5032 RVA: 0x00073C98 File Offset: 0x00071E98
		public EntityUid? GetClickedEntity(MapCoordinates coordinates)
		{
			EntityUid value = this.GetClickableEntities(coordinates).FirstOrDefault<EntityUid>();
			if (!value.IsValid())
			{
				return null;
			}
			return new EntityUid?(value);
		}

		// Token: 0x060013A9 RID: 5033 RVA: 0x00073CCB File Offset: 0x00071ECB
		public IEnumerable<EntityUid> GetClickableEntities(EntityCoordinates coordinates)
		{
			return this.GetClickableEntities(coordinates.ToMap(this._entityManager));
		}

		// Token: 0x060013AA RID: 5034 RVA: 0x00073CE0 File Offset: 0x00071EE0
		public IEnumerable<EntityUid> GetClickableEntities(MapCoordinates coordinates)
		{
			HashSet<ComponentTreeEntry<SpriteComponent>> hashSet = this._entityManager.EntitySysManager.GetEntitySystem<SpriteTreeSystem>().QueryAabb(coordinates.MapId, Box2.CenteredAround(coordinates.Position, new ValueTuple<float, float>(1f, 1f)), true);
			List<ValueTuple<EntityUid, int, uint, float>> list = new List<ValueTuple<EntityUid, int, uint, float>>(hashSet.Count);
			EntityQuery<ClickableComponent> entityQuery = this._entityManager.GetEntityQuery<ClickableComponent>();
			EntityQuery<TransformComponent> entityQuery2 = this._entityManager.GetEntityQuery<TransformComponent>();
			IEye currentEye = this._eyeManager.CurrentEye;
			foreach (ComponentTreeEntry<SpriteComponent> componentTreeEntry in hashSet)
			{
				ClickableComponent clickableComponent;
				int item;
				uint item2;
				float item3;
				if (entityQuery.TryGetComponent(componentTreeEntry.Uid, ref clickableComponent) && clickableComponent.CheckClick(componentTreeEntry.Component, componentTreeEntry.Transform, entityQuery2, coordinates.Position, currentEye, out item, out item2, out item3))
				{
					list.Add(new ValueTuple<EntityUid, int, uint, float>(componentTreeEntry.Uid, item, item2, item3));
				}
			}
			if (list.Count == 0)
			{
				return Array.Empty<EntityUid>();
			}
			list.Sort(this._comparer);
			return from a in list
			select a.Item1;
		}

		// Token: 0x060013AB RID: 5035 RVA: 0x00073E24 File Offset: 0x00072024
		protected virtual void OnKeyBindStateChanged(ViewportBoundKeyEventArgs args)
		{
			InputSystem inputSystem;
			if (!this._entitySystemManager.TryGetEntitySystem<InputSystem>(ref inputSystem))
			{
				return;
			}
			BoundKeyEventArgs keyEventArgs = args.KeyEventArgs;
			BoundKeyFunction function = keyEventArgs.Function;
			KeyFunctionId keyFunctionId = this._inputManager.NetworkBindMap.KeyFunctionID(function);
			EntityCoordinates entityCoordinates = default(EntityCoordinates);
			EntityUid? entityUid = null;
			IViewportControl viewportControl = args.Viewport as IViewportControl;
			if (viewportControl != null)
			{
				MapCoordinates mapCoordinates = viewportControl.ScreenToMap(keyEventArgs.PointerLocation.Position);
				entityUid = this.GetClickedEntity(mapCoordinates);
				MapGridComponent mapGridComponent;
				entityCoordinates = (this._mapManager.TryFindGridAt(mapCoordinates, ref mapGridComponent) ? mapGridComponent.MapToGrid(mapCoordinates) : EntityCoordinates.FromMap(this._mapManager, mapCoordinates));
			}
			FullInputCmdMessage fullInputCmdMessage = new FullInputCmdMessage(this._timing.CurTick, this._timing.TickFraction, keyFunctionId, keyEventArgs.State, entityCoordinates, keyEventArgs.PointerLocation, entityUid.GetValueOrDefault());
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			ICommonSession commonSession = (localPlayer != null) ? localPlayer.Session : null;
			if (inputSystem.HandleInputCommand(commonSession, function, fullInputCmdMessage, false))
			{
				keyEventArgs.Handle();
			}
		}

		// Token: 0x040009D7 RID: 2519
		[Dependency]
		private readonly IEyeManager _eyeManager;

		// Token: 0x040009D8 RID: 2520
		[Dependency]
		private readonly IInputManager _inputManager;

		// Token: 0x040009D9 RID: 2521
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x040009DA RID: 2522
		[Dependency]
		private readonly IEntitySystemManager _entitySystemManager;

		// Token: 0x040009DB RID: 2523
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x040009DC RID: 2524
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x040009DD RID: 2525
		[Dependency]
		protected readonly IUserInterfaceManager UserInterfaceManager;

		// Token: 0x040009DE RID: 2526
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x040009DF RID: 2527
		[Dependency]
		private readonly IViewVariablesManager _vvm;

		// Token: 0x040009E0 RID: 2528
		private GameplayStateBase.ClickableEntityComparer _comparer;

		// Token: 0x0200030A RID: 778
		[NullableContext(0)]
		private sealed class ClickableEntityComparer : IComparer<ValueTuple<EntityUid, int, uint, float>>
		{
			// Token: 0x060013AD RID: 5037 RVA: 0x00073F2C File Offset: 0x0007212C
			public int Compare([TupleElementNames(new string[]
			{
				"clicked",
				"depth",
				"renderOrder",
				"bottom"
			})] ValueTuple<EntityUid, int, uint, float> x, [TupleElementNames(new string[]
			{
				"clicked",
				"depth",
				"renderOrder",
				"bottom"
			})] ValueTuple<EntityUid, int, uint, float> y)
			{
				int num = y.Item2.CompareTo(x.Item2);
				if (num != 0)
				{
					return num;
				}
				num = y.Item3.CompareTo(x.Item3);
				if (num != 0)
				{
					return num;
				}
				num = y.Item4.CompareTo(x.Item4);
				if (num != 0)
				{
					return num;
				}
				return y.Item1.CompareTo(x.Item1);
			}
		}
	}
}
