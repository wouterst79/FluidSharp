using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

namespace FluidSharp.Views.iOS.Services
{


    // https://developer.apple.com/library/archive/documentation/StringsTextFonts/Conceptual/TextAndWebiPhoneOS/KeyboardManagement/KeyboardManagement.html

    // https://docs.microsoft.com/en-us/dotnet/api/uikit.uikeyboard.notifications.observedidshow?view=xamarin-ios-sdk-12

    public class KeyboardTracker
    {

        NSObject _keyboardShowObserver;
        NSObject _keyboardHideObserver;
        NSObject _keyboardChangeObserver;

        Action<nfloat> OnKeyboardHeightChanged;

        public KeyboardTracker(Action<nfloat> onKeyboardHeightChanged)
        {
            OnKeyboardHeightChanged = onKeyboardHeightChanged;
        }

        public void RegisterForKeyboardNotifications()
        {
            _keyboardShowObserver = UIKeyboard.Notifications.ObserveWillShow((s, e) => OnKeyboardShow(e));
            _keyboardChangeObserver = UIKeyboard.Notifications.ObserveWillChangeFrame((s, e) => OnKeyboardShow(e));
            _keyboardHideObserver = UIKeyboard.Notifications.ObserveWillHide((s, e) => OnKeyboardHide(e));
        }

        public void UnregisterForKeyboardNotifications()
        {
            if (_keyboardShowObserver != null)
            {
                _keyboardShowObserver.Dispose();
                _keyboardShowObserver = null;
            }

            if (_keyboardChangeObserver != null)
            {
                _keyboardChangeObserver.Dispose();
                _keyboardChangeObserver = null;
            }

            if (_keyboardHideObserver != null)
            {
                _keyboardHideObserver.Dispose();
                _keyboardHideObserver = null;
            }

        }

        protected virtual void OnKeyboardShow(UIKeyboardEventArgs notification)
        {
            OnKeyboardHeightChanged(notification.FrameEnd.Height);
        }

        private void OnKeyboardHide(UIKeyboardEventArgs notification)
        {
            OnKeyboardHeightChanged(0);
        }

    }

}