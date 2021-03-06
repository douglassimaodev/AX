﻿using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using XLabs.Ioc;
using XLabs.Platform.Device;

namespace InstaXamarinMobile.Droid
{
    [Activity(Label = "InstaXamarinMobile", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            var resolverContainer = new SimpleContainer();
            resolverContainer.Register<IDevice>(t => AndroidDevice.CurrentDevice);
            Resolver.SetResolver(resolverContainer.GetResolver());

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            FormsPlugin.Iconize.Droid.IconControls.Init(Resource.Id.toolbar, Resource.Id.sliding_tabs);

            LoadApplication(new App());

            //LoadApplication(UXDivers.Gorilla.Droid.Player.CreateApplication(
            //   ApplicationContext,
            //   new UXDivers.Gorilla.Config("Gorilla on WelitonDesktop")));

        }
    }
}

