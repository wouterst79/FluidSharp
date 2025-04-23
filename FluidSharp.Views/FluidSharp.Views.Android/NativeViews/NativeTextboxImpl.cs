#if DEBUG
#define PRINTEVENTS
#endif
using FluidSharp.Widgets.Native;
using SkiaSharp;
using SkiaSharp.TextBlocks;
using System;
using System.Collections.Generic;
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
using Android.Views.InputMethods;
using Android.InputMethodServices;
using Gfx = Android.Graphics;
using Android.Text.Method;
using Java.Lang;

namespace FluidSharp.Views.Android.NativeViews
{

    public struct NativeTextboxState
    {

        public bool? Visible;
        public SKRect? Rect;
        public NativeTextboxWidget Properties;
        public float Scale;

        public NativeTextboxState(bool? visible, SKRect? rect, NativeTextboxWidget properties, float scale)
        {
            Visible = visible;
            Rect = rect;
            Properties = properties;
            Scale = scale;
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
            if (Scale != other.Scale)
                return true;
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
        public DateTime? TextReceived;


        private object RequestLock = new object();
        private NativeTextboxState? RequestedState;
        private NativeTextboxState CurrentState = new NativeTextboxState();

        private int updatecounter;

        public NativeTextboxImpl(Context context, Func<Task> requestRedraw) : base(context)
        {

            RequestRedraw = requestRedraw;

            Focusable = true;
            FocusableInTouchMode = true;

            Background = null;

            //var color = Gfx.Color.Red;
            //SetBackgroundColor(color);
            //SetCursorVisible(true);
            //SetAutoSizeTextTypeWithDefaults(AutoSizeTextType.Uniform);
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
            var setText = SetText;
            if (setText != null)
            {
                var text = Text;
                Task.Run(async () =>
                {
                    try
                    {
                        lock (RequestLock)
                        {
                            TextReceived = DateTime.Now;
                            ++updatecounter;
                            CurrentState.Properties.Text = text;
#if PRINTEVENTS
                            System.Diagnostics.Debug.WriteLine($"recving text: {text}");
#endif
                        }
                        await setText(text);
                        await RequestRedraw();
                    }
                    finally
                    {
                    }
                });
            }
        }

        public void SetVisible(bool visible)
        {
            RequestState(new NativeTextboxState(visible, null, null, CurrentState.Scale));
        }

        public void SetBounds(SKRect nativebounds)
        {
            RequestState(new NativeTextboxState(null, nativebounds, null, CurrentState.Scale), true);
        }

        public void UpdateControl(NativeViewWidget nativeViewWidget, SKRect rect, SKRect original)
        {
            var textbox = (NativeTextboxWidget)nativeViewWidget;
            var scale = rect.Width / original.Width;
            RequestState(new NativeTextboxState(null, null, textbox, scale));
            SetText = textbox.SetText;
        }

        private bool waiting;
        private void RequestState(NativeTextboxState state, bool wait = false)
        {
            lock (RequestLock)
            {
                var startapply = false;
                if (RequestedState != null)
                {
                    RequestedState.Value.Update(state);
                    startapply = waiting;
                }
                else if (state.HasChanges(CurrentState))
                {
                    waiting = wait;
                    RequestedState = state;
                    startapply = !wait;
                }
                if (startapply)
                {
                    var count = (++updatecounter % 1000);
                    ((Activity)base.Context).RunOnUiThread(() => ApplyState(count));
                }
            }
        }

        private void ApplyState(int count)
        {

            lock (RequestLock)
            {

                if (RequestedState is null) return;
                var requested = RequestedState.Value;
                RequestedState = null;

                if (requested.Visible.HasValue && CurrentState.Visible != requested.Visible)
                {
#if PRINTEVENTS
                    System.Diagnostics.Debug.WriteLine($"setting visible: {requested.Visible}");
#endif
                    CurrentState.Visible = requested.Visible;
                    Visibility = requested.Visible.Value ? ViewStates.Visible : ViewStates.Gone;
                    ShowKeyboard(requested.Visible.Value);
                }

                var currentParams = LayoutParameters as RelativeLayout.LayoutParams;
                if (currentParams != null && CurrentState.Rect != null)
                {
                    CurrentState.Rect = SKRect.Create(currentParams.LeftMargin, currentParams.TopMargin, currentParams.Width, CurrentState.Rect.Value.Height);
                }

                if (requested.Rect.HasValue && CurrentState.Rect != requested.Rect)
                {
#if PRINTEVENTS
                    System.Diagnostics.Debug.WriteLine($"setting rect: {requested.Rect}");
#endif
                    CurrentState.Rect = requested.Rect;
                    var nativebounds = requested.Rect.Value;
                    //var parameters = new RelativeLayout.LayoutParams((int)nativebounds.Width, (int)nativebounds.Height);
                    var parameters = new RelativeLayout.LayoutParams((int)nativebounds.Width, RelativeLayout.LayoutParams.WrapContent);
                    parameters.LeftMargin = (int)nativebounds.Left;
                    // v-center to give the native textbox margin some room
                    parameters.TopMargin = (int)(nativebounds.Top - (Height - nativebounds.Height) / 2);
                    LayoutParameters = parameters;
                }

                if (requested.Properties != null)
                {

                    if (CurrentState.Properties is null)
                    {
                        CurrentState.Properties = new NativeTextboxWidget(Context, null, null, new Font(0), SKColors.Transparent, false, requested.Properties.Keyboard == Keyboard.Default ? Keyboard.Url : Keyboard.Default);
                    }

                    if (TextReceived.HasValue && (Text == requested.Properties.Text || TextReceived.Value.AddSeconds(1) < DateTime.Now))
                    {
                        TextReceived = null;
#if PRINTEVENTS
                        System.Diagnostics.Debug.WriteLine($"unlockg text: {requested.Properties.Text}");
#endif
                    }

                    if (!TextReceived.HasValue && Text != requested.Properties.Text)
                    {

                        var text = requested.Properties.Text;
#if PRINTEVENTS
                        System.Diagnostics.Debug.WriteLine($"setting text: {text}");
#endif
                        SetText = null;
                        Text = text;
                        SetSelection(text.Length);
                        SetText = requested.Properties.SetText;

                    }
                    Apply(ref CurrentState.Properties.Font, requested.Properties.Font, font =>
                    {
                        Typeface = font.ToTypeface();
                        var unitType = ComplexUnitType.Dip;
                        var size = font.TextSize;
                        SetTextSize(unitType, size);
                        //SetTextSize(ComplexUnitType.Dip, (float)font.TextSize / requested.Scale);
                    });
                    Apply(ref CurrentState.Properties.TextColor, requested.Properties.TextColor, color =>
                    {
                        var acolor = color.ToAndroid().ToArgb();
                        int[][] s_colorStates = { new[] { global::Android.Resource.Attribute.StateEnabled }, new[] { -global::Android.Resource.Attribute.StateEnabled } };
                        SetTextColor(new ColorStateList(s_colorStates, new[] { acolor, acolor }));
                    });
                    Apply(ref CurrentState.Properties.HasFocus, requested.Properties.HasFocus, hasfocus =>
                    {
                        if (hasfocus && Focusable)
                        {
                            CurrentState.Properties.HasFocus = this.RequestFocus();
                            if (CurrentState.Properties.HasFocus)
                            {
                                // show keyboard
                                InputMethodManager mgr = (InputMethodManager)(((Activity)base.Context).GetSystemService(global::Android.Content.Context.InputMethodService));
                                mgr.ShowSoftInput(this, ShowFlags.Implicit);
                            }
                        }
                    });
                    Apply(ref CurrentState.Properties.Keyboard, requested.Properties.Keyboard, keyboard =>
                    {
                        InputType = keyboard.ToInputType();
                        if (keyboard == Keyboard.Numeric)
                        {
                            try
                            {
                                try
                                {
                                    KeyListener = new DecimalKeyListener();
                                }
                                catch (IncompatibleClassChangeError)
                                {
                                    KeyListener = DigitsKeyListener.GetInstance("1234567890,.");
                                }
                            }
                            catch { }
                        }
                    });

                    CurrentState.Scale = requested.Scale;

                    void Apply<T>(ref T current, T requested, Action<T> setvalue)
                    {
                        if (requested is null || !current.Equals(requested))
                        {
                            current = requested;
                            setvalue(requested);
                        }
                    }

                }

            }
        }

        private void ShowKeyboard(bool show)
        {

            var inputManager = (InputMethodManager)base.Context.GetSystemService(global::Android.Content.Context.InputMethodService);

            if (show)
                inputManager.ShowSoftInput(this, ShowFlags.Implicit);
            else
                inputManager.HideSoftInputFromWindow(WindowToken, HideSoftInputFlags.None);

        }

    }
}
