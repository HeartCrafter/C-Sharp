﻿using System;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LSamplePluginWPF
{
    /// <summary>
    /// Interaction logic for Test_WPFWindow.xaml
    /// </summary>
    public partial class Test_WPFWindow : Window
    {
        bool runOnStart = true;

        public bool RunOnStart
        {
            get
            {
                return runOnStart;
            }

            set
            {
                runOnStart = value;
            }
        }

        /// <summary>
        /// Initializes window
        /// </summary>
        public Test_WPFWindow()
        {
            InitializeComponent();

            // Attaches drag window event
            MouseLeftButtonDown += DragWindow;

            Left = Properties.Settings.Default.WindowLocation.X;
            Top = Properties.Settings.Default.WindowLocation.Y;
        }

        /// <summary>
        /// Allows to drag window
        /// </summary>
        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        /// <summary>
        /// Speak Button Click event
        /// </summary>
        private void test_button_speak_Click(object sender, RoutedEventArgs e)
        {
            jarvisWPF.PublicClass.SpeechSynth.SpeakRandomPhrase(test_textBox_Speak.Text);
        }

        /// <summary>
        /// Emulate Button Click event
        /// </summary>
        private void test_button_emulate_Click(object sender, RoutedEventArgs e)
        {
            jarvisWPF.Classes.Plugins.PluginController.EmulateSpeech(test_textBox_Emulate.Text);
        }

        /// <summary>
        /// Highlights move icon
        /// </summary>
        private void image_MouseEnter(object sender, MouseEventArgs e)
        {
            Image img = (Image)sender;
            DoubleAnimation ani = new DoubleAnimation(1, TimeSpan.FromMilliseconds(100));
            img.BeginAnimation(Image.OpacityProperty, ani);
        }

        /// <summary>
        /// Unhighlights move icon
        /// </summary>
        private void image_MouseLeave(object sender, MouseEventArgs e)
        {
            Image img = (Image)sender;
            DoubleAnimation ani = new DoubleAnimation(.08, TimeSpan.FromMilliseconds(100));
            img.BeginAnimation(Image.OpacityProperty, ani);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.WindowLocation = new System.Drawing.Point((int)Left, (int)Top);
            Properties.Settings.Default.LoadOnStart = RunOnStart;
            Properties.Settings.Default.Save();
        }

        private void test_checkBox_LoadOnStart_MouseEnter(object sender, MouseEventArgs e)
        {
            CheckBox chkBox = (CheckBox)sender;            
            DoubleAnimation ani = new DoubleAnimation(1, TimeSpan.FromMilliseconds(100));
            chkBox.BeginAnimation(OpacityProperty, ani);
            test_labelBlock_LoadOnStart.BeginAnimation(OpacityProperty, ani);
        }

        private void test_checkBox_LoadOnStart_MouseLeave(object sender, MouseEventArgs e)
        {
            CheckBox chkBox = (CheckBox)sender;
            DoubleAnimation ani = new DoubleAnimation(.08, TimeSpan.FromMilliseconds(100));
            chkBox.BeginAnimation(OpacityProperty, ani);
            test_labelBlock_LoadOnStart.BeginAnimation(OpacityProperty, ani);
        }
    }
}
