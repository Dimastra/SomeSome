using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Content.Server.Administration.Managers;
using Content.Server.CPUJob.JobQueues;
using Content.Server.CPUJob.JobQueues.Queues;
using Content.Server.NPC.Components;
using Content.Server.NPC.HTN.Preconditions;
using Content.Server.NPC.HTN.PrimitiveTasks;
using Content.Server.NPC.Systems;
using Content.Shared.Administration;
using Content.Shared.NPC;
using Robust.Server.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Players;
using Robust.Shared.Prototypes;

namespace Content.Server.NPC.HTN
{
	// Token: 0x02000348 RID: 840
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class HTNSystem : EntitySystem
	{
		// Token: 0x1700027C RID: 636
		// (get) Token: 0x06001190 RID: 4496 RVA: 0x0005C810 File Offset: 0x0005AA10
		public IReadOnlyDictionary<HTNCompoundTask, List<HTNTask>[]> CompoundBranches
		{
			get
			{
				return this._compoundBranches;
			}
		}

		// Token: 0x06001191 RID: 4497 RVA: 0x0005C818 File Offset: 0x0005AA18
		public override void Initialize()
		{
			base.Initialize();
			this._sawmill = Logger.GetSawmill("npc.htn");
			base.SubscribeLocalEvent<HTNComponent, ComponentShutdown>(new ComponentEventHandler<HTNComponent, ComponentShutdown>(this.OnHTNShutdown), null, null);
			base.SubscribeNetworkEvent<RequestHTNMessage>(new EntitySessionEventHandler<RequestHTNMessage>(this.OnHTNMessage), null, null);
			this._prototypeManager.PrototypesReloaded += this.OnPrototypeLoad;
			this.OnLoad();
		}

		// Token: 0x06001192 RID: 4498 RVA: 0x0005C880 File Offset: 0x0005AA80
		private void OnHTNMessage(RequestHTNMessage msg, EntitySessionEventArgs args)
		{
			if (!this._admin.HasAdminFlag((IPlayerSession)args.SenderSession, AdminFlags.Debug))
			{
				this._subscribers.Remove(args.SenderSession);
				return;
			}
			if (this._subscribers.Add(args.SenderSession))
			{
				return;
			}
			this._subscribers.Remove(args.SenderSession);
		}

		// Token: 0x06001193 RID: 4499 RVA: 0x0005C8E3 File Offset: 0x0005AAE3
		public override void Shutdown()
		{
			base.Shutdown();
			this._prototypeManager.PrototypesReloaded -= this.OnPrototypeLoad;
		}

		// Token: 0x06001194 RID: 4500 RVA: 0x0005C904 File Offset: 0x0005AB04
		private void OnLoad()
		{
			foreach (HTNComponent comp in base.EntityQuery<HTNComponent>(true))
			{
				CancellationTokenSource planningToken = comp.PlanningToken;
				if (planningToken != null)
				{
					planningToken.Cancel();
				}
				comp.PlanningToken = null;
				if (comp.Plan != null)
				{
					comp.Plan.CurrentOperator.Shutdown(comp.Blackboard, HTNOperatorStatus.Failed);
					comp.Plan = null;
				}
			}
			this._compoundBranches.Clear();
			foreach (HTNCompoundTask compound in this._prototypeManager.EnumeratePrototypes<HTNCompoundTask>())
			{
				this.UpdateCompound(compound);
			}
			foreach (HTNPrimitiveTask primitive in this._prototypeManager.EnumeratePrototypes<HTNPrimitiveTask>())
			{
				this.UpdatePrimitive(primitive);
			}
		}

		// Token: 0x06001195 RID: 4501 RVA: 0x0005CA20 File Offset: 0x0005AC20
		private void OnPrototypeLoad(PrototypesReloadedEventArgs obj)
		{
			this.OnLoad();
		}

		// Token: 0x06001196 RID: 4502 RVA: 0x0005CA28 File Offset: 0x0005AC28
		private void UpdatePrimitive(HTNPrimitiveTask primitive)
		{
			foreach (HTNPrecondition htnprecondition in primitive.Preconditions)
			{
				htnprecondition.Initialize(this.EntityManager.EntitySysManager);
			}
			primitive.Operator.Initialize(this.EntityManager.EntitySysManager);
		}

		// Token: 0x06001197 RID: 4503 RVA: 0x0005CA9C File Offset: 0x0005AC9C
		private void UpdateCompound(HTNCompoundTask compound)
		{
			List<HTNTask>[] branchies = new List<HTNTask>[compound.Branches.Count];
			this._compoundBranches.Add(compound, branchies);
			for (int i = 0; i < compound.Branches.Count; i++)
			{
				HTNBranch branch = compound.Branches[i];
				List<HTNTask> brancho = new List<HTNTask>(branch.TaskPrototypes.Count);
				branchies[i] = brancho;
				foreach (string proto in branch.TaskPrototypes)
				{
					HTNCompoundTask compTask;
					HTNPrimitiveTask primTask;
					if (this._prototypeManager.TryIndex<HTNCompoundTask>(proto, ref compTask))
					{
						brancho.Add(compTask);
					}
					else if (this._prototypeManager.TryIndex<HTNPrimitiveTask>(proto, ref primTask))
					{
						brancho.Add(primTask);
					}
					else
					{
						this._sawmill.Error("Unable to find HTNTask for " + proto + " on " + compound.ID);
					}
				}
				foreach (HTNPrecondition htnprecondition in branch.Preconditions)
				{
					htnprecondition.Initialize(this.EntityManager.EntitySysManager);
				}
			}
		}

		// Token: 0x06001198 RID: 4504 RVA: 0x0005CBEC File Offset: 0x0005ADEC
		private void OnHTNShutdown(EntityUid uid, HTNComponent component, ComponentShutdown args)
		{
			CancellationTokenSource planningToken = component.PlanningToken;
			if (planningToken != null)
			{
				planningToken.Cancel();
			}
			component.PlanningJob = null;
		}

		// Token: 0x06001199 RID: 4505 RVA: 0x0005CC06 File Offset: 0x0005AE06
		public void Replan(HTNComponent component)
		{
			component.PlanAccumulator = 0f;
		}

		// Token: 0x0600119A RID: 4506 RVA: 0x0005CC14 File Offset: 0x0005AE14
		public void UpdateNPC(ref int count, int maxUpdates, float frameTime)
		{
			this._planQueue.Process();
			foreach (ValueTuple<ActiveNPCComponent, HTNComponent> valueTuple in base.EntityQuery<ActiveNPCComponent, HTNComponent>(false))
			{
				HTNComponent comp = valueTuple.Item2;
				if (count >= maxUpdates)
				{
					break;
				}
				if (comp.PlanningJob != null)
				{
					if (comp.PlanningJob.Exception != null)
					{
						ISawmill sawmill = this._sawmill;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(40, 1);
						defaultInterpolatedStringHandler.AppendLiteral("Received exception on planning job for ");
						defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(comp.Owner);
						defaultInterpolatedStringHandler.AppendLiteral("!");
						sawmill.Fatal(defaultInterpolatedStringHandler.ToStringAndClear());
						this._npc.SleepNPC(comp.Owner, null);
						Exception exception = comp.PlanningJob.Exception;
						base.RemComp<HTNComponent>(comp.Owner);
						throw exception;
					}
					if (comp.PlanningJob.Status != JobStatus.Finished)
					{
						continue;
					}
					bool newPlanBetter = false;
					if (comp.Plan != null && comp.PlanningJob.Result != null)
					{
						List<int> oldMtr = comp.Plan.BranchTraversalRecord;
						List<int> mtr = comp.PlanningJob.Result.BranchTraversalRecord;
						for (int i = 0; i < oldMtr.Count; i++)
						{
							if (i < mtr.Count && oldMtr[i] > mtr[i])
							{
								newPlanBetter = true;
								break;
							}
						}
					}
					if (comp.Plan == null || newPlanBetter)
					{
						HTNPlan plan = comp.Plan;
						if (plan != null)
						{
							plan.CurrentTask.Operator.Shutdown(comp.Blackboard, HTNOperatorStatus.BetterPlan);
						}
						comp.Plan = comp.PlanningJob.Result;
						if (comp.Plan != null)
						{
							this.StartupTask(comp.Plan.Tasks[comp.Plan.Index], comp.Blackboard, comp.Plan.Effects[comp.Plan.Index]);
						}
						foreach (ICommonSession session in this._subscribers)
						{
							StringBuilder text = new StringBuilder();
							if (comp.Plan != null)
							{
								StringBuilder stringBuilder = text;
								StringBuilder stringBuilder2 = stringBuilder;
								StringBuilder.AppendInterpolatedStringHandler appendInterpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(5, 1, stringBuilder);
								appendInterpolatedStringHandler.AppendLiteral("BTR: ");
								appendInterpolatedStringHandler.AppendFormatted(string.Join<int>(", ", comp.Plan.BranchTraversalRecord));
								stringBuilder2.AppendLine(ref appendInterpolatedStringHandler);
								text.AppendLine("tasks:");
								foreach (HTNPrimitiveTask task in comp.Plan.Tasks)
								{
									stringBuilder = text;
									StringBuilder stringBuilder3 = stringBuilder;
									appendInterpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(2, 1, stringBuilder);
									appendInterpolatedStringHandler.AppendLiteral("- ");
									appendInterpolatedStringHandler.AppendFormatted(task.ID);
									stringBuilder3.AppendLine(ref appendInterpolatedStringHandler);
								}
							}
							base.RaiseNetworkEvent(new HTNMessage
							{
								Uid = comp.Owner,
								Text = text.ToString()
							}, session.ConnectedClient);
						}
					}
					comp.PlanningJob = null;
					comp.PlanningToken = null;
				}
				this.Update(comp, frameTime);
				count++;
			}
		}

		// Token: 0x0600119B RID: 4507 RVA: 0x0005CF90 File Offset: 0x0005B190
		private void Update(HTNComponent component, float frameTime)
		{
			if (component.PlanningJob == null)
			{
				component.PlanAccumulator -= frameTime;
			}
			if (component.PlanAccumulator <= 0f)
			{
				this.RequestPlan(component);
			}
			if (component.Plan == null)
			{
				return;
			}
			HTNOperatorStatus status = HTNOperatorStatus.Finished;
			while (status != HTNOperatorStatus.Continuing && component.Plan != null)
			{
				HTNOperator currentOperator = component.Plan.CurrentOperator;
				NPCBlackboard blackboard = component.Blackboard;
				status = currentOperator.Update(blackboard, frameTime);
				switch (status)
				{
				case HTNOperatorStatus.Continuing:
					break;
				case HTNOperatorStatus.Failed:
					currentOperator.Shutdown(blackboard, status);
					component.Plan = null;
					break;
				case HTNOperatorStatus.Finished:
					currentOperator.Shutdown(blackboard, status);
					component.Plan.Index++;
					if (component.Plan.Tasks.Count <= component.Plan.Index)
					{
						component.Plan = null;
					}
					else
					{
						this.StartupTask(component.Plan.Tasks[component.Plan.Index], component.Blackboard, component.Plan.Effects[component.Plan.Index]);
					}
					break;
				default:
					throw new InvalidOperationException();
				}
			}
		}

		// Token: 0x0600119C RID: 4508 RVA: 0x0005D0B8 File Offset: 0x0005B2B8
		private void StartupTask(HTNPrimitiveTask primitive, NPCBlackboard blackboard, [Nullable(new byte[]
		{
			2,
			1,
			1
		})] Dictionary<string, object> effects)
		{
			if (effects != null && primitive.ApplyEffectsOnStartup)
			{
				foreach (KeyValuePair<string, object> keyValuePair in effects)
				{
					string text;
					object obj;
					keyValuePair.Deconstruct(out text, out obj);
					string key = text;
					object value = obj;
					blackboard.SetValue(key, value);
				}
			}
			primitive.Operator.Startup(blackboard);
		}

		// Token: 0x0600119D RID: 4509 RVA: 0x0005D130 File Offset: 0x0005B330
		private void RequestPlan(HTNComponent component)
		{
			if (component.PlanningJob != null)
			{
				return;
			}
			component.PlanAccumulator += component.PlanCooldown;
			CancellationTokenSource cancelToken = new CancellationTokenSource();
			HTNPlan plan = component.Plan;
			List<int> branchTraversal = (plan != null) ? plan.BranchTraversalRecord : null;
			HTNPlanJob job = new HTNPlanJob(0.02, this, this._prototypeManager.Index<HTNCompoundTask>(component.RootTask), component.Blackboard.ShallowClone(), branchTraversal, cancelToken.Token);
			this._planQueue.EnqueueJob(job);
			component.PlanningJob = job;
			component.PlanningToken = cancelToken;
		}

		// Token: 0x0600119E RID: 4510 RVA: 0x0005D1C0 File Offset: 0x0005B3C0
		public string GetDomain(HTNCompoundTask compound)
		{
			int indent = 0;
			StringBuilder builder = new StringBuilder();
			this.AppendDomain(builder, compound, ref indent);
			return builder.ToString();
		}

		// Token: 0x0600119F RID: 4511 RVA: 0x0005D1E8 File Offset: 0x0005B3E8
		private void AppendDomain(StringBuilder builder, HTNTask task, ref int indent)
		{
			string buffer = string.Concat(Enumerable.Repeat<string>("    ", indent));
			HTNPrimitiveTask primitive = task as HTNPrimitiveTask;
			if (primitive != null)
			{
				builder.AppendLine(buffer + "Primitive: " + task.ID);
				builder.AppendLine(buffer + "  operator: " + primitive.Operator.GetType().Name);
				return;
			}
			HTNCompoundTask compound = task as HTNCompoundTask;
			if (compound != null)
			{
				builder.AppendLine(buffer + "Compound: " + task.ID);
				List<HTNTask>[] compoundBranches = this.CompoundBranches[compound];
				for (int i = 0; i < compound.Branches.Count; i++)
				{
					HTNBranch htnbranch = compound.Branches[i];
					builder.AppendLine(buffer + "  branch:");
					indent++;
					foreach (HTNTask branchTask in compoundBranches[i])
					{
						this.AppendDomain(builder, branchTask, ref indent);
					}
					indent--;
				}
			}
		}

		// Token: 0x04000A90 RID: 2704
		[Dependency]
		private readonly IAdminManager _admin;

		// Token: 0x04000A91 RID: 2705
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000A92 RID: 2706
		[Dependency]
		private readonly NPCSystem _npc;

		// Token: 0x04000A93 RID: 2707
		private ISawmill _sawmill;

		// Token: 0x04000A94 RID: 2708
		private readonly JobQueue _planQueue = new JobQueue();

		// Token: 0x04000A95 RID: 2709
		private readonly HashSet<ICommonSession> _subscribers = new HashSet<ICommonSession>();

		// Token: 0x04000A96 RID: 2710
		private Dictionary<HTNCompoundTask, List<HTNTask>[]> _compoundBranches = new Dictionary<HTNCompoundTask, List<HTNTask>[]>();
	}
}
