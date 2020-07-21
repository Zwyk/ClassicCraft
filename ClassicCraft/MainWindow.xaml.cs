using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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

        public JsonUtil.JsonSim sim;
        public JsonUtil.JsonPlayer player;

        public MainWindow()
        {
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            InitializeComponent();
            main = this;

            Program.LoadJsons();
            sim = Program.jsonSim;
            player = Program.jsonPlayer;

            LoadSimConfig(sim);
            LoadPlayer();
        }

        private void LoadSimConfig(JsonUtil.JsonSim json)
        {
            LogFight.SelectedIndex = sim.LogFight ? 1 : 0;
            if (json.TargetError) TargetError.Text = json.TargetErrorPct.ToString();
            else TargetError.SelectedIndex = 0;
            NbSim.Text = json.NbSim.ToString();
            FightLength.Text = json.FightLength.ToString();
            FightLengthVariation.Text = json.FightLengthMod.ToString();
            BossLevel.Text = json.Boss.Level.ToString();
            BossLifeMode.SelectedIndex = json.BossAutoLife ? 0 : 1;
            BossLowLifeTime.Text = json.BossLowLifeTime.ToString();
            BossArmor.Text = json.Boss.Armor.ToString();
            if(json.Boss.SchoolResists.ContainsKey("All")) BossAllResist.Text = json.Boss.SchoolResists["All"].ToString();
            else BossAllResist.SelectedIndex = 0;
            Tanking.IsChecked = json.Tanking;
            TankingHitEvery.Text = json.TankHitEvery.ToString();
            TankingRage.Text = json.TankHitRage.ToString();
            UnlimitedMana.IsChecked = json.UnlimitedMana;
            UnlimitedResource.IsChecked = json.UnlimitedResource;
        }

        private void SaveSimConfig(object sender, RoutedEventArgs e)
        {
            sim.LogFight = LogFight.SelectedIndex == 1;
            sim.TargetError = TargetError.SelectedIndex == 0;
            sim.TargetErrorPct = TargetError.SelectedIndex == 0 ? 0.2 : double.Parse(TargetError.Text);
            sim.NbSim = int.Parse(NbSim.Text);
            sim.FightLength = double.Parse(FightLength.Text);
            sim.FightLengthMod = double.Parse(FightLengthVariation.Text);
            sim.Boss.Level = int.Parse(BossLevel.Text);
            sim.BossAutoLife = BossLifeMode.SelectedIndex == 0;
            sim.BossLowLifeTime = double.Parse(BossLowLifeTime.Text);
            sim.Boss.Armor = int.Parse(BossArmor.Text);
            sim.Boss.SchoolResists = new Dictionary<string, int>();
            if(BossAllResist.SelectedIndex != 0)
            {
                sim.Boss.SchoolResists.Add("All", int.Parse(BossAllResist.Text));
            }
            else
            {
                sim.Boss.SchoolResists.Add("Arcane", int.Parse(BossArcaneResist.Text));
                sim.Boss.SchoolResists.Add("Fire", int.Parse(BossFireResist.Text));
                sim.Boss.SchoolResists.Add("Frost", int.Parse(BossFrostResist.Text));
                sim.Boss.SchoolResists.Add("Light", int.Parse(BossLightResist.Text));
                sim.Boss.SchoolResists.Add("Nature", int.Parse(BossNatureResist.Text));
                sim.Boss.SchoolResists.Add("Shadow", int.Parse(BossShadowResist.Text));
            }
            sim.Tanking = Tanking.IsChecked == true;
            sim.TankHitEvery = double.Parse(TankingHitEvery.Text);
            sim.TankHitRage = double.Parse(TankingRage.Text);
            sim.UnlimitedMana = UnlimitedMana.IsChecked == true;
            sim.UnlimitedResource = UnlimitedResource.IsChecked == true;
            
            Program.SaveJsons();
        }

        private void LoadPlayer()
        {

        }

        private static UIElement Copy(UIElement toCopy)
        {
            return XamlReader.Parse(XamlWriter.Save(toCopy)) as UIElement;
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

        public void Run_On()
        {
            Dispatcher.Invoke(new System.Action(() => {
                RunButton.IsEnabled = true;
            }));
        }

        public void Run_Off()
        {
            Dispatcher.Invoke(new System.Action(() => {
                RunButton.IsEnabled = false;
            }));
        }

        private void CheckAllChildrenCheckboxes(Panel panel, bool check = true)
        {
            if (panel != null)
            {
                foreach (UIElement uie in GetAllChildren(panel))
                {
                    if (uie is CheckBox) ((CheckBox)uie).IsChecked = check;
                }
            }
        }

        private void CheckParentCheckbox(CheckBox parent, bool? check)
        {
            if (parent != null)
            {
                if (parent.IsChecked != check)
                {
                    parent.IsChecked = null;
                }
            }
        }

        private List<UIElement> GetAllChildren(UIElement elem)
        {
            List<UIElement> res = new List<UIElement>();

            if (elem is Panel)
            {
                foreach (UIElement child in ((Panel)elem).Children)
                {
                    res.AddRange(GetAllChildren(child));
                }
            }
            else if (elem is Border)
            {
                res.AddRange(GetAllChildren(((Border)elem).Child));
            }
            else
            {
                res.Add(elem);
            }

            return res;
        }

        private void CheckAllCheckboxes(List<UIElement> list, bool check = true)
        {
            if (list != null)
            {
                foreach (UIElement uie in list)
                {
                    if (uie is CheckBox) ((CheckBox)uie).IsChecked = check;
                }
            }
        }

        private void Run_Click(object sender, RoutedEventArgs e)
        {
            if(!Program.Running)
            {
                Run_Off();
                Tabs.SelectedIndex = 7;
                Task.Factory.StartNew(() => Program.Run(this, @".\..\..\"));
            }
        }

        private void TargetError_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(NbSim != null)
            {
                NbSim.IsEnabled = TargetError.SelectedIndex == 0;
            }
        }

        private void LogFight_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(TargetError != null && NbSim != null)
            {
                TargetError.SelectedIndex = 0;
                TargetError.IsEnabled = LogFight.SelectedIndex == 0;
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

        private void CheckMeOnly(object sender, List<CheckBox> all)
        {
            if (sender != null && all.Contains(sender))
            {
                CheckBox cb = sender as CheckBox;
                if (cb.IsChecked == true)
                {
                    foreach (CheckBox c in all)
                    {
                        if (c != cb)
                        {
                            c.IsChecked = false;
                        }
                    }
                }
            }
        }

        private void CheckBoxDebuffsMagical_Changed(object sender, RoutedEventArgs e)
        {
            CheckAllChildrenCheckboxes(MagicalDebuffs, CheckBoxDebuffsMagical.IsChecked == true);
        }

        private void MagicalDebuff_Changed(object sender, RoutedEventArgs e)
        {
            if(sender != null)
            {
                CheckParentCheckbox(CheckBoxDebuffsMagical, ((CheckBox)sender).IsChecked);
            }
        }

        private void CheckBoxDebuffsPhysical_Changed(object sender, RoutedEventArgs e)
        {
            CheckAllChildrenCheckboxes(PhysicalDebuffs, CheckBoxDebuffsPhysical.IsChecked == true);
        }

        private void PhysicalDebuff_Changed(object sender, RoutedEventArgs e)
        {
            if (sender != null)
            {
                CheckMeOnly(sender, new List<CheckBox> { SunderArmor, ExposeArmor });

                CheckParentCheckbox(CheckBoxDebuffsPhysical, ((CheckBox)sender).IsChecked);
            }
        }

        private void CheckBoxWorldBuffs_Changed(object sender, RoutedEventArgs e)
        {
            CheckAllChildrenCheckboxes(WorldBuffs, CheckBoxWorldBuffs.IsChecked == true);
        }

        private void WorldBuff_Changed(object sender, RoutedEventArgs e)
        {
            if (sender != null)
            {
                CheckMeOnly(sender, new List<CheckBox> { DMFDmg, DMFStr, DMFAgi, DMFInt, DMFSpi });

                CheckParentCheckbox(CheckBoxWorldBuffs, ((CheckBox)sender).IsChecked);
            }
        }

        private void CheckBoxRaidBuffs_Changed(object sender, RoutedEventArgs e)
        {
            CheckAllChildrenCheckboxes(RaidBuffs, CheckBoxRaidBuffs.IsChecked == true);
        }

        private void RaidBuff_Changed(object sender, RoutedEventArgs e)
        {
            if (sender != null)
            {
                CheckParentCheckbox(CheckBoxRaidBuffs, ((CheckBox)sender).IsChecked);
            }
        }

        private void CheckBoxConsumables_Changed(object sender, RoutedEventArgs e)
        {
            CheckAllChildrenCheckboxes(Consumables, CheckBoxConsumables.IsChecked == true);
        }

        private void Consumable_Changed(object sender, RoutedEventArgs e)
        {
            if (sender != null)
            {
                CheckMeOnly(sender, new List<CheckBox> { BL1, BL2, BL3 });
                CheckMeOnly(sender, new List<CheckBox> { Agi1, Agi2 });
                CheckMeOnly(sender, new List<CheckBox> { Str1, Str2 });
                CheckMeOnly(sender, new List<CheckBox> { AP1, AP2 });
                CheckMeOnly(sender, new List<CheckBox> { Food1, Food2, Food3 });

                CheckParentCheckbox(CheckBoxConsumables, ((CheckBox)sender).IsChecked);
            }
        }

        private void ItemType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(ItemType != null && DamageMin != null)
            {
                bool enable = ItemType.SelectedIndex < 3;
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

        private void AllResist_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (BossAllResist != null && BossArcaneResist != null && double.TryParse(BossAllResist.Text, out _))
            {
                BossArcaneResist.Text = BossAllResist.Text;
                BossFireResist.Text = BossAllResist.Text;
                BossFrostResist.Text = BossAllResist.Text;
                BossNatureResist.Text = BossAllResist.Text;
                BossLightResist.Text = BossAllResist.Text;
                BossShadowResist.Text = BossAllResist.Text;
            }
        }

        private void BossResist_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(sender != null && BossAllResist != null)
            {
                BossAllResist.SelectedIndex = 0;
            }
        }
    }
}
