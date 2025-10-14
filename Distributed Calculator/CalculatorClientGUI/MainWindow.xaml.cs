using Grpc.Net.Client;
using CalculatorServer; // gRPC generated namespace
using System;
using System.Windows;
using System.Windows.Controls;

namespace CalculatorClientGUI
{
    public partial class MainWindow : Window
    {
        private Calculator.CalculatorClient? client;
        private string[] servers = { "localhost:5001", "localhost:5002", "localhost:5003" };
        private int currentServerIndex = 0;

        public MainWindow()
        {
            InitializeComponent();

            // Placeholder handling
            Number1TextBox.TextChanged += (s, e) =>
                Placeholder1.Visibility = string.IsNullOrEmpty(Number1TextBox.Text) ? Visibility.Visible : Visibility.Hidden;
            Number2TextBox.TextChanged += (s, e) =>
                Placeholder2.Visibility = string.IsNullOrEmpty(Number2TextBox.Text) ? Visibility.Visible : Visibility.Hidden;

            // Event handlers
            SquareButton.Click += async (s, e) => await CallSquare();
            CubeButton.Click += async (s, e) => await CallCube();
            MultiplyButton.Click += async (s, e) => await CallMultiply();
            ConnectButton.Click += (s, e) => ConnectToServer();
        }

        private void ConnectToServer()
        {
            try
            {
                if (ManualModeRadio.IsChecked == true)
                {
                    string address = ((ComboBoxItem)ServerComboBox.SelectedItem).Content.ToString()!;
                    var channel = GrpcChannel.ForAddress($"http://{address}");
                    client = new Calculator.CalculatorClient(channel);
                    ResultTextBlock.Text = $"✅ Connected manually to {address}";
                }
                else
                {
                    // Auto connect to next available
                    client = GetNextServerClient();
                    ResultTextBlock.Text = $"✅ Auto connected to {servers[currentServerIndex]}";
                }
            }
            catch (Exception ex)
            {
                client = null;
                ResultTextBlock.Text = $"❌ Connection failed: {ex.Message}";
            }
        }

        private Calculator.CalculatorClient GetNextServerClient()
        {
            for (int i = 0; i < servers.Length; i++)
            {
                string address = servers[currentServerIndex];
                currentServerIndex = (currentServerIndex + 1) % servers.Length;

                try
                {
                    var channel = GrpcChannel.ForAddress($"http://{address}");
                    var testClient = new Calculator.CalculatorClient(channel);

                    // Test small call
                    testClient.Square(new CalculationRequest { Number = 1 });
                    return testClient;
                }
                catch
                {
                    // Try next one if failed
                    continue;
                }
            }
            throw new Exception("No servers available.");
        }

        private async System.Threading.Tasks.Task CallSquare()
        {
            if (!int.TryParse(Number1TextBox.Text, out int number))
            {
                ResultTextBlock.Text = "⚠️ Invalid Number 1 input.";
                return;
            }

            try
            {
                EnsureConnected();
                var response = await client!.SquareAsync(new CalculationRequest { Number = number });
                ResultTextBlock.Text = $"✅ Square of {number} = {response.Result}";
            }
            catch (Exception ex)
            {
                ResultTextBlock.Text = $"❌ Error: {ex.Message}";
            }
        }

        private async System.Threading.Tasks.Task CallCube()
        {
            if (!int.TryParse(Number1TextBox.Text, out int number))
            {
                ResultTextBlock.Text = "⚠️ Invalid Number 1 input.";
                return;
            }

            try
            {
                EnsureConnected();
                var response = await client!.CubeAsync(new CalculationRequest { Number = number });
                ResultTextBlock.Text = $"✅ Cube of {number} = {response.Result}";
            }
            catch (Exception ex)
            {
                ResultTextBlock.Text = $"❌ Error: {ex.Message}";
            }
        }

        private async System.Threading.Tasks.Task CallMultiply()
        {
            if (!int.TryParse(Number1TextBox.Text, out int number1) ||
                !int.TryParse(Number2TextBox.Text, out int number2))
            {
                ResultTextBlock.Text = "⚠️ Invalid input for numbers.";
                return;
            }

            try
            {
                EnsureConnected();
                var response = await client!.SlowMultiplyAsync(new MultiplyRequest
                {
                    Number1 = number1,
                    Number2 = number2
                });
                ResultTextBlock.Text = $"✅ {number1} × {number2} = {response.Result}";
            }
            catch (Exception ex)
            {
                ResultTextBlock.Text = $"❌ Error: {ex.Message}";
            }
        }

        private void EnsureConnected()
        {
            if (client == null)
                ConnectToServer();
        }
    }
}
