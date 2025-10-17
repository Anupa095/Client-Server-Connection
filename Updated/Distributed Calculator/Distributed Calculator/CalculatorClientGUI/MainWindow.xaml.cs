using Grpc.Net.Client;
using CalculatorServer; // gRPC generated namespace
using Shared;
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;

namespace CalculatorClientGUI
{
    public partial class MainWindow : Window
    {
        private Calculator.CalculatorClient? client;
        private string[] servers = { "localhost:5001", "localhost:5002", "localhost:5003" };
        private int currentServerIndex = 0;
        private VectorClock vectorClock = new();
        // Map server host (e.g. "localhost:5001") to last known clock value for display convenience
        private Dictionary<string, int> serverClockSnapshot = new();

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
            AddiyionButton.Click += (s, e) => ApplyRangePartition();
            ConnectButton.Click += (s, e) => ConnectToServer();
            ClearButton.Click += (s, e) => ClearCalculation();
        }

        // Parse a range string like "1-10" and divide it into three contiguous subranges.
        private void ApplyRangePartition()
        {
            string input = RangeTextBox.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(input))
            {
                ResultTextBlock.Text = "⚠️ Enter a range like 1-10.";
                return;
            }

            // expected format: start-end
            var parts = input.Split('-');
            if (parts.Length != 2 || !int.TryParse(parts[0], out int start) || !int.TryParse(parts[1], out int end) || start > end)
            {
                ResultTextBlock.Text = "⚠️ Invalid range. Use start-end with start <= end.";
                return;
            }

            // Check which servers are available
            var availableServers = new List<int>();
            for (int i = 0; i < servers.Length; i++)
            {
                try
                {
                    var channel = GrpcChannel.ForAddress($"http://{servers[i]}");
                    var testClient = new Calculator.CalculatorClient(channel);
                    testClient.Square(new CalculationRequest { Number = 1 });
                    availableServers.Add(i);
                }
                catch
                {
                    // Server is down
                    continue;
                }
            }

            if (availableServers.Count == 0)
            {
                ResultTextBlock.Text = "⚠️ No servers are available!";
                return;
            }

            // total numbers inclusive
            int total = end - start + 1;
            int baseSize = total / availableServers.Count;
            int remainder = total % availableServers.Count; // distribute the remainder to the earlier ranges

            (int s, int e)[] ranges = new (int s, int e)[3];
            int cursor = start;

            // Initialize all ranges as inactive
            for (int i = 0; i < 3; i++)
            {
                ranges[i] = (0, -1);
            }

            // Distribute ranges only to available servers
            for (int idx = 0; idx < availableServers.Count; idx++)
            {
                int serverIndex = availableServers[idx];
                int size = baseSize + (idx < remainder ? 1 : 0);
                if (size <= 0) continue;

                ranges[serverIndex] = (cursor, cursor + size - 1);
                cursor += size;
            }

            // Update UI per server with status
            Server1Range.Text = ranges[0].e >= ranges[0].s ? $"Range: {ranges[0].s}-{ranges[0].e} (Active)" : "Status: Inactive";
            Server2Range.Text = ranges[1].e >= ranges[1].s ? $"Range: {ranges[1].s}-{ranges[1].e} (Active)" : "Status: Inactive";
            Server3Range.Text = ranges[2].e >= ranges[2].s ? $"Range: {ranges[2].s}-{ranges[2].e} (Active)" : "Status: Inactive";

            // Compute partial sums for each active server
            long totalSum = 0;
            StringBuilder resultDetails = new StringBuilder();
            resultDetails.AppendLine("Calculation Details:");

            foreach (int serverIndex in availableServers)
            {
                if (ranges[serverIndex].e >= ranges[serverIndex].s)
                {
                    // Calculate partial sum for this server's range
                    long rangeSize = ranges[serverIndex].e - ranges[serverIndex].s + 1;
                    long rangeSum = rangeSize * (ranges[serverIndex].s + ranges[serverIndex].e) / 2;
                    totalSum += rangeSum;

                    resultDetails.AppendLine($"Server {serverIndex + 1} calculated sum for range {ranges[serverIndex].s}-{ranges[serverIndex].e} = {rangeSum:N0}");
                }
            }

            // Update vector clocks only for active servers (increment each active server once in ascending order)
            availableServers.Sort();
            foreach (int serverIndex in availableServers)
            {
                if (ranges[serverIndex].e >= ranges[serverIndex].s)
                {
                    vectorClock.Increment(servers[serverIndex]);
                }
            }

            // Persist latest clock values into serverClockSnapshot
            var latest = vectorClock.GetClock();
            foreach (var s in servers)
            {
                if (latest.ContainsKey(s))
                    serverClockSnapshot[s] = latest[s];
                else if (!serverClockSnapshot.ContainsKey(s))
                    serverClockSnapshot[s] = 0;
            }

            // Show the final result prominently
            resultDetails.AppendLine($"\nTotal Sum = {totalSum:N0}");
            ResultTextBlock.Text = resultDetails.ToString();
            UpdateServerDisplay();
        }

        private void UpdateServerDisplay()
        {
            // helper to show clock and last process for each server
            void SetServerUI(int idx, TextBlock clockTb, TextBlock? procTb, List<int> activeServers)
            {
                string server = servers[idx];
                var snapshotLive = vectorClock.GetClock();
                bool isActive = activeServers.Contains(idx);
                var values = new int[servers.Length];
                
                if (!isActive)
                {
                    // Server is down: show [0,0,0] and Clock: 0
                    clockTb.Text = "Clock: 0";
                    for (int i = 0; i < servers.Length; i++) 
                        values[i] = 0;
                }
                else
                {
                    // Server is active: show progressive vector clock up to this server's index
                    for (int i = 0; i < servers.Length; i++)
                    {
                        if (i <= idx)
                            values[i] = snapshotLive.ContainsKey(servers[i]) ? snapshotLive[servers[i]] : 0;
                        else
                            values[i] = 0;
                    }
                    clockTb.Text = $"Clock: {values[idx]}";
                }

                // Set vector clock values in array format
                TextBlock vectorClockBlock = idx switch
                {
                    0 => Server1Vector,
                    1 => Server2Vector,
                    2 => Server3Vector,
                    _ => null
                };

                if (vectorClockBlock != null)
                    vectorClockBlock.Text = $"[{string.Join(",", values)}]";

                // Update server status color based on availability
                var parentGrid = clockTb.Parent as Grid;
                if (parentGrid != null)
                {
                    parentGrid.Background = isActive ?
                        System.Windows.Media.Brushes.LightGreen :
                        System.Windows.Media.Brushes.LightGray;
                }
            }

            // Get current active servers
            var activeServers = new List<int>();
            for (int i = 0; i < servers.Length; i++)
            {
                try
                {
                    var channel = GrpcChannel.ForAddress($"http://{servers[i]}");
                    var testClient = new Calculator.CalculatorClient(channel);
                    var request = new CalculationRequest { Number = 1 };
                    testClient.Square(request);
                    activeServers.Add(i);
                }
                catch
                {
                    // Server is down
                    continue;
                }
            }

            SetServerUI(0, Server1Clock, null, activeServers);
            SetServerUI(1, Server2Clock, null, activeServers);
            SetServerUI(2, Server3Clock, null, activeServers);
        }

        private void ConnectToServer()
        {
            try
            {
                if (ManualModeRadio.IsChecked == true)
                {
                    string address = ((ComboBoxItem)ServerComboBox.SelectedItem).Content.ToString()!;
                    try
                    {
                        var channel = GrpcChannel.ForAddress($"http://{address}");
                        var testClient = new Calculator.CalculatorClient(channel);
                        // Try a test call to check if server is up
                        testClient.Square(new CalculationRequest { Number = 1 });
                        client = testClient;
                        ResultTextBlock.Text = $"✅ Connected manually to {address}";
                        // Only update display, do not increment vector clock on connect
                        UpdateServerDisplay();
                    }
                    catch
                    {
                        client = null;
                        ResultTextBlock.Text = $"❌ Server {address} is down.";
                    }
                }
                else
                {
                    // Auto connect to next available
                    client = GetNextServerClient();
                    ResultTextBlock.Text = $"✅ Auto connected to {servers[currentServerIndex]}";
                    // Only update display, do not increment vector clock on connect
                    UpdateServerDisplay();
                }
            }
            catch (Exception ex)
            {
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
                                        // Only update display, do not increment vector clock on connect
                                        UpdateServerDisplay();
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
                string serverKey = GetConnectedServerKey();
                bool calculationDone = false;
                int attempts = 0;
                while (!calculationDone && attempts < servers.Length)
                {
                    try
                    {
                        var channel = GrpcChannel.ForAddress($"http://{serverKey}");
                        var testClient = new Calculator.CalculatorClient(channel);
                        var response = await testClient.SquareAsync(new CalculationRequest { Number = number });
                        ResultTextBlock.Text = $"✅ Square of {number} = {response.Result:N0} (Server: {serverKey})";
                        vectorClock.Increment(serverKey);
                        serverClockSnapshot[serverKey] = vectorClock.GetClock().ContainsKey(serverKey) ? vectorClock.GetClock()[serverKey] : 0;
                        UpdateServerDisplay();
                        calculationDone = true;
                    }
                    catch
                    {
                        // Server is down, try next
                        attempts++;
                        int idx = Array.IndexOf(servers, serverKey);
                        serverKey = servers[(idx + 1) % servers.Length];
                        if (attempts == servers.Length)
                        {
                            ResultTextBlock.Text = $"❌ All servers are down.";
                        }
                    }
                }
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
                string serverKey = GetConnectedServerKey();
                bool calculationDone = false;
                int attempts = 0;
                while (!calculationDone && attempts < servers.Length)
                {
                    try
                    {
                        var channel = GrpcChannel.ForAddress($"http://{serverKey}");
                        var testClient = new Calculator.CalculatorClient(channel);
                        var response = await testClient.CubeAsync(new CalculationRequest { Number = number });
                        ResultTextBlock.Text = $"✅ Cube of {number} = {response.Result:N0} (Server: {serverKey})";
                        vectorClock.Increment(serverKey);
                        serverClockSnapshot[serverKey] = vectorClock.GetClock().ContainsKey(serverKey) ? vectorClock.GetClock()[serverKey] : 0;
                        UpdateServerDisplay();
                        calculationDone = true;
                    }
                    catch
                    {
                        // Server is down, try next
                        attempts++;
                        int idx = Array.IndexOf(servers, serverKey);
                        serverKey = servers[(idx + 1) % servers.Length];
                        if (attempts == servers.Length)
                        {
                            ResultTextBlock.Text = $"❌ All servers are down.";
                        }
                    }
                }
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
                string serverKey = GetConnectedServerKey();
                bool calculationDone = false;
                int attempts = 0;
                while (!calculationDone && attempts < servers.Length)
                {
                    try
                    {
                        var channel = GrpcChannel.ForAddress($"http://{serverKey}");
                        var testClient = new Calculator.CalculatorClient(channel);
                        var response = await testClient.SlowMultiplyAsync(new MultiplyRequest
                        {
                            Number1 = number1,
                            Number2 = number2
                        });
                        ResultTextBlock.Text = $"✅ {number1:N0} × {number2:N0} = {response.Result:N0} (Server: {serverKey})";
                        vectorClock.Increment(serverKey);
                        serverClockSnapshot[serverKey] = vectorClock.GetClock().ContainsKey(serverKey) ? vectorClock.GetClock()[serverKey] : 0;
                        UpdateServerDisplay();
                        calculationDone = true;
                    }
                    catch
                    {
                        // Server is down, try next
                        attempts++;
                        int idx = Array.IndexOf(servers, serverKey);
                        serverKey = servers[(idx + 1) % servers.Length];
                        if (attempts == servers.Length)
                        {
                            ResultTextBlock.Text = $"❌ All servers are down.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ResultTextBlock.Text = $"❌ Error: {ex.Message}";
            }
        }

        private string GetConnectedServerKey()
        {
            // try to determine server from the last connection selection
            if (ManualModeRadio.IsChecked == true)
            {
                return ((ComboBoxItem)ServerComboBox.SelectedItem).Content.ToString()!;
            }
            else
            {
                // currentServerIndex points to the next server to try; get previous index used
                int idx = (currentServerIndex - 1 + servers.Length) % servers.Length;
                return servers[idx];
            }
        }

        private void EnsureConnected()
        {
            if (client == null)
                ConnectToServer();
        }

        private void ClearCalculation()
        {
            // Clear input fields
            Number1TextBox.Clear();
            Number2TextBox.Clear();
            RangeTextBox.Clear();
            
            // Reset result display
            ResultTextBlock.Text = "Results will appear here";
            
            // Reset server range displays
            Server1Range.Text = "Range: -";
            Server2Range.Text = "Range: -";
            Server3Range.Text = "Range: -";
            
            // Reset vector clock
            vectorClock = new VectorClock();
            serverClockSnapshot.Clear();
            
            // Update server display to show [0,0,0]
            UpdateServerDisplay();
        }
    }
}
