using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace CN_WPF
{
    /// <summary>
    /// Helpers to animate framework elements in specific ways
    /// </summary>
    public static class FrameworkElementAnimations
    {
        #region Slide In / Out

        /// <summary>
        /// Slides an element in
        /// </summary>
        /// <param name="element">The element to animate</param>
        /// <param name="direction">The direction of the slide</param>
        /// <param name="seconds">The time the animation will take</param>
        /// <param name="keepMargin">Whether to keep the element at the same width during animation</param>
        /// <param name="size">The animation width/height to animate to. If not specified the elements size is used</param>
        /// <param name="firstLoad">Indicates if this is the first load</param>
        /// <returns></returns>
        public static async Task SlideAndFadeInAsync(this FrameworkElement element, AnimationSlideInDirection direction, bool firstLoad, float seconds = 0.3f, bool keepMargin = true, int size = 0)
        {
            // Create the storyboard
            var sb = new Storyboard();

            // Slide in the correct direction
            switch (direction)
            {
                // Add slide from left animation
                case AnimationSlideInDirection.Left:
                    sb.AddSlideFromLeft(seconds, size == 0 ? element.ActualWidth : size, keepMargin: keepMargin);
                    break;
                // Add slide from right animation
                case AnimationSlideInDirection.Right:
                    sb.AddSlideFromRight(seconds, size == 0 ? element.ActualWidth : size, keepMargin: keepMargin);
                    break;
                // Add slide from top animation
                case AnimationSlideInDirection.Top:
                    sb.AddSlideFromTop(seconds, size == 0 ? element.ActualHeight : size, keepMargin: keepMargin);
                    break;
                // Add slide from bottom animation
                case AnimationSlideInDirection.Bottom:
                    sb.AddSlideFromBottom(seconds, size == 0 ? element.ActualHeight : size, keepMargin: keepMargin);
                    break;
            }
            // Add fade in animation
            sb.AddFadeIn(seconds , Math.Abs(seconds) > 1e-5 && firstLoad);

            // Start animating
            sb.Begin(element);

            // Make page visible only if we are animating or its the first load
            if (Math.Abs(seconds) > 1e-5 || firstLoad)
                element.Visibility = Visibility.Visible;

            // Wait for it to finish
            await Task.Delay((int)(seconds * 1000));
        }

        /// <summary>
        /// Slides an element out
        /// </summary>
        /// <param name="element">The element to animate</param>
        /// <param name="direction">The direction of the slide (this is for the reverse slide out action, so Left would slide out to left)</param>
        /// <param name="seconds">The time the animation will take</param>
        /// <param name="keepMargin">Whether to keep the element at the same width during animation</param>
        /// <param name="size">The animation width/height to animate to. If not specified the elements size is used</param>
        /// <returns></returns>
        public static async Task SlideAndFadeOutAsync(this FrameworkElement element, AnimationSlideInDirection direction, float seconds = 0.3f, bool keepMargin = true, int size = 0)
        {
            // Create the storyboard
            var sb = new Storyboard();

            // Slide in the correct direction
            switch (direction)
            {
                // Add slide to left animation
                case AnimationSlideInDirection.Left:
                    sb.AddSlideToLeft(seconds, size == 0 ? element.ActualWidth : size, keepMargin: keepMargin);
                    break;
                // Add slide to right animation
                case AnimationSlideInDirection.Right:
                    sb.AddSlideToRight(seconds, size == 0 ? element.ActualWidth : size, keepMargin: keepMargin);
                    break;
                // Add slide to top animation
                case AnimationSlideInDirection.Top:
                    sb.AddSlideToTop(seconds, size == 0 ? element.ActualHeight : size, keepMargin: keepMargin);
                    break;
                // Add slide to bottom animation
                case AnimationSlideInDirection.Bottom:
                    sb.AddSlideToBottom(seconds, size == 0 ? element.ActualHeight : size, keepMargin: keepMargin);
                    break;
            }

            // Add fade in animation
            sb.AddFadeOut(seconds);

            // Start animating
            sb.Begin(element);

            // Make page visible only if we are animating
            if (Math.Abs(seconds) > 1e-5)
                element.Visibility = Visibility.Visible;

            // Wait for it to finish
            await Task.Delay((int)(seconds * 1000));

            // Make element invisible
            if (Math.Abs(element.Opacity) < 1e-5)
                element.Visibility = Visibility.Hidden;
        }

        #endregion

        #region Fade In / Out

        /// <summary>
        /// Fades an element in
        /// </summary>
        /// <param name="element">The element to animate</param>
        /// <param name="seconds">The time the animation will take</param>
        /// <param name="firstLoad">Indicates if this is the first load</param>
        /// <returns></returns>
        public static async Task FadeInAsync(this FrameworkElement element, bool firstLoad, float seconds = 0.3f)
        {
            // Create the storyboard
            var sb = new Storyboard();

            // Add fade in animation
            sb.AddFadeIn(seconds);

            // Start animating
            sb.Begin(element);

            // Make page visible only if we are animating or its the first load
            if (Math.Abs(seconds) > 1e-5 || firstLoad)
                element.Visibility = Visibility.Visible;

            // Wait for it to finish
            await Task.Delay((int)(seconds * 1000));
        }

        /// <summary>
        /// Fades out an element
        /// </summary>
        /// <param name="element">The element to animate</param>
        /// <param name="seconds">The time the animation will take</param>
        /// <param name="firstLoad">Indicates if this is the first load</param>
        /// <returns></returns>
        public static async Task FadeOutAsync(this FrameworkElement element, float seconds = 0.3f)
        {
            // Create the storyboard
            var sb = new Storyboard();

            // Add fade in animation
            sb.AddFadeOut(seconds);

            // Start animating
            sb.Begin(element);

            // Make page visible only if we are animating or its the first load
            if (Math.Abs(seconds) > 1e-5)
                element.Visibility = Visibility.Visible;

            // Wait for it to finish
            await Task.Delay((int)(seconds * 1000));

            // Fully hide the element
            element.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region Rotate CW / CCW

        /// <summary>
        /// Rotate an element CW
        /// </summary>
        /// <param name="element">The element to animate</param>
        /// <param name="seconds">The time the animation will take</param>
        /// <param name="firstLoad">Indicates if this is the first load</param>
        /// <returns></returns>
        public static async Task RotateCWAsync(this FrameworkElement element, bool firstLoad, float seconds = 0.3f)
        {
            element.Visibility = Visibility.Visible;
            // Create the storyboard
            var sb = new Storyboard();

            // Add fade in animation
            sb.AddRotateCW(seconds);

            // Start animating
            sb.Begin(element);
            
            // Wait for it to finish
            await Task.Delay((int)(seconds * 1000));
        }

        /// <summary>
        /// Rotate an element CCW
        /// </summary>
        /// <param name="element">The element to animate</param>
        /// <param name="seconds">The time the animation will take</param>
        /// <returns></returns>
        public static async Task RotateCCWAsync(this FrameworkElement element, float seconds = 0.3f)
        {
            element.Visibility = Visibility.Visible;
            // Create the storyboard
            var sb = new Storyboard();

            // Add fade in animation
            sb.AddRotateCCW(seconds);

            // Start animating
            sb.Begin(element);
            
            // Wait for it to finish
            await Task.Delay((int)(seconds * 1000));

            // Fully hide the element
//            element.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region ScaleY Expand / Shrink

        /// <summary>
        /// Scale an element y direction expand from 0 to 1
        /// </summary>
        /// <param name="element">The element to animate</param>
        /// <param name="seconds">The time the animation will take</param>
        /// <param name="firstLoad">Indicates if this is the first load</param>
        /// <returns></returns>
        public static async Task ScaleYExpand(this FrameworkElement element, bool firstLoad, float seconds = 0.3f)
        {
            element.Visibility = Visibility.Collapsed;
            // Create the storyboard
            var sb = new Storyboard();

            // Add fade in animation
            sb.AddScaleYExpand(seconds);

            // Start animating
            sb.Begin(element);

            if (Math.Abs(seconds) > 1e-5 || firstLoad)
                element.Visibility = Visibility.Visible;

            // Wait for it to finish
            await Task.Delay((int)(seconds * 1000));
        }

        /// <summary>
        /// Scale an element y direction shrink from 0 to 1
        /// </summary>
        /// <param name="element">The element to animate</param>
        /// <param name="seconds">The time the animation will take</param>
        /// <returns></returns>
        public static async Task ScaleYShrink(this FrameworkElement element, float seconds = 0.3f)
        {
            element.Visibility = Visibility.Visible;
            // Create the storyboard
            var sb = new Storyboard();

            // Add fade in animation
            sb.AddScaleYShrink(seconds);

            // Start animating
            sb.Begin(element);

            // Wait for it to finish
            await Task.Delay((int)(seconds * 1000));

            // Fully hide the element
                        element.Visibility = Visibility.Collapsed;
        }

        #endregion
        #region ScrollView Container Expand / Shrink

        /// <summary>
        /// Change a ScrollView Container's Height from 0 to 1
        /// Height is determined by its children's actual height
        /// </summary>
        /// <param name="element">The element to animate</param>
        /// <param name="seconds">The time the animation will take</param>
        /// <param name="firstLoad">Indicates if this is the first load</param>
        /// <returns></returns>
        public static async Task ScrollViewExpand(this FrameworkElement element, bool firstLoad, float seconds = 0.3f)
        {
            element.Visibility = Visibility.Visible;
            // Create the storyboard
            var sb = new Storyboard();

            // Add fade in animation
            sb.AddScrollViewExpand(seconds, element);

            // Start animating
            sb.Begin(element);
            
            // Wait for it to finish
            await Task.Delay((int)(seconds * 1000));
        }

        /// <summary>
        /// Change a ScrollView Container's Height from 1 to 0
        /// Height is determined by its children's actual height
        /// </summary>
        /// <param name="element">The element to animate</param>
        /// <param name="seconds">The time the animation will take</param>
        /// <returns></returns>
        public static async Task ScrollViewShrink(this FrameworkElement element, float seconds = 0.3f)
        {
            element.Visibility = Visibility.Visible;
            // Create the storyboard
            var sb = new Storyboard();

            // Add fade in animation
            sb.AddScrollViewShrink(seconds, element);

            // Start animating
            sb.Begin(element);

            // Wait for it to finish
            await Task.Delay((int)(seconds * 1000));
        }

        #endregion
        #region Marquee

        /// <summary>
        /// Animates a marquee style element
        /// The structure should be:
        /// [Border ClipToBounds="True"]
        ///   [Border local:AnimateMarqueeProperty.Value="True"]
        ///      [Content HorizontalAlignment="Left"]
        ///   [/Border]
        /// [/Border]
        /// </summary>
        /// <param name="element">The element to animate</param>
        /// <param name="seconds">The time the animation will take</param>
        /// <returns></returns>
        public static void MarqueeAsync(this FrameworkElement element, float seconds = 3f)
        {
            // Create the storyboard
//            var sb = new Storyboard();
//
//            // Run until element is unloaded
//            var unloaded = false;
//
//            // Monitor for element unloading
//            element.Unloaded += (s, e) => unloaded = true;
            
        }

        #endregion
    }
}
