using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Construction.Components;
using Content.Server.Stack;
using Content.Shared.Construction.Components;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Stacks;
using Content.Shared.Tag;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Construction
{
	// Token: 0x020005F1 RID: 1521
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MachineFrameSystem : EntitySystem
	{
		// Token: 0x0600209E RID: 8350 RVA: 0x000AB7C4 File Offset: 0x000A99C4
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<MachineFrameComponent, ComponentInit>(new ComponentEventHandler<MachineFrameComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<MachineFrameComponent, ComponentStartup>(new ComponentEventHandler<MachineFrameComponent, ComponentStartup>(this.OnStartup), null, null);
			base.SubscribeLocalEvent<MachineFrameComponent, InteractUsingEvent>(new ComponentEventHandler<MachineFrameComponent, InteractUsingEvent>(this.OnInteractUsing), null, null);
			base.SubscribeLocalEvent<MachineFrameComponent, ExaminedEvent>(new ComponentEventHandler<MachineFrameComponent, ExaminedEvent>(this.OnMachineFrameExamined), null, null);
		}

		// Token: 0x0600209F RID: 8351 RVA: 0x000AB827 File Offset: 0x000A9A27
		private void OnInit(EntityUid uid, MachineFrameComponent component, ComponentInit args)
		{
			component.BoardContainer = this._container.EnsureContainer<Container>(uid, "machine_board", null);
			component.PartContainer = this._container.EnsureContainer<Container>(uid, "machine_parts", null);
		}

		// Token: 0x060020A0 RID: 8352 RVA: 0x000AB85C File Offset: 0x000A9A5C
		private void OnStartup(EntityUid uid, MachineFrameComponent component, ComponentStartup args)
		{
			this.RegenerateProgress(component);
			ConstructionComponent construction;
			if (base.TryComp<ConstructionComponent>(uid, ref construction))
			{
				this._construction.SetPathfindingTarget(uid, "machine", construction);
			}
		}

		// Token: 0x060020A1 RID: 8353 RVA: 0x000AB890 File Offset: 0x000A9A90
		private void OnInteractUsing(EntityUid uid, MachineFrameComponent component, InteractUsingEvent args)
		{
			MachineBoardComponent machineBoard;
			if (!component.HasBoard && base.TryComp<MachineBoardComponent>(args.Used, ref machineBoard))
			{
				if (this._container.TryRemoveFromContainer(args.Used, false))
				{
					component.BoardContainer.Insert(args.Used, null, null, null, null, null);
					this.ResetProgressAndRequirements(component, machineBoard);
					ConstructionComponent construction;
					if (base.TryComp<ConstructionComponent>(uid, ref construction))
					{
						this._construction.ResetEdge(uid, construction);
						return;
					}
				}
			}
			else if (component.HasBoard)
			{
				MachinePartComponent machinePart;
				if (base.TryComp<MachinePartComponent>(args.Used, ref machinePart))
				{
					if (!component.Requirements.ContainsKey(machinePart.PartType))
					{
						return;
					}
					if (component.Progress[machinePart.PartType] != component.Requirements[machinePart.PartType] && this._container.TryRemoveFromContainer(args.Used, false) && component.PartContainer.Insert(args.Used, null, null, null, null, null))
					{
						Dictionary<string, int> progress = component.Progress;
						string text = machinePart.PartType;
						int num = progress[text];
						progress[text] = num + 1;
						args.Handled = true;
						return;
					}
				}
				StackComponent stack;
				if (base.TryComp<StackComponent>(args.Used, ref stack))
				{
					string type = stack.StackTypeId;
					if (type == null)
					{
						return;
					}
					if (!component.MaterialRequirements.ContainsKey(type))
					{
						return;
					}
					if (component.MaterialProgress[type] == component.MaterialRequirements[type])
					{
						return;
					}
					int needed = component.MaterialRequirements[type] - component.MaterialProgress[type];
					int count = stack.Count;
					if (count < needed)
					{
						if (!component.PartContainer.Insert(stack.Owner, null, null, null, null, null))
						{
							return;
						}
						Dictionary<string, int> materialProgress = component.MaterialProgress;
						string text = type;
						materialProgress[text] += count;
						args.Handled = true;
						return;
					}
					else
					{
						EntityUid? splitStack = this._stack.Split(args.Used, needed, base.Comp<TransformComponent>(uid).Coordinates, stack);
						if (splitStack == null)
						{
							return;
						}
						if (!component.PartContainer.Insert(splitStack.Value, null, null, null, null, null))
						{
							return;
						}
						Dictionary<string, int> materialProgress = component.MaterialProgress;
						string text = type;
						materialProgress[text] += needed;
						args.Handled = true;
						return;
					}
				}
				else
				{
					foreach (KeyValuePair<string, GenericPartInfo> keyValuePair in component.ComponentRequirements)
					{
						string text;
						GenericPartInfo genericPartInfo;
						keyValuePair.Deconstruct(out text, out genericPartInfo);
						string compName = text;
						GenericPartInfo info = genericPartInfo;
						if (component.ComponentProgress[compName] < info.Amount)
						{
							ComponentRegistration registration = this._factory.GetRegistration(compName, false);
							if (base.HasComp(args.Used, registration.Type) && this._container.TryRemoveFromContainer(args.Used, false) && component.PartContainer.Insert(args.Used, null, null, null, null, null))
							{
								Dictionary<string, int> componentProgress = component.ComponentProgress;
								text = compName;
								int num = componentProgress[text];
								componentProgress[text] = num + 1;
								args.Handled = true;
								return;
							}
						}
					}
					foreach (KeyValuePair<string, GenericPartInfo> keyValuePair in component.TagRequirements)
					{
						string text;
						GenericPartInfo genericPartInfo;
						keyValuePair.Deconstruct(out text, out genericPartInfo);
						string tagName = text;
						GenericPartInfo info2 = genericPartInfo;
						if (component.TagProgress[tagName] < info2.Amount && this._tag.HasTag(args.Used, tagName) && this._container.TryRemoveFromContainer(args.Used, false) && component.PartContainer.Insert(args.Used, null, null, null, null, null))
						{
							Dictionary<string, int> tagProgress = component.TagProgress;
							text = tagName;
							int num = tagProgress[text];
							tagProgress[text] = num + 1;
							args.Handled = true;
							break;
						}
					}
				}
			}
		}

		// Token: 0x060020A2 RID: 8354 RVA: 0x000ABCA0 File Offset: 0x000A9EA0
		public bool IsComplete(MachineFrameComponent component)
		{
			if (!component.HasBoard)
			{
				return false;
			}
			foreach (KeyValuePair<string, int> keyValuePair in component.Requirements)
			{
				string text;
				int num;
				keyValuePair.Deconstruct(out text, out num);
				string part = text;
				int amount = num;
				if (component.Progress[part] < amount)
				{
					return false;
				}
			}
			foreach (KeyValuePair<string, int> keyValuePair in component.MaterialRequirements)
			{
				string text;
				int num;
				keyValuePair.Deconstruct(out text, out num);
				string type = text;
				int amount2 = num;
				if (component.MaterialProgress[type] < amount2)
				{
					return false;
				}
			}
			foreach (KeyValuePair<string, GenericPartInfo> keyValuePair2 in component.ComponentRequirements)
			{
				string text;
				GenericPartInfo genericPartInfo;
				keyValuePair2.Deconstruct(out text, out genericPartInfo);
				string compName = text;
				GenericPartInfo info = genericPartInfo;
				if (component.ComponentProgress[compName] < info.Amount)
				{
					return false;
				}
			}
			foreach (KeyValuePair<string, GenericPartInfo> keyValuePair2 in component.TagRequirements)
			{
				string text;
				GenericPartInfo genericPartInfo;
				keyValuePair2.Deconstruct(out text, out genericPartInfo);
				string tagName = text;
				GenericPartInfo info2 = genericPartInfo;
				if (component.TagProgress[tagName] < info2.Amount)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060020A3 RID: 8355 RVA: 0x000ABE60 File Offset: 0x000AA060
		public void ResetProgressAndRequirements(MachineFrameComponent component, MachineBoardComponent machineBoard)
		{
			component.Requirements = new Dictionary<string, int>(machineBoard.Requirements);
			component.MaterialRequirements = new Dictionary<string, int>(machineBoard.MaterialIdRequirements);
			component.ComponentRequirements = new Dictionary<string, GenericPartInfo>(machineBoard.ComponentRequirements);
			component.TagRequirements = new Dictionary<string, GenericPartInfo>(machineBoard.TagRequirements);
			component.Progress.Clear();
			component.MaterialProgress.Clear();
			component.ComponentProgress.Clear();
			component.TagProgress.Clear();
			foreach (KeyValuePair<string, int> keyValuePair in component.Requirements)
			{
				string text;
				int num;
				keyValuePair.Deconstruct(out text, out num);
				string machinePart = text;
				component.Progress[machinePart] = 0;
			}
			foreach (KeyValuePair<string, int> keyValuePair in component.MaterialRequirements)
			{
				string text;
				int num;
				keyValuePair.Deconstruct(out text, out num);
				string stackType = text;
				component.MaterialProgress[stackType] = 0;
			}
			foreach (KeyValuePair<string, GenericPartInfo> keyValuePair2 in component.ComponentRequirements)
			{
				string text;
				GenericPartInfo genericPartInfo;
				keyValuePair2.Deconstruct(out text, out genericPartInfo);
				string compName = text;
				component.ComponentProgress[compName] = 0;
			}
			foreach (KeyValuePair<string, GenericPartInfo> keyValuePair2 in component.TagRequirements)
			{
				string text;
				GenericPartInfo genericPartInfo;
				keyValuePair2.Deconstruct(out text, out genericPartInfo);
				string compName2 = text;
				component.TagProgress[compName2] = 0;
			}
		}

		// Token: 0x060020A4 RID: 8356 RVA: 0x000AC040 File Offset: 0x000AA240
		public void RegenerateProgress(MachineFrameComponent component)
		{
			if (!component.HasBoard)
			{
				component.TagRequirements.Clear();
				component.MaterialRequirements.Clear();
				component.ComponentRequirements.Clear();
				component.TagRequirements.Clear();
				component.Progress.Clear();
				component.MaterialProgress.Clear();
				component.ComponentProgress.Clear();
				component.TagProgress.Clear();
				return;
			}
			EntityUid board = component.BoardContainer.ContainedEntities[0];
			MachineBoardComponent machineBoard;
			if (!base.TryComp<MachineBoardComponent>(board, ref machineBoard))
			{
				return;
			}
			this.ResetProgressAndRequirements(component, machineBoard);
			foreach (EntityUid part in component.PartContainer.ContainedEntities)
			{
				MachinePartComponent machinePart;
				if (base.TryComp<MachinePartComponent>(part, ref machinePart))
				{
					if (!component.Requirements.ContainsKey(machinePart.PartType))
					{
						continue;
					}
					if (!component.Progress.ContainsKey(machinePart.PartType))
					{
						component.Progress[machinePart.PartType] = 1;
					}
					else
					{
						Dictionary<string, int> progress = component.Progress;
						string text = machinePart.PartType;
						int num = progress[text];
						progress[text] = num + 1;
					}
				}
				StackComponent stack;
				if (base.TryComp<StackComponent>(part, ref stack))
				{
					string type = stack.StackTypeId;
					if (type == null || !component.MaterialRequirements.ContainsKey(type))
					{
						continue;
					}
					if (!component.MaterialProgress.ContainsKey(type))
					{
						component.MaterialProgress[type] = 1;
					}
					else
					{
						Dictionary<string, int> materialProgress = component.MaterialProgress;
						string text = type;
						int num = materialProgress[text];
						materialProgress[text] = num + 1;
					}
				}
				foreach (KeyValuePair<string, GenericPartInfo> keyValuePair in component.ComponentRequirements)
				{
					string text;
					GenericPartInfo genericPartInfo;
					keyValuePair.Deconstruct(out text, out genericPartInfo);
					string compName = text;
					ComponentRegistration registration = this._factory.GetRegistration(compName, false);
					if (base.HasComp(part, registration.Type))
					{
						if (!component.ComponentProgress.ContainsKey(compName))
						{
							component.ComponentProgress[compName] = 1;
						}
						else
						{
							Dictionary<string, int> componentProgress = component.ComponentProgress;
							text = compName;
							int num = componentProgress[text];
							componentProgress[text] = num + 1;
						}
					}
				}
				foreach (KeyValuePair<string, GenericPartInfo> keyValuePair in component.TagRequirements)
				{
					string text;
					GenericPartInfo genericPartInfo;
					keyValuePair.Deconstruct(out text, out genericPartInfo);
					string tagName = text;
					if (this._tag.HasTag(part, tagName))
					{
						if (!component.TagProgress.ContainsKey(tagName))
						{
							component.TagProgress[tagName] = 1;
						}
						else
						{
							Dictionary<string, int> tagProgress = component.TagProgress;
							text = tagName;
							int num = tagProgress[text];
							tagProgress[text] = num + 1;
						}
					}
				}
			}
		}

		// Token: 0x060020A5 RID: 8357 RVA: 0x000AC35C File Offset: 0x000AA55C
		private void OnMachineFrameExamined(EntityUid uid, MachineFrameComponent component, ExaminedEvent args)
		{
			if (!args.IsInDetailsRange)
			{
				return;
			}
			if (component.HasBoard)
			{
				args.PushMarkup(Loc.GetString("machine-frame-component-on-examine-label", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("board", this.EntityManager.GetComponent<MetaDataComponent>(component.BoardContainer.ContainedEntities[0]).EntityName)
				}));
			}
		}

		// Token: 0x04001423 RID: 5155
		[Dependency]
		private readonly IComponentFactory _factory;

		// Token: 0x04001424 RID: 5156
		[Dependency]
		private readonly SharedContainerSystem _container;

		// Token: 0x04001425 RID: 5157
		[Dependency]
		private readonly TagSystem _tag;

		// Token: 0x04001426 RID: 5158
		[Dependency]
		private readonly StackSystem _stack;

		// Token: 0x04001427 RID: 5159
		[Dependency]
		private readonly ConstructionSystem _construction;
	}
}
