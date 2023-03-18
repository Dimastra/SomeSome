using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Chemistry.Components.SolutionManager;
using Content.Server.Mind;
using Content.Server.Mind.Components;
using Content.Server.Power.Components;
using Content.Shared.Chemistry.Components;
using Content.Shared.Tag;
using Robust.Server.Bql;
using Robust.Shared.GameObjects;

namespace Content.Server.Bql
{
	// Token: 0x020006F4 RID: 1780
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class QuerySelectors
	{
		// Token: 0x02000B05 RID: 2821
		[Nullable(0)]
		[RegisterBqlQuerySelector]
		public sealed class MindfulQuerySelector : BqlQuerySelector
		{
			// Token: 0x17000866 RID: 2150
			// (get) Token: 0x060036B8 RID: 14008 RVA: 0x00121E48 File Offset: 0x00120048
			public override string Token
			{
				get
				{
					return "mindful";
				}
			}

			// Token: 0x17000867 RID: 2151
			// (get) Token: 0x060036B9 RID: 14009 RVA: 0x00121E4F File Offset: 0x0012004F
			public override QuerySelectorArgument[] Arguments
			{
				get
				{
					return Array.Empty<QuerySelectorArgument>();
				}
			}

			// Token: 0x060036BA RID: 14010 RVA: 0x00121E58 File Offset: 0x00120058
			public override IEnumerable<EntityUid> DoSelection(IEnumerable<EntityUid> input, IReadOnlyList<object> arguments, bool isInverted, IEntityManager entityManager)
			{
				return input.Where(delegate(EntityUid e)
				{
					MindComponent mind;
					if (entityManager.TryGetComponent<MindComponent>(e, ref mind))
					{
						Mind mind2 = mind.Mind;
						bool flag;
						if (mind2 == null)
						{
							flag = false;
						}
						else
						{
							EntityUid? visitingEntity = mind2.VisitingEntity;
							flag = (visitingEntity != null && (visitingEntity == null || visitingEntity.GetValueOrDefault() == e));
						}
						return flag ^ isInverted;
					}
					return isInverted;
				});
			}

			// Token: 0x060036BB RID: 14011 RVA: 0x00121E8C File Offset: 0x0012008C
			public override IEnumerable<EntityUid> DoInitialSelection(IReadOnlyList<object> arguments, bool isInverted, IEntityManager entityManager)
			{
				return this.DoSelection(from x in entityManager.EntityQuery<MindComponent>(false)
				select x.Owner, arguments, isInverted, entityManager);
			}
		}

		// Token: 0x02000B06 RID: 2822
		[Nullable(0)]
		[RegisterBqlQuerySelector]
		public sealed class TaggedQuerySelector : BqlQuerySelector
		{
			// Token: 0x17000868 RID: 2152
			// (get) Token: 0x060036BD RID: 14013 RVA: 0x00121ECA File Offset: 0x001200CA
			public override string Token
			{
				get
				{
					return "tagged";
				}
			}

			// Token: 0x17000869 RID: 2153
			// (get) Token: 0x060036BE RID: 14014 RVA: 0x00121ED1 File Offset: 0x001200D1
			public override QuerySelectorArgument[] Arguments
			{
				get
				{
					return new QuerySelectorArgument[]
					{
						4
					};
				}
			}

			// Token: 0x060036BF RID: 14015 RVA: 0x00121EE0 File Offset: 0x001200E0
			public override IEnumerable<EntityUid> DoSelection(IEnumerable<EntityUid> input, IReadOnlyList<object> arguments, bool isInverted, IEntityManager entityManager)
			{
				return input.Where(delegate(EntityUid e)
				{
					TagComponent tag;
					return (entityManager.TryGetComponent<TagComponent>(e, ref tag) && tag.Tags.Contains((string)arguments[0])) ^ isInverted;
				});
			}

			// Token: 0x060036C0 RID: 14016 RVA: 0x00121F1B File Offset: 0x0012011B
			public override IEnumerable<EntityUid> DoInitialSelection(IReadOnlyList<object> arguments, bool isInverted, IEntityManager entityManager)
			{
				return this.DoSelection(from x in entityManager.EntityQuery<TagComponent>(false)
				select x.Owner, arguments, isInverted, entityManager);
			}
		}

		// Token: 0x02000B07 RID: 2823
		[Nullable(0)]
		[RegisterBqlQuerySelector]
		public sealed class AliveQuerySelector : BqlQuerySelector
		{
			// Token: 0x1700086A RID: 2154
			// (get) Token: 0x060036C2 RID: 14018 RVA: 0x00121F59 File Offset: 0x00120159
			public override string Token
			{
				get
				{
					return "alive";
				}
			}

			// Token: 0x1700086B RID: 2155
			// (get) Token: 0x060036C3 RID: 14019 RVA: 0x00121F60 File Offset: 0x00120160
			public override QuerySelectorArgument[] Arguments
			{
				get
				{
					return Array.Empty<QuerySelectorArgument>();
				}
			}

			// Token: 0x060036C4 RID: 14020 RVA: 0x00121F68 File Offset: 0x00120168
			public override IEnumerable<EntityUid> DoSelection(IEnumerable<EntityUid> input, IReadOnlyList<object> arguments, bool isInverted, IEntityManager entityManager)
			{
				return input.Where(delegate(EntityUid e)
				{
					MindComponent mind;
					bool flag;
					if (entityManager.TryGetComponent<MindComponent>(e, ref mind))
					{
						Mind mind2 = mind.Mind;
						flag = (mind2 == null || !mind2.CharacterDeadPhysically);
					}
					else
					{
						flag = false;
					}
					return flag ^ isInverted;
				});
			}

			// Token: 0x060036C5 RID: 14021 RVA: 0x00121F9C File Offset: 0x0012019C
			public override IEnumerable<EntityUid> DoInitialSelection(IReadOnlyList<object> arguments, bool isInverted, IEntityManager entityManager)
			{
				return this.DoSelection(from x in entityManager.EntityQuery<MindComponent>(false)
				select x.Owner, arguments, isInverted, entityManager);
			}
		}

		// Token: 0x02000B08 RID: 2824
		[Nullable(0)]
		[RegisterBqlQuerySelector]
		public sealed class HasReagentQuerySelector : BqlQuerySelector
		{
			// Token: 0x1700086C RID: 2156
			// (get) Token: 0x060036C7 RID: 14023 RVA: 0x00121FDA File Offset: 0x001201DA
			public override string Token
			{
				get
				{
					return "hasreagent";
				}
			}

			// Token: 0x1700086D RID: 2157
			// (get) Token: 0x060036C8 RID: 14024 RVA: 0x00121FE1 File Offset: 0x001201E1
			public override QuerySelectorArgument[] Arguments
			{
				get
				{
					return new QuerySelectorArgument[]
					{
						4
					};
				}
			}

			// Token: 0x060036C9 RID: 14025 RVA: 0x00121FF0 File Offset: 0x001201F0
			public override IEnumerable<EntityUid> DoSelection(IEnumerable<EntityUid> input, IReadOnlyList<object> arguments, bool isInverted, IEntityManager entityManager)
			{
				string reagent = (string)arguments[0];
				Func<KeyValuePair<string, Solution>, bool> <>9__1;
				return input.Where(delegate(EntityUid e)
				{
					SolutionContainerManagerComponent solutionContainerManagerComponent;
					if (entityManager.TryGetComponent<SolutionContainerManagerComponent>(e, ref solutionContainerManagerComponent))
					{
						IEnumerable<KeyValuePair<string, Solution>> solutions = solutionContainerManagerComponent.Solutions;
						Func<KeyValuePair<string, Solution>, bool> predicate;
						if ((predicate = <>9__1) == null)
						{
							predicate = (<>9__1 = ((KeyValuePair<string, Solution> solution) => solution.Value.ContainsReagent(reagent)));
						}
						return solutions.Any(predicate) ^ isInverted;
					}
					return isInverted;
				});
			}

			// Token: 0x060036CA RID: 14026 RVA: 0x00122036 File Offset: 0x00120236
			public override IEnumerable<EntityUid> DoInitialSelection(IReadOnlyList<object> arguments, bool isInverted, IEntityManager entityManager)
			{
				return this.DoSelection(from x in entityManager.EntityQuery<SolutionContainerManagerComponent>(false)
				select x.Owner, arguments, isInverted, entityManager);
			}
		}

		// Token: 0x02000B09 RID: 2825
		[Nullable(0)]
		[RegisterBqlQuerySelector]
		public sealed class ApcPoweredQuerySelector : BqlQuerySelector
		{
			// Token: 0x1700086E RID: 2158
			// (get) Token: 0x060036CC RID: 14028 RVA: 0x00122074 File Offset: 0x00120274
			public override string Token
			{
				get
				{
					return "apcpowered";
				}
			}

			// Token: 0x1700086F RID: 2159
			// (get) Token: 0x060036CD RID: 14029 RVA: 0x0012207B File Offset: 0x0012027B
			public override QuerySelectorArgument[] Arguments
			{
				get
				{
					return Array.Empty<QuerySelectorArgument>();
				}
			}

			// Token: 0x060036CE RID: 14030 RVA: 0x00122084 File Offset: 0x00120284
			public override IEnumerable<EntityUid> DoSelection(IEnumerable<EntityUid> input, IReadOnlyList<object> arguments, bool isInverted, IEntityManager entityManager)
			{
				return input.Where(delegate(EntityUid e)
				{
					ApcPowerReceiverComponent apcPowerReceiver;
					if (!entityManager.TryGetComponent<ApcPowerReceiverComponent>(e, ref apcPowerReceiver))
					{
						return isInverted;
					}
					return apcPowerReceiver.Powered ^ isInverted;
				});
			}

			// Token: 0x060036CF RID: 14031 RVA: 0x001220B8 File Offset: 0x001202B8
			public override IEnumerable<EntityUid> DoInitialSelection(IReadOnlyList<object> arguments, bool isInverted, IEntityManager entityManager)
			{
				return this.DoSelection(from x in entityManager.EntityQuery<ApcPowerReceiverComponent>(false)
				select x.Owner, arguments, isInverted, entityManager);
			}
		}
	}
}
