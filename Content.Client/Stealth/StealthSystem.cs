using System;
using System.Runtime.CompilerServices;
using Content.Client.Interactable.Components;
using Content.Shared.Stealth;
using Content.Shared.Stealth.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client.Stealth
{
	// Token: 0x0200012F RID: 303
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class StealthSystem : SharedStealthSystem
	{
		// Token: 0x06000828 RID: 2088 RVA: 0x0002F6E8 File Offset: 0x0002D8E8
		public override void Initialize()
		{
			base.Initialize();
			this._shader = this._protoMan.Index<ShaderPrototype>("Stealth").InstanceUnique();
			base.SubscribeLocalEvent<StealthComponent, ComponentRemove>(new ComponentEventHandler<StealthComponent, ComponentRemove>(this.OnRemove), null, null);
			base.SubscribeLocalEvent<StealthComponent, BeforePostShaderRenderEvent>(new ComponentEventHandler<StealthComponent, BeforePostShaderRenderEvent>(this.OnShaderRender), null, null);
		}

		// Token: 0x06000829 RID: 2089 RVA: 0x0002F73E File Offset: 0x0002D93E
		[NullableContext(2)]
		public override void SetEnabled(EntityUid uid, bool value, StealthComponent component = null)
		{
			if (!base.Resolve<StealthComponent>(uid, ref component, true) || component.Enabled == value)
			{
				return;
			}
			base.SetEnabled(uid, value, component);
			this.SetShader(uid, value, component, null);
		}

		// Token: 0x0600082A RID: 2090 RVA: 0x0002F76C File Offset: 0x0002D96C
		[NullableContext(2)]
		private void SetShader(EntityUid uid, bool enabled, StealthComponent component = null, SpriteComponent sprite = null)
		{
			if (!base.Resolve<StealthComponent, SpriteComponent>(uid, ref component, ref sprite, false))
			{
				return;
			}
			sprite.Color = Color.White;
			sprite.PostShader = (enabled ? this._shader : null);
			sprite.GetScreenTexture = enabled;
			sprite.RaiseShaderEvent = enabled;
			if (!enabled)
			{
				if (component.HadOutline)
				{
					base.AddComp<InteractionOutlineComponent>(uid);
				}
				return;
			}
			InteractionOutlineComponent interactionOutlineComponent;
			if (base.TryComp<InteractionOutlineComponent>(uid, ref interactionOutlineComponent))
			{
				base.RemCompDeferred(uid, interactionOutlineComponent);
				component.HadOutline = true;
			}
		}

		// Token: 0x0600082B RID: 2091 RVA: 0x0002F7E5 File Offset: 0x0002D9E5
		protected override void OnInit(EntityUid uid, StealthComponent component, ComponentInit args)
		{
			base.OnInit(uid, component, args);
			this.SetShader(uid, component.Enabled, component, null);
		}

		// Token: 0x0600082C RID: 2092 RVA: 0x0002F7FF File Offset: 0x0002D9FF
		private void OnRemove(EntityUid uid, StealthComponent component, ComponentRemove args)
		{
			this.SetShader(uid, false, component, null);
		}

		// Token: 0x0600082D RID: 2093 RVA: 0x0002F80C File Offset: 0x0002DA0C
		private void OnShaderRender(EntityUid uid, StealthComponent component, BeforePostShaderRenderEvent args)
		{
			EntityUid parentUid = base.Transform(uid).ParentUid;
			if (!parentUid.IsValid())
			{
				return;
			}
			TransformComponent transformComponent = base.Transform(parentUid);
			Vector2 vector = args.Viewport.WorldToLocal(transformComponent.WorldPosition);
			vector.X = -vector.X;
			float num = base.GetVisibility(uid, component);
			num = Math.Clamp(num, -1f, 1f);
			this._shader.SetParameter("reference", vector);
			this._shader.SetParameter("visibility", num);
			num = MathF.Max(0f, num);
			args.Sprite.Color = new Color(num, num, 1f, 1f);
		}

		// Token: 0x04000425 RID: 1061
		[Dependency]
		private readonly IPrototypeManager _protoMan;

		// Token: 0x04000426 RID: 1062
		private ShaderInstance _shader;
	}
}
