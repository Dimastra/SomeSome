using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Actions;
using Content.Server.Administration;
using Content.Server.Cargo.Systems;
using Content.Server.GameTicking;
using Content.Server.Popups;
using Content.Server.Power.EntitySystems;
using Content.Server.Xenoarchaeology.Equipment.Components;
using Content.Server.Xenoarchaeology.XenoArtifacts.Events;
using Content.Server.Xenoarchaeology.XenoArtifacts.Triggers.Components;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.Administration;
using Content.Shared.Popups;
using Content.Shared.Whitelist;
using Content.Shared.Xenoarchaeology.XenoArtifacts;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Timing;

namespace Content.Server.Xenoarchaeology.XenoArtifacts
{
	// Token: 0x0200001F RID: 31
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ArtifactSystem : EntitySystem
	{
		// Token: 0x0600005D RID: 93 RVA: 0x00003B24 File Offset: 0x00001D24
		public void InitializeActions()
		{
			base.SubscribeLocalEvent<ArtifactComponent, ComponentStartup>(new ComponentEventHandler<ArtifactComponent, ComponentStartup>(this.OnStartup), null, null);
			base.SubscribeLocalEvent<ArtifactComponent, ComponentRemove>(new ComponentEventHandler<ArtifactComponent, ComponentRemove>(this.OnRemove), null, null);
			base.SubscribeLocalEvent<ArtifactComponent, ArtifactSelfActivateEvent>(new ComponentEventHandler<ArtifactComponent, ArtifactSelfActivateEvent>(this.OnSelfActivate), null, null);
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00003B64 File Offset: 0x00001D64
		private void OnStartup(EntityUid uid, ArtifactComponent component, ComponentStartup args)
		{
			InstantActionPrototype proto;
			if (this._prototype.TryIndex<InstantActionPrototype>("ArtifactActivate", ref proto))
			{
				this._actions.AddAction(uid, new InstantAction(proto), null, null, true);
			}
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00003BA4 File Offset: 0x00001DA4
		private void OnRemove(EntityUid uid, ArtifactComponent component, ComponentRemove args)
		{
			InstantActionPrototype proto;
			if (this._prototype.TryIndex<InstantActionPrototype>("ArtifactActivate", ref proto))
			{
				this._actions.RemoveAction(uid, new InstantAction(proto), null);
			}
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00003BD8 File Offset: 0x00001DD8
		private void OnSelfActivate(EntityUid uid, ArtifactComponent component, ArtifactSelfActivateEvent args)
		{
			if (component.CurrentNode == null)
			{
				return;
			}
			int curNode = component.CurrentNode.Id;
			this._popup.PopupEntity(Loc.GetString("activate-artifact-popup-self", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("node", curNode)
			}), uid, uid, PopupType.Small);
			this.TryActivateArtifact(uid, new EntityUid?(uid), component);
			args.Handled = true;
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00003C48 File Offset: 0x00001E48
		public void InitializeCommands()
		{
			this._conHost.RegisterCommand("forceartifactnode", "Forces an artifact to traverse to a given node", "forceartifacteffect <uid> <node ID>", new ConCommandCallback(this.ForceArtifactNode), new ConCommandCompletionCallback(this.ForceArtifactNodeCompletions), false);
			this._conHost.RegisterCommand("getartifactmaxvalue", "Reports the maximum research point value for a given artifact", "forceartifacteffect <uid>", new ConCommandCallback(this.GetArtifactMaxValue), false);
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00003CB0 File Offset: 0x00001EB0
		[AdminCommand(AdminFlags.Fun)]
		private void ForceArtifactNode(IConsoleShell shell, string argstr, string[] args)
		{
			if (args.Length != 2)
			{
				shell.WriteError("Argument length must be 2");
			}
			int id;
			EntityUid uid;
			if (!EntityUid.TryParse(args[0], ref uid) || !int.TryParse(args[1], out id))
			{
				return;
			}
			ArtifactComponent artifact;
			if (!base.TryComp<ArtifactComponent>(uid, ref artifact) || artifact.NodeTree == null)
			{
				return;
			}
			ArtifactNode node = artifact.NodeTree.AllNodes.FirstOrDefault((ArtifactNode n) => n.Id == id);
			if (node != null)
			{
				this.EnterNode(uid, ref node, null);
			}
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00003D34 File Offset: 0x00001F34
		private CompletionResult ForceArtifactNodeCompletions(IConsoleShell shell, string[] args)
		{
			EntityUid uid;
			ArtifactComponent artifact;
			if (args.Length == 2 && EntityUid.TryParse(args[0], ref uid) && base.TryComp<ArtifactComponent>(uid, ref artifact) && artifact.NodeTree != null)
			{
				return CompletionResult.FromHintOptions(from s in artifact.NodeTree.AllNodes
				select s.Id.ToString(), "<node id>");
			}
			return CompletionResult.Empty;
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00003DAC File Offset: 0x00001FAC
		[AdminCommand(AdminFlags.Debug)]
		private void GetArtifactMaxValue(IConsoleShell shell, string argstr, string[] args)
		{
			if (args.Length != 1)
			{
				shell.WriteError("Argument length must be 1");
			}
			EntityUid uid;
			if (!EntityUid.TryParse(args[0], ref uid))
			{
				return;
			}
			ArtifactComponent artifact;
			if (!base.TryComp<ArtifactComponent>(uid, ref artifact) || artifact.NodeTree == null)
			{
				return;
			}
			int pointSum = this.GetResearchPointValue(uid, artifact, true);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 3);
			defaultInterpolatedStringHandler.AppendLiteral("Max point value for ");
			defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid));
			defaultInterpolatedStringHandler.AppendLiteral(" with ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(artifact.NodeTree.AllNodes.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" nodes: ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(pointSum);
			shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00003E64 File Offset: 0x00002064
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ArtifactComponent, MapInitEvent>(new ComponentEventHandler<ArtifactComponent, MapInitEvent>(this.OnInit), null, null);
			base.SubscribeLocalEvent<ArtifactComponent, PriceCalculationEvent>(new ComponentEventRefHandler<ArtifactComponent, PriceCalculationEvent>(this.GetPrice), null, null);
			base.SubscribeLocalEvent<RoundEndTextAppendEvent>(new EntityEventHandler<RoundEndTextAppendEvent>(this.OnRoundEnd), null, null);
			this.InitializeCommands();
			this.InitializeActions();
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00003EBF File Offset: 0x000020BF
		private void OnInit(EntityUid uid, ArtifactComponent component, MapInitEvent args)
		{
			this.RandomizeArtifact(uid, component);
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00003ECC File Offset: 0x000020CC
		private void GetPrice(EntityUid uid, ArtifactComponent component, ref PriceCalculationEvent args)
		{
			if (component.NodeTree == null)
			{
				return;
			}
			float price = component.NodeTree.AllNodes.Sum((ArtifactNode x) => this.GetNodePrice(x, component));
			float fullyExploredBonus = component.NodeTree.AllNodes.Any((ArtifactNode x) => !x.Triggered) ? 1f : 1.25f;
			args.Price = (double)(price * fullyExploredBonus);
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00003F6C File Offset: 0x0000216C
		private float GetNodePrice(ArtifactNode node, ArtifactComponent component)
		{
			if (!node.Discovered)
			{
				return 0f;
			}
			float priceMultiplier = node.Triggered ? 1f : 0.25f;
			int nodeDanger = (node.Depth + node.Effect.TargetDepth + node.Trigger.TargetDepth) / 3;
			return MathF.Pow(2f, (float)nodeDanger) * (float)component.PricePerNode * priceMultiplier;
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00003FD4 File Offset: 0x000021D4
		[NullableContext(2)]
		public int GetResearchPointValue(EntityUid uid, ArtifactComponent component = null, bool getMaxPrice = false)
		{
			if (!base.Resolve<ArtifactComponent>(uid, ref component, true) || component.NodeTree == null)
			{
				return 0;
			}
			float num = component.NodeTree.AllNodes.Sum((ArtifactNode n) => this.GetNodePointValue(n, component, getMaxPrice));
			float fullyExploredBonus = (component.NodeTree.AllNodes.All((ArtifactNode x) => x.Triggered) | getMaxPrice) ? 1.25f : 1f;
			return (int)(num * fullyExploredBonus);
		}

		// Token: 0x0600006A RID: 106 RVA: 0x0000408C File Offset: 0x0000228C
		private float GetNodePointValue(ArtifactNode node, ArtifactComponent component, bool getMaxPrice = false)
		{
			float valueDeduction = 1f;
			if (!getMaxPrice)
			{
				if (!node.Discovered)
				{
					return 0f;
				}
				valueDeduction = ((!node.Triggered) ? 0.25f : 1f);
			}
			int nodeDanger = (node.Depth + node.Effect.TargetDepth + node.Trigger.TargetDepth) / 3;
			return (float)component.PointsPerNode * MathF.Pow(component.PointDangerMultiplier, (float)nodeDanger) * valueDeduction;
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00004100 File Offset: 0x00002300
		public void RandomizeArtifact(EntityUid uid, ArtifactComponent component)
		{
			int nodeAmount = this._random.Next(component.NodesMin, component.NodesMax);
			component.NodeTree = new ArtifactTree();
			this.GenerateArtifactNodeTree(uid, ref component.NodeTree, nodeAmount);
			this.EnterNode(uid, ref component.NodeTree.StartNode, component);
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00004154 File Offset: 0x00002354
		[NullableContext(2)]
		public bool TryActivateArtifact(EntityUid uid, EntityUid? user = null, ArtifactComponent component = null)
		{
			if (!base.Resolve<ArtifactComponent>(uid, ref component, true))
			{
				return false;
			}
			if (component.IsSuppressed)
			{
				return false;
			}
			if (this._gameTiming.CurTime - component.LastActivationTime < component.CooldownTime)
			{
				return false;
			}
			this.ForceActivateArtifact(uid, user, component);
			return true;
		}

		// Token: 0x0600006D RID: 109 RVA: 0x000041A8 File Offset: 0x000023A8
		[NullableContext(2)]
		public void ForceActivateArtifact(EntityUid uid, EntityUid? user = null, ArtifactComponent component = null)
		{
			if (!base.Resolve<ArtifactComponent>(uid, ref component, true))
			{
				return;
			}
			if (component.CurrentNode == null)
			{
				return;
			}
			component.LastActivationTime = this._gameTiming.CurTime;
			ArtifactActivatedEvent ev = new ArtifactActivatedEvent
			{
				Activator = user
			};
			base.RaiseLocalEvent<ArtifactActivatedEvent>(uid, ev, true);
			component.CurrentNode.Triggered = true;
			if (component.CurrentNode.Edges.Any<ArtifactNode>())
			{
				ArtifactNode newNode = this.GetNewNode(uid, component);
				if (newNode == null)
				{
					return;
				}
				this.EnterNode(uid, ref newNode, component);
			}
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00004228 File Offset: 0x00002428
		[return: Nullable(2)]
		private ArtifactNode GetNewNode(EntityUid uid, ArtifactComponent component)
		{
			if (component.CurrentNode == null)
			{
				return null;
			}
			List<ArtifactNode> allNodes = component.CurrentNode.Edges;
			BiasedArtifactComponent bias;
			TraversalDistorterComponent trav;
			if (base.TryComp<BiasedArtifactComponent>(uid, ref bias) && base.TryComp<TraversalDistorterComponent>(bias.Provider, ref trav) && RandomExtensions.Prob(this._random, trav.BiasChance) && this.IsPowered(bias.Provider, this.EntityManager, null))
			{
				BiasDirection biasDirection = trav.BiasDirection;
				if (biasDirection != BiasDirection.In)
				{
					if (biasDirection == BiasDirection.Out)
					{
						List<ArtifactNode> bar = (from x in allNodes
						where x.Depth > component.CurrentNode.Depth
						select x).ToList<ArtifactNode>();
						if (bar.Any<ArtifactNode>())
						{
							allNodes = bar;
						}
					}
				}
				else
				{
					List<ArtifactNode> foo = (from x in allNodes
					where x.Depth < component.CurrentNode.Depth
					select x).ToList<ArtifactNode>();
					if (foo.Any<ArtifactNode>())
					{
						allNodes = foo;
					}
				}
			}
			List<ArtifactNode> undiscoveredNodes = (from x in allNodes
			where !x.Discovered
			select x).ToList<ArtifactNode>();
			ArtifactNode newNode = RandomExtensions.Pick<ArtifactNode>(this._random, allNodes);
			if (undiscoveredNodes.Any<ArtifactNode>() && RandomExtensions.Prob(this._random, 0.75f))
			{
				newNode = RandomExtensions.Pick<ArtifactNode>(this._random, undiscoveredNodes);
			}
			return newNode;
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00004370 File Offset: 0x00002570
		[NullableContext(2)]
		public bool TryGetNodeData<T>(EntityUid uid, [Nullable(1)] string key, [NotNullWhen(true)] out T data, ArtifactComponent component = null)
		{
			data = default(T);
			if (!base.Resolve<ArtifactComponent>(uid, ref component, true))
			{
				return false;
			}
			if (component.CurrentNode == null)
			{
				return false;
			}
			object dat;
			if (component.CurrentNode.NodeData.TryGetValue(key, out dat) && dat is T)
			{
				T value = (T)((object)dat);
				data = value;
				return true;
			}
			return false;
		}

		// Token: 0x06000070 RID: 112 RVA: 0x000043CC File Offset: 0x000025CC
		public void SetNodeData(EntityUid uid, string key, object value, [Nullable(2)] ArtifactComponent component = null)
		{
			if (!base.Resolve<ArtifactComponent>(uid, ref component, true))
			{
				return;
			}
			if (component.CurrentNode == null)
			{
				return;
			}
			component.CurrentNode.NodeData[key] = value;
		}

		// Token: 0x06000071 RID: 113 RVA: 0x000043F8 File Offset: 0x000025F8
		private void OnRoundEnd(RoundEndTextAppendEvent ev)
		{
			foreach (ArtifactComponent artifactComp in base.EntityQuery<ArtifactComponent>(false))
			{
				artifactComp.CooldownTime = TimeSpan.Zero;
				base.EnsureComp<ArtifactTimerTriggerComponent>(artifactComp.Owner).ActivationRate = TimeSpan.FromSeconds(0.5);
			}
		}

		// Token: 0x06000072 RID: 114 RVA: 0x0000446C File Offset: 0x0000266C
		private void GenerateArtifactNodeTree(EntityUid artifact, ref ArtifactTree tree, int nodeAmount)
		{
			if (nodeAmount < 1)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(62, 1);
				defaultInterpolatedStringHandler.AppendLiteral("nodeAmount ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(nodeAmount);
				defaultInterpolatedStringHandler.AppendLiteral(" is less than 1. Aborting artifact tree generation.");
				Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			List<ArtifactNode> uninitializedNodes = new List<ArtifactNode>
			{
				new ArtifactNode()
			};
			tree.StartNode = uninitializedNodes.First<ArtifactNode>();
			while (uninitializedNodes.Any<ArtifactNode>())
			{
				this.GenerateNode(artifact, ref uninitializedNodes, ref tree, nodeAmount);
			}
		}

		// Token: 0x06000073 RID: 115 RVA: 0x000044E8 File Offset: 0x000026E8
		private void GenerateNode(EntityUid artifact, ref List<ArtifactNode> uninitializedNodes, ref ArtifactTree tree, int targetNodeAmount)
		{
			if (!uninitializedNodes.Any<ArtifactNode>())
			{
				return;
			}
			ArtifactNode node = uninitializedNodes.First<ArtifactNode>();
			uninitializedNodes.Remove(node);
			node.Id = this._random.Next(10000, 100000);
			int maxEdges = Math.Max(1, targetNodeAmount - tree.AllNodes.Count - uninitializedNodes.Count - 1);
			maxEdges = Math.Min(maxEdges, 4);
			int minEdges = Math.Clamp(targetNodeAmount - tree.AllNodes.Count - uninitializedNodes.Count - 1, 0, 1);
			int edgeAmount = this._random.Next(minEdges, maxEdges);
			for (int i = 0; i < edgeAmount; i++)
			{
				ArtifactNode neighbor = new ArtifactNode
				{
					Depth = node.Depth + 1
				};
				node.Edges.Add(neighbor);
				neighbor.Edges.Add(node);
				uninitializedNodes.Add(neighbor);
			}
			node.Trigger = this.GetRandomTrigger(artifact, ref node);
			node.Effect = this.GetRandomEffect(artifact, ref node);
			tree.AllNodes.Add(node);
		}

		// Token: 0x06000074 RID: 116 RVA: 0x000045F8 File Offset: 0x000027F8
		private ArtifactTriggerPrototype GetRandomTrigger(EntityUid artifact, ref ArtifactNode node)
		{
			List<ArtifactTriggerPrototype> source = this._prototype.EnumeratePrototypes<ArtifactTriggerPrototype>().Where(delegate(ArtifactTriggerPrototype x)
			{
				EntityWhitelist whitelist = x.Whitelist;
				if (whitelist == null || whitelist.IsValid(artifact, this.EntityManager))
				{
					EntityWhitelist blacklist = x.Blacklist;
					return blacklist == null || !blacklist.IsValid(artifact, this.EntityManager);
				}
				return false;
			}).ToList<ArtifactTriggerPrototype>();
			List<int> validDepth = (from x in source
			select x.TargetDepth).Distinct<int>().ToList<int>();
			Dictionary<int, float> weights = this.GetDepthWeights(validDepth, node.Depth);
			int selectedRandomTargetDepth = this.GetRandomTargetDepth(weights);
			List<ArtifactTriggerPrototype> targetTriggers = (from x in source
			where x.TargetDepth == selectedRandomTargetDepth
			select x).ToList<ArtifactTriggerPrototype>();
			return RandomExtensions.Pick<ArtifactTriggerPrototype>(this._random, targetTriggers);
		}

		// Token: 0x06000075 RID: 117 RVA: 0x000046AC File Offset: 0x000028AC
		private ArtifactEffectPrototype GetRandomEffect(EntityUid artifact, ref ArtifactNode node)
		{
			List<ArtifactEffectPrototype> source = this._prototype.EnumeratePrototypes<ArtifactEffectPrototype>().Where(delegate(ArtifactEffectPrototype x)
			{
				EntityWhitelist whitelist = x.Whitelist;
				if (whitelist == null || whitelist.IsValid(artifact, this.EntityManager))
				{
					EntityWhitelist blacklist = x.Blacklist;
					return blacklist == null || !blacklist.IsValid(artifact, this.EntityManager);
				}
				return false;
			}).ToList<ArtifactEffectPrototype>();
			List<int> validDepth = (from x in source
			select x.TargetDepth).Distinct<int>().ToList<int>();
			Dictionary<int, float> weights = this.GetDepthWeights(validDepth, node.Depth);
			int selectedRandomTargetDepth = this.GetRandomTargetDepth(weights);
			List<ArtifactEffectPrototype> targetEffects = (from x in source
			where x.TargetDepth == selectedRandomTargetDepth
			select x).ToList<ArtifactEffectPrototype>();
			return RandomExtensions.Pick<ArtifactEffectPrototype>(this._random, targetEffects);
		}

		// Token: 0x06000076 RID: 118 RVA: 0x00004760 File Offset: 0x00002960
		private Dictionary<int, float> GetDepthWeights(IEnumerable<int> depths, int targetDepth)
		{
			Dictionary<int, float> weights = new Dictionary<int, float>();
			foreach (int d in depths)
			{
				float w = 10f / (0.75f * MathF.Sqrt(6.2831855f)) * MathF.Pow(2.7182817f, -MathF.Pow((float)(d - targetDepth) / 0.75f, 2f));
				weights.Add(d, w);
			}
			return weights;
		}

		// Token: 0x06000077 RID: 119 RVA: 0x000047E8 File Offset: 0x000029E8
		private int GetRandomTargetDepth(Dictionary<int, float> weights)
		{
			float sum = weights.Values.Sum();
			float accumulated = 0f;
			float rand = this._random.NextFloat() * sum;
			foreach (KeyValuePair<int, float> keyValuePair in weights)
			{
				int num;
				float num2;
				keyValuePair.Deconstruct(out num, out num2);
				int key = num;
				float weight = num2;
				accumulated += weight;
				if (accumulated >= rand)
				{
					return key;
				}
			}
			return RandomExtensions.Pick<int>(this._random, weights.Keys);
		}

		// Token: 0x06000078 RID: 120 RVA: 0x00004888 File Offset: 0x00002A88
		private void EnterNode(EntityUid uid, ref ArtifactNode node, [Nullable(2)] ArtifactComponent component = null)
		{
			if (!base.Resolve<ArtifactComponent>(uid, ref component, true))
			{
				return;
			}
			if (component.CurrentNode != null)
			{
				this.ExitNode(uid, component);
			}
			component.CurrentNode = node;
			foreach (KeyValuePair<string, EntityPrototype.ComponentRegistryEntry> keyValuePair in node.Effect.Components.Concat(node.Effect.PermanentComponents).Concat(node.Trigger.Components))
			{
				string text;
				EntityPrototype.ComponentRegistryEntry componentRegistryEntry;
				keyValuePair.Deconstruct(out text, out componentRegistryEntry);
				string name = text;
				EntityPrototype.ComponentRegistryEntry entry = componentRegistryEntry;
				ComponentRegistration reg = this._componentFactory.GetRegistration(name, false);
				if (node.Discovered && this.EntityManager.HasComponent(uid, reg.Type))
				{
					if (node.Effect.PermanentComponents.ContainsKey(name))
					{
						continue;
					}
					this.EntityManager.RemoveComponent(uid, reg.Type);
				}
				Component component2 = (Component)this._componentFactory.GetComponent(reg);
				component2.Owner = uid;
				object temp = component2;
				this._serialization.CopyTo(entry.Component, ref temp, null, false, false);
				this.EntityManager.AddComponent<Component>(uid, (Component)temp, true);
			}
			node.Discovered = true;
			base.RaiseLocalEvent<ArtifactNodeEnteredEvent>(uid, new ArtifactNodeEnteredEvent(component.CurrentNode.Id), false);
		}

		// Token: 0x06000079 RID: 121 RVA: 0x000049EC File Offset: 0x00002BEC
		[NullableContext(2)]
		private void ExitNode(EntityUid uid, ArtifactComponent component = null)
		{
			if (!base.Resolve<ArtifactComponent>(uid, ref component, true))
			{
				return;
			}
			ArtifactNode node = component.CurrentNode;
			if (node == null)
			{
				return;
			}
			foreach (string name in node.Effect.Components.Keys.Concat(node.Trigger.Components.Keys))
			{
				ComponentRegistration comp = this._componentFactory.GetRegistration(name, false);
				this.EntityManager.RemoveComponentDeferred(uid, comp.Type);
			}
			component.CurrentNode = null;
		}

		// Token: 0x04000055 RID: 85
		[Dependency]
		private readonly ActionsSystem _actions;

		// Token: 0x04000056 RID: 86
		[Dependency]
		private readonly PopupSystem _popup;

		// Token: 0x04000057 RID: 87
		[Dependency]
		private readonly IConsoleHost _conHost;

		// Token: 0x04000058 RID: 88
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x04000059 RID: 89
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x0400005A RID: 90
		[Dependency]
		private readonly IPrototypeManager _prototype;

		// Token: 0x0400005B RID: 91
		[Dependency]
		private readonly IComponentFactory _componentFactory;

		// Token: 0x0400005C RID: 92
		[Dependency]
		private readonly ISerializationManager _serialization;

		// Token: 0x0400005D RID: 93
		private const int MaxEdgesPerNode = 4;
	}
}
