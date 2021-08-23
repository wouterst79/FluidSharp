using FluidSharp.Layouts;
using FluidSharp.State;
using FluidSharp.Widgets;
using FluidSharp.Widgets.CrossPlatform;
using SkiaSharp;
using SkiaSharp.TextBlocks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FluidSharp.Samples.All
{
    public class SampleApp : IWidgetSource
    {

        // Platform Styles
        public List<PlatformStyle> PlatformStyles;
        public PlatformStyle SelectedPlatformStyle;

        // Samples
        public List<Sample> Samples = new List<Sample>();
        public Sample SelectedSample;

        public SampleApp()
        {

            // Populate Platform Styles
            PlatformStyles = new List<PlatformStyle>() { PlatformStyle.Material, PlatformStyle.Cupertino, PlatformStyle.UWP };
            SelectedPlatformStyle = PlatformStyles[0];

            // Dynamically load all samples from this assembly
            var assembly = GetType().Assembly;
            var sampletype = typeof(Sample);
            foreach (var candidate in assembly.GetTypes())
                if (sampletype.IsAssignableFrom(candidate) && candidate != sampletype)
                {
                    var constructor = candidate.GetConstructor(new Type[] { });
                    var sample = (Sample)constructor.Invoke(null);
                    Samples.Add(sample);
                }

            // Sort by name
            Samples.Sort((s1, s2) => s1.Name.CompareTo(s2.Name));

            // Select the first sample by default
            SelectedSample = Samples[0];

        }

        public Widget MakeWidget(VisualState visualState)
        {
            return new Layout()
            {

                Margin = new Margins(10, 40),

                Columns =
                {
                    new LayoutSize.Available(.33f),
                    new LayoutSize.Remaining()
                },

                Cells =
                {

                    // list of samples
                    new LayoutCell(0, 0,
                        ShortList.Make(SelectedPlatformStyle, visualState, Samples,
                            Samples, sample => SelectedSample == sample, SKColors.LightGray, async sample => SelectedSample = sample,
                            (sample) => new Text(new Font(14), SKColors.Black, sample.Name)
                        )
                    ),

                    // sample content
                    new LayoutCell(1, 0, SelectedSample.MakeWidget(visualState))

                }

            };
        }

    }
}
