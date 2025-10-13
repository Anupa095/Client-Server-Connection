using Grpc.Net.Client;
using CalculatorServer; // gRPC generated classes
using System;
using System.Windows;
using System.Windows.Controls;

namespace CalculatorClientGUI
{
    public partial class MainWindow : Window
    {
        private Calculator.CalculatorClient? client;

        public MainWindow()
        {
            InitializeComponent();
            ConnectToServer();

            // Button event handlers
            SquareButton.Click += async (s, e) => await CallSquare();
            CubeButton.Click += async (s, e) => await CallCube();
            MultiplyButton.Click += async (s, e) => await CallMultiply();

            ServerComboBox.SelectionChanged += (s, e) => ConnectToServer();
        }

        private void ConnectToServer()
        {
            try
            {
                string address = ((ComboBoxItem)ServerComboBox.SelectedItem).Content.ToString()!;
                var channel = GrpcChannel.ForAddress($"http://{address}");
                client = new Calculator.CalculatorClient(channel);
                ResultTextBlock.Text = $"Connected to {address}";
            }
            catch (Exception ex)
            {
                ResultTextBlock.Text = $"Connection error: {ex.Message}";
            }
        }

        private async System.Threading.Tasks.Task CallSquare()
        {
            if (!int.TryParse(Number1TextBox.Text, out int number))
            {
                ResultTextBlock.Text = "Invalid input for Number 1";
                return;
            }

            try
            {
                var response = await client!.SquareAsync(new CalculationRequest { Number = number });
                ResultTextBlock.Text = $"Square: {response.Result}";
            }
            catch (Exception ex)
            {
                ResultTextBlock.Text = $"Error: {ex.Message}";
            }
        }

        private async System.Threading.Tasks.Task CallCube()
        {
            if (!int.TryParse(Number1TextBox.Text, out int number))
            {
                ResultTextBlock.Text = "Invalid input for Number 1";
                return;
            }

            try
            {
                var response = await client!.CubeAsync(new CalculationRequest { Number = number });
                ResultTextBlock.Text = $"Cube: {response.Result}";
            }
            catch (Exception ex)
            {
                ResultTextBlock.Text = $"Error: {ex.Message}";
            }
        }

        private async System.Threading.Tasks.Task CallMultiply()
        {
            if (!int.TryParse(Number1TextBox.Text, out int number1) ||
                !int.TryParse(Number2TextBox.Text, out int number2))
            {
                ResultTextBlock.Text = "Invalid input for Number 1 or Number 2";
                return;
            }

            try
            {
                var response = await client!.SlowMultiplyAsync(new MultiplyRequest
                {
                    Number1 = number1,
                    Number2 = number2
                });
                ResultTextBlock.Text = $"Multiply: {response.Result}";
            }
            catch (Exception ex)
            {
                ResultTextBlock.Text = $"Error: {ex.Message}";
            }
        }
    }
}
