using System;
using System.Runtime.CompilerServices;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client.CombatMode
{
	// Token: 0x020003B2 RID: 946
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ColoredScreenBorderOverlay : Overlay
	{
		// Token: 0x170004D7 RID: 1239
		// (get) Token: 0x0600177E RID: 6014 RVA: 0x0000689B File Offset: 0x00004A9B
		public override OverlaySpace Space
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x0600177F RID: 6015 RVA: 0x00086E6A File Offset: 0x0008506A
		public ColoredScreenBorderOverlay()
		{
			IoCManager.InjectDependencies<ColoredScreenBorderOverlay>(this);
			this._shader = this._prototypeManager.Index<ShaderPrototype>("ColoredScreenBorder").Instance();
		}

		// Token: 0x06001780 RID: 6016 RVA: 0x00086E94 File Offset: 0x00085094
		protected override void Draw(in OverlayDrawArgs args)
		{
			DrawingHandleWorld worldHandle = args.WorldHandle;
			worldHandle.UseShader(this._shader);
			Box2 worldAABB = args.WorldAABB;
			worldHandle.DrawRect(worldAABB, Color.White, true);
			worldHandle.UseShader(null);
		}

		// Token: 0x04000C02 RID: 3074
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000C03 RID: 3075
		private readonly ShaderInstance _shader;
	}
}
