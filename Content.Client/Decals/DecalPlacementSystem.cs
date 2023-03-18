using System;
using System.Runtime.CompilerServices;
using Content.Client.Actions;
using Content.Client.Decals.Overlays;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.Decals;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Players;
using Robust.Shared.Prototypes;

namespace Content.Client.Decals
{
	// Token: 0x02000359 RID: 857
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DecalPlacementSystem : EntitySystem
	{
		// Token: 0x0600152C RID: 5420 RVA: 0x0007C6A0 File Offset: 0x0007A8A0
		[return: TupleElementNames(new string[]
		{
			"Decal",
			"Snap",
			"Angle",
			"Color"
		})]
		[return: Nullable(new byte[]
		{
			0,
			2
		})]
		public ValueTuple<DecalPrototype, bool, Angle, Color> GetActiveDecal()
		{
			if (!this._active || this._decalId == null)
			{
				return new ValueTuple<DecalPrototype, bool, Angle, Color>(null, false, Angle.Zero, Color.Wheat);
			}
			return new ValueTuple<DecalPrototype, bool, Angle, Color>(this._protoMan.Index<DecalPrototype>(this._decalId), this._snap, this._decalAngle, this._decalColor);
		}

		// Token: 0x0600152D RID: 5421 RVA: 0x0007C6F8 File Offset: 0x0007A8F8
		public override void Initialize()
		{
			base.Initialize();
			this._overlay.AddOverlay(new DecalPlacementOverlay(this, this._transform, this._sprite));
			CommandBinds.Builder.Bind(EngineKeyFunctions.EditorPlaceObject, new PointerStateInputCmdHandler(delegate(ICommonSession session, EntityCoordinates coords, EntityUid uid)
			{
				if (!this._active || this._placing || this._decalId == null)
				{
					return false;
				}
				this._placing = true;
				if (this._snap)
				{
					Vector2 vector;
					vector..ctor((float)((double)MathF.Round(coords.X - 0.5f, MidpointRounding.AwayFromZero) + 0.5), (float)((double)MathF.Round(coords.Y - 0.5f, MidpointRounding.AwayFromZero) + 0.5));
					coords = coords.WithPosition(vector);
				}
				coords = coords.Offset(new Vector2(-0.5f, -0.5f));
				if (!coords.IsValid(this.EntityManager))
				{
					return false;
				}
				Decal decal = new Decal(coords.Position, this._decalId, new Color?(this._decalColor), this._decalAngle, this._zIndex, this._cleanable);
				base.RaiseNetworkEvent(new RequestDecalPlacementEvent(decal, coords));
				return true;
			}, delegate(ICommonSession session, EntityCoordinates coords, EntityUid uid)
			{
				if (!this._active)
				{
					return false;
				}
				this._placing = false;
				return true;
			}, true)).Bind(EngineKeyFunctions.EditorCancelPlace, new PointerStateInputCmdHandler(delegate(ICommonSession session, EntityCoordinates coords, EntityUid uid)
			{
				if (!this._active || this._erasing)
				{
					return false;
				}
				this._erasing = true;
				base.RaiseNetworkEvent(new RequestDecalRemovalEvent(coords));
				return true;
			}, delegate(ICommonSession session, EntityCoordinates coords, EntityUid uid)
			{
				if (!this._active)
				{
					return false;
				}
				this._erasing = false;
				return true;
			}, true)).Register<DecalPlacementSystem>();
			base.SubscribeLocalEvent<FillActionSlotEvent>(new EntityEventHandler<FillActionSlotEvent>(this.OnFillSlot), null, null);
			base.SubscribeLocalEvent<PlaceDecalActionEvent>(new EntityEventHandler<PlaceDecalActionEvent>(this.OnPlaceDecalAction), null, null);
		}

		// Token: 0x0600152E RID: 5422 RVA: 0x0007C7AC File Offset: 0x0007A9AC
		private void OnPlaceDecalAction(PlaceDecalActionEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			if (args.Target.GetGridUid(this.EntityManager) == null)
			{
				return;
			}
			args.Handled = true;
			if (args.Snap)
			{
				Vector2 vector;
				vector..ctor((float)((double)MathF.Round(args.Target.X - 0.5f, MidpointRounding.AwayFromZero) + 0.5), (float)((double)MathF.Round(args.Target.Y - 0.5f, MidpointRounding.AwayFromZero) + 0.5));
				args.Target = args.Target.WithPosition(vector);
			}
			args.Target = args.Target.Offset(new Vector2(-0.5f, -0.5f));
			Decal decal = new Decal(args.Target.Position, args.DecalId, new Color?(args.Color), Angle.FromDegrees(args.Rotation), args.ZIndex, args.Cleanable);
			base.RaiseNetworkEvent(new RequestDecalPlacementEvent(decal, args.Target));
		}

		// Token: 0x0600152F RID: 5423 RVA: 0x0007C8B8 File Offset: 0x0007AAB8
		private void OnFillSlot(FillActionSlotEvent ev)
		{
			if (!this._active || this._placing)
			{
				return;
			}
			if (ev.Action != null)
			{
				return;
			}
			DecalPrototype decalPrototype;
			if (this._decalId == null || !this._protoMan.TryIndex<DecalPrototype>(this._decalId, ref decalPrototype))
			{
				return;
			}
			PlaceDecalActionEvent @event = new PlaceDecalActionEvent
			{
				DecalId = this._decalId,
				Color = this._decalColor,
				Rotation = this._decalAngle.Degrees,
				Snap = this._snap,
				ZIndex = this._zIndex,
				Cleanable = this._cleanable
			};
			WorldTargetAction worldTargetAction = new WorldTargetAction();
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(5, 3);
			defaultInterpolatedStringHandler.AppendFormatted(this._decalId);
			defaultInterpolatedStringHandler.AppendLiteral(" (");
			defaultInterpolatedStringHandler.AppendFormatted(this._decalColor.ToHex());
			defaultInterpolatedStringHandler.AppendLiteral(", ");
			defaultInterpolatedStringHandler.AppendFormatted<int>((int)this._decalAngle.Degrees);
			defaultInterpolatedStringHandler.AppendLiteral(")");
			worldTargetAction.DisplayName = defaultInterpolatedStringHandler.ToStringAndClear();
			worldTargetAction.Icon = decalPrototype.Sprite;
			worldTargetAction.Repeat = true;
			worldTargetAction.CheckCanAccess = false;
			worldTargetAction.CheckCanInteract = false;
			worldTargetAction.Range = -1f;
			worldTargetAction.Event = @event;
			worldTargetAction.IconColor = this._decalColor;
			ev.Action = worldTargetAction;
		}

		// Token: 0x06001530 RID: 5424 RVA: 0x0007CA06 File Offset: 0x0007AC06
		public override void Shutdown()
		{
			base.Shutdown();
			this._overlay.RemoveOverlay<DecalPlacementOverlay>();
			CommandBinds.Unregister<DecalPlacementSystem>();
		}

		// Token: 0x06001531 RID: 5425 RVA: 0x0007CA1F File Offset: 0x0007AC1F
		public void UpdateDecalInfo(string id, Color color, float rotation, bool snap, int zIndex, bool cleanable)
		{
			this._decalId = id;
			this._decalColor = color;
			this._decalAngle = Angle.FromDegrees((double)rotation);
			this._snap = snap;
			this._zIndex = zIndex;
			this._cleanable = cleanable;
		}

		// Token: 0x06001532 RID: 5426 RVA: 0x0007CA54 File Offset: 0x0007AC54
		public void SetActive(bool active)
		{
			this._active = active;
			if (this._active)
			{
				this._inputManager.Contexts.SetActiveContext("editor");
				return;
			}
			this._inputSystem.SetEntityContextActive();
		}

		// Token: 0x04000AFA RID: 2810
		[Dependency]
		private readonly IInputManager _inputManager;

		// Token: 0x04000AFB RID: 2811
		[Dependency]
		private readonly IOverlayManager _overlay;

		// Token: 0x04000AFC RID: 2812
		[Dependency]
		private readonly IPrototypeManager _protoMan;

		// Token: 0x04000AFD RID: 2813
		[Dependency]
		private readonly InputSystem _inputSystem;

		// Token: 0x04000AFE RID: 2814
		[Dependency]
		private readonly SharedTransformSystem _transform;

		// Token: 0x04000AFF RID: 2815
		[Dependency]
		private readonly SpriteSystem _sprite;

		// Token: 0x04000B00 RID: 2816
		[Nullable(2)]
		private string _decalId;

		// Token: 0x04000B01 RID: 2817
		private Color _decalColor = Color.White;

		// Token: 0x04000B02 RID: 2818
		private Angle _decalAngle = Angle.Zero;

		// Token: 0x04000B03 RID: 2819
		private bool _snap;

		// Token: 0x04000B04 RID: 2820
		private int _zIndex;

		// Token: 0x04000B05 RID: 2821
		private bool _cleanable;

		// Token: 0x04000B06 RID: 2822
		private bool _active;

		// Token: 0x04000B07 RID: 2823
		private bool _placing;

		// Token: 0x04000B08 RID: 2824
		private bool _erasing;
	}
}
