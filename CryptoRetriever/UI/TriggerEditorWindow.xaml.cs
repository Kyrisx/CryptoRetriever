﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using CryptoRetriever.Strats;
using Trigger = CryptoRetriever.Strats.Trigger;

namespace CryptoRetriever.UI {
    /// <summary>
    /// Allows the user to create or modify a trigger.
    /// If the window closes from the user pressing Okay,
    /// the result is stored in Trigger. Otherwise Trigger
    /// will be null if, which can happen if the user cancels
    /// or closes the window without pressing Okay.
    /// 
    /// If you want to edit a Trigger, set WorkingTrigger before
    /// showing the window.
    /// </summary>
    public partial class TriggerEditorWindow : Window {
        /// <summary>
        /// Contains the trigger being worked on.
        /// When Okay is pressed, this becomes the result Trigger.
        /// This can be set before showing the window to edit
        /// an existing trigger.
        /// </summary>
        public Trigger WorkingTrigger { get; set; }

        /// <summary>
        /// Contains the resulting Trigger or null if
        /// the window was closed without pressing Okay. 
        /// </summary>
        public Trigger Trigger { get; private set; } = null;

        public TriggerEditorWindow() {
            InitializeComponent();
        }

        private void OnConditionClicked(object sender, MouseButtonEventArgs e) {
            ConditionEditorWindow editor = new ConditionEditorWindow();
            editor.Trigger = WorkingTrigger;
            UiHelper.CenterWindowInWindow(editor, this);
            editor.ShowDialog();
        }

        private void OnOkayClicked(object sender, RoutedEventArgs e) {
            Trigger = WorkingTrigger;
            Trigger.Name = _triggerNameTextBox.Text;
            Close();
        }

        private void OnCancelClicked(object sender, RoutedEventArgs e) {
            Close();
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e) {
            if (WorkingTrigger == null) {
                // Create a dummy trigger for now
                WorkingTrigger = new Trigger("SomeTrigger");
                WorkingTrigger.Condition = new LogicComparison(
                    new LogicComparison(
                        new BoolCondition("IsCool", true),
                        new OrLogicOperator(),
                        new BoolCondition("NotCool", false)),
                    new AndLogicOperator(),
                    new NumberComparison(
                        new NumberValue(5),
                        new GreaterThanNumberOperator(),
                        new MathValue(
                            new NumberValue(2),
                            new AdditionOperator(),
                            new MathValue(
                                new NumberValue(3),
                                new MultiplicationOperator(),
                                new NumberValue(5)
                            )
                        )
                    )
                );
            }

            _triggerNameTextBox.Text = WorkingTrigger.Name;
        }
    }
}
