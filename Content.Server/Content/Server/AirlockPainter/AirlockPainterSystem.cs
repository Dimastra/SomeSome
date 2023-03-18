using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Content.Server.Administration.Logs;
using Content.Server.DoAfter;
using Content.Server.Popups;
using Content.Server.UserInterface;
using Content.Shared.Administration.Logs;
using Content.Shared.AirlockPainter;
using Content.Shared.AirlockPainter.Prototypes;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Doors.Components;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Player;

namespace Content.Server.AirlockPainter
{
	// Token: 0x020007F1 RID: 2033
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AirlockPainterSystem : SharedAirlockPainterSystem
	{
		// Token: 0x06002BFB RID: 11259 RVA: 0x000E6588 File Offset: 0x000E4788
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<AirlockPainterComponent, AfterInteractEvent>(new ComponentEventHandler<AirlockPainterComponent, AfterInteractEvent>(this.AfterInteractOn), null, null);
			base.SubscribeLocalEvent<AirlockPainterComponent, ActivateInWorldEvent>(new ComponentEventHandler<AirlockPainterComponent, ActivateInWorldEvent>(this.OnActivate), null, null);
			base.SubscribeLocalEvent<AirlockPainterComponent, AirlockPainterSpritePickedMessage>(new ComponentEventHandler<AirlockPainterComponent, AirlockPainterSpritePickedMessage>(this.OnSpritePicked), null, null);
			base.SubscribeLocalEvent<AirlockPainterComponent, DoAfterEvent<AirlockPainterSystem.AirlockPainterData>>(new ComponentEventHandler<AirlockPainterComponent, DoAfterEvent<AirlockPainterSystem.AirlockPainterData>>(this.OnDoAfter), null, null);
		}

		// Token: 0x06002BFC RID: 11260 RVA: 0x000E65EC File Offset: 0x000E47EC
		private void OnDoAfter(EntityUid uid, AirlockPainterComponent component, DoAfterEvent<AirlockPainterSystem.AirlockPainterData> args)
		{
			if (args.Handled || args.Cancelled)
			{
				component.IsSpraying = false;
				return;
			}
			if (args.Args.Target != null)
			{
				this._audio.Play(component.SpraySound, Filter.Pvs(uid, 2f, this.EntityManager, null, null), uid, true, null);
				this._appearance.SetData(args.Args.Target.Value, DoorVisuals.BaseRSI, args.AdditionalData.Sprite, null);
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Action;
				LogImpact impact = LogImpact.Low;
				LogStringHandler logStringHandler = new LogStringHandler(9, 2);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.Args.User), "user", "ToPrettyString(args.Args.User)");
				logStringHandler.AppendLiteral(" painted ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.Args.Target.Value), "target", "ToPrettyString(args.Args.Target.Value)");
				adminLogger.Add(type, impact, ref logStringHandler);
				component.IsSpraying = false;
			}
			args.Handled = true;
		}

		// Token: 0x06002BFD RID: 11261 RVA: 0x000E6700 File Offset: 0x000E4900
		private void OnActivate(EntityUid uid, AirlockPainterComponent component, ActivateInWorldEvent args)
		{
			ActorComponent actor;
			if (!this.EntityManager.TryGetComponent<ActorComponent>(args.User, ref actor))
			{
				return;
			}
			this.DirtyUI(uid, component);
			BoundUserInterface uiorNull = component.Owner.GetUIOrNull(AirlockPainterUiKey.Key);
			if (uiorNull != null)
			{
				uiorNull.Open(actor.PlayerSession);
			}
			args.Handled = true;
		}

		// Token: 0x06002BFE RID: 11262 RVA: 0x000E6758 File Offset: 0x000E4958
		private void AfterInteractOn(EntityUid uid, AirlockPainterComponent component, AfterInteractEvent args)
		{
			if (!component.IsSpraying)
			{
				EntityUid? target2 = args.Target;
				if (target2 != null)
				{
					EntityUid target = target2.GetValueOrDefault();
					if (target.Valid && args.CanReach)
					{
						PaintableAirlockComponent airlock;
						if (!this.EntityManager.TryGetComponent<PaintableAirlockComponent>(target, ref airlock))
						{
							return;
						}
						AirlockGroupPrototype grp;
						if (!this._prototypeManager.TryIndex<AirlockGroupPrototype>(airlock.Group, ref grp))
						{
							Logger.Error("Group not defined: %s", new object[]
							{
								airlock.Group
							});
							return;
						}
						string style = base.Styles[component.Index];
						string sprite;
						if (!grp.StylePaths.TryGetValue(style, out sprite))
						{
							string msg = Loc.GetString("airlock-painter-style-not-available");
							this._popupSystem.PopupEntity(msg, args.User, args.User, PopupType.Small);
							return;
						}
						component.IsSpraying = true;
						AirlockPainterSystem.AirlockPainterData airlockPainterData = new AirlockPainterSystem.AirlockPainterData(sprite);
						EntityUid user = args.User;
						float sprayTime = component.SprayTime;
						target2 = new EntityUid?(target);
						EntityUid? used = new EntityUid?(uid);
						DoAfterEventArgs doAfterEventArgs = new DoAfterEventArgs(user, sprayTime, default(CancellationToken), target2, used)
						{
							BreakOnTargetMove = true,
							BreakOnUserMove = true,
							BreakOnDamage = true,
							BreakOnStun = true,
							NeedHand = true
						};
						this._doAfterSystem.DoAfter<AirlockPainterSystem.AirlockPainterData>(doAfterEventArgs, airlockPainterData);
						ISharedAdminLogManager adminLogger = this._adminLogger;
						LogType type = LogType.Action;
						LogImpact impact = LogImpact.Low;
						LogStringHandler logStringHandler = new LogStringHandler(23, 4);
						logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.User), "user", "ToPrettyString(args.User)");
						logStringHandler.AppendLiteral(" is painting ");
						logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "target", "ToPrettyString(uid)");
						logStringHandler.AppendLiteral(" to '");
						logStringHandler.AppendFormatted(style);
						logStringHandler.AppendLiteral("' at ");
						logStringHandler.AppendFormatted<EntityCoordinates>(base.Transform(uid).Coordinates, "targetlocation", "Transform(uid).Coordinates");
						adminLogger.Add(type, impact, ref logStringHandler);
						return;
					}
				}
			}
		}

		// Token: 0x06002BFF RID: 11263 RVA: 0x000E692C File Offset: 0x000E4B2C
		private void OnSpritePicked(EntityUid uid, AirlockPainterComponent component, AirlockPainterSpritePickedMessage args)
		{
			component.Index = args.Index;
			this.DirtyUI(uid, component);
		}

		// Token: 0x06002C00 RID: 11264 RVA: 0x000E6942 File Offset: 0x000E4B42
		[NullableContext(2)]
		private void DirtyUI(EntityUid uid, AirlockPainterComponent component = null)
		{
			if (!base.Resolve<AirlockPainterComponent>(uid, ref component, true))
			{
				return;
			}
			this._userInterfaceSystem.TrySetUiState(uid, AirlockPainterUiKey.Key, new AirlockPainterBoundUserInterfaceState(component.Index), null, null, true);
		}

		// Token: 0x04001B3C RID: 6972
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x04001B3D RID: 6973
		[Dependency]
		private readonly UserInterfaceSystem _userInterfaceSystem;

		// Token: 0x04001B3E RID: 6974
		[Dependency]
		private readonly DoAfterSystem _doAfterSystem;

		// Token: 0x04001B3F RID: 6975
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x04001B40 RID: 6976
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x04001B41 RID: 6977
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x02000B3B RID: 2875
		[NullableContext(0)]
		private struct AirlockPainterData : IEquatable<AirlockPainterSystem.AirlockPainterData>
		{
			// Token: 0x060038D5 RID: 14549 RVA: 0x0012786D File Offset: 0x00125A6D
			[NullableContext(1)]
			public AirlockPainterData(string Sprite)
			{
				this.Sprite = Sprite;
			}

			// Token: 0x060038D6 RID: 14550 RVA: 0x00127878 File Offset: 0x00125A78
			[CompilerGenerated]
			public override readonly string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("AirlockPainterData");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			// Token: 0x060038D7 RID: 14551 RVA: 0x001278C4 File Offset: 0x00125AC4
			[CompilerGenerated]
			private readonly bool PrintMembers(StringBuilder builder)
			{
				builder.Append("Sprite = ");
				builder.Append(this.Sprite);
				return true;
			}

			// Token: 0x060038D8 RID: 14552 RVA: 0x001278E0 File Offset: 0x00125AE0
			[CompilerGenerated]
			public static bool operator !=(AirlockPainterSystem.AirlockPainterData left, AirlockPainterSystem.AirlockPainterData right)
			{
				return !(left == right);
			}

			// Token: 0x060038D9 RID: 14553 RVA: 0x001278EC File Offset: 0x00125AEC
			[CompilerGenerated]
			public static bool operator ==(AirlockPainterSystem.AirlockPainterData left, AirlockPainterSystem.AirlockPainterData right)
			{
				return left.Equals(right);
			}

			// Token: 0x060038DA RID: 14554 RVA: 0x001278F6 File Offset: 0x00125AF6
			[CompilerGenerated]
			public override readonly int GetHashCode()
			{
				return EqualityComparer<string>.Default.GetHashCode(this.Sprite);
			}

			// Token: 0x060038DB RID: 14555 RVA: 0x00127908 File Offset: 0x00125B08
			[CompilerGenerated]
			public override readonly bool Equals(object obj)
			{
				return obj is AirlockPainterSystem.AirlockPainterData && this.Equals((AirlockPainterSystem.AirlockPainterData)obj);
			}

			// Token: 0x060038DC RID: 14556 RVA: 0x00127920 File Offset: 0x00125B20
			[CompilerGenerated]
			public readonly bool Equals(AirlockPainterSystem.AirlockPainterData other)
			{
				return EqualityComparer<string>.Default.Equals(this.Sprite, other.Sprite);
			}

			// Token: 0x060038DD RID: 14557 RVA: 0x00127938 File Offset: 0x00125B38
			[NullableContext(1)]
			[CompilerGenerated]
			public readonly void Deconstruct(out string Sprite)
			{
				Sprite = this.Sprite;
			}

			// Token: 0x0400299A RID: 10650
			[Nullable(1)]
			public string Sprite;
		}
	}
}
