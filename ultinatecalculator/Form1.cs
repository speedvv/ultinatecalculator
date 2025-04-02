using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace UltimateCalculator
{
    public class Form1 : Form
    {
        TextBox txtDisplay;
        ListBox lstHistory;
        string operation = "";
        double value = 0;
        bool isResultShown = false;

        public Form1()
        {
            InitializeCalculator();
            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;
        }

        private void InitializeCalculator()
        {
            this.Text = "Ultimate Scientific Calculator";
            this.Size = new Size(420, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.BackColor = Color.FromArgb(28, 28, 30);
            this.MaximizeBox = false;

            txtDisplay = new TextBox
            {
                Font = new Font("Consolas", 22),
                Location = new Point(10, 10),
                Size = new Size(380, 45),
                ReadOnly = true,
                TextAlign = HorizontalAlignment.Right,
                BackColor = Color.FromArgb(40, 40, 43),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(txtDisplay);

            lstHistory = new ListBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(10, 440),
                Size = new Size(380, 100),
                BackColor = Color.Black,
                ForeColor = Color.LightGray
            };
            this.Controls.Add(lstHistory);

            string[] buttons = {
                "7", "8", "9", "/", "√",
                "4", "5", "6", "*", "1/x",
                "1", "2", "3", "-", "sin",
                ".", "0", "=", "+", "cos",
                "C", "←", "%", "^", "log"
            };

            int x = 10, y = 70;
            for (int i = 0; i < buttons.Length; i++)
            {
                Button btn = new Button
                {
                    Text = buttons[i],
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    Size = new Size(70, 50),
                    Location = new Point(x, y),
                    BackColor = Color.FromArgb(55, 55, 60),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.Click += Button_Click;
                this.Controls.Add(btn);

                x += 75;
                if ((i + 1) % 5 == 0)
                {
                    x = 10;
                    y += 55;
                }
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            HandleInput(btn.Text);
        }

        private void HandleInput(string text)
        {
            try
            {
                if (text == "C")
                {
                    txtDisplay.Text = "";
                    operation = "";
                    value = 0;
                }
                else if (text == "←")
                {
                    if (txtDisplay.Text.Length > 0)
                        txtDisplay.Text = txtDisplay.Text.Substring(0, txtDisplay.Text.Length - 1);
                }
                else if (text == "=")
                {
                    if (string.IsNullOrWhiteSpace(txtDisplay.Text)) return;

                    double secondValue = double.Parse(txtDisplay.Text, CultureInfo.InvariantCulture);
                    double result = operation switch
                    {
                        "+" => value + secondValue,
                        "-" => value - secondValue,
                        "*" => value * secondValue,
                        "/" => secondValue != 0 ? value / secondValue : throw new DivideByZeroException(),
                        "%" => value % secondValue,
                        "^" => Math.Pow(value, secondValue),
                        _ => 0
                    };

                    txtDisplay.Text = result.ToString(CultureInfo.InvariantCulture);
                    SaveHistory($"{value} {operation} {secondValue} = {result}");
                    isResultShown = true;
                    operation = "";
                }
                else if ("+-*/%^".Contains(text))
                {
                    if (string.IsNullOrWhiteSpace(txtDisplay.Text)) return;
                    value = double.Parse(txtDisplay.Text, CultureInfo.InvariantCulture);
                    operation = text;
                    txtDisplay.Text = "";
                }
                else if (text == "√")
                {
                    double num = double.Parse(txtDisplay.Text, CultureInfo.InvariantCulture);
                    double result = Math.Sqrt(num);
                    txtDisplay.Text = result.ToString(CultureInfo.InvariantCulture);
                    SaveHistory($"√({num}) = {result}");
                }
                else if (text == "1/x")
                {
                    double num = double.Parse(txtDisplay.Text, CultureInfo.InvariantCulture);
                    if (num == 0) throw new DivideByZeroException();
                    double result = 1 / num;
                    txtDisplay.Text = result.ToString(CultureInfo.InvariantCulture);
                    SaveHistory($"1/({num}) = {result}");
                }
                else if (text == "sin")
                {
                    double num = double.Parse(txtDisplay.Text, CultureInfo.InvariantCulture);
                    double result = Math.Sin(num * Math.PI / 180);
                    txtDisplay.Text = result.ToString(CultureInfo.InvariantCulture);
                    SaveHistory($"sin({num}) = {result}");
                }
                else if (text == "cos")
                {
                    double num = double.Parse(txtDisplay.Text, CultureInfo.InvariantCulture);
                    double result = Math.Cos(num * Math.PI / 180);
                    txtDisplay.Text = result.ToString(CultureInfo.InvariantCulture);
                    SaveHistory($"cos({num}) = {result}");
                }
                else if (text == "log")
                {
                    double num = double.Parse(txtDisplay.Text, CultureInfo.InvariantCulture);
                    double result = Math.Log10(num);
                    txtDisplay.Text = result.ToString(CultureInfo.InvariantCulture);
                    SaveHistory($"log({num}) = {result}");
                }
                else if (text == ".")
                {
                    if (!txtDisplay.Text.Contains(".")) txtDisplay.Text += ".";
                }
                else
                {
                    if (isResultShown) txtDisplay.Text = "";
                    txtDisplay.Text += text;
                    isResultShown = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) HandleInput("=");
            else if (e.KeyCode == Keys.Back) HandleInput("←");
            else if (e.Control && e.KeyCode == Keys.C) Clipboard.SetText(txtDisplay.Text);
            else if (e.Control && e.KeyCode == Keys.V) txtDisplay.Text += Clipboard.GetText();
        }

        private void SaveHistory(string line)
        {
            lstHistory.Items.Insert(0, line);
            File.AppendAllText("history.txt", line + Environment.NewLine);
        }
    }
}