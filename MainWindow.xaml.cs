using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Ranger
{
    public partial class MainWindow : Window

    {
        public static ObservableCollection<Skill> Skills = new ObservableCollection<Skill>();
        public static ObservableCollection<Resource> Resources = new ObservableCollection<Resource>();

        Dictionary<DaysOfWeek, int[]> DayOfWeekCoverage;
        Rectangle[] mondayRects;
        Rectangle[] tuesdayRects;
        Rectangle[] wednesdayRects;
        Rectangle[] thursdayRects;
        Rectangle[] fridayRects;
        Rectangle[] saturdayRects;
        Rectangle[] sundayRects;

        const int MaxHours = 24;

        private static Resource? SelectedResource;
        private static string? FilePath;

        private string DefaultTitle = "Ranger (v 0.1)"; 
        private string NewTitle = "<New>";

        public static bool HasChanges { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DgSkills.ItemsSource = Skills;
            DgResources.ItemsSource = Resources;
            this.Title = DefaultTitle + " " + NewTitle;
            this.DataContext = this;

            HasChanges = false;
            CreateRectArray();
            PlotCoverageUI(null);
            SetDefaultWindow();
        }

        #region ToolBar
        private void BtnSaveFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(FilePath))
                {
                    BtnSaveFileAs_Click(sender, e);
                }
                else
                {
                    InputFile? inputFile = new InputFile();
                    inputFile.Skills = Skills.ToList();
                    inputFile.Resources = Resources.ToList();

                    Util.SerializeObject(inputFile, FilePath);
                    this.Title = DefaultTitle + " " + FilePath;
                    HasChanges = false;
                }   
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void BtnSaveFileAs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                InputFile? inputFile = new InputFile();
                inputFile.Skills = Skills.ToList();
                inputFile.Resources = Resources.ToList();

                Microsoft.Win32.SaveFileDialog saveFileDlg = new Microsoft.Win32.SaveFileDialog();
                saveFileDlg.Title = "Save Ranger file";
                saveFileDlg.Filter = "Ranger file|*.rng";
                saveFileDlg.ShowDialog();
                if (saveFileDlg.FileName != "")
                {
                    Util.SerializeObject(inputFile, saveFileDlg.FileName);
                    this.Title = DefaultTitle + " " + saveFileDlg.FileName;
                    FilePath = saveFileDlg.FileName;
                }
                HasChanges = false;
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void BtnSelectFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (HasChanges == true &&  MessageBox.Show("If you have any unsaved changes, you will loose all of them. Continue?", "Save changes", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                {
                    return;
                }

                InputFile inputFile = new InputFile();
                Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
                openFileDlg.Title = "Open Ranger file";
                openFileDlg.Filter = "Ranger file|*.rng";
                openFileDlg.ShowDialog();
                if (openFileDlg.FileName != "")
                {
                    FilePath = openFileDlg.FileName;
                    inputFile = (InputFile)Util.DeSerializeObject(inputFile, FilePath);
                    if (inputFile is null)
                    {
                        throw new ArgumentException("Input file is not in correct format.");
                    }
                    else
                    {
                        Resources.Clear();
                        Skills.Clear();
                        foreach (var resource in inputFile.Resources)
                        {
                            Resources.Add(resource);
                        }

                        foreach (var skill in inputFile.Skills)
                        {
                            Skills.Add(skill);
                        }

                        this.Title = DefaultTitle + " " + FilePath;
                    }
                }
                HasChanges = false;
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (HasChanges == true && MessageBox.Show("If you have any unsaved changes, you will loose all of them. Continue?", "Save changes", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                {
                    return;
                }

                SetDefaultWindow();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void SetDefaultWindow()
        {
            FilePath = "";
            this.Title = DefaultTitle + " " + NewTitle;

            Skills.Clear();
            Resources.Clear();
        }

        #endregion

        #region SkillGrid
        private void BtnAddSkill_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HasChanges = true;
                Skills.Add(new Skill(Util.GetRandomString(14)));
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void BtnDeleteSkill_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HasChanges = true;
                Skill skill = (Skill)((System.Windows.Controls.Button)e.Source).DataContext;
                Skills.Remove(skill);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }
        #endregion

        #region ResourceGrid
        private void BtnAddResource_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HasChanges = true;
                Resources.Add(new Resource(Util.GetRandomString(14)));
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void BtnCopyResource_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HasChanges = true;
                if (SelectedResource is null)
                {
                    throw new InvalidOperationException("No resource selected. Please select a resource.");
                }

                Resources.Add(Util.GetResourceCopy(SelectedResource));
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void BtnDeleteResource_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HasChanges = true;
                Resource resource = (Resource)((System.Windows.Controls.Button)e.Source).DataContext;

                Resources.Remove(resource);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void DgResources_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                SelectedResource = null;
                DgResourcesSkillMap.ItemsSource = null;
                DgShiftAllocation.ItemsSource = null;

                if (DgResources.SelectedIndex > -1)
                {
                    SelectedResource = (Resource)DgResources.SelectedItem;
                    DgResourcesSkillMap.ItemsSource = SelectedResource.Skills;
                    DgShiftAllocation.ItemsSource = SelectedResource.AvailabilityWindows;
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }
        #endregion

        #region SkillmapGrid
        private void BtnMapResourceSkill_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Skills.Count == 0 || Resources.Count == 0)
                {
                    throw new InvalidOperationException("Create atleast one skill and resource.");
                }

                if (SelectedResource is null)
                {
                    throw new InvalidOperationException("No resource selected. Please select a resource.");
                }

                if (DgSkills.SelectedIndex < 0)
                {
                    throw new InvalidOperationException("No skill selected. Please select a skill.");
                }

                Skill selectedSkill = (Skill)DgSkills.SelectedItem;

                if (SelectedResource.Skills.Contains(selectedSkill))
                {
                    throw new InvalidOperationException("The selected skill is already mapped to the selected resource.");
                }

                SelectedResource?.Skills.Add(selectedSkill);
                HasChanges = true;
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void BtnUnmapResourceSkill_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Skill skill = (Skill)((System.Windows.Controls.Button)e.Source).DataContext;
                SelectedResource.Skills.Remove(skill);
                HasChanges = true;
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }
        #endregion

        #region ShiftAllocationGrid
        private void BtnAllocateShift_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedResource is not null)
                {
                    SelectedResource.AvailabilityWindows.Add(new AvailabilityWindow(DaysOfWeek.Mon, TimeOnly.MinValue, TimeOnly.MinValue));
                    HasChanges = true;
                }
                else
                {
                    MessageBox.Show("No resource selected. Select a resource by clicking on the 'Select' button.", "Select resource", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void BtnDeleteShift_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AvailabilityWindow aw = (AvailabilityWindow)((System.Windows.Controls.Button)e.Source).DataContext;
                SelectedResource?.AvailabilityWindows.Remove(aw);
                HasChanges = true;
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }
        #endregion
      
        private void ShowError(string errorMsg)
        {
            MessageBox.Show($"An Error has occured: {errorMsg}","Oops!", MessageBoxButton.OK,MessageBoxImage.Error);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (HasChanges == true && MessageBox.Show("If you have any unsaved changes, you will loose all of them. Continue?", "Save changes", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                {
                   e.Cancel = true;
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void PlotCoverage(AvailabilityWindow? aw)
        {
            try
            {
                if (aw is null)
                {
                    return;
                }

                if (aw.StartTime.Hour < aw.EndTime.Hour)
                {
                    for (int i = aw.StartTime.Hour; i < aw.EndTime.Hour; i++)
                    {
                        DayOfWeekCoverage[aw.DayOfWeek][i] += 1;
                    }
                }
                else if (aw.StartTime.Hour > aw.EndTime.Hour)
                {
                    for (int i = aw.StartTime.Hour; i < 24; i++)
                    {
                        DayOfWeekCoverage[aw.DayOfWeek][i] += 1;
                    }

                    LinkedList<string> daysList = new LinkedList<string>(Enum.GetNames(typeof(DaysOfWeek)));

                    //find next day
                    DaysOfWeek nextDay = DaysOfWeek.Mon;

                    LinkedListNode<string> currentNode = daysList.Find(aw.DayOfWeek.ToString());
                    LinkedListNode<string> nextNode = currentNode.Next ?? daysList.First;
                    _=Enum.TryParse<DaysOfWeek>(nextNode.Value, out nextDay);

                    //next day found, now plot
                    for (int i = 0; i < aw.EndTime.Hour; i++)
                    {
                        DayOfWeekCoverage[nextDay][i] += 1;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void BtnRefreshPlot_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Skill skill = (Skill)CboSkills.SelectedItem;
                if (skill is null) return;

                PlotCoverageUI(null);

                DayOfWeekCoverage = new();
                foreach (DaysOfWeek day in Enum.GetValues(typeof(DaysOfWeek)))
                {
                    DayOfWeekCoverage.Add(day, new int[24]);
                }

                foreach (Resource res in Resources)
                {
                    if (res.Skills.Any(x => x.Name == skill.Name))
                    {
                        foreach (DaysOfWeek day in Enum.GetValues(typeof(DaysOfWeek)))
                        {
                            AvailabilityWindow? aw = null;
                            try
                            {
                                aw = res.AvailabilityWindows.SingleOrDefault(x => x.DayOfWeek == day);
                            }
                            catch (System.InvalidOperationException)
                            {
                                ShowError("Duplicate shift found for resource " + res.Name);
                            }
                            
                            PlotCoverage(aw);
                        }
                    }
                }

                PlotCoverageUI(DayOfWeekCoverage);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void PlotCoverageUI(Dictionary<DaysOfWeek, int[]>? plot)
        {
            try
            {
                if(plot is null) //clean the UI bars
                {
                    for (int i = 0; i < MaxHours; i++)
                    {
                        mondayRects[i].Fill = Brushes.Gray;
                        mondayRects[i].ToolTip = "";

                        tuesdayRects[i].Fill = Brushes.Gray;
                        tuesdayRects[i].ToolTip = "";

                        wednesdayRects[i].Fill = Brushes.Gray;
                        wednesdayRects[i].ToolTip = "";

                        thursdayRects[i].Fill = Brushes.Gray;
                        thursdayRects[i].ToolTip = "";

                        fridayRects[i].Fill = Brushes.Gray;
                        fridayRects[i].ToolTip = "";

                        saturdayRects[i].Fill = Brushes.Gray;
                        saturdayRects[i].ToolTip = "";

                        sundayRects[i].Fill = Brushes.Gray;
                        sundayRects[i].ToolTip = "";
                    }
                }
                else
                {
                    for (int i = 0; i < MaxHours; i++)
                    {
                        mondayRects[i].Fill = new SolidColorBrush(GetHeatMapColor(DayOfWeekCoverage[DaysOfWeek.Mon][i], 0, Resources.Count));
                        mondayRects[i].ToolTip = new ToolTip() { Content = DayOfWeekCoverage[DaysOfWeek.Mon][i] };

                        tuesdayRects[i].Fill = new SolidColorBrush(GetHeatMapColor(DayOfWeekCoverage[DaysOfWeek.Tue][i], 0, Resources.Count));
                        tuesdayRects[i].ToolTip = new ToolTip() { Content = DayOfWeekCoverage[DaysOfWeek.Tue][i] };

                        wednesdayRects[i].Fill = new SolidColorBrush(GetHeatMapColor(DayOfWeekCoverage[DaysOfWeek.Wed][i], 0, Resources.Count));
                        wednesdayRects[i].ToolTip = new ToolTip() { Content = DayOfWeekCoverage[DaysOfWeek.Wed][i] };

                        thursdayRects[i].Fill = new SolidColorBrush(GetHeatMapColor(DayOfWeekCoverage[DaysOfWeek.Thu][i], 0, Resources.Count));
                        thursdayRects[i].ToolTip = new ToolTip() { Content = DayOfWeekCoverage[DaysOfWeek.Thu][i] };

                        fridayRects[i].Fill = new SolidColorBrush(GetHeatMapColor(DayOfWeekCoverage[DaysOfWeek.Fri][i], 0, Resources.Count));
                        fridayRects[i].ToolTip = new ToolTip() { Content = DayOfWeekCoverage[DaysOfWeek.Fri][i] };

                        saturdayRects[i].Fill = new SolidColorBrush(GetHeatMapColor(DayOfWeekCoverage[DaysOfWeek.Sat][i], 0, Resources.Count));
                        saturdayRects[i].ToolTip = new ToolTip() { Content = DayOfWeekCoverage[DaysOfWeek.Sat][i] };

                        sundayRects[i].Fill = new SolidColorBrush(GetHeatMapColor(DayOfWeekCoverage[DaysOfWeek.Sun][i], 0, Resources.Count));
                        sundayRects[i].ToolTip = new ToolTip() { Content = DayOfWeekCoverage[DaysOfWeek.Sun][i] };
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private Color GetHeatMapColor(double value, double min, double max)
        {
            Color firstColour = Colors.LightSkyBlue; //LightSkyBlue
            Color secondColour = Colors.RoyalBlue; //RoyalBlue

            return LerpColor(value,firstColour,secondColour);

            // Example: Take the RGB
            //135-206-250 // Light Sky Blue
            // 65-105-225 // Royal Blue
            // 70-101-25 // Delta

            int rOffset = Math.Max(firstColour.R, secondColour.R);
            int gOffset = Math.Max(firstColour.G, secondColour.G);
            int bOffset = Math.Max(firstColour.B, secondColour.B);

            int deltaR = Math.Abs(firstColour.R - secondColour.R);
            int deltaG = Math.Abs(firstColour.G - secondColour.G);
            int deltaB = Math.Abs(firstColour.B - secondColour.B);

            double deniminator = (max - min);
            if (deniminator == 0)
            {
                deniminator = 1; // to avoid divid by 0 error
            }
            double val = (value - min) / deniminator;
            int r = rOffset - Convert.ToByte(deltaR * (1 - val));
            int g = gOffset - Convert.ToByte(deltaG * (1 - val));
            int b = bOffset - Convert.ToByte(deltaB * (1 - val));

            return Color.FromArgb(255, (byte)r, (byte)g, (byte)b);
        }

        public Color LerpColor(double value, Color startColor, Color endColor)
        {
            // reuse some code from the HeatMapColor

            int r = (int)((endColor.R * value) + (startColor.R * (1 - value)));
            int g = (int)((endColor.G * value) + (startColor.G * (1 - value)));
            int b = (int)((endColor.B * value) + (startColor.B * (1 - value)));

            return Color.FromArgb(255, (byte)r, (byte)g, (byte)b);
        }


        private void CreateRectArray()
        {
            mondayRects = new Rectangle[24];
            mondayRects[0] = MonRect0;
            mondayRects[1] = MonRect1;
            mondayRects[2] = MonRect2;
            mondayRects[3] = MonRect3;
            mondayRects[4] = MonRect4;
            mondayRects[5] = MonRect5;
            mondayRects[6] = MonRect6;
            mondayRects[7] = MonRect7;
            mondayRects[8] = MonRect8;
            mondayRects[9] = MonRect9;
            mondayRects[10] = MonRect10;
            mondayRects[11] = MonRect11;
            mondayRects[12] = MonRect12;
            mondayRects[13] = MonRect13;
            mondayRects[14] = MonRect14;
            mondayRects[15] = MonRect15;
            mondayRects[16] = MonRect16;
            mondayRects[17] = MonRect17;
            mondayRects[18] = MonRect18;
            mondayRects[19] = MonRect19;
            mondayRects[20] = MonRect20;
            mondayRects[21] = MonRect21;
            mondayRects[22] = MonRect22;
            mondayRects[23] = MonRect23;

            tuesdayRects = new Rectangle[24];
            tuesdayRects[0] = TueRect0;
            tuesdayRects[1] = TueRect1;
            tuesdayRects[2] = TueRect2;
            tuesdayRects[3] = TueRect3;
            tuesdayRects[4] = TueRect4;
            tuesdayRects[5] = TueRect5;
            tuesdayRects[6] = TueRect6;
            tuesdayRects[7] = TueRect7;
            tuesdayRects[8] = TueRect8;
            tuesdayRects[9] = TueRect9;
            tuesdayRects[10] = TueRect10;
            tuesdayRects[11] = TueRect11;
            tuesdayRects[12] = TueRect12;
            tuesdayRects[13] = TueRect13;
            tuesdayRects[14] = TueRect14;
            tuesdayRects[15] = TueRect15;
            tuesdayRects[16] = TueRect16;
            tuesdayRects[17] = TueRect17;
            tuesdayRects[18] = TueRect18;
            tuesdayRects[19] = TueRect19;
            tuesdayRects[20] = TueRect20;
            tuesdayRects[21] = TueRect21;
            tuesdayRects[22] = TueRect22;
            tuesdayRects[23] = TueRect23;

            wednesdayRects = new Rectangle[24];
            wednesdayRects[0] = WedRect0;
            wednesdayRects[1] = WedRect1;
            wednesdayRects[2] = WedRect2;
            wednesdayRects[3] = WedRect3;
            wednesdayRects[4] = WedRect4;
            wednesdayRects[5] = WedRect5;
            wednesdayRects[6] = WedRect6;
            wednesdayRects[7] = WedRect7;
            wednesdayRects[8] = WedRect8;
            wednesdayRects[9] = WedRect9;
            wednesdayRects[10] = WedRect10;
            wednesdayRects[11] = WedRect11;
            wednesdayRects[12] = WedRect12;
            wednesdayRects[13] = WedRect13;
            wednesdayRects[14] = WedRect14;
            wednesdayRects[15] = WedRect15;
            wednesdayRects[16] = WedRect16;
            wednesdayRects[17] = WedRect17;
            wednesdayRects[18] = WedRect18;
            wednesdayRects[19] = WedRect19;
            wednesdayRects[20] = WedRect20;
            wednesdayRects[21] = WedRect21;
            wednesdayRects[22] = WedRect22;
            wednesdayRects[23] = WedRect23;

            thursdayRects = new Rectangle[24];
            thursdayRects[0] = ThuRect0;
            thursdayRects[1] = ThuRect1;
            thursdayRects[2] = ThuRect2;
            thursdayRects[3] = ThuRect3;
            thursdayRects[4] = ThuRect4;
            thursdayRects[5] = ThuRect5;
            thursdayRects[6] = ThuRect6;
            thursdayRects[7] = ThuRect7;
            thursdayRects[8] = ThuRect8;
            thursdayRects[9] = ThuRect9;
            thursdayRects[10] = ThuRect10;
            thursdayRects[11] = ThuRect11;
            thursdayRects[12] = ThuRect12;
            thursdayRects[13] = ThuRect13;
            thursdayRects[14] = ThuRect14;
            thursdayRects[15] = ThuRect15;
            thursdayRects[16] = ThuRect16;
            thursdayRects[17] = ThuRect17;
            thursdayRects[18] = ThuRect18;
            thursdayRects[19] = ThuRect19;
            thursdayRects[20] = ThuRect20;
            thursdayRects[21] = ThuRect21;
            thursdayRects[22] = ThuRect22;
            thursdayRects[23] = ThuRect23;

            fridayRects = new Rectangle[24];
            fridayRects[0] = FriRect0;
            fridayRects[1] = FriRect1;
            fridayRects[2] = FriRect2;
            fridayRects[3] = FriRect3;
            fridayRects[4] = FriRect4;
            fridayRects[5] = FriRect5;
            fridayRects[6] = FriRect6;
            fridayRects[7] = FriRect7;
            fridayRects[8] = FriRect8;
            fridayRects[9] = FriRect9;
            fridayRects[10] = FriRect10;
            fridayRects[11] = FriRect11;
            fridayRects[12] = FriRect12;
            fridayRects[13] = FriRect13;
            fridayRects[14] = FriRect14;
            fridayRects[15] = FriRect15;
            fridayRects[16] = FriRect16;
            fridayRects[17] = FriRect17;
            fridayRects[18] = FriRect18;
            fridayRects[19] = FriRect19;
            fridayRects[20] = FriRect20;
            fridayRects[21] = FriRect21;
            fridayRects[22] = FriRect22;
            fridayRects[23] = FriRect23;

            saturdayRects = new Rectangle[24];
            saturdayRects[0] = SatRect0;
            saturdayRects[1] = SatRect1;
            saturdayRects[2] = SatRect2;
            saturdayRects[3] = SatRect3;
            saturdayRects[4] = SatRect4;
            saturdayRects[5] = SatRect5;
            saturdayRects[6] = SatRect6;
            saturdayRects[7] = SatRect7;
            saturdayRects[8] = SatRect8;
            saturdayRects[9] = SatRect9;
            saturdayRects[10] = SatRect10;
            saturdayRects[11] = SatRect11;
            saturdayRects[12] = SatRect12;
            saturdayRects[13] = SatRect13;
            saturdayRects[14] = SatRect14;
            saturdayRects[15] = SatRect15;
            saturdayRects[16] = SatRect16;
            saturdayRects[17] = SatRect17;
            saturdayRects[18] = SatRect18;
            saturdayRects[19] = SatRect19;
            saturdayRects[20] = SatRect20;
            saturdayRects[21] = SatRect21;
            saturdayRects[22] = SatRect22;
            saturdayRects[23] = SatRect23;

            sundayRects = new Rectangle[24];
            sundayRects[0] = SunRect0;
            sundayRects[1] = SunRect1;
            sundayRects[2] = SunRect2;
            sundayRects[3] = SunRect3;
            sundayRects[4] = SunRect4;
            sundayRects[5] = SunRect5;
            sundayRects[6] = SunRect6;
            sundayRects[7] = SunRect7;
            sundayRects[8] = SunRect8;
            sundayRects[9] = SunRect9;
            sundayRects[10] = SunRect10;
            sundayRects[11] = SunRect11;
            sundayRects[12] = SunRect12;
            sundayRects[13] = SunRect13;
            sundayRects[14] = SunRect14;
            sundayRects[15] = SunRect15;
            sundayRects[16] = SunRect16;
            sundayRects[17] = SunRect17;
            sundayRects[18] = SunRect18;
            sundayRects[19] = SunRect19;
            sundayRects[20] = SunRect20;
            sundayRects[21] = SunRect21;
            sundayRects[22] = SunRect22;
            sundayRects[23] = SunRect23;
        }
    }
}
