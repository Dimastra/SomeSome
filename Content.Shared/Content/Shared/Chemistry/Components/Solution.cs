using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Chemistry.Components
{
	// Token: 0x020005FC RID: 1532
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[DataDefinition]
	[Serializable]
	public sealed class Solution : IEnumerable<Solution.ReagentQuantity>, IEnumerable, ISerializationHooks
	{
		// Token: 0x170003C0 RID: 960
		// (get) Token: 0x0600129F RID: 4767 RVA: 0x0003CC51 File Offset: 0x0003AE51
		// (set) Token: 0x060012A0 RID: 4768 RVA: 0x0003CC59 File Offset: 0x0003AE59
		[ViewVariables]
		public FixedPoint2 Volume { get; set; }

		// Token: 0x170003C1 RID: 961
		// (get) Token: 0x060012A1 RID: 4769 RVA: 0x0003CC62 File Offset: 0x0003AE62
		// (set) Token: 0x060012A2 RID: 4770 RVA: 0x0003CC6A File Offset: 0x0003AE6A
		[DataField("maxVol", false, 1, false, false, null)]
		[ViewVariables]
		public FixedPoint2 MaxVolume { get; set; }

		// Token: 0x170003C2 RID: 962
		// (get) Token: 0x060012A3 RID: 4771 RVA: 0x0003CC74 File Offset: 0x0003AE74
		public float FillFraction
		{
			get
			{
				if (!(this.MaxVolume == 0))
				{
					return this.Volume.Float() / this.MaxVolume.Float();
				}
				return 1f;
			}
		}

		// Token: 0x170003C3 RID: 963
		// (get) Token: 0x060012A4 RID: 4772 RVA: 0x0003CCB2 File Offset: 0x0003AEB2
		// (set) Token: 0x060012A5 RID: 4773 RVA: 0x0003CCBA File Offset: 0x0003AEBA
		[ViewVariables]
		[DataField("canReact", false, 1, false, false, null)]
		public bool CanReact { get; set; }

		// Token: 0x170003C4 RID: 964
		// (get) Token: 0x060012A6 RID: 4774 RVA: 0x0003CCC3 File Offset: 0x0003AEC3
		// (set) Token: 0x060012A7 RID: 4775 RVA: 0x0003CCCB File Offset: 0x0003AECB
		[ViewVariables]
		[DataField("canMix", false, 1, false, false, null)]
		public bool CanMix { get; set; }

		// Token: 0x170003C5 RID: 965
		// (get) Token: 0x060012A8 RID: 4776 RVA: 0x0003CCD4 File Offset: 0x0003AED4
		[ViewVariables]
		public FixedPoint2 AvailableVolume
		{
			get
			{
				return this.MaxVolume - this.Volume;
			}
		}

		// Token: 0x170003C6 RID: 966
		// (get) Token: 0x060012A9 RID: 4777 RVA: 0x0003CCE7 File Offset: 0x0003AEE7
		// (set) Token: 0x060012AA RID: 4778 RVA: 0x0003CCEF File Offset: 0x0003AEEF
		[ViewVariables]
		[DataField("temperature", false, 1, false, false, null)]
		public float Temperature { get; set; }

		// Token: 0x060012AB RID: 4779 RVA: 0x0003CCF8 File Offset: 0x0003AEF8
		public bool CanAddSolution(Solution solution)
		{
			return solution.Volume <= this.AvailableVolume;
		}

		// Token: 0x060012AC RID: 4780 RVA: 0x0003CD0C File Offset: 0x0003AF0C
		[NullableContext(2)]
		public void UpdateHeatCapacity(IPrototypeManager protoMan)
		{
			IoCManager.Resolve<IPrototypeManager>(ref protoMan);
			this._heatCapacityDirty = false;
			this._heatCapacity = 0f;
			foreach (Solution.ReagentQuantity reagent in this.Contents)
			{
				this._heatCapacity += (float)reagent.Quantity * protoMan.Index<ReagentPrototype>(reagent.ReagentId).SpecificHeat;
			}
		}

		// Token: 0x060012AD RID: 4781 RVA: 0x0003CD9C File Offset: 0x0003AF9C
		[NullableContext(2)]
		public float GetHeatCapacity(IPrototypeManager protoMan)
		{
			if (this._heatCapacityDirty)
			{
				this.UpdateHeatCapacity(protoMan);
			}
			return this._heatCapacity;
		}

		// Token: 0x060012AE RID: 4782 RVA: 0x0003CDB3 File Offset: 0x0003AFB3
		public Solution() : this(2)
		{
		}

		// Token: 0x060012AF RID: 4783 RVA: 0x0003CDBC File Offset: 0x0003AFBC
		public Solution(int capacity)
		{
			this.Contents = new List<Solution.ReagentQuantity>(2);
			this.MaxVolume = FixedPoint2.Zero;
			this.CanReact = true;
			this.Temperature = 293.15f;
			this._heatCapacityDirty = true;
			base..ctor();
			this.Contents = new List<Solution.ReagentQuantity>(capacity);
		}

		// Token: 0x060012B0 RID: 4784 RVA: 0x0003CE0B File Offset: 0x0003B00B
		public Solution(string reagentId, FixedPoint2 quantity) : this()
		{
			this.AddReagent(reagentId, quantity, true);
		}

		// Token: 0x060012B1 RID: 4785 RVA: 0x0003CE1C File Offset: 0x0003B01C
		public Solution(IEnumerable<Solution.ReagentQuantity> reagents, bool setMaxVol = true)
		{
			this.Contents = new List<Solution.ReagentQuantity>(2);
			this.MaxVolume = FixedPoint2.Zero;
			this.CanReact = true;
			this.Temperature = 293.15f;
			this._heatCapacityDirty = true;
			base..ctor();
			this.Contents = new List<Solution.ReagentQuantity>(reagents);
			this.Volume = FixedPoint2.Zero;
			foreach (Solution.ReagentQuantity reagent in this.Contents)
			{
				this.Volume += reagent.Quantity;
			}
			if (setMaxVol)
			{
				this.MaxVolume = this.Volume;
			}
			this.ValidateSolution();
		}

		// Token: 0x060012B2 RID: 4786 RVA: 0x0003CEE4 File Offset: 0x0003B0E4
		public Solution(Solution solution)
		{
			this.Contents = new List<Solution.ReagentQuantity>(2);
			this.MaxVolume = FixedPoint2.Zero;
			this.CanReact = true;
			this.Temperature = 293.15f;
			this._heatCapacityDirty = true;
			base..ctor();
			this.Volume = solution.Volume;
			this._heatCapacity = solution._heatCapacity;
			this._heatCapacityDirty = solution._heatCapacityDirty;
			this.Contents = Extensions.ShallowClone<Solution.ReagentQuantity>(solution.Contents);
			this.ValidateSolution();
		}

		// Token: 0x060012B3 RID: 4787 RVA: 0x0003CF62 File Offset: 0x0003B162
		public Solution Clone()
		{
			return new Solution(this);
		}

		// Token: 0x060012B4 RID: 4788 RVA: 0x0003CF6A File Offset: 0x0003B16A
		public void ValidateSolution()
		{
		}

		// Token: 0x060012B5 RID: 4789 RVA: 0x0003CF6C File Offset: 0x0003B16C
		void ISerializationHooks.AfterDeserialization()
		{
			this.Volume = FixedPoint2.Zero;
			foreach (Solution.ReagentQuantity reagent in this.Contents)
			{
				this.Volume += reagent.Quantity;
			}
			if (this.MaxVolume == FixedPoint2.Zero)
			{
				this.MaxVolume = this.Volume;
			}
		}

		// Token: 0x060012B6 RID: 4790 RVA: 0x0003CFF8 File Offset: 0x0003B1F8
		public bool ContainsReagent(string reagentId)
		{
			using (List<Solution.ReagentQuantity>.Enumerator enumerator = this.Contents.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.ReagentId == reagentId)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x060012B7 RID: 4791 RVA: 0x0003D058 File Offset: 0x0003B258
		public bool TryGetReagent(string reagentId, out FixedPoint2 quantity)
		{
			foreach (Solution.ReagentQuantity reagent in this.Contents)
			{
				if (reagent.ReagentId == reagentId)
				{
					quantity = reagent.Quantity;
					return true;
				}
			}
			quantity = FixedPoint2.New(0);
			return false;
		}

		// Token: 0x060012B8 RID: 4792 RVA: 0x0003D0D4 File Offset: 0x0003B2D4
		[NullableContext(2)]
		public string GetPrimaryReagentId()
		{
			if (this.Contents.Count == 0)
			{
				return null;
			}
			Solution.ReagentQuantity max = default(Solution.ReagentQuantity);
			foreach (Solution.ReagentQuantity reagent in this.Contents)
			{
				if (reagent.Quantity >= max.Quantity)
				{
					max = reagent;
				}
			}
			return max.ReagentId;
		}

		// Token: 0x060012B9 RID: 4793 RVA: 0x0003D154 File Offset: 0x0003B354
		public void AddReagent(string reagentId, FixedPoint2 quantity, bool dirtyHeatCap = true)
		{
			if (quantity <= 0)
			{
				return;
			}
			this.Volume += quantity;
			this._heatCapacityDirty = (this._heatCapacityDirty || dirtyHeatCap);
			for (int i = 0; i < this.Contents.Count; i++)
			{
				Solution.ReagentQuantity reagent = this.Contents[i];
				if (!(reagent.ReagentId != reagentId))
				{
					this.Contents[i] = new Solution.ReagentQuantity(reagentId, reagent.Quantity + quantity);
					this.ValidateSolution();
					return;
				}
			}
			this.Contents.Add(new Solution.ReagentQuantity(reagentId, quantity));
			this.ValidateSolution();
		}

		// Token: 0x060012BA RID: 4794 RVA: 0x0003D1F9 File Offset: 0x0003B3F9
		public void AddReagent(ReagentPrototype proto, FixedPoint2 quantity)
		{
			this.AddReagent(proto.ID, quantity, false);
			this._heatCapacity += quantity.Float() * proto.SpecificHeat;
		}

		// Token: 0x060012BB RID: 4795 RVA: 0x0003D224 File Offset: 0x0003B424
		public void AddReagent(ReagentPrototype proto, FixedPoint2 quantity, float temperature, [Nullable(2)] IPrototypeManager protoMan)
		{
			if (this._heatCapacityDirty)
			{
				this.UpdateHeatCapacity(protoMan);
			}
			float totalThermalEnergy = this.Temperature * this._heatCapacity + temperature * proto.SpecificHeat;
			this.AddReagent(proto, quantity);
			this.Temperature = ((this._heatCapacity == 0f) ? 0f : (totalThermalEnergy / this._heatCapacity));
		}

		// Token: 0x060012BC RID: 4796 RVA: 0x0003D284 File Offset: 0x0003B484
		public void ScaleSolution(int scale)
		{
			if (scale == 1)
			{
				return;
			}
			if (scale <= 0)
			{
				this.RemoveAllSolution();
				return;
			}
			this._heatCapacity *= (float)scale;
			this.Volume *= scale;
			for (int i = 0; i < this.Contents.Count; i++)
			{
				Solution.ReagentQuantity old = this.Contents[i];
				this.Contents[i] = new Solution.ReagentQuantity(old.ReagentId, old.Quantity * scale);
			}
			this.ValidateSolution();
		}

		// Token: 0x060012BD RID: 4797 RVA: 0x0003D310 File Offset: 0x0003B510
		public void ScaleSolution(float scale)
		{
			if (scale == 1f)
			{
				return;
			}
			if (scale == 0f)
			{
				this.RemoveAllSolution();
				return;
			}
			this.Volume = FixedPoint2.Zero;
			for (int i = this.Contents.Count - 1; i >= 0; i--)
			{
				Solution.ReagentQuantity old = this.Contents[i];
				FixedPoint2 newQuantity = old.Quantity * scale;
				if (newQuantity == FixedPoint2.Zero)
				{
					Extensions.RemoveSwap<Solution.ReagentQuantity>(this.Contents, i);
				}
				else
				{
					this.Contents[i] = new Solution.ReagentQuantity(old.ReagentId, newQuantity);
					this.Volume += newQuantity;
				}
			}
			this._heatCapacityDirty = true;
			this.ValidateSolution();
		}

		// Token: 0x060012BE RID: 4798 RVA: 0x0003D3C8 File Offset: 0x0003B5C8
		public FixedPoint2 GetReagentQuantity(string reagentId)
		{
			for (int i = 0; i < this.Contents.Count; i++)
			{
				Solution.ReagentQuantity reagent = this.Contents[i];
				if (reagent.ReagentId == reagentId)
				{
					return reagent.Quantity;
				}
			}
			return FixedPoint2.Zero;
		}

		// Token: 0x060012BF RID: 4799 RVA: 0x0003D414 File Offset: 0x0003B614
		public FixedPoint2 RemoveReagent(string reagentId, FixedPoint2 quantity)
		{
			if (quantity <= FixedPoint2.Zero)
			{
				return FixedPoint2.Zero;
			}
			int i = 0;
			while (i < this.Contents.Count)
			{
				Solution.ReagentQuantity reagent = this.Contents[i];
				if (!(reagent.ReagentId != reagentId))
				{
					FixedPoint2 curQuantity = reagent.Quantity;
					FixedPoint2 newQuantity = curQuantity - quantity;
					this._heatCapacityDirty = true;
					if (newQuantity <= 0)
					{
						Extensions.RemoveSwap<Solution.ReagentQuantity>(this.Contents, i);
						this.Volume -= curQuantity;
						this.ValidateSolution();
						return curQuantity;
					}
					this.Contents[i] = new Solution.ReagentQuantity(reagentId, newQuantity);
					this.Volume -= quantity;
					this.ValidateSolution();
					return quantity;
				}
				else
				{
					i++;
				}
			}
			return FixedPoint2.Zero;
		}

		// Token: 0x060012C0 RID: 4800 RVA: 0x0003D4E3 File Offset: 0x0003B6E3
		public void RemoveAllSolution()
		{
			this.Contents.Clear();
			this.Volume = FixedPoint2.Zero;
			this._heatCapacityDirty = false;
			this._heatCapacity = 0f;
		}

		// Token: 0x060012C1 RID: 4801 RVA: 0x0003D510 File Offset: 0x0003B710
		public Solution SplitSolution(FixedPoint2 toTake)
		{
			if (toTake <= FixedPoint2.Zero)
			{
				return new Solution();
			}
			Solution newSolution;
			if (toTake >= this.Volume)
			{
				newSolution = this.Clone();
				this.RemoveAllSolution();
				return newSolution;
			}
			FixedPoint2 origVol = this.Volume;
			int effVol = this.Volume.Value;
			newSolution = new Solution(this.Contents.Count)
			{
				Temperature = this.Temperature
			};
			long remaining = (long)toTake.Value;
			for (int i = this.Contents.Count - 1; i >= 0; i--)
			{
				Solution.ReagentQuantity reagent = this.Contents[i];
				long split = remaining * (long)reagent.Quantity.Value / (long)effVol;
				if (split <= 0L)
				{
					effVol -= reagent.Quantity.Value;
				}
				else
				{
					FixedPoint2 splitQuantity = FixedPoint2.FromCents((int)split);
					FixedPoint2 newQuantity = reagent.Quantity - splitQuantity;
					if (newQuantity > FixedPoint2.Zero)
					{
						this.Contents[i] = new Solution.ReagentQuantity(reagent.ReagentId, newQuantity);
					}
					else
					{
						Extensions.RemoveSwap<Solution.ReagentQuantity>(this.Contents, i);
					}
					newSolution.Contents.Add(new Solution.ReagentQuantity(reagent.ReagentId, splitQuantity));
					this.Volume -= splitQuantity;
					remaining -= split;
					effVol -= reagent.Quantity.Value;
				}
			}
			newSolution.Volume = origVol - this.Volume;
			this._heatCapacityDirty = true;
			newSolution._heatCapacityDirty = true;
			this.ValidateSolution();
			newSolution.ValidateSolution();
			return newSolution;
		}

		// Token: 0x060012C2 RID: 4802 RVA: 0x0003D6AC File Offset: 0x0003B8AC
		public void RemoveSolution(FixedPoint2 toTake)
		{
			if (toTake <= FixedPoint2.Zero)
			{
				return;
			}
			if (toTake >= this.Volume)
			{
				this.RemoveAllSolution();
				return;
			}
			int effVol = this.Volume.Value;
			this.Volume -= toTake;
			long remaining = (long)toTake.Value;
			for (int i = this.Contents.Count - 1; i >= 0; i--)
			{
				Solution.ReagentQuantity reagent = this.Contents[i];
				long split = remaining * (long)reagent.Quantity.Value / (long)effVol;
				if (split <= 0L)
				{
					effVol -= reagent.Quantity.Value;
				}
				else
				{
					FixedPoint2 splitQuantity = FixedPoint2.FromCents((int)split);
					FixedPoint2 newQuantity = reagent.Quantity - splitQuantity;
					if (newQuantity > FixedPoint2.Zero)
					{
						this.Contents[i] = new Solution.ReagentQuantity(reagent.ReagentId, newQuantity);
					}
					else
					{
						Extensions.RemoveSwap<Solution.ReagentQuantity>(this.Contents, i);
					}
					remaining -= split;
					effVol -= reagent.Quantity.Value;
				}
			}
			this._heatCapacityDirty = true;
			this.ValidateSolution();
		}

		// Token: 0x060012C3 RID: 4803 RVA: 0x0003D7D0 File Offset: 0x0003B9D0
		public void AddSolution(Solution otherSolution, [Nullable(2)] IPrototypeManager protoMan)
		{
			if (otherSolution.Volume <= FixedPoint2.Zero)
			{
				return;
			}
			this.Volume += otherSolution.Volume;
			bool closeTemps = MathHelper.CloseTo(otherSolution.Temperature, this.Temperature, 1E-07f);
			float totalThermalEnergy = 0f;
			if (!closeTemps)
			{
				IoCManager.Resolve<IPrototypeManager>(ref protoMan);
				if (this._heatCapacityDirty)
				{
					this.UpdateHeatCapacity(protoMan);
				}
				if (otherSolution._heatCapacityDirty)
				{
					otherSolution.UpdateHeatCapacity(protoMan);
				}
				totalThermalEnergy = this._heatCapacity * this.Temperature + otherSolution._heatCapacity * otherSolution.Temperature;
			}
			for (int i = 0; i < otherSolution.Contents.Count; i++)
			{
				Solution.ReagentQuantity otherReagent = otherSolution.Contents[i];
				bool found = false;
				for (int j = 0; j < this.Contents.Count; j++)
				{
					Solution.ReagentQuantity reagent = this.Contents[j];
					if (reagent.ReagentId == otherReagent.ReagentId)
					{
						found = true;
						this.Contents[j] = new Solution.ReagentQuantity(reagent.ReagentId, reagent.Quantity + otherReagent.Quantity);
						break;
					}
				}
				if (!found)
				{
					this.Contents.Add(new Solution.ReagentQuantity(otherReagent.ReagentId, otherReagent.Quantity));
				}
			}
			this._heatCapacity += otherSolution._heatCapacity;
			if (closeTemps)
			{
				this._heatCapacityDirty |= otherSolution._heatCapacityDirty;
			}
			else
			{
				this.Temperature = ((this._heatCapacity == 0f) ? 0f : (totalThermalEnergy / this._heatCapacity));
			}
			this.ValidateSolution();
		}

		// Token: 0x060012C4 RID: 4804 RVA: 0x0003D974 File Offset: 0x0003BB74
		[NullableContext(2)]
		public Color GetColor(IPrototypeManager protoMan)
		{
			if (this.Volume == FixedPoint2.Zero)
			{
				return Color.Transparent;
			}
			IoCManager.Resolve<IPrototypeManager>(ref protoMan);
			Color mixColor = default(Color);
			FixedPoint2 runningTotalQuantity = FixedPoint2.New(0);
			bool first = true;
			foreach (Solution.ReagentQuantity reagent in this.Contents)
			{
				runningTotalQuantity += reagent.Quantity;
				ReagentPrototype proto;
				if (protoMan.TryIndex<ReagentPrototype>(reagent.ReagentId, ref proto))
				{
					if (first)
					{
						first = false;
						mixColor = proto.SubstanceColor;
					}
					else
					{
						float interpolateValue = reagent.Quantity.Float() / runningTotalQuantity.Float();
						mixColor = Color.InterpolateBetween(mixColor, proto.SubstanceColor, interpolateValue);
					}
				}
			}
			return mixColor;
		}

		// Token: 0x060012C5 RID: 4805 RVA: 0x0003DA48 File Offset: 0x0003BC48
		[Obsolete("Use ReactiveSystem.DoEntityReaction")]
		public void DoEntityReaction(EntityUid uid, ReactionMethod method)
		{
			IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<ReactiveSystem>().DoEntityReaction(uid, this, method);
		}

		// Token: 0x060012C6 RID: 4806 RVA: 0x0003DA5C File Offset: 0x0003BC5C
		public IEnumerator<Solution.ReagentQuantity> GetEnumerator()
		{
			return this.Contents.GetEnumerator();
		}

		// Token: 0x060012C7 RID: 4807 RVA: 0x0003DA6E File Offset: 0x0003BC6E
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x060012C8 RID: 4808 RVA: 0x0003DA78 File Offset: 0x0003BC78
		public void SetContents(IEnumerable<Solution.ReagentQuantity> reagents, bool setMaxVol = false)
		{
			this.RemoveAllSolution();
			this._heatCapacityDirty = true;
			this.Contents = new List<Solution.ReagentQuantity>(reagents);
			foreach (Solution.ReagentQuantity reagent in this.Contents)
			{
				this.Volume += reagent.Quantity;
			}
			if (setMaxVol)
			{
				this.MaxVolume = this.Volume;
			}
			this.ValidateSolution();
		}

		// Token: 0x04001158 RID: 4440
		[DataField("reagents", false, 1, false, false, null)]
		public List<Solution.ReagentQuantity> Contents;

		// Token: 0x0400115E RID: 4446
		[Nullable(2)]
		public string Name;

		// Token: 0x0400115F RID: 4447
		[ViewVariables]
		private float _heatCapacity;

		// Token: 0x04001160 RID: 4448
		[ViewVariables]
		private bool _heatCapacityDirty;

		// Token: 0x02000857 RID: 2135
		[Nullable(0)]
		[NetSerializable]
		[DataDefinition]
		[Serializable]
		public readonly struct ReagentQuantity : IComparable<Solution.ReagentQuantity>
		{
			// Token: 0x06001961 RID: 6497 RVA: 0x0004FE8F File Offset: 0x0004E08F
			public ReagentQuantity(string reagentId, FixedPoint2 quantity)
			{
				this.ReagentId = reagentId;
				this.Quantity = quantity;
			}

			// Token: 0x06001962 RID: 6498 RVA: 0x0004FEA0 File Offset: 0x0004E0A0
			[ExcludeFromCodeCoverage]
			public override string ToString()
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
				defaultInterpolatedStringHandler.AppendFormatted(this.ReagentId);
				defaultInterpolatedStringHandler.AppendLiteral(":");
				defaultInterpolatedStringHandler.AppendFormatted<FixedPoint2>(this.Quantity);
				return defaultInterpolatedStringHandler.ToStringAndClear();
			}

			// Token: 0x06001963 RID: 6499 RVA: 0x0004FEE4 File Offset: 0x0004E0E4
			public int CompareTo(Solution.ReagentQuantity other)
			{
				return this.Quantity.Float().CompareTo(other.Quantity.Float());
			}

			// Token: 0x06001964 RID: 6500 RVA: 0x0004FF10 File Offset: 0x0004E110
			public void Deconstruct(out string reagentId, out FixedPoint2 quantity)
			{
				reagentId = this.ReagentId;
				quantity = this.Quantity;
			}

			// Token: 0x04001977 RID: 6519
			[DataField("ReagentId", false, 1, false, false, typeof(PrototypeIdSerializer<ReagentPrototype>))]
			public readonly string ReagentId;

			// Token: 0x04001978 RID: 6520
			[DataField("Quantity", false, 1, false, false, null)]
			public readonly FixedPoint2 Quantity;
		}
	}
}
