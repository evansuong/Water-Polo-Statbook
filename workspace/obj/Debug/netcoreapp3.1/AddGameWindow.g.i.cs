﻿#pragma checksum "..\..\..\AddGameWindow.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "27E737CCFA98F48FA7C32C12C88FB0F3D7D603BD"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using Water_Polo_Statbook;


namespace Water_Polo_Statbook {
    
    
    /// <summary>
    /// AddGameWindow
    /// </summary>
    public partial class AddGameWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 36 "..\..\..\AddGameWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BackBTN;
        
        #line default
        #line hidden
        
        
        #line 55 "..\..\..\AddGameWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox OppNameTB;
        
        #line default
        #line hidden
        
        
        #line 62 "..\..\..\AddGameWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox GameTypeCB;
        
        #line default
        #line hidden
        
        
        #line 72 "..\..\..\AddGameWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox GameLocCB;
        
        #line default
        #line hidden
        
        
        #line 87 "..\..\..\AddGameWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox MonthCB;
        
        #line default
        #line hidden
        
        
        #line 103 "..\..\..\AddGameWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox DayCB;
        
        #line default
        #line hidden
        
        
        #line 138 "..\..\..\AddGameWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox YearTB;
        
        #line default
        #line hidden
        
        
        #line 141 "..\..\..\AddGameWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button CreateBTN;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.8.1.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Water Polo Statbook;component/addgamewindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\AddGameWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.8.1.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.BackBTN = ((System.Windows.Controls.Button)(target));
            
            #line 36 "..\..\..\AddGameWindow.xaml"
            this.BackBTN.Click += new System.Windows.RoutedEventHandler(this.BackBTN_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.OppNameTB = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.GameTypeCB = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 4:
            this.GameLocCB = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 5:
            this.MonthCB = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 6:
            this.DayCB = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 7:
            this.YearTB = ((System.Windows.Controls.TextBox)(target));
            
            #line 138 "..\..\..\AddGameWindow.xaml"
            this.YearTB.GotFocus += new System.Windows.RoutedEventHandler(this.YearTB_GotFocus);
            
            #line default
            #line hidden
            return;
            case 8:
            this.CreateBTN = ((System.Windows.Controls.Button)(target));
            
            #line 141 "..\..\..\AddGameWindow.xaml"
            this.CreateBTN.Click += new System.Windows.RoutedEventHandler(this.CreateBTN_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

