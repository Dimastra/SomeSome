using System;
using System.Runtime.CompilerServices;
using Content.Server.EUI;
using Content.Server.Explosion.EntitySystems;
using Content.Shared.Administration;
using Content.Shared.Eui;
using Content.Shared.Explosion;
using Robust.Shared.GameObjects;
using Robust.Shared.Log;

namespace Content.Server.Administration.UI
{
	// Token: 0x0200080A RID: 2058
	public sealed class SpawnExplosionEui : BaseEui
	{
		// Token: 0x06002CAA RID: 11434 RVA: 0x000E8A70 File Offset: 0x000E6C70
		[NullableContext(1)]
		public override void HandleMessage(EuiMessageBase msg)
		{
			if (msg is SpawnExplosionEuiMsg.Close)
			{
				base.Close();
				return;
			}
			SpawnExplosionEuiMsg.PreviewRequest request = msg as SpawnExplosionEuiMsg.PreviewRequest;
			if (request == null)
			{
				return;
			}
			if (request.TotalIntensity <= 0f || request.IntensitySlope <= 0f)
			{
				return;
			}
			ExplosionVisualsState explosion = EntitySystem.Get<ExplosionSystem>().GenerateExplosionPreview(request);
			if (explosion == null)
			{
				Logger.Error("Failed to generate explosion preview.");
				return;
			}
			base.SendMessage(new SpawnExplosionEuiMsg.PreviewData(explosion, request.IntensitySlope, request.TotalIntensity));
		}
	}
}
