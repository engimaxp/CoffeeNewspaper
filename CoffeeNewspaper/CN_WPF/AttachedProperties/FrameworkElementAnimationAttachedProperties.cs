﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace CN_WPF
{
    /// <summary>
    ///     A base class to run any animation method when a boolean is set to true
    ///     and a reverse animation when set to false
    /// </summary>
    /// <typeparam name="Parent"></typeparam>
    public abstract class AnimateBaseProperty<Parent> : BaseAttachedProperty<Parent, bool>
        where Parent : BaseAttachedProperty<Parent, bool>, new()
    {
        public override void OnValueUpdated(DependencyObject sender, object value)
        {
            // Get the framework element
            if (!(sender is FrameworkElement element))
                return;

            // Try and get the already loaded reference
            var alreadyLoadedReference = mAlreadyLoaded.FirstOrDefault(f => Equals(f.Key.Target, sender));

            // Try and get the first load reference

            // Don't fire if the value doesn't change
            if ((bool) sender.GetValue(ValueProperty) == (bool) value && alreadyLoadedReference.Key != null)
                return;

            // On first load...
            if (alreadyLoadedReference.Key == null)
            {
                // Create weak reference
                var weakReference = new WeakReference(sender);

                // Flag that we are in first load but have not finished it
                mAlreadyLoaded[weakReference] = false;

                // Start off hidden before we decide how to animate
                element.Visibility = Visibility.Hidden;

                // Create a single self-unhookable event 
                // for the elements Loaded event
                async void onLoaded(object ss, RoutedEventArgs ee)
                {
                    // Unhook ourselves
                    element.Loaded -= onLoaded;

                    // Slight delay after load is needed for some elements to get laid out
                    // and their width/heights correctly calculated
                    await Task.Delay(5);

                    // Refresh the first load value in case it changed
                    // since the 5ms delay
                    var firstLoadReference = mFirstLoadValue.FirstOrDefault(f => Equals(f.Key.Target, sender));

                    // Do desired animation
                    DoAnimation(element, firstLoadReference.Key != null ? firstLoadReference.Value : (bool) value,
                        true);

                    // Flag that we have finished first load
                    mAlreadyLoaded[weakReference] = true;
                }

                // Hook into the Loaded event of the element
                element.Loaded += onLoaded;
            }
            // If we have started a first load but not fired the animation yet, update the property
            else if (!alreadyLoadedReference.Value)
            {
                mFirstLoadValue[new WeakReference(sender)] = (bool) value;
            }
            else
                // Do desired animation
            {
                DoAnimation(element, (bool) value, false);
            }
        }

        /// <summary>
        ///     The animation method that is fired when the value changes
        /// </summary>
        /// <param name="element">The element</param>
        /// <param name="value">The new value</param>
        protected virtual void DoAnimation(FrameworkElement element, bool value, bool firstLoad)
        {
        }

        #region Protected Properties

        /// <summary>
        ///     True if this is the very first time the value has been updated
        ///     Used to make sure we run the logic at least once during first load
        /// </summary>
        private readonly Dictionary<WeakReference, bool> mAlreadyLoaded = new Dictionary<WeakReference, bool>();

        /// <summary>
        ///     The most recent value used if we get a value changed before we do the first load
        /// </summary>
        private readonly Dictionary<WeakReference, bool> mFirstLoadValue = new Dictionary<WeakReference, bool>();

        #endregion
    }

    /// <summary>
    ///     Fades in an image once the source changes
    /// </summary>
    public class FadeInImageOnLoadProperty : AnimateBaseProperty<FadeInImageOnLoadProperty>
    {
        public override void OnValueUpdated(DependencyObject sender, object value)
        {
            // Make sure we have an image
            if (!(sender is Image image))
                return;

            // If we want to animate in...
            if ((bool) value)
                // Listen for target change
                image.TargetUpdated += Image_TargetUpdatedAsync;
            // Otherwise
            else
                // Make sure we unhooked
                image.TargetUpdated -= Image_TargetUpdatedAsync;
        }

        private async void Image_TargetUpdatedAsync(object sender, DataTransferEventArgs e)
        {
            // Fade in image
            await (sender as Image).FadeInAsync(false);
        }
    }

    /// <summary>
    ///     Animates a framework element sliding it in from the left on show
    ///     and sliding out to the left on hide
    /// </summary>
    public class AnimateSlideInFromLeftProperty : AnimateBaseProperty<AnimateSlideInFromLeftProperty>
    {
        protected override async void DoAnimation(FrameworkElement element, bool value, bool firstLoad)
        {
            if (value)
                // Animate in
                await element.SlideAndFadeInAsync(AnimationSlideInDirection.Left, firstLoad, firstLoad ? 0 : 0.3f,
                    false);
            else
                // Animate out
                await element.SlideAndFadeOutAsync(AnimationSlideInDirection.Left, firstLoad ? 0 : 0.3f, false);
        }
    }

    /// <summary>
    ///     Animates a framework element sliding it in from the right on show
    ///     and sliding out to the right on hide
    /// </summary>
    public class AnimateSlideInFromRightProperty : AnimateBaseProperty<AnimateSlideInFromRightProperty>
    {
        protected override async void DoAnimation(FrameworkElement element, bool value, bool firstLoad)
        {
            if (value)
                // Animate in
                await element.SlideAndFadeInAsync(AnimationSlideInDirection.Right, firstLoad, firstLoad ? 0 : 0.3f,
                    false);
            else
                // Animate out
                await element.SlideAndFadeOutAsync(AnimationSlideInDirection.Right, firstLoad ? 0 : 0.3f, false);
        }
    }

    /// <summary>
    ///     Animates a framework element sliding it in from the right on show
    ///     and sliding out to the right on hide
    /// </summary>
    public class AnimateSlideInFromRightMarginProperty : AnimateBaseProperty<AnimateSlideInFromRightMarginProperty>
    {
        protected override async void DoAnimation(FrameworkElement element, bool value, bool firstLoad)
        {
            if (value)
                // Animate in
                await element.SlideAndFadeInAsync(AnimationSlideInDirection.Right, firstLoad, firstLoad ? 0 : 0.3f,
                    true);
            else
                // Animate out
                await element.SlideAndFadeOutAsync(AnimationSlideInDirection.Right, firstLoad ? 0 : 0.3f, true);
        }
    }

    /// <summary>
    ///     Animates a framework element sliding up from the bottom on show
    ///     and sliding out to the bottom on hide
    /// </summary>
    public class AnimateSlideInFromBottomProperty : AnimateBaseProperty<AnimateSlideInFromBottomProperty>
    {
        protected override async void DoAnimation(FrameworkElement element, bool value, bool firstLoad)
        {
            if (value)
                // Animate in
                await element.SlideAndFadeInAsync(AnimationSlideInDirection.Bottom, firstLoad, firstLoad ? 0 : 0.3f,
                    false);
            else
                // Animate out
                await element.SlideAndFadeOutAsync(AnimationSlideInDirection.Bottom, firstLoad ? 0 : 0.3f, false);
        }
    }

    /// <summary>
    ///     Animates a framework element sliding up from the bottom on load
    ///     if the value is true
    /// </summary>
    public class AnimateSlideInFromBottomOnLoadProperty : AnimateBaseProperty<AnimateSlideInFromBottomOnLoadProperty>
    {
        protected override async void DoAnimation(FrameworkElement element, bool value, bool firstLoad)
        {
            // Animate in
            await element.SlideAndFadeInAsync(AnimationSlideInDirection.Bottom, !value, !value ? 0 : 0.3f, false);
        }
    }

    /// <summary>
    ///     Animates a framework element sliding up from the bottom on show
    ///     and sliding out to the bottom on hide
    ///     NOTE: Keeps the margin
    /// </summary>
    public class AnimateSlideInFromBottomMarginProperty : AnimateBaseProperty<AnimateSlideInFromBottomMarginProperty>
    {
        protected override async void DoAnimation(FrameworkElement element, bool value, bool firstLoad)
        {
            if (value)
                // Animate in
                await element.SlideAndFadeInAsync(AnimationSlideInDirection.Bottom, firstLoad, firstLoad ? 0 : 0.3f,
                    true);
            else
                // Animate out
                await element.SlideAndFadeOutAsync(AnimationSlideInDirection.Bottom, firstLoad ? 0 : 0.3f, true);
        }
    }

    /// <summary>
    ///     Animates a framework element fading in on show
    ///     and fading out on hide
    /// </summary>
    public class AnimateFadeInProperty : AnimateBaseProperty<AnimateFadeInProperty>
    {
        protected override async void DoAnimation(FrameworkElement element, bool value, bool firstLoad)
        {
            if (value)
                // Animate in
                await element.FadeInAsync(firstLoad, firstLoad ? 0 : 0.3f);
            else
                // Animate out
                await element.FadeOutAsync(firstLoad ? 0 : 0.3f);
        }
    }

    /// <summary>
    ///     Animates a scroll view expand from 0 to 1
    ///     its a work around to display smooth expand animation like expander control
    ///     ref https://www.codeproject.com/Articles/248112/Templating-WPF-Expander-Control#animation
    /// </summary>
    public class AnimateScrollViewExpandProperty : AnimateBaseProperty<AnimateScrollViewExpandProperty>
    {
        protected override async void DoAnimation(FrameworkElement element, bool value, bool firstLoad)
        {
            if (value)
                // Animate in
                await element.ScrollViewExpand(firstLoad, firstLoad ? 0 : 0.3f);
            else
                // Animate out
                await element.ScrollViewShrink(firstLoad ? 0 : 0.3f);
        }
    }

    /// <summary>
    ///     Animates a framework element expand from 0 to 1
    /// </summary>
    public class AnimateScaleYExpandProperty : AnimateBaseProperty<AnimateScaleYExpandProperty>
    {
        protected override async void DoAnimation(FrameworkElement element, bool value, bool firstLoad)
        {
            if (value)
                // Animate in
                await element.ScaleYExpand(firstLoad, firstLoad ? 0 : 0.3f);
            else
                // Animate out
                await element.ScaleYShrink(firstLoad ? 0 : 0.3f);
        }
    }

    /// <summary>
    ///     Animates a framework element Rotate 180 cw or -180 ccw
    /// </summary>
    public class AnimateCWProperty : AnimateBaseProperty<AnimateCWProperty>
    {
        protected override async void DoAnimation(FrameworkElement element, bool value, bool firstLoad)
        {
            if (value)
                // ccw out
                await element.RotateCCWAsync(0);
            else
                // cw in
                await element.RotateCWAsync(firstLoad, 0);
        }
    }
}