using System;
using System.Runtime.CompilerServices;
using Content.Server.MachineLinking.Components;
using Content.Shared.Interaction;
using Content.Shared.MachineLinking;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Utility;

namespace Content.Server.MachineLinking.System
{
	// Token: 0x020003F4 RID: 1012
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TwoWayLeverSystem : EntitySystem
	{
		// Token: 0x060014BF RID: 5311 RVA: 0x0006CD48 File Offset: 0x0006AF48
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<TwoWayLeverComponent, ComponentInit>(new ComponentEventHandler<TwoWayLeverComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<TwoWayLeverComponent, ActivateInWorldEvent>(new ComponentEventHandler<TwoWayLeverComponent, ActivateInWorldEvent>(this.OnActivated), null, null);
			base.SubscribeLocalEvent<TwoWayLeverComponent, GetVerbsEvent<InteractionVerb>>(new ComponentEventHandler<TwoWayLeverComponent, GetVerbsEvent<InteractionVerb>>(this.OnGetInteractionVerbs), null, null);
		}

		// Token: 0x060014C0 RID: 5312 RVA: 0x0006CD97 File Offset: 0x0006AF97
		private void OnInit(EntityUid uid, TwoWayLeverComponent component, ComponentInit args)
		{
			this._signalSystem.EnsureTransmitterPorts(uid, new string[]
			{
				component.LeftPort,
				component.RightPort,
				component.MiddlePort
			});
		}

		// Token: 0x060014C1 RID: 5313 RVA: 0x0006CDC8 File Offset: 0x0006AFC8
		private void OnActivated(EntityUid uid, TwoWayLeverComponent component, ActivateInWorldEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			TwoWayLeverState state;
			switch (component.State)
			{
			case TwoWayLeverState.Middle:
				state = (component.NextSignalLeft ? TwoWayLeverState.Left : TwoWayLeverState.Right);
				break;
			case TwoWayLeverState.Right:
				state = TwoWayLeverState.Middle;
				break;
			case TwoWayLeverState.Left:
				state = TwoWayLeverState.Middle;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			component.State = state;
			this.StateChanged(uid, component);
			args.Handled = true;
		}

		// Token: 0x060014C2 RID: 5314 RVA: 0x0006CE30 File Offset: 0x0006B030
		private void OnGetInteractionVerbs(EntityUid uid, TwoWayLeverComponent component, GetVerbsEvent<InteractionVerb> args)
		{
			if (!args.CanAccess || !args.CanInteract || args.Hands == null)
			{
				return;
			}
			InteractionVerb verbLeft = new InteractionVerb
			{
				Act = delegate()
				{
					TwoWayLeverComponent component2 = component;
					TwoWayLeverState state = component.State;
					TwoWayLeverState state2;
					if (state != TwoWayLeverState.Middle)
					{
						if (state != TwoWayLeverState.Right)
						{
							throw new ArgumentOutOfRangeException();
						}
						state2 = TwoWayLeverState.Middle;
					}
					else
					{
						state2 = TwoWayLeverState.Left;
					}
					component2.State = state2;
					this.StateChanged(uid, component);
				},
				Category = VerbCategory.Lever,
				Message = Loc.GetString("two-way-lever-cant"),
				Disabled = (component.State == TwoWayLeverState.Left),
				Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/rotate_ccw.svg.192dpi.png", "/")),
				Text = Loc.GetString("two-way-lever-left")
			};
			args.Verbs.Add(verbLeft);
			InteractionVerb verbRight = new InteractionVerb
			{
				Act = delegate()
				{
					TwoWayLeverComponent component2 = component;
					TwoWayLeverState state = component.State;
					TwoWayLeverState state2;
					if (state != TwoWayLeverState.Middle)
					{
						if (state != TwoWayLeverState.Left)
						{
							throw new ArgumentOutOfRangeException();
						}
						state2 = TwoWayLeverState.Middle;
					}
					else
					{
						state2 = TwoWayLeverState.Right;
					}
					component2.State = state2;
					this.StateChanged(uid, component);
				},
				Category = VerbCategory.Lever,
				Message = Loc.GetString("two-way-lever-cant"),
				Disabled = (component.State == TwoWayLeverState.Right),
				Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/rotate_cw.svg.192dpi.png", "/")),
				Text = Loc.GetString("two-way-lever-right")
			};
			args.Verbs.Add(verbRight);
		}

		// Token: 0x060014C3 RID: 5315 RVA: 0x0006CF70 File Offset: 0x0006B170
		private void StateChanged(EntityUid uid, TwoWayLeverComponent component)
		{
			if (component.State == TwoWayLeverState.Middle)
			{
				component.NextSignalLeft = !component.NextSignalLeft;
			}
			AppearanceComponent appearance;
			if (base.TryComp<AppearanceComponent>(uid, ref appearance))
			{
				this._appearance.SetData(uid, TwoWayLeverVisuals.State, component.State, appearance);
			}
			string text;
			switch (component.State)
			{
			case TwoWayLeverState.Middle:
				text = component.MiddlePort;
				break;
			case TwoWayLeverState.Right:
				text = component.RightPort;
				break;
			case TwoWayLeverState.Left:
				text = component.LeftPort;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			string port = text;
			this._signalSystem.InvokePort(uid, port, null);
		}

		// Token: 0x04000CD2 RID: 3282
		[Dependency]
		private readonly SignalLinkerSystem _signalSystem;

		// Token: 0x04000CD3 RID: 3283
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x04000CD4 RID: 3284
		private const string _leftToggleImage = "rotate_ccw.svg.192dpi.png";

		// Token: 0x04000CD5 RID: 3285
		private const string _rightToggleImage = "rotate_cw.svg.192dpi.png";
	}
}
