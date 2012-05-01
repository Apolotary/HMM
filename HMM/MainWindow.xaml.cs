using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Accord.Statistics.Models;
using Accord.Statistics.Models.Markov;

namespace HMM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int alphabet;
        private int states;
        private List<int[]> sequences;
        private double likelihood;
        private HiddenMarkovModel hmm;
        private int[] inputSequence;
        private double calculatedLikelihood;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            sequences = new List<int[]>();
            sequences.Add(new int[] { 0, 1, 1, 1, 1, 0, 1, 1, 1, 1 });
            sequences.Add(new int[] { 0, 1, 1, 1, 0, 1, 1, 1, 1, 1 });
            sequences.Add(new int[] { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1 });
            sequences.Add(new int[] { 0, 1, 1, 1, 1, 1 });
            sequences.Add(new int[] { 0, 1, 1, 1, 1, 1, 1 });
            sequences.Add(new int[] { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1 });
            sequences.Add(new int[] { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1 });

            foreach (var sequence in sequences)
            {
                for (int i = 0; i < sequence.Count(); i++)
                {
                    sequenceBox.Text += sequence[i];
                    if (i != sequence.Count()-1)
                    {
                        sequenceBox.Text += ",";
                    }
                }
                sequenceBox.Text += "\n";
            }
        }

        private void sequenceBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            learnLabel.Visibility = System.Windows.Visibility.Hidden;
        }

        private void learnButton_Click(object sender, RoutedEventArgs e)
        {
            ReadSequenceBox();
            ReadCharBox();
            ReadStateBox();
            ReadLikelihoodBox();

            int[][] sequenceArray = SequenceListToArray();

            hmm = new HiddenMarkovModel(alphabet, states);

            hmm.Learn(sequenceArray, likelihood);
        }

        private int[][] SequenceListToArray()
        {
            int[][] sequenceArray = new int[sequences.Count][];

            for (int i = 0; i < sequences.Count(); ++i)
            {
                sequenceArray[i] = sequences[i];
            }

            return sequenceArray;
        }

        private void ReadCharBox()
        {
            if (!Regex.IsMatch(charBox.Text, @"^[0-9]+$"))
            {
                throw new System.ArgumentException("Parameter can be only a digit", "original");
            }

            alphabet = Convert.ToInt32(charBox.Text);
        }

        private void ReadStateBox()
        {
            if (!Regex.IsMatch(stateBox.Text, @"^[0-9]+$"))
            {
                throw new System.ArgumentException("Parameter can be only a digit", "original");
            }
            states = Convert.ToInt32(stateBox.Text);
        }

        private void ReadLikelihoodBox()
        {
            if (!Regex.IsMatch(likeBox.Text, @"^[0-9.]+$"))
            {
                throw new System.ArgumentException("Parameter can be only a digit", "original");
            }
            likelihood = double.Parse(likeBox.Text);
        }

        private void ReadSequenceBox()
        {
            if(!Regex.IsMatch(sequenceBox.Text, @"^[0-9,\n]+$"))
            {
                throw new System.ArgumentException("Parameter can be only a digit", "original");
            }

            char[] allChars = sequenceBox.Text.ToCharArray();

            int[] listOfInts = new int[allChars.Count()];
            sequences = new List<int[]>();

            int k = 0;

            for (int i = 0; i < allChars.Count(); ++i)
            {
                if (Int32.TryParse(allChars[i].ToString(), out listOfInts[k]))
                {
                    k++;
                }
                if (allChars[i] == '\n')
                {
                    int[] temp = new int[k];

                    for (int j = 0; j < k; j++)
                    {
                        temp[j] = listOfInts[j];
                    }
                    sequences.Add(temp);

                    k = 0;
                    listOfInts = new int[allChars.Count()];
                }
            }

            learnLabel.Visibility = System.Windows.Visibility.Visible;
            calculateBox.IsEnabled = true;
        }

        private void ReadInputSequence()
        {
            if (!Regex.IsMatch(inputSequenceBox.Text, @"^[0-9,]+$"))
            {
                throw new System.ArgumentException("Parameter can be only a digit", "original");
            }
            char[] allChars = inputSequenceBox.Text.ToCharArray();

            int[] allNums = new int[allChars.Count()];

            int k = 0;

            for (int i = 0; i < allChars.Count(); ++i)
            {
                if (Int32.TryParse(allChars[i].ToString(), out allNums[k]))
                {
                    k++;
                }
            }

            inputSequence = new int[k];

            for (int i = 0; i < k; i++)
            {
                inputSequence[i] = allNums[i];
            }

        }

        private void calculateBox_Click(object sender, RoutedEventArgs e)
        {
            ReadInputSequence();

            calculatedLikelihood = hmm.Evaluate(inputSequence);
            
            probabilityBox.Text = string.Format("{0:0.0000}",  calculatedLikelihood);
        }
    }
}
