﻿using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using WindowsInput;
using GestureSign.Common.Plugins;

namespace GestureSign.CorePlugins.MouseActions
{
    public class MouseActionsPlugin : IPlugin
    {
        #region Private Variables

        private MouseActionsUI _gui = null;
        private MouseActionsSettings _settings = null;

        #endregion

        #region PInvoke Declarations

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out Point lpPoint);

        #endregion

        #region Public Properties

        public string Name
        {
            get { return "鼠标动作"; }
        }

        public string Description
        {
            get { return GetDescription(); }
        }

        public UserControl GUI
        {
            get { return _gui ?? (_gui = CreateGUI()); }
        }

        public MouseActionsUI TypedGUI
        {
            get { return (MouseActionsUI)GUI; }
        }

        public string Category
        {
            get { return "鼠标"; }
        }

        public bool IsAction
        {
            get { return true; }
        }

        #endregion

        #region Public Methods

        public void Initialize()
        {

        }

        public bool Gestured(PointInfo actionPoint)
        {
            if (_settings == null)
                return false;

            InputSimulator simulator = new InputSimulator();
            try
            {
                switch (_settings.MouseAction)
                {
                    case MouseActions.HorizontalScroll:
                        simulator.Mouse.HorizontalScroll(_settings.ScrollAmount);
                        return true;
                    case MouseActions.VerticalScroll:
                        simulator.Mouse.VerticalScroll(_settings.ScrollAmount);
                        return true;
                    case MouseActions.MoveMouseBy:
                        {
                            Point p;
                            if (GetCursorPos(out p))
                            {
                                if (SetCursorPos(_settings.MovePoint.X + p.X, _settings.MovePoint.Y + p.Y))
                                {
                                    return true;
                                }
                            }
                            return false;
                        }
                    case MouseActions.MoveMouseTo:
                        return SetCursorPos(_settings.MovePoint.X, _settings.MovePoint.Y);
                }

                switch (_settings.ClickPosition)
                {
                    case ClickPositions.LastUp:
                        var lastUpPoint = actionPoint.Points.Last().Last();
                        SetCursorPos(lastUpPoint.X, lastUpPoint.Y);
                        break;
                    case ClickPositions.LastDown:
                        var lastDownPoint = actionPoint.Points.Last().First();
                        SetCursorPos(lastDownPoint.X, lastDownPoint.Y);
                        break;
                    case ClickPositions.FirstUp:
                        var firstUpPoint = actionPoint.Points.First().Last();
                        SetCursorPos(firstUpPoint.X, firstUpPoint.Y);
                        break;
                    case ClickPositions.FirstDown:
                        var firstDownPoint = actionPoint.Points.First().First();
                        SetCursorPos(firstDownPoint.X, firstDownPoint.Y);
                        break;
                }

                MethodInfo clickMethod = typeof(IMouseSimulator).GetMethod(_settings.MouseAction.ToString());
                clickMethod.Invoke(simulator.Mouse, null);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool Deserialize(string SerializedData)
        {
            return PluginHelper.DeserializeSettings(SerializedData, out _settings);
        }

        public string Serialize()
        {
            if (_gui != null)
                _settings = _gui.Settings;

            if (_settings == null)
                _settings = new MouseActionsSettings();

            return PluginHelper.SerializeSettings(_settings);
        }

        #endregion

        #region Private Methods

        private MouseActionsUI CreateGUI()
        {
            MouseActionsUI newGUI = new MouseActionsUI();

            newGUI.Loaded += (o, e) =>
            {
                TypedGUI.Settings = _settings;
            };

            return newGUI;
        }

        private string GetDescription()
        {
            switch (_settings.MouseAction)
            {
                case MouseActions.HorizontalScroll:
                    return "水平向" + (_settings.ScrollAmount >= 0 ? "右" : "左") + "滚动" + Math.Abs(_settings.ScrollAmount) +
                           "单位";
                case MouseActions.VerticalScroll:
                    return "垂直向" + (_settings.ScrollAmount >= 0 ? "上" : "下") + "滚动" + Math.Abs(_settings.ScrollAmount) +
                        "单位";
                case MouseActions.MoveMouseBy:
                    return "鼠标位移" + _settings.MovePoint;
                case MouseActions.MoveMouseTo:
                    return "鼠标移动至"+_settings.MovePoint;
            }

            return String.Format("{0} {1}",
                ClickPositionDescription.DescriptionDict[_settings.ClickPosition],
                 MouseActionDescription.DescriptionDict[_settings.MouseAction]);
        }

        #endregion

        #region Host Control

        public IHostControl HostControl { get; set; }

        #endregion
    }
}