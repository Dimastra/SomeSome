using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Interaction;
using Content.Shared.Physics;
using Content.Shared.Whitelist;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client.Outline
{
	// Token: 0x020001F5 RID: 501
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TargetOutlineSystem : EntitySystem
	{
		// Token: 0x06000CC6 RID: 3270 RVA: 0x0004AA68 File Offset: 0x00048C68
		public override void Initialize()
		{
			base.Initialize();
			this._shaderTargetValid = this._prototypeManager.Index<ShaderPrototype>("SelectionOutlineInrange").InstanceUnique();
			this._shaderTargetInvalid = this._prototypeManager.Index<ShaderPrototype>("SelectionOutline").InstanceUnique();
		}

		// Token: 0x06000CC7 RID: 3271 RVA: 0x0004AAA6 File Offset: 0x00048CA6
		public void Disable()
		{
			if (!this._enabled)
			{
				return;
			}
			this._enabled = false;
			this.RemoveHighlights();
		}

		// Token: 0x06000CC8 RID: 3272 RVA: 0x0004AAC0 File Offset: 0x00048CC0
		[NullableContext(2)]
		public void Enable(float range, bool checkObstructions, Func<EntityUid, bool> predicate, EntityWhitelist whitelist, CancellableEntityEventArgs validationEvent)
		{
			this.Range = range;
			this.CheckObstruction = checkObstructions;
			this.Predicate = predicate;
			this.Whitelist = whitelist;
			this.ValidationEvent = validationEvent;
			this._enabled = (this.Predicate != null || this.Whitelist != null || this.ValidationEvent != null);
		}

		// Token: 0x06000CC9 RID: 3273 RVA: 0x0004AB14 File Offset: 0x00048D14
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			if (!this._enabled || !this._timing.IsFirstTimePredicted)
			{
				return;
			}
			this.HighlightTargets();
		}

		// Token: 0x06000CCA RID: 3274 RVA: 0x0004AB3C File Offset: 0x00048D3C
		private void HighlightTargets()
		{
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			EntityUid? entityUid = (localPlayer != null) ? localPlayer.ControlledEntity : null;
			if (entityUid != null)
			{
				EntityUid valueOrDefault = entityUid.GetValueOrDefault();
				if (valueOrDefault.Valid)
				{
					this.RemoveHighlights();
					Vector2 position = this._eyeManager.ScreenToMap(this._inputManager.MouseScreenPosition).Position;
					Box2 box;
					box..ctor(position - this.LookupSize / 2f, position + this.LookupSize / 2f);
					HashSet<EntityUid> entitiesIntersecting = this._lookup.GetEntitiesIntersecting(this._eyeManager.CurrentMap, box, 5);
					EntityQuery<SpriteComponent> entityQuery = base.GetEntityQuery<SpriteComponent>();
					foreach (EntityUid entityUid2 in entitiesIntersecting)
					{
						SpriteComponent spriteComponent;
						if (entityQuery.TryGetComponent(entityUid2, ref spriteComponent) && spriteComponent.Visible)
						{
							Func<EntityUid, bool> predicate = this.Predicate;
							bool flag = predicate == null || predicate(entityUid2);
							if (flag && this.Whitelist != null)
							{
								flag = this.Whitelist.IsValid(entityUid2, null);
							}
							if (flag && this.ValidationEvent != null)
							{
								this.ValidationEvent.Uncancel();
								base.RaiseLocalEvent(entityUid2, this.ValidationEvent, false);
								flag = !this.ValidationEvent.Cancelled;
							}
							if (!flag)
							{
								if (this._highlightedSprites.Remove(spriteComponent) && (spriteComponent.PostShader == this._shaderTargetValid || spriteComponent.PostShader == this._shaderTargetInvalid))
								{
									spriteComponent.PostShader = null;
									spriteComponent.RenderOrder = 0U;
								}
							}
							else
							{
								if (this.CheckObstruction)
								{
									flag = this._interactionSystem.InRangeUnobstructed(valueOrDefault, entityUid2, this.Range, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, false);
								}
								else if (this.Range >= 0f)
								{
									Vector2 worldPosition = base.Transform(valueOrDefault).WorldPosition;
									Vector2 worldPosition2 = base.Transform(entityUid2).WorldPosition;
									flag = ((worldPosition - worldPosition2).LengthSquared <= this.Range);
								}
								if (spriteComponent.PostShader != null && spriteComponent.PostShader != this._shaderTargetValid && spriteComponent.PostShader != this._shaderTargetInvalid)
								{
									break;
								}
								spriteComponent.PostShader = (flag ? this._shaderTargetValid : this._shaderTargetInvalid);
								spriteComponent.RenderOrder = this.EntityManager.CurrentTick.Value;
								this._highlightedSprites.Add(spriteComponent);
							}
						}
					}
					return;
				}
			}
		}

		// Token: 0x06000CCB RID: 3275 RVA: 0x0004ADE4 File Offset: 0x00048FE4
		private void RemoveHighlights()
		{
			foreach (SpriteComponent spriteComponent in this._highlightedSprites)
			{
				if (spriteComponent.PostShader == this._shaderTargetValid || spriteComponent.PostShader == this._shaderTargetInvalid)
				{
					spriteComponent.PostShader = null;
					spriteComponent.RenderOrder = 0U;
				}
			}
			this._highlightedSprites.Clear();
		}

		// Token: 0x04000682 RID: 1666
		[Dependency]
		private readonly IEyeManager _eyeManager;

		// Token: 0x04000683 RID: 1667
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x04000684 RID: 1668
		[Dependency]
		private readonly EntityLookupSystem _lookup;

		// Token: 0x04000685 RID: 1669
		[Dependency]
		private readonly IInputManager _inputManager;

		// Token: 0x04000686 RID: 1670
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000687 RID: 1671
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000688 RID: 1672
		[Dependency]
		private readonly SharedInteractionSystem _interactionSystem;

		// Token: 0x04000689 RID: 1673
		private bool _enabled;

		// Token: 0x0400068A RID: 1674
		[Nullable(2)]
		public EntityWhitelist Whitelist;

		// Token: 0x0400068B RID: 1675
		[Nullable(2)]
		public Func<EntityUid, bool> Predicate;

		// Token: 0x0400068C RID: 1676
		[Nullable(2)]
		public CancellableEntityEventArgs ValidationEvent;

		// Token: 0x0400068D RID: 1677
		public float Range = -1f;

		// Token: 0x0400068E RID: 1678
		public bool CheckObstruction = true;

		// Token: 0x0400068F RID: 1679
		public float LookupSize = 2f;

		// Token: 0x04000690 RID: 1680
		private const string ShaderTargetValid = "SelectionOutlineInrange";

		// Token: 0x04000691 RID: 1681
		private const string ShaderTargetInvalid = "SelectionOutline";

		// Token: 0x04000692 RID: 1682
		[Nullable(2)]
		private ShaderInstance _shaderTargetValid;

		// Token: 0x04000693 RID: 1683
		[Nullable(2)]
		private ShaderInstance _shaderTargetInvalid;

		// Token: 0x04000694 RID: 1684
		private readonly HashSet<SpriteComponent> _highlightedSprites = new HashSet<SpriteComponent>();
	}
}
