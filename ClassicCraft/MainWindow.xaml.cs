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
using Newtonsoft.Json;

namespace ClassicCraftGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string CONFIG_FOLDER = "Config";

        public static string PLAYER_CONFIG_FOLDER = "Player";
        public static string SIM_CONFIG_FOLDER = "Simulation";

        public static string ITEMS_FOLDER = "Items";
        public static string ARMOR_ITEMS_FOLDER = "Armor";
        public static string WEAPON_ITEMS_FOLDER = "Weapon";
        public static string ENCHANTS_ITEMS_FOLDER = "Enchants";

        public List<JsonUtil.JsonItem> jsonItems = new List<JsonUtil.JsonItem>();
        public List<string> itemsFileNames = new List<string>();
        public int selectedItemIndex = 0;

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

            LoadItems();

            Program.LoadConfigJsons();
            sim = Program.jsonSim;
            player = Program.jsonPlayer;

            LoadSimConfig(sim);
            LoadPlayer(player);
        }

        private void Run(object sender, RoutedEventArgs e)
        {
            if (!Program.Running)
            {
                SavePlayer();
                SaveSimConfig();

                Run_Disable();
                Tabs.SelectedIndex = 4;

                Task.Factory.StartNew(() => Program.Run(this));
            }
        }

        #region Events

        private void TargetError_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NbSim != null)
            {
                NbSim.IsEnabled = TargetError.SelectedIndex == 0;
            }
        }

        private void LogFight_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TargetError != null && NbSim != null)
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
            CheckAllChildrenCheckboxes(MagicalDebuffs, CheckBoxDebuffsMagical.IsChecked == true);
        }

        private void MagicalDebuff_Changed(object sender, RoutedEventArgs e)
        {
            if (sender != null)
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
            if (ItemType != null && DamageMin != null)
            {
                bool enable = ItemType.SelectedIndex < 3;
                WeaponType.SelectedIndex = enable ? 0 : -1;
                WeaponType.IsEnabled = enable;
                DamageMin.IsEnabled = enable;
                DamageMax.IsEnabled = enable;
                Speed.IsEnabled = enable;

                if (!enable)
                {
                    DamageMin.Text = "";
                    DamageMax.Text = "";
                    Speed.Text = "";
                }
            }
        }

        private void StatsAdd_Click(object sender, RoutedEventArgs e)
        {
            AddStat();
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
            if (sender != null && BossAllResist != null)
            {
                BossAllResist.SelectedIndex = 0;
            }
        }

        private void ItemsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ItemName.Text = ItemsList.Text;

            if (ItemsList.SelectedIndex != -1)
            {
                selectedItemIndex = ItemsList.SelectedIndex;

                JsonUtil.JsonItem item = CurrentItem();

                if (item is JsonUtil.JsonWeapon)
                {
                    JsonUtil.JsonWeapon weapon = item as JsonUtil.JsonWeapon;

                    ItemType.SelectedIndex = IsRangedWeapon(weapon) ? 2 : (weapon.TwoHanded ? 1 : 0);

                    WeaponType.SelectedIndex = GetComboBoxIndexWithString(WeaponType, weapon.Type);

                    DamageMin.Text = weapon.DamageMin.ToString();
                    DamageMax.Text = weapon.DamageMax.ToString();
                    Speed.Text = weapon.Speed.ToString();
                }
                else
                {
                    ItemType.SelectedIndex = GetComboBoxIndexWithString(ItemType, item.Slot);

                    DamageMin.Text = "";
                    DamageMax.Text = "";
                    Speed.Text = "";
                }

                ResetStats();
                for (int i = 1; i < item.Stats.Count; i++)
                {
                    AddStat();
                }

                int j = 0;
                foreach (string s in item.Stats.Keys)
                {
                    Grid g = StackPanelStats.Children[j] as Grid;
                    ComboBox stat = g.Children[0] as ComboBox;
                    TextBox val = g.Children[1] as TextBox;

                    for (int i = 0; i < stat.Items.Count; i++)
                    {
                        string cbs = (stat.Items[i] as ComboBoxItem).Content.ToString();
                        if (statsStringsToAttribute[cbs] == s)
                        {
                            stat.SelectedIndex = i;
                            break;
                        }
                    }

                    val.Text = item.Stats[s].ToString();
                    j++;
                }
            }

        }

        private void ItemsList_TextChanged(object sender, TextChangedEventArgs e)
        {
            ItemName.Text = ItemsList.Text;
        }

        private void ItemName_TextChanged(object sender, TextChangedEventArgs e)
        {
            ItemsList.Text = ItemName.Text;
        }

        #endregion

        #region Loading & Saving

        private void LoadItems()
        {
            try
            {
                int index = 0;

                foreach (string s in Directory.GetFiles(System.IO.Path.Combine(Program.basePath(), ITEMS_FOLDER, WEAPON_ITEMS_FOLDER), "*.json"))
                {
                    JsonUtil.JsonWeapon item = JsonConvert.DeserializeObject<JsonUtil.JsonWeapon>(File.ReadAllText(s));
                    jsonItems.Add(item);
                    itemsFileNames.Add(System.IO.Path.GetFileNameWithoutExtension(s));
                    ItemsList.Items.Add(item.Name);
                    index++;
                }

                foreach (string s in Directory.GetFiles(System.IO.Path.Combine(Program.basePath(), ITEMS_FOLDER, ARMOR_ITEMS_FOLDER), "*.json"))
                {
                    JsonUtil.JsonItem item = JsonConvert.DeserializeObject<JsonUtil.JsonItem>(File.ReadAllText(s));
                    jsonItems.Add( item);
                    itemsFileNames.Add(System.IO.Path.GetFileNameWithoutExtension(s));
                    ItemsList.Items.Add(item.Name);
                    index++;
                }
            }
            catch (Exception e)
            {
                Program.Debug("Error while loading items : " + e.ToString());
            }
        }

        private void SaveItemClick(object sender, RoutedEventArgs e)
        {
            SaveItem();
        }

        private void SaveItem()
        {
            JsonUtil.JsonItem item = UpdateCurrentItem();

            string path;
            if (itemsFileNames[selectedItemIndex] != null)
            {
                path = System.IO.Path.Combine(Program.basePath(), ITEMS_FOLDER, item.Slot.Equals("Weapon") ? WEAPON_ITEMS_FOLDER : ARMOR_ITEMS_FOLDER, itemsFileNames[selectedItemIndex] + ".json");

                if (ItemsList.Items[selectedItemIndex].ToString() != ItemName.Text)
                {
                    File.Delete(path);

                    itemsFileNames[selectedItemIndex] = UnusedFileNameFromString(ItemName.Text);
                    path = System.IO.Path.Combine(Program.basePath(), ITEMS_FOLDER, item.Slot.Equals("Weapon") ? WEAPON_ITEMS_FOLDER : ARMOR_ITEMS_FOLDER, itemsFileNames[selectedItemIndex] + ".json");
                }
            }
            else
            {
                itemsFileNames[selectedItemIndex] = UnusedFileNameFromString(ItemName.Text);
                path = System.IO.Path.Combine(Program.basePath(), ITEMS_FOLDER, item.Slot.Equals("Weapon") ? WEAPON_ITEMS_FOLDER : ARMOR_ITEMS_FOLDER, itemsFileNames[selectedItemIndex] + ".json");
            }

            File.WriteAllText(path, JsonConvert.SerializeObject(CurrentItem(), Newtonsoft.Json.Formatting.Indented));

            ItemsList.Items[selectedItemIndex] = ItemName.Text;
            ItemsList.SelectedIndex = selectedItemIndex;
        }

        private string UnusedFileNameFromString(string s)
        {
            string res = SafeFileName(s);

            if (itemsFileNames.Contains(res))
            {
                int i = 1;
                string currentName;
                do
                {
                    currentName = res + " (" + i + ")";
                    i++;
                }
                while (itemsFileNames.Contains(res));

                res = currentName;
            }

            return res;
        }

        private JsonUtil.JsonItem UpdateCurrentItem()
        {
            JsonUtil.JsonItem item = CurrentItem();
            item.Name = ItemName.Text;

            item.Stats = new Dictionary<string, double>();
            foreach (Grid g in StackPanelStats.Children)
            {
                ComboBox stat = g.Children[0] as ComboBox;
                TextBox val = g.Children[1] as TextBox;

                if (stat.SelectedIndex >= 0 && val.Text != "")
                {
                    foreach (string k in statsStringsToAttribute.Keys)
                    {
                        if (k == stat.Text)
                        {
                            item.Stats[statsStringsToAttribute[k]] = double.Parse(val.Text);
                        }
                    }
                }
            }

            if (ItemIsWeapon())
            {
                item.Slot = "Weapon";

                JsonUtil.JsonWeapon weapon = item as JsonUtil.JsonWeapon;
                weapon.TwoHanded = ItemType.SelectedIndex == 1;
                weapon.Type = WeaponType.Text;
                weapon.DamageMin = int.Parse(DamageMin.Text);
                weapon.DamageMax = int.Parse(DamageMax.Text);
                weapon.Speed = double.Parse(Speed.Text);

                jsonItems[selectedItemIndex] = weapon;
            }
            else
            {
                item.Slot = ItemType.Text;
            }

            return item;
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
            if (json.Boss.SchoolResists.ContainsKey("All")) BossAllResist.Text = json.Boss.SchoolResists["All"].ToString();
            else BossAllResist.SelectedIndex = 0;
            Tanking.IsChecked = json.Tanking;
            TankingHitEvery.Text = json.TankHitEvery.ToString();
            TankingRage.Text = json.TankHitRage.ToString();
            UnlimitedMana.IsChecked = json.UnlimitedMana;
            UnlimitedResource.IsChecked = json.UnlimitedResource;
        }

        private void SaveSimConfigClick(object sender, RoutedEventArgs e)
        {
            SaveSimConfig();
        }

        private void SaveSimConfig()
        {
            sim.LogFight = LogFight.SelectedIndex == 1;
            sim.TargetError = TargetError.SelectedIndex != 0;
            sim.TargetErrorPct = TargetError.SelectedIndex == 0 ? 0.2 : double.Parse(TargetError.Text);
            sim.NbSim = int.Parse(NbSim.Text);
            sim.FightLength = double.Parse(FightLength.Text);
            sim.FightLengthMod = double.Parse(FightLengthVariation.Text);
            sim.Boss.Level = int.Parse(BossLevel.Text);
            sim.BossAutoLife = BossLifeMode.SelectedIndex == 0;
            sim.BossLowLifeTime = double.Parse(BossLowLifeTime.Text);
            sim.Boss.Armor = int.Parse(BossArmor.Text);
            sim.Boss.SchoolResists = new Dictionary<string, int>();
            if (BossAllResist.SelectedIndex != 0)
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

        private void LoadPlayer(JsonUtil.JsonPlayer json)
        {

        }

        private void SavePlayerClick(object sender, RoutedEventArgs e)
        {
            SavePlayer();
        }

        private void SavePlayer()
        {
            // TODO

            Program.SaveJsons();
        }

        #endregion

        #region Util

        private void ResetStats()
        {
            StackPanelStats.Children.RemoveRange(1, StackPanelStats.Children.Count - 1);
            Grid firstRow = StackPanelStats.Children[0] as Grid;
            (firstRow.Children[0] as ComboBox).SelectedIndex = -1;
            (firstRow.Children[1] as TextBox).Text = "";
        }

        private Grid AddStat()
        {
            Grid copy = Copy(StackPanelStats.Children[StackPanelStats.Children.Count - 1]) as Grid;
            (copy.Children[0] as ComboBox).SelectedIndex = -1;
            (copy.Children[1] as TextBox).Text = "";
            StackPanelStats.Children.Add(copy);
            return copy;
        }

        private static UIElement Copy(UIElement toCopy)
        {
            return XamlReader.Parse(XamlWriter.Save(toCopy)) as UIElement;
        }

        private JsonUtil.JsonItem CurrentItem()
        {
            return jsonItems[selectedItemIndex];
        }

        private bool ItemIsWeapon()
        {
            return ItemIsWeapon(CurrentItem());
        }

        private bool ItemIsWeapon(int index)
        {
            return ItemIsWeapon(jsonItems[index]);
        }

        private bool ItemIsWeapon(JsonUtil.JsonItem item)
        {
            return item.Slot.Equals("Weapon");
        }

        private string SafeFileName(string s)
        {
            string res = s;

            foreach (var c in System.IO.Path.GetInvalidFileNameChars())
            {
                res = res.Replace(c, '-');
            }

            return s;
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

        private void NumberCheck(object sender, TextCompositionEventArgs e)
        {
            string s = "";
            if (sender is ComboBox)
            {
                s = (sender as ComboBox).Text;
            }
            else if (sender is TextBox)
            {
                s = (sender as TextBox).Text;
            }
            bool containsDot = s.Contains('.');
            e.Handled = !((!containsDot && e.Text == ".") || double.TryParse(e.Text, out _));
        }

        private void WholeNumberCheck(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _);
        }

        private static int GetComboBoxIndexWithString(ComboBox cb, string s)
        {
            for (int i = 0; i < cb.Items.Count; i++)
            {
                string cbs = (cb.Items[i] as ComboBoxItem).Content.ToString();
                if (cbs == s)
                {
                    return i;
                }
            }

            return -1;
        }

        #endregion

        #region ClassicCraft Util

        private bool IsRangedWeapon(JsonUtil.JsonWeapon weapon)
        {
            return weapon.Slot == "Bow" || weapon.Slot == "Crossbow" || weapon.Slot == "Gun" || weapon.Slot == "Wand";
        }

        private static Dictionary<string, string> statsStringsToAttribute = new Dictionary<string, string>()
        {
            { "Strength", "Str" },
            { "Agility", "Agi" },
            { "Intellect", "Int" },
            { "Spirit", "Spi" },
            { "Stamina", "Sta" },
            { "Attack Power", "AP" },
            { "Spell Power", "SP" },
            { "Healing Power", "HSP" },
            { "Hit Chance", "Hit" },
            { "Spell Hit Chance", "SHit" },
            { "Critical Chance", "Crit" },
            { "Spell Critical Chance", "SCrit" },
            { "MP5", "MP5" },
            { "Attack Speed", "Haste" },
            { "Armor Penetration", "ArmorPen" },
            { "Weapon Damage", "WDmg" },
            { "Skill Axe", "Axe" },
            { "Skill Dagger", "Dagger" },
            { "Skill Fist", "Fist" },
            { "Skill Mace", "Mace" },
            { "Skill Polearm", "Polearm" },
            { "Skill Staff", "Staff" },
            { "Skill Sword", "Sword" },
            { "Skill Bow", "Bow" },
            { "Skill Crossbow", "Crossbow" },
            { "Skill Gun", "Gun" },
        };

        #endregion

        #region GUI remote access

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

        public void Run_Enable()
        {
            Dispatcher.Invoke(new System.Action(() => {
                RunButton.IsEnabled = true;
            }));
        }

        public void Run_Disable()
        {
            Dispatcher.Invoke(new System.Action(() => {
                RunButton.IsEnabled = false;
            }));
        }

        #endregion

        private bool CollectionContainsString(ItemCollection col, string s)
        {
            foreach (object o in col)
            {
                if (o.ToString() == s)
                {
                    return true;
                }
            }
            return false;
        }

        private void NewItemClick(object sender, RoutedEventArgs e)
        {
            NewItem();
        }

        private void NewItem()
        {
            string name = "New Item";

            if (CollectionContainsString(ItemsList.Items, name))
            {
                int i = 1;
                string currentName;
                do
                {
                    currentName = name + " (" + i + ")";
                    i++;
                }
                while (CollectionContainsString(ItemsList.Items, currentName));

                name = currentName;
            }

            NewItem(name, "Head");
        }

        private void CopyItemClick(object sender, RoutedEventArgs e)
        {
            JsonUtil.JsonItem item = CurrentItem();
            string name = item.Name;

            if (CollectionContainsString(ItemsList.Items, name))
            {
                int i = 1;
                string currentName;
                do
                {
                    currentName = name + " (" + i + ")";
                    i++;
                }
                while (CollectionContainsString(ItemsList.Items, currentName));

                name = currentName;
            }

            if(item is JsonUtil.JsonWeapon)
            {
                JsonUtil.JsonWeapon weapon = item as JsonUtil.JsonWeapon;
                NewWeapon(name, weapon.TwoHanded, weapon.Type, weapon.DamageMin, weapon.DamageMax, weapon.Speed, weapon.Stats, weapon.Enchantment, weapon.Buff);
            }
            else
            {
                NewItem(name, item.Slot, item.Stats, item.Enchantment);
            }
        }

        private void NewItem(string name, string slot, Dictionary<string, double> attributes = null, JsonUtil.JsonEnchantment enchant = null)
        {
            jsonItems.Add(new JsonUtil.JsonItem(0, name, slot, attributes, enchant));
            itemsFileNames.Add(null);

            ItemsList.Items.Add(name);

            ItemsList.SelectedIndex = ItemsList.Items.Count - 1;
        }

        private void NewWeapon(string name, bool twoHanded, string type, double dmgMin, double dmgMax, double speed, Dictionary<string, double> attributes = null, JsonUtil.JsonEnchantment enchant = null, JsonUtil.JsonEnchantment buffs = null)
        {
            jsonItems.Add(new JsonUtil.JsonWeapon(dmgMin, dmgMax, speed, twoHanded, type, 0, name, attributes, enchant, buffs));
            itemsFileNames.Add(null);

            ItemsList.Items.Add(name);

            ItemsList.SelectedIndex = ItemsList.Items.Count - 1;
        }

        private void DeleteItemClick(object sender, RoutedEventArgs e)
        {
            if(itemsFileNames[selectedItemIndex] != null)
            {
                File.Delete(System.IO.Path.Combine(Program.basePath(), ITEMS_FOLDER, CurrentItem().Slot.Equals("Weapon") ? WEAPON_ITEMS_FOLDER : ARMOR_ITEMS_FOLDER, itemsFileNames[selectedItemIndex] + ".json"));
            }

            jsonItems.RemoveAt(selectedItemIndex);
            itemsFileNames.RemoveAt(selectedItemIndex);
            ItemsList.Items.RemoveAt(selectedItemIndex);

            if(ItemsList.Items.Count > 0)
            {
                selectedItemIndex -= selectedItemIndex >= ItemsList.Items.Count ? 1 : 0;
                ItemsList.SelectedIndex = selectedItemIndex;
            }
            else
            {
                NewItem();
            }
        }
    }
}
