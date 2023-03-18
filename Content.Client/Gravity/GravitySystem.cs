using System;
using System.Runtime.CompilerServices;
using Content.Shared.Camera;
using Content.Shared.Gravity;
using Robust.Client.Player;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Random;

namespace Content.Client.Gravity
{
	// Token: 0x020002FA RID: 762
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GravitySystem : SharedGravitySystem
	{
		// Token: 0x0600131D RID: 4893 RVA: 0x00071AD0 File Offset: 0x0006FCD0
		public override void Initialize()
		{
			base.Initialize();
			this.InitializeShake();
		}

		// Token: 0x0600131E RID: 4894 RVA: 0x00071ADE File Offset: 0x0006FCDE
		private void InitializeShake()
		{
			base.SubscribeLocalEvent<GravityShakeComponent, ComponentInit>(new ComponentEventHandler<GravityShakeComponent, ComponentInit>(this.OnShakeInit), null, null);
		}

		// Token: 0x0600131F RID: 4895 RVA: 0x00071AF4 File Offset: 0x0006FCF4
		private void OnShakeInit(EntityUid uid, GravityShakeComponent component, ComponentInit args)
		{
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			EntityUid? entityUid = (localPlayer != null) ? localPlayer.ControlledEntity : null;
			TransformComponent transformComponent;
			if (base.TryComp<TransformComponent>(entityUid, ref transformComponent))
			{
				EntityUid? entityUid2 = transformComponent.GridUid;
				if (entityUid2 == null || (entityUid2 != null && entityUid2.GetValueOrDefault() != uid))
				{
					entityUid2 = transformComponent.MapUid;
					if (entityUid2 == null || (entityUid2 != null && entityUid2.GetValueOrDefault() != uid))
					{
						return;
					}
				}
				GravityComponent gravityComponent;
				if (this.Timing.IsFirstTimePredicted && base.TryComp<GravityComponent>(uid, ref gravityComponent))
				{
					this._audio.PlayGlobal(gravityComponent.GravityShakeSound, Filter.Local(), true, new AudioParams?(AudioParams.Default.WithVolume(-2f)));
				}
				return;
			}
		}

		// Token: 0x06001320 RID: 4896 RVA: 0x00071BD8 File Offset: 0x0006FDD8
		[NullableContext(2)]
		protected override void ShakeGrid(EntityUid uid, GravityComponent gravity = null)
		{
			base.ShakeGrid(uid, gravity);
			if (!base.Resolve<GravityComponent>(uid, ref gravity, true) || !this.Timing.IsFirstTimePredicted)
			{
				return;
			}
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			EntityUid? entityUid = (localPlayer != null) ? localPlayer.ControlledEntity : null;
			TransformComponent transformComponent;
			if (!base.TryComp<TransformComponent>(entityUid, ref transformComponent))
			{
				return;
			}
			EntityUid? entityUid2 = transformComponent.GridUid;
			if (entityUid2 != null && (entityUid2 == null || !(entityUid2.GetValueOrDefault() != uid)))
			{
				if (transformComponent.GridUid == null)
				{
					entityUid2 = transformComponent.MapUid;
					if (entityUid2 == null || (entityUid2 != null && entityUid2.GetValueOrDefault() != uid))
					{
						return;
					}
				}
				Vector2 kickback = new Vector2(this._random.NextFloat(), this._random.NextFloat()) * 100f;
				this._sharedCameraRecoil.KickCamera(entityUid.Value, kickback, null);
				return;
			}
		}

		// Token: 0x04000993 RID: 2451
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000994 RID: 2452
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000995 RID: 2453
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x04000996 RID: 2454
		[Dependency]
		private readonly SharedCameraRecoilSystem _sharedCameraRecoil;
	}
}
