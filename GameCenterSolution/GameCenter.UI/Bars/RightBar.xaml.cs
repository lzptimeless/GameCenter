﻿using AppCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameCenter.UI.Bars
{
    /// <summary>
    /// Interaction logic for RightBar.xaml
    /// </summary>
    public partial class RightBar : UserControl, IBar
    {
        public RightBar()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
        }

        public void Release()
        {
        }
    }
}
