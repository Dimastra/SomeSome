using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Server.NodeContainer.Nodes;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Server.NodeContainer.NodeGroups
{
	// Token: 0x02000386 RID: 902
	[NullableContext(1)]
	[Nullable(0)]
	[NodeGroup(new NodeGroupID[]
	{
		NodeGroupID.Pipe
	})]
	public sealed class PipeNet : BaseNodeGroup, IPipeNet, INodeGroup, IGasMixtureHolder
	{
		// Token: 0x1700029D RID: 669
		// (get) Token: 0x06001270 RID: 4720 RVA: 0x0005F43E File Offset: 0x0005D63E
		// (set) Token: 0x06001271 RID: 4721 RVA: 0x0005F446 File Offset: 0x0005D646
		[ViewVariables]
		public GasMixture Air { get; set; } = new GasMixture
		{
			Temperature = 293.15f
		};

		// Token: 0x1700029E RID: 670
		// (get) Token: 0x06001272 RID: 4722 RVA: 0x0005F44F File Offset: 0x0005D64F
		// (set) Token: 0x06001273 RID: 4723 RVA: 0x0005F457 File Offset: 0x0005D657
		public EntityUid? Grid { get; private set; }

		// Token: 0x06001274 RID: 4724 RVA: 0x0005F460 File Offset: 0x0005D660
		public override void Initialize(Node sourceNode, IEntityManager entMan)
		{
			base.Initialize(sourceNode, entMan);
			this.Grid = entMan.GetComponent<TransformComponent>(sourceNode.Owner).GridUid;
			if (this.Grid == null)
			{
				return;
			}
			this._atmosphereSystem = entMan.EntitySysManager.GetEntitySystem<AtmosphereSystem>();
			this._atmosphereSystem.AddPipeNet(this.Grid.Value, this);
		}

		// Token: 0x06001275 RID: 4725 RVA: 0x0005F4C8 File Offset: 0x0005D6C8
		public void Update()
		{
			AtmosphereSystem atmosphereSystem = this._atmosphereSystem;
			if (atmosphereSystem == null)
			{
				return;
			}
			atmosphereSystem.React(this.Air, this);
		}

		// Token: 0x06001276 RID: 4726 RVA: 0x0005F4E4 File Offset: 0x0005D6E4
		public override void LoadNodes(List<Node> groupNodes)
		{
			base.LoadNodes(groupNodes);
			foreach (Node node in groupNodes)
			{
				PipeNode pipeNode = (PipeNode)node;
				this.Air.Volume += pipeNode.Volume;
			}
		}

		// Token: 0x06001277 RID: 4727 RVA: 0x0005F550 File Offset: 0x0005D750
		public override void RemoveNode(Node node)
		{
			base.RemoveNode(node);
			if (node.Deleting)
			{
				PipeNode pipe = node as PipeNode;
				if (pipe != null)
				{
					this.Air.Multiply(1f - pipe.Volume / this.Air.Volume);
					this.Air.Volume -= pipe.Volume;
					return;
				}
			}
		}

		// Token: 0x06001278 RID: 4728 RVA: 0x0005F5B4 File Offset: 0x0005D7B4
		public override void AfterRemake([Nullable(new byte[]
		{
			1,
			1,
			2,
			1
		})] IEnumerable<IGrouping<INodeGroup, Node>> newGroups)
		{
			this.RemoveFromGridAtmos();
			List<GasMixture> newAir = new List<GasMixture>(newGroups.Count<IGrouping<INodeGroup, Node>>());
			foreach (IGrouping<INodeGroup, Node> grouping in newGroups)
			{
				IPipeNet newPipeNet = grouping.Key as IPipeNet;
				if (newPipeNet != null)
				{
					newAir.Add(newPipeNet.Air);
				}
			}
			AtmosphereSystem atmosphereSystem = this._atmosphereSystem;
			if (atmosphereSystem == null)
			{
				return;
			}
			atmosphereSystem.DivideInto(this.Air, newAir);
		}

		// Token: 0x06001279 RID: 4729 RVA: 0x0005F638 File Offset: 0x0005D838
		private void RemoveFromGridAtmos()
		{
			if (this.Grid == null)
			{
				return;
			}
			AtmosphereSystem atmosphereSystem = this._atmosphereSystem;
			if (atmosphereSystem == null)
			{
				return;
			}
			atmosphereSystem.RemovePipeNet(this.Grid.Value, this);
		}

		// Token: 0x0600127A RID: 4730 RVA: 0x0005F678 File Offset: 0x0005D878
		public override string GetDebugData()
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(33, 3);
			defaultInterpolatedStringHandler.AppendLiteral("Pressure: ");
			defaultInterpolatedStringHandler.AppendFormatted<float>(this.Air.Pressure, "G3");
			defaultInterpolatedStringHandler.AppendLiteral("\nTemperature: ");
			defaultInterpolatedStringHandler.AppendFormatted<float>(this.Air.Temperature, "G3");
			defaultInterpolatedStringHandler.AppendLiteral("\nVolume: ");
			defaultInterpolatedStringHandler.AppendFormatted<float>(this.Air.Volume, "G3");
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}

		// Token: 0x04000B56 RID: 2902
		[Nullable(2)]
		[ViewVariables]
		private AtmosphereSystem _atmosphereSystem;
	}
}
