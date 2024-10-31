using LBoL.Base;
using LBoL.ConfigData;
using LBoL.EntityLib.Cards.Neutral.NoColor;
using LBoL.EntityLib.Exhibits.Shining;
using LBoL.EntityLib.UltimateSkills;
using LBoL_Doremy.DoremyChar.Cards.Basic;
using LBoL_Doremy.DoremyChar.Cards.Common;
using LBoL_Doremy.DoremyChar.Exhibits;
using LBoL_Doremy.DoremyChar.Ults;
using LBoL_Doremy.RootTemplates;
using LBoL_Doremy.StaticResources;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using UnityEngine;

namespace LBoL_Doremy.DoremyChar.DoremyPU
{
    public sealed class DoremyCavalierDef : DPlayerDef
    {
        public static List<ManaColor> offColors = new List<ManaColor>() { ManaColor.Red, ManaColor.Black, ManaColor.Green, ManaColor.Colorless };

        public const string Name = "Doremy";

        public const string Color = "#b24ac4";//"#97269b";

        public override IdContainer GetId() => this.SelfId();

        public override LocalizationOption LoadLocalization() => Loc.PlayerUnitBatchLoc.AddEntity(this);

        public override PlayerImages LoadPlayerImages()
        {
            var pi = new PlayerImages();
            pi.AutoLoad(Name, (s) => ResourceLoader.LoadSprite(s, Sources.playerImgsSource, ppu: 100, 1, FilterMode.Bilinear, generateMipMaps: true), (s) => ResourceLoader.LoadSpriteAsync(s, Sources.playerImgsSource));
            return pi;
        }

        public override PlayerUnitConfig MakeConfig()
        {

            var con = new PlayerUnitConfig(
                Id: "",
                ShowOrder: 0,
                Order: 0,
                UnlockLevel: 0,
                ModleName: nameof(DoremyCavalier),
                NarrativeColor: Color,
                IsSelectable: true,
                // 2do TEMP
                MaxHp: 69,
                InitialMana: new ManaGroup() { White = 2, Blue = 2 },
                InitialMoney: 42,
                InitialPower: 34,
                // 2do TEMP
                UltimateSkillA: nameof(DoremyCavalierUUlt),
                UltimateSkillB: nameof(DoremyCavalierWUlt),
                ExhibitA: nameof(DoremyCavalierUEx),
                ExhibitB: nameof(DoremyCavalierWEx),
                DeckA: new string[] { nameof(Shoot), nameof(Shoot), nameof(Boundary), nameof(Boundary), 
                    nameof(DoremyCavalierAttackU), nameof(DoremyCavalierAttackU),
                    nameof(DoremyCavalierDefenseW), nameof(DoremyCavalierDefenseW), nameof(DoremyCavalierDefenseW), 
                    nameof(DoremyDreamblast) },
                DeckB: new string[] { nameof(Shoot), nameof(Shoot), nameof(Boundary), nameof(Boundary),
                    nameof(DoremyCavalierAttackW), nameof(DoremyCavalierAttackW),
                    nameof(DoremyCavalierDefenseU), nameof(DoremyCavalierDefenseU), nameof(DoremyCavalierDefenseU),
                    nameof(DoremySleepyStrikes) },
                // 2do diff
                DifficultyA: 2,
                DifficultyB: 1);
            return con;
        }
    }

    [EntityLogic(typeof(DoremyCavalierDef))]
    public sealed class DoremyCavalier : DPlayer
    { }
}
