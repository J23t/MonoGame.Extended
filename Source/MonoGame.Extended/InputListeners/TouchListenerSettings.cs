﻿using MonoGame.Extended.ViewportAdapters;

namespace MonoGame.Extended.InputListeners
{
    public class TouchListenerSettings : InputListenerSettings<TouchListener>
    {
        public TouchListenerSettings()
        {
        }

        public ViewportAdapter ViewportAdapter { get; set; }

        public override TouchListener CreateListener()
        {
            return new TouchListener(this);
        }
    }
}