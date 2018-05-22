﻿using System;
using System.IO;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SudokyProject
{
    public partial class Form2 : Form
    {
        private SudokuGame _sudoku;
        //private Form1 _form1;
        public static int _hint = 10;
        private int[,] SudokuMatrix;
        private TextBox[,] textArray;
        private bool checkValue = false, checkTime = false;
        private int millisek, sek, min;

        public Form2(int hint)
        {
            InitializeComponent();
            SetDesktopLocation(500, 250);
            CreateTextBoxes();
            _hint = hint;
            theAmountOfHint.Text = _hint.ToString();

        }

        private void ShowGame()
        {
            _sudoku = new SudokuGame();
            textArray = new TextBox[9, 9];
            for (int row = 0; row < tablePanel1.RowCount; row++)
            {
                for (int clm = 0; clm < tablePanel1.ColumnCount; clm++)
                {
                    var box = GetTextBoxAt(row, clm);
                    box.Text = _sudoku.Numbers[clm, row].ToString();
                    box.ReadOnly = true;
                    textArray[clm, row] = box;

                }
            }
            SudokuMatrix = _sudoku.Numbers;
            ChooseHowMuchCellsToDelete(levelBox.SelectedIndex);

        }

        private TextBox GetTextBoxAt(int row, int clm)
        {
            return (TextBox)tablePanel1.GetControlFromPosition(row, clm);
        }

        private void DeleteSomeCells(int numb)
        {
            Random rng = new Random();
            for (int i = 0; i < numb; i++)
            {

                int x1 = rng.Next(0, 9);
                int y1 = rng.Next(0, 9);
                var box = GetTextBoxAt(x1, y1);

                if (box.Text != "")
                {
                    box.Text = "";
                    box.ReadOnly = false;
                }
            }
        }

        private void ChooseHowMuchCellsToDelete(int level)
        {
            Random rng = new Random();
            switch (level)
            {

                case 0:
                    DeleteSomeCells(rng.Next(46, 51));
                    break;
                case 1:
                    DeleteSomeCells(rng.Next(51, 56));
                    break;
                case 2:
                    DeleteSomeCells(rng.Next(56, 61));
                    break;
            }
        }

        private void CreateTextBoxes()
        {
            for (int row = 0; row < tablePanel1.RowCount; row++)
            {
                for (int clm = 0; clm < tablePanel1.ColumnCount; clm++)
                {
                    var textBox = new TextBox
                    {
                        TextAlign = HorizontalAlignment.Center,
                        Font = new Font("Arial", 20f, FontStyle.Bold),
                        AutoSize = false,
                        Dock = DockStyle.Fill,
                        MaxLength = 1,
                        BackColor = Color.MistyRose
                    };
                    textBox.KeyPress += textBox_KeyPress;
                    tablePanel1.Controls.Add(textBox, row, clm);
                }
            }
        }


        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar))
                switch (e.KeyChar)
                {
                    case ' ':
                        e.Handled = false;
                        break;
                    case (char)Keys.Back:
                        e.Handled = false;
                        break;
                    default:
                        e.Handled = true;
                        break;
                }
            else
            {
                e.Handled = false;

            }
            if (!(e.KeyChar == ' ' | e.KeyChar == '0')) return;
            e.KeyChar = (char)Keys.Back;
        }

        private void tablePanel1_Paint(object sender, PaintEventArgs e)
        {
            var height = GetTextBoxAt(0, 3).Top - GetTextBoxAt(0, 2).Bottom;

            e.Graphics.FillRectangle(Brushes.IndianRed, 0, 0, tablePanel1.Width, height);
            e.Graphics.FillRectangle(Brushes.IndianRed, GetTextBoxAt(0, 2).Left, GetTextBoxAt(0, 2).Bottom, tablePanel1.Width, height);
            e.Graphics.FillRectangle(Brushes.IndianRed, GetTextBoxAt(0, 5).Left, GetTextBoxAt(0, 5).Bottom, tablePanel1.Width, height);
            e.Graphics.FillRectangle(Brushes.IndianRed, GetTextBoxAt(0, 8).Left, GetTextBoxAt(0, 8).Bottom, tablePanel1.Width, height);

            e.Graphics.FillRectangle(Brushes.IndianRed, 0, 0, height, tablePanel1.Width);
            e.Graphics.FillRectangle(Brushes.IndianRed, GetTextBoxAt(2, 0).Right, GetTextBoxAt(2, 0).Top, height, tablePanel1.Width);
            e.Graphics.FillRectangle(Brushes.IndianRed, GetTextBoxAt(5, 0).Right, GetTextBoxAt(5, 0).Top, height, tablePanel1.Width);
            e.Graphics.FillRectangle(Brushes.IndianRed, GetTextBoxAt(8, 0).Right, GetTextBoxAt(8, 0).Top, height, tablePanel1.Width);
        }

        private void playGameButton_Click(object sender, EventArgs e)
        {
            if (levelBox.SelectedIndex < 0)
            {
                MessageBox.Show("Вы не выбрали уровень", "Предупреждение");
            }
            else
            {
                
                for (int row = 0; row < tablePanel1.RowCount; row++)
                {
                    for (int clm = 0; clm < tablePanel1.ColumnCount; clm++)
                    {
                        var box = GetTextBoxAt(row, clm);
                        if (box.ForeColor == Color.DarkOliveGreen)
                        {
                            box.ReadOnly = false;
                            box.ForeColor = Color.Black;
                            break;
                        }

                    }
                }

                nowYouCanUseIt();
                tablePanel1.Enabled = true;
                ShowGame();
                min = 0; sek = 0; millisek = 0;
                timer1.Start();
            }

        }

        private void resetChangesButton_Click(object sender, EventArgs e)
        {
            for (int row = 0; row < tablePanel1.RowCount; row++)
            {
                for (int clm = 0; clm < tablePanel1.ColumnCount; clm++)
                {
                    var box = GetTextBoxAt(row, clm);
                    if (box.ReadOnly == false)
                        box.Text = string.Empty;
                }
            }
        }

        private void getHintButton_Click(object sender, EventArgs e)
        {
            int countOfHints = _hint;

            if (countOfHints > 0)
            {
                for (int row = 0; row < tablePanel1.RowCount; row++)
                {
                    for (int clm = 0; clm < tablePanel1.ColumnCount; clm++)
                    {
                        var box = GetTextBoxAt(row, clm);
                        if (box.Text == "")
                        {
                            box.Text = SudokuMatrix[clm, row].ToString();
                            row = tablePanel1.ColumnCount;
                            box.ReadOnly = true;
                            box.ForeColor = Color.DarkOliveGreen;
                            break;
                        }

                    }
                }
                countOfHints--;
                theAmountOfHint.Text = countOfHints.ToString();
            }
            else
                MessageBox.Show("У вас недостаточно подсказок","Предупреждение",MessageBoxButtons.OK);
            _hint = countOfHints;
        }

        private static bool CheckSudoku(int[] sudoku)               // метод возвращает true, если судоку составлен правильно
        {
            for (int index = 0; index < 81; index++)
            {
                if (9 < sudoku[index] || sudoku[index] < 0) //Проверка значений ячеек в диапазоне 1 до 9
                    return false;

                if (sudoku[index] != 0)                     //Проверяем на совпадение только ненулевые ячейки
                {
                    //проверяем ячейку на совпадение значения с другими ячейками в строке, столбце, сегменте
                    if (CheckPossibleValue(sudoku[index], index, sudoku) == false)
                        return false;
                }
            }
            return true;
        }

        //Метод проверяее значение на совпадение с другими ячейками в строке, столбце, сегменте
        //в метод поступает: проверяемое значение, индекс ячейки, где проверяется это значение, и массив судоку
        static bool CheckPossibleValue(int value, int index, int[] sudoku)
        {
            //проверяем по строке и столбцу
            for (int j = 0; j < 9; j++)
            {
                if (value == sudoku[(index / 9) * 9 + j]    //находим первый элемент строки (index / 9) * 9
                    & index != (index / 9) * 9 + j)         //условие чтобы ячейка не сравнивала себя с собой
                {
                    return false;                           //возвращаем false, если было совпадение значения с ячейками в строке
                }

                if (value == sudoku[(index % 9) + 9 * j]    //находим первый элемент столбца (index % 9) + 9
                    & index != (index % 9) + 9 * j)
                {
                    return false;                           //возвращаем false, если было совпадение значения с ячейками в столбце
                }
            }
            //проверяем по сегменту
            for (int j = 0; j < 3; j++)
            {
                for (int k = 0; k < 3; k++)
                {                                           //находим первый элемент сегмента ((index / 27 * 27) + (index % 9) / 3 * 3)
                    if (value == sudoku[(index / 27 * 27) + ((index % 9) / 3 * 3) + (9 * j) + k]
                        & index != ((index / 27 * 27) + (index % 9) / 3 * 3 + (9 * j) + k))
                    {
                        return false;                       //возвращаем false, если было совпадение значения с ячейками в сегменте
                    }
                }
            }
            return true;                                    //возвращаем true, если совпадений не было
        }

        private void saveResultButton_Click(object sender, EventArgs e)
        {
            if (playerName.Text!="")
            {
                string[] lines = new string[4];


                lines[3] = playerTime.Text;

                lines[0] = gameLevel.Text;
                lines[1] = playerName.Text;
                lines[2] = playerStore.Text;
                File.AppendAllLines("playerTable.txt", lines.Select(t => t.ToString()));
                infoPanel.Visible = false;
                nowYouCanUseIt();
            }
            else
            {
                MessageBox.Show("Введите имя Игрока","Предупреждение",MessageBoxButtons.OK);
            }
            
        }


        private int getStore(int min, int sek, int level)
        {
            int store = 0; int optimalMin = 0;
            int optimalSek = 0; int count = 0;

            switch (level)
            {

                case 0:
                    optimalMin = 5;
                    store = 250;
                    count = 10;
                    break;
                case 1:
                    optimalMin = 7;
                    store = 500;
                    count = 50;
                    break;
                case 2:
                    optimalMin = 10;
                    store = 800;
                    count = 100;
                    break;
            }

            if (Form1.hint != int.Parse(theAmountOfHint.Text))
            {
                store -= (Form1.hint - int.Parse(theAmountOfHint.Text)) * count;
            }
            else
            {
                if (min < optimalMin)
                {
                    store += (optimalMin - min) * 60;
                    if (sek > optimalSek)
                    {
                        store += sek - optimalSek;
                    }
                }
            }



            return store;
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form1 newform = new Form1();
            Form1.hint = _hint;

        }

        private void cancelSavingResultButton_Click(object sender, EventArgs e)
        {
            infoPanel.Visible = false;
            youCantUseIt();
            levelBox.Enabled = true;
            playGameButton.Enabled = true;

        }

        private void youCantUseIt()
        {
            getHintButton.Enabled = false;
            StopOrRestartTimeButton.Enabled = false;
            checkBox1.Enabled = false;
            finishTheGameButton.Enabled = false;
            playGameButton.Enabled = false;
            resetChangesButton.Enabled = false;
            levelBox.Enabled = false;
        }
        private void nowYouCanUseIt()
        {
            getHintButton.Enabled = true;
            StopOrRestartTimeButton.Enabled = true;
            checkBox1.Enabled = true;
            finishTheGameButton.Enabled = true;
            playGameButton.Enabled = true;
            resetChangesButton.Enabled = true;
            levelBox.Enabled = true;
        }

        private void finishTheGameButton_Click(object sender, EventArgs e)
        {
            int[] matrix = new int[81];
            int k = 0;
            bool flag = false;

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    var box = GetTextBoxAt(i, j);
                    if (box.Text != "")
                    {
                        matrix[k] = int.Parse(box.Text);
                        k++;
                    }
                    else
                    {
                        flag = true;
                        i = 9;
                        break;
                    }
                        
                }
            }
            if (flag!=true)
            {
                if (CheckSudoku(matrix) == true)
                {
                    timer1.Stop();
                    DialogResult result = MessageBox.Show("Cохранить результат?", "Завершено", MessageBoxButtons.YesNo);
                    tablePanel1.Enabled = false;
                    if (result == DialogResult.Yes)
                    {
                        youCantUseIt();
                        infoPanel.Visible = true;
                        playerTime.Text = min + " : " + sek + " : " + millisek;
                        gameLevel.Text = levelBox.SelectedItem.ToString();
                        playerStore.Text = getStore(min, sek, levelBox.SelectedIndex).ToString();
                    }
                }
            }
            else
            {
                
                MessageBox.Show("Вы не заполнили все клетки Судоку", "Не завершено", MessageBoxButtons.OK);
           
            }
                

        }

        private void checkBox1_Click(object sender, EventArgs e)
        {
            bool isChecked = true;

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {

                    if (textArray[j, i].Text != "")
                    {
                        if (textArray[j, i].Text != SudokuMatrix[j, i].ToString())
                        {

                            if (checkValue == true)
                            {
                                textArray[j, i].ForeColor = Color.Black;
                                isChecked = false;
                            }
                            else
                            {
                                textArray[j, i].ForeColor = Color.Red;
                                isChecked = true;
                            }

                        }
                    }

                }
            }
            checkValue = isChecked;

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            millisek += 1;
            if (millisek == 10)
            {
                millisek = 0;
                sek += 1;
            }
            if (sek == 60)
            {
                sek = 0;
                min += 1;
            }

            timeStatusLabel.Text = "Время решения " + min + " : " + sek + " : " + millisek;
        }

        private void StopOrRestartTimeButton_Click(object sender, EventArgs e)
        {
            bool ischecked = true;
            if (checkTime == true)
            {
                timer1.Start();
                tablePanel1.Enabled = true;
                ischecked = false;
                nowYouCanUseIt();
            }
            else
            {
                timer1.Stop();
                tablePanel1.Enabled = false;
                ischecked = true;
                youCantUseIt();
                StopOrRestartTimeButton.Enabled = true;
            }
            checkTime = ischecked;

        }
    }
}
