using Cysharp.Threading.Tasks;
using HarmonyLib;
using LBoL.Presentation;
using LBoL_Doremy.StaticResources;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using TMPro.SpriteAssetUtilities;
using UnityEngine;

namespace LBoL_Doremy.ExtraAssets
{
    public static class AssetManager
    {
        static DoremyAssets doremyAssets;
        public static DoremyAssets DoremyAssets 
        { 
            get 
            { 
                if (!finishedLoading) 
                {
                    Log.LogError("Additional Doremy assets did not finish loading.");
                } 
                return doremyAssets; 
            } 
            private set => doremyAssets = value; 
        }

        /*public static void RegisterLoad()
        {
            EntityManager.AddPostLoadAction(DoLoadAsync);
        }*/

        static bool finishedLoading = false;

        public static async void DoLoadAsync()
        {
            finishedLoading = false;
            doremyAssets = new DoremyAssets();
            doremyAssets.createdIcon =  await ResourceLoader.LoadSpriteAsync("CreatedIcon.png", Sources.extraImgs);
            doremyAssets.dreamLevel = await ResourceLoader.LoadSpriteAsync("DreamLevel.png", Sources.extraImgs);

            doremyAssets.purpleBar = await ResourceLoader.LoadSpriteAsync("PurpleBar.png", Sources.extraImgs);
            finishedLoading = true;
        }


    }

    public class DoremyAssets
    {
        public Sprite createdIcon;
        public Sprite dreamLevel;
        public Sprite purpleBar;


    }
}
