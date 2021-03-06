﻿namespace GestureSign.CorePlugins.Volume
{
    public class VolumeSettings
    {
        #region Public Properties

        //Tri-state, matches cboMethod.Items indexes 0 = Increase, 1 = Decrease, 2 = Toggle Mute
        public int Method { get; set; }

        //Stored as actual percent, whole number 10, 20, etc.
        public int Percent { get; set; }

        #endregion
    }
}
