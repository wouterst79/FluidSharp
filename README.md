# FluidSharp
FluidSharp is a high performance mobile first multi-platform UI layout framework based on Skia.


## Using FluidSharp

SkiaSharp.TextBlocks is available as a convenient NuGet package, to use install the package like this:

```
nuget install FluidSharp
```


## Sample

```
    public abstract class Sample : IWidgetSource
    {
        public abstract string Name { get; }
        public abstract Widget MakeWidget(VisualState visualState);
    }

    public class HelloWorld : Sample
    {
        public override string Name => "Hello world";
        public override Widget MakeWidget(VisualState visualState)
        {
            return new Text(new Font(14), SKColors.Black, "Hello World!");
        }
    }

    fluidWidgetView1.WidgetSource = new Sample();
    
```

See the Samples folder for more samples.
