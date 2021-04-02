#if DEBUG
#define PRINTEVENTS
#endif
using FluidSharp.Widgets.Native;
using SkiaSharp;
using SkiaSharp.TextBlocks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Views;
using Android.Widget;
using Android.Content;
using Android.Util;
using Android.Content.Res;
using Android.Runtime;
using Android.App;

namespace FluidSharp.Views.Android.NativeViews
{

    public struct NativeTextboxState
    {

        public bool? Visible;
        public SKRect? Rect;
        public NativeTextboxWidget? Properties;

        public NativeTextboxState(bool? visible, SKRect? rect, NativeTextboxWidget properties)
        {
            Visible = visible;
            Rect = rect;
            Properties = properties;
        }

        public bool HasChanges(NativeTextboxState other)
        {
            if (Visible.HasValue && Visible != other.Visible)
                return true;
            if (Rect.HasValue && Rect != other.Rect)
                return true;
            if (Properties != null)
            {
                if (other.Properties is null)
                    return true;
                if (Properties.Text != other.Properties.Text)
                    return true;
                if (!Properties.Font.Equals(other.Properties.Font))
                    return true;
                if (Properties.TextColor != other.Properties.TextColor)
                    return true;
                if (Properties.HasFocus != other.Properties.HasFocus)
                    return true;
                if (Properties.Keyboard != other.Properties.Keyboard)
                    return true;
            }
            return false;
        }

        public void Update(NativeTextboxState other)
        {
            Visible = other.Visible ?? Visible;
            Rect = other.Rect ?? Rect;
            Properties = other.Properties ?? Properties;
        }

    }

    public class NativeTextboxImpl : EditText, INativeViewImpl//, IUITextFieldDelegate
    {

        public new object Context;

        private Func<Task> RequestRedraw;
        private new Func<string, Task> SetText;
        private bool settingText;


        private object RequestLock = new object();
        private NativeTextboxState? RequestedState;
        private NativeTextboxState CurrentState = new NativeTextboxState();


        public NativeTextboxImpl(Context context, Func<Task> requestRedraw) : base(context)
        {

            RequestRedraw = requestRedraw;

            Background = null;

            //Delegate = this;

            TextChanged += NativeTextboxImpl_TextChanged;

        }

        private bool _disposed;
        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;
            _disposed = true;
            if (disposing)
            {
                TextChanged -= NativeTextboxImpl_TextChanged;
            }
            base.Dispose(disposing);
        }

        private void NativeTextboxImpl_TextChanged(object sender, EventArgs e)
        {
            OnTextChanged();
        }

        protected void OnTextChanged()
        {
            if (SetText != null)
            {
                var text = Text;
                Task.Run(async () =>
                {
                    try
                    {
                        lock (RequestLock)
                        {
                            settingText = true;
                            CurrentState.Properties.Text = text;
                        }
                        await SetText(text);
                        await RequestRedraw();
                    }
                    finally
                    {
                        //settingText = false;
                    }
                });
            }
        }

        public void SetVisible(bool visible)
        {
            RequestState(new NativeTextboxState(visible, null, null));
        }

        public void SetBounds(SKRect nativebounds)
        {
            RequestState(new NativeTextboxState(null, nativebounds, null));
        }

        public void UpdateControl(NativeViewWidget nativeViewWidget, SKRect rect, SKRect original)
        {
            var textbox = (NativeTextboxWidget)nativeViewWidget;
            if (!settingText)
                RequestState(new NativeTextboxState(null, null, textbox));
            else
                if (textbox.Text == this.Text)
                    settingText = false;
        }

        private void RequestState(NativeTextboxState state)
        {
            lock (RequestLock)
            {
                if (RequestedState != null)
                {
                    RequestedState.Value.Update(state);
                }
                else if (state.HasChanges(CurrentState))
                {
                    RequestedState = state;
                    ((Activity)base.Context).RunOnUiThread(ApplyState);
                }
            }
        }

        private void ApplyState()
        {
            lock (RequestLock)
            {

                if (RequestedState is null) return;
                var requested = RequestedState.Value;

                if (requested.Visible.HasValue && CurrentState.Visible != requested.Visible)
                {
#if PRINTEVENTS
                    System.Diagnostics.Debug.WriteLine($"setting visible: {requested.Visible}");
#endif
                    CurrentState.Visible = requested.Visible;
                    Visibility = requested.Visible.Value ? ViewStates.Visible : ViewStates.Gone;
                }

                if (requested.Rect.HasValue && CurrentState.Rect != requested.Rect)
                {
#if PRINTEVENTS
                    System.Diagnostics.Debug.WriteLine($"setting rect: {requested.Rect}");
#endif
                    CurrentState.Rect = requested.Rect;
                    var nativebounds = requested.Rect.Value;
                    var parameters = new RelativeLayout.LayoutParams((int)nativebounds.Width, (int)nativebounds.Height);
                    parameters.LeftMargin = (int)nativebounds.Left;
                    parameters.TopMargin = (int)nativebounds.Top;
                    LayoutParameters = parameters;
                }

                if (requested.Properties != null)
                {

                    if (CurrentState.Properties is null)
                    {
                        CurrentState.Properties = new NativeTextboxWidget(Context, null, null, new Font(0), SKColors.Transparent, false, requested.Properties.Keyboard == Keyboard.Default ? Keyboard.Url : Keyboard.Default);
                    }

                    Apply(ref CurrentState.Properties.Text, requested.Properties.Text, text =>
                    {
#if PRINTEVENTS
                        System.Diagnostics.Debug.WriteLine($"setting text: {text} ({CurrentState.Properties.Text} => {requested.Properties.Text})");
#endif
                        SetText = null;
                        Text = text;
                        SetText = requested.Properties.SetText;
                    });
                    Apply(ref CurrentState.Properties.Font, requested.Properties.Font, font =>
                    {
                        Typeface = font.ToTypeface();
                        SetTextSize(ComplexUnitType.Sp, (float)font.TextSize);
                    });
                    Apply(ref CurrentState.Properties.TextColor, requested.Properties.TextColor, color =>
                    {
                        var acolor = color.ToAndroid().ToArgb();
                        int[][] s_colorStates = { new[] { global::Android.Resource.Attribute.StateEnabled }, new[] { -global::Android.Resource.Attribute.StateEnabled } };
                        SetTextColor(new ColorStateList(s_colorStates, new[] { acolor, acolor }));
                    });
                    Apply(ref CurrentState.Properties.HasFocus, requested.Properties.HasFocus, hasfocus => { if (hasfocus && Focusable) RequestFocus(); });
                    Apply(ref CurrentState.Properties.Keyboard, requested.Properties.Keyboard, keyboard => InputType = keyboard.ToInputType());

                    void Apply<T>(ref T current, T requested, Action<T> setvalue)
                    {
                        if (requested is null || !current.Equals(requested))
                        {
                            setvalue(requested);
                            current = requested;
                        }
                    }

                }

                RequestedState = null;

            }
        }

    }
}
