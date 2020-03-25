using FluidSharp.Paint;
using SkiaSharp.TextBlocks.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp
{

    public class Device
    {

        public FlowDirection FlowDirection = FlowDirection.LeftToRight;

        public float FontSizeScale = 1f;
        public float DefaultScale = 1f;

        public bool PixelRounding = false;

        private ImagePainter imagePainter;
        public ImagePainter ImagePainter => imagePainter ?? (imagePainter = new ImagePainter(this));

        public static IImageSource SharedImageSource;
        public IImageSource ImageSource => SharedImageSource;

    }

}
