using System;
using System.Runtime.CompilerServices;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Client.Interactable.Components
{
	// Token: 0x020002A9 RID: 681
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class InteractionOutlineComponent : Component
	{
		// Token: 0x0600113D RID: 4413 RVA: 0x00066784 File Offset: 0x00064984
		public void OnMouseEnter(bool inInteractionRange, int renderScale)
		{
			this._lastRenderScale = renderScale;
			this._inRange = inInteractionRange;
			SpriteComponent spriteComponent;
			if (this._entMan.TryGetComponent<SpriteComponent>(base.Owner, ref spriteComponent) && spriteComponent.PostShader == null)
			{
				this._shader = this.MakeNewShader(inInteractionRange, renderScale);
				spriteComponent.PostShader = this._shader;
			}
		}

		// Token: 0x0600113E RID: 4414 RVA: 0x000667D8 File Offset: 0x000649D8
		public void OnMouseLeave()
		{
			SpriteComponent spriteComponent;
			if (this._entMan.TryGetComponent<SpriteComponent>(base.Owner, ref spriteComponent))
			{
				if (spriteComponent.PostShader == this._shader)
				{
					spriteComponent.PostShader = null;
				}
				spriteComponent.RenderOrder = 0U;
			}
			ShaderInstance shader = this._shader;
			if (shader != null)
			{
				shader.Dispose();
			}
			this._shader = null;
		}

		// Token: 0x0600113F RID: 4415 RVA: 0x00066830 File Offset: 0x00064A30
		public void UpdateInRange(bool inInteractionRange, int renderScale)
		{
			SpriteComponent spriteComponent;
			if (this._entMan.TryGetComponent<SpriteComponent>(base.Owner, ref spriteComponent) && spriteComponent.PostShader == this._shader && (inInteractionRange != this._inRange || this._lastRenderScale != renderScale))
			{
				this._inRange = inInteractionRange;
				this._lastRenderScale = renderScale;
				this._shader = this.MakeNewShader(this._inRange, this._lastRenderScale);
				spriteComponent.PostShader = this._shader;
			}
		}

		// Token: 0x06001140 RID: 4416 RVA: 0x000668A4 File Offset: 0x00064AA4
		private ShaderInstance MakeNewShader(bool inRange, int renderScale)
		{
			string text = inRange ? "SelectionOutlineInrange" : "SelectionOutline";
			ShaderInstance shaderInstance = this._prototypeManager.Index<ShaderPrototype>(text).InstanceUnique();
			shaderInstance.SetParameter("outline_width", 1f * (float)renderScale);
			return shaderInstance;
		}

		// Token: 0x0400086F RID: 2159
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000870 RID: 2160
		[Dependency]
		private readonly IEntityManager _entMan;

		// Token: 0x04000871 RID: 2161
		private const float DefaultWidth = 1f;

		// Token: 0x04000872 RID: 2162
		private const string ShaderInRange = "SelectionOutlineInrange";

		// Token: 0x04000873 RID: 2163
		private const string ShaderOutOfRange = "SelectionOutline";

		// Token: 0x04000874 RID: 2164
		private bool _inRange;

		// Token: 0x04000875 RID: 2165
		[Nullable(2)]
		private ShaderInstance _shader;

		// Token: 0x04000876 RID: 2166
		private int _lastRenderScale;
	}
}
