using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Content.Server.CPUJob.JobQueues;
using Content.Server.NPC.HTN.Preconditions;
using Content.Server.NPC.HTN.PrimitiveTasks;

namespace Content.Server.NPC.HTN
{
	// Token: 0x02000347 RID: 839
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class HTNPlanJob : Job<HTNPlan>
	{
		// Token: 0x0600118B RID: 4491 RVA: 0x0005C5EC File Offset: 0x0005A7EC
		public HTNPlanJob(double maxTime, HTNSystem htn, HTNCompoundTask rootTask, NPCBlackboard blackboard, [Nullable(2)] List<int> branchTraversal, CancellationToken cancellationToken = default(CancellationToken)) : base(maxTime, cancellationToken)
		{
			this._htn = htn;
			this._rootTask = rootTask;
			this._blackboard = blackboard;
			this._branchTraversal = branchTraversal;
		}

		// Token: 0x0600118C RID: 4492 RVA: 0x0005C618 File Offset: 0x0005A818
		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		protected override Task<HTNPlan> Process()
		{
			HTNPlanJob.<Process>d__5 <Process>d__;
			<Process>d__.<>t__builder = AsyncTaskMethodBuilder<HTNPlan>.Create();
			<Process>d__.<>4__this = this;
			<Process>d__.<>1__state = -1;
			<Process>d__.<>t__builder.Start<HTNPlanJob.<Process>d__5>(ref <Process>d__);
			return <Process>d__.<>t__builder.Task;
		}

		// Token: 0x0600118D RID: 4493 RVA: 0x0005C65C File Offset: 0x0005A85C
		private Task<bool> PrimitiveConditionMet(HTNPrimitiveTask primitive, NPCBlackboard blackboard, [Nullable(new byte[]
		{
			1,
			2,
			1,
			1
		})] List<Dictionary<string, object>> appliedStates)
		{
			HTNPlanJob.<PrimitiveConditionMet>d__6 <PrimitiveConditionMet>d__;
			<PrimitiveConditionMet>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<PrimitiveConditionMet>d__.<>4__this = this;
			<PrimitiveConditionMet>d__.primitive = primitive;
			<PrimitiveConditionMet>d__.blackboard = blackboard;
			<PrimitiveConditionMet>d__.appliedStates = appliedStates;
			<PrimitiveConditionMet>d__.<>1__state = -1;
			<PrimitiveConditionMet>d__.<>t__builder.Start<HTNPlanJob.<PrimitiveConditionMet>d__6>(ref <PrimitiveConditionMet>d__);
			return <PrimitiveConditionMet>d__.<>t__builder.Task;
		}

		// Token: 0x0600118E RID: 4494 RVA: 0x0005C6B8 File Offset: 0x0005A8B8
		private bool TryFindSatisfiedMethod(HTNCompoundTask compound, Queue<HTNTask> tasksToProcess, NPCBlackboard blackboard, ref int mtrIndex)
		{
			List<HTNTask>[] compBranches = this._htn.CompoundBranches[compound];
			for (int i = mtrIndex; i < compound.Branches.Count; i++)
			{
				HTNBranch htnbranch = compound.Branches[i];
				bool isValid = true;
				using (List<HTNPrecondition>.Enumerator enumerator = htnbranch.Preconditions.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (!enumerator.Current.IsMet(blackboard))
						{
							isValid = false;
							break;
						}
					}
				}
				if (isValid)
				{
					foreach (HTNTask task in compBranches[i])
					{
						tasksToProcess.Enqueue(task);
					}
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600118F RID: 4495 RVA: 0x0005C794 File Offset: 0x0005A994
		private void RestoreTolastDecomposedTask(Stack<HTNPlanJob.DecompositionState> decompHistory, Queue<HTNTask> tasksToProcess, [Nullable(new byte[]
		{
			1,
			2,
			1,
			1
		})] List<Dictionary<string, object>> appliedStates, List<HTNPrimitiveTask> finalPlan, ref int primitiveCount, ref NPCBlackboard blackboard, ref int mtrIndex, ref List<int> btr)
		{
			tasksToProcess.Clear();
			HTNPlanJob.DecompositionState lastDecomp;
			if (!decompHistory.TryPop(out lastDecomp))
			{
				return;
			}
			mtrIndex = lastDecomp.BranchTraversal + 1;
			int count = finalPlan.Count;
			finalPlan.RemoveRange(count - primitiveCount, primitiveCount);
			appliedStates.RemoveRange(count - primitiveCount, primitiveCount);
			btr.RemoveRange(count - primitiveCount, primitiveCount);
			primitiveCount = lastDecomp.PrimitiveCount;
			blackboard = lastDecomp.Blackboard;
			tasksToProcess.Enqueue(lastDecomp.CompoundTask);
		}

		// Token: 0x04000A8C RID: 2700
		private readonly HTNSystem _htn;

		// Token: 0x04000A8D RID: 2701
		private readonly HTNCompoundTask _rootTask;

		// Token: 0x04000A8E RID: 2702
		private NPCBlackboard _blackboard;

		// Token: 0x04000A8F RID: 2703
		[Nullable(2)]
		private List<int> _branchTraversal;

		// Token: 0x02000976 RID: 2422
		[Nullable(0)]
		private sealed class DecompositionState
		{
			// Token: 0x04002089 RID: 8329
			public NPCBlackboard Blackboard;

			// Token: 0x0400208A RID: 8330
			public int PrimitiveCount;

			// Token: 0x0400208B RID: 8331
			public HTNCompoundTask CompoundTask;

			// Token: 0x0400208C RID: 8332
			public int BranchTraversal;
		}
	}
}
