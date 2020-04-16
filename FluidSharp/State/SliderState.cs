//using FluidSharp.Animations;
//using FluidSharp.Touch;
//using FluidSharp.Widgets;
//using FluidSharp.Widgets.CrossPlatform;
//using SkiaSharp;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;

//namespace FluidSharp.State
//{
//    public class SliderState
//    {

//        private object Context;

//        public SKPoint Location;

//        public SliderState() { }

//        public bool IsContext(object context)
//        {
//            if (Context == null) return context == null;
//            return Context.Equals(context);
//        }

//        public async Task SetTouchPoint(object context, SKPoint location, VisualState visualState)
//        {

//            Context = context;
//            Location = location;

//            await visualState.RequestRedraw();

//        }

//    }
//}
