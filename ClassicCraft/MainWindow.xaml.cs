using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public static string DB_FOLDER = "DB";
        public static string ARMOR_ITEMS_FOLDER = "Armor";
        public static string WEAPON_ITEMS_FOLDER = "Weapon";
        public static string ENCHANTS_FOLDER = "Enchant";

        public List<JsonUtil.JsonItem> jsonItems = new List<JsonUtil.JsonItem>();
        public Dictionary<JsonUtil.JsonItem, string> itemsFileNames = new Dictionary<JsonUtil.JsonItem, string>();
        public int selectedItemIndex = 0;
        public Dictionary<Player.Slot, List<JsonUtil.JsonItem>> itemsByPlayerSlot = new Dictionary<Player.Slot, List<JsonUtil.JsonItem>>();

        public List<JsonUtil.JsonWeapon> jsonWeapons = new List<JsonUtil.JsonWeapon>();
        public Dictionary<JsonUtil.JsonWeapon, string> weaponsFileNames = new Dictionary<JsonUtil.JsonWeapon, string>();
        public int selectedWeaponIndex = 0;

        public List<JsonUtil.JsonEnchantment> jsonEnchants = new List<JsonUtil.JsonEnchantment>();
        public Dictionary<JsonUtil.JsonEnchantment, string> enchantsFileNames = new Dictionary<JsonUtil.JsonEnchantment, string>();
        public int selectedEnchantIndex = 0;
        public Dictionary<Player.Slot, List<JsonUtil.JsonEnchantment>> enchantsByPlayerSlot = new Dictionary<Player.Slot, List<JsonUtil.JsonEnchantment>>();

        public static MainWindow main;

        public JsonUtil.JsonSim sim;
        public JsonUtil.JsonPlayer player;

        public TextBox CurrentConsole;

        public int nbSim = 0;

        public static bool noDB = true;

        public MainWindow()
        {
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
            
            InitializeComponent();
            main = this;
            
            ConsoleTabControl.Items.Remove(ConsoleEmpty);
            
            if(!noDB)
            {
                PopulateSlotLists();
                LoadDB();
            }

            Program.LoadConfigJsons();
            sim = Program.jsonSim;
            player = Program.jsonPlayer;

            LoadSimConfig();
            LoadPlayer();
            
            if(!noDB)
            {
                SavePlayer();
            }

            DataObject.AddPastingHandler(Talents, Talents_OnPaste);

            ItemsList.SelectedIndex = 0;
            WeaponsList.SelectedIndex = 0;
            EnchantsList.SelectedIndex = 0;
        }

        private void Run(object sender, RoutedEventArgs e)
        {
            if (!Program.Running)
            {
                SavePlayer();
                SaveSimConfig();

                NewConsole();

                Run_Disable();
                Tabs.SelectedIndex = 4;

                Task.Factory.StartNew(() => Program.Run(this));
            }
        }

        private void LoadDB()
        {
            LoadItems();
            LoadWeapons();
            LoadEnchants();
        }

        #region Sim config

        public void LoadSimConfig()
        {
            LogFight.SelectedIndex = sim.LogFight ? 1 : 0;
            if (sim.TargetError) TargetError.Text = sim.TargetErrorPct.ToString();
            else TargetError.SelectedIndex = 0;
            NbSim.Text = sim.NbSim.ToString();
            StatsWeights.SelectedIndex = sim.StatsWeights ? 0 : 1;
            FightLength.Text = sim.FightLength.ToString();
            FightLengthVariation.Text = sim.FightLengthMod.ToString();
            BossLevel.Text = sim.Boss.Level.ToString();
            BossLifeMode.SelectedIndex = sim.BossAutoLife ? 0 : 1;
            BossLowLifeTime.Text = sim.BossLowLifeTime.ToString();
            BossArmor.Text = sim.Boss.Armor.ToString();
            if (sim.Boss.SchoolResists.ContainsKey("All")) BossAllResist.Text = sim.Boss.SchoolResists["All"].ToString();
            else BossAllResist.SelectedIndex = 0;
            Tanking.IsChecked = sim.Tanking;
            TankingHitEvery.Text = sim.TankHitEvery.ToString();
            TankingRage.Text = sim.TankHitRage.ToString();
            UnlimitedMana.IsChecked = sim.UnlimitedMana;
            UnlimitedResource.IsChecked = sim.UnlimitedResource;
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
            sim.StatsWeights = StatsWeights.SelectedIndex == 0;
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

            Program.jsonSim = sim;
            Program.SaveJsons(false, true);
        }

        #endregion

        #region Player

        public void LoadPlayer()
        {
            Race.SelectedIndex = GetComboBoxIndexWithString(Race, player.Race);
            Class.SelectedIndex = GetComboBoxIndexWithString(Class, player.Class);
            Talents.Text = player.Talents;

            if(!noDB)
            {
                foreach (string slot in player.Weapons.Keys)
                {
                    JsonUtil.JsonWeapon weapon = player.Weapons[slot];
                    LoadPlayerWeapon(weapon, slot);
                    PlayerSlotToComboBox(Player.ToSlot(slot)).SelectedValue = weapon.Name;
                    if (weapon.Enchantment != null)
                    {
                        LoadPlayerEnchant(weapon.Enchantment, slot);
                        if (PlayerEnchantSlotToComboBox(Player.ToSlot(slot)) != null) PlayerEnchantSlotToComboBox(Player.ToSlot(slot)).SelectedValue = weapon.Enchantment.Name;
                    }
                }
                foreach (string slot in player.Equipment.Keys)
                {
                    JsonUtil.JsonItem item = player.Equipment[slot];
                    LoadPlayerItem(item, slot);
                    PlayerSlotToComboBox(Player.ToSlot(slot)).SelectedValue = item.Name;
                    if (item.Enchantment != null)
                    {
                        LoadPlayerEnchant(item.Enchantment, slot);
                        if (PlayerEnchantSlotToComboBox(Player.ToSlot(slot)) != null) PlayerEnchantSlotToComboBox(Player.ToSlot(slot)).SelectedValue = item.Enchantment.Name;
                    }
                }
            }
        }

        private void SavePlayerClick(object sender, RoutedEventArgs e)
        {
            SavePlayer();
        }

        private void SavePlayer()
        {
            player.Race = Race.Text;
            player.Class = Class.Text;
            player.Talents = Talents.Text;
            
            Program.jsonPlayer = player;
            Program.SaveJsons(true, false);
        }

        #endregion

        #region Items

        private JsonUtil.JsonItem UpdateCurrentItem()
        {
            JsonUtil.JsonItem item = CurrentItem();
            item.Name = ItemName.Text;

            item.Stats = new Dictionary<string, double>();
            foreach (Grid g in StackPanelStats.Children)
            {
                ComboBox stat = g.Children[1] as ComboBox;
                TextBox val = g.Children[2] as TextBox;

                if (stat.SelectedIndex >= 0 && val.Text != "")
                {
                    foreach (string k in statsStringsToAttribute.Keys)
                    {
                        if (k == stat.Text && double.TryParse(val.Text, out _))
                        {
                            item.Stats[statsStringsToAttribute[k]] = double.Parse(val.Text);
                        }
                    }
                }
            }

            string oldSlot = item.Slot != ItemSlot.Text ? item.Slot : "";

            item.Slot = ItemSlot.Text;

            UpdateJsonItem(item, oldSlot);

            return item;
        }

        private void LoadItems()
        {
            try
            {
                foreach (string s in Directory.GetFiles(System.IO.Path.Combine(Program.basePath(), DB_FOLDER, ARMOR_ITEMS_FOLDER), "*.json"))
                {
                    JsonUtil.JsonItem item = JsonConvert.DeserializeObject<JsonUtil.JsonItem>(File.ReadAllText(s));
                    
                    if (jsonItems.Any(it => it.Id == item.Id))
                    {
                        item.Id = jsonItems.Max(it => it.Id) + 1;

                        string path = System.IO.Path.Combine(Program.basePath(), DB_FOLDER, ARMOR_ITEMS_FOLDER, s);

                        File.WriteAllText(path, JsonConvert.SerializeObject(item, Newtonsoft.Json.Formatting.Indented));
                    }

                    AddJsonItem(item);
                    itemsFileNames[item] = System.IO.Path.GetFileNameWithoutExtension(s);
                    ItemsList.Items.Add(item.Name);
                }
            }
            catch (Exception e)
            {
                Program.Debug("Error while loading items : " + e.ToString());
            }
        }

        private void SaveItemClick(object sender, RoutedEventArgs e)
        {
            if (CurrentItem().Name != ItemName.Text)
            {
                string name = ItemName.Text;
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

                    ItemName.Text = currentName;
                }
            }

            SaveCurrentItem();
        }

        private void SaveCurrentItem()
        {
            JsonUtil.JsonItem item = UpdateCurrentItem();

            string path;
            if (itemsFileNames[item] != null)
            {
                path = System.IO.Path.Combine(Program.basePath(), DB_FOLDER, ARMOR_ITEMS_FOLDER, itemsFileNames[item] + ".json");


                if (ItemsList.Items[selectedItemIndex].ToString() != ItemName.Text)
                {
                    File.Delete(path);

                    itemsFileNames[item] = UnusedFileNameFromString(ItemName.Text);
                    path = System.IO.Path.Combine(Program.basePath(), DB_FOLDER, ARMOR_ITEMS_FOLDER, itemsFileNames[item] + ".json");
                }
            }
            else
            {
                itemsFileNames[item] = UnusedFileNameFromString(ItemName.Text);
                path = System.IO.Path.Combine(Program.basePath(), DB_FOLDER, ARMOR_ITEMS_FOLDER, itemsFileNames[item] + ".json");
            }

            JsonUtil.JsonEnchantment e = item.Enchantment;
            item.Enchantment = null;
            File.WriteAllText(path, JsonConvert.SerializeObject(item, Newtonsoft.Json.Formatting.Indented));
            item.Enchantment = e;

            jsonItems[selectedItemIndex] = item;
            ItemsList.Items[selectedItemIndex] = ItemName.Text;
            ItemsList.SelectedIndex = selectedItemIndex;
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

            NewItem(name, item.Slot, item.Stats, item.Enchantment);
        }

        private void ItemsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ItemsList.SelectedIndex != -1)
            {
                selectedItemIndex = ItemsList.SelectedIndex;

                JsonUtil.JsonItem item = CurrentItem();

                ItemName.Text = item.Name;

                ItemSlot.SelectedIndex = GetComboBoxIndexWithString(ItemSlot, item.Slot);

                if (ItemPanel.Children.Contains(WeaponPanel))
                {
                    ItemPanel.Children.Remove(WeaponPanel);
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
                    ComboBox stat = g.Children[1] as ComboBox;
                    TextBox val = g.Children[2] as TextBox;

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

        private void LoadPlayerItem(JsonUtil.JsonItem item, string slot)
        {
            if (!jsonItems.Contains(item))
            {
                if (jsonItems.Any(e => e.Id == item.Id))
                {
                    List<JsonUtil.JsonItem> wes = jsonItems.Where(w => w.Id == item.Id && w.Name == item.Name && w.Slot == item.Slot && w.Stats.Count == item.Stats.Count
                                                                    && !w.Stats.Except(item.Stats).Any()).ToList();
                    JsonUtil.JsonItem we = wes.Count > 0 ? wes.First() : null;
                    if (we != null)
                    {
                        JsonUtil.JsonEnchantment enc = item.Enchantment;
                        item = we;
                        item.Enchantment = enc;
                        return;
                    }
                    else
                    {
                        item.Id = jsonItems.Max(e => e.Id) + 1;
                    }
                }

                if (CollectionContainsString(ItemsList.Items, item.Name))
                {
                    int i = 1;
                    string currentName;
                    do
                    {
                        currentName = item.Name + " (" + i + ")";
                        i++;
                    }
                    while (CollectionContainsString(ItemsList.Items, currentName));

                    item.Name = currentName;
                }
                
                if (item.Slot == null) item.Slot = StringPlayerSlotToSlot(slot);

                itemsFileNames[item] = UnusedFileNameFromString(item.Name);
                AddJsonItem(item);
                ItemsList.Items.Add(item.Name);

                JsonUtil.JsonEnchantment en = item.Enchantment;
                item.Enchantment = null;
                string path = System.IO.Path.Combine(Program.basePath(), DB_FOLDER, ARMOR_ITEMS_FOLDER, itemsFileNames[item] + ".json");
                File.WriteAllText(path, JsonConvert.SerializeObject(item, Newtonsoft.Json.Formatting.Indented));
                item.Enchantment = en;
            }
        }

        private void NewItem(string name, string slot, Dictionary<string, double> attributes = null, JsonUtil.JsonEnchantment enchant = null)
        {
            JsonUtil.JsonItem item = new JsonUtil.JsonItem(jsonItems.Max(it => it.Id) + 1, name, slot, attributes, enchant);
            AddJsonItem(item);
            itemsFileNames[item] = null;

            ItemsList.Items.Add(name);

            ItemsList.SelectedIndex = ItemsList.Items.Count - 1;
        }

        private void DeleteItemClick(object sender, RoutedEventArgs e)
        {
            JsonUtil.JsonItem item = jsonItems[selectedItemIndex];

            if (itemsFileNames[item] != null)
            {
                File.Delete(System.IO.Path.Combine(Program.basePath(), DB_FOLDER, ARMOR_ITEMS_FOLDER, itemsFileNames[item] + ".json"));
            }

            RemoveJsonItem(jsonItems[selectedItemIndex]);
            itemsFileNames.Remove(item);
            ItemsList.Items.RemoveAt(selectedItemIndex);

            if (ItemsList.Items.Count > 0)
            {
                selectedItemIndex -= selectedItemIndex >= ItemsList.Items.Count ? 1 : 0;
                ItemsList.SelectedIndex = selectedItemIndex;
            }
            else
            {
                NewItem();
            }
        }

        private void AddJsonItem(JsonUtil.JsonItem item)
        {
            jsonItems.Add(item);
            
            foreach (Player.Slot pslot in SlotUtil.ToPlayerSlot(SlotUtil.FromString(item.Slot)))
            {
                itemsByPlayerSlot[pslot].Add(item);
                PlayerSlotToComboBox(pslot).Items.Add(item.Name);
            }
        }

        private void UpdateJsonItem(JsonUtil.JsonItem item, string oldSlot = "")
        {
            if(oldSlot != "")
            {
                foreach (Player.Slot pslot in SlotUtil.ToPlayerSlot(SlotUtil.FromString(oldSlot)))
                {
                    PlayerSlotToComboBox(pslot).Items.RemoveAt(itemsByPlayerSlot[pslot].IndexOf(item));
                    itemsByPlayerSlot[pslot].Remove(item);
                }
                foreach (Player.Slot pslot in SlotUtil.ToPlayerSlot(SlotUtil.FromString(item.Slot)))
                {
                    itemsByPlayerSlot[pslot].Add(item);
                    PlayerSlotToComboBox(pslot).Items.Add(item.Name);
                }
            }
            else
            {
                foreach (Player.Slot pslot in SlotUtil.ToPlayerSlot(SlotUtil.FromString(item.Slot)))
                {
                    PlayerSlotToComboBox(pslot).Items[itemsByPlayerSlot[pslot].IndexOf(item)] = item.Name;
                }
            }
        }

        private void RemoveJsonItem(JsonUtil.JsonItem item)
        {
            jsonItems.Remove(item);

            foreach (Player.Slot pslot in SlotUtil.ToPlayerSlot(SlotUtil.FromString(item.Slot)))
            {
                PlayerSlotToComboBox(pslot).Items.RemoveAt(itemsByPlayerSlot[pslot].IndexOf(item));
                itemsByPlayerSlot[pslot].Remove(item);
            }
        }

        private void ItemSlot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(ItemSlot.SelectedIndex > -1)
            {
                JsonUtil.JsonItem item = CurrentItem();
                string newSlot = (ItemSlot.Items[ItemSlot.SelectedIndex] as ComboBoxItem).Content.ToString();
                
                if (ItemSlot.Text != "" && item.Slot != newSlot)
                {
                    string oldSlot = item.Slot;

                    item.Slot = newSlot;

                    UpdateJsonItem(item, oldSlot);
                }
            }
        }

        #endregion

        #region Weapons

        private JsonUtil.JsonWeapon UpdateCurrentWeapon()
        {
            JsonUtil.JsonWeapon weapon = CurrentWeapon();
            weapon.Name = WeaponName.Text;

            weapon.Stats = new Dictionary<string, double>();
            foreach (Grid g in StackPanelStats.Children)
            {
                ComboBox stat = g.Children[1] as ComboBox;
                TextBox val = g.Children[2] as TextBox;

                if (stat.SelectedIndex >= 0 && val.Text != "")
                {
                    foreach (string k in statsStringsToAttribute.Keys)
                    {
                        if (k == stat.Text && double.TryParse(val.Text, out _))
                        {
                            weapon.Stats[statsStringsToAttribute[k]] = double.Parse(val.Text);
                        }
                    }
                }
            }
            
            weapon.Slot = IsRangedWeapon(weapon) ? "Ranged" : "Weapon";

            weapon.TwoHanded = TwoHanded.IsChecked == true;
            weapon.Type = WeaponType.Text;
            weapon.DamageMin = int.Parse(DamageMin.Text);
            weapon.DamageMax = int.Parse(DamageMax.Text);
            weapon.Speed = double.Parse(Speed.Text);
            
            UpdateJsonWeapon(weapon);

            return weapon;
        }

        private void LoadWeapons()
        {
            try
            {
                foreach (string s in Directory.GetFiles(System.IO.Path.Combine(Program.basePath(), DB_FOLDER, WEAPON_ITEMS_FOLDER), "*.json"))
                {
                    JsonUtil.JsonWeapon weapon = JsonConvert.DeserializeObject<JsonUtil.JsonWeapon>(File.ReadAllText(s));

                    if (jsonWeapons.Any(it => it.Id == weapon.Id))
                    {
                        weapon.Id = jsonWeapons.Max(it => it.Id) + 1;

                        string path = System.IO.Path.Combine(Program.basePath(), DB_FOLDER, WEAPON_ITEMS_FOLDER, s);

                        File.WriteAllText(path, JsonConvert.SerializeObject(weapon, Newtonsoft.Json.Formatting.Indented));
                    }

                    AddJsonWeapon(weapon);
                    weaponsFileNames[weapon] = System.IO.Path.GetFileNameWithoutExtension(s);
                    WeaponsList.Items.Add(weapon.Name);
                }
            }
            catch (Exception e)
            {
                Program.Debug("Error while loading weapons : " + e.ToString());
            }
        }

        private void SaveWeaponClick(object sender, RoutedEventArgs e)
        {
            if (CurrentWeapon().Name != WeaponName.Text)
            {
                string name = WeaponName.Text;
                if (CollectionContainsString(WeaponsList.Items, name))
                {
                    int i = 1;
                    string currentName;
                    do
                    {
                        currentName = name + " (" + i + ")";
                        i++;
                    }
                    while (CollectionContainsString(WeaponsList.Items, currentName));

                    WeaponName.Text = currentName;
                }
            }

            SaveWeapon();
        }

        private void SaveWeapon()
        {
            JsonUtil.JsonWeapon weapon = UpdateCurrentWeapon();

            string path;
            if (weaponsFileNames[weapon] != null)
            {
                path = System.IO.Path.Combine(Program.basePath(), DB_FOLDER, WEAPON_ITEMS_FOLDER, weaponsFileNames[weapon] + ".json");


                if (WeaponsList.Items[selectedWeaponIndex].ToString() != WeaponName.Text)
                {
                    File.Delete(path);

                    weaponsFileNames[weapon] = UnusedFileNameFromString(WeaponName.Text);
                    path = System.IO.Path.Combine(Program.basePath(), DB_FOLDER, WEAPON_ITEMS_FOLDER, weaponsFileNames[weapon] + ".json");
                }
            }
            else
            {
                weaponsFileNames[weapon] = UnusedFileNameFromString(WeaponName.Text);
                path = System.IO.Path.Combine(Program.basePath(), DB_FOLDER, WEAPON_ITEMS_FOLDER, weaponsFileNames[weapon] + ".json");
            }
            
            JsonUtil.JsonEnchantment e = weapon.Enchantment;
            weapon.Enchantment = null;
            File.WriteAllText(path, JsonConvert.SerializeObject(weapon, Newtonsoft.Json.Formatting.Indented));
            weapon.Enchantment = e;

            jsonWeapons[selectedWeaponIndex] = weapon;
            WeaponsList.Items[selectedWeaponIndex] = WeaponName.Text;
            WeaponsList.SelectedIndex = selectedWeaponIndex;
        }

        private void NewWeaponClick(object sender, RoutedEventArgs e)
        {
            NewWeapon();
        }

        private void NewWeapon()
        {
            string name = "New Weapon";

            if (CollectionContainsString(WeaponsList.Items, name))
            {
                int i = 1;
                string currentName;
                do
                {
                    currentName = name + " (" + i + ")";
                    i++;
                }
                while (CollectionContainsString(WeaponsList.Items, currentName));

                name = currentName;
            }

            NewWeapon(name, false, "Axe", 1, 2, 1);
        }

        private void CopyWeaponClick(object sender, RoutedEventArgs e)
        {
            JsonUtil.JsonWeapon weapon = CurrentWeapon();
            string name = weapon.Name;

            if (CollectionContainsString(WeaponsList.Items, name))
            {
                int i = 1;
                string currentName;
                do
                {
                    currentName = name + " (" + i + ")";
                    i++;
                }
                while (CollectionContainsString(WeaponsList.Items, currentName));

                name = currentName;
            }
            
            NewWeapon(name, weapon.TwoHanded, weapon.Type, weapon.DamageMin, weapon.DamageMax, weapon.Speed, weapon.Stats, weapon.Enchantment, weapon.Buff);
        }

        private void WeaponsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (WeaponsList.SelectedIndex != -1)
            {
                selectedWeaponIndex = WeaponsList.SelectedIndex;

                JsonUtil.JsonWeapon weapon = CurrentWeapon();

                WeaponName.Text = weapon.Name;

                WeaponType.SelectedIndex = GetComboBoxIndexWithString(WeaponType, weapon.Type);
                TwoHanded.IsChecked = weapon.TwoHanded;
                DamageMin.Text = weapon.DamageMin.ToString();
                DamageMax.Text = weapon.DamageMax.ToString();
                Speed.Text = weapon.Speed.ToString();

                ResetStats();
                for (int i = 1; i < weapon.Stats.Count; i++)
                {
                    AddStat();
                }

                int j = 0;
                foreach (string s in weapon.Stats.Keys)
                {
                    Grid g = StackPanelStats.Children[j] as Grid;
                    ComboBox stat = g.Children[1] as ComboBox;
                    TextBox val = g.Children[2] as TextBox;

                    for (int i = 0; i < stat.Items.Count; i++)
                    {
                        string cbs = (stat.Items[i] as ComboBoxItem).Content.ToString();
                        if (statsStringsToAttribute[cbs] == s)
                        {
                            stat.SelectedIndex = i;
                            break;
                        }
                    }

                    val.Text = weapon.Stats[s].ToString();
                    j++;
                }
            }

        }

        private void LoadPlayerWeapon(JsonUtil.JsonWeapon weapon, string slot)
        {
            if (!jsonWeapons.Contains(weapon))
            {
                if (jsonWeapons.Any(e => e.Id == weapon.Id))
                {
                    List<JsonUtil.JsonWeapon> wes = jsonWeapons.Where(w => w.Id == weapon.Id && w.Name == weapon.Name && w.Slot == weapon.Slot && w.School == weapon.School
                                                                    && w.Speed == weapon.Speed && w.TwoHanded == weapon.TwoHanded && w.Type == weapon.Type && w.Stats.Count == weapon.Stats.Count
                                                                    && !w.Stats.Except(weapon.Stats).Any()).ToList();
                    JsonUtil.JsonWeapon we = wes.Count > 0 ? wes.First() : null;
                    if (we != null)
                    {
                        JsonUtil.JsonEnchantment enc = weapon.Enchantment;
                        weapon = we;
                        weapon.Enchantment = enc;
                        return;
                    }
                    else
                    {
                        weapon.Id = jsonWeapons.Max(e => e.Id) + 1;
                    }
                }

                if (CollectionContainsString(WeaponsList.Items, weapon.Name))
                {
                    int i = 1;
                    string currentName;
                    do
                    {
                        currentName = weapon.Name + " (" + i + ")";
                        i++;
                    }
                    while (CollectionContainsString(WeaponsList.Items, currentName));

                    weapon.Name = currentName;
                }

                if (weapon.Slot == null) weapon.Slot = StringPlayerSlotToSlot(slot);

                weaponsFileNames[weapon] = UnusedFileNameFromString(weapon.Name);
                AddJsonWeapon(weapon);
                WeaponsList.Items.Add(weapon.Name);

                JsonUtil.JsonEnchantment en = weapon.Enchantment;
                weapon.Enchantment = null;
                string path = System.IO.Path.Combine(Program.basePath(), DB_FOLDER, WEAPON_ITEMS_FOLDER, weaponsFileNames[weapon] + ".json");
                File.WriteAllText(path, JsonConvert.SerializeObject(weapon, Newtonsoft.Json.Formatting.Indented));
                weapon.Enchantment = en;
            }
        }

        private void NewWeapon(string name, bool twoHanded, string type, double dmgMin, double dmgMax, double speed, Dictionary<string, double> attributes = null, JsonUtil.JsonEnchantment enchant = null, JsonUtil.JsonEnchantment buffs = null)
        {
            JsonUtil.JsonWeapon weapon = new JsonUtil.JsonWeapon(dmgMin, dmgMax, speed, twoHanded, type, jsonWeapons.Max(w => w.Id) + 1, name, attributes, enchant, buffs);
            AddJsonWeapon(weapon);
            weaponsFileNames[weapon] = null;

            WeaponsList.Items.Add(name);

            WeaponsList.SelectedIndex = WeaponsList.Items.Count - 1;
        }

        private void DeleteWeaponClick(object sender, RoutedEventArgs e)
        {
            JsonUtil.JsonWeapon weapon = CurrentWeapon();

            if (weaponsFileNames[weapon] != null)
            {
                File.Delete(System.IO.Path.Combine(Program.basePath(), DB_FOLDER, WEAPON_ITEMS_FOLDER, weaponsFileNames[weapon] + ".json"));
            }

            RemoveJsonWeapon(weapon);
            weaponsFileNames.Remove(weapon);
            WeaponsList.Items.RemoveAt(selectedWeaponIndex);

            if (WeaponsList.Items.Count > 0)
            {
                selectedWeaponIndex -= selectedWeaponIndex >= WeaponsList.Items.Count ? 1 : 0;
                WeaponsList.SelectedIndex = selectedWeaponIndex;
            }
            else
            {
                NewWeapon();
            }
        }

        private void WeaponType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(TwoHanded != null)
            {
                if(WeaponType.SelectedIndex > 2)
                {
                    TwoHanded.IsEnabled = false;

                    if (WeaponType.SelectedIndex > 6)       // Ranged
                    {
                        TwoHanded.IsChecked = false;
                    }
                    else if (WeaponType.SelectedIndex > 4)  // 2-Handed
                    {
                        TwoHanded.IsChecked = true;
                    }
                    else
                    {                                       // 1-Handed
                        TwoHanded.IsChecked = false;
                    }
                }
                else
                {
                    TwoHanded.IsEnabled = true;
                }

                JsonUtil.JsonWeapon weapon = CurrentWeapon();
                
                if (WeaponType.SelectedIndex > 6 && weapon.Slot != "Ranged")
                {
                    weapon.Slot = "Ranged";
                    UpdateJsonWeapon(weapon);
                }
                else if (WeaponType.SelectedIndex <= 6 && weapon.Slot == "Ranged")
                {
                    weapon.Slot = "Weapon";
                    UpdateJsonWeapon(weapon);
                }
            }
        }

        private void TwoHanded_Checked(object sender, RoutedEventArgs e)
        {
            JsonUtil.JsonWeapon weapon = CurrentWeapon();
            
            if(weapon.TwoHanded != TwoHanded.IsChecked)
            {
                weapon.TwoHanded = TwoHanded.IsChecked == true;
                UpdateJsonWeapon(weapon);
            }
        }

        private void AddJsonWeapon(JsonUtil.JsonWeapon weapon)
        {
            jsonWeapons.Add(weapon);
            
            foreach (Player.Slot pslot in SlotUtil.ToPlayerSlot(SlotUtil.FromString(weapon.Slot)))
            {
                itemsByPlayerSlot[pslot].Add(weapon);
                PlayerSlotToComboBox(pslot).Items.Add(weapon.Name);
            }
        }

        private void UpdateJsonWeapon(JsonUtil.JsonWeapon weapon)
        {
            bool isRanged = weapon.Slot == "Ranged";
            bool isMH = !isRanged;
            bool isOH = isMH && !weapon.TwoHanded;


            if (!isRanged && itemsByPlayerSlot[Player.Slot.Ranged].Contains(weapon))
            {
                PlayerSlotToComboBox(Player.Slot.Ranged).Items.RemoveAt(itemsByPlayerSlot[Player.Slot.Ranged].IndexOf(weapon));
                itemsByPlayerSlot[Player.Slot.Ranged].Remove(weapon);
            }
            else if (isRanged && !itemsByPlayerSlot[Player.Slot.Ranged].Contains(weapon))
            {
                itemsByPlayerSlot[Player.Slot.Ranged].Add(weapon);
                PlayerSlotToComboBox(Player.Slot.Ranged).Items.Add(weapon.Name);
            }

            if (!isMH && itemsByPlayerSlot[Player.Slot.MH].Contains(weapon))
            {
                PlayerSlotToComboBox(Player.Slot.MH).Items.RemoveAt(itemsByPlayerSlot[Player.Slot.MH].IndexOf(weapon));
                itemsByPlayerSlot[Player.Slot.MH].Remove(weapon);
            }
            else if (isMH && !itemsByPlayerSlot[Player.Slot.MH].Contains(weapon))
            {
                itemsByPlayerSlot[Player.Slot.MH].Add(weapon);
                PlayerSlotToComboBox(Player.Slot.MH).Items.Add(weapon.Name);
            }

            if (!isOH && itemsByPlayerSlot[Player.Slot.OH].Contains(weapon))
            {
                PlayerSlotToComboBox(Player.Slot.OH).Items.RemoveAt(itemsByPlayerSlot[Player.Slot.OH].IndexOf(weapon));
                itemsByPlayerSlot[Player.Slot.OH].Remove(weapon);
            }
            else if (isOH && !itemsByPlayerSlot[Player.Slot.OH].Contains(weapon))
            {
                itemsByPlayerSlot[Player.Slot.OH].Add(weapon);
                PlayerSlotToComboBox(Player.Slot.OH).Items.Add(weapon.Name);
            }
        }

        private void RemoveJsonWeapon(JsonUtil.JsonWeapon weapon)
        {
            jsonWeapons.Remove(weapon);

            foreach (Player.Slot pslot in SlotUtil.ToPlayerSlot(SlotUtil.FromString(weapon.Slot)))
            {
                PlayerSlotToComboBox(pslot).Items.RemoveAt(itemsByPlayerSlot[pslot].IndexOf(weapon));
                itemsByPlayerSlot[pslot].Remove(weapon);
            }
        }

        #endregion

        #region Enchants

        private JsonUtil.JsonEnchantment UpdateCurrentEnchant()
        {
            JsonUtil.JsonEnchantment enchant = CurrentEnchant();
            enchant.Name = EnchantName.Text;

            enchant.Stats = new Dictionary<string, double>();
            foreach (Grid g in StackPanelStatsEnchants.Children)
            {
                ComboBox stat = g.Children[1] as ComboBox;
                TextBox val = g.Children[2] as TextBox;

                if (stat.SelectedIndex >= 0 && val.Text != "")
                {
                    foreach (string k in statsStringsToAttribute.Keys)
                    {
                        if (k == stat.Text && double.TryParse(val.Text, out _))
                        {
                            enchant.Stats[statsStringsToAttribute[k]] = double.Parse(val.Text);
                        }
                    }
                }
            }

            string oldSlot = enchant.Slot != EnchantSlot.Text ? enchant.Slot : "";

            enchant.Slot = EnchantSlot.Text;

            UpdateJsonEnchant(enchant, oldSlot);

            return enchant;
        }

        private void LoadEnchants()
        {
            try
            {
                foreach (string s in Directory.GetFiles(System.IO.Path.Combine(Program.basePath(), DB_FOLDER, ENCHANTS_FOLDER), "*.json"))
                {
                    JsonUtil.JsonEnchantment enchant = JsonConvert.DeserializeObject<JsonUtil.JsonEnchantment>(File.ReadAllText(s));

                    if (jsonEnchants.Any(it => it.Id == enchant.Id))
                    {
                        enchant.Id = jsonEnchants.Max(it => it.Id) + 1;

                        string path = System.IO.Path.Combine(Program.basePath(), DB_FOLDER, ENCHANTS_FOLDER, s);

                        File.WriteAllText(path, JsonConvert.SerializeObject(enchant, Newtonsoft.Json.Formatting.Indented));
                    }

                    AddJsonEnchant(enchant);
                    enchantsFileNames[enchant] = System.IO.Path.GetFileNameWithoutExtension(s);
                    EnchantsList.Items.Add(enchant.Name);
                }
            }
            catch (Exception e)
            {
                Program.Debug("Error while loading enchants : " + e.ToString());
            }
        }

        private void SaveEnchantClick(object sender, RoutedEventArgs e)
        {
            if (CurrentEnchant().Name != EnchantName.Text)
            {
                string name = EnchantName.Text;
                if (CollectionContainsString(EnchantsList.Items, name))
                {
                    int i = 1;
                    string currentName;
                    do
                    {
                        currentName = name + " (" + i + ")";
                        i++;
                    }
                    while (CollectionContainsString(EnchantsList.Items, currentName));

                    EnchantName.Text = currentName;
                }
            }

            SaveCurrentEnchant();
        }

        private void SaveCurrentEnchant()
        {
            JsonUtil.JsonEnchantment enchant = UpdateCurrentEnchant();

            string path;
            if (enchantsFileNames[enchant] != null)
            {
                path = System.IO.Path.Combine(Program.basePath(), DB_FOLDER, ENCHANTS_FOLDER, enchantsFileNames[enchant] + ".json");

                if (EnchantsList.Items[selectedEnchantIndex].ToString() != EnchantName.Text)
                {
                    File.Delete(path);

                    enchantsFileNames[enchant] = UnusedFileNameFromString(EnchantName.Text);
                    path = System.IO.Path.Combine(Program.basePath(), DB_FOLDER, ENCHANTS_FOLDER, enchantsFileNames[enchant] + ".json");
                }
            }
            else
            {
                enchantsFileNames[enchant] = UnusedFileNameFromString(EnchantName.Text);
                path = System.IO.Path.Combine(Program.basePath(), DB_FOLDER, ENCHANTS_FOLDER, enchantsFileNames[enchant] + ".json");
            }

            File.WriteAllText(path, JsonConvert.SerializeObject(enchant, Newtonsoft.Json.Formatting.Indented));

            EnchantsList.Items[selectedEnchantIndex] = EnchantName.Text;
            EnchantsList.SelectedIndex = selectedEnchantIndex;
        }

        private void NewEnchantClick(object sender, RoutedEventArgs e)
        {
            NewEnchant();
        }

        private void NewEnchant()
        {
            string name = "New Enchant";

            if (CollectionContainsString(EnchantsList.Items, name))
            {
                int i = 1;
                string currentName;
                do
                {
                    currentName = name + " (" + i + ")";
                    i++;
                }
                while (CollectionContainsString(EnchantsList.Items, currentName));

                name = currentName;
            }

            NewEnchant(name);
        }

        private void CopyEnchantClick(object sender, RoutedEventArgs e)
        {
            JsonUtil.JsonEnchantment enchant = CurrentEnchant();
            string name = enchant.Name;

            if (CollectionContainsString(EnchantsList.Items, name))
            {
                int i = 1;
                string currentName;
                do
                {
                    currentName = name + " (" + i + ")";
                    i++;
                }
                while (CollectionContainsString(EnchantsList.Items, currentName));

                name = currentName;
            }
            
                NewEnchant(name, enchant.Slot, enchant.Stats);
        }

        private void EnchantsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EnchantsList.SelectedIndex != -1)
            {
                selectedEnchantIndex = EnchantsList.SelectedIndex;

                JsonUtil.JsonEnchantment enchant = CurrentEnchant();

                EnchantName.Text = enchant.Name;

                EnchantSlot.SelectedIndex = GetComboBoxIndexWithString(EnchantSlot, enchant.Slot);

                ResetStatsEnchants();
                for (int i = 1; i < enchant.Stats.Count; i++)
                {
                    AddStatEnchants();
                }

                int j = 0;
                foreach (string s in enchant.Stats.Keys)
                {
                    Grid g = StackPanelStatsEnchants.Children[j] as Grid;
                    ComboBox stat = g.Children[1] as ComboBox;
                    TextBox val = g.Children[2] as TextBox;

                    for (int i = 0; i < stat.Items.Count; i++)
                    {
                        string cbs = (stat.Items[i] as ComboBoxItem).Content.ToString();
                        if (statsStringsToAttribute[cbs] == s)
                        {
                            stat.SelectedIndex = i;
                            break;
                        }
                    }

                    val.Text = enchant.Stats[s].ToString();
                    j++;
                }
            }

        }

        private void LoadPlayerEnchant(JsonUtil.JsonEnchantment enchant, string slot)
        {
            if (!jsonEnchants.Contains(enchant))
            {
                if(jsonEnchants.Any(e => e.Id == enchant.Id))
                {
                    List<JsonUtil.JsonEnchantment> ens = jsonEnchants.Where(e => e.Id == enchant.Id && e.Name == enchant.Name && e.Slot == enchant.Slot && e.Stats.Count == enchant.Stats.Count && !e.Stats.Except(enchant.Stats).Any()).ToList();
                    JsonUtil.JsonEnchantment en = ens.Count > 0 ? ens.First() : null;
                    if (en != null)
                    {
                        enchant = en;
                        return;
                    }
                    else
                    {
                        enchant.Id = jsonEnchants.Max(e => e.Id) + 1;
                    }
                }

                if (CollectionContainsString(EnchantsList.Items, enchant.Name))
                {
                    int i = 1;
                    string currentName;
                    do
                    {
                        currentName = enchant.Name + " (" + i + ")";
                        i++;
                    }
                    while (CollectionContainsString(EnchantsList.Items, currentName));

                    enchant.Name = currentName;
                }

                if (enchant.Slot == null) enchant.Slot = StringPlayerSlotToSlot(slot);

                enchantsFileNames[enchant] = UnusedFileNameFromString(enchant.Name);
                AddJsonEnchant(enchant);
                EnchantsList.Items.Add(enchant.Name);

                string path = System.IO.Path.Combine(Program.basePath(), DB_FOLDER, ENCHANTS_FOLDER, enchantsFileNames[enchant] + ".json");
                File.WriteAllText(path, JsonConvert.SerializeObject(enchant, Newtonsoft.Json.Formatting.Indented));
            }
        }

        private void NewEnchant(string name, string slot = "Any", Dictionary<string, double> attributes = null)
        {
            JsonUtil.JsonEnchantment enchant = new JsonUtil.JsonEnchantment(jsonEnchants.Max(en => en.Id) + 1, name, slot, attributes);

            AddJsonEnchant(enchant);
            enchantsFileNames[enchant] = null;

            EnchantsList.Items.Add(name);

            EnchantsList.SelectedIndex = EnchantsList.Items.Count - 1;
        }

        private void DeleteEnchantClick(object sender, RoutedEventArgs e)
        {
            JsonUtil.JsonEnchantment enchant = CurrentEnchant();

            if (enchantsFileNames[enchant] != null)
            {
                File.Delete(System.IO.Path.Combine(Program.basePath(), DB_FOLDER, ENCHANTS_FOLDER, enchantsFileNames[enchant] + ".json"));
            }

            RemoveJsonEnchant(enchant);
            enchantsFileNames.Remove(enchant);
            EnchantsList.Items.RemoveAt(selectedEnchantIndex);

            if (EnchantsList.Items.Count > 0)
            {
                selectedEnchantIndex -= selectedEnchantIndex >= EnchantsList.Items.Count ? 1 : 0;
                EnchantsList.SelectedIndex = selectedEnchantIndex;
            }
            else
            {
                NewEnchant();
            }
        }

        private void AddJsonEnchant(JsonUtil.JsonEnchantment enchant)
        {
            jsonEnchants.Add(enchant);

            foreach (Player.Slot pslot in SlotUtil.ToPlayerSlot(SlotUtil.FromString(enchant.Slot)))
            {
                if (PlayerEnchantSlotToComboBox(pslot) != null)
                {
                    enchantsByPlayerSlot[pslot].Add(enchant);
                    PlayerEnchantSlotToComboBox(pslot).Items.Add(enchant.Name);
                }
            }
        }

        private void UpdateJsonEnchant(JsonUtil.JsonEnchantment enchant, string oldSlot = "")
        {
            if (oldSlot != "")
            {
                foreach (Player.Slot pslot in SlotUtil.ToPlayerSlot(SlotUtil.FromString(oldSlot)))
                {
                    if (PlayerEnchantSlotToComboBox(pslot) != null)
                    {
                        PlayerEnchantSlotToComboBox(pslot).Items.RemoveAt(enchantsByPlayerSlot[pslot].IndexOf(enchant));
                        enchantsByPlayerSlot[pslot].Remove(enchant);
                    }
                }
                foreach (Player.Slot pslot in SlotUtil.ToPlayerSlot(SlotUtil.FromString(enchant.Slot)))
                {
                    if (PlayerEnchantSlotToComboBox(pslot) != null)
                    {
                        enchantsByPlayerSlot[pslot].Add(enchant);
                        PlayerEnchantSlotToComboBox(pslot).Items.Add(enchant.Name);
                    }
                }
            }
            else
            {
                foreach (Player.Slot pslot in SlotUtil.ToPlayerSlot(SlotUtil.FromString(enchant.Slot)))
                {
                    if (PlayerEnchantSlotToComboBox(pslot) != null)
                    {
                        PlayerEnchantSlotToComboBox(pslot).Items[enchantsByPlayerSlot[pslot].IndexOf(enchant)] = enchant.Name;
                    }
                }
            }
        }

        private void RemoveJsonEnchant(JsonUtil.JsonEnchantment enchant)
        {
            jsonEnchants.Remove(enchant);

            foreach (Player.Slot pslot in SlotUtil.ToPlayerSlot(SlotUtil.FromString(enchant.Slot)))
            {
                if (PlayerEnchantSlotToComboBox(pslot) != null)
                {
                    PlayerEnchantSlotToComboBox(pslot).Items.RemoveAt(enchantsByPlayerSlot[pslot].IndexOf(enchant));
                    enchantsByPlayerSlot[pslot].Remove(enchant);
                }
            }
        }

        private void EnchantSlot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ItemSlot.SelectedIndex > -1)
            {
                JsonUtil.JsonEnchantment enchant = CurrentEnchant();
                string newSlot = (EnchantSlot.Items[EnchantSlot.SelectedIndex] as ComboBoxItem).Content.ToString();

                if (EnchantSlot.Text != "" && enchant.Slot != newSlot)
                {
                    string oldSlot = enchant.Slot;

                    enchant.Slot = newSlot;

                    UpdateJsonEnchant(enchant, oldSlot);
                }
            }
        }

        #endregion

        #region Console

        private void NewConsole()
        {
            nbSim++;
            DeleteConsoleButton.IsEnabled = true;

            TabItem newConsole = Copy(ConsoleEmpty) as TabItem;
            newConsole.Header = "Simulation " + nbSim;
            CurrentConsole = (newConsole.Content as ScrollViewer).Content as TextBox;
            ConsoleTabControl.Items.Add(newConsole);
            ConsoleTabControl.SelectedIndex = ConsoleTabControl.Items.Count - 1;
        }

        private void DeleteConsole_Click(object sender, RoutedEventArgs e)
        {
            if (ConsoleTabControl.Items.Count > 0 && ConsoleTabControl.SelectedIndex >= 0)
            {
                ConsoleTabControl.Items.RemoveAt(ConsoleTabControl.SelectedIndex);

                if (ConsoleTabControl.Items.Count == 0)
                {
                    DeleteConsoleButton.IsEnabled = false;
                }
            }
        }

        #endregion

        #region Util

        private void PopulateSlotLists()
        {
            foreach (Player.Slot slot in (Player.Slot[])Enum.GetValues(typeof(Player.Slot)))
            {
                itemsByPlayerSlot[slot] = new List<JsonUtil.JsonItem>();
                enchantsByPlayerSlot[slot] = new List<JsonUtil.JsonEnchantment>();
            }
        }

        public ComboBox PlayerSlotToComboBox(Player.Slot slot)
        {
            switch (slot)
            {
                case Player.Slot.MH: return MH;
                case Player.Slot.OH: return OH;
                case Player.Slot.Ranged: return Ranged;
                case Player.Slot.Head: return Head;
                case Player.Slot.Shoulders: return Shoulders;
                case Player.Slot.Neck: return Neck;
                case Player.Slot.Back: return Back;
                case Player.Slot.Chest: return Chest;
                case Player.Slot.Wrists: return Wrists;
                case Player.Slot.Hands: return Hands;
                case Player.Slot.Waist: return Waist;
                case Player.Slot.Legs: return Legs;
                case Player.Slot.Feet: return Feet;
                case Player.Slot.Finger1: return Finger1;
                case Player.Slot.Finger2: return Finger2;
                case Player.Slot.Trinket1: return Trinket1;
                case Player.Slot.Trinket2: return Trinket2;
                default: return null;
            }
        }

        public string StringPlayerSlotToSlot(string pslot)
        {
            switch (pslot)
            {
                case "Trinket1": return "Trinket";
                case "Trinket2": return "Trinket";
                case "Finger1": return "Finger";
                case "Finger2": return "Finger";
                case "MH": return "Weapon";
                case "OH": return "Weapon";
                default: return pslot;
            }
        }

        public ComboBox PlayerEnchantSlotToComboBox(Player.Slot slot)
        {
            switch (slot)
            {
                case Player.Slot.MH: return EnchantMH;
                case Player.Slot.OH: return EnchantOH;
                case Player.Slot.Ranged: return EnchantRanged;
                case Player.Slot.Head: return EnchantHead;
                case Player.Slot.Shoulders: return EnchantShoulders;
                case Player.Slot.Back: return EnchantBack;
                case Player.Slot.Chest: return EnchantChest;
                case Player.Slot.Wrists: return EnchantWrists;
                case Player.Slot.Hands: return EnchantHands;
                case Player.Slot.Legs: return EnchantLegs;
                case Player.Slot.Feet: return EnchantFeet;
                default: return null;
            }
        }

        private string UnusedFileNameFromString(string s)
        {
            return UnusedStringFromList(SafeFileName(s), itemsFileNames.Values.ToList());
        }

        private string UnusedStringFromList(string s, List<string> list)
        {
            string res = s;

            if (list.Contains(res))
            {
                int i = 1;
                do
                {
                    res = s + " (" + i + ")";
                    i++;
                }
                while (list.Contains(res));
            }

            return res;
        }

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

        private void ResetStats()
        {
            StackPanelStats.Children.RemoveRange(1, StackPanelStats.Children.Count - 1);
            Grid firstRow = StackPanelStats.Children[0] as Grid;
            (firstRow.Children[1] as ComboBox).SelectedIndex = -1;
            (firstRow.Children[2] as TextBox).Text = "";
        }

        private void ResetStatsEnchants()
        {
            StackPanelStatsEnchants.Children.RemoveRange(1, StackPanelStatsEnchants.Children.Count - 1);
            Grid firstRow = StackPanelStatsEnchants.Children[0] as Grid;
            (firstRow.Children[1] as ComboBox).SelectedIndex = -1;
            (firstRow.Children[2] as TextBox).Text = "";
        }

        private Grid AddStat()
        {
            Grid copy = Copy(StackPanelStats.Children[0]) as Grid;
            (copy.Children[0] as Button).IsEnabled = true;
            (copy.Children[0] as Button).Click += StatsRemove_Click;
            (copy.Children[1] as ComboBox).SelectedIndex = -1;
            (copy.Children[2] as TextBox).Text = "";
            StackPanelStats.Children.Add(copy);
            return copy;
        }

        private Grid AddStatWeapons()
        {
            Grid copy = Copy(StackPanelStatsWeapons.Children[0]) as Grid;
            (copy.Children[0] as Button).IsEnabled = true;
            (copy.Children[0] as Button).Click += StatsRemoveWeapons_Click;
            (copy.Children[1] as ComboBox).SelectedIndex = -1;
            (copy.Children[2] as TextBox).Text = "";
            StackPanelStatsWeapons.Children.Add(copy);
            return copy;
        }

        private Grid AddStatEnchants()
        {
            Grid copy = Copy(StackPanelStatsEnchants.Children[0]) as Grid;
            (copy.Children[0] as Button).IsEnabled = true;
            (copy.Children[0] as Button).Click += StatsRemoveEnchants_Click;
            (copy.Children[1] as ComboBox).SelectedIndex = -1;
            (copy.Children[2] as TextBox).Text = "";
            StackPanelStatsEnchants.Children.Add(copy);
            return copy;
        }

        private void RemoveStat(Grid g)
        {
            if (StackPanelStats.Children.Count > 1)
            {
                StackPanelStats.Children.Remove(g);
            }
            else
            {
                Grid first = StackPanelStats.Children[0] as Grid;
                (first.Children[0] as Button).IsEnabled = true;
                (first.Children[0] as Button).Click += StatsRemove_Click;
                (first.Children[1] as ComboBox).SelectedIndex = -1;
                (first.Children[2] as TextBox).Text = "";
            }
        }

        private void RemoveStatWeapons(Grid g)
        {
            if (StackPanelStatsWeapons.Children.Count > 1)
            {
                StackPanelStatsWeapons.Children.Remove(g);
            }
            else
            {
                Grid first = StackPanelStatsWeapons.Children[0] as Grid;
                (first.Children[0] as Button).IsEnabled = true;
                (first.Children[0] as Button).Click += StatsRemoveWeapons_Click;
                (first.Children[1] as ComboBox).SelectedIndex = -1;
                (first.Children[2] as TextBox).Text = "";
            }
        }

        private void RemoveStatEnchants(Grid g)
        {
            if (StackPanelStatsEnchants.Children.Count > 1)
            {
                StackPanelStatsEnchants.Children.Remove(g);
            }
            else
            {
                Grid first = StackPanelStatsEnchants.Children[0] as Grid;
                (first.Children[0] as Button).IsEnabled = true;
                (first.Children[0] as Button).Click += StatsRemoveEnchants_Click;
                (first.Children[1] as ComboBox).SelectedIndex = -1;
                (first.Children[2] as TextBox).Text = "";
            }
        }

        private static UIElement Copy(UIElement toCopy)
        {
            return XamlReader.Parse(XamlWriter.Save(toCopy)) as UIElement;
        }

        private JsonUtil.JsonItem CurrentItem()
        {
            return jsonItems[selectedItemIndex];
        }

        private JsonUtil.JsonWeapon CurrentWeapon()
        {
            return jsonWeapons[selectedWeaponIndex];
        }

        private JsonUtil.JsonEnchantment CurrentEnchant()
        {
            return jsonEnchants[selectedEnchantIndex];
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

        public static bool IsRangedWeapon(JsonUtil.JsonWeapon weapon)
        {
            return weapon.Type == "Bow" || weapon.Type == "Crossbow" || weapon.Type == "Gun" || weapon.Type == "Wand";
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
                CurrentConsole.Text = s;
            }));
        }

        public void ConsoleTextAdd(string s, bool newLine = true)
        {
            Dispatcher.Invoke(new System.Action(() => {
                if (newLine && CurrentConsole.Text != "") CurrentConsole.Text += "\n";
                CurrentConsole.Text += s;
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

        #region Events

        private void ResetPlayerItemSelect_Click(object sender, RoutedEventArgs e)
        {
            Grid g = (sender as Button).Parent as Grid;
            (g.Children[2] as ComboBox).SelectedIndex = -1;
            if(g.Children.Count > 3) (g.Children[3] as ComboBox).SelectedIndex = -1;
            if(g.Children.Count > 4) (g.Children[4] as ComboBox).SelectedIndex = -1;
        }

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
                TargetError.IsEnabled = LogFight.SelectedIndex == 0;
                TargetError.SelectedIndex = TargetError.IsEnabled ? 2 : 0;
                StatsWeights.SelectedIndex = 1;
                StatsWeights.IsEnabled = LogFight.SelectedIndex == 0;
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

        private void StatsAdd_Click(object sender, RoutedEventArgs e)
        {
            AddStat();
        }

        private void StatsAddWeapons_Click(object sender, RoutedEventArgs e)
        {
            AddStatWeapons();
        }

        private void StatsAddEnchants_Click(object sender, RoutedEventArgs e)
        {
            AddStatEnchants();
        }

        private void StatsRemove_Click(object sender, RoutedEventArgs e)
        {
            RemoveStat((sender as Button).Parent as Grid);
        }

        private void StatsRemoveWeapons_Click(object sender, RoutedEventArgs e)
        {
            RemoveStatWeapons((sender as Button).Parent as Grid);
        }

        private void StatsRemoveEnchants_Click(object sender, RoutedEventArgs e)
        {
            RemoveStatEnchants((sender as Button).Parent as Grid);
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

        private void ItemsList_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if(ItemsList.SelectedIndex > -1) ItemName.Text = ItemsList.Text;
        }

        private void ItemName_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if(ItemsList.SelectedIndex > -1) ItemsList.Text = ItemName.Text;
        }

        private void WeaponsList_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if(WeaponsList.SelectedIndex > -1) WeaponName.Text = WeaponsList.Text;
        }

        private void WeaponName_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if(WeaponsList.SelectedIndex > -1) WeaponsList.Text = WeaponName.Text;
        }

        private void EnchantsList_TextChanged(object sender, TextChangedEventArgs e)
        {
            //EnchantName.Text = EnchantsList.Text;
        }

        private void EnchantName_TextChanged(object sender, TextChangedEventArgs e)
        {
            //EnchantsList.Text = EnchantName.Text;
        }

        private void TalentsHyperlink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(@"https://" + (Program.version == ClassicCraft.Version.Vanilla ? "classic" : "tbc") + ".wowhead.com/talent-calc/" + Class.Text.ToLower() + "/" + Talents.Text));
            }
            catch (Exception ex)
            {
                Program.Debug(ex.ToString());
            }
        }

        private void Talents_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !(e.Text == "-" || int.TryParse(e.Text, out _));
        }

        private void Talents_OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            string str = e.DataObject.GetData(typeof(string)) as string;

            string tal = str.Split('/').Last();

            bool ok = true;

            foreach (string s in tal.Split('-'))
            {
                if (!ulong.TryParse(s, out _))
                {
                    ok = false;
                    break;
                }
            }

            if (ok)
            {
                Talents.Text = tal;
            }

            e.CancelCommand();
        }

        #endregion
    }
}
