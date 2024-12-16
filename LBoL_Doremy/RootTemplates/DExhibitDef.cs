using LBoL.ConfigData;
using LBoL_Doremy.StaticResources;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoL_Doremy.RootTemplates
{
    public abstract class DExhibitDef : ExhibitTemplate
    {
        public const string inactive = "Inactive";
        public override IdContainer GetId() => this.SelfId();

        public override LocalizationOption LoadLocalization() => Loc.ExhibitsBatchLoc.AddEntity(this);

        public override ExhibitSprites LoadSprite()
        {
            var exS = new ExhibitSprites(ResourceLoader.LoadSprite(GetId() + ".png", Sources.exAndBomb));

            return exS;
        }

    }
}
