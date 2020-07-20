using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using ClassicCraft;

namespace ClassicCraftGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow main;

        private static UIElement Copy(UIElement toCopy)
        {
            return XamlReader.Parse(XamlWriter.Save(toCopy)) as UIElement;
        }

        public MainWindow()
        {
            main = this;
            InitializeComponent();
        }

        public void ConsoleTextSet(string s)
        {
            Dispatcher.Invoke(new System.Action(() => {
                Console.Text = s;
            }));
        }

        public void ConsoleTextAdd(string s, bool newLine = true)
        {
            Dispatcher.Invoke(new System.Action(() => {
                if (newLine && Console.Text != "") Console.Text += "\n";
                Console.Text += s;
            }));
        }

        public void SetProgress(double pct)
        {
            Dispatcher.Invoke(new System.Action(() => {
                ProgressPercent.Text = String.Format("{0:N2}%", pct);
                ProgressBar.Value = pct;
            }));
        }

        public void SetProgressText(string str)
        {
            Dispatcher.Invoke(new System.Action(() => {
                ProgressText.Text = str;
            }));
        }

        private void Run_Click(object sender, RoutedEventArgs e)
        {
            if(!Program.Running)
            {
                Tabs.SelectedIndex = 7;
                Task.Factory.StartNew(() => Program.Run(this, @".\..\..\"));
            }
        }

        private void TargetError_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(NbSim != null)
            {
                NbSim.IsEnabled = (string)((ComboBoxItem)((ComboBox)sender).SelectedValue).Content == "Fixed number";
            }
        }

        private void LogFight_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(TargetError != null && NbSim != null)
            {
                if ((string)((ComboBoxItem)((ComboBox)sender).SelectedValue).Content == "Enabled")
                {
                    TargetError.IsEnabled = false;
                    NbSim.IsEnabled = true;
                    NbSim.SelectedIndex = 0;
                    NbSim.Items.RemoveAt(2);
                    NbSim.Items.RemoveAt(2);
                    NbSim.Items.RemoveAt(2);
                }
                else
                {
                    TargetError.IsEnabled = true;
                    NbSim.IsEnabled = (string)((ComboBoxItem)TargetError.SelectedValue).Content == "Fixed number";
                    NbSim.Items.Add("100");
                    NbSim.Items.Add("1000");
                    NbSim.Items.Add("10000");
                }
            }
        }

        private void BossLifeMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BossLowLifeTime != null)
            {
                BossLowLifeTime.IsEnabled = (string)((ComboBoxItem)((ComboBox)sender).SelectedValue).Content == "Custom";
            }
        }

        private static readonly Regex REG_NUMBER = new Regex("[^0-9.]+");

        private void NumberCheck(object sender, TextCompositionEventArgs e)
        {
            e.Handled = REG_NUMBER.IsMatch(e.Text);
        }

        private static readonly Regex REG_WHOLENUMBER = new Regex("[^0-9]+");

        private void WholeNumberCheck(object sender, TextCompositionEventArgs e)
        {
            e.Handled = REG_WHOLENUMBER.IsMatch(e.Text);
        }

        private void Class_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void PlayerTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*
                if(AddPlayer != null && AddPlayer.IsSelected)
                {
                    string s = XamlWriter.Save(SampleTab);

                    StringReader stringReader = new StringReader(s);

                    XmlReader xmlReader = XmlTextReader.Create(stringReader, new XmlReaderSettings());
                    XmlReaderSettings sx = new XmlReaderSettings();

                    object x = XamlReader.Load(xmlReader);

                    PlayerTabs.Items.Insert(PlayerTabs.Items.Count-1, x);

                    ((TabItem)x).Header = "Player" + (PlayerTabs.Items.Count - 1);

                    PlayerTabs.SelectedIndex = PlayerTabs.Items.Count - 2;
                }
            */
        }

        private void Tanking_Changed(object sender, RoutedEventArgs e)
        {
            if (TankingHitEvery != null && TankingRage != null)
            {
                bool enable = ((CheckBox)sender).IsChecked == true;
                TankingHitEvery.IsEnabled = enable;
                TankingRage.IsEnabled = enable;
            }
        }

        private void CheckBoxDebuffsMagical_Changed(object sender, RoutedEventArgs e)
        {
            if(MagicalDebuffs != null)
            {
                foreach(UIElement uie in MagicalDebuffs.Children)
                {
                    ((CheckBox)uie).IsChecked = CheckBoxDebuffsMagical.IsChecked;
                }
            }
        }

        private void MagicalDebuff_Changed(object sender, RoutedEventArgs e)
        {
            if(CheckBoxDebuffsMagical != null)
            {
                if(CheckBoxDebuffsMagical.IsChecked != ((CheckBox)sender).IsChecked)
                {
                    CheckBoxDebuffsMagical.IsChecked = null;
                }
            }
        }

        private void CheckBoxDebuffsPhysical_Changed(object sender, RoutedEventArgs e)
        {
            if (PhysicalDebuffs != null)
            {
                foreach (UIElement uie in PhysicalDebuffs.Children)
                {
                    ((CheckBox)uie).IsChecked = CheckBoxDebuffsPhysical.IsChecked;
                }
            }
        }

        private void PhysicalDebuff_Changed(object sender, RoutedEventArgs e)
        {
            CheckBox s = (CheckBox)sender;

            if (CheckBoxDebuffsPhysical != null)
            {
                if (CheckBoxDebuffsPhysical.IsChecked != s.IsChecked)
                {
                    CheckBoxDebuffsPhysical.IsChecked = null;
                }

                SunderArmor.IsEnabled = ExposeArmor.IsChecked == false;
                if(ExposeArmor.IsChecked == true)
                {
                    SunderArmor.IsChecked = false;
                }
            }
        }

        private void CheckBoxWorldBuffs_Changed(object sender, RoutedEventArgs e)
        {

        }

        private void WorldBuff_Changed(object sender, RoutedEventArgs e)
        {

        }

        private void CheckBoxRaidBuffs_Changed(object sender, RoutedEventArgs e)
        {

        }

        private void RaidBuff_Changed(object sender, RoutedEventArgs e)
        {

        }

        private void CheckBoxConsumables_Changed(object sender, RoutedEventArgs e)
        {

        }

        private void Consumable_Changed(object sender, RoutedEventArgs e)
        {

        }

        private void ItemType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(ItemType != null && DamageMin != null)
            {
                bool enable = ItemType.SelectedIndex == 0 || ItemType.SelectedIndex == 1 || ItemType.SelectedIndex == 2;
                WeaponType.SelectedIndex = enable ? 0 : -1;
                WeaponType.IsEnabled = enable;
                DamageMin.IsEnabled = enable;
                DamageMax.IsEnabled = enable;
                Speed.IsEnabled = enable;
            }
        }

        private void StatsAdd_Click(object sender, RoutedEventArgs e)
        {
            Grid copy = Copy(StackPanelStats.Children[StackPanelStats.Children.Count - 1]) as Grid;
            (copy.Children[0] as ComboBox).SelectedIndex = -1;
            (copy.Children[1] as TextBox).Text = "";
            StackPanelStats.Children.Add(copy);
        }
    }
}
