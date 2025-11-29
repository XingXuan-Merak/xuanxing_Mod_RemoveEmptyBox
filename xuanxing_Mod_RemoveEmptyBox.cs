using Duckov;
using Duckov.Economy;
using Duckov.Modding;
using Duckov.UI;
using Duckov.Utilities;
using HarmonyLib;
using ItemStatsSystem;
using SodaCraft.Localizations;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace xuanxing_Mod_RemoveEmptyBox
{
    [System.Serializable]
    public class ModConfig
    {
        public bool Info_ItemValue = true;
        public bool Info_ItemDecompose = true;
        public bool Action_ItemDecompose = false;
        public bool Action_ItemDecomposeAll = false;
        public bool Action_ItemPickAll = false;
        public bool Action_DeleteBox = false;
        public bool Action_DeleteBoxAll = false;
        public bool Action_DeleteBoxClear = false;
        public bool Action_ItemValueCheck = false;
        public float DeleteRadius = 5f;
        public float ValueThreshold = 100f;
    }
    [HarmonyPatch]
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        public static string mod_name = "移除低价值容器";
        private static readonly ModConfig config = new ModConfig();
        private static readonly AccessTools.FieldRef<LootView, InteractableLootbox> TargetLootBox = AccessTools.FieldRefAccess<LootView, InteractableLootbox>("targetLootBox");
        private Harmony? harmony;
        private MethodInfo? pickAllBtnMethod;
        private RectTransform? buttonRowContainer;
        private HorizontalLayoutGroup? buttonRowLayout;
        private Button? pickAllButton;
        private Button? deleteBoxButton;
        private Button? decomposeAllButton;
        private static Button? decomposeButton;
        private TextMeshProUGUI? _text1 = null;
        private TextMeshProUGUI? _text2 = null;
        private TextMeshProUGUI Text1
        {
            get
            {
                if (_text1 == null) _text1 = Instantiate(GameplayDataSettings.UIStyle.TemplateTextUGUI);
                return _text1;
            }
        }
        private TextMeshProUGUI Text2
        {
            get
            {
                if (_text2 == null) _text2 = Instantiate(GameplayDataSettings.UIStyle.TemplateTextUGUI);
                return _text2;
            }
        }
        private static string PersistentConfigPath => Path.Combine(UnityEngine.Application.dataPath + "/../ModConfigs/xuanxing_Mod_RemoveEmptyBox", "ModConfig.json");
        void OnEnable()
        {
            ModManager.OnModActivated += OnModActivated;
            LootView.onOpen += OnLootViewOpened;
            LootView.onClose += OnLootViewClosed;
            ItemHoveringUI.onSetupMeta += OnSetupMeta;
            ItemHoveringUI.onSetupItem += OnSetupItemHoveringUI;
            if (ModConfigAPI.IsAvailable())
            {
                SetupModConfig();
                LoadConfigFromModConfig();
            }
            else
            {
                LoadConfigFromFile();
                Debug.LogWarning("【移除空容器mod】：ModConfig不可用,从本地加载配置");
            }
            harmony = new Harmony("RemoveEmptyBox");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Debug.Log("【移除空容器mod】执行：OnEnable");
        }
        void OnDisable()
        {
            ModManager.OnModActivated -= OnModActivated;
            LootView.onOpen -= OnLootViewOpened;
            LootView.onClose -= OnLootViewClosed;
            ItemHoveringUI.onSetupMeta -= OnSetupMeta;
            ItemHoveringUI.onSetupItem -= OnSetupItemHoveringUI;
            Destroy(_text1);
            Destroy(_text2);
            Destroy(buttonRowContainer?.gameObject);
            Destroy(pickAllButton?.gameObject);
            Destroy(deleteBoxButton?.gameObject);
            Destroy(decomposeButton?.gameObject);
            Destroy(decomposeAllButton?.gameObject);
            pickAllButton = null;
            deleteBoxButton = null;
            decomposeButton = null;
            decomposeAllButton = null;
            buttonRowContainer = null;
            buttonRowLayout = null;
            ModConfigAPI.SafeRemoveOnOptionsChangedDelegate(OnModConfigOptionsChanged);
            harmony?.UnpatchAll("RemoveEmptyBox");
            Debug.Log("【移除空容器mod】执行：OnDisable");
        }
        void OnDestroy()
        {
            Destroy(_text1);
            Destroy(_text2);
            Destroy(buttonRowContainer?.gameObject);
            Destroy(pickAllButton?.gameObject);
            Destroy(deleteBoxButton?.gameObject);
            Destroy(decomposeButton?.gameObject);
            Destroy(decomposeAllButton?.gameObject);
            pickAllButton = null;
            deleteBoxButton = null;
            decomposeButton = null;
            decomposeAllButton = null;
            buttonRowContainer = null;
            buttonRowLayout = null;
        }
        private void OnModActivated(ModInfo info, Duckov.Modding.ModBehaviour behaviour)
        {
            if (info.name == ModConfigAPI.ModConfigName)
            {
                SetupModConfig();
                LoadConfigFromModConfig();
            }
        }
        private void SetupModConfig()
        {
            if (!ModConfigAPI.IsAvailable())
            {
                Debug.LogWarning("【移除空容器mod】：ModConfig不可用");
                return;
            }
            Debug.Log("【移除空容器mod】执行：加载ModConfig配置项");
            ModConfigAPI.SafeAddOnOptionsChangedDelegate(OnModConfigOptionsChanged);
            //中英文
            bool isChinese = new[] { SystemLanguage.Chinese, SystemLanguage.ChineseSimplified, SystemLanguage.ChineseTraditional }.Contains(LocalizationManager.CurrentLanguage);
            ModConfigAPI.SafeAddInputWithSlider(
                mod_name,
                "ValueThreshold",
                isChinese ? "价值阈值" : "Value Threshold",
                typeof(float),
                config.ValueThreshold,
                new Vector2(0f, 2000f)
            );
            ModConfigAPI.SafeAddBoolDropdownList(
                mod_name,
                "Action_ItemValueCheck",
                isChinese ? "开启价值检测功能" : "Action: Item Value Check",
                config.Action_ItemValueCheck
            );
            ModConfigAPI.SafeAddInputWithSlider(
                mod_name,
                "DeleteRadius",
                isChinese ? "移除半径" : "Delete Radius",
                typeof(float),
                config.DeleteRadius,
                new Vector2(1f, 20f)
            );
            ModConfigAPI.SafeAddBoolDropdownList(
                mod_name,
                "Action_DeleteBoxAll",
                isChinese ? "开启范围删除功能" : "Action: Delete Box All",
                config.Action_DeleteBoxAll
            );
            ModConfigAPI.SafeAddBoolDropdownList(
                mod_name,
                "Action_DeleteBoxClear",
                isChinese ? "开启清空删除功能" : "Action: Delete Box Clear",
                config.Action_DeleteBoxClear
            );
            ModConfigAPI.SafeAddBoolDropdownList(
                mod_name,
                "Action_DeleteBox",
                isChinese ? "开启一键删除功能" : "Action: Delete Box",
                config.Action_DeleteBox
            );
            ModConfigAPI.SafeAddBoolDropdownList(
                mod_name,
                "Action_ItemPickAll",
                isChinese ? "开启一键拾取功能" : "Action: Item Pick All",
                config.Action_ItemPickAll
            );
            ModConfigAPI.SafeAddBoolDropdownList(
                mod_name,
                "Action_ItemDecomposeAll",
                isChinese ? "开启一键分解功能" : "Action: Item Decompose All",
                config.Action_ItemDecomposeAll
            );
            ModConfigAPI.SafeAddBoolDropdownList(
                mod_name,
                "Action_ItemDecompose",
                isChinese ? "开启右键分解功能" : "Action: Item Decompose",
                config.Action_ItemDecompose
            );
            ModConfigAPI.SafeAddBoolDropdownList(
                mod_name,
                "Info_ItemDecompose",
                isChinese ? "开启物品分解信息" : "Info: Item Decompose",
                config.Info_ItemDecompose
            );
            ModConfigAPI.SafeAddBoolDropdownList(
                mod_name,
                "Info_ItemValue",
                isChinese ? "开启物品价值信息" : "Info: Item Value",
                config.Info_ItemValue
            );
            Debug.Log("【移除空容器mod】执行：加载ModConfig配置项完成");
        }
        private void OnModConfigOptionsChanged(string key)
        {
            if (!key.StartsWith(mod_name + "_")) return;
            LoadConfigFromModConfig();
            SaveConfigToFile();
            Debug.Log($"【移除空容器mod】执行：ModConfig配置项更新 - {key}");
        }
        private void LoadConfigFromModConfig()
        {
            if (!ModConfigAPI.IsAvailable())
            {
                Debug.LogWarning("【移除空容器mod】：ModConfig不可用");
                return;
            }
            config.Info_ItemValue = ModConfigAPI.SafeLoad<bool>(mod_name, "Info_ItemValue", config.Info_ItemValue);
            config.Info_ItemDecompose = ModConfigAPI.SafeLoad<bool>(mod_name, "Info_ItemDecompose", config.Info_ItemDecompose);
            config.Action_ItemDecompose = ModConfigAPI.SafeLoad<bool>(mod_name, "Action_ItemDecompose", config.Action_ItemDecompose);
            config.Action_ItemDecomposeAll = ModConfigAPI.SafeLoad<bool>(mod_name, "Action_ItemDecomposeAll", config.Action_ItemDecomposeAll);
            config.Action_ItemPickAll = ModConfigAPI.SafeLoad<bool>(mod_name, "Action_ItemPickAll", config.Action_ItemPickAll);
            config.Action_DeleteBox = ModConfigAPI.SafeLoad<bool>(mod_name, "Action_DeleteBox", config.Action_DeleteBox);
            config.Action_DeleteBoxAll = ModConfigAPI.SafeLoad<bool>(mod_name, "Action_DeleteBoxAll", config.Action_DeleteBoxAll);
            config.Action_DeleteBoxClear = ModConfigAPI.SafeLoad<bool>(mod_name, "Action_DeleteBoxClear", config.Action_DeleteBoxClear);
            config.Action_ItemValueCheck = ModConfigAPI.SafeLoad<bool>(mod_name, "Action_ItemValueCheck", config.Action_ItemValueCheck);
            config.DeleteRadius = ModConfigAPI.SafeLoad<float>(mod_name, "DeleteRadius", config.DeleteRadius);
            config.ValueThreshold = ModConfigAPI.SafeLoad<float>(mod_name, "ValueThreshold", config.ValueThreshold);
        }
        private void LoadConfigFromFile()
        {
            try
            {
                if (File.Exists(PersistentConfigPath))
                {
                    string json = File.ReadAllText(PersistentConfigPath);
                    ModConfig fileConfig = JsonUtility.FromJson<ModConfig>(json);
                    if (fileConfig != null)
                    {
                        config.Info_ItemValue = fileConfig.Info_ItemValue;
                        config.Info_ItemDecompose = fileConfig.Info_ItemDecompose;
                        config.Action_ItemDecompose = fileConfig.Action_ItemDecompose;
                        config.Action_ItemDecomposeAll = fileConfig.Action_ItemDecomposeAll;
                        config.Action_ItemPickAll = fileConfig.Action_ItemPickAll;
                        config.Action_DeleteBox = fileConfig.Action_DeleteBox;
                        config.Action_DeleteBoxAll = fileConfig.Action_DeleteBoxAll;
                        config.Action_DeleteBoxClear = fileConfig.Action_DeleteBoxClear;
                        config.Action_ItemValueCheck = fileConfig.Action_ItemValueCheck;
                        config.DeleteRadius = fileConfig.DeleteRadius;
                        config.ValueThreshold = fileConfig.ValueThreshold;
                        Debug.Log("【移除空容器mod】执行：从本地文件加载配置成功");
                    }
                }
                else
                {
                    SaveConfigToFile();
                    Debug.LogWarning("【移除空容器mod】本地配置文件不存在，创建默认配置");
                }
            }
            catch
            {
                Debug.LogError("【移除空容器mod】加载本地配置文件失败");
            }
        }
        private void SaveConfigToFile()
        {
            try
            {
                string directory = Path.GetDirectoryName(PersistentConfigPath);
                if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
                File.WriteAllText(PersistentConfigPath, JsonUtility.ToJson(config, true));
                Debug.Log("【移除空容器mod】执行：配置已保存到本地文件");
            }
            catch
            {
                Debug.LogError("【移除空容器mod】保存配置到文件失败");
            }
        }
        private void OnSetupMeta(ItemHoveringUI uiInstance, ItemMetaData data)
        {
            Text1.gameObject.SetActive(false);
            Text2.gameObject.SetActive(false);
        }
        private void OnSetupItemHoveringUI(ItemHoveringUI uiInstance, Item item)
        {
            if (item == null)
            {
                Text1.gameObject.SetActive(false);
                Text2.gameObject.SetActive(false);
                return;
            }
            SetupTextComponent(Text1, uiInstance.LayoutParent);
            SetupTextComponent(Text2, uiInstance.LayoutParent);
            Text1.gameObject.SetActive(config.Info_ItemDecompose);
            Text2.gameObject.SetActive(config.Info_ItemValue);
            if (config.Info_ItemDecompose) Text1.text = GetDecompositionInfo(item);
            if (config.Info_ItemValue) Text2.text = $"${item.GetTotalRawValue() / 2}";
        }
        private void SetupTextComponent(TextMeshProUGUI text, Transform parent)
        {
            text.fontSize = 18f;
            text.gameObject.SetActive(true);
            text.transform.SetAsLastSibling();
            text.transform.SetParent(parent, false);
        }
        private string GetDecompositionInfo(Item item)
        {
            //存在分解物品表时添加文本，每4个换行
            var formula = DecomposeDatabase.Instance.GetFormula(item.TypeID);
            if (!formula.valid) return string.Empty;
            var outputs = GetOutputsFromCost(formula.result);
            if (outputs.Count <= 0) return string.Empty;
            var stringbuilder = new StringBuilder("可拆解出：");
            for (int i = 0; i < outputs.Count; i++)
            {
                var (itemId, count) = outputs[i];
                var meta = ItemAssetsCollection.GetMetaData(itemId);
                if (string.IsNullOrEmpty(meta.Name)) break;
                stringbuilder.Append($"{meta.DisplayName} x{count * item.StackCount}");
                stringbuilder.Append(i < outputs.Count - 1 ? ((i + 1) % 4 == 0 ? "\n" : "，") : "");
            }
            return stringbuilder.ToString();
        }
        private List<(int itemId, int count)> GetOutputsFromCost(Cost cost) => cost.items?.Select(entry => (entry.id, (int)entry.amount)).ToList() ?? new List<(int, int)>();
        void OnLootViewOpened(ManagedUIElement lootView)
        {
            //设置开启时、无按钮时、存在原始按钮样板，生成复制按钮，绑定功能
            var baseButton = typeof(LootView).GetField("pickAllButton", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(LootView.Instance) as Button;
            if (baseButton == null) return;
            if (buttonRowContainer == null) CreateButtonRowContainer(baseButton);
            if (config.Action_ItemPickAll && pickAllButton == null)
            {
                pickAllButton = CreateButton(baseButton, "Pick All Button", "拾取全部", OnPickAllClick);
                pickAllBtnMethod = typeof(LootView).GetMethod("OnPickAllButtonClicked", BindingFlags.NonPublic | BindingFlags.Instance);
                if (buttonRowContainer != null && pickAllButton != null)
                {
                    pickAllButton.transform.SetParent(buttonRowContainer, false);
                }
            }
            if (config.Action_ItemDecomposeAll && decomposeAllButton == null)
            {
                decomposeAllButton = CreateButton(baseButton, "Decompose All Button", "分解全部", OnDecomposeAllClick);
                if (buttonRowContainer != null && decomposeAllButton != null)
                {
                    decomposeAllButton.transform.SetParent(buttonRowContainer, false);
                    ColorUtility.TryParseHtmlString("#FF9933", out var colorOrange);
                    SetButtonColor(decomposeAllButton, colorOrange);
                }
            }
            if (config.Action_DeleteBox && deleteBoxButton == null)
            {
                deleteBoxButton = CreateButton(baseButton, "Delete Box Button", "移除容器", OnDeleteBoxClick);
                if (buttonRowContainer != null && deleteBoxButton != null)
                {
                    deleteBoxButton.transform.SetParent(buttonRowContainer, false);
                    SetButtonColor(deleteBoxButton, Color.red);
                }
            }
            var inventory = LootView.Instance?.TargetInventory;
            if (inventory != null)
            {
                inventory.onContentChanged -= OnInventoryChanged;
                inventory.onContentChanged += OnInventoryChanged;
            }
            RefreshButton();
        }
        private void OnLootViewClosed(ManagedUIElement lootView)
        {
            var inventory = LootView.Instance?.TargetInventory;
            if (inventory != null) inventory.onContentChanged -= OnInventoryChanged;
        }
        private void OnInventoryChanged(Inventory inventory, int arg2)
        {
            //库存变化时刷新按钮
            var lootView = LootView.Instance;
            var targetInventory = lootView.TargetInventory;
            if (targetInventory == null) return;
            var targetLootBox = TargetLootBox(lootView);
            if (targetLootBox.name != "PlayerStorage" && targetInventory.GetItemCount() < 20) RefreshButton();
        }
        private void OnPickAllClick()
        {
            //原生拾取功能
            //清空执行移除箱子
            var lootView = LootView.Instance;
            var targetInventory = lootView.TargetInventory;
            if (targetInventory == null) return;
            pickAllBtnMethod?.Invoke(lootView, null);
            if (config.Action_DeleteBoxClear && targetInventory.IsEmpty()) OnDeleteBoxClick();
        }
        private static async void OnDecomposeClick(ItemOperationMenu menu, Item item)
        {
            await DecomposeDatabase.Decompose(item, item.StackCount);
            AudioManager.PlayPutItemSFX(item);
            menu.Close();
        }
        private async void OnDecomposeAllClick()
        {
            var lootView = LootView.Instance;
            var inventory = lootView.TargetInventory;
            if (inventory == null) return;
            var targetLootBox = TargetLootBox(lootView);
            var itemsToDecompose = inventory.Where(item => item != null && DecomposeDatabase.CanDecompose(item) && (!targetLootBox.needInspect || item.Inspected)).ToList();
            if (itemsToDecompose.Count <= 0) return;
            foreach (var item in itemsToDecompose)
            {
                await DecomposeDatabase.Decompose(item, item.StackCount);
                AudioManager.PlayPutItemSFX(item);
            }
            RefreshButton();
        }
        private void OnDeleteBoxClick()
        {
            var lootView = LootView.Instance;
            var inventory = lootView.TargetInventory;
            if (inventory == null) return;
            var targetLootBox = TargetLootBox(lootView);
            if (targetLootBox.name == "PlayerStorage")
            {
                CharacterMainControl.Main.PopText("这个箱子不能删！", -1f);
                deleteBoxButton?.gameObject.SetActive(false);
                return;
            }
            targetLootBox.gameObject.SetActive(false);
            if (config.Action_DeleteBoxClear)
            {
                lootView.Close();
                inventory.DestroyAllContent();
            }
            if (config.Action_DeleteBoxAll)
            {
                var boxesToDelete = GetSearchedBoxesInRange(targetLootBox.transform.position, config.DeleteRadius);
                foreach (var box in boxesToDelete) { if (box != null && box.name != "PlayerStorage") box.gameObject.SetActive(false); }
            }
        }
        private List<InteractableLootbox> GetSearchedBoxesInRange(Vector3 centerPos, float radius)
        {
            //搜寻interactable层的InteractableLootbox
            List<InteractableLootbox> result = new List<InteractableLootbox>();
            int interactableLayer = LayerMask.NameToLayer("Interactable");
            if (interactableLayer == -1)
            {
                CharacterMainControl.Main.PopText("【移除空容器mod】未找到Interactable层");
                return result;
            }
            int layerMask = 1 << interactableLayer;
            Collider[] colliders = new Collider[32];
            int numColliders = Physics.OverlapSphereNonAlloc(centerPos, radius, colliders, layerMask, QueryTriggerInteraction.Collide);
            for (int i = 0; i < numColliders; i++)
            {
                Collider collider = colliders[i];
                if (collider == null) continue;
                InteractableLootbox box = collider.GetComponent<InteractableLootbox>();
                if (box != null && box.Looted && box.name != "PlayerStorage" && !result.Contains(box)) result.Add(box);
            }
            return result;
        }
        private void CreateButtonRowContainer(Button baseButton)
        {
            //创建与原始按钮同级的容器
            //水平排布，按钮间距10
            if (baseButton == null) return;
            var modButtonContainer = new GameObject("ModButtonContainer", typeof(RectTransform));
            var originalParent = baseButton.transform.parent as RectTransform;
            if (originalParent == null) return;
            buttonRowContainer = modButtonContainer.GetComponent<RectTransform>();
            buttonRowContainer.SetParent(originalParent, false);
            buttonRowContainer.SetSiblingIndex(baseButton.transform.GetSiblingIndex() + 1);
            buttonRowLayout = modButtonContainer.AddComponent<HorizontalLayoutGroup>();
            buttonRowLayout.spacing = 10f;
        }
        private static Button CreateButton(Button baseButton, string buttonName, string buttonText, UnityAction? onClick = null)
        {
            //复制按钮，清除文本、添加文本，清除功能、添加功能
            var newButton = Instantiate(baseButton, baseButton.transform.parent);
            newButton.name = buttonName;
            var textLocalizor = newButton.GetComponentInChildren<TextLocalizor>();
            if (textLocalizor != null) Destroy(textLocalizor);
            var textComponent = newButton.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null && !string.IsNullOrEmpty(buttonText)) textComponent.text = buttonText;
            if (onClick != null)
            {
                newButton.onClick.RemoveAllListeners();
                newButton.onClick.AddListener(onClick);
            }
            return newButton;
        }
        private static Button CreateDecomposeButton(Button baseButton) => CreateButton(baseButton, "Decompose Button", "分解");
        private void SetButtonColor(Button button, Color color)
        {
            if (button == null) return;
            var colors = button.colors;
            colors.normalColor = color;
            colors.highlightedColor = Color.Lerp(color, Color.white, 0.3f);
            colors.pressedColor = Color.Lerp(color, Color.black, 0.3f);
            colors.selectedColor = color;
            button.colors = colors;
        }
        private void RefreshButton()
        {
            //为玩家储物箱时移除按钮
            var lootView = LootView.Instance;
            var inventory = lootView.TargetInventory;
            var targetLootBox = TargetLootBox(lootView);
            if (inventory == null) return;
            if (targetLootBox.name == "PlayerStorage")
            {
                pickAllButton?.gameObject.SetActive(false);
                deleteBoxButton?.gameObject.SetActive(false);
                decomposeAllButton?.gameObject.SetActive(false);
                buttonRowContainer?.gameObject.SetActive(false);
                return;
            }
            //设置关闭时移除按钮
            //判断是否物品数量大于0，存在可分解物品
            //拾取分解移除固定顺序
            int buttonCount = 0;
            if (config.Action_DeleteBox && deleteBoxButton != null) buttonCount++;
            if (config.Action_ItemPickAll && pickAllButton != null) buttonCount++;
            if (config.Action_ItemDecomposeAll && decomposeAllButton != null) buttonCount++;
            buttonRowContainer?.gameObject.SetActive(buttonCount > 0);
            if (!config.Action_DeleteBox && deleteBoxButton != null)
            {
                Destroy(deleteBoxButton?.gameObject);
                deleteBoxButton = null;
            }
            if (!config.Action_ItemPickAll && pickAllButton != null)
            {
                Destroy(pickAllButton?.gameObject);
                pickAllButton = null;
            }
            if (!config.Action_ItemDecomposeAll && decomposeAllButton != null)
            {
                Destroy(decomposeAllButton?.gameObject);
                decomposeAllButton = null;
            }
            deleteBoxButton?.gameObject.SetActive(inventory != null);
            if (pickAllButton != null)
            {
                pickAllButton.gameObject.SetActive(inventory != null);
                pickAllButton.interactable = inventory?.GetItemCount() > 0;
            }
            if (decomposeAllButton != null)
            {
                decomposeAllButton.gameObject.SetActive(inventory != null);
                decomposeAllButton.interactable = inventory?.Any(item => item != null && DecomposeDatabase.CanDecompose(item)) == true;
            }
            if (buttonRowContainer != null)
            {
                pickAllButton?.transform.SetAsFirstSibling();
                if (decomposeAllButton != null)
                {
                    if (pickAllButton != null) decomposeAllButton.transform.SetSiblingIndex(1);
                    else decomposeAllButton.transform.SetAsFirstSibling();
                }
                deleteBoxButton?.transform.SetAsLastSibling();
            }
        }
        private static IEnumerator DelayedRefresh(Inventory targetInventory)
        {
            yield return new WaitForEndOfFrame();
            var targetLootBox = TargetLootBox(LootView.Instance);
            if (targetLootBox.name != "PlayerStorage" && targetInventory.GetItemCount() < 20) FindObjectOfType<ModBehaviour>()?.RefreshButton();
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(LootView), "RefreshPickAllButton")]
        private static void Postfix_RefreshPickAllButton()
        {
            //右键时刷新内置按钮时刷新mod按钮
            //延后获取库存数目
            var lootView = LootView.Instance;
            if (lootView == null) return;
            var targetInventory = lootView.TargetInventory;
            if (targetInventory == null) return;
            lootView.StartCoroutine(DelayedRefresh(targetInventory));
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ItemOperationMenu), "Setup")]
        private static void Postfix_Setup(ItemOperationMenu __instance, ItemDisplay ___TargetDisplay, Button ___btn_Split)
        {
            //复制拆分按钮，添加分解功能
            if (!config.Action_ItemDecompose)
            {
                decomposeButton?.gameObject.SetActive(false);
                return;
            }
            if (decomposeButton == null)
            {
                decomposeButton = CreateDecomposeButton(___btn_Split ?? __instance.GetComponentInChildren<Button>());
                if (decomposeButton == null) return;
            }
            var item = ___TargetDisplay?.Target;
            if (item == null)
            {
                decomposeButton.gameObject.SetActive(false);
                return;
            }
            var canDecompose = DecomposeDatabase.CanDecompose(item);
            decomposeButton.gameObject.SetActive(canDecompose);
            if (canDecompose)
            {
                decomposeButton.onClick.RemoveAllListeners();
                decomposeButton.onClick.AddListener(() => OnDecomposeClick(__instance, item));
            }
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(InteractableLootbox), "CheckHideIfEmpty")]
        private static bool Prefix_CheckHideIfEmpty(InteractableLootbox __instance)
        {
            if (__instance.name == "PlayerStorage") return true;
            Debug.Log("【移除空容器mod】执行：Prefix");
            try
            {
                //无物品时移除
                //存在标记、图纸、钥匙、任务、现金时不移除
                //所有物品均小于阈值时移除
                //最高价值物品为武器护甲但小于十倍阈值时，没有非武器护甲的物品或其不超过阈值时移除
                if (__instance.Inventory.IsEmpty()) __instance.gameObject.SetActive(false);
                else if (config.Action_ItemValueCheck)
                {
                    var importantTags = new[] { "Formula", "Key", "Quest", "Cash" };
                    var items = __instance.Inventory.Where(item => item != null).ToList();
                    foreach (var item in items)
                    {
                        if (ItemWishlist.Instance.IsManuallyWishlisted(item.TypeID))
                        {
                            CharacterMainControl.Main.PopText("这里面还有我需要的东西没拿", -1f);
                            return true;
                        }
                        if (importantTags.Any(tag => item.Tags.Contains(tag)))
                        {
                            if (item.Tags.Contains("Key")) CharacterMainControl.Main.PopText("这里面还有钥匙没拿", -1f);
                            else if (item.Tags.Contains("Formula")) CharacterMainControl.Main.PopText("这里面还有图纸没拿", -1f);
                            else if (item.Tags.Contains("Quest")) CharacterMainControl.Main.PopText("这里面还有任务道具没拿", -1f);
                            return true;
                        }
                    }
                    var sortedItems = items.OrderByDescending(item => item.GetTotalRawValue()).ToList();
                    var highestValueItem = sortedItems.First();
                    Debug.Log($"【移除空容器mod】执行：容器内不存在白名单。最高价值物品：{highestValueItem.GetTotalRawValue()}，阈值：{config.ValueThreshold}");
                    if (highestValueItem.GetTotalRawValue() / 2 < config.ValueThreshold) __instance.gameObject.SetActive(false);
                    else if (highestValueItem.GetTotalRawValue() / 20 < config.ValueThreshold && (highestValueItem.Tags.Contains("Weapon") || highestValueItem.Tags.Contains("Armor")))
                    {
                        var firstNonWeaponOrArmorItem = sortedItems.FirstOrDefault(item => !item.Tags.Contains("Weapon") && !item.Tags.Contains("Armor"));
                        if (firstNonWeaponOrArmorItem == null || firstNonWeaponOrArmorItem.GetTotalRawValue() / 2 < config.ValueThreshold) __instance.gameObject.SetActive(false);
                    }
                }
                Debug.Log("【移除空容器mod】成功");
            }
            catch
            {
                CharacterMainControl.Main.PopText("【移除空容器mod】失败：try-catch存在问题");
            }
            return true;//继续执行原代码
        }
    }
}