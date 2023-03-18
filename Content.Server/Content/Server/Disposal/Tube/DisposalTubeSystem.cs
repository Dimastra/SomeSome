using System;
using System.Runtime.CompilerServices;
using System.Text;
using Content.Server.Construction.Completions;
using Content.Server.Disposal.Tube.Components;
using Content.Server.Hands.Components;
using Content.Server.UserInterface;
using Content.Shared.Destructible;
using Content.Shared.Disposal.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Popups;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Server.Disposal.Tube
{
	// Token: 0x02000556 RID: 1366
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DisposalTubeSystem : EntitySystem
	{
		// Token: 0x06001CD3 RID: 7379 RVA: 0x000999B8 File Offset: 0x00097BB8
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<DisposalTubeComponent, AnchorStateChangedEvent>(new ComponentEventRefHandler<DisposalTubeComponent, AnchorStateChangedEvent>(this.OnAnchorChange), null, null);
			base.SubscribeLocalEvent<DisposalTubeComponent, ContainerRelayMovementEntityEvent>(new ComponentEventRefHandler<DisposalTubeComponent, ContainerRelayMovementEntityEvent>(this.OnRelayMovement), null, null);
			base.SubscribeLocalEvent<DisposalTubeComponent, BreakageEventArgs>(new ComponentEventHandler<DisposalTubeComponent, BreakageEventArgs>(this.OnBreak), null, null);
			base.SubscribeLocalEvent<DisposalRouterComponent, ActivatableUIOpenAttemptEvent>(new ComponentEventHandler<DisposalRouterComponent, ActivatableUIOpenAttemptEvent>(this.OnOpenRouterUIAttempt), null, null);
			base.SubscribeLocalEvent<DisposalTaggerComponent, ActivatableUIOpenAttemptEvent>(new ComponentEventHandler<DisposalTaggerComponent, ActivatableUIOpenAttemptEvent>(this.OnOpenTaggerUIAttempt), null, null);
			base.SubscribeLocalEvent<DisposalTubeComponent, ComponentStartup>(new ComponentEventHandler<DisposalTubeComponent, ComponentStartup>(this.OnStartup), null, null);
			base.SubscribeLocalEvent<DisposalTubeComponent, ConstructionBeforeDeleteEvent>(new ComponentEventHandler<DisposalTubeComponent, ConstructionBeforeDeleteEvent>(this.OnDeconstruct), null, null);
		}

		// Token: 0x06001CD4 RID: 7380 RVA: 0x00099A57 File Offset: 0x00097C57
		private void OnDeconstruct(EntityUid uid, DisposalTubeComponent component, ConstructionBeforeDeleteEvent args)
		{
			component.Disconnect();
		}

		// Token: 0x06001CD5 RID: 7381 RVA: 0x00099A5F File Offset: 0x00097C5F
		private void OnStartup(EntityUid uid, DisposalTubeComponent component, ComponentStartup args)
		{
			this.UpdateAnchored(uid, component, base.Transform(uid).Anchored);
		}

		// Token: 0x06001CD6 RID: 7382 RVA: 0x00099A78 File Offset: 0x00097C78
		private void OnRelayMovement(EntityUid uid, DisposalTubeComponent component, ref ContainerRelayMovementEntityEvent args)
		{
			if (this._gameTiming.CurTime < component.LastClang + DisposalTubeComponent.ClangDelay)
			{
				return;
			}
			component.LastClang = this._gameTiming.CurTime;
			SoundSystem.Play(component.ClangSound.GetSound(null, null), Filter.Pvs(uid, 2f, null, null, null), uid, null);
		}

		// Token: 0x06001CD7 RID: 7383 RVA: 0x00099AE4 File Offset: 0x00097CE4
		private void OnBreak(EntityUid uid, DisposalTubeComponent component, BreakageEventArgs args)
		{
			component.Disconnect();
		}

		// Token: 0x06001CD8 RID: 7384 RVA: 0x00099AEC File Offset: 0x00097CEC
		private void OnOpenRouterUIAttempt(EntityUid uid, DisposalRouterComponent router, ActivatableUIOpenAttemptEvent args)
		{
			HandsComponent hands;
			if (!base.TryComp<HandsComponent>(args.User, ref hands))
			{
				uid.PopupMessage(args.User, Loc.GetString("disposal-router-window-tag-input-activate-no-hands"));
				return;
			}
			if (hands.ActiveHandEntity != null)
			{
				args.Cancel();
			}
			this.UpdateRouterUserInterface(router);
		}

		// Token: 0x06001CD9 RID: 7385 RVA: 0x00099B40 File Offset: 0x00097D40
		private void OnOpenTaggerUIAttempt(EntityUid uid, DisposalTaggerComponent tagger, ActivatableUIOpenAttemptEvent args)
		{
			HandsComponent hands;
			if (!base.TryComp<HandsComponent>(args.User, ref hands))
			{
				uid.PopupMessage(args.User, Loc.GetString("disposal-tagger-window-activate-no-hands"));
				return;
			}
			if (hands.ActiveHandEntity != null)
			{
				args.Cancel();
			}
			BoundUserInterface userInterface = tagger.UserInterface;
			if (userInterface == null)
			{
				return;
			}
			userInterface.SetState(new SharedDisposalTaggerComponent.DisposalTaggerUserInterfaceState(tagger.Tag), null, true);
		}

		// Token: 0x06001CDA RID: 7386 RVA: 0x00099BA8 File Offset: 0x00097DA8
		private void UpdateRouterUserInterface(DisposalRouterComponent router)
		{
			if (router.Tags.Count <= 0)
			{
				BoundUserInterface userInterface = router.UserInterface;
				if (userInterface == null)
				{
					return;
				}
				userInterface.SetState(new SharedDisposalRouterComponent.DisposalRouterUserInterfaceState(""), null, true);
				return;
			}
			else
			{
				StringBuilder taglist = new StringBuilder();
				foreach (string tag in router.Tags)
				{
					taglist.Append(tag);
					taglist.Append(", ");
				}
				taglist.Remove(taglist.Length - 2, 2);
				BoundUserInterface userInterface2 = router.UserInterface;
				if (userInterface2 == null)
				{
					return;
				}
				userInterface2.SetState(new SharedDisposalRouterComponent.DisposalRouterUserInterfaceState(taglist.ToString()), null, true);
				return;
			}
		}

		// Token: 0x06001CDB RID: 7387 RVA: 0x00099C68 File Offset: 0x00097E68
		private void OnAnchorChange(EntityUid uid, DisposalTubeComponent component, ref AnchorStateChangedEvent args)
		{
			this.UpdateAnchored(uid, component, args.Anchored);
		}

		// Token: 0x06001CDC RID: 7388 RVA: 0x00099C78 File Offset: 0x00097E78
		private void UpdateAnchored(EntityUid uid, DisposalTubeComponent component, bool anchored)
		{
			if (anchored)
			{
				component.Connect();
				this._appearanceSystem.SetData(uid, DisposalTubeVisuals.VisualState, DisposalTubeVisualState.Anchored, null);
				return;
			}
			component.Disconnect();
			this._appearanceSystem.SetData(uid, DisposalTubeVisuals.VisualState, DisposalTubeVisualState.Free, null);
		}

		// Token: 0x06001CDD RID: 7389 RVA: 0x00099CC8 File Offset: 0x00097EC8
		[NullableContext(2)]
		public IDisposalTubeComponent NextTubeFor(EntityUid target, Direction nextDirection, IDisposalTubeComponent targetTube = null)
		{
			if (!base.Resolve<IDisposalTubeComponent>(target, ref targetTube, true))
			{
				return null;
			}
			Direction oppositeDirection = DirectionExtensions.GetOpposite(nextDirection);
			TransformComponent xform = base.Transform(targetTube.Owner);
			MapGridComponent grid;
			if (!this._mapManager.TryGetGrid(xform.GridUid, ref grid))
			{
				return null;
			}
			EntityCoordinates position = xform.Coordinates;
			foreach (EntityUid entity in grid.GetInDir(position, nextDirection))
			{
				IDisposalTubeComponent tube;
				if (this.EntityManager.TryGetComponent<IDisposalTubeComponent>(entity, ref tube) && tube.CanConnect(oppositeDirection, targetTube) && targetTube.CanConnect(nextDirection, tube))
				{
					return tube;
				}
			}
			return null;
		}

		// Token: 0x04001275 RID: 4725
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x04001276 RID: 4726
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x04001277 RID: 4727
		[Dependency]
		private readonly SharedAppearanceSystem _appearanceSystem;
	}
}
