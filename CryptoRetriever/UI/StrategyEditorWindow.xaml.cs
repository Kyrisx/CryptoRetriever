﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CryptoRetriever.Filter;
using CryptoRetriever.Strats;

namespace CryptoRetriever.UI {
    /// <summary>
    /// Allows the user to modify or create a strategy with a UI
    /// </summary>
    public partial class StrategyEditorWindow : Window {
        private Strategy _strategy; // The currently edited strategy
        private Strategy _result = null; // This will be set to the strategy when the editing is done

        /// <summary>
        /// Gets the strategy modified or created by this editor or
        /// null if the editor was cancelled or closed without saving.
        /// </summary>
        public Strategy Strategy {
            get {
                return _result;
            }
        }

        public StrategyEditorWindow() {
            InitializeComponent();
            SetStrategy(new Strategy());

            // Temp
            _strategy.Filters.Add(new GaussianFilter(2));
            _strategy.Filters.Add(new GaussianFilter(1));
            _strategy.States.Add(new State("Default"));
            _strategy.Triggers.Add(new Strats.Trigger("Sample trigger"));
        }

        /// <summary>
        /// Sets the strategy to edit and adjusts the UI to reflect the
        /// values contained as its defaults.
        /// </summary>
        /// <param name="strategyToEdit">The strategy to edit.</param>
        public void SetStrategy(Strategy strategyToEdit) {
            _strategy = strategyToEdit;
            _nameTextBox.Text = strategyToEdit.Name;
            _accountStartingFiatTextBox.Text = "" + _strategy.Account.CurrencyBalance;
            _accountStartingAssetsTextBox.Text = "" + _strategy.Account.AssetBalance;
            if (_strategy.Start != DateTime.MinValue)
                _startDatePicker.SelectedDate = _strategy.Start;
            if (_strategy.End != DateTime.MinValue)
                _endDatePicker.SelectedDate = _strategy.End;
            _filtersView.ItemsSource = _strategy.Filters;
            _statesView.ItemsSource = _strategy.States;
            _triggersView.ItemsSource = _strategy.Triggers;
        }

        private void OnCancelButtonClicked(object sender, RoutedEventArgs e) {
            Close();
        }

        private void OnSaveButtonClicked(object sender, RoutedEventArgs e) {
            // Validate the name 
            // ?? TODO: We need to check for duplicate names but there's no persistance yet
            String name = _nameTextBox.Text;
            if (String.IsNullOrWhiteSpace(name)) {
                MessageBox.Show("You must enter a name.");
                return;
            }

            // Validate account
            double startingFiat = -1;
            double startingAssets = -1;
            if (!Double.TryParse(_accountStartingFiatTextBox.Text, out startingFiat) ||
                !Double.TryParse(_accountStartingAssetsTextBox.Text, out startingAssets) ||
                startingFiat < 0 || 
                startingAssets < 0) {
                MessageBox.Show("Starting Fiat and Assets need to be 0 or greater.");
                return;
            }

            // Check the start/end dates. If they're valid dates use them otherwise
            // indicate that the dataset start and end should be used.
            // The validity of these dates depends on the dataset. Arguably they shouldn't
            // be set until the strategy is actually run on a dataset.
            DateTime startDate = DateTime.MinValue;
            DateTime endDate = DateTime.MinValue;
            if (_startDatePicker.SelectedDate.HasValue)
                startDate = _startDatePicker.SelectedDate.Value;
            if (_endDatePicker.SelectedDate.HasValue)
                endDate = _endDatePicker.SelectedDate.Value;

            if (endDate < startDate) {
                MessageBox.Show("The end date cannot be before the start date.");
                return;
            }

            // The Filters/States/Triggers are updated
            // as we go on the working Strategy so no
            // need to do anything for those.

            _strategy.Name = name;
            _strategy.Account = new Account(startingFiat, startingAssets);
            _strategy.Start = startDate;
            _strategy.End = endDate;
            _result = _strategy;

            Close();
        }

        private void OnAddFilterClicked(object sender, RoutedEventArgs e) {
            AddFilterDialog addFilterDialog = new AddFilterDialog();
            UiHelper.CenterWindowInWindow(addFilterDialog, this);
            addFilterDialog.ShowDialog();

            if (addFilterDialog.Filter != null) {
                _strategy.Filters.Add(addFilterDialog.Filter);
            }
        }

        private void OnRemoveFilterClicked(object sender, RoutedEventArgs e) {
            RemoveSelectedItemsFromListBox(_strategy.Filters, _filtersView);
        }

        private void OnAddStateClicked(object sender, RoutedEventArgs e) {
            InputBoxDialog inputBox = new InputBoxDialog();
            inputBox.Title = "New State";
            inputBox.Message = "Enter a state ID";
            inputBox.InitialText = "";
            UiHelper.CenterWindowInWindow(inputBox, this);
            inputBox.ShowDialog();

            if (!String.IsNullOrEmpty(inputBox.EnteredText))
                _strategy.States.Add(new State(inputBox.EnteredText));
        }

        private void OnRemoveStateClicked(object sender, RoutedEventArgs e) {
            RemoveSelectedItemsFromListBox(_strategy.States, _statesView);
        }

        private void OnAddTriggerClicked(object sender, RoutedEventArgs e) {
            TriggerEditorWindow triggerEditor = new TriggerEditorWindow();
            UiHelper.CenterWindowInWindow(triggerEditor, this);
            triggerEditor.ShowDialog();

            if (triggerEditor.Trigger != null) {
                _strategy.Triggers.Add(triggerEditor.Trigger);
            }
        }

        private void OnTriggerDoubleClicked(object sender, MouseButtonEventArgs e) {
            if (_triggersView.SelectedIndex >= 0) {
                TriggerEditorWindow triggerEditor = new TriggerEditorWindow();
                UiHelper.CenterWindowInWindow(triggerEditor, this);

                // ?? TODO: This should be a deep copy but there's no way to save conditions
                // right now so it will be a reference until that is implemented.
                triggerEditor.WorkingTrigger = _strategy.Triggers[_triggersView.SelectedIndex];
                triggerEditor.ShowDialog();

                if (triggerEditor.Trigger != null) {
                    _strategy.Triggers[_triggersView.SelectedIndex] = triggerEditor.Trigger;
                }
            }
        }

        private void OnRemoveTriggerClicked(object sender, RoutedEventArgs e) {
            RemoveSelectedItemsFromListBox(_strategy.Triggers, _triggersView);
        }

        private void RemoveSelectedItemsFromListBox<T>(ObservableCollection<T> list, ListBox listbox) {
            List<T> itemsToRemove = new List<T>();
            foreach (T item in listbox.SelectedItems) {
                itemsToRemove.Add(item);
            }
            foreach (T item in itemsToRemove) {
                list.Remove(item);
            }
        }
    }
}
