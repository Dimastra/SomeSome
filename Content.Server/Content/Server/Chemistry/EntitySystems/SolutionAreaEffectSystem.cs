using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Chemistry.Components;
using Content.Server.Chemistry.ReactionEffects;
using Content.Shared.Chemistry.Reaction;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.GameObjects;

namespace Content.Server.Chemistry.EntitySystems
{
	// Token: 0x02000699 RID: 1689
	public sealed class SolutionAreaEffectSystem : EntitySystem
	{
		// Token: 0x06002324 RID: 8996 RVA: 0x000B794E File Offset: 0x000B5B4E
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SolutionAreaEffectComponent, ReactionAttemptEvent>(new ComponentEventHandler<SolutionAreaEffectComponent, ReactionAttemptEvent>(this.OnReactionAttempt), null, null);
		}

		// Token: 0x06002325 RID: 8997 RVA: 0x000B796C File Offset: 0x000B5B6C
		public override void Update(float frameTime)
		{
			SolutionAreaEffectInceptionComponent[] array = this.EntityManager.EntityQuery<SolutionAreaEffectInceptionComponent>(false).ToArray<SolutionAreaEffectInceptionComponent>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].InceptionUpdate(frameTime);
			}
		}

		// Token: 0x06002326 RID: 8998 RVA: 0x000B79A4 File Offset: 0x000B5BA4
		[NullableContext(1)]
		private void OnReactionAttempt(EntityUid uid, SolutionAreaEffectComponent component, ReactionAttemptEvent args)
		{
			if (args.Solution.Name != "solutionArea")
			{
				return;
			}
			using (List<ReagentEffect>.Enumerator enumerator = args.Reaction.Effects.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current is AreaReactionEffect)
					{
						args.Cancel();
						break;
					}
				}
			}
		}
	}
}
