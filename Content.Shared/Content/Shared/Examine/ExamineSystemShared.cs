using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Eye.Blinding;
using Content.Shared.Interaction;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Physics;
using Content.Shared.Verbs;
using Robust.Shared.ComponentTrees;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Utility;

namespace Content.Shared.Examine
{
	// Token: 0x020004AB RID: 1195
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class ExamineSystemShared : EntitySystem
	{
		// Token: 0x06000E67 RID: 3687
		public abstract void SendExamineTooltip(EntityUid player, EntityUid target, FormattedMessage message, bool getVerbs, bool centerAtCursor);

		// Token: 0x06000E68 RID: 3688 RVA: 0x0002E424 File Offset: 0x0002C624
		public bool IsInDetailsRange(EntityUid examiner, EntityUid entity)
		{
			return entity.IsClientSide() || (!this.MobStateSystem.IsIncapacitated(examiner, null) && this._interactionSystem.InRangeUnobstructed(examiner, entity, 3f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, false) && (this._containerSystem.IsInSameOrTransparentContainer(examiner, entity, null, null, true) || this._interactionSystem.CanAccessViaStorage(examiner, entity)));
		}

		// Token: 0x06000E69 RID: 3689 RVA: 0x0002E48C File Offset: 0x0002C68C
		public bool CanExamine(EntityUid examiner, EntityUid examined)
		{
			return examined.IsClientSide() || (!base.Deleted(examined, null) && this.CanExamine(examiner, this.EntityManager.GetComponent<TransformComponent>(examined).MapPosition, (EntityUid entity) => entity == examiner || entity == examined, new EntityUid?(examined), null));
		}

		// Token: 0x06000E6A RID: 3690 RVA: 0x0002E508 File Offset: 0x0002C708
		[NullableContext(2)]
		public virtual bool CanExamine(EntityUid examiner, MapCoordinates target, SharedInteractionSystem.Ignored predicate = null, EntityUid? examined = null, ExaminerComponent examinerComp = null)
		{
			if (!base.Resolve<ExaminerComponent>(examiner, ref examinerComp, false))
			{
				return false;
			}
			if (examinerComp.SkipChecks)
			{
				return true;
			}
			if (examined != null)
			{
				ExamineAttemptEvent ev = new ExamineAttemptEvent(examiner);
				base.RaiseLocalEvent<ExamineAttemptEvent>(examined.Value, ev, false);
				if (ev.Cancelled)
				{
					return false;
				}
			}
			return !examinerComp.CheckInRangeUnOccluded || (!(this.EntityManager.GetComponent<TransformComponent>(examiner).MapID != target.MapId) && ExamineSystemShared.InRangeUnOccluded(this.EntityManager.GetComponent<TransformComponent>(examiner).MapPosition, target, this.GetExaminerRange(examiner, null), predicate, true, null));
		}

		// Token: 0x06000E6B RID: 3691 RVA: 0x0002E5A4 File Offset: 0x0002C7A4
		[NullableContext(2)]
		public float GetExaminerRange(EntityUid examiner, MobStateComponent mobState = null)
		{
			if (base.Resolve<MobStateComponent>(examiner, ref mobState, false))
			{
				if (this.MobStateSystem.IsDead(examiner, mobState))
				{
					return 0.75f;
				}
				BlindableComponent blind;
				if (this.MobStateSystem.IsCritical(examiner, mobState) || (base.TryComp<BlindableComponent>(examiner, ref blind) && blind.Sources > 0))
				{
					return 1.3f;
				}
				BlurryVisionComponent blurry;
				if (base.TryComp<BlurryVisionComponent>(examiner, ref blurry) && blurry.Magnitude != 0f)
				{
					return Math.Clamp(16f - 2f * (8f - blurry.Magnitude), 2f, 16f);
				}
			}
			return 16f;
		}

		// Token: 0x06000E6C RID: 3692 RVA: 0x0002E640 File Offset: 0x0002C840
		public bool IsOccluded(EntityUid uid)
		{
			SharedEyeComponent eye;
			return base.TryComp<SharedEyeComponent>(uid, ref eye) && eye.DrawFov;
		}

		// Token: 0x06000E6D RID: 3693 RVA: 0x0002E660 File Offset: 0x0002C860
		[NullableContext(2)]
		public static bool InRangeUnOccluded(MapCoordinates origin, MapCoordinates other, float range, SharedInteractionSystem.Ignored predicate, bool ignoreInsideBlocker = true, IEntityManager entMan = null)
		{
			Func<EntityUid, SharedInteractionSystem.Ignored, bool> wrapped2 = (EntityUid uid, SharedInteractionSystem.Ignored wrapped) => wrapped != null && wrapped(uid);
			return ExamineSystemShared.InRangeUnOccluded<SharedInteractionSystem.Ignored>(origin, other, range, predicate, wrapped2, ignoreInsideBlocker, entMan);
		}

		// Token: 0x06000E6E RID: 3694 RVA: 0x0002E69C File Offset: 0x0002C89C
		public static bool InRangeUnOccluded<[Nullable(2)] TState>(MapCoordinates origin, MapCoordinates other, float range, TState state, Func<EntityUid, TState, bool> predicate, bool ignoreInsideBlocker = true, [Nullable(2)] IEntityManager entMan = null)
		{
			if (other.MapId != origin.MapId || other.MapId == MapId.Nullspace)
			{
				return false;
			}
			Vector2 dir = other.Position - origin.Position;
			float length = dir.Length;
			if (range > 0f && length > range + 0.01f)
			{
				return false;
			}
			if (MathHelper.CloseTo(length, 0f, 1E-07f))
			{
				return true;
			}
			if (length > 100f)
			{
				Logger.Warning("InRangeUnOccluded check performed over extreme range. Limiting CollisionRay size.");
				length = 100f;
			}
			ComponentTreeSystem<OccluderTreeComponent, OccluderComponent> componentTreeSystem = EntitySystem.Get<OccluderSystem>();
			IoCManager.Resolve<IEntityManager>(ref entMan);
			Ray ray;
			ray..ctor(origin.Position, dir.Normalized);
			List<RayCastResults> rayResults = componentTreeSystem.IntersectRayWithPredicate<TState>(origin.MapId, ref ray, length, state, predicate, false).ToList<RayCastResults>();
			if (rayResults.Count == 0)
			{
				return true;
			}
			if (!ignoreInsideBlocker)
			{
				return false;
			}
			foreach (RayCastResults result in rayResults)
			{
				OccluderComponent o;
				if (entMan.TryGetComponent<OccluderComponent>(result.HitEntity, ref o))
				{
					Box2 bBox = o.BoundingBox;
					bBox = bBox.Translated(entMan.GetComponent<TransformComponent>(o.Owner).WorldPosition);
					if (!bBox.Contains(origin.Position, true) && !bBox.Contains(other.Position, true))
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06000E6F RID: 3695 RVA: 0x0002E810 File Offset: 0x0002CA10
		[NullableContext(2)]
		public static bool InRangeUnOccluded(EntityUid origin, EntityUid other, float range, SharedInteractionSystem.Ignored predicate, bool ignoreInsideBlocker = true)
		{
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			MapCoordinates originPos = entityManager.GetComponent<TransformComponent>(origin).MapPosition;
			MapCoordinates otherPos = entityManager.GetComponent<TransformComponent>(other).MapPosition;
			return ExamineSystemShared.InRangeUnOccluded(originPos, otherPos, range, predicate, ignoreInsideBlocker, null);
		}

		// Token: 0x06000E70 RID: 3696 RVA: 0x0002E848 File Offset: 0x0002CA48
		public static bool InRangeUnOccluded(EntityUid origin, IComponent other, float range, [Nullable(2)] SharedInteractionSystem.Ignored predicate, bool ignoreInsideBlocker = true)
		{
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			MapCoordinates originPos = entityManager.GetComponent<TransformComponent>(origin).MapPosition;
			MapCoordinates otherPos = entityManager.GetComponent<TransformComponent>(other.Owner).MapPosition;
			return ExamineSystemShared.InRangeUnOccluded(originPos, otherPos, range, predicate, ignoreInsideBlocker, null);
		}

		// Token: 0x06000E71 RID: 3697 RVA: 0x0002E884 File Offset: 0x0002CA84
		[NullableContext(2)]
		public static bool InRangeUnOccluded(EntityUid origin, EntityCoordinates other, float range, SharedInteractionSystem.Ignored predicate, bool ignoreInsideBlocker = true)
		{
			IEntityManager entMan = IoCManager.Resolve<IEntityManager>();
			MapCoordinates mapPosition = entMan.GetComponent<TransformComponent>(origin).MapPosition;
			MapCoordinates otherPos = other.ToMap(entMan);
			return ExamineSystemShared.InRangeUnOccluded(mapPosition, otherPos, range, predicate, ignoreInsideBlocker, null);
		}

		// Token: 0x06000E72 RID: 3698 RVA: 0x0002E8B7 File Offset: 0x0002CAB7
		[NullableContext(2)]
		public static bool InRangeUnOccluded(EntityUid origin, MapCoordinates other, float range, SharedInteractionSystem.Ignored predicate, bool ignoreInsideBlocker = true)
		{
			return ExamineSystemShared.InRangeUnOccluded(IoCManager.Resolve<IEntityManager>().GetComponent<TransformComponent>(origin).MapPosition, other, range, predicate, ignoreInsideBlocker, null);
		}

		// Token: 0x06000E73 RID: 3699 RVA: 0x0002E8D4 File Offset: 0x0002CAD4
		public FormattedMessage GetExamineText(EntityUid entity, EntityUid? examiner)
		{
			FormattedMessage message = new FormattedMessage();
			if (examiner == null)
			{
				return message;
			}
			bool doNewline = false;
			if (!string.IsNullOrEmpty(this.EntityManager.GetComponent<MetaDataComponent>(entity).EntityDescription))
			{
				message.AddText(this.EntityManager.GetComponent<MetaDataComponent>(entity).EntityDescription);
				doNewline = true;
			}
			message.PushColor(Color.DarkGray);
			bool isInDetailsRange = this.IsInDetailsRange(examiner.Value, entity);
			ExaminedEvent examinedEvent = new ExaminedEvent(message, entity, examiner.Value, isInDetailsRange, doNewline);
			base.RaiseLocalEvent<ExaminedEvent>(entity, examinedEvent, true);
			message.Pop();
			return message;
		}

		// Token: 0x06000E74 RID: 3700 RVA: 0x0002E960 File Offset: 0x0002CB60
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<GroupExamineComponent, GetVerbsEvent<ExamineVerb>>(new ComponentEventHandler<GroupExamineComponent, GetVerbsEvent<ExamineVerb>>(this.OnGroupExamineVerb), null, null);
		}

		// Token: 0x06000E75 RID: 3701 RVA: 0x0002E97C File Offset: 0x0002CB7C
		private void OnGroupExamineVerb(EntityUid uid, GroupExamineComponent component, GetVerbsEvent<ExamineVerb> args)
		{
			using (List<ExamineGroup>.Enumerator enumerator = component.ExamineGroups.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ExamineGroup group = enumerator.Current;
					if (this.EntityHasComponent(uid, group.Components))
					{
						ExamineVerb examineVerb = new ExamineVerb
						{
							Act = delegate()
							{
								this.SendExamineGroup(args.User, args.Target, group);
								group.Entries.Clear();
							},
							Text = group.ContextText,
							Message = group.HoverMessage,
							Category = VerbCategory.Examine,
							Icon = new SpriteSpecifier.Texture(new ResourcePath(group.Icon, "/"))
						};
						args.Verbs.Add(examineVerb);
					}
				}
			}
		}

		// Token: 0x06000E76 RID: 3702 RVA: 0x0002EA84 File Offset: 0x0002CC84
		public bool EntityHasComponent(EntityUid uid, List<string> components)
		{
			foreach (string comp in components)
			{
				ComponentRegistration componentRegistration;
				if (this._componentFactory.TryGetRegistration(comp, ref componentRegistration, false) && base.HasComp(uid, componentRegistration.Type))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000E77 RID: 3703 RVA: 0x0002EAF4 File Offset: 0x0002CCF4
		public void SendExamineGroup(EntityUid user, EntityUid target, ExamineGroup group)
		{
			FormattedMessage message = new FormattedMessage();
			if (group.Title != null)
			{
				message.AddMarkup(Loc.GetString(group.Title));
				message.PushNewline();
			}
			message.AddMessage(ExamineSystemShared.GetFormattedMessageFromExamineEntries(group.Entries));
			this.SendExamineTooltip(user, target, message, false, false);
		}

		// Token: 0x06000E78 RID: 3704 RVA: 0x0002EB44 File Offset: 0x0002CD44
		public static FormattedMessage GetFormattedMessageFromExamineEntries(List<ExamineEntry> entries)
		{
			FormattedMessage formattedMessage = new FormattedMessage();
			entries.Sort((ExamineEntry a, ExamineEntry b) => b.Priority.CompareTo(a.Priority));
			bool first = true;
			foreach (ExamineEntry entry in entries)
			{
				if (!first)
				{
					formattedMessage.PushNewline();
				}
				else
				{
					first = false;
				}
				formattedMessage.AddMessage(entry.Message);
			}
			return formattedMessage;
		}

		// Token: 0x06000E79 RID: 3705 RVA: 0x0002EBD4 File Offset: 0x0002CDD4
		public void AddDetailedExamineVerb(GetVerbsEvent<ExamineVerb> verbsEvent, Component component, List<ExamineEntry> entries, string verbText, string iconTexture = "/Textures/Interface/examine-star.png", string hoverMessage = "")
		{
			GroupExamineComponent groupExamine;
			if (base.TryComp<GroupExamineComponent>(verbsEvent.Target, ref groupExamine))
			{
				string componentName = this._componentFactory.GetComponentName(component.GetType());
				foreach (ExamineGroup examineGroup in groupExamine.ExamineGroups)
				{
					if (examineGroup.Components.Contains(componentName))
					{
						using (List<ExamineEntry>.Enumerator enumerator2 = examineGroup.Entries.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								if (enumerator2.Current.ComponentName == componentName)
								{
									return;
								}
							}
						}
						using (List<ExamineEntry>.Enumerator enumerator2 = entries.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								ExamineEntry entry = enumerator2.Current;
								examineGroup.Entries.Add(entry);
							}
							return;
						}
					}
				}
			}
			FormattedMessage formattedMessage = ExamineSystemShared.GetFormattedMessageFromExamineEntries(entries);
			ExamineVerb examineVerb = new ExamineVerb
			{
				Act = delegate()
				{
					this.SendExamineTooltip(verbsEvent.User, verbsEvent.Target, formattedMessage, false, false);
				},
				Text = verbText,
				Message = hoverMessage,
				Category = VerbCategory.Examine,
				Icon = new SpriteSpecifier.Texture(new ResourcePath(iconTexture, "/"))
			};
			verbsEvent.Verbs.Add(examineVerb);
		}

		// Token: 0x06000E7A RID: 3706 RVA: 0x0002ED74 File Offset: 0x0002CF74
		public void AddDetailedExamineVerb(GetVerbsEvent<ExamineVerb> verbsEvent, Component component, ExamineEntry entry, string verbText, string iconTexture = "/Textures/Interface/examine-star.png", string hoverMessage = "")
		{
			this.AddDetailedExamineVerb(verbsEvent, component, new List<ExamineEntry>
			{
				entry
			}, verbText, iconTexture, hoverMessage);
		}

		// Token: 0x06000E7B RID: 3707 RVA: 0x0002ED90 File Offset: 0x0002CF90
		public void AddDetailedExamineVerb(GetVerbsEvent<ExamineVerb> verbsEvent, Component component, FormattedMessage message, string verbText, string iconTexture = "/Textures/Interface/examine-star.png", string hoverMessage = "")
		{
			string componentName = this._componentFactory.GetComponentName(component.GetType());
			this.AddDetailedExamineVerb(verbsEvent, component, new ExamineEntry(componentName, 0f, message), verbText, iconTexture, hoverMessage);
		}

		// Token: 0x04000D9D RID: 3485
		[Dependency]
		private readonly SharedContainerSystem _containerSystem;

		// Token: 0x04000D9E RID: 3486
		[Dependency]
		private readonly SharedInteractionSystem _interactionSystem;

		// Token: 0x04000D9F RID: 3487
		[Dependency]
		protected readonly MobStateSystem MobStateSystem;

		// Token: 0x04000DA0 RID: 3488
		public const float MaxRaycastRange = 100f;

		// Token: 0x04000DA1 RID: 3489
		public const float CritExamineRange = 1.3f;

		// Token: 0x04000DA2 RID: 3490
		public const float DeadExamineRange = 0.75f;

		// Token: 0x04000DA3 RID: 3491
		public const float ExamineRange = 16f;

		// Token: 0x04000DA4 RID: 3492
		protected const float ExamineDetailsRange = 3f;

		// Token: 0x04000DA5 RID: 3493
		[Dependency]
		private readonly IComponentFactory _componentFactory;

		// Token: 0x04000DA6 RID: 3494
		public const string DefaultIconTexture = "/Textures/Interface/examine-star.png";
	}
}
