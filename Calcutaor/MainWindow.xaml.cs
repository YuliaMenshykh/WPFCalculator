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

namespace Calcutaor
{
    interface CalcItem
    {
        
    }

    class CalcItemNumber : CalcItem
    {
        public long number { get; set; }
        public CalcItemNumber(long number)
        {
            this.number = number;   
        }
    }

    class CalcItemOp : CalcItem
    {
        public enum Operation { plus, minus, multiply, divide };

        public Operation operation { get; set; }
        public CalcItemOp( Operation operation)
        {
            this.operation = operation;
        }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<CalcItem> items = new();
        string numberFormatter = "0";

        List<Button> digitButtons { get {
                var list = new List<Button>();
                for(int i = 0; i <= 9; i++)
                {
                    list.Add((Button) FindName($"digit{i}Btn"));
                }
                return list;
            } 
        }
        public MainWindow()
        {
            InitializeComponent();

            foreach(Button button in digitButtons)
            {
                button.Click += DigitButtonClick;
            }

            this.addOpBtn.Click += PlusOperation;
            this.minusOpBtn.Click += MinusOperation;
            this.multOpBtn.Click += MultiplyOperation;
            this.divideOpBtn.Click += DivideOperation;

            this.addOpBtn.Click += HandleOperationButtonClicked;
            this.minusOpBtn.Click += HandleOperationButtonClicked;
            this.multOpBtn.Click += HandleOperationButtonClicked;
            this.divideOpBtn.Click += HandleOperationButtonClicked;

            this.equalsOpBtn.Click += EqualButtonClicked;
        }

        private void EqualButtonClicked(object sender, RoutedEventArgs e)
        {
            long sum = 0;
            foreach(var item in items)
            {
                if(item is CalcItemNumber)
                {

                    if (items.IndexOf(item) == 0)
                    {
                        sum += ((CalcItemNumber)item).number;
                        continue;
                    }
                    var prevOp = (CalcItemOp)items.ElementAt(items.IndexOf(item) - 1);
                    switch (prevOp.operation)
                    {
                        case CalcItemOp.Operation.plus: sum += ((CalcItemNumber)item).number; break;
                        case CalcItemOp.Operation.minus: sum -= ((CalcItemNumber)item).number; break;
                    }
                }
                if (item is CalcItemOp) continue;
            }

            items.Clear();
            items.Add(new CalcItemNumber(sum));
            resultText.Text = FormatTextView();
        }

        private void DivideOperation(object sender, RoutedEventArgs e)
        {
            items.Add(new CalcItemOp(CalcItemOp.Operation.divide));
        }

        private void MultiplyOperation(object sender, RoutedEventArgs e)
        {
            items.Add(new CalcItemOp(CalcItemOp.Operation.multiply));
        }

        private void MinusOperation(object sender, RoutedEventArgs e)
        {
            items.Add(new CalcItemOp(CalcItemOp.Operation.minus));
        }

        private void PlusOperation(object sender, RoutedEventArgs e)
        {
            items.Add(new CalcItemOp(CalcItemOp.Operation.plus));
        }

        private void HandleOperationButtonClicked(object sender, RoutedEventArgs e)
        {
            numberFormatter = "0";
            resultText.Text = FormatTextView();
        }

        private string FormatTextView()
        {
            string sum = "";
            foreach(var item in items)
            {
                if (item is CalcItemOp)
                {
                    switch(((CalcItemOp) item).operation)
                    {
                        case CalcItemOp.Operation.multiply: sum += " x "; break;
                        case CalcItemOp.Operation.plus: sum += " + "; break;
                        case CalcItemOp.Operation.divide: sum += " / "; break;
                        case CalcItemOp.Operation.minus: sum += " - "; break;
                    }
                }

                if(item is CalcItemNumber)
                {
                    sum += ((CalcItemNumber)item).number.ToString();
                }
            }
            return sum;
        }


        private void DigitButtonClick(object sender, RoutedEventArgs e)
        {
            Button _sender = (Button) sender;
            long digit = Int64.Parse(_sender.Content.ToString() ?? "0");

            long finalDigit = Int64.Parse(numberFormatter + digit);
            if (finalDigit.ToString().Length > 15) return;


            numberFormatter = finalDigit.ToString();
            var lastItem = items.LastOrDefault();
            if (lastItem == null || lastItem is CalcItemOp) items.Add(new CalcItemNumber(finalDigit));
            else if (lastItem is CalcItemNumber)
            {
                ((CalcItemNumber)lastItem).number = finalDigit;
            }


            resultText.Text = FormatTextView();
        }
    }
}
