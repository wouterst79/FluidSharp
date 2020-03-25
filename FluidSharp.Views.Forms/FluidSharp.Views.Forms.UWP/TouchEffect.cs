using FluidSharp.Touch;
using System;
using System.Linq;
using TouchTracking = FluidSharp.Views.Forms;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;
using SkiaSharp;
using FluidSharp.Touch;

// https://github.com/xamarin/xamarin-forms-samples/blob/master/Effects/TouchTrackingEffect/TouchTrackingEffect/TouchTrackingEffect.UWP/TouchEffect.cs

[assembly: ResolutionGroupName("FluidSharp")]
[assembly: ExportEffect(typeof(FluidSharp.UWP.Renderers.TouchEffect), "TouchEffect")]
namespace FluidSharp.UWP.Renderers
{


    public class TouchEffect : PlatformEffect
    {
        FrameworkElement frameworkElement;
        TouchTracking.TouchEffect effect;
        Action<Element, TouchActionEventArgs> onTouchAction;

        protected override void OnAttached()
        {
            // Get the Windows FrameworkElement corresponding to the Element that the effect is attached to
            frameworkElement = Control == null ? Container : Control;

            // Get access to the TouchEffect class in the .NET Standard library
            effect = (TouchTracking.TouchEffect)Element.Effects.
                        FirstOrDefault(e => e is TouchTracking.TouchEffect);

            if (effect != null && frameworkElement != null)
            {
                // Save the method to call on touch events
                onTouchAction = effect.OnTouchAction;

                // Set event handlers on FrameworkElement
                frameworkElement.PointerEntered += OnPointerEntered;
                frameworkElement.PointerPressed += OnPointerPressed;
                frameworkElement.PointerMoved += OnPointerMoved;
                frameworkElement.PointerReleased += OnPointerReleased;
                frameworkElement.PointerExited += OnPointerExited;
                frameworkElement.PointerCanceled += OnPointerCancelled;
            }
        }

        protected override void OnDetached()
        {
            if (onTouchAction != null)
            {
                // Release event handlers on FrameworkElement
                frameworkElement.PointerEntered -= OnPointerEntered;
                frameworkElement.PointerPressed -= OnPointerPressed;
                frameworkElement.PointerMoved -= OnPointerMoved;
                frameworkElement.PointerReleased -= OnPointerReleased;
                frameworkElement.PointerExited -= OnPointerEntered;
                frameworkElement.PointerCanceled -= OnPointerCancelled;
            }
        }

        void OnPointerEntered(object sender, PointerRoutedEventArgs args)
        {
            CommonHandler(sender, TouchActionType.Entered, args);
        }

        void OnPointerPressed(object sender, PointerRoutedEventArgs args)
        {
            CommonHandler(sender, TouchActionType.Pressed, args);

            // Check setting of Capture property
            if (effect.Capture)
            {
                (sender as FrameworkElement).CapturePointer(args.Pointer);
            }
        }

        void OnPointerMoved(object sender, PointerRoutedEventArgs args)
        {
            CommonHandler(sender, TouchActionType.Moved, args);
        }

        void OnPointerReleased(object sender, PointerRoutedEventArgs args)
        {
            CommonHandler(sender, TouchActionType.Released, args);
        }

        void OnPointerExited(object sender, PointerRoutedEventArgs args)
        {
            CommonHandler(sender, TouchActionType.Exited, args);
        }

        void OnPointerCancelled(object sender, PointerRoutedEventArgs args)
        {
            CommonHandler(sender, TouchActionType.Cancelled, args);
        }

        void CommonHandler(object sender, TouchActionType touchActionType, PointerRoutedEventArgs args)
        {


            var pointerPoint = args.GetCurrentPoint(sender as UIElement);
            var viewpoint = pointerPoint.Position;

            var transform = (sender as UIElement).TransformToVisual(Window.Current.Content);
            var windowpoint = transform.TransformPoint(viewpoint);

            onTouchAction(Element, new TouchActionEventArgs(args.Pointer.PointerId,
                                                            touchActionType,
                                                            new SKPoint((float)windowpoint.X, (float)windowpoint.Y),
                                                            new SKPoint((float)viewpoint.X, (float)viewpoint.Y),
                                                            args.Pointer.IsInContact));
        }
    }
}
