【功能mod】自动清除空容器 | Remove Empty Box v1.2.5.1


[h1]自动清除空容器[/h1]
[h2]此为v1.2.5.1版本[/h2]
[h3]直接订阅即可[/h3]

[hr][/hr]
[h2]使用说明：[/h2][list]
[*][h3]基础功能：在搜寻容器后，若容器内已无物品，则自动移除当前容器[/h3]
[*]可以适当减轻电脑加载压力，在多个尸体处于同一位置时快速的处理上层尸体而无需搬运，以及方便在回程时无需担忧哪个箱子还遗漏了东西
[*][h3]额外功能（默认开启）：[/h3][list]
[*]物品价值信息：在物品信息中显示该组物品的售卖价值
[*]物品分解信息：在物品信息中显示该组物品的分解产物
[*]其他mod也可能已经存在此种功能从而导致在物品信息中出现一样的信息，若如此可以在设置中关闭该项信息显示功能[/list]
[*][h3]高级功能（默认关闭）：[/h3][list]
[*]右键分解功能：右键物品时若可被分解则出现分解按钮，点击后分解该组物品
[*]一键分解功能：在战利品页面添加“分解全部”按钮，点击后分解容器内所有可被分解的物品
[*]一键拾取功能：在战利品页面添加“拾取全部”按钮，点击后拾取容器内所有物品，若已使用各种一键拾取mod可在设置中关闭该项功能
[*]一键删除功能：在战利品页面添加“移除容器”按钮，点击后直接移除当前容器，即使它仍满足白名单，但不会直接关闭页面，请在关闭前确认是否已拾取所需物品
[*]清空删除功能：在点击“拾取全部”按钮后，若容器已清空则会“移除容器”；在点击“移除容器”按钮后，会直接清空容器并关闭页面
[*]范围删除功能：在点击“移除容器”按钮后，会移除以此容器为中心的范围内所有已搜寻过的容器。可自定义范围大小，范围为1-20
[*]价值检测功能：若容器内物品最高价值低于阈值，则自动移除容器。可自定义阈值大小，范围为0-2000[/list]
[*]若需要打开以上功能，在加载mod后，可以去到游戏本体目录中G:\Steam\steamapps\common\Escape from Duckov\ModConfigs\xuanxing_Mod_RemoveEmptyBox，修改ModConfig文件，将各个false改为true，重启游戏后生效
[*]此mod也支持了“ModConfig” mod，在同时订阅了“ModConfig”后可以在游戏过程中，在设置页面中实时的打开关闭各种功能和拖动或输入检测数值，而不必重启游戏
[*]不仅仅是在搜寻容器后，在进入地图时，若生成的容器内物品最高价值低于阈值，则此低价值容器不会生成，你所能找到的容器中至少存在一个符合价值或标签要求的物品（仓库区的物资箱目前无法执行该条功能）
[*]对标签为图纸、钥匙、任务、现金的物品做了特殊处理，在容器内存在这些物品时不会自动移除容器
[*]对标签为武器、护甲的物品做了特殊处理，在容器内存在这些物品时其判断标准为阈值的十倍，以避免容器内只存在武器和护甲时但因其价值较高所以不移除容器的情况
[*]对已标记的物品做了特殊处理，在容器内存在这些物品时不会自动移除容器，按N键或右键点击标记以标记物品
[*]虽然看起来与“所有物资箱激活”mod相冲突，但实际可以同时使用，具体表现为所有容器生成后其中的低价值容器被移除
[*]可以很好的与各种一键拾取和自动拾取mod一同使用[/list]

[hr][/hr]
[h3]v1.2.5.1更新日志：[/h3][list]
[*]修复了无法使用制作台与自提柜的问题和无法提交任务道具的问题
[*]修复了在整理仓库时的严重卡顿问题
[*]当容器内多于20个物品时暂停按钮的对库存的监听以应对超大容量mod箱子的整理卡顿问题
[*]增加了清空删除功能，可以在点击“拾取全部”和“移除容器”按钮后，直接移除容器并关闭页面
[*]没有实体的容器也可以使用“移除容器”功能，以清空内部物品
[*]对于所提及的“自动收集机器人”mod，它目前所能兼容的功能有：[list]
[*][h3]在自动拾取容器内物品后直接移除容器[/h3]
[*]多于20个物品时整理不会造成卡顿
[*]能够正常的点击“移除容器”按钮用来清空机器人内的物品[/list][/list]

[hr][/hr]
[h3]已发现的问题：[/h3][list]
[*]仓库区的地图加载逻辑异常，你将会发现地图上仍然出现了低于价格阈值的容器，并且容器在清空或低于价格阈值时不会立即消失而是在第二次打开时才会消失
[*]但仓库区的敌人战利品不受影响，其余区域的物资箱与战利品均无此问题[/list]

[hr][/hr]
若出现任何问题或任何想添加的功能与想法均可留言反馈

玩的开心请点个赞哦~只有更多的评价才能让更多人看到哦~


[h1]Remove Empty Box[/h1]
[h2]Version 1.2.5.1[/h2]
[h3]Simply subscribe to enable[/h3]

[hr][/hr]
[h2]Usage Instructions:[/h2][list]
[*][h3]Basic Function: After searching a container, if the container is empty, it will be automatically removed.[/h3]
[*]This can appropriately reduce computer load pressure, quickly process stacked corpses without moving them, and make it convenient not to worry about which boxes still have leftover items on the return trip.
[*][h3]Additional Functions:[/h3][list]
[*]Item Value Information: Displays the sale value of the item stack in the item information.
[*]Item Disassembly Information: Displays the disassembly products of the item stack in the item information.
[*]Other mods might already have this functionality, potentially causing duplicate information to appear in the item info. If so, you can disable this information display in the settings.[/list]
[*][h3]Advanced Functions:[/h3][list]
[*]Right-Click Disassemble Function: Right-clicking an item will show a "Disassemble" button if the item can be disassembled. Click it to disassemble the entire stack.
[*]One-Click Disassemble Function: Adds a "Disassemble All" button to the Loot screen. Click it to disassemble all disassemblable items in the container.
[*]One-Click Loot Function: Adds a "Loot All" button to the Loot screen. Click it to loot all items in the container. If you already use various one-click loot mods, you can disable this function in the settings.
[*]One-Click Delete Function: Adds a "Remove Container" button to the Loot screen. Click it to directly remove the current container, even if it still meets the whitelist criteria. However, the screen won't close automatically; please confirm you have looted the desired items before closing.
[*]Clear and Delete Function: After clicking the "Loot All" button, if the container has been cleared, it will be "Remove Container"; After clicking the 'Remove Container' button, the container will be emptied directly and the page will be closed.
[*]Range Delete Function: After clicking the "Remove Container" button, all searched containers within a customizable range (1-20) centered on this container will be removed.
[*]Value Check Function: Automatically removes the container if the highest value of the items inside is below a customizable threshold (0-2000).[/list]
[*]To enable the above functions, after loading the mod, go to the game's main directory G:\Steam\steamapps\common\Escape from Duckov\ModConfigs\xuanxing_Mod_RemoveEmptyBox, modify the ModConfig file, change the respective 'false' values to 'true', and restart the game for the changes to take effect.
[*]This mod also supports the "ModConfig" mod. If you have also subscribed to "ModConfig", you can enable/disable various functions in real-time and drag or input detection values on the settings page during the game without restarting.[/list]




[*]Not only after searching containers but also when entering a map, if the highest-value item in a generated container is below the threshold, the low-value container will not spawn. The containers you find will contain at least one item that meets the value or tag requirements.
[*]Special handling is applied to items with the tags: Formula, Key, Quest, Cash. Containers will not be removed if they contain such items.
[*]Special handling is applied to items with the tags: Weapon, Armor. If a container contains such items, the judgment standard is ten times the threshold. This avoids situations where containers are not removed simply because they contain high-value weapons or armor.
[*]Special handling is applied to marked items. Containers containing these items will not be automatically removed. Press the 'N' key or right-click to mark items.
[*]Although it may seem to conflict with the "MoreLootBox" mod, they can actually be used together. The effect is that after all containers are generated, the low-value ones among them are removed.
[*]Can be used well with various one-click loot mods.[/list]

[hr][/hr]
[h3]v1.2.5 Update Log:[/h3][list]
[*]Removed whitelist protection for items tagged as Food and Special.
[*]Added whitelist protection for marked items.
[*]Added the total value and disassembly information for the entire item stack in the detailed item information.
[*]Added a "Disassemble" button to the item right-click menu.
[*]Added "Loot All", "Disassemble All", and "Remove Container" buttons to the item looting screen.
[*]Container removal can now be set individually to remove only the current container or all containers within a range.[/list]

[hr][/hr]
[h3]Known Issues:[/h3][list]
[*]The map loading logic for the Warehouse area is abnormal. You will find that containers below the value threshold still appear on the map, and containers that are emptied or fall below the value threshold do not disappear immediately but only disappear upon being opened a second time.
[*]However, enemy loot in the Warehouse area is unaffected. Loot containers and loot in other areas do not have this issue.[/list]

[hr][/hr]
Report issues or suggest features via comments.

"Enjoying the mod? Leave a thumbs-up!"
More ratings help more players discover it!