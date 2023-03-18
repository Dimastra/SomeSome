using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Humanoid.Markings;
using Content.Shared.Humanoid.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Shared.Humanoid
{
	// Token: 0x0200040D RID: 1037
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedHumanoidAppearanceSystem : EntitySystem
	{
		// Token: 0x06000C30 RID: 3120 RVA: 0x0002854B File Offset: 0x0002674B
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<HumanoidAppearanceComponent, ComponentGetState>(new ComponentEventRefHandler<HumanoidAppearanceComponent, ComponentGetState>(this.OnGetState), null, null);
		}

		// Token: 0x06000C31 RID: 3121 RVA: 0x00028568 File Offset: 0x00026768
		private void OnGetState(EntityUid uid, HumanoidAppearanceComponent component, ref ComponentGetState args)
		{
			args.State = new HumanoidAppearanceState(component.MarkingSet, component.PermanentlyHidden, component.HiddenLayers, component.CustomBaseLayers, component.Sex, component.Gender, component.Age, component.Species, component.SkinColor, component.EyeColor);
		}

		// Token: 0x06000C32 RID: 3122 RVA: 0x000285BC File Offset: 0x000267BC
		[NullableContext(2)]
		public void SetLayerVisibility(EntityUid uid, HumanoidVisualLayers layer, bool visible, bool permanent = false, HumanoidAppearanceComponent humanoid = null)
		{
			if (!base.Resolve<HumanoidAppearanceComponent>(uid, ref humanoid, true))
			{
				return;
			}
			bool dirty = false;
			this.SetLayerVisibility(uid, humanoid, layer, visible, permanent, ref dirty);
			if (dirty)
			{
				base.Dirty(humanoid, null);
			}
		}

		// Token: 0x06000C33 RID: 3123 RVA: 0x000285F4 File Offset: 0x000267F4
		public void SetLayersVisibility(EntityUid uid, IEnumerable<HumanoidVisualLayers> layers, bool visible, bool permanent = false, [Nullable(2)] HumanoidAppearanceComponent humanoid = null)
		{
			if (!base.Resolve<HumanoidAppearanceComponent>(uid, ref humanoid, true))
			{
				return;
			}
			bool dirty = false;
			foreach (HumanoidVisualLayers layer in layers)
			{
				this.SetLayerVisibility(uid, humanoid, layer, visible, permanent, ref dirty);
			}
			if (dirty)
			{
				base.Dirty(humanoid, null);
			}
		}

		// Token: 0x06000C34 RID: 3124 RVA: 0x00028660 File Offset: 0x00026860
		protected virtual void SetLayerVisibility(EntityUid uid, HumanoidAppearanceComponent humanoid, HumanoidVisualLayers layer, bool visible, bool permanent, ref bool dirty)
		{
			if (visible)
			{
				if (permanent)
				{
					dirty |= humanoid.PermanentlyHidden.Remove(layer);
				}
				dirty |= humanoid.HiddenLayers.Remove(layer);
				return;
			}
			if (permanent)
			{
				dirty |= humanoid.PermanentlyHidden.Add(layer);
			}
			dirty |= humanoid.HiddenLayers.Add(layer);
		}

		// Token: 0x06000C35 RID: 3125 RVA: 0x000286C8 File Offset: 0x000268C8
		public void SetSpecies(EntityUid uid, string species, bool sync = true, [Nullable(2)] HumanoidAppearanceComponent humanoid = null)
		{
			SpeciesPrototype prototype;
			if (!base.Resolve<HumanoidAppearanceComponent>(uid, ref humanoid, true) || !this._prototypeManager.TryIndex<SpeciesPrototype>(species, ref prototype))
			{
				return;
			}
			humanoid.Species = species;
			humanoid.MarkingSet.FilterSpecies(species, this._markingManager, null);
			List<Marking> oldMarkings = humanoid.MarkingSet.GetForwardEnumerator().ToList<Marking>();
			humanoid.MarkingSet = new MarkingSet(oldMarkings, prototype.MarkingPoints, this._markingManager, this._prototypeManager);
			if (sync)
			{
				base.Dirty(humanoid, null);
			}
		}

		// Token: 0x06000C36 RID: 3126 RVA: 0x0002874B File Offset: 0x0002694B
		[NullableContext(2)]
		public virtual void SetSkinColor(EntityUid uid, Color skinColor, bool sync = true, HumanoidAppearanceComponent humanoid = null)
		{
			if (!base.Resolve<HumanoidAppearanceComponent>(uid, ref humanoid, true))
			{
				return;
			}
			humanoid.SkinColor = skinColor;
			if (sync)
			{
				base.Dirty(humanoid, null);
			}
		}

		// Token: 0x06000C37 RID: 3127 RVA: 0x00028770 File Offset: 0x00026970
		[NullableContext(2)]
		public void SetBaseLayerId(EntityUid uid, HumanoidVisualLayers layer, string id, bool sync = true, HumanoidAppearanceComponent humanoid = null)
		{
			if (!base.Resolve<HumanoidAppearanceComponent>(uid, ref humanoid, true))
			{
				return;
			}
			HumanoidAppearanceState.CustomBaseLayerInfo info;
			if (humanoid.CustomBaseLayers.TryGetValue(layer, out info))
			{
				Dictionary<HumanoidVisualLayers, HumanoidAppearanceState.CustomBaseLayerInfo> customBaseLayers = humanoid.CustomBaseLayers;
				HumanoidAppearanceState.CustomBaseLayerInfo value = info;
				value.ID = id;
				customBaseLayers[layer] = value;
			}
			else
			{
				humanoid.CustomBaseLayers[layer] = new HumanoidAppearanceState.CustomBaseLayerInfo(id, null);
			}
			if (sync)
			{
				base.Dirty(humanoid, null);
			}
		}

		// Token: 0x06000C38 RID: 3128 RVA: 0x000287E0 File Offset: 0x000269E0
		[NullableContext(2)]
		public void SetBaseLayerColor(EntityUid uid, HumanoidVisualLayers layer, Color? color, bool sync = true, HumanoidAppearanceComponent humanoid = null)
		{
			if (!base.Resolve<HumanoidAppearanceComponent>(uid, ref humanoid, true))
			{
				return;
			}
			HumanoidAppearanceState.CustomBaseLayerInfo info;
			if (humanoid.CustomBaseLayers.TryGetValue(layer, out info))
			{
				Dictionary<HumanoidVisualLayers, HumanoidAppearanceState.CustomBaseLayerInfo> customBaseLayers = humanoid.CustomBaseLayers;
				HumanoidAppearanceState.CustomBaseLayerInfo value = info;
				value.Color = color;
				customBaseLayers[layer] = value;
			}
			else
			{
				humanoid.CustomBaseLayers[layer] = new HumanoidAppearanceState.CustomBaseLayerInfo(null, color);
			}
			if (sync)
			{
				base.Dirty(humanoid, null);
			}
		}

		// Token: 0x06000C39 RID: 3129 RVA: 0x00028848 File Offset: 0x00026A48
		[NullableContext(2)]
		public void SetSex(EntityUid uid, Sex sex, bool sync = true, HumanoidAppearanceComponent humanoid = null)
		{
			if (!base.Resolve<HumanoidAppearanceComponent>(uid, ref humanoid, true) || humanoid.Sex == sex)
			{
				return;
			}
			Sex oldSex = humanoid.Sex;
			humanoid.Sex = sex;
			base.RaiseLocalEvent<SexChangedEvent>(uid, new SexChangedEvent(oldSex, sex), false);
			if (sync)
			{
				base.Dirty(humanoid, null);
			}
		}

		// Token: 0x04000C2B RID: 3115
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000C2C RID: 3116
		[Dependency]
		private readonly MarkingManager _markingManager;

		// Token: 0x04000C2D RID: 3117
		public const string DefaultSpecies = "Human";

		// Token: 0x04000C2E RID: 3118
		public const string DefaultVoice = "Garithos";

		// Token: 0x04000C2F RID: 3119
		public static readonly Dictionary<Sex, string> DefaultSexVoice = new Dictionary<Sex, string>
		{
			{
				Sex.Male,
				"Eugene"
			},
			{
				Sex.Female,
				"Kseniya"
			},
			{
				Sex.Unsexed,
				"Xenia"
			}
		};
	}
}
