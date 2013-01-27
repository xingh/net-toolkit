﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace NET.Tools.WPF.Skins
{
    public static class Sims3Skin
    {
        public static SkinBundle SkinBundle
        {
            get
            {
                return new SkinBundle(
                    new Uri(@"pack://application:,,,/NET.Tools.WPF.Skins.Sims3Skin;component/Sims3Skin.xaml"),
                    "Sims3Window");
            }
        }
    }
}
