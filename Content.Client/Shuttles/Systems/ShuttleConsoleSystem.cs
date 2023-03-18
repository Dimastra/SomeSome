using System;
using System.Runtime.CompilerServices;
using Content.Shared.Input;
using Content.Shared.Shuttles.Components;
using Content.Shared.Shuttles.Systems;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Log;

namespace Content.Client.Shuttles.Systems
{
	// Token: 0x02000154 RID: 340
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ShuttleConsoleSystem : SharedShuttleConsoleSystem
	{
		// Token: 0x06000900 RID: 2304 RVA: 0x00035590 File Offset: 0x00033790
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<PilotComponent, ComponentHandleState>(new ComponentEventRefHandler<PilotComponent, ComponentHandleState>(this.OnPilotComponentHandleState), null, null);
			IInputCmdContext inputCmdContext = this._input.Contexts.New("shuttle", "common");
			inputCmdContext.AddFunction(ContentKeyFunctions.ShuttleStrafeUp);
			inputCmdContext.AddFunction(ContentKeyFunctions.ShuttleStrafeDown);
			inputCmdContext.AddFunction(ContentKeyFunctions.ShuttleStrafeLeft);
			inputCmdContext.AddFunction(ContentKeyFunctions.ShuttleStrafeRight);
			inputCmdContext.AddFunction(ContentKeyFunctions.ShuttleRotateLeft);
			inputCmdContext.AddFunction(ContentKeyFunctions.ShuttleRotateRight);
			inputCmdContext.AddFunction(ContentKeyFunctions.ShuttleBrake);
		}

		// Token: 0x06000901 RID: 2305 RVA: 0x0003561D File Offset: 0x0003381D
		public override void Shutdown()
		{
			base.Shutdown();
			this._input.Contexts.Remove("shuttle");
		}

		// Token: 0x06000902 RID: 2306 RVA: 0x0003563C File Offset: 0x0003383C
		protected override void HandlePilotShutdown(EntityUid uid, PilotComponent component, ComponentShutdown args)
		{
			base.HandlePilotShutdown(uid, component, args);
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			bool flag;
			if (localPlayer == null)
			{
				flag = true;
			}
			else
			{
				EntityUid? controlledEntity = localPlayer.ControlledEntity;
				flag = (controlledEntity == null || (controlledEntity != null && controlledEntity.GetValueOrDefault() != uid));
			}
			if (flag)
			{
				return;
			}
			this._input.Contexts.SetActiveContext("human");
		}

		// Token: 0x06000903 RID: 2307 RVA: 0x000356AC File Offset: 0x000338AC
		private void OnPilotComponentHandleState(EntityUid uid, PilotComponent component, ref ComponentHandleState args)
		{
			SharedShuttleConsoleSystem.PilotComponentState pilotComponentState = args.Current as SharedShuttleConsoleSystem.PilotComponentState;
			if (pilotComponentState == null)
			{
				return;
			}
			EntityUid valueOrDefault = pilotComponentState.Console.GetValueOrDefault();
			if (!valueOrDefault.IsValid())
			{
				component.Console = null;
				this._input.Contexts.SetActiveContext("human");
				return;
			}
			ShuttleConsoleComponent console;
			if (!base.TryComp<ShuttleConsoleComponent>(valueOrDefault, ref console))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Unable to set Helmsman console to ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(valueOrDefault);
				Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			component.Console = console;
			this.ActionBlockerSystem.UpdateCanMove(uid, null);
			this._input.Contexts.SetActiveContext("shuttle");
		}

		// Token: 0x04000488 RID: 1160
		[Dependency]
		private readonly IInputManager _input;

		// Token: 0x04000489 RID: 1161
		[Dependency]
		private readonly IPlayerManager _playerManager;
	}
}
