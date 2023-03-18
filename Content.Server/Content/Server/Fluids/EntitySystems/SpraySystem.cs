using System;
using System.Runtime.CompilerServices;
using Content.Server.Chemistry.Components;
using Content.Server.Chemistry.EntitySystems;
using Content.Server.Cooldown;
using Content.Server.Extinguisher;
using Content.Server.Fluids.Components;
using Content.Server.Popups;
using Content.Shared.Chemistry.Components;
using Content.Shared.Cooldown;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Vapor;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Server.Fluids.EntitySystems
{
	// Token: 0x020004F2 RID: 1266
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SpraySystem : EntitySystem
	{
		// Token: 0x06001A1F RID: 6687 RVA: 0x00089C28 File Offset: 0x00087E28
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SprayComponent, AfterInteractEvent>(new ComponentEventHandler<SprayComponent, AfterInteractEvent>(this.OnAfterInteract), null, new Type[]
			{
				typeof(FireExtinguisherSystem)
			});
		}

		// Token: 0x06001A20 RID: 6688 RVA: 0x00089C58 File Offset: 0x00087E58
		private void OnAfterInteract(EntityUid uid, SprayComponent component, AfterInteractEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			args.Handled = true;
			Solution solution;
			if (!this._solutionContainer.TryGetSolution(uid, "spray", out solution, null))
			{
				return;
			}
			SprayAttemptEvent ev = new SprayAttemptEvent(args.User);
			base.RaiseLocalEvent<SprayAttemptEvent>(uid, ev, false);
			if (ev.Cancelled)
			{
				return;
			}
			TimeSpan curTime = this._gameTiming.CurTime;
			ItemCooldownComponent cooldown;
			if (base.TryComp<ItemCooldownComponent>(uid, ref cooldown) && curTime < cooldown.CooldownEnd)
			{
				return;
			}
			if (solution.Volume <= 0)
			{
				this._popupSystem.PopupEntity(Loc.GetString("spray-component-is-empty-message"), uid, args.User, PopupType.Small);
				return;
			}
			EntityQuery<TransformComponent> xformQuery = base.GetEntityQuery<TransformComponent>();
			MapCoordinates userMapPos = xformQuery.GetComponent(args.User).MapPosition;
			Vector2 diffPos = args.ClickLocation.ToMap(this.EntityManager).Position - userMapPos.Position;
			if (diffPos == Vector2.Zero || diffPos == Vector2.NaN)
			{
				return;
			}
			float diffLength = diffPos.Length;
			Vector2 diffNorm = diffPos.Normalized;
			Angle diffAngle = DirectionExtensions.ToAngle(diffNorm);
			Vector2 threeQuarters = diffNorm * 0.75f;
			Vector2 quarter = diffNorm * 0.25f;
			int amount = Math.Max(Math.Min((solution.Volume / component.TransferAmount).Int(), component.VaporAmount), 1);
			float spread = component.VaporSpread / (float)amount;
			for (int i = 0; i < amount; i++)
			{
				Angle rotation;
				rotation..ctor(diffAngle + Angle.FromDegrees((double)(spread * (float)i)) - Angle.FromDegrees((double)(spread * (float)(amount - 1) / 2f)));
				MapCoordinates target = userMapPos.Offset((diffNorm + rotation.ToVec()).Normalized * diffLength + quarter);
				float distance = target.Position.Length;
				if (distance > component.SprayDistance)
				{
					target = userMapPos.Offset(diffNorm * component.SprayDistance);
				}
				Solution newSolution = this._solutionContainer.SplitSolution(uid, solution, component.TransferAmount);
				if (newSolution.Volume <= FixedPoint2.Zero)
				{
					break;
				}
				MapCoordinates vaporPos = userMapPos.Offset((distance < 1f) ? quarter : threeQuarters);
				EntityUid vapor = base.Spawn(component.SprayedPrototype, vaporPos);
				TransformComponent vaporXform = xformQuery.GetComponent(vapor);
				this._transform.SetWorldRotation(vaporXform, rotation);
				AppearanceComponent appearance;
				if (base.TryComp<AppearanceComponent>(vapor, ref appearance))
				{
					this._appearance.SetData(vapor, VaporVisuals.Color, solution.GetColor(this._proto).WithAlpha(1f), appearance);
					this._appearance.SetData(vapor, VaporVisuals.State, true, appearance);
				}
				VaporComponent vaporComponent = base.Comp<VaporComponent>(vapor);
				this._vapor.TryAddSolution(vaporComponent, newSolution);
				Vector2 impulseDirection = rotation.ToVec();
				this._vapor.Start(vaporComponent, vaporXform, impulseDirection, component.SprayVelocity, target, component.SprayAliveTime, new EntityUid?(args.User));
			}
			this._audio.PlayPvs(component.SpraySound, uid, new AudioParams?(component.SpraySound.Params.WithVariation(new float?(0.125f))));
			base.RaiseLocalEvent<RefreshItemCooldownEvent>(uid, new RefreshItemCooldownEvent(curTime, curTime + TimeSpan.FromSeconds((double)component.CooldownTime)), true);
		}

		// Token: 0x0400106B RID: 4203
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x0400106C RID: 4204
		[Dependency]
		private readonly IPrototypeManager _proto;

		// Token: 0x0400106D RID: 4205
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x0400106E RID: 4206
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x0400106F RID: 4207
		[Dependency]
		private readonly SolutionContainerSystem _solutionContainer;

		// Token: 0x04001070 RID: 4208
		[Dependency]
		private readonly VaporSystem _vapor;

		// Token: 0x04001071 RID: 4209
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x04001072 RID: 4210
		[Dependency]
		private readonly TransformSystem _transform;
	}
}
