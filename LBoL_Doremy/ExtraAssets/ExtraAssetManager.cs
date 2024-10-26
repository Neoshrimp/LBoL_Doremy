using Cysharp.Threading.Tasks;
using LBoL.Presentation;
using LBoL_Doremy.StaticResources;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LBoL_Doremy.ExtraAssets
{
    public static class ExtraAssetManager
    {

        public static DoremyAssets DoremyAssets { get; private set; }
        public static void RegisterLoad()
        {
            EntityManager.AddPostLoadAction(DoLoadAsync);
        }

        private static async void DoLoadAsync()
        {
            DoremyAssets = new DoremyAssets();
            DoremyAssets.CreatedIcon =  await ResourceLoader.LoadSpriteAsync("CreatedIcon.png", Sources.extraImgs);
        }
    }

    public class DoremyAssets
    {
        private Sprite createdIcon;
        private Sprite dreamLayerDepth;

        public Sprite CreatedIcon { get => createdIcon; set => createdIcon = value; }
        public Sprite DreamLayerDepth { get => dreamLayerDepth; set => dreamLayerDepth = value; }
    }
}
