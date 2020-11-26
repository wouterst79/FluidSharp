#if DEBUG
//#define PRINTEVENTS
#endif
using System;
using System.Collections.Generic;
using FluidSharp.Widgets.Native;
using SkiaSharp;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using FluidSharp.Engine;

namespace FluidSharp.Interop
{

    public abstract class NativeViewManagerBase<TNativeControlBaseType> : INativeViewManager
    {

        private Dictionary<Type, Func<NativeViewWidget, TNativeControlBaseType, bool>> IsMatches = new Dictionary<Type, Func<NativeViewWidget, TNativeControlBaseType, bool>>();
        private Dictionary<Type, Func<NativeViewWidget, TNativeControlBaseType>> Makes = new Dictionary<Type, Func<NativeViewWidget, TNativeControlBaseType>>();


        public abstract IEnumerable<TNativeControlBaseType> GetChildren();
        public abstract void RegisterNewControl(TNativeControlBaseType newControl);
        public abstract SKSize GetControlSize(TNativeControlBaseType control);
        
        public abstract void UpdateControl(TNativeControlBaseType control, NativeViewWidget nativeViewWidget, SKRect rect);
        public abstract void SetControlVisible(TNativeControlBaseType control, bool visible);


        public void RegisterNativeView<TNativeViewWidget, TNativeControl>(Func<TNativeViewWidget, TNativeControl, bool> ismatch, Func<TNativeViewWidget, TNativeControl> create)
            where TNativeViewWidget : NativeViewWidget
            where TNativeControl : TNativeControlBaseType
        {
            var nativeviewwidgettype = typeof(TNativeViewWidget);

            Func<NativeViewWidget, TNativeControlBaseType, bool> castismatch = (w, b) => ismatch((TNativeViewWidget)w, (TNativeControl)b);
            IsMatches[nativeviewwidgettype] = castismatch;

            Func<NativeViewWidget, TNativeControlBaseType> castcreate = (w) => create((TNativeViewWidget)w);
            Makes[nativeviewwidgettype] = castcreate;

        }

        public bool MatchesWidget(NativeViewWidget nativeViewWidget, TNativeControlBaseType control)
        {
            var type = nativeViewWidget.GetType();
            var ismatch = IsMatches[type];
            return ismatch(nativeViewWidget, control);
        }

        public TNativeControlBaseType MakeControlForWidget(NativeViewWidget nativeViewWidget)
        {
            var type = nativeViewWidget.GetType();

            if (!Makes.ContainsKey(type))
                throw new EngineException($"NativeView not registered for type {type.Name}");

            var make = Makes[type];
            var newchild = make(nativeViewWidget);

            SetControlVisible(newchild, false);

            RegisterNewControl(newchild);

            Debug.WriteLine($"new native child control created");

            return newchild;
        }

        private List<TNativeControlBaseType> Children;
        private HashSet<TNativeControlBaseType> Touched = new HashSet<TNativeControlBaseType>();
        public void PaintStarted()
        {

            // capture current list of controls
            Children = new List<TNativeControlBaseType>(GetChildren());
            Touched.Clear();

#if PRINTEVENTS
            Debug.WriteLine($"paint started: {Children.Count} children");
#endif

        }

        private TNativeControlBaseType GetNativeControl(NativeViewWidget nativeViewWidget, bool touch)
        {

            TNativeControlBaseType result = default;

            // find the control matching the widget
            foreach (var candidate in Children)
                if (MatchesWidget(nativeViewWidget, candidate))
                {
                    if (touch) Touched.Add(candidate);
                    result = candidate;
                    break;
                }

            if (result == null)
            {
                // not found, create it now
                result = MakeControlForWidget(nativeViewWidget);
                Children.Add(result);
                Touched.Add(result);
            }

            return result;

        }

        public SKSize Measure(NativeViewWidget nativeViewWidget, SKSize boundaries)
        {
            var control = GetNativeControl(nativeViewWidget, false);
            var size = GetControlSize(control);
            return size;
        }

        public void UpdateNativeView(NativeViewWidget nativeViewWidget, SKRect rect)
        {

            var control = GetNativeControl(nativeViewWidget, true);

            UpdateControl(control, nativeViewWidget, rect);

            //SetControlVisible(control, true);

        }

        public void PaintCompleted()
        {
            // hide all children that weren't "painted" in this layout run
            foreach (var child in Children)
                SetControlVisible(child, Touched.Contains(child));
//            if (!Touched.Contains(child))
        }

    }

}

