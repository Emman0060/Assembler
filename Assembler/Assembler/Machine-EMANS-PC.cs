using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.DesignerServices;
using System.Runtime.Hosting;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Assembler
{
    public partial class Machine : Form
    {
        private List<int> ListedMachineCode;
        Label[] MainMemoryArray = new Label[129];
        Label[] MainMemoryArrayValue = new Label[129];
        Label[] RegistersArrayLabel = new Label[8]; //dw
        Label[] RegistersArrayValueLabel = new Label[8]; //dw
        Label[] GeneralRegistersArrayLabel = new Label[16];
        Label[] GeneralRegistersArrayValueLabel = new Label[16];
        Dictionary<string, int> LabelsAndValues = new Dictionary<string, int>();
        Dictionary<string, int> BranchingLabels = new Dictionary<string, int>();
        List<string> NewUserCode = new List<string>();
        Dictionary<string, int> InstructionAndBitPatterns = new Dictionary<string, int>()
        {
            {"LDR", 0b_0100000},
            {"STR", 0b_0100010},
            {"ADD", 0b_0001000},
            {"SUB", 0b_0001010},
            {"MOV", 0b_0100100},
            {"CMP", 0b_1001000},
            {"B", 0b_0010000},
            {"BEQ", 0b_0010010},
            {"BGT",0b_0010110},
            {"BNE",  0b_0010100},
            {"BLT",  0b_0010111},
            {"AND", 0b_1000000},
            {"ORR", 0b_1000010},
            {"EOR", 0b_1000100},
            {"MVN",  0b_1000110},
            {"LSL", 0b_0001100},
            {"LSR", 0b_0001110},
            {"HALT", 0b_1001010},
            {"INP", 0b_1001100},
            {"LDRM", 0b_1001101 }
        };
        string[] RegistersLabel = { "PC: ", "CIR: ", "MAR: ", "MDR: ", "ACC: ", "INPUT: ", "OUTPUT: " };

        private string[] AQAInstructionSet = { "LDR", "STR", "ADD", "SUB", "MOV", "CMP", "B", "BEQ", "BGT", "BNE", "BLT", "AND", "ORR", "EOR", "MVN", "LSL", "LSR", "HALT", "INP", "LDRM" };
        private int[] AQABinaryPatterns = new int[] { 0b_0100000, 0b_0100010, 0b_0001000, 0b_0001010, 0b_0100100, 0b_1001000, 0b_0010000, 0b_0010010, 0b_0010110, 0b_0010100, 0b_0010111, 0b_1000000, 0b_1000010, 0b_1000100, 0b_1000110, 0b_0001100, 0b_0001110, 0b_1001010, 0b_1001100, 0b_1001101 };
        int[] RegisterArray = new int[8];
        int ProgramCounter;
        int MemoryAddressRegister;
        int MemoryDataRegister;
        int MemoryBufferRegister;
        int CurrentInstructionRegister;
        int Accumulator;
        int[] GeneralRegisterArray = new int[16];
        List<string> LabelsNames = new List<string>();
        int[] MemoryArray = new int[129];
        private string SourceCode;
        string[] Tokens;
        bool ProgramIsRunning = true;
        bool IsDenaryMode = false;
        public Machine()
        {
            InitializeComponent();
            DrawMainMemory();
            DrawRegisters();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private string[] TokeniseSourceCode()
        {
            string[] Tokens = GetSourceCode().Split();
            return Tokens;
        }


        public List<int> ValidSyntax()
        {
            string[] SourceCodeArray = TokeniseSourceCode();
            bool IsValid = true;
            int LineCount = 0;
            int TotalErrors = 0;
            int listcount = 0;
            List<int> LineOfErrorsAndNumberOfErrors = new List<int>();
            ResolveLabels();

            for (int i = 0; i < SourceCodeArray.Length; i++)
            {
                if (SourceCodeArray[i] == "LDRM") ///LOAD RAM
                {
                    if (!Regex.IsMatch(SourceCodeArray[i + 2], @"^[#]\d+$") || !Regex.IsMatch(SourceCodeArray[i + 2], @"^[?]\d+$"))
                    {
                        IsValid = false;
                        LineOfErrorsAndNumberOfErrors.Add(LineCount);
                        TotalErrors++;
                    }
                    LineCount++;
                }
                if (SourceCodeArray[i] == "INP")
                {
                    if (!SourceCodeArray[i + 1].Contains('R') || !Regex.IsMatch(SourceCodeArray[i + 2], @"^[#,?]\d+$"))
                    {
                        IsValid = false;
                        LineOfErrorsAndNumberOfErrors.Add(LineCount);
                        TotalErrors++;
                    }
                    LineCount++;
                }
                if (SourceCodeArray[i] == "LDR")
                {
                    if (!SourceCodeArray[i + 1].Contains('R') || !Regex.IsMatch(SourceCodeArray[i + 2], @"^\d+$"))
                    {
                        IsValid = false;
                        LineOfErrorsAndNumberOfErrors.Add(LineCount);
                        TotalErrors++;
                    }
                    LineCount++;
                }
                if (SourceCodeArray[i] == "STR")
                {
                    if (!SourceCodeArray[i + 1].Contains('R') || !Regex.IsMatch(SourceCodeArray[i + 2], @"^\d+$"))
                    {
                        IsValid = false;
                        LineOfErrorsAndNumberOfErrors.Add(LineCount);
                        TotalErrors++;
                    }
                    LineCount++;
                }
                if (SourceCodeArray[i] == "ADD")
                {
                    if (!SourceCodeArray[i + 1].Contains('R') || !SourceCodeArray[i + 2].Contains('R') || !Regex.IsMatch(SourceCodeArray[i + 3], @"^\d+$"))
                    {
                        IsValid = false;
                        LineOfErrorsAndNumberOfErrors.Add(LineCount);
                        TotalErrors++;
                    }
                    LineCount++;
                }
                if (SourceCodeArray[i] == "SUB")
                {
                    if (!SourceCodeArray[i + 1].Contains('R') || !SourceCodeArray[i + 2].Contains('R') || !Regex.IsMatch(SourceCodeArray[i + 3], @"^\d+$"))
                    {
                        IsValid = false;
                        LineOfErrorsAndNumberOfErrors.Add(LineCount);
                        TotalErrors++;
                    }
                    LineCount++;
                }
                if (SourceCodeArray[i] == "MOV")
                {
                    if (!SourceCodeArray[i + 1].Contains('R') || !Regex.IsMatch(SourceCodeArray[i + 2], @"^\d+$"))
                    {
                        IsValid = false;
                        LineOfErrorsAndNumberOfErrors.Add(LineCount);
                        TotalErrors++;
                    }
                    LineCount++;
                }
                if (SourceCodeArray[i] == "CMP")
                {
                    if (!SourceCodeArray[i + 1].Contains('R') || !Regex.IsMatch(SourceCodeArray[i + 2], @"^\d+$"))
                    {
                        IsValid = false;
                        LineOfErrorsAndNumberOfErrors.Add(LineCount);
                        TotalErrors++;
                    }
                    LineCount++;
                }
                if (SourceCodeArray[i] == "BGT" || SourceCodeArray[i] == "BEQ" || SourceCodeArray[i] == "BNE" || SourceCodeArray[i] == "BLT")
                {
                    if (!BranchingLabels.ContainsKey(SourceCodeArray[i + 1]))
                    {
                        IsValid = false;
                        LineOfErrorsAndNumberOfErrors.Add(LineCount);
                        TotalErrors++;
                    }
                    LineCount++;
                }
                if (SourceCodeArray[i] == "B")
                {
                    if (!BranchingLabels.ContainsKey(SourceCodeArray[i + 1]))
                    {
                        IsValid = false;
                        LineOfErrorsAndNumberOfErrors.Add(LineCount);
                        TotalErrors++;
                    }
                    LineCount++;
                }
                if (SourceCodeArray[i] == "AND")
                {
                    if (!SourceCodeArray[i + 1].Contains('R') || !SourceCodeArray[i + 2].Contains('R') || !Regex.IsMatch(SourceCodeArray[i + 3], @"^\d+$"))
                    {
                        IsValid = false;
                        LineOfErrorsAndNumberOfErrors.Add(LineCount);
                        TotalErrors++;
                    }
                    LineCount++;
                }
                if (SourceCodeArray[i] == "ORR")
                {
                    if (!SourceCodeArray[i + 1].Contains('R') || !SourceCodeArray[i + 2].Contains('R') || !Regex.IsMatch(SourceCodeArray[i + 3], @"^\d+$"))
                    {
                        IsValid = false;
                        LineOfErrorsAndNumberOfErrors.Add(LineCount);
                        TotalErrors++;
                    }
                    LineCount++;
                }
                if (SourceCodeArray[i] == "EOR")
                {
                    if (!SourceCodeArray[i + 1].Contains('R') || !SourceCodeArray[i + 2].Contains('R') || !Regex.IsMatch(SourceCodeArray[i + 3], @"^\d+$"))
                    {
                        IsValid = false;
                        LineOfErrorsAndNumberOfErrors.Add(LineCount);
                        TotalErrors++;
                    }
                    LineCount++;
                }
                if (SourceCodeArray[i] == "MVN")
                {
                    if (!SourceCodeArray[i + 1].Contains('R') || !Regex.IsMatch(SourceCodeArray[i + 2], @"^\d+$"))
                    {
                        IsValid = false;
                        LineOfErrorsAndNumberOfErrors.Add(LineCount);
                        TotalErrors++;
                    }
                    LineCount++;
                }
                if (SourceCodeArray[i] == "LSL")
                {
                    if (!SourceCodeArray[i + 1].Contains('R') || !SourceCodeArray[i + 2].Contains('R') || !Regex.IsMatch(SourceCodeArray[i + 3], @"^\d+$"))
                    {
                        IsValid = false;
                        LineOfErrorsAndNumberOfErrors.Add(LineCount);
                        TotalErrors++;
                    }
                    LineCount++;
                }
                if (SourceCodeArray[i] == "LSR")
                {
                    if (!SourceCodeArray[i + 1].Contains('R') || !SourceCodeArray[i + 2].Contains('R') || !Regex.IsMatch(SourceCodeArray[i + 3], @"^\d+$"))
                    {
                        IsValid = false;
                        LineOfErrorsAndNumberOfErrors.Add(LineCount);
                        TotalErrors++;
                    }
                    LineCount++;
                }
                if (SourceCodeArray[i] == "HALT")
                {
                    IsValid = true;
                }
            }
            LineOfErrorsAndNumberOfErrors.Add(TotalErrors);
            return LineOfErrorsAndNumberOfErrors;
        }

        private void DrawRegisters()
        {
            string[] RegistersLabel = { "PC: ", "CIR: ", "MAR: ", "MDR: ", "ACC: ", "INPUT: ", "OUTPUT: " };

            int count = 0;
            int y = 0;
            int x = 0;

            ///labels
            for (int i = 0; i < RegistersLabel.Length; i++)
            {
                y += 30;

                RegistersArrayLabel[i] = new Label();
                RegistersArrayLabel[i].Text = RegistersLabel[count];
                RegistersArrayLabel[i].Padding = new Padding(2, 2, 2, 2);
                RegistersArrayLabel[i].Size = new Size(80, 25);
                RegistersArrayLabel[i].TextAlign = System.Drawing.ContentAlignment.BottomCenter;
                RegistersArrayLabel[i].Font = new Font(Label.DefaultFont, FontStyle.Bold);
                RegistersArrayLabel[i].Location = new Point(x + 10, y);
                RegistersPanel.Controls.Add(RegistersArrayLabel[i]);
                if (count < 6)
                {
                    count++;
                }
            }

            ///values
            for (int i = 0; i < RegistersArrayValueLabel.Length - 1; i++)
            {
                y += 31;

                RegistersArrayValueLabel[i] = new Label();
                RegistersArrayValueLabel[i].Text = "0";
                RegistersArrayValueLabel[i].Padding = new Padding(2, 2, 2, 2);
                RegistersArrayValueLabel[i].Size = new Size(80, 25);
                RegistersArrayValueLabel[i].BackColor = Color.White;
                RegistersArrayValueLabel[i].Location = new Point(x + 100, y - 210);
                RegistersPanel.Controls.Add(RegistersArrayValueLabel[i]);
            }

            for (int i = 0; i < GeneralRegistersArrayLabel.Length; i++)
            {
                if (i % 4 == 0)
                {
                    y += 70;
                    x = 0;
                }
                x += 72;

                GeneralRegistersArrayLabel[i] = new Label();
                GeneralRegistersArrayLabel[i].Name = "Memory" + i;
                GeneralRegistersArrayLabel[i].Text = Convert.ToString(i);
                GeneralRegistersArrayLabel[i].Font = new Font(Label.DefaultFont, FontStyle.Bold);
                GeneralRegistersArrayLabel[i].Size = new Size(80, 25);
                GeneralRegistersArrayLabel[i].TextAlign = System.Drawing.ContentAlignment.BottomCenter;
                GeneralRegistersArrayLabel[i].Padding = new Padding(2, 2, 2, 2);
                GeneralRegistersArrayLabel[i].Location = new Point(x - 70, y - 90);
                RegistersPanel.Controls.Add(GeneralRegistersArrayLabel[i]);
            }

            for (int i = 0; i < GeneralRegistersArrayValueLabel.Length; i++)
            {
                if (i % 4 == 0)
                {
                    y += 70;
                    x = 0;
                }
                x += 72;

                GeneralRegistersArrayValueLabel[i] = new Label();
                GeneralRegistersArrayValueLabel[i].Name = "Memory" + i;
                GeneralRegistersArrayValueLabel[i].Text = "0";
                GeneralRegistersArrayValueLabel[i].TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                GeneralRegistersArrayValueLabel[i].Size = new Size(60, 25);
                GeneralRegistersArrayValueLabel[i].Padding = new Padding(2, 2, 2, 2);
                GeneralRegistersArrayValueLabel[i].BackColor = Color.White;
                GeneralRegistersArrayValueLabel[i].Location = new Point(x - 70, y - 345);
                RegistersPanel.Controls.Add(GeneralRegistersArrayValueLabel[i]);
            }
        }

        private void DrawMainMemory()
        {
            int y = 0;
            int x = 10;

            for (int i = 0; i < MainMemoryArray.Length; i++)
            {
                if (i % 8 == 0)
                {
                    y += 105;
                    x = 0;
                }

                x += 118;
                MainMemoryArray[i] = new Label();
                MainMemoryArray[i].Name = "Memory" + i;
                MainMemoryArray[i].Text = Convert.ToString(i);
                MainMemoryArray[i].Font = new Font(Label.DefaultFont, FontStyle.Bold);
                MainMemoryArray[i].Size = new Size(115, 40);
                MainMemoryArray[i].Padding = new Padding(2, 2, 2, 2);
                MainMemoryArray[i].Location = new Point(x - 90, y - 96);
                MainMemoryArray[i].TextAlign = System.Drawing.ContentAlignment.BottomCenter;
                MainMemoryPanel.Controls.Add(MainMemoryArray[i]);

                MainMemoryArrayValue[i] = new Label();
                MainMemoryArrayValue[i].Name = "Memory" + i;
                MainMemoryArrayValue[i].Text = "0";
                MainMemoryArrayValue[i].Size = new Size(115, 40);
                MainMemoryArrayValue[i].Padding = new Padding(2, 2, 2, 2);
                MainMemoryArrayValue[i].BackColor = Color.White;
                MainMemoryArrayValue[i].Location = new Point(x - 90, y - 56);
                MainMemoryArrayValue[i].TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                MainMemoryPanel.Controls.Add(MainMemoryArrayValue[i]);
            }



        }

        public string GetSourceCode()
        {
                SourceCode = GetText().Trim();
                return SourceCode;
        }

        private string GetText()
        {
            string UserText = UserInputBox.Text;
            return UserText;
        }

        private string[] Tokenisation()
        {

            string NormalisedInput = GetText().ToUpper();
            List<string> TMPPreNormalisedInput = NormalisedInput.Split(new char[] { ' ' }).ToList();
            List<string> PreNormalisedInput = new List<string>();
            string tmp = "";

            string[] PreNormalised = UserInputBox.Text.Split('\n');

            return PreNormalised;
        }

        private void LoadIntoRAM()
        {

            string DenaryOfInsructions = "";
            string tmp = "";
            int tmp2 = 0;
            int CurrentAddress = 0;
            int index = 0;
            List<string> Labels = new List<string>();
            for (int i = 0; i < UserInputBox.Lines.Count(); i++)
            {
                string[] LineWords = UserInputBox.Lines[i].Trim().Split(' ');

                for (int j = 0; j < LineWords.Length; j++)
                {
                    if (AQAInstructionSet.Contains(LineWords[j]))
                    {
                        for (int t = 0; t < AQAInstructionSet.Length; t++)
                        {
                            if (AQAInstructionSet[t] == LineWords[j])
                            {
                                index = t;
                            }

                        }
                        DenaryOfInsructions += (AQABinaryPatterns[index]);
                        Console.WriteLine(DenaryOfInsructions);
                    }

                    if (LineWords[j].Substring(0, 1) == "R")
                    {
                        tmp = LineWords[j].Substring(1);
                        tmp2 = Convert.ToInt32(tmp) + 15;
                        DenaryOfInsructions += (Convert.ToInt32(tmp2));
                        Console.WriteLine(DenaryOfInsructions);
                    }

                    if (LineWords[j].Substring(0, 1) != "R" && !AQAInstructionSet.Contains(LineWords[j]))
                    {
                        DenaryOfInsructions += LineWords[j];
                    }


                }

                MainMemoryArrayValue[CurrentAddress].Text += DenaryOfInsructions;

                CurrentAddress++;
                DenaryOfInsructions = "";
            }
            CurrentAddress = 0;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlgOpen = new OpenFileDialog())
            {
                try
                {
                    dlgOpen.Filter = "All files(*.*)|*.*";
                    dlgOpen.InitialDirectory = "D:";
                    dlgOpen.Title = "Open";
                    if (dlgOpen.ShowDialog() == DialogResult.OK)
                    {
                        StreamReader sr = new StreamReader(dlgOpen.FileName, Encoding.Default);
                        string str = sr.ReadToEnd();
                        sr.Close();
                        UserInputBox.Text = str;
                    }
                }
                catch (Exception errorMsg)
                {
                    MessageBox.Show(errorMsg.Message);
                }
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
             
        }

        private void UserInputBox_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void saveASToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveFileDialog SaveSourceCode = new SaveFileDialog();

            SaveSourceCode.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            SaveSourceCode.FilterIndex = 2;
            SaveSourceCode.RestoreDirectory = true;

            if (SaveSourceCode.ShowDialog() == DialogResult.OK)
            {
                System.IO.StreamWriter file = new System.IO.StreamWriter(SaveSourceCode.FileName.ToString());
                file.WriteLine(UserInputBox.Text);
                file.Close();
            }
        }

        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlgOpen = new OpenFileDialog())
            {
                try
                {
                    dlgOpen.Filter = "All files(*.*)|*.*";
                    dlgOpen.InitialDirectory = "D:";
                    dlgOpen.Title = "Open";
                    if (dlgOpen.ShowDialog() == DialogResult.OK)
                    {
                        StreamReader sr = new StreamReader(dlgOpen.FileName, Encoding.Default);
                        string str = sr.ReadToEnd();
                        sr.Close();
                        // Show the text in the rich textbox rtbMain
                        UserInputBox.Text = str;
                    }
                }
                catch (Exception errorMsg)
                {
                    MessageBox.Show(errorMsg.Message);
                }
            }
        }

        private void checkErrorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ErrorMenuTextBox.Clear();

            List<int> LineOfErrorsAndNumberOfErrors = ValidSyntax();
            ErrorMenuTextBox.Text += "Total Number of Errors: " + LineOfErrorsAndNumberOfErrors[LineOfErrorsAndNumberOfErrors.Count - 1] + "\n" + "\n";

            for (int i = 0; i < LineOfErrorsAndNumberOfErrors.Count - 1; i++)
            {
                ErrorMenuTextBox.Text += "There are errors on line: " + LineOfErrorsAndNumberOfErrors[i] + "\n";
            }
        }

        private void toolStripButton3_Click_1(object sender, EventArgs e)
        {
            List<int> LineOfErrorsAndNumberOfErrors = ValidSyntax();

            if (LineOfErrorsAndNumberOfErrors[LineOfErrorsAndNumberOfErrors.Count - 1] == 0)
            {
                //Run();
            }
            else
            {
                Console.WriteLine("Error in code");
            }
        }

        private void DisplayErrors()
        {

            List<int> LineOfErrorsAndNumberOfErrors = ValidSyntax();
            ErrorMenuTextBox.Text = "";

            if (!ValidateLabels())
            {
                ErrorMenuTextBox.Text += "Error when resolving labels";
                ErrorMenuTextBox.Text += "\n";
            }

            if (LineOfErrorsAndNumberOfErrors[LineOfErrorsAndNumberOfErrors.Count - 1] == 0)
            {
                ErrorMenuTextBox.Text += "No other errors found";
                ErrorMenuTextBox.Text += "\n";
            }

            else
            {
                for (int i = 0; i < LineOfErrorsAndNumberOfErrors[LineOfErrorsAndNumberOfErrors.Count - 1]; i++)
                {
                    ErrorMenuTextBox.Text += "\n" + "Error on line : " + LineOfErrorsAndNumberOfErrors[i];
                }
            }
        }

        private void checkErrorsToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            DisplayErrors();
        }

        private void AssembleInstructions()
        {
            string MachinePattern = "";
            List<string> TheLabels = ResolveLabels();

            if (ValidateLabels())
            {
                System.Windows.Forms.MessageBox.Show("Labels have been resolved successfully");
                DisplayMachineCode();
            }

            else
            {
                System.Windows.Forms.MessageBox.Show("Labels have not been resolved");
            }
        }

        private bool ValidateLabels()
        {

            string[] ListOfMnemonicsTMP = TokeniseSourceCode();
            List<string> ListOfMnemonics = new List<string>();
            List<string> VerifyLabels = new List<string>();
            List<string> DuplicatesVerifyLabels = new List<string>();


            for (int i = 0; i < ListOfMnemonicsTMP.Length; i++) //GETS RID OF EMPTY SPACES
            {
                if (ListOfMnemonicsTMP[i] != "")
                {
                    ListOfMnemonics.Add(ListOfMnemonicsTMP[i]);
                }
            }

            for (int i = 0; i < ListOfMnemonics.Count; i++)
            {
                if (!AQAInstructionSet.Contains(ListOfMnemonics[i]) && ListOfMnemonics[i].Substring(0, 1) != "R" && ListOfMnemonics[i].Substring(0, 1) != "#" && !char.IsDigit(ListOfMnemonics[i][0]))
                {
                    VerifyLabels.Add(ListOfMnemonics[i]);
                }
            }

            int count = 0;
            string tmp = "";
            for (int i = 0; i < VerifyLabels.Count; i++) // LOOP LOOP
            {
                if (!DuplicatesVerifyLabels.Contains(VerifyLabels[i])) //making a duplicate lsit to check g
                {
                    DuplicatesVerifyLabels.Add(VerifyLabels[i]);
                    count = 0;
                }

                else if (DuplicatesVerifyLabels.Contains(VerifyLabels[i]))
                {
                    count++;
                }
            }

            if (count == 1 || DuplicatesVerifyLabels.Count == 0)
            {
                return true;
            }

            else
            {
                return false;
            }
        }
        private List<string> ResolveLabels()
        {
            string[] UserCode = TokeniseSourceCode();
            UserCode = UserCode.Where(x => !string.IsNullOrEmpty(x)).ToArray();
            List<string> NewUserCode = new List<string>();
            List<string> LineOfInstructions = Tokenisation().ToList();
            int count = 0;

            for (int i = 0; i < UserCode.Length; i++)
            {
                if (AQAInstructionSet.Contains(UserCode[i]))
                {
                    count++;
                }
                if (!AQAInstructionSet.Contains(UserCode[i]) && !BranchingLabels.ContainsKey(UserCode[i]) && UserCode[i][0] != 'R' && int.TryParse(UserCode[i], out _) == false && UserCode[i][0] != '#') ///gets rid of labels
                {
                    BranchingLabels.Add(UserCode[i], count); ///save name of label and pos in array
                }

                else
                {
                    NewUserCode.Add(UserCode[i]);
                }
            }
            
            count = 0;

            for (int i = 0; i < UserCode.Length; i++)
            {
                if (AQAInstructionSet.Contains(UserCode[i]) || BranchingLabels.ContainsKey(UserCode[i]) )
                {
                    count++;
                }
                if (!AQAInstructionSet.Contains(UserCode[i]) && !LabelsAndValues.ContainsKey(UserCode[i]) && UserCode[i][0] != 'R' && int.TryParse(UserCode[i], out _) == false && UserCode[i][0] != '#') ///gets rid of labels
                {
                    LabelsAndValues.Add(UserCode[i], count); ///save name of label and pos in array
                }

                else
                {
                    NewUserCode.Add(UserCode[i]);
                }
            }
            return NewUserCode;


        }

        private void toolStripButton3_Click_2(object sender, EventArgs e)
        {
            
            ProgramIsRunning = true;
            Interpret();
        }

        private void fAQToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadIntoMemory();
        }

        //// <summary>
        //// address of the next instruction in PC in placed in the MAR
        //// Then the instruction is placed from memory into the MDR.
        //// The instruction is then placed into the CIR 
        //// Instruction is then decoded.
        //// Instruction is then  executed
        //// </summary>
        private async void Interpret()
        {
            string MachineCode;
            string TMPMachineCode;
            string Instruction;
            string TMPInstruction;
            string AddressingMode;
            string Register1;
            string Register2;
            string Operand1;

            for (ProgramCounter = 0; ProgramCounter < MemoryArray.Length; ProgramCounter++)
            {
                RegistersArrayValueLabel[0].Text = Convert.ToString(ProgramCounter);
                Console.WriteLine(ProgramCounter);
                MemoryAddressRegister = MemoryArray[ProgramCounter];
                MemoryDataRegister = MemoryAddressRegister;
                CurrentInstructionRegister = MemoryDataRegister;

                if (MemoryAddressRegister != 0 )
                {
                    MachineCode = Convert.ToString(MemoryArray[ProgramCounter],2);
                    TMPMachineCode = Convert.ToString(MemoryArray[ProgramCounter + 1], 2);
                    if (MachineCode.Length == 17)
                    {
                        MachineCode = "0" + MachineCode;
                    }

                    if(MachineCode.Length == 18)
                    {
                        Instruction = MachineCode.Substring(0, 7);
                        AddressingMode = MachineCode.Substring(7, 2);
                        Register1 = MachineCode.Substring(9, 5);
                        Operand1 = MachineCode.Substring(14, 4);

                        if (InstructionAndBitPatterns.ContainsValue(Convert.ToInt32(Instruction, 2)))
                        {
                            string TheInstruction = InstructionAndBitPatterns.FirstOrDefault(x => x.Value == Convert.ToInt32(Instruction, 2)).Key;
                           
                            //// 01 - direct //10 - immedidate // 11 - relative

                            if (TheInstruction == "INP")
                            {
                                if (AddressingMode == "01")
                                {
                                    int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                    GeneralRegisterArray[RegisterAddress] = GeneralRegisterArray[Convert.ToInt32(Operand1,2)];
                                    GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(GeneralRegisterArray[Convert.ToInt32(Operand1, 2)],2);
                                    await Task.Delay(1000);
                                }

                                if (AddressingMode == "10")
                                {
                                    int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                    GeneralRegisterArray[RegisterAddress] = Convert.ToInt32(Operand1, 2);
                                    GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(Convert.ToInt32(Operand1, 2),2);
                                    await Task.Delay(1000);
                                }

                                if (AddressingMode == "11")
                                {
                                    int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                    GeneralRegisterArray[RegisterAddress] = MemoryArray[Convert.ToInt32(Operand1, 2)];
                                    GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1, 2)],2);
                                    await Task.Delay(1000);
                                }
                            }
                            //// 01 - direct //10 - immedidate // 11 - relative

                            if (TheInstruction == "LDR")
                            {
                                if (AddressingMode == "01")
                                {
                                    int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                    GeneralRegisterArray[RegisterAddress] = MemoryArray[Convert.ToInt32(Operand1, 2)];
                                    GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1, 2)], 2);
                                    await Task.Delay(1000);
                                }

                                if (AddressingMode == "10")
                                {
                                    int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                    GeneralRegisterArray[RegisterAddress] = Convert.ToInt32(Operand1, 2);
                                    GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1, 2)], 2);
                                    await Task.Delay(1000);
                                }

                                if (AddressingMode == "11")
                                {
                                    int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                    GeneralRegisterArray[RegisterAddress] = MemoryArray[Convert.ToInt32(Operand1, 2)];
                                    GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1, 2)], 2);
                                    await Task.Delay(1000);
                                }
                            }
                            if (TheInstruction == "LDRM")
                            {
                                if (AddressingMode == "01")
                                {
                                    int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                    MemoryArray[RegisterAddress] = GeneralRegisterArray[Convert.ToInt32(Register1, 2) - 15];
                                    MainMemoryArrayValue[RegisterAddress].Text = Convert.ToString(GeneralRegisterArray[Convert.ToInt32(Register1, 2) - 15], 2);
                                    await Task.Delay(1000);
                                }

                                if (AddressingMode == "10")
                                {
                                    int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                    MemoryArray[RegisterAddress] = Convert.ToInt32(Operand1, 2);
                                    MainMemoryArrayValue[RegisterAddress].Text = Convert.ToString(Convert.ToInt32(Operand1, 2), 2);
                                    await Task.Delay(1000);
                                }

                                if (AddressingMode == "11")
                                {
                                    int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                    MemoryArray[RegisterAddress] = MemoryArray[Convert.ToInt32(Operand1, 2)];
                                    MainMemoryArrayValue[RegisterAddress].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1, 2)], 2);
                                    await Task.Delay(1000);
                                }
                            }

                            if (TheInstruction == "STR")
                            {
                                if (AddressingMode == "01")
                                {
                                    int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                    MemoryArray[Convert.ToInt32(Operand1, 2)] = GeneralRegisterArray[RegisterAddress];
                                    MainMemoryArrayValue[Convert.ToInt32(Operand1, 2)].Text = Convert.ToString(GeneralRegisterArray[RegisterAddress], 2);
                                    await Task.Delay(1000);
                                }

                                if (AddressingMode == "10")
                                {
                                    int RegisterAddress  = Convert.ToInt32(Register1, 2) - 15;
                                    MemoryArray[RegisterAddress] = Convert.ToInt32(Operand1, 2);
                                    MainMemoryArrayValue[RegisterAddress].Text = Convert.ToString(Convert.ToInt32(Operand1, 2));
                                    await Task.Delay(1000);

                                }

                                if (AddressingMode == "11")
                                {
                                    int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                    MemoryArray[RegisterAddress] = MemoryArray[Convert.ToInt32(Operand1, 2)];
                                    MainMemoryArrayValue[RegisterAddress].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1, 2)]);
                                    await Task.Delay(1000);
                                }
                            }
                            //// 01 - direct //10 - immedidate // 11 - relative

                            if (TheInstruction == "MOV")
                            {
                                if (AddressingMode == "01")
                                {
                                    int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                    GeneralRegisterArray[RegisterAddress] = GeneralRegisterArray[Convert.ToInt32(Operand1, 2)];
                                    GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(GeneralRegisterArray[Convert.ToInt32(Operand1, 2)],2);
                                    await Task.Delay(1000);
                                }

                                if (AddressingMode == "10")
                                {
                                    int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                    GeneralRegisterArray[RegisterAddress] = Convert.ToInt32(Operand1, 2);
                                    GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(Convert.ToInt32(Operand1, 2), 2);
                                    await Task.Delay(1000);
                                }

                                if (AddressingMode == "11")
                                {
                                    int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                    GeneralRegisterArray[RegisterAddress] = MemoryArray[Convert.ToInt32(Operand1)];
                                    GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1)], 2);
                                    await Task.Delay(1000);
                                }
                            }

                            if (TheInstruction == "CMP")
                            {
                                TMPInstruction = TMPMachineCode.Substring(0, 5);
                                string TMPTheInstruction = InstructionAndBitPatterns.FirstOrDefault(x => x.Value == Convert.ToInt32(TMPInstruction, 2)).Key;
                                string LocationString = TMPMachineCode.Substring(5, 4);

                                if (AddressingMode == "01")
                                {
                                    string BranchingInstruction = "00" + Convert.ToString(MemoryArray[ProgramCounter + 1], 2);
                                    int RegisterAddress = GeneralRegisterArray[Convert.ToInt32(Register1, 2) - 15];
                                    int CompareWith = GeneralRegisterArray[Convert.ToInt32(Operand1, 2)];

                                    if (InstructionAndBitPatterns.FirstOrDefault(x => x.Value == Convert.ToInt32(BranchingInstruction.Substring(0, 7), 2)).Key == "BEQ")
                                    {
                                        if ( RegisterAddress == CompareWith)
                                        {
                                            RegistersArrayValueLabel[0].Text = Convert.ToString(Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2));
                                            ProgramCounter = Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2) - 1;
                                        }

                                    }

                                    if (InstructionAndBitPatterns.FirstOrDefault(x => x.Value == Convert.ToInt32(BranchingInstruction.Substring(0, 7), 2)).Key == "BNE")
                                    {
                                        if (GeneralRegisterArray[RegisterAddress] != CompareWith)
                                        {
                                            RegistersArrayValueLabel[0].Text = Convert.ToString(Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2));
                                            ProgramCounter = Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2) - 1;
                                        }
                                    }

                                    if (InstructionAndBitPatterns.FirstOrDefault(x => x.Value == Convert.ToInt32(BranchingInstruction.Substring(0, 7), 2)).Key == "BGT")
                                    {
                                        if (GeneralRegisterArray[RegisterAddress] > CompareWith)
                                        {
                                            RegistersArrayValueLabel[0].Text = Convert.ToString(Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2));
                                            ProgramCounter = Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2) - 1;
                                        }
                                    }

                                    if (InstructionAndBitPatterns.FirstOrDefault(x => x.Value == Convert.ToInt32(BranchingInstruction.Substring(0, 7), 2)).Key == "BLT")
                                    {
                                        if (GeneralRegisterArray[RegisterAddress] < CompareWith)
                                        {
                                            RegistersArrayValueLabel[0].Text = Convert.ToString(Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2));
                                            ProgramCounter = Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2) - 1;
                                        }
                                    }
                                    await Task.Delay(1000);
                                }

                                if (AddressingMode == "10")
                                {
                                    string BranchingInstruction = "00" + Convert.ToString(MemoryArray[ProgramCounter + 1], 2);

                                    int RegisterAddress = GeneralRegisterArray[Convert.ToInt32(Register1, 2) - 15];
                                    int CompareWith = Convert.ToInt32(Operand1, 2);

                                    if (InstructionAndBitPatterns.FirstOrDefault(x => x.Value == Convert.ToInt32(BranchingInstruction.Substring(0, 7), 2)).Key == "BEQ")
                                    {
                                        if (RegisterAddress == CompareWith)
                                        {
                                            RegistersArrayValueLabel[0].Text = Convert.ToString(Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2));
                                            ProgramCounter = Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2) - 1;
                                        }

                                    }

                                    if (InstructionAndBitPatterns.FirstOrDefault(x => x.Value == Convert.ToInt32(BranchingInstruction.Substring(0, 7), 2)).Key == "BNE")
                                    {
                                        if (GeneralRegisterArray[RegisterAddress] != CompareWith)
                                        {
                                            RegistersArrayValueLabel[0].Text = Convert.ToString(Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2));
                                            ProgramCounter = Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2) - 1;
                                        }
                                    }

                                    if (InstructionAndBitPatterns.FirstOrDefault(x => x.Value == Convert.ToInt32(BranchingInstruction.Substring(0, 7), 2)).Key == "BGT")
                                    {
                                        if (GeneralRegisterArray[RegisterAddress] > CompareWith)
                                        {
                                            RegistersArrayValueLabel[0].Text = Convert.ToString(Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2));
                                            ProgramCounter = Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2) - 1;
                                        }
                                    }

                                    if (InstructionAndBitPatterns.FirstOrDefault(x => x.Value == Convert.ToInt32(BranchingInstruction.Substring(0, 7), 2)).Key == "BLT")
                                    {
                                        if (GeneralRegisterArray[RegisterAddress] < CompareWith)
                                        {
                                            RegistersArrayValueLabel[0].Text = Convert.ToString(Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2));
                                            ProgramCounter = Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2) - 1;
                                        }
                                    }
                                    await Task.Delay(1000);
                                }

                                if (AddressingMode == "11")
                                {
                                    string BranchingInstruction = "00" + Convert.ToString(MemoryArray[ProgramCounter + 1], 2);

                                    int RegisterAddress = GeneralRegisterArray[Convert.ToInt32(Register1, 2) - 15];
                                    int CompareWith = MemoryArray[Convert.ToInt32(Operand1, 2)];

                                    if (InstructionAndBitPatterns.FirstOrDefault(x => x.Value == Convert.ToInt32(BranchingInstruction.Substring(0, 7), 2)).Key == "BEQ")
                                    {
                                        if (RegisterAddress == CompareWith)
                                        {
                                            RegistersArrayValueLabel[0].Text = Convert.ToString(Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2));
                                            ProgramCounter = Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2) - 1;
                                        }

                                    }

                                    if (InstructionAndBitPatterns.FirstOrDefault(x => x.Value == Convert.ToInt32(BranchingInstruction.Substring(0, 7), 2)).Key == "BNE")
                                    {
                                        if (GeneralRegisterArray[RegisterAddress] != CompareWith)
                                        {
                                            RegistersArrayValueLabel[0].Text = Convert.ToString(Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2));
                                            ProgramCounter = Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2) - 1;
                                        }
                                    }

                                    if (InstructionAndBitPatterns.FirstOrDefault(x => x.Value == Convert.ToInt32(BranchingInstruction.Substring(0, 7), 2)).Key == "BGT")
                                    {
                                        if (GeneralRegisterArray[RegisterAddress] > CompareWith)
                                        {
                                            RegistersArrayValueLabel[0].Text = Convert.ToString(Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2));
                                            ProgramCounter = Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2) - 1;
                                        }
                                    }

                                    if (InstructionAndBitPatterns.FirstOrDefault(x => x.Value == Convert.ToInt32(BranchingInstruction.Substring(0, 7), 2)).Key == "BLT")
                                    {
                                        if (GeneralRegisterArray[RegisterAddress] < CompareWith)
                                        {
                                            RegistersArrayValueLabel[0].Text = Convert.ToString(Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2));
                                            ProgramCounter = Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2) - 1;
                                        }
                                    }
                                    await Task.Delay(1000);
                                }

                            }

                            //// 01 - direct //10 - immedidate // 11 - relative

                            if (TheInstruction == "MVN")
                            {
                                if (AddressingMode == "01")
                                {
                                    int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                    int result = ~GeneralRegisterArray[Convert.ToInt32(Operand1,2)];
                                    GeneralRegisterArray[RegisterAddress] = result;
                                    GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(result, 2);
                                    await Task.Delay(1000);
                                }

                                if (AddressingMode == "10")
                                {
                                    int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                    int result = ~Convert.ToInt32(Operand1, 2);
                                    GeneralRegisterArray[RegisterAddress] = result;
                                    GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(result, 2);
                                    await Task.Delay(1000);
                                }

                                if (AddressingMode == "11")
                                {
                                    int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                    int result = ~MemoryArray[Convert.ToInt32(Operand1, 2)];
                                    GeneralRegisterArray[RegisterAddress] = result;
                                    GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(result, 2);
                                    await Task.Delay(1000);

                                }
                            }

                        }

                    }

                    if (MachineCode.Length == 9)
                    {
                        MachineCode = "00" + MachineCode;

                        if (InstructionAndBitPatterns.FirstOrDefault(x => x.Value == Convert.ToInt32(MachineCode.Substring(0, 7), 2)).Key == "B")
                        {
                            RegistersArrayValueLabel[0].Text = Convert.ToString(Convert.ToInt32(MachineCode.Substring(7, 4), 2));
                            ProgramCounter = Convert.ToInt32(MachineCode.Substring(7, 4), 2)-2;
                            await Task.Delay(1000);
                            continue;
                        }
                    }

                    if (MachineCode.Length > 18)
                    {

                        if (MachineCode.Length == 20)
                        {
                            MachineCode = "000" + MachineCode;
                        }

                        Instruction = MachineCode.Substring(0, 7);

                        if (InstructionAndBitPatterns.ContainsValue(Convert.ToInt32(Instruction, 2)))
                        {
                            string TheInstruction = InstructionAndBitPatterns.FirstOrDefault(x => x.Value == Convert.ToInt32(Instruction, 2)).Key;
                            //// 01 - direct //10 - immedidate // 11 - relative


                            if (TheInstruction == "ADD")
                            {
                                int sum = 0;
                                Instruction = MachineCode.Substring(0, 7);
                                AddressingMode = MachineCode.Substring(7, 2);
                                Register1 = MachineCode.Substring(9, 5);
                                Register2 = MachineCode.Substring(14, 5);
                                Operand1 = MachineCode.Substring(19, 4);

                                int RegisterAddress1 = Convert.ToInt32(Register1, 2) - 15;
                                int RegisterAddress2 = Convert.ToInt32(Register2, 2) - 15;


                                if (AddressingMode == "01")
                                {
                                    sum = GeneralRegisterArray[RegisterAddress2] + GeneralRegisterArray[Convert.ToInt32(Operand1, 2)];
                                    GeneralRegisterArray[RegisterAddress1] = sum;
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(sum, 2);
                                    await Task.Delay(1000);
                                }

                                if (AddressingMode == "10")
                                {
                                    sum = GeneralRegisterArray[RegisterAddress2] + Convert.ToInt32(Operand1, 2);
                                    GeneralRegisterArray[RegisterAddress1] = sum;
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(sum,2);
                                    await Task.Delay(1000);
                                }

                                if (AddressingMode == "11")
                                {
                                    sum = GeneralRegisterArray[RegisterAddress2] + MemoryArray[Convert.ToInt32(Operand1, 2)];
                                    GeneralRegisterArray[RegisterAddress1] = sum;
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(sum, 2);
                                    await Task.Delay(1000);
                                }
                            }

                            if (TheInstruction == "LSL")
                            {

                                int result = 0;
                                Instruction = MachineCode.Substring(0, 7);
                                AddressingMode = MachineCode.Substring(7, 2);
                                Register1 = MachineCode.Substring(9, 5);
                                Register2 = MachineCode.Substring(14, 5);
                                Operand1 = MachineCode.Substring(19, 4);

                                int RegisterAddress1 = Convert.ToInt32(Register1, 2) - 15;
                                int RegisterAddress2 = Convert.ToInt32(Register2, 2) - 15;
                                
                                if (AddressingMode == "01")
                                {
                                    result = RegisterAddress2 << GeneralRegisterArray[Convert.ToInt32(Operand1, 2)];
                                    GeneralRegisterArray[RegisterAddress1] = result;
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(result, 2);
                                    await Task.Delay(1000);
                                }

                                if (AddressingMode == "10")
                                {
                                    result = RegisterAddress2 << Convert.ToInt32(Operand1, 2);
                                    GeneralRegisterArray[RegisterAddress1] = result;
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(result, 2);
                                    await Task.Delay(1000);
                                }

                                if (AddressingMode == "11")
                                {
                                    result = RegisterAddress2 << MemoryArray[Convert.ToInt32(Operand1, 2)];
                                    GeneralRegisterArray[RegisterAddress1] = result;
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(result, 2);
                                    await Task.Delay(1000);
                                }
                            }

                            if (TheInstruction == "LSR")
                            {
                                int result = 0;
                                Instruction = MachineCode.Substring(0, 7);
                                AddressingMode = MachineCode.Substring(7, 2);
                                Register1 = MachineCode.Substring(9, 5);
                                Register2 = MachineCode.Substring(14, 5);
                                Operand1 = MachineCode.Substring(19, 4);

                                int RegisterAddress1 = Convert.ToInt32(Register1, 2) - 15;
                                int RegisterAddress2 = Convert.ToInt32(Register2, 2) - 15;

                                if (AddressingMode == "01")
                                {
                                    result = RegisterAddress2 >> GeneralRegisterArray[Convert.ToInt32(Operand1, 2)];
                                    GeneralRegisterArray[RegisterAddress1] = result;
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(result, 2);
                                    await Task.Delay(1000);
                                }

                                if (AddressingMode == "10")
                                {
                                    result = RegisterAddress2 >> Convert.ToInt32(Operand1, 2);
                                    GeneralRegisterArray[RegisterAddress1] = result;
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(result, 2);
                                    await Task.Delay(1000);
                                }

                                if (AddressingMode == "11")
                                {
                                    result = RegisterAddress2 >> MemoryArray[Convert.ToInt32(Operand1, 2)];
                                    GeneralRegisterArray[RegisterAddress1] = result;
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(result, 2);
                                    await Task.Delay(1000);
                                }
                            }

                            if (TheInstruction == "AND")
                            {
                                int result;
                                Instruction = MachineCode.Substring(0, 7);
                                AddressingMode = MachineCode.Substring(7, 2);
                                Register1 = MachineCode.Substring(9, 5);
                                Register2 = MachineCode.Substring(14, 5);
                                Operand1 = MachineCode.Substring(19, 4);

                                if (AddressingMode == "01")
                                {
                                    result = GeneralRegisterArray[Convert.ToInt32(Register2,2)-15] & GeneralRegisterArray[Convert.ToInt32(Operand1, 2)];
                                    GeneralRegisterArray[Convert.ToInt32(Register1, 2)-15] = result;
                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2)-15].Text = Convert.ToString(result,2);
                                    await Task.Delay(1000);
                                }

                                if (AddressingMode == "10")
                                {
                                    result = GeneralRegisterArray[Convert.ToInt32(Register2, 2)] & Convert.ToInt32(Operand1, 2);
                                    GeneralRegisterArray[Convert.ToInt32(Register1, 2)] = result;
                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2)].Text = Convert.ToString(result,2);
                                    await Task.Delay(1000);
                                }

                                if (AddressingMode == "11")
                                {
                                    result = GeneralRegisterArray[Convert.ToInt32(Register2, 2)] & MemoryArray[Convert.ToInt32(Operand1, 2)];
                                    GeneralRegisterArray[Convert.ToInt32(Register1, 2)] = result;
                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2)].Text = Convert.ToString(result, 2);
                                    await Task.Delay(1000);

                                }
                            }

                            if (TheInstruction == "ORR")///not working.wrong base idk error
                            {
                                int result = 0;
                                Instruction = MachineCode.Substring(0, 7);
                                AddressingMode = MachineCode.Substring(7, 2);
                                Register1 = MachineCode.Substring(9, 5);
                                Register2 = MachineCode.Substring(14, 5);
                                Operand1 = MachineCode.Substring(19, 4);

                                if (AddressingMode == "01")
                                {
                                    result = GeneralRegisterArray[Convert.ToInt32(Register2, 1)] | GeneralRegisterArray[Convert.ToInt32(Operand1,2)];
                                    GeneralRegisterArray[Convert.ToInt32(Register1, 2)] = result;
                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2)].Text = Convert.ToString(result, 2);
                                    await Task.Delay(1000);
                                }

                                if (AddressingMode == "10")
                                {
                                    result = GeneralRegisterArray[Convert.ToInt32(Register2,1)] | Convert.ToInt32(Operand1, 2);
                                    GeneralRegisterArray[Convert.ToInt32(Register1,2)] = result;
                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2)].Text = Convert.ToString(result);
                                    await Task.Delay(1000);
                                }

                                if (AddressingMode == "11")
                                {
                                    result = GeneralRegisterArray[Convert.ToInt32(Register2, 1)] | MemoryArray[Convert.ToInt32(Operand1, 2)];
                                    GeneralRegisterArray[Convert.ToInt32(Register1, 2)] = result;
                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2)].Text = Convert.ToString(result, 2);
                                    await Task.Delay(1000);
                                }
                            }

                            if (TheInstruction == "EOR")
                            {
                                int sum = 0;
                                Instruction = MachineCode.Substring(0, 7);
                                AddressingMode = MachineCode.Substring(7, 2);
                                Register1 = MachineCode.Substring(9, 5);
                                Register2 = MachineCode.Substring(14, 5);
                                Operand1 = MachineCode.Substring(19, 4);

                                if (AddressingMode == "01")
                                {

                                }

                                if (AddressingMode == "10")
                                {

                                }

                                if (AddressingMode == "11")
                                {

                                }
                            }

                            if (TheInstruction == "SUB")
                            {
                                int sum = 0;
                                Instruction = MachineCode.Substring(0, 7);
                                AddressingMode = MachineCode.Substring(7, 2);
                                Register1 = MachineCode.Substring(9, 5);
                                Register2 = MachineCode.Substring(14, 5);
                                Operand1 = MachineCode.Substring(19, 4);

                                int RegisterAddress1 = Convert.ToInt32(Register1, 2) - 15;
                                int RegisterAddress2 = Convert.ToInt32(Register2, 2) - 15;

                                if (AddressingMode == "01")
                                {
                                    sum = GeneralRegisterArray[RegisterAddress2] - GeneralRegisterArray[Convert.ToInt32(Operand1, 2)];
                                    GeneralRegisterArray[RegisterAddress1] = sum;
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(sum, 2);
                                    await Task.Delay(1000);
                                }

                                if (AddressingMode == "10")
                                {
                                    sum = GeneralRegisterArray[RegisterAddress2] - Convert.ToInt32(Operand1, 2);
                                    GeneralRegisterArray[RegisterAddress1] = sum;
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(sum, 2);
                                    await Task.Delay(1000);
                                }

                                if (AddressingMode == "11")
                                {
                                    sum = GeneralRegisterArray[RegisterAddress2] - MemoryArray[Convert.ToInt32(Operand1, 2)];
                                    GeneralRegisterArray[RegisterAddress1] = sum;
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(sum, 2);
                                    await Task.Delay(1000);
                                }
                            }

                        }
                    }

                }
                else
                {
                    break;
                }
            }
        }

        private void LoadIntoMemory()
        {
            string tmp;
            int MachinePattern = 0;
            List<string> LineItems = new List<string>();
            List<string> LineOfInstructions = Tokenisation().ToList();
            string word = "";
            int MemoryAddress = 0;
            bool IsImmediate = false;
            bool IsDirect = false;
            bool IsRelative = false;

            ResolveLabels();
            for (int i = 0; i < LineOfInstructions.Count; i++)
            {

                LineItems = LineOfInstructions[i].Split(' ').ToList();

                if (LineItems[LineItems.Count-1].Contains("#"))
                {
                    IsRelative = false;
                    IsDirect = false;
                    IsImmediate = true;
                }

                else if (!LineItems[LineItems.Count - 1].Contains("#") && !LineItems[LineItems.Count - 1].Contains("?")) ////relative addressing ?
                {
                    IsDirect = true;
                    IsImmediate = false;
                    IsRelative = false;
                }

                else if (LineItems[LineItems.Count-1].Contains("?"))
                {
                    IsRelative = true;
                    IsDirect = false;
                    IsImmediate = false;
                }
                //// 01 - direct //10 - immedidate // 11 - relative
                for (int j = 0; j < LineItems.Count; j++)
                {
                    if (InstructionAndBitPatterns.ContainsKey(LineItems[j]))
                    {
                        MachinePattern += Convert.ToInt32(InstructionAndBitPatterns[LineItems[j]]);

                        if (IsRelative && LineItems[j] != "B" && LineItems[j] != "BGT" && LineItems[j] != "BNE" && LineItems[j] != "BLT" && LineItems[j] != "BEQ")
                        {
                            MachinePattern = MachinePattern << 2;
                            MachinePattern += 3;
                        }

                        if (IsImmediate && LineItems[j] != "B" && LineItems[j] != "BGT" && LineItems[j] != "BNE" && LineItems[j] != "BLT" && LineItems[j] != "BEQ")
                        {
                            MachinePattern = MachinePattern << 2;
                            MachinePattern += 2;
                        }

                        if (IsDirect && LineItems[j] != "B" && LineItems[j] != "BGT" && LineItems[j] != "BNE" && LineItems[j] != "BLT" && LineItems[j] != "BEQ")
                        {
                            MachinePattern = MachinePattern << 2;
                            MachinePattern += 1;
                        }
                        if (LineItems[j] == "B" || LineItems[j] == "BGT" || LineItems[j] == "BLT" || LineItems[j] == "BNE" || LineItems[j] == "BEQ")
                        {
                            MachinePattern = MachinePattern << 4;

                            MachinePattern += LabelsAndValues.FirstOrDefault(x => x.Key == LineItems[j+1]).Value;
                        }
                    }

                    else if (LineItems[j][0] == 'R' && !InstructionAndBitPatterns.ContainsKey(LineItems[j]))
                    {
                        MachinePattern = MachinePattern << 5;
                        MachinePattern += (Convert.ToInt32(LineItems[j].Substring(1)) + 15);
                    }

                    else if (LineItems[j][0] == '#')
                    {
                        MachinePattern = MachinePattern << 4;
                        MachinePattern += Convert.ToInt32(LineItems[j].Substring(1));
                    }

                    else if (LineItems[j][0] == '?')
                    {
                        MachinePattern = MachinePattern << 4;
                        MachinePattern += Convert.ToInt32(LineItems[j].Substring(1));
                    }

                    else if (char.IsDigit(LineItems[j][0]))
                    {
                        MachinePattern = MachinePattern << 4;
                        MachinePattern += Convert.ToInt32(LineItems[j]);
                    }

                    else if(!AQAInstructionSet.Contains(LineItems[j]) && !BranchingLabels.ContainsKey(LineItems[j]) && LineItems[j][0] != 'R' && int.TryParse(LineItems[j], out _) == false && LineItems[j][0] != '#')
                    {
                        MachinePattern = MachinePattern << 4;
                        MachinePattern += j;
                    }

                }

                MemoryArray[MemoryAddress] = MachinePattern;
                MemoryAddress++;

                MachinePattern = 0;
            }



            for (int i = 0; i < MemoryArray.Length; i++)
            {

                if (MemoryArray[i] == 0)
                {
                    break;
                }

                else
                {
                    MainMemoryArrayValue[i].Text = Convert.ToString(MemoryArray[i], 2);
                }

            }

        }
    
        private void DisplayMachineCode()
        {
            List<string> ListOfMnemonics = ResolveLabels();
            ListOfMnemonics = ListOfMnemonics.Where(x => !string.IsNullOrEmpty(x)).ToList();
            string MachinePattern = "";
            int count = 0;

            for (int i = 0; i < ListOfMnemonics.Count; i++)
            {
                count++;

                if (InstructionAndBitPatterns.ContainsKey(ListOfMnemonics[i]))
                {
                    MachinePattern += Convert.ToString(Convert.ToInt32(InstructionAndBitPatterns[ListOfMnemonics[i]]), 2);
                }

                else if (ListOfMnemonics[i][0] == 'R' && !InstructionAndBitPatterns.ContainsKey(ListOfMnemonics[i]))
                {
                    MachinePattern += Convert.ToString(Convert.ToInt32(ListOfMnemonics[i].Substring(1)) + 15, 2);
                }

                else if (ListOfMnemonics[i][0] == '#')
                {
                    MachinePattern += Convert.ToString(Convert.ToInt32(ListOfMnemonics[i].Substring(1)), 2);
                }

                else if (char.IsDigit(ListOfMnemonics[i][0]))
                {
                    MachinePattern += Convert.ToString(Convert.ToInt32(ListOfMnemonics[i]), 2);
                }

                if (char.IsDigit(ListOfMnemonics[i][0]) || ListOfMnemonics[i][0] == '#')
                {
                    MachineCodeTextBox.Text += MachinePattern;
                    MachineCodeTextBox.Text += "\r\n";
                    MachinePattern = "";
                }
            }
        }
       
        private void assembleCOdeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AssembleInstructions();
        }

        private void exitToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void toggleDenaryModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int tmp;
            int tmp2;
            if (!IsDenaryMode)
            {
                for (int i = 0; i < MemoryArray.Length; i++)
                {
                    tmp = MemoryArray[i];
                    MainMemoryArrayValue[i].Text = Convert.ToString(tmp);
                }
                for (int i = 0; i < GeneralRegisterArray.Length; i++)
                {
                    tmp2 = GeneralRegisterArray[i];
                    GeneralRegistersArrayValueLabel[i].Text = Convert.ToString(tmp2);
                }
                IsDenaryMode = true;
            }
            else
            {
                for (int i = 0; i < MemoryArray.Length; i++)
                {
                    tmp = MemoryArray[i];
                    MainMemoryArrayValue[i].Text = Convert.ToString(tmp,2);
                }
                for (int i = 0; i < GeneralRegisterArray.Length; i++)
                {
                    tmp2 = GeneralRegisterArray[i];
                    GeneralRegistersArrayValueLabel[i].Text = Convert.ToString(tmp2,2);
                }
            }



        }

        private void Machine_Load(object sender, EventArgs e)
        {

        }

        private void SourceCodeAndMachineCodeTable_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lMCModeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void addressingToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void UserInputBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void fastColoredTextBox1_Load(object sender, EventArgs e)
        {

        }
    }
}