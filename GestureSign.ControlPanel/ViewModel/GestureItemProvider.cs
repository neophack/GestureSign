﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using GestureSign.Common.Gestures;
using GestureSign.ControlPanel.Common;

namespace GestureSign.ControlPanel.ViewModel
{
    public class GestureItemProvider
    {
        private static ObservableCollection<GestureItem> _gestureItems;

        static GestureItemProvider()
        {
            GestureManager.Instance.GestureSaved += (o, e) => { Update(); };

            if (GestureManager.Instance.FinishedLoading) Update();
            GestureManager.Instance.OnLoadGesturesCompleted += (o, e) => { Application.Current.Dispatcher.Invoke(Update); };
        }

        public static ObservableCollection<GestureItem> GestureItems
        {
            get { return _gestureItems; }
            set { _gestureItems = value; }
        }

        private static void Update()
        {
            if (_gestureItems == null)
                _gestureItems = new ObservableCollection<GestureItem>();
            else
                _gestureItems.Clear();

            // Get all available gestures from gesture manager
            IEnumerable<IGesture> results = GestureManager.Instance.Gestures.OrderBy(g => g.Name);

            var brush = Application.Current.Resources["HighlightBrush"] as Brush ?? Brushes.RoyalBlue;
            foreach (IGesture gesture in results)
            {
                GestureItem newItem = new GestureItem()
                {
                    Image = GestureImage.CreateImage(gesture.Points, new Size(65, 65), brush),
                    Name = gesture.Name
                };
                GestureItems.Add(newItem);
            }
        }
    }
}