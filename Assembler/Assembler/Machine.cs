using FastColoredTextBoxNS;
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
using System.Security.AccessControl;
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
            {"LDRM", 0b_1001101 },
            {"OUT", 0b_1001110 }

        };
        string[] RegistersLabel = { "PC: ", "CIR: ", "MAR: ", "MDR: ", "ACC: ", "INPUT: ", "OUTPUT: " };

        private string[] AQAInstructionSet = { "LDR", "STR", "ADD", "SUB", "MOV", "CMP", "B", "BEQ", "BGT", "BNE", "BLT", "AND", "ORR", "EOR", "MVN", "LSL", "LSR", "HALT", "INP", "LDRM", "OUT" };

        private int[] AQABinaryPatterns = new int[] { 0b_0100000, 0b_0100010, 0b_0001000, 0b_0001010, 0b_0100100, 0b_1001000, 0b_0010000, 0b_0010010, 0b_0010110, 0b_0010100, 0b_0010111, 0b_1000000, 0b_1000010, 0b_1000100, 0b_1000110, 0b_0001100, 0b_0001110, 0b_1001010, 0b_1001100, 0b_1001101, 0b_1001110 };
        int[] RegisterArray = new int[8];
        int wait = 1000;
        int ProgramCounter;
        int MemoryAddressRegister;
        int MemoryDataRegister;
        int MemoryBufferRegister;
        int CurrentInstructionRegister;
        int Accumulator;
        int[] GeneralRegisterArray = new int[16];
        List<string> LabelsNames = new List<string>();
        List<string> ErrorHandling = new List<string>();
        int[] MemoryArray = new int[129];
        private string SourceCode;
        string[] Tokens;
        bool ProgramIsRunning = true;
        bool IsDenaryMode = false;
        bool FileHasBeenSaved = false;
        string SavedFilename;
        public Machine()
        {
            InitializeComponent();
            DrawMainMemory();
            DrawRegisters();
            UserInputBox.Select();
            LabelOfSpeed.Visible = false;
            SpeedOfExecution_txt.Visible = false;
            ConfirmSpeed.Visible = false;
            textBox1.Visible = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private string[] TokeniseSourceCode()
        {
            string[] Tokens = GetSourceCode().Split(); //seperates source code into words
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

            ///These validate the source code.
            ///It ensures that each instruction is properly formatted
            for (int i = 0; i < SourceCodeArray.Length; i++)
            {
                if (SourceCodeArray[i] == "LDRM") ///LOAD RAM
                {
                    ///checks whether the string contains a # or @ and then any other digit.
                    if (!SourceCodeArray[i + 1].Contains('R') || !Regex.IsMatch(SourceCodeArray[i + 2], @"^[#,@]\d+$") && !Regex.IsMatch(SourceCodeArray[i + 2], @"^\d+$"))
                    {
                        IsValid = false;
                        LineOfErrorsAndNumberOfErrors.Add(LineCount);
                        TotalErrors++;
                    }
                    LineCount++;
                }
                if (SourceCodeArray[i] == "INP")
                {
                    if (!SourceCodeArray[i + 1].Contains('R') || !Regex.IsMatch(SourceCodeArray[i + 2], @"^[#,@]\d+$") && !Regex.IsMatch(SourceCodeArray[i + 2], @"^\d+$"))
                    {
                        IsValid = false;
                        LineOfErrorsAndNumberOfErrors.Add(LineCount);
                        TotalErrors++;
                    }
                    LineCount++;
                }
                if (SourceCodeArray[i] == "LDR")
                {
                    if (!SourceCodeArray[i + 1].Contains('R') || !Regex.IsMatch(SourceCodeArray[i + 2], @"^[#,@]\d+$") && !Regex.IsMatch(SourceCodeArray[i + 2], @"^\d+$"))
                    {
                        IsValid = false;
                        LineOfErrorsAndNumberOfErrors.Add(LineCount);
                        TotalErrors++;
                    }
                    LineCount++;
                }
                if (SourceCodeArray[i] == "STR")
                {
                    if (!SourceCodeArray[i + 1].Contains('R') || !Regex.IsMatch(SourceCodeArray[i + 2], @"^[#,@]\d+$") && !Regex.IsMatch(SourceCodeArray[i + 2], @"^\d+$"))
                    {
                        IsValid = false;
                        LineOfErrorsAndNumberOfErrors.Add(LineCount);
                        TotalErrors++;
                    }
                    LineCount++;
                }
                if (SourceCodeArray[i] == "ADD")
                {
                    if (!SourceCodeArray[i + 1].Contains('R') || !SourceCodeArray[i + 2].Contains('R') || !Regex.IsMatch(SourceCodeArray[i + 3], @"^[#,@]\d+$") && !Regex.IsMatch(SourceCodeArray[i + 3], @"^\d+$"))
                    {
                        IsValid = false;
                        LineOfErrorsAndNumberOfErrors.Add(LineCount);
                        TotalErrors++;
                    }
                    LineCount++;
                }
                if (SourceCodeArray[i] == "SUB")
                {
                    if (!SourceCodeArray[i + 1].Contains('R') || !SourceCodeArray[i + 2].Contains('R') || !Regex.IsMatch(SourceCodeArray[i + 3], @"^[#,@]\d+$") && !Regex.IsMatch(SourceCodeArray[i + 3], @"^\d+$"))
                    {
                        IsValid = false;
                        LineOfErrorsAndNumberOfErrors.Add(LineCount);
                        TotalErrors++;
                    }
                    LineCount++;
                }
                if (SourceCodeArray[i] == "MOV")
                {
                    if (!SourceCodeArray[i + 1].Contains('R') || !Regex.IsMatch(SourceCodeArray[i + 2], @"^[#,@]\d+$") && !Regex.IsMatch(SourceCodeArray[i + 2], @"^\d+$"))
                    {
                        IsValid = false;
                        LineOfErrorsAndNumberOfErrors.Add(LineCount);
                        TotalErrors++;
                    }
                    LineCount++;
                }
                if (SourceCodeArray[i] == "CMP")
                {
                    if (!SourceCodeArray[i + 1].Contains('R') || !Regex.IsMatch(SourceCodeArray[i + 2], @"^[#,@]\d+$") && !Regex.IsMatch(SourceCodeArray[i + 2], @"^\d+$"))
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
                    try
                    {
                        if (!BranchingLabels.ContainsKey(SourceCodeArray[i + 1]))
                        {
                            IsValid = false;
                            LineOfErrorsAndNumberOfErrors.Add(LineCount);
                            TotalErrors++;
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show("Loop not properly defined.");
                    }
                    LineCount++;
                }
                if (SourceCodeArray[i] == "AND")
                {
                    if (!SourceCodeArray[i + 1].Contains('R') || !SourceCodeArray[i + 2].Contains('R') || !Regex.IsMatch(SourceCodeArray[i + 3], @"^[#,@]\d+$") && !Regex.IsMatch(SourceCodeArray[i + 3], @"^\d+$"))
                    {
                        IsValid = false;
                        LineOfErrorsAndNumberOfErrors.Add(LineCount);
                        TotalErrors++;
                    }
                    LineCount++;
                }
                if (SourceCodeArray[i] == "OUT")
                {
                    if (!SourceCodeArray[i + 1].Contains('R') && !SourceCodeArray[i + 1].Contains('#') && !SourceCodeArray[i + 1].Contains('?'))
                    {
                        IsValid = false;
                        LineOfErrorsAndNumberOfErrors.Add(LineCount);
                        TotalErrors++;
                    }
                    LineCount++;
                }
                if (SourceCodeArray[i] == "ORR")
                {
                    if (!SourceCodeArray[i + 1].Contains('R') || !SourceCodeArray[i + 2].Contains('R') || !Regex.IsMatch(SourceCodeArray[i + 3], @"^[#,@]\d+$") && !Regex.IsMatch(SourceCodeArray[i + 3], @"^\d+$"))
                    {
                        IsValid = false;
                        LineOfErrorsAndNumberOfErrors.Add(LineCount);
                        TotalErrors++;
                    }
                    LineCount++;
                }
                if (SourceCodeArray[i] == "EOR")
                {
                    if (!SourceCodeArray[i + 1].Contains('R') || !SourceCodeArray[i + 2].Contains('R') || !Regex.IsMatch(SourceCodeArray[i + 3], @"^[#,@]\d+$") && !Regex.IsMatch(SourceCodeArray[i + 3], @"^\d+$"))
                    {
                        IsValid = false;
                        LineOfErrorsAndNumberOfErrors.Add(LineCount);
                        TotalErrors++;
                    }
                    LineCount++;
                }
                if (SourceCodeArray[i] == "MVN")
                {
                    if (!SourceCodeArray[i + 1].Contains('R') || !Regex.IsMatch(SourceCodeArray[i + 2], @"^[#,@]\d+$") && !Regex.IsMatch(SourceCodeArray[i + 2], @"^\d+$"))
                    {
                        IsValid = false;
                        LineOfErrorsAndNumberOfErrors.Add(LineCount);
                        TotalErrors++;
                    }
                    LineCount++;
                }
                if (SourceCodeArray[i] == "LSL")
                {
                    if (!SourceCodeArray[i + 1].Contains('R') || !SourceCodeArray[i + 2].Contains('R') || !Regex.IsMatch(SourceCodeArray[i + 3], @"^[#,@]\d+$") && !Regex.IsMatch(SourceCodeArray[i + 3], @"^\d+$"))
                    {
                        IsValid = false;
                        LineOfErrorsAndNumberOfErrors.Add(LineCount);
                        TotalErrors++;
                    }
                    LineCount++;
                }
                if (SourceCodeArray[i] == "LSR")
                {
                    if (!SourceCodeArray[i + 1].Contains('R') || !SourceCodeArray[i + 2].Contains('R') || !Regex.IsMatch(SourceCodeArray[i + 3], @"^[#,@]\d+$") && !Regex.IsMatch(SourceCodeArray[i + 3], @"^\d+$"))
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
                RegistersArrayValueLabel[i].Size = new Size(160, 25);
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

            ///Iterates through the code line by line 
            ///Splits the line in seperate components and then checks if it is in the Instruction set
            ///Then sets index to the place of the instruction
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
                    }

                    if (LineWords[j].Substring(0, 1) == "R")
                    {
                        tmp = LineWords[j].Substring(1);
                        tmp2 = Convert.ToInt32(tmp) + 15;
                        DenaryOfInsructions += (Convert.ToInt32(tmp2));
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

        private void saveASToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ///Used to save files into users directory 
            SaveFileDialog SaveSourceCode = new SaveFileDialog();

            SaveSourceCode.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            SaveSourceCode.FilterIndex = 2;
            SaveSourceCode.RestoreDirectory = true;

            if (SaveSourceCode.ShowDialog() == DialogResult.OK)
            {
                System.IO.StreamWriter File = new System.IO.StreamWriter(SaveSourceCode.FileName.ToString());
                SavedFilename = SaveSourceCode.FileName;
                File.WriteLine(UserInputBox.Text);
                File.Close();
                FileHasBeenSaved = true;

            }
            ///source below + learn
        }

        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ///Used to open files from directory
            ///It took some inspiration from https://stackoverflow.com/questions/31425190/c-sharp-saving-a-text-in-a-specific-directory 
            ///In addition, i learned how to do a few other things that helped me create this
            using (OpenFileDialog OpenFile = new OpenFileDialog())
            {
                try
                {
                    OpenFile.Filter = "All files(*.*)|*.*";
                    OpenFile.InitialDirectory = "D:";
                    OpenFile.Title = "Open";
                    if (OpenFile.ShowDialog() == DialogResult.OK)
                    {
                        StreamReader sr = new StreamReader(OpenFile.FileName, Encoding.Default);
                        string str = sr.ReadToEnd();
                        sr.Close();
                        UserInputBox.Text = str;
                    }
                }
                catch (Exception ErrorMessage)
                {
                    MessageBox.Show(ErrorMessage.Message);
                }
            }
        }

        private void checkErrorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ErrorMenuTextBox.Clear();
            ///The last number in LineOfErrorsAndNumberOfErrors will contain the number of errors in the code, if it is 0 there is no errors.
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

            if (LineOfErrorsAndNumberOfErrors[LineOfErrorsAndNumberOfErrors.Count - 1] == 0 && ErrorHandling.Count == 0) //check the number of error and if its 0 then it does the following
            {
                ErrorMenuTextBox.Text += "No other errors found";
                ErrorMenuTextBox.Text += "\n";
            }

            else
            {
                for (int i = 0; i < LineOfErrorsAndNumberOfErrors[LineOfErrorsAndNumberOfErrors.Count - 1]; i++)
                {
                    ErrorMenuTextBox.Text += "\n" + "Error on line : " + LineOfErrorsAndNumberOfErrors[i]; ///writes the errors and their line number
                }
            }
        }

        private void checkErrorsToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            DisplayErrors();
        }

        private void AssembleInstructions()
        {
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
            for (int i = 0; i < VerifyLabels.Count; i++) // LOOP LOOP
            {
                if (!DuplicatesVerifyLabels.Contains(VerifyLabels[i])) //making a duplicate list to check
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
                if (AQAInstructionSet.Contains(UserCode[i]) || BranchingLabels.ContainsKey(UserCode[i]))
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
            ProgramCounter = 0;
            ProgramIsRunning = true;
            List<int> LineOfErrorsAndNumberOfErrors = ValidSyntax();
            if (LineOfErrorsAndNumberOfErrors[LineOfErrorsAndNumberOfErrors.Count - 1] != 0)
            {
                System.Windows.Forms.MessageBox.Show("Input is invalid");
            }
            else
            {
                Interpret();
            }
        }

        private void fAQToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProgramCounter = 0;
            if (UserInputBox.Text == "")
            {
                MessageBox.Show("Error: No code to assemble");
                return;
            }
            if (FileHasBeenSaved)
            {
                System.IO.StreamWriter File = new System.IO.StreamWriter(SavedFilename.ToString());
                File.WriteLine(UserInputBox.Text);
                File.Close();
                FileHasBeenSaved = true;
            }
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
            ///checks the program is running and then which mode so that any value changes can be in the correct mode

            while (ProgramIsRunning)
            {
                if (!IsDenaryMode)
                {
                    RegistersArrayValueLabel[0].Text = Convert.ToString(ProgramCounter);
                    Console.WriteLine(ProgramCounter);
                    MemoryAddressRegister = MemoryArray[ProgramCounter];
                    RegistersArrayValueLabel[2].Text = Convert.ToString(MemoryAddressRegister, 2);
                    await Task.Delay(1000);
                    MemoryDataRegister = MemoryAddressRegister;
                    RegistersArrayValueLabel[3].Text = Convert.ToString(MemoryDataRegister, 2);
                    CurrentInstructionRegister = MemoryDataRegister;
                    RegistersArrayValueLabel[1].Text = Convert.ToString(CurrentInstructionRegister, 2);

                }
                else
                {
                    RegistersArrayValueLabel[0].Text = Convert.ToString(ProgramCounter);
                    Console.WriteLine(ProgramCounter);
                    MemoryAddressRegister = MemoryArray[ProgramCounter];
                    RegistersArrayValueLabel[2].Text = Convert.ToString(MemoryAddressRegister);
                    await Task.Delay(1000);
                    MemoryDataRegister = MemoryAddressRegister;
                    RegistersArrayValueLabel[3].Text = Convert.ToString(MemoryDataRegister);
                    CurrentInstructionRegister = MemoryDataRegister;
                    RegistersArrayValueLabel[1].Text = Convert.ToString(CurrentInstructionRegister);
                }

                //makes sure that the memory is not empty
                if (MemoryAddressRegister != 0)
                {
                    MachineCode = Convert.ToString(MemoryArray[ProgramCounter], 2);
                    TMPMachineCode = Convert.ToString(MemoryArray[ProgramCounter + 1], 2);
                    if (MachineCode.Length == 17)
                    {
                        MachineCode = "0" + MachineCode;
                    }

                    if (MachineCode.Length == 13)
                    {
                        Instruction = MachineCode.Substring(0, 7);
                        AddressingMode = MachineCode.Substring(7, 2);
                        Register1 = MachineCode.Substring(9, 4);
                        string TheInstruction = InstructionAndBitPatterns.FirstOrDefault(x => x.Value == Convert.ToInt32(Instruction, 2)).Key;

                        if (TheInstruction == "HALT")
                        {
                            break;
                        }
                        if (TheInstruction == "OUT")
                        {
                            if (AddressingMode == "01")
                            {

                                if (IsDenaryMode)
                                {
                                    RegistersArrayValueLabel[6].Text = Convert.ToString(GeneralRegisterArray[Convert.ToInt32(MachineCode.Substring(9, 5), 2) - 15]);

                                }
                                else
                                {
                                    RegistersArrayValueLabel[6].Text = Convert.ToString(GeneralRegisterArray[Convert.ToInt32(MachineCode.Substring(9, 5), 2) - 15], 2);


                                }
                            }

                            if (AddressingMode == "10")
                            {
                                if (IsDenaryMode)
                                {
                                    RegistersArrayValueLabel[6].Text = Convert.ToString(Convert.ToInt32(Register1, 2));

                                }
                                else
                                {
                                    RegistersArrayValueLabel[6].Text = Convert.ToString(Convert.ToInt32(Register1, 2), 2);


                                }
                            }
                            if (AddressingMode == "11")
                            {

                                if (IsDenaryMode)
                                {
                                    RegistersArrayValueLabel[6].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Register1, 2)]);

                                }
                                else
                                {
                                    RegistersArrayValueLabel[6].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Register1, 2)], 2);
                                }
                            }
                        }
                    }
                    if (MachineCode.Length == 14)
                    {

                        Instruction = MachineCode.Substring(0, 7);
                        AddressingMode = MachineCode.Substring(7, 2);
                        Register1 = MachineCode.Substring(9, 5);
                        string TheInstruction = InstructionAndBitPatterns.FirstOrDefault(x => x.Value == Convert.ToInt32(Instruction, 2)).Key;

                        if (TheInstruction == "OUT")
                        {
                            if (AddressingMode == "01")
                            {

                                if (IsDenaryMode)
                                {
                                    RegistersArrayValueLabel[6].Text = Convert.ToString(GeneralRegisterArray[Convert.ToInt32(MachineCode.Substring(9, 5), 2) - 15]);

                                }
                                else
                                {
                                    RegistersArrayValueLabel[6].Text = Convert.ToString(GeneralRegisterArray[Convert.ToInt32(MachineCode.Substring(9, 5), 2) - 15], 2);


                                }
                            }

                            if (AddressingMode == "10")
                            {
                                if (IsDenaryMode)
                                {
                                    RegistersArrayValueLabel[6].Text = Convert.ToString(Convert.ToInt32(Register1, 2));

                                }
                                else
                                {
                                    RegistersArrayValueLabel[6].Text = Convert.ToString(Convert.ToInt32(Register1, 2), 2);


                                }
                            }
                            if (AddressingMode == "11")
                            {

                                if (IsDenaryMode)
                                {
                                    RegistersArrayValueLabel[6].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Register1, 2)]);

                                }
                                else
                                {
                                    RegistersArrayValueLabel[6].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Register1, 2)], 2);
                                }
                            }
                        }
                    }

                    if (MachineCode.Length == 18)
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
                                    GeneralRegisterArray[RegisterAddress] = GeneralRegisterArray[Convert.ToInt32(Operand1, 2)];

                                    if (IsDenaryMode)
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(GeneralRegisterArray[Convert.ToInt32(Operand1)]);
                                        RegistersArrayValueLabel[5].Text = Convert.ToString(GeneralRegisterArray[Convert.ToInt32(Operand1)]);

                                    }
                                    else
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(GeneralRegisterArray[Convert.ToInt32(Operand1, 2)], 2);
                                        RegistersArrayValueLabel[5].Text = Convert.ToString(GeneralRegisterArray[Convert.ToInt32(Operand1)], 2);

                                    }
                                    GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.LightPink;
                                    await Task.Delay(250);
                                    GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.White;
                                    await Task.Delay(wait);
                                }

                                if (AddressingMode == "10")
                                {
                                    int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                    GeneralRegisterArray[RegisterAddress] = Convert.ToInt32(Operand1, 2);
                                    GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(Convert.ToInt32(Operand1, 2), 2);
                                    if (IsDenaryMode)
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(Convert.ToInt32(Operand1, 2));
                                        RegistersArrayValueLabel[5].Text = Convert.ToString(Convert.ToInt32(Operand1, 2));
                                    }
                                    else
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(Convert.ToInt32(Operand1, 2), 2);
                                        RegistersArrayValueLabel[5].Text = Convert.ToString(Convert.ToInt32(Operand1, 2), 2);
                                    }
                                    GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.LightPink;
                                    await Task.Delay(250);
                                    GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.White;
                                    await Task.Delay(wait);
                                }

                                if (AddressingMode == "11")
                                {
                                    int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                    GeneralRegisterArray[RegisterAddress] = MemoryArray[Convert.ToInt32(Operand1, 2)];
                                    GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1, 2)], 2);
                                    if (IsDenaryMode)
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1, 2)]);
                                        RegistersArrayValueLabel[5].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1, 2)]);

                                    }
                                    else
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1, 2)], 2);
                                        RegistersArrayValueLabel[5].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1, 2)], 2);

                                    }
                                    GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.LightPink;
                                    await Task.Delay(250);
                                    GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.White;
                                    await Task.Delay(wait);
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

                                    if (IsDenaryMode)
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1, 2)]);

                                    }
                                    else
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1, 2)], 2);
                                    }
                                    GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.LightPink;
                                    await Task.Delay(250);
                                    GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.White;
                                    await Task.Delay(wait);
                                }

                                if (AddressingMode == "10")
                                {
                                    int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                    GeneralRegisterArray[RegisterAddress] = Convert.ToInt32(Operand1, 2);
                                    GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1, 2)], 2);

                                    if (IsDenaryMode)
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1, 2)]);

                                    }
                                    else
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1, 2)], 2);
                                    }
                                    GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.LightPink;
                                    await Task.Delay(250);
                                    GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.White;
                                    await Task.Delay(wait);
                                }

                                if (AddressingMode == "11")
                                {
                                    int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                    GeneralRegisterArray[RegisterAddress] = MemoryArray[Convert.ToInt32(Operand1, 2)];
                                    GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1, 2)], 2);

                                    if (IsDenaryMode)
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1, 2)]);

                                    }
                                    else
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1, 2)], 2);

                                    }
                                    GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.LightPink;
                                    await Task.Delay(250);
                                    GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.White;
                                    await Task.Delay(wait);
                                }
                            }
                            if (TheInstruction == "LDRM")
                            {
                                if (AddressingMode == "01")
                                {
                                    int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                    MemoryArray[RegisterAddress] = GeneralRegisterArray[Convert.ToInt32(Register1, 2) - 15];
                                    MainMemoryArrayValue[RegisterAddress].Text = Convert.ToString(GeneralRegisterArray[Convert.ToInt32(Register1, 2) - 15], 2);

                                    if (IsDenaryMode)
                                    {
                                        MainMemoryArrayValue[RegisterAddress].Text = Convert.ToString(GeneralRegisterArray[Convert.ToInt32(Register1, 2) - 15]);

                                    }
                                    else
                                    {
                                        MainMemoryArrayValue[RegisterAddress].Text = Convert.ToString(GeneralRegisterArray[Convert.ToInt32(Register1, 2) - 15], 2);

                                    }
                                    MainMemoryArrayValue[RegisterAddress].BackColor = Color.LightPink;
                                    await Task.Delay(250);
                                    MainMemoryArrayValue[RegisterAddress].BackColor = Color.White;
                                    await Task.Delay(wait);
                                }

                                if (AddressingMode == "10")
                                {
                                    int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                    MemoryArray[RegisterAddress] = Convert.ToInt32(Operand1, 2);
                                    MainMemoryArrayValue[RegisterAddress].Text = Convert.ToString(Convert.ToInt32(Operand1, 2), 2);

                                    if (IsDenaryMode)
                                    {
                                        MainMemoryArrayValue[RegisterAddress].Text = Convert.ToString(Convert.ToInt32(Operand1, 2));

                                    }
                                    else
                                    {
                                        MainMemoryArrayValue[RegisterAddress].Text = Convert.ToString(Convert.ToInt32(Operand1, 2), 2);

                                    }
                                    MainMemoryArrayValue[RegisterAddress].BackColor = Color.LightPink;
                                    await Task.Delay(250);
                                    MainMemoryArrayValue[RegisterAddress].BackColor = Color.White;
                                    await Task.Delay(wait);
                                }

                                if (AddressingMode == "11")
                                {
                                    int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                    MemoryArray[RegisterAddress] = MemoryArray[Convert.ToInt32(Operand1, 2)];
                                    MainMemoryArrayValue[RegisterAddress].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1, 2)], 2);

                                    if (IsDenaryMode)
                                    {
                                        MainMemoryArrayValue[RegisterAddress].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1, 2)]);

                                    }
                                    else
                                    {
                                        MainMemoryArrayValue[RegisterAddress].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1, 2)], 2);

                                    }

                                    MainMemoryArrayValue[RegisterAddress].BackColor = Color.LightPink;
                                    await Task.Delay(250);
                                    MainMemoryArrayValue[RegisterAddress].BackColor = Color.White;
                                    await Task.Delay(wait);
                                }
                            }

                            if (TheInstruction == "STR")
                            {
                                if (AddressingMode == "01")
                                {
                                    int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                    MemoryArray[Convert.ToInt32(Operand1, 2)] = GeneralRegisterArray[RegisterAddress];

                                    MainMemoryArrayValue[Convert.ToInt32(Operand1, 2)].Text = Convert.ToString(GeneralRegisterArray[RegisterAddress], 2);

                                    if (IsDenaryMode)
                                    {
                                        MainMemoryArrayValue[Convert.ToInt32(Operand1, 2)].Text = Convert.ToString(GeneralRegisterArray[RegisterAddress]);
                                    }
                                    else
                                    {
                                        MainMemoryArrayValue[Convert.ToInt32(Operand1, 2)].Text = Convert.ToString(GeneralRegisterArray[RegisterAddress], 2);
                                    }

                                    MainMemoryArrayValue[Convert.ToInt32(Operand1, 2)].BackColor = Color.LightPink;
                                    await Task.Delay(250);
                                    MainMemoryArrayValue[Convert.ToInt32(Operand1, 2)].BackColor = Color.White;
                                    await Task.Delay(wait);
                                }

                                if (AddressingMode == "10")
                                {
                                    int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                    MemoryArray[RegisterAddress] = Convert.ToInt32(Operand1, 2);
                                    MainMemoryArrayValue[RegisterAddress].Text = Convert.ToString(Convert.ToInt32(Operand1, 2));

                                    if (IsDenaryMode)
                                    {
                                        MainMemoryArrayValue[RegisterAddress].Text = Convert.ToString(Convert.ToInt32(Operand1));
                                    }
                                    else
                                    {
                                        MainMemoryArrayValue[RegisterAddress].Text = Convert.ToString(Convert.ToInt32(Operand1, 2));
                                    }

                                    MainMemoryArrayValue[RegisterAddress].BackColor = Color.LightPink;
                                    await Task.Delay(250);
                                    MainMemoryArrayValue[RegisterAddress].BackColor = Color.White;
                                    await Task.Delay(wait);
                                }

                                if (AddressingMode == "11")
                                {
                                    int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                    MemoryArray[RegisterAddress] = MemoryArray[Convert.ToInt32(Operand1, 2)];
                                    MainMemoryArrayValue[RegisterAddress].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1, 2)]);

                                    if (IsDenaryMode)
                                    {
                                        MainMemoryArrayValue[RegisterAddress].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1)]);
                                    }
                                    else
                                    {
                                        MainMemoryArrayValue[RegisterAddress].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1, 2)]);
                                    }

                                    MainMemoryArrayValue[RegisterAddress].BackColor = Color.LightPink;
                                    await Task.Delay(250);
                                    MainMemoryArrayValue[RegisterAddress].BackColor = Color.White;
                                    await Task.Delay(wait);
                                }
                            }
                            //// 01 - direct //10 - immedidate // 11 - relative

                            if (TheInstruction == "MOV")
                            {
                                if (AddressingMode == "01")
                                {
                                    int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                    GeneralRegisterArray[RegisterAddress] = GeneralRegisterArray[Convert.ToInt32(Operand1, 2)];
                                    GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(GeneralRegisterArray[Convert.ToInt32(Operand1, 2)], 2);

                                    if (IsDenaryMode)
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(GeneralRegisterArray[Convert.ToInt32(Operand1, 2)]);
                                    }
                                    else
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(GeneralRegisterArray[Convert.ToInt32(Operand1, 2)], 2);
                                    }

                                    GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.LightPink;
                                    await Task.Delay(250);
                                    GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.White;
                                    await Task.Delay(wait);
                                }

                                if (AddressingMode == "10")
                                {
                                    int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                    GeneralRegisterArray[RegisterAddress] = Convert.ToInt32(Operand1, 2);
                                    GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(Convert.ToInt32(Operand1, 2), 2);

                                    if (IsDenaryMode)
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(Convert.ToInt32(Operand1, 2));
                                    }
                                    else
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(Convert.ToInt32(Operand1, 2), 2);
                                    }

                                    GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.LightPink;
                                    await Task.Delay(250);
                                    GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.White;
                                    await Task.Delay(wait);
                                }

                                if (AddressingMode == "11")
                                {
                                    int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                    GeneralRegisterArray[RegisterAddress] = MemoryArray[Convert.ToInt32(Operand1)];
                                    GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1)], 2);

                                    if (IsDenaryMode)
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1)]);
                                    }
                                    else
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1)], 2);
                                    }

                                    GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.LightPink;
                                    await Task.Delay(250);
                                    GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.White;
                                    await Task.Delay(wait);
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
                                        if (RegisterAddress == CompareWith)
                                        {
                                            ProgramCounter = Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2) - 1;
                                            await Task.Delay(wait);

                                        }

                                    }

                                    if (InstructionAndBitPatterns.FirstOrDefault(x => x.Value == Convert.ToInt32(BranchingInstruction.Substring(0, 7), 2)).Key == "BNE")
                                    {
                                        if (GeneralRegisterArray[RegisterAddress] != CompareWith)
                                        {
                                            ProgramCounter = Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2) - 1;
                                            await Task.Delay(wait);

                                        }
                                    }

                                    if (InstructionAndBitPatterns.FirstOrDefault(x => x.Value == Convert.ToInt32(BranchingInstruction.Substring(0, 7), 2)).Key == "BGT")
                                    {
                                        if (GeneralRegisterArray[RegisterAddress] > CompareWith)
                                        {
                                            ProgramCounter = Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2) - 1;

                                            await Task.Delay(wait);

                                        }
                                    }

                                    if (InstructionAndBitPatterns.FirstOrDefault(x => x.Value == Convert.ToInt32(BranchingInstruction.Substring(0, 7), 2)).Key == "BLT")
                                    {
                                        if (GeneralRegisterArray[RegisterAddress] < CompareWith)
                                        {
                                            ProgramCounter = Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2) - 1;

                                            await Task.Delay(wait);

                                        }
                                    }
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
                                            ProgramCounter = Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2) - 2;

                                            await Task.Delay(wait);

                                        }

                                    }

                                    if (InstructionAndBitPatterns.FirstOrDefault(x => x.Value == Convert.ToInt32(BranchingInstruction.Substring(0, 7), 2)).Key == "BNE")
                                    {
                                        if (RegisterAddress != CompareWith)
                                        {
                                            ProgramCounter = Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2) - 2;

                                            await Task.Delay(wait);

                                        }
                                    }

                                    if (InstructionAndBitPatterns.FirstOrDefault(x => x.Value == Convert.ToInt32(BranchingInstruction.Substring(0, 7), 2)).Key == "BGT")
                                    {
                                        if (RegisterAddress > CompareWith)
                                        {
                                            ProgramCounter = Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2);

                                            await Task.Delay(wait);

                                        }
                                    }

                                    if (InstructionAndBitPatterns.FirstOrDefault(x => x.Value == Convert.ToInt32(BranchingInstruction.Substring(0, 7), 2)).Key == "BLT")
                                    {
                                        if (RegisterAddress < CompareWith)
                                        {
                                            ProgramCounter = Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2) - 2;

                                            await Task.Delay(wait);

                                        }
                                    }
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
                                            ProgramCounter = Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2) - 2;

                                            await Task.Delay(wait);

                                        }

                                    }

                                    if (InstructionAndBitPatterns.FirstOrDefault(x => x.Value == Convert.ToInt32(BranchingInstruction.Substring(0, 7), 2)).Key == "BNE")
                                    {
                                        if (RegisterAddress != CompareWith)
                                        {
                                            ProgramCounter = Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2) - 2;

                                            await Task.Delay(wait);

                                        }
                                    }

                                    if (InstructionAndBitPatterns.FirstOrDefault(x => x.Value == Convert.ToInt32(BranchingInstruction.Substring(0, 7), 2)).Key == "BGT")
                                    {
                                        if (RegisterAddress > CompareWith)
                                        {
                                            ProgramCounter = Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2) - 2;

                                            await Task.Delay(wait);

                                        }
                                    }

                                    if (InstructionAndBitPatterns.FirstOrDefault(x => x.Value == Convert.ToInt32(BranchingInstruction.Substring(0, 7), 2)).Key == "BLT")
                                    {
                                        if (RegisterAddress < CompareWith)
                                        {
                                            ProgramCounter = Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2) - 2;

                                            await Task.Delay(wait);

                                        }
                                    }

                                }

                            }

                            //// 01 - direct //10 - immedidate // 11 - relative

                            if (TheInstruction == "MVN")
                            {
                                if (AddressingMode == "01")
                                {
                                    int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                    int result = ~GeneralRegisterArray[Convert.ToInt32(Operand1, 2)];
                                    GeneralRegisterArray[RegisterAddress] = result;
                                    GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(result, 2);

                                    if (IsDenaryMode)
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(result);
                                    }
                                    else
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(result, 2);
                                    }

                                    GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.LightPink;
                                    await Task.Delay(250);
                                    GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.White;
                                    await Task.Delay(wait);
                                }

                                if (AddressingMode == "10")
                                {
                                    int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                    int result = ~Convert.ToInt32(Operand1, 2);
                                    GeneralRegisterArray[RegisterAddress] = result;
                                    GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(result, 2);

                                    if (IsDenaryMode)
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(result);
                                    }
                                    else
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(result, 2);
                                    }

                                    GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.LightPink;
                                    await Task.Delay(250);
                                    GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.White;
                                    await Task.Delay(wait);
                                }

                                if (AddressingMode == "11")
                                {
                                    int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                    int result = ~MemoryArray[Convert.ToInt32(Operand1, 2)];
                                    GeneralRegisterArray[RegisterAddress] = result;
                                    GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(result, 2);

                                    if (IsDenaryMode)
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(result);
                                    }
                                    else
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(result, 2);
                                    }

                                    GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.LightPink;
                                    await Task.Delay(250);
                                    GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.White;
                                    await Task.Delay(wait);

                                }
                            }

                        }

                    }

                    if (MachineCode.Length == 9)
                    {
                        MachineCode = "00" + MachineCode;

                        if (InstructionAndBitPatterns.FirstOrDefault(x => x.Value == Convert.ToInt32(MachineCode.Substring(0, 7), 2)).Key == "B")
                        {
                            ProgramCounter = Convert.ToInt32(MachineCode.Substring(7, 4), 2) - 2;

                            await Task.Delay(wait);
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

                                    if (IsDenaryMode)
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(sum);
                                        RegistersArrayValueLabel[4].Text = Convert.ToString(sum);

                                    }
                                    else
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(sum, 2);
                                        RegistersArrayValueLabel[4].Text = Convert.ToString(sum, 2);

                                    }


                                    GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.LightPink;
                                    await Task.Delay(250);
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.White;
                                    await Task.Delay(wait);
                                }

                                if (AddressingMode == "10")
                                {

                                    sum = GeneralRegisterArray[RegisterAddress2] + Convert.ToInt32(Operand1, 2);
                                    GeneralRegisterArray[RegisterAddress1] = sum;
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(Convert.ToInt32(GeneralRegisterArray[RegisterAddress1]), 2);

                                    if (IsDenaryMode)
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(Convert.ToInt32(GeneralRegisterArray[RegisterAddress1]));
                                        RegistersArrayValueLabel[4].Text = Convert.ToString(Convert.ToInt32(GeneralRegisterArray[RegisterAddress1]));

                                    }
                                    else
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(Convert.ToInt32(GeneralRegisterArray[RegisterAddress1]), 2);
                                        RegistersArrayValueLabel[4].Text = Convert.ToString(Convert.ToInt32(GeneralRegisterArray[RegisterAddress1]));
                                    }

                                    GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.LightPink;
                                    await Task.Delay(250);
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.White;
                                    await Task.Delay(wait);

                                }
                                if (AddressingMode == "11")
                                {
                                    sum = GeneralRegisterArray[RegisterAddress2] + MemoryArray[Convert.ToInt32(Operand1, 2)];
                                    GeneralRegisterArray[RegisterAddress1] = sum;
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(sum, 2);

                                    if (IsDenaryMode)
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(sum);
                                        RegistersArrayValueLabel[4].Text = Convert.ToString(Convert.ToInt32(GeneralRegisterArray[RegisterAddress1]));
                                    }
                                    else
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(sum, 2);
                                        RegistersArrayValueLabel[4].Text = Convert.ToString(Convert.ToInt32(GeneralRegisterArray[RegisterAddress1]), 2);
                                    }

                                    GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.LightPink;
                                    await Task.Delay(250);
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.White;
                                    await Task.Delay(wait);
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
                                    result = GeneralRegisterArray[RegisterAddress2] << GeneralRegisterArray[Convert.ToInt32(Operand1, 2)];
                                    GeneralRegisterArray[RegisterAddress1] = result;
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(result, 2);

                                    if (IsDenaryMode)
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(result);
                                    }
                                    else
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(result, 2);
                                    }

                                    GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.LightPink;
                                    await Task.Delay(250);
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.White;
                                    await Task.Delay(wait);
                                }

                                if (AddressingMode == "10")
                                {
                                    result = GeneralRegisterArray[RegisterAddress2] << Convert.ToInt32(Operand1, 2);
                                    GeneralRegisterArray[RegisterAddress1] = result;
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(result, 2);

                                    if (IsDenaryMode)
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(result);
                                    }
                                    else
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(result, 2);
                                    }

                                    GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.LightPink;
                                    await Task.Delay(250);
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.White;
                                    await Task.Delay(wait);
                                }

                                if (AddressingMode == "11")
                                {
                                    result = GeneralRegisterArray[RegisterAddress2] << MemoryArray[Convert.ToInt32(Operand1, 2)];
                                    GeneralRegisterArray[RegisterAddress1] = result;
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(result, 2);

                                    if (IsDenaryMode)
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(result);
                                    }
                                    else
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(result, 2);
                                    }

                                    GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.LightPink;
                                    await Task.Delay(250);
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.White;
                                    await Task.Delay(wait);
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
                                    result = GeneralRegisterArray[RegisterAddress2] >> GeneralRegisterArray[Convert.ToInt32(Operand1, 2)];
                                    GeneralRegisterArray[RegisterAddress1] = result;
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(result, 2);

                                    if (IsDenaryMode)
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(result);
                                    }
                                    else
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(result, 2);
                                    }

                                    GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.LightPink;
                                    await Task.Delay(250);
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.White;

                                    await Task.Delay(wait);
                                }

                                if (AddressingMode == "10")
                                {
                                    result = GeneralRegisterArray[RegisterAddress2] >> Convert.ToInt32(Operand1, 2);
                                    GeneralRegisterArray[RegisterAddress1] = result;
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(result, 2);

                                    if (IsDenaryMode)
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(result);
                                    }
                                    else
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(result, 2);
                                    }

                                    GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.LightPink;
                                    await Task.Delay(250);
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.White;
                                    await Task.Delay(wait);
                                }

                                if (AddressingMode == "11")
                                {
                                    result = GeneralRegisterArray[RegisterAddress2] >> MemoryArray[Convert.ToInt32(Operand1, 2)];
                                    GeneralRegisterArray[RegisterAddress1] = result;
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(result, 2);

                                    if (IsDenaryMode)
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(result);
                                    }
                                    else
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(result, 2);
                                    }

                                    GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.LightPink;
                                    await Task.Delay(250);
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.White;
                                    await Task.Delay(wait);
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
                                    result = GeneralRegisterArray[Convert.ToInt32(Register2, 2) - 15] & GeneralRegisterArray[Convert.ToInt32(Operand1, 2)];
                                    GeneralRegisterArray[Convert.ToInt32(Register1, 2) - 15] = result;
                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].Text = Convert.ToString(result, 2);

                                    if (IsDenaryMode)
                                    {
                                        GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].Text = Convert.ToString(result);
                                    }
                                    else
                                    {
                                        GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].Text = Convert.ToString(result, 2);
                                    }

                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].BackColor = Color.LightPink;
                                    await Task.Delay(250);
                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].BackColor = Color.White;
                                    await Task.Delay(wait);
                                }

                                if (AddressingMode == "10")
                                {
                                    result = GeneralRegisterArray[Convert.ToInt32(Register2, 2)] & Convert.ToInt32(Operand1, 2);
                                    GeneralRegisterArray[Convert.ToInt32(Register1, 2)] = result;
                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2)].Text = Convert.ToString(result, 2);

                                    if (IsDenaryMode)
                                    {
                                        GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2)].Text = Convert.ToString(result);
                                    }
                                    else
                                    {
                                        GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2)].Text = Convert.ToString(result, 2);
                                    }

                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2)].BackColor = Color.LightPink;
                                    await Task.Delay(250);
                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2)].BackColor = Color.White;
                                    await Task.Delay(wait);
                                }

                                if (AddressingMode == "11")
                                {
                                    result = GeneralRegisterArray[Convert.ToInt32(Register2, 2)] & MemoryArray[Convert.ToInt32(Operand1, 2)];
                                    GeneralRegisterArray[Convert.ToInt32(Register1, 2)] = result;
                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2)].Text = Convert.ToString(result, 2);

                                    if (IsDenaryMode)
                                    {
                                        GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2)].Text = Convert.ToString(result);
                                    }
                                    else
                                    {
                                        GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2)].Text = Convert.ToString(result, 2);
                                    }

                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2)].BackColor = Color.LightPink;
                                    await Task.Delay(250);
                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2)].BackColor = Color.White;
                                    await Task.Delay(wait);

                                }
                            }

                            if (TheInstruction == "ORR")
                            {
                                int result;
                                Instruction = MachineCode.Substring(0, 7);
                                AddressingMode = MachineCode.Substring(7, 2);
                                Register1 = MachineCode.Substring(9, 5);
                                Register2 = MachineCode.Substring(14, 5);
                                Operand1 = MachineCode.Substring(19, 4);

                                Console.WriteLine(Convert.ToInt32(Register2, 2));

                                if (AddressingMode == "01")
                                {
                                    Console.WriteLine(Convert.ToInt32(Register2, 2));
                                    result = GeneralRegisterArray[Convert.ToInt32(Register2, 2)-15] | GeneralRegisterArray[Convert.ToInt32(Operand1, 2)];
                                    GeneralRegisterArray[Convert.ToInt32(Register1, 2) - 15] = result;
                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].Text = Convert.ToString(result, 2);

                                    if (IsDenaryMode)
                                    {
                                        GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].Text = Convert.ToString(result);
                                    }
                                    else
                                    {
                                        GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].Text = Convert.ToString(result, 2);
                                    }

                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].BackColor = Color.LightPink;
                                    await Task.Delay(250);
                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].BackColor = Color.White;
                                    await Task.Delay(wait);
                                }

                                if (AddressingMode == "10")
                                {
                                    result = GeneralRegisterArray[Convert.ToInt32(Register2, 2) - 15] | Convert.ToInt32(Operand1, 2);
                                    GeneralRegisterArray[Convert.ToInt32(Register1, 2) - 15] = result;
                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].Text = Convert.ToString(result);

                                    if (IsDenaryMode)
                                    {
                                        GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].Text = Convert.ToString(result);
                                    }
                                    else
                                    {
                                        GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].Text = Convert.ToString(result, 2);
                                    }

                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].BackColor = Color.LightPink;
                                    await Task.Delay(250);
                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].BackColor = Color.White;
                                    await Task.Delay(wait);
                                }

                                if (AddressingMode == "11")
                                {
                                    result = GeneralRegisterArray[Convert.ToInt32(Register2, 2) - 15] | MemoryArray[Convert.ToInt32(Operand1, 2)];
                                    GeneralRegisterArray[Convert.ToInt32(Register1, 2) - 15] = result;
                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].Text = Convert.ToString(result, 2);

                                    if (IsDenaryMode)
                                    {
                                        GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2)].Text = Convert.ToString(result);
                                    }
                                    else
                                    {
                                        GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2)].Text = Convert.ToString(result, 2);
                                    }

                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2)].BackColor = Color.LightPink;
                                    await Task.Delay(250);
                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2)].BackColor = Color.White;
                                    await Task.Delay(wait);
                                }
                            }

                            if (TheInstruction == "EOR")
                            {
                                int result = 0;
                                Instruction = MachineCode.Substring(0, 7);
                                AddressingMode = MachineCode.Substring(7, 2);
                                Register1 = MachineCode.Substring(9, 5);
                                Register2 = MachineCode.Substring(14, 5);
                                Operand1 = MachineCode.Substring(19, 4);

                                if (AddressingMode == "01")
                                {
                                    result = GeneralRegisterArray[Convert.ToInt32(Register2, 2) - 15] ^ GeneralRegisterArray[Convert.ToInt32(Operand1, 2)];
                                    GeneralRegisterArray[Convert.ToInt32(Register1, 2) - 15] = result;
                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].Text = Convert.ToString(result, 2);

                                    if (IsDenaryMode)
                                    {
                                        GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].Text = Convert.ToString(result);
                                    }
                                    else
                                    {
                                        GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].Text = Convert.ToString(result, 2);
                                    }

                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].BackColor = Color.LightPink;
                                    await Task.Delay(250);
                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].BackColor = Color.White;
                                    await Task.Delay(wait);
                                }

                                if (AddressingMode == "10")
                                {
                                    result = GeneralRegisterArray[Convert.ToInt32(Register2, 2) - 15] ^ Convert.ToInt32(Operand1, 2);
                                    GeneralRegisterArray[Convert.ToInt32(Register1, 2) - 15] = result;
                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].Text = Convert.ToString(result);

                                    if (IsDenaryMode)
                                    {
                                        GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].Text = Convert.ToString(result);
                                    }
                                    else
                                    {
                                        GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].Text = Convert.ToString(result, 2);
                                    }

                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].BackColor = Color.LightPink;
                                    await Task.Delay(250);
                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].BackColor = Color.White;
                                    await Task.Delay(wait);
                                }

                                if (AddressingMode == "11")
                                {
                                    result = GeneralRegisterArray[Convert.ToInt32(Register2, 2) - 15] ^ MemoryArray[Convert.ToInt32(Operand1, 2)];
                                    GeneralRegisterArray[Convert.ToInt32(Register1, 2)] = result;
                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].Text = Convert.ToString(result, 2);

                                    if (IsDenaryMode)
                                    {
                                        GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].Text = Convert.ToString(result);
                                    }
                                    else
                                    {
                                        GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].Text = Convert.ToString(result, 2);
                                    }

                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2)].BackColor = Color.LightPink;
                                    await Task.Delay(250);
                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2)].BackColor = Color.White;
                                    await Task.Delay(wait);
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

                                    if (IsDenaryMode)
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(sum);
                                        RegistersArrayValueLabel[4].Text = Convert.ToString(sum);

                                    }
                                    else
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(sum, 2);
                                        RegistersArrayValueLabel[4].Text = Convert.ToString(sum, 2);
                                    }

                                    GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.LightPink;
                                    await Task.Delay(250);
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.White;
                                    await Task.Delay(wait);
                                }

                                if (AddressingMode == "10")
                                {
                                    sum = GeneralRegisterArray[RegisterAddress2] - Convert.ToInt32(Operand1, 2);
                                    GeneralRegisterArray[RegisterAddress1] = sum;
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(sum, 2);

                                    if (IsDenaryMode)
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(sum);
                                        RegistersArrayValueLabel[4].Text = Convert.ToString(sum);
                                    }
                                    else
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(sum, 2);
                                        RegistersArrayValueLabel[4].Text = Convert.ToString(sum, 2);
                                    }

                                    GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.LightPink;
                                    await Task.Delay(250);
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.White;
                                    await Task.Delay(wait);
                                }

                                if (AddressingMode == "11")
                                {
                                    sum = GeneralRegisterArray[RegisterAddress2] - MemoryArray[Convert.ToInt32(Operand1, 2)];
                                    GeneralRegisterArray[RegisterAddress1] = sum;
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(sum, 2);

                                    if (IsDenaryMode)
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(sum);
                                        RegistersArrayValueLabel[4].Text = Convert.ToString(sum);
                                    }
                                    else
                                    {
                                        GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(sum, 2);
                                        RegistersArrayValueLabel[4].Text = Convert.ToString(sum, 2);
                                    }

                                    GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.LightPink;
                                    await Task.Delay(250);
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.White;
                                    await Task.Delay(wait);
                                }
                            }

                        }
                    }
                    ProgramCounter++;
                }
                else
                {
                    break;
                }
            }
        }

        private void StepInterpret()
        {
            string MachineCode;
            string TMPMachineCode;
            string Instruction;
            string TMPInstruction;
            string AddressingMode;
            string Register1;
            string Register2;
            string Operand1;

            if (!IsDenaryMode)
            {
                RegistersArrayValueLabel[0].Text = Convert.ToString(ProgramCounter);
                MemoryAddressRegister = MemoryArray[ProgramCounter];
                RegistersArrayValueLabel[2].Text = Convert.ToString(MemoryAddressRegister, 2);
                MemoryDataRegister = MemoryAddressRegister;
                RegistersArrayValueLabel[3].Text = Convert.ToString(MemoryDataRegister, 2);
                CurrentInstructionRegister = MemoryDataRegister;
                RegistersArrayValueLabel[1].Text = Convert.ToString(CurrentInstructionRegister, 2);
            }
            else
            {
                RegistersArrayValueLabel[0].Text = Convert.ToString(ProgramCounter);
                MemoryAddressRegister = MemoryArray[ProgramCounter];
                RegistersArrayValueLabel[2].Text = Convert.ToString(MemoryAddressRegister);
                MemoryDataRegister = MemoryAddressRegister;
                RegistersArrayValueLabel[3].Text = Convert.ToString(MemoryDataRegister);
                CurrentInstructionRegister = MemoryDataRegister;
                RegistersArrayValueLabel[1].Text = Convert.ToString(CurrentInstructionRegister);
            }


            if (MemoryAddressRegister != 0)
            {
                MachineCode = Convert.ToString(MemoryArray[ProgramCounter], 2);
                TMPMachineCode = Convert.ToString(MemoryArray[ProgramCounter + 1], 2);
                if (MachineCode.Length == 17)
                {
                    MachineCode = "0" + MachineCode;
                }

                if (MachineCode.Length == 13)
                {
                    Instruction = MachineCode.Substring(0, 7);
                    string TheInstruction = InstructionAndBitPatterns.FirstOrDefault(x => x.Value == Convert.ToInt32(Instruction, 2)).Key;

                    if (TheInstruction == "HALT")
                    {
                        return;
                    }
                }
                if (MachineCode.Length == 14)
                {

                    Instruction = MachineCode.Substring(0, 7);
                    AddressingMode = MachineCode.Substring(7, 2);
                    Register1 = MachineCode.Substring(9, 5);
                    string TheInstruction = InstructionAndBitPatterns.FirstOrDefault(x => x.Value == Convert.ToInt32(Instruction, 2)).Key;

                    if (TheInstruction == "OUT")
                    {
                        if (AddressingMode == "01")
                        {

                            if (IsDenaryMode)
                            {
                                RegistersArrayValueLabel[6].Text = Convert.ToString(GeneralRegisterArray[Convert.ToInt32(MachineCode.Substring(9, 5), 2) - 15]);

                            }
                            else
                            {
                                RegistersArrayValueLabel[6].Text = Convert.ToString(GeneralRegisterArray[Convert.ToInt32(MachineCode.Substring(9, 5), 2) - 15], 2);


                            }
                        }

                        if (AddressingMode == "10")
                        {
                            if (IsDenaryMode)
                            {
                                RegistersArrayValueLabel[6].Text = Convert.ToString(Convert.ToInt32(Register1, 2));

                            }
                            else
                            {
                                RegistersArrayValueLabel[6].Text = Convert.ToString(Convert.ToInt32(Register1, 2), 2);


                            }
                        }
                        if (AddressingMode == "11")
                        {

                            if (IsDenaryMode)
                            {
                                RegistersArrayValueLabel[6].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Register1, 2)]);

                            }
                            else
                            {
                                RegistersArrayValueLabel[6].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Register1, 2)], 2);
                            }
                        }
                    }
                }
                if (MachineCode.Length == 18)
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
                                GeneralRegisterArray[RegisterAddress] = GeneralRegisterArray[Convert.ToInt32(Operand1, 2)];

                                if (IsDenaryMode)
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(GeneralRegisterArray[Convert.ToInt32(Operand1)]);
                                }
                                else
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(GeneralRegisterArray[Convert.ToInt32(Operand1, 2)], 2);
                                }
                                GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.LightPink;
                                Task.Delay(250);

                                GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.White;
                            }

                            if (AddressingMode == "10")
                            {
                                int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                GeneralRegisterArray[RegisterAddress] = Convert.ToInt32(Operand1, 2);
                                GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(Convert.ToInt32(Operand1, 2), 2);
                                if (IsDenaryMode)
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(Convert.ToInt32(Operand1, 2));

                                }
                                else
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(Convert.ToInt32(Operand1, 2), 2);

                                }
                                GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.LightPink;
                                Task.Delay(250);

                                GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.White;
                            }

                            if (AddressingMode == "11")
                            {
                                int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                GeneralRegisterArray[RegisterAddress] = MemoryArray[Convert.ToInt32(Operand1, 2)];
                                GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1, 2)], 2);
                                if (IsDenaryMode)
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1, 2)]);

                                }
                                else
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1, 2)], 2);

                                }
                                GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.LightPink;
                                Task.Delay(250);

                                GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.White;
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

                                if (IsDenaryMode)
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1, 2)]);

                                }
                                else
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1, 2)], 2);
                                }
                                GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.LightPink;
                                Task.Delay(250);

                                GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.White;
                            }

                            if (AddressingMode == "10")
                            {
                                int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                GeneralRegisterArray[RegisterAddress] = Convert.ToInt32(Operand1, 2);
                                GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1, 2)], 2);

                                if (IsDenaryMode)
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1, 2)]);

                                }
                                else
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1, 2)], 2);
                                }
                                GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.LightPink;
                                Task.Delay(250);

                                GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.White;
                            }

                            if (AddressingMode == "11")
                            {
                                int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                GeneralRegisterArray[RegisterAddress] = MemoryArray[Convert.ToInt32(Operand1, 2)];
                                GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1, 2)], 2);

                                if (IsDenaryMode)
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1, 2)]);

                                }
                                else
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1, 2)], 2);

                                }
                                GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.LightPink;
                                Task.Delay(250);

                                GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.White;
                            }
                        }
                        if (TheInstruction == "LDRM")
                        {
                            if (AddressingMode == "01")
                            {
                                int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                MemoryArray[RegisterAddress] = GeneralRegisterArray[Convert.ToInt32(Register1, 2) - 15];
                                MainMemoryArrayValue[RegisterAddress].Text = Convert.ToString(GeneralRegisterArray[Convert.ToInt32(Register1, 2) - 15], 2);

                                if (IsDenaryMode)
                                {
                                    MainMemoryArrayValue[RegisterAddress].Text = Convert.ToString(GeneralRegisterArray[Convert.ToInt32(Register1, 2) - 15]);

                                }
                                else
                                {
                                    MainMemoryArrayValue[RegisterAddress].Text = Convert.ToString(GeneralRegisterArray[Convert.ToInt32(Register1, 2) - 15], 2);

                                }
                                MainMemoryArrayValue[RegisterAddress].BackColor = Color.LightPink;
                                Task.Delay(250);

                                MainMemoryArrayValue[RegisterAddress].BackColor = Color.White;
                            }

                            if (AddressingMode == "10")
                            {
                                int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                MemoryArray[RegisterAddress] = Convert.ToInt32(Operand1, 2);
                                MainMemoryArrayValue[RegisterAddress].Text = Convert.ToString(Convert.ToInt32(Operand1, 2), 2);

                                if (IsDenaryMode)
                                {
                                    MainMemoryArrayValue[RegisterAddress].Text = Convert.ToString(Convert.ToInt32(Operand1, 2));

                                }
                                else
                                {
                                    MainMemoryArrayValue[RegisterAddress].Text = Convert.ToString(Convert.ToInt32(Operand1, 2), 2);

                                }
                                MainMemoryArrayValue[RegisterAddress].BackColor = Color.LightPink;
                                Task.Delay(250);

                                MainMemoryArrayValue[RegisterAddress].BackColor = Color.White;
                            }

                            if (AddressingMode == "11")
                            {
                                int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                MemoryArray[RegisterAddress] = MemoryArray[Convert.ToInt32(Operand1, 2)];
                                MainMemoryArrayValue[RegisterAddress].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1, 2)], 2);

                                if (IsDenaryMode)
                                {
                                    MainMemoryArrayValue[RegisterAddress].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1, 2)]);

                                }
                                else
                                {
                                    MainMemoryArrayValue[RegisterAddress].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1, 2)], 2);

                                }

                                MainMemoryArrayValue[RegisterAddress].BackColor = Color.LightPink;
                                Task.Delay(250);

                                MainMemoryArrayValue[RegisterAddress].BackColor = Color.White;
                            }
                        }

                        if (TheInstruction == "STR")
                        {
                            if (AddressingMode == "01")
                            {
                                int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                MemoryArray[Convert.ToInt32(Operand1, 2)] = GeneralRegisterArray[RegisterAddress];

                                MainMemoryArrayValue[Convert.ToInt32(Operand1, 2)].Text = Convert.ToString(GeneralRegisterArray[RegisterAddress], 2);

                                if (IsDenaryMode)
                                {
                                    MainMemoryArrayValue[Convert.ToInt32(Operand1, 2)].Text = Convert.ToString(GeneralRegisterArray[RegisterAddress]);
                                }
                                else
                                {
                                    MainMemoryArrayValue[Convert.ToInt32(Operand1, 2)].Text = Convert.ToString(GeneralRegisterArray[RegisterAddress], 2);
                                }

                                MainMemoryArrayValue[Convert.ToInt32(Operand1, 2)].BackColor = Color.LightPink;
                                Task.Delay(250);

                                MainMemoryArrayValue[Convert.ToInt32(Operand1, 2)].BackColor = Color.White;
                            }

                            if (AddressingMode == "10")
                            {
                                int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                MemoryArray[RegisterAddress] = Convert.ToInt32(Operand1, 2);
                                MainMemoryArrayValue[RegisterAddress].Text = Convert.ToString(Convert.ToInt32(Operand1, 2));

                                if (IsDenaryMode)
                                {
                                    MainMemoryArrayValue[RegisterAddress].Text = Convert.ToString(Convert.ToInt32(Operand1));
                                }
                                else
                                {
                                    MainMemoryArrayValue[RegisterAddress].Text = Convert.ToString(Convert.ToInt32(Operand1, 2));
                                }

                                MainMemoryArrayValue[RegisterAddress].BackColor = Color.LightPink;
                                Task.Delay(250);

                                MainMemoryArrayValue[RegisterAddress].BackColor = Color.White;
                            }

                            if (AddressingMode == "11")
                            {
                                int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                MemoryArray[RegisterAddress] = MemoryArray[Convert.ToInt32(Operand1, 2)];
                                MainMemoryArrayValue[RegisterAddress].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1, 2)]);

                                if (IsDenaryMode)
                                {
                                    MainMemoryArrayValue[RegisterAddress].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1)]);
                                }
                                else
                                {
                                    MainMemoryArrayValue[RegisterAddress].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1, 2)]);
                                }

                                MainMemoryArrayValue[RegisterAddress].BackColor = Color.LightPink;
                                Task.Delay(250);

                                MainMemoryArrayValue[RegisterAddress].BackColor = Color.White;
                            }
                        }
                        //// 01 - direct //10 - immedidate // 11 - relative

                        if (TheInstruction == "MOV")
                        {
                            if (AddressingMode == "01")
                            {
                                int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                GeneralRegisterArray[RegisterAddress] = GeneralRegisterArray[Convert.ToInt32(Operand1, 2)];
                                GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(GeneralRegisterArray[Convert.ToInt32(Operand1, 2)], 2);

                                if (IsDenaryMode)
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(GeneralRegisterArray[Convert.ToInt32(Operand1, 2)]);
                                }
                                else
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(GeneralRegisterArray[Convert.ToInt32(Operand1, 2)], 2);
                                }

                                GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.LightPink;
                                Task.Delay(250);

                                GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.White;
                            }

                            if (AddressingMode == "10")
                            {
                                int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                GeneralRegisterArray[RegisterAddress] = Convert.ToInt32(Operand1, 2);
                                GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(Convert.ToInt32(Operand1, 2), 2);

                                if (IsDenaryMode)
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(Convert.ToInt32(Operand1, 2));
                                }
                                else
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(Convert.ToInt32(Operand1, 2), 2);
                                }

                                GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.LightPink;
                                Task.Delay(250);

                                GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.White;
                            }

                            if (AddressingMode == "11")
                            {
                                int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                GeneralRegisterArray[RegisterAddress] = MemoryArray[Convert.ToInt32(Operand1)];
                                GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1)], 2);

                                if (IsDenaryMode)
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1)]);
                                }
                                else
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(MemoryArray[Convert.ToInt32(Operand1)], 2);
                                }

                                GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.LightPink;
                                Task.Delay(250);

                                GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.White;
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
                                    if (RegisterAddress == CompareWith)
                                    {
                                        ProgramCounter = Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2) - 1;

                                    }

                                }

                                if (InstructionAndBitPatterns.FirstOrDefault(x => x.Value == Convert.ToInt32(BranchingInstruction.Substring(0, 7), 2)).Key == "BNE")
                                {
                                    if (GeneralRegisterArray[RegisterAddress] != CompareWith)
                                    {
                                        ProgramCounter = Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2) - 1;

                                    }
                                }

                                if (InstructionAndBitPatterns.FirstOrDefault(x => x.Value == Convert.ToInt32(BranchingInstruction.Substring(0, 7), 2)).Key == "BGT")
                                {
                                    if (GeneralRegisterArray[RegisterAddress] > CompareWith)
                                    {
                                        ProgramCounter = Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2) - 1;

                                    }
                                }

                                if (InstructionAndBitPatterns.FirstOrDefault(x => x.Value == Convert.ToInt32(BranchingInstruction.Substring(0, 7), 2)).Key == "BLT")
                                {
                                    if (GeneralRegisterArray[RegisterAddress] < CompareWith)
                                    {
                                        ProgramCounter = Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2) - 1;


                                    }
                                }
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
                                        ProgramCounter = Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2) - 2;


                                    }

                                }

                                if (InstructionAndBitPatterns.FirstOrDefault(x => x.Value == Convert.ToInt32(BranchingInstruction.Substring(0, 7), 2)).Key == "BNE")
                                {
                                    if (RegisterAddress != CompareWith)
                                    {
                                        ProgramCounter = Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2) - 2;


                                    }
                                }

                                if (InstructionAndBitPatterns.FirstOrDefault(x => x.Value == Convert.ToInt32(BranchingInstruction.Substring(0, 7), 2)).Key == "BGT")
                                {
                                    if (RegisterAddress > CompareWith)
                                    {
                                        ProgramCounter = Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2) - 2;


                                    }
                                }

                                if (InstructionAndBitPatterns.FirstOrDefault(x => x.Value == Convert.ToInt32(BranchingInstruction.Substring(0, 7), 2)).Key == "BLT")
                                {
                                    if (RegisterAddress < CompareWith)
                                    {
                                        ProgramCounter = Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2) - 2;


                                    }
                                }
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
                                        ProgramCounter = Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2) - 2;


                                    }

                                }

                                if (InstructionAndBitPatterns.FirstOrDefault(x => x.Value == Convert.ToInt32(BranchingInstruction.Substring(0, 7), 2)).Key == "BNE")
                                {
                                    if (RegisterAddress != CompareWith)
                                    {
                                        ProgramCounter = Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2) - 2;


                                    }
                                }

                                if (InstructionAndBitPatterns.FirstOrDefault(x => x.Value == Convert.ToInt32(BranchingInstruction.Substring(0, 7), 2)).Key == "BGT")
                                {
                                    if (RegisterAddress > CompareWith)
                                    {
                                        ProgramCounter = Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2) - 2;


                                    }
                                }

                                if (InstructionAndBitPatterns.FirstOrDefault(x => x.Value == Convert.ToInt32(BranchingInstruction.Substring(0, 7), 2)).Key == "BLT")
                                {
                                    if (RegisterAddress < CompareWith)
                                    {
                                        ProgramCounter = Convert.ToInt32(BranchingInstruction.Substring(7, 4), 2) - 2;


                                    }
                                }

                            }

                        }

                        //// 01 - direct //10 - immedidate // 11 - relative

                        if (TheInstruction == "MVN")
                        {
                            if (AddressingMode == "01")
                            {
                                int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                int result = ~GeneralRegisterArray[Convert.ToInt32(Operand1, 2)];
                                GeneralRegisterArray[RegisterAddress] = result;
                                GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(result, 2);

                                if (IsDenaryMode)
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(result);
                                }
                                else
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(result, 2);
                                }

                                GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.LightPink;
                                Task.Delay(250);

                                GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.White;
                            }

                            if (AddressingMode == "10")
                            {
                                int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                int result = ~Convert.ToInt32(Operand1, 2);
                                GeneralRegisterArray[RegisterAddress] = result;
                                GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(result, 2);

                                if (IsDenaryMode)
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(result);
                                }
                                else
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(result, 2);
                                }

                                GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.LightPink;
                                Task.Delay(250);

                                GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.White;
                            }

                            if (AddressingMode == "11")
                            {
                                int RegisterAddress = Convert.ToInt32(Register1, 2) - 15;
                                int result = ~MemoryArray[Convert.ToInt32(Operand1, 2)];
                                GeneralRegisterArray[RegisterAddress] = result;
                                GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(result, 2);

                                if (IsDenaryMode)
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(result);
                                }
                                else
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress].Text = Convert.ToString(result, 2);
                                }

                                GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.LightPink;
                                Task.Delay(250);

                                GeneralRegistersArrayValueLabel[RegisterAddress].BackColor = Color.White;

                            }
                        }

                    }

                }

                if (MachineCode.Length == 9)
                {
                    MachineCode = "00" + MachineCode;

                    if (InstructionAndBitPatterns.FirstOrDefault(x => x.Value == Convert.ToInt32(MachineCode.Substring(0, 7), 2)).Key == "B")
                    {
                        ProgramCounter = Convert.ToInt32(MachineCode.Substring(7, 4), 2) - 2;

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

                                if (IsDenaryMode)
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(sum);
                                    RegistersArrayValueLabel[4].Text = Convert.ToString(sum);

                                }
                                else
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(sum, 2);
                                    RegistersArrayValueLabel[4].Text = Convert.ToString(sum, 2);

                                }


                                GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.LightPink;
                                Task.Delay(250);
                                GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.White;
                                Task.Delay(250);
                            }

                            if (AddressingMode == "10")
                            {

                                sum = GeneralRegisterArray[RegisterAddress2] + Convert.ToInt32(Operand1, 2);
                                GeneralRegisterArray[RegisterAddress1] = sum;
                                GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(Convert.ToInt32(GeneralRegisterArray[RegisterAddress1]), 2);

                                if (IsDenaryMode)
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(Convert.ToInt32(GeneralRegisterArray[RegisterAddress1]));
                                    RegistersArrayValueLabel[4].Text = Convert.ToString(Convert.ToInt32(GeneralRegisterArray[RegisterAddress1]));

                                }
                                else
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(Convert.ToInt32(GeneralRegisterArray[RegisterAddress1]), 2);
                                    RegistersArrayValueLabel[4].Text = Convert.ToString(Convert.ToInt32(GeneralRegisterArray[RegisterAddress1]));
                                }

                                GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.LightPink;
                                Task.Delay(250);
                                GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.White;
                                Task.Delay(250);

                            }
                            if (AddressingMode == "11")
                            {
                                sum = GeneralRegisterArray[RegisterAddress2] + MemoryArray[Convert.ToInt32(Operand1, 2)];
                                GeneralRegisterArray[RegisterAddress1] = sum;
                                GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(sum, 2);

                                if (IsDenaryMode)
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(sum);
                                    RegistersArrayValueLabel[4].Text = Convert.ToString(Convert.ToInt32(GeneralRegisterArray[RegisterAddress1]));
                                }
                                else
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(sum, 2);
                                    RegistersArrayValueLabel[4].Text = Convert.ToString(Convert.ToInt32(GeneralRegisterArray[RegisterAddress1]), 2);
                                }

                                GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.LightPink;
                                Task.Delay(250);
                                GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.White;
                                Task.Delay(250);
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
                                result = GeneralRegisterArray[RegisterAddress2] << GeneralRegisterArray[Convert.ToInt32(Operand1, 2)];
                                GeneralRegisterArray[RegisterAddress1] = result;
                                GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(result, 2);

                                if (IsDenaryMode)
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(result);
                                }
                                else
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(result, 2);
                                }

                                GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.LightPink;
                                Task.Delay(250);

                                GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.White;
                            }

                            if (AddressingMode == "10")
                            {
                                result = GeneralRegisterArray[RegisterAddress2] << Convert.ToInt32(Operand1, 2);
                                GeneralRegisterArray[RegisterAddress1] = result;
                                GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(result, 2);

                                if (IsDenaryMode)
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(result);
                                }
                                else
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(result, 2);
                                }

                                GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.LightPink;
                                Task.Delay(100);
                                GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.White;
                            }

                            if (AddressingMode == "11")
                            {
                                result = GeneralRegisterArray[RegisterAddress2] << MemoryArray[Convert.ToInt32(Operand1, 2)];
                                GeneralRegisterArray[RegisterAddress1] = result;
                                GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(result, 2);

                                if (IsDenaryMode)
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(result);
                                }
                                else
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(result, 2);
                                }

                                GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.LightPink;
                                Task.Delay(200);
                                GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.White;
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
                                result = GeneralRegisterArray[RegisterAddress2] >> GeneralRegisterArray[Convert.ToInt32(Operand1, 2)];
                                GeneralRegisterArray[RegisterAddress1] = result;
                                GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(result, 2);

                                if (IsDenaryMode)
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(result);
                                }
                                else
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(result, 2);
                                }

                                GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.LightPink;
                                Task.Delay(200);
                                GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.White;

                            }

                            if (AddressingMode == "10")
                            {
                                result = GeneralRegisterArray[RegisterAddress2] >> Convert.ToInt32(Operand1, 2);
                                GeneralRegisterArray[RegisterAddress1] = result;
                                GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(result, 2);

                                if (IsDenaryMode)
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(result);
                                }
                                else
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(result, 2);
                                }

                                GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.LightPink;
                                Task.Delay(200);
                                GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.White;
                            }

                            if (AddressingMode == "11")
                            {
                                result = GeneralRegisterArray[RegisterAddress2] >> MemoryArray[Convert.ToInt32(Operand1, 2)];
                                GeneralRegisterArray[RegisterAddress1] = result;
                                GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(result, 2);

                                if (IsDenaryMode)
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(result);
                                }
                                else
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(result, 2);
                                }

                                GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.LightPink;
                                Task.Delay(200);
                                GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.White;
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
                                result = GeneralRegisterArray[Convert.ToInt32(Register2, 2) - 15] & GeneralRegisterArray[Convert.ToInt32(Operand1, 2)];
                                GeneralRegisterArray[Convert.ToInt32(Register1, 2) - 15] = result;
                                GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].Text = Convert.ToString(result, 2);

                                if (IsDenaryMode)
                                {
                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].Text = Convert.ToString(result);
                                }
                                else
                                {
                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].Text = Convert.ToString(result, 2);
                                }

                                GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].BackColor = Color.LightPink;
                                Task.Delay(200);

                                GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].BackColor = Color.White;
                            }

                            if (AddressingMode == "10")
                            {
                                result = GeneralRegisterArray[Convert.ToInt32(Register2, 2)] & Convert.ToInt32(Operand1, 2);
                                GeneralRegisterArray[Convert.ToInt32(Register1, 2)] = result;
                                GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2)].Text = Convert.ToString(result, 2);

                                if (IsDenaryMode)
                                {
                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2)].Text = Convert.ToString(result);
                                }
                                else
                                {
                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2)].Text = Convert.ToString(result, 2);
                                }

                                GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2)].BackColor = Color.LightPink;
                                Task.Delay(200);
                                GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2)].BackColor = Color.White;
                            }

                            if (AddressingMode == "11")
                            {
                                result = GeneralRegisterArray[Convert.ToInt32(Register2, 2)] & MemoryArray[Convert.ToInt32(Operand1, 2)];
                                GeneralRegisterArray[Convert.ToInt32(Register1, 2)] = result;
                                GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2)].Text = Convert.ToString(result, 2);

                                if (IsDenaryMode)
                                {
                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2)].Text = Convert.ToString(result);
                                }
                                else
                                {
                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2)].Text = Convert.ToString(result, 2);
                                }

                                GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2)].BackColor = Color.LightPink;
                                Task.Delay(200);

                                GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2)].BackColor = Color.White;

                            }
                        }

                        if (TheInstruction == "ORR")
                        {
                            int result = 0;
                            Instruction = MachineCode.Substring(0, 7);
                            AddressingMode = MachineCode.Substring(7, 2);
                            Register1 = MachineCode.Substring(9, 5);
                            Register2 = MachineCode.Substring(14, 5);
                            Operand1 = MachineCode.Substring(19, 4);

                            if (AddressingMode == "01")
                            {
                                result = GeneralRegisterArray[Convert.ToInt32(Register2, 2) - 15] | GeneralRegisterArray[Convert.ToInt32(Operand1, 2)];
                                GeneralRegisterArray[Convert.ToInt32(Register1, 2)] = result;
                                GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].Text = Convert.ToString(result, 2);

                                if (IsDenaryMode)
                                {
                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].Text = Convert.ToString(result);
                                }
                                else
                                {
                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].Text = Convert.ToString(result, 2);
                                }

                                GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].BackColor = Color.LightPink;
                                Task.Delay(200);

                                GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].BackColor = Color.White;
                            }

                            if (AddressingMode == "10")
                            {
                                result = GeneralRegisterArray[Convert.ToInt32(Register2, 2) - 15] | Convert.ToInt32(Operand1, 2);
                                GeneralRegisterArray[Convert.ToInt32(Register1, 2) - 15] = result;
                                GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2)].Text = Convert.ToString(result);

                                if (IsDenaryMode)
                                {
                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].Text = Convert.ToString(result);
                                }
                                else
                                {
                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].Text = Convert.ToString(result, 2);
                                }

                                GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].BackColor = Color.LightPink;
                                Task.Delay(200);

                                GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].BackColor = Color.White;
                            }

                            if (AddressingMode == "11")
                            {
                                result = GeneralRegisterArray[Convert.ToInt32(Register2, 2) - 15] | MemoryArray[Convert.ToInt32(Operand1, 2)];
                                GeneralRegisterArray[Convert.ToInt32(Register1, 2) - 15] = result;
                                GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2)].Text = Convert.ToString(result, 2);

                                if (IsDenaryMode)
                                {
                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].Text = Convert.ToString(result);
                                }
                                else
                                {
                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].Text = Convert.ToString(result, 2);
                                }

                                GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].BackColor = Color.LightPink;
                                Task.Delay(200);

                                GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].BackColor = Color.White;
                            }
                        }

                        if (TheInstruction == "EOR")
                        {
                            int result = 0;
                            Instruction = MachineCode.Substring(0, 7);
                            AddressingMode = MachineCode.Substring(7, 2);
                            Register1 = MachineCode.Substring(9, 5);
                            Register2 = MachineCode.Substring(14, 5);
                            Operand1 = MachineCode.Substring(19, 4);

                            if (AddressingMode == "01")
                            {
                                result = GeneralRegisterArray[Convert.ToInt32(Register2, 2) - 15] ^ GeneralRegisterArray[Convert.ToInt32(Operand1, 2)];
                                GeneralRegisterArray[Convert.ToInt32(Register1, 2) - 15] = result;
                                GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2)].Text = Convert.ToString(result, 2);

                                if (IsDenaryMode)
                                {
                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].Text = Convert.ToString(result);
                                }
                                else
                                {
                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].Text = Convert.ToString(result, 2);
                                }

                                GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].BackColor = Color.LightPink;
                                Task.Delay(200);

                                GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].BackColor = Color.White;
                            }

                            if (AddressingMode == "10")
                            {
                                result = GeneralRegisterArray[Convert.ToInt32(Register2, 2) - 15] ^ Convert.ToInt32(Operand1, 2);
                                GeneralRegisterArray[Convert.ToInt32(Register1, 2) - 15] = result;
                                GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2)].Text = Convert.ToString(result);

                                if (IsDenaryMode)
                                {
                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].Text = Convert.ToString(result);
                                }
                                else
                                {
                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].Text = Convert.ToString(result, 2);
                                }

                                GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].BackColor = Color.LightPink;
                                Task.Delay(200);

                                GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].BackColor = Color.White;
                            }

                            if (AddressingMode == "11")
                            {
                                result = GeneralRegisterArray[Convert.ToInt32(Register2, 2) - 15] ^ MemoryArray[Convert.ToInt32(Operand1, 2)];
                                GeneralRegisterArray[Convert.ToInt32(Register1, 2) - 15] = result;
                                GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2)].Text = Convert.ToString(result, 2);

                                if (IsDenaryMode)
                                {
                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].Text = Convert.ToString(result);
                                }
                                else
                                {
                                    GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].Text = Convert.ToString(result, 2);
                                }

                                GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].BackColor = Color.LightPink;
                                Task.Delay(200);

                                GeneralRegistersArrayValueLabel[Convert.ToInt32(Register1, 2) - 15].BackColor = Color.White;
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

                                if (IsDenaryMode)
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(sum);
                                    RegistersArrayValueLabel[4].Text = Convert.ToString(sum);

                                }
                                else
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(sum, 2);
                                    RegistersArrayValueLabel[4].Text = Convert.ToString(sum, 2);
                                }

                                GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.LightPink;
                                Task.Delay(200);
                                GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.White;
                                Task.Delay(200);
                            }

                            if (AddressingMode == "10")
                            {
                                sum = GeneralRegisterArray[RegisterAddress2] - Convert.ToInt32(Operand1, 2);
                                GeneralRegisterArray[RegisterAddress1] = sum;
                                GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(sum, 2);

                                if (IsDenaryMode)
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(sum);
                                    RegistersArrayValueLabel[4].Text = Convert.ToString(sum);
                                }
                                else
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(sum, 2);
                                    RegistersArrayValueLabel[4].Text = Convert.ToString(sum, 2);
                                }

                                GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.LightPink;
                                Task.Delay(200);
                                GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.White;
                                Task.Delay(200);
                            }

                            if (AddressingMode == "11")
                            {
                                sum = GeneralRegisterArray[RegisterAddress2] - MemoryArray[Convert.ToInt32(Operand1, 2)];
                                GeneralRegisterArray[RegisterAddress1] = sum;
                                GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(sum, 2);

                                if (IsDenaryMode)
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(sum);
                                    RegistersArrayValueLabel[4].Text = Convert.ToString(sum);
                                }
                                else
                                {
                                    GeneralRegistersArrayValueLabel[RegisterAddress1].Text = Convert.ToString(sum, 2);
                                    RegistersArrayValueLabel[4].Text = Convert.ToString(sum, 2);
                                }

                                GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.LightPink;
                                Task.Delay(250);
                                GeneralRegistersArrayValueLabel[RegisterAddress1].BackColor = Color.White;
                                Task.Delay(250);
                            }
                        }

                    }
                }
            }

        }
        private void LoadIntoMemory()
        {
            int MachinePattern = 0;
            List<string> LineItems = new List<string>();
            List<string> LineOfInstructions = Tokenisation().ToList();
            int MemoryAddress = 0;
            bool IsImmediate = false;
            bool IsDirect = false;
            bool IsRelative = false;

            ResolveLabels();
            for (int i = 0; i < LineOfInstructions.Count; i++)
            {

                LineItems = LineOfInstructions[i].Split(' ').ToList();
                for (int p = 0; p < LineItems.Count; p++)
                {
                    if (LineItems[p] == "" || LineItems[p] == " ")
                    {
                        LineItems.RemoveAt(p);
                    }
                }

                try
                {
                    if (LineItems[LineItems.Count - 1].Contains("#"))
                    {
                        IsRelative = false;
                        IsDirect = false;
                        IsImmediate = true;
                    }


                    else if (!LineItems[LineItems.Count - 1].Contains("#") && !LineItems[LineItems.Count - 1].Contains("@")) ////relative addressing 
                    {
                        IsDirect = true;
                        IsImmediate = false;
                        IsRelative = false;
                    }

                    else if (LineItems[LineItems.Count - 1].Contains("@"))
                    {
                        IsRelative = true;
                        IsDirect = false;
                        IsImmediate = false;
                    }
                }
                catch (Exception e)
                {
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

                            MachinePattern += LabelsAndValues.FirstOrDefault(x => x.Key == LineItems[j + 1]).Value;
                        }
                    }
                    else if (LineItems[j][0] == 'R' && !InstructionAndBitPatterns.ContainsKey(LineItems[j])) //detect registers
                    {
                        MachinePattern = MachinePattern << 5;
                        MachinePattern += (Convert.ToInt32(LineItems[j].Substring(1)) + 15);
                    }

                    else if (LineItems[j][0] == '#')// detect immedate addressing
                    {
                        MachinePattern = MachinePattern << 4;
                        MachinePattern += Convert.ToInt32(LineItems[j].Substring(1));
                    }

                    else if (LineItems[j][0] == '?')//detects relative addressing
                    {
                        MachinePattern = MachinePattern << 4;
                        MachinePattern += Convert.ToInt32(LineItems[j].Substring(1));
                    }

                    else if (char.IsDigit(LineItems[j][0])) //detects direct addressing
                    {
                        MachinePattern = MachinePattern << 4;
                        MachinePattern += Convert.ToInt32(LineItems[j]);
                    }

                    else if (!AQAInstructionSet.Contains(LineItems[j]) && !BranchingLabels.ContainsKey(LineItems[j]) && LineItems[j][0] != 'R' && int.TryParse(LineItems[j], out _) == false && LineItems[j][0] != '#')
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
                    if (!IsDenaryMode)
                    {
                        MainMemoryArrayValue[i].Text = Convert.ToString(MemoryArray[i], 2);
                    }
                    if (IsDenaryMode)
                    {

                        MainMemoryArrayValue[i].Text = Convert.ToString(MemoryArray[i]);

                    }
                }

            }

        }

        private void DisplayMachineCode()
        {
            MachineCodeTextBox.Text = "";
            List<string> ListOfMnemonics = ResolveLabels();
            ListOfMnemonics = ListOfMnemonics.Where(x => !string.IsNullOrEmpty(x)).ToList();
            string MachinePattern = "";
            int count = 0;


            for (int i = 0; i < ListOfMnemonics.Count / 2; i++)
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
                ///essensially this methods check that the labels in the code have been found twice and keeps the line number of the label that it will be looping back to.
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

            //CHECK IF THE VALUES ARE ALREADY IN DENARY IF NOT THEN CONVERTS TO MACHINE CODE AND VICE VERSA.
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
                System.Windows.Forms.MessageBox.Show("Denary mode is ON");
            }
            else
            {
                for (int i = 0; i < MemoryArray.Length; i++)
                {
                    tmp = MemoryArray[i];
                    MainMemoryArrayValue[i].Text = Convert.ToString(tmp, 2);
                }
                for (int i = 0; i < GeneralRegisterArray.Length; i++)
                {
                    tmp2 = GeneralRegisterArray[i];
                    GeneralRegistersArrayValueLabel[i].Text = Convert.ToString(tmp2, 2);
                }
                IsDenaryMode = false;
                System.Windows.Forms.MessageBox.Show("Denary mode is OFF");
            }

        }

        private void lMCModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < MemoryArray.Length; i++)
            {
                MemoryArray[i] = 0;
            }

            for (int i = 0; i < MainMemoryArrayValue.Length; i++)
            {
                MainMemoryArrayValue[i].Text = "0";
            }
        }

        private void addressingToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            ///checks if the code is running and if it is stops running it
            if (ProgramIsRunning)
            {
                ProgramIsRunning = false;
            }
            else if (!ProgramIsRunning)
            {
                ProgramIsRunning = true;
                Interpret();
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ProgramIsRunning = false;
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            UserInputBox.Text = "INP R0 #1\r\nLOOP ADD R0 R0 #1\r\nCMP R0 #5\r\nBLT LOOP"; ///example
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            UserInputBox.Text = "LDRM R8 #2\r\nLDRM R9 #3\r\nLDR R4 8\r\nLDR R5 9\r\nADD R6 R5 4"; ///example
        }

        private void instructionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("LDR Rd, <memory ref> Load the value stored in the memory location \r\nspecified by <memory ref> into register d. \r\nSTR Rd, <memory ref> Store the value that is in register d into the \r\nmemory location specified by <memory ref>. \r\nADD Rd, Rn, <operand2> Add the value specified in <operand2> to the value in register n and store the result in register d. \r\nSUB Rd, Rn, <operand2> Subtract the value specified by <operand2> \r\nfrom the value in register n and store the result \r\nin register d. \r\nMOV Rd, <operand2> Copy the value specified by <operand2> into \r\nregister d. \r\nCMP Rn, <operand2> Compare the value stored in register n with the \r\nvalue specified by <operand2>. \r\nB <label> Always branch to the instruction at position <label> in the program. \r\nB<condition> <label> Branch to the instruction at position <label> if \r\nthe last comparison met the criterion specified by <condition>.  Possible values for <condition> and their meanings are: \r\nEQ: equal to   NE: not equal to \r\nGT: greater than  LT: less than \r\nAND Rd, Rn, <operand2> Perform a bitwise logical AND operation \r\nbetween the value in register n and the value specified by <operand2> and store the result in register d. \r\nORR Rd, Rn, <operand2> Perform a bitwise logical OR operation between the value in register n and the value specified by \r\n<operand2> and store the result in register d. \r\nEOR Rd, Rn, <operand2> Perform a bitwise logical XOR (exclusive or) \r\noperation between the value in register n and the value specified by <operand2> and store the result in register d. \r\nMVN Rd, <operand2> Perform a bitwise logical NOT operation on the \r\nvalue specified by <operand2> and store the result in register d. \r\nLSL Rd, Rn, <operand2> Logically shift left the value stored in register n by the number of bits specified by <operand2> and store the result in register d. \r\nLSR Rd, Rn, <operand2> Logically shift right the value stored in register n by the number of bits specified by <operand2>  and store the result in register d. \r\nHALT \r\nStops the execution of the program. \r\nOUT Outputs the value in register d."); ///explanation
        }

        FastColoredTextBoxNS.Style DarkKhakiStyle = new TextStyle(Brushes.DarkKhaki, null, FontStyle.Regular);
        FastColoredTextBoxNS.Style GreenStyle = new TextStyle(Brushes.Green, null, FontStyle.Regular);
        FastColoredTextBoxNS.Style BlueStyle = new TextStyle(Brushes.Blue, null, FontStyle.Regular);
        FastColoredTextBoxNS.Style PurpleStyle = new TextStyle(Brushes.Purple, null, FontStyle.Regular);
        FastColoredTextBoxNS.Style RedHighlight = new TextStyle(Brushes.Red, null, FontStyle.Underline);

        private void UserInputBox_TextChanged(object sender, FastColoredTextBoxNS.TextChangedEventArgs e)
        {
            WordCount.Text = "Word Count: " + UserInputBox.TextLength; ///WORD COUNT...
            string word = "";

            for (int i = 0; i < UserInputBox.Text.Length; i++)
            {
                word += UserInputBox.Text[i];

                if (UserInputBox.Text[i] == ' ' || UserInputBox.Text[i] == '\n')
                {
                    if (AQAInstructionSet.Contains(word.Trim()))
                    {
                        e.ChangedRange.SetStyle(BlueStyle, word);
                        word = "";
                        ///INSTRUCTIONS
                    }

                    if (Regex.IsMatch(word, @"R\d+")) //Words that START WITH R THEN 1 OR MORE NUMBERS AFTER (REGISTERS)
                    {
                        e.ChangedRange.SetStyle(PurpleStyle, word);
                        word = "";
                    }

                    ///BASICALLY LOOPS OR ANY WORDS NOT A INSTRUCTION OR REGISTER OR NUMBER
                    if (!AQAInstructionSet.Contains(word.Trim()) && !word.Trim().Contains('R') && !word.Trim().Contains('#') && !word.Trim().Contains('@') && !Regex.IsMatch(word.Trim(), @"^\d+"))
                    {
                        e.ChangedRange.SetStyle(DarkKhakiStyle, word);
                        word = "";
                    }

                    if (word.Trim().Contains('#') || word.Trim().Contains('@') || Regex.IsMatch(word.Trim(), @"^\d+"))
                    {
                        e.ChangedRange.SetStyle(GreenStyle, word);
                        word = "";
                    } // OPERANDS ISH
                }
            }
        }

        private void clearRegistersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < RegisterArray.Length; i++)
            {
                RegisterArray[i] = 0;
            }

            for (int i = 0; i < GeneralRegistersArrayValueLabel.Length; i++)
            {
                GeneralRegistersArrayValueLabel[i].Text = "0";
            }
            ///clears registers
        }

        private void executionSpeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LabelOfSpeed.Visible = true;
            SpeedOfExecution_txt.Visible = true;
            ConfirmSpeed.Visible = true;
            textBox1.Visible = true;
            ///you can change execution speed
        }

        private void SetExecutionSpeed(int speed)
        {
            wait = speed;//setter
        }

        private void ConfirmSpeed_Click(object sender, EventArgs e)
        {
            ///checks that the input it not negative asnd is a digit and then changes speed else it gives an error emssage and you can enter another value
            if (Regex.IsMatch(SpeedOfExecution_txt.Text, @"\d+") && Convert.ToInt32(SpeedOfExecution_txt.Text) > 0)
            {
                SetExecutionSpeed(Convert.ToInt32(SpeedOfExecution_txt.Text));
                System.Windows.Forms.MessageBox.Show("Execution Speed has been changed");
                LabelOfSpeed.Visible = false;
                SpeedOfExecution_txt.Visible = false;
                ConfirmSpeed.Visible = false;
                textBox1.Visible = false;

            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Invalid Input");
            }
        }

        private void fAQToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("FAQ\r\nHow does this program work?\r\nThis is an assembler created by Emmanuel Agyena. It has the whole AQA instruction set and other extra instructions like INP, LDRM, and OUT. Programmers and students can program using this IDE I have created for fun or to solve programs that may appear in their exams. \r\n\r\nWhat does the green arrow do?\r\nOnce you have written your program and loaded it into memory, it will execute the next instruction each time you click it.\r\n\r\nWhat does the red sign do?\r\nWhen you select the red sign, it completely stops the execution of the program.\r\n\r\n");
        }

        private string TwosCompliment(string val)
        {

            string InvertedBinary = "";
            string TwosComplimentForm = "";

            if (IsDenaryMode)
            {
                TwosComplimentForm = Convert.ToString(~(Convert.ToInt32(val)) + 1, 2);
                return TwosComplimentForm;
            }
            else
            {
                TwosComplimentForm = Convert.ToString(~Convert.ToInt32(val, 2) + 1, 2);
            }
            return "0";

        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            ///checks that their is no errors before execution a cycle.
            ProgramIsRunning = true;
            List<int> LineOfErrorsAndNumberOfErrors = ValidSyntax();
            if (LineOfErrorsAndNumberOfErrors[LineOfErrorsAndNumberOfErrors.Count - 1] != 0)
            {
                System.Windows.Forms.MessageBox.Show("Input is invalid");
            }
            else
            {
                StepInterpret();
                ProgramCounter++;
            }

        }

        private void DisplayMachineCodeInText()
        {

            ///first it checks which method of asddressing was used and then adds the addressing code to the string and then finally it adds the value of the register to it.
            int MachinePattern = 0;
            List<string> LineItems = new List<string>();
            List<string> LineOfInstructions = Tokenisation().ToList();
            bool IsImmediate = false;
            bool IsDirect = false;
            bool IsRelative = false;

            ResolveLabels();
            for (int i = 0; i < LineOfInstructions.Count; i++)
            {

                LineItems = LineOfInstructions[i].Split(' ').ToList();
                for (int p = 0; p < LineItems.Count; p++)
                {
                    if (LineItems[p] == "" || LineItems[p] == " ")
                    {
                        LineItems.RemoveAt(p);
                    }
                }
                try
                {


                if (LineItems[LineItems.Count - 1].Contains("#"))
                {
                    IsRelative = false;
                    IsDirect = false;
                    IsImmediate = true;
                }

                else if (!LineItems[LineItems.Count - 1].Contains("#") && !LineItems[LineItems.Count - 1].Contains("@")) ////relative addressing 
                {
                    IsDirect = true;
                    IsImmediate = false;
                    IsRelative = false;
                }

                else if (LineItems[LineItems.Count - 1].Contains("@"))
                {
                    IsRelative = true;
                    IsDirect = false;
                    IsImmediate = false;
                }
                }
                catch (Exception e)
                {

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

                            MachinePattern += LabelsAndValues.FirstOrDefault(x => x.Key == LineItems[j + 1]).Value;
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

                    else if (LineItems[j][0] == '@')
                    {
                        MachinePattern = MachinePattern << 4;
                        MachinePattern += Convert.ToInt32(LineItems[j].Substring(1));
                    }

                    else if (char.IsDigit(LineItems[j][0]))
                    {
                        MachinePattern = MachinePattern << 4;
                        MachinePattern += Convert.ToInt32(LineItems[j]);
                    }

                    else if (!AQAInstructionSet.Contains(LineItems[j]) && !BranchingLabels.ContainsKey(LineItems[j]) && LineItems[j][0] != 'R' && int.TryParse(LineItems[j], out _) == false && LineItems[j][0] != '#')
                    {
                        MachinePattern = MachinePattern << 4;
                        MachinePattern += j;
                    }

                }

                MachineCodeTextBox.Text += Convert.ToString(MachinePattern, 2) + '\n';
                MachinePattern = 0;

            }
        }
        private void viewMachineCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] lines = UserInputBox.Lines.ToArray();
            DisplayMachineCodeInText();
        }

        private void UserInputBox_Load(object sender, EventArgs e)
        {

        }

        private void Machine_Load(object sender, EventArgs e)
        {

        }
    }
}