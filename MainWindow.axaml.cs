using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Controls;
using Avalonia.Interactivity;
using windowsCalcOOP;

namespace Calculator;
public partial class MainWindow : Window
{
    private ISolve solver;
    string PreviousButton;
    public MainWindow()
    {
        InitializeComponent();
        Solver solver1 = new Solver();
        solver = (ISolve?)solver1;
        TextBox1.Text = "";
    }
    public void ButtonHandler(object sender, RoutedEventArgs args)
    {
        if (sender is Button button)
        {
            string ButtonText = button.Content.ToString();
            if (PreviousButton == "=")
            {
                TextBox1.Text = "";
                PreviousButton = "";
            }

            switch (ButtonText)
            {
                case "1":
                    TextBox1.Text += "1";
                    break;
                case "2":
                    TextBox1.Text += "2";
                    break;
                case "3":
                    TextBox1.Text += "3";
                    break;
                case "4":
                    TextBox1.Text += "4";
                    break;
                case "5":
                    TextBox1.Text += "5";
                    break;
                case "6":
                    TextBox1.Text += "6";
                    break;
                case "7":
                    TextBox1.Text += "7";
                    break;
                case "8":
                    TextBox1.Text += "8";
                    break;
                case "9":
                    TextBox1.Text += "9";
                    break;
                case "0":
                    TextBox1.Text += ButtonText;
                    solver.Accumulate(ButtonText);
                    break;
                case "%":
                case "*":
                case "+":
                case "-":
                case "/":
                case "+/-":
                    string[] tokens = TextBox1.Text.Split(' ');
                    for (int i = tokens.Length - 1; i >= 0; i--)
                    {
                        if (double.TryParse(tokens[i], out double lastNumber))
                        {
                            tokens[i] = (-lastNumber).ToString();
                            break;
                        }
                    }
                    TextBox1.Text = string.Join(" ", tokens);
                    solver.Clear();
                    solver.Accumulate(TextBox1.Text);
                    break;
                case "=":
                    PreviousButton = "=";
                    if (TextBox1.Text == "")
                    {
                        TextBox1.Text = "undefined";
                        break;
                    }
                    string postfixExpression = InfixToPostfix(TextBox1.Text);
                    double result = EvaluatePostfix(postfixExpression);
                    TextBox1.Text = double.IsNaN(result) ? "undefined" : Math.Round(result, 6).ToString();
                    break;
                case "AC":
                    TextBox1.Text = "";
                    break;
            }
        }
    }
    private string InfixToPostfix(string infixExpression)
    {
//To see what is going on???        
//Console.WriteLine("Infix expression: " + infixExpression);
        Stack<char> operatorStack = new Stack<char>();
        StringBuilder postfix = new StringBuilder();
        StringBuilder currentnumber = new StringBuilder();
        Dictionary<char, int> precedence = new Dictionary<char, int>()
        {{'+', 1}, {'-', 1}, {'*', 2}, {'/', 2}, {'%', 2}};
        bool wasPreviousOperator = true;
        for (int i = 0; i < infixExpression.Length; i++)
        {
            char ch = infixExpression[i];
            bool isDigit = char.IsDigit(ch);
            bool isNegativeSign = (ch == '-' && wasPreviousOperator);
            if (isDigit || isNegativeSign)
            {
                currentnumber.Append(ch);
                wasPreviousOperator = false;
            }
            else
            {
                if (currentnumber.Length > 0)
                {
                    postfix.Append(currentnumber).Append(" ");
                    currentnumber.Clear();
                }
                switch (ch)
                {
                    case '(':
                        operatorStack.Push(ch);
                        break;
                    case ')':
                        while (operatorStack.Peek() != '(') postfix.Append(operatorStack.Pop()).Append(" ");
                            operatorStack.Pop();
                        break;
                    default:
                        if (precedence.ContainsKey(ch))
                        {
                            while (operatorStack.Count > 0 && precedence[ch] <= precedence[operatorStack.Peek()])
                                postfix.Append(operatorStack.Pop()).Append(" ");

                            operatorStack.Push(ch);
                        }
                        break;
                }
            }
            wasPreviousOperator = true;
        }
        Console.WriteLine("Postfix expression: " + postfix.ToString());
        if (currentnumber.Length > 0) 
            postfix.Append(currentnumber.ToString()).Append(" ");
        while (operatorStack.Count > 0)
        {
            Console.WriteLine("Operator: " + operatorStack.Peek());
            postfix.Append(operatorStack.Pop()).Append(" ");
        }
        Console.WriteLine("Updated Postfix expression: " + postfix.ToString());
        return postfix.ToString().Trim();
    }
    private double EvaluatePostfix(string postfixExpression)
    {
        Stack<double> stack = new Stack<double>();
//this is to see what in the world is going on
//Console.WriteLine("Postfix Expression: " + postfixExpression);
        foreach (string token in postfixExpression.Split(' '))
        {
            if (double.TryParse(token, out double value))
                stack.Push(value);
            else
            {
                if (stack.Count < 2)// bad expression                    
                    return double.NaN;

                double operand2 = stack.Pop();
                double operand1 = stack.Pop();
                switch (token)
                {
                    case "%":
                        stack.Push(operand1 % operand2);
                        break;
                    case "+":
                        stack.Push(operand1 + operand2);
                        break;
                    case "-":
                        stack.Push(operand1 - operand2);
                        break;
                    case "*":
                        stack.Push(operand1 * operand2);
                        break;
                    case "/":
                        if (operand2 == 0) // Dividing by 0
                            return double.NaN;

                        stack.Push(operand1 / operand2);
                        break;
                }
            }
        }
        return stack.Pop();
    }
    public bool isZero(double d)
    {
        return d == 0;
    }
}
public interface ISolve
{
    void Accumulate(string s);
    void Clear();
    double Solve();
}