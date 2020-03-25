using FluidSharp.Layouts;
using FluidSharp.Widgets;
using SkiaSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.Touch
{

    public class GestureArena
    {

        public DateTime Started = DateTime.Now;
        public long InitialPointerId;

        public List<GestureDetector> DetectorOrder = new List<GestureDetector>();
        public ConcurrentDictionary<GestureDetector, HitTestHit> Detectors = new ConcurrentDictionary<GestureDetector, HitTestHit>();

        private ConcurrentDictionary<long, SKPoint> StartLocationsOnDevice = new ConcurrentDictionary<long, SKPoint>();
        private ConcurrentDictionary<long, SKPoint> StartLocationsInView = new ConcurrentDictionary<long, SKPoint>();
        private ConcurrentDictionary<long, DateTime> StartTimes = new ConcurrentDictionary<long, DateTime>();

        public GestureArena(List<HitTestHit> hits, long pointerId)
        {

            InitialPointerId = pointerId;

            foreach (var hit in hits)
                if (hit.Widget is GestureDetector gestureDetector)
                {
                    DetectorOrder.Add(gestureDetector);
                    Detectors.TryAdd(gestureDetector, hit);
                }

            if (Detectors.Count == 1)
            {
                var detector = DetectorOrder[0];
                detector.Invoke(detector.OnWin);
            }

        }

        public void Touch(long pointerId, TouchActionType type, SKPoint locationOnDevice, SKPoint locationInView, bool isInContact, out bool isCompleted)
        {

            var time = DateTime.Now;
            var isinitialpointer = pointerId == InitialPointerId;

            switch (type)
            {
                case TouchActionType.Pressed:
                    {

                        StartLocationsOnDevice.AddOrUpdate(pointerId, locationOnDevice, (i, p) => locationOnDevice);
                        StartLocationsInView.AddOrUpdate(pointerId, locationInView, (i, p) => locationInView);
                        StartTimes.AddOrUpdate(pointerId, time, (i, t) => time);

                        foreach (var detector in DetectorOrder)
                            if (Detectors.TryGetValue(detector, out var hit))
                            {
                                var locationInWidget = hit.LocationInWidget;
                                if (detector.IsMultiTouch || isinitialpointer)
                                    detector.Pressed(locationInWidget);
                            }

                    }
                    break;
                case TouchActionType.Moved:
                    {

                        var startlocationOnDevice = StartLocationsOnDevice.GetOrAdd(pointerId, locationOnDevice);
                        var startlocationInView = StartLocationsInView.GetOrAdd(pointerId, locationInView);
                        var starttime = StartTimes.GetOrAdd(pointerId, DateTime.Now);

                        var haswon = Detectors.Count == 1;
                        foreach (var detector in DetectorOrder)
                            if (Detectors.TryGetValue(detector, out var hit))
                            {

                                var movement = new GestureDetector.Movement(hit, startlocationOnDevice, locationOnDevice, startlocationInView, locationInView, starttime, time, haswon, isinitialpointer);

                                //System.Diagnostics.Debug.WriteLine($"{detector}");

                                if (detector.IsMultiTouch || isinitialpointer)
                                {
                                    var move = detector.Move(movement);
                                    if (move.win)
                                    {
                                        detector.Invoke(detector.OnWin);
                                        var current = new List<GestureDetector>(Detectors.Keys);
                                        Detectors.Clear();
                                        foreach (var candidate in current)
                                        {
                                            if (candidate == detector)
                                                Detectors.TryAdd(detector, hit);
                                            else
                                                candidate.Cancelled();
                                        }
                                        break;
                                    }
                                    else if (move.loose)
                                    {
                                        detector.Cancelled();
                                        Detectors.TryRemove(detector, out _);
                                    }
                                }
                            }

                    }
                    break;
                case TouchActionType.Released:
                    {

                        foreach (var detector in DetectorOrder)
                            if (Detectors.TryGetValue(detector, out var hit))
                            {
                                if (detector.IsMultiTouch || isinitialpointer)
                                {
                                    detector.Released(DateTime.Now.Subtract(Started));
                                    Detectors.TryRemove(detector, out _);
                                }
                            }

                    }
                    break;

                case TouchActionType.Cancelled:
                    {

                        foreach (var detector in DetectorOrder)
                            if (Detectors.TryGetValue(detector, out var hit))
                            {
                                if (detector.IsMultiTouch || isinitialpointer)
                                {
                                    detector.Cancelled();
                                    Detectors.TryRemove(detector, out _);
                                }
                            }

                    }
                    break;
            }


            isCompleted = Detectors.Count == 0;

        }

    }
}
