from __future__ import annotations
from .level_data import level_data, level_names, game_items
from typing import TYPE_CHECKING
from BaseClasses import Item, ItemClassification
import random

if TYPE_CHECKING:
    from .world import ChipsChallengeTestWorld

class ChipsChallengeTestItem(Item):
    game = "Chip's Challenge Test"

base_levels:dict[str, ChipsChallengeTestItem] = {}

all_items:dict[str, ChipsChallengeTestItem] = {}

extra_items:dict[str, int] = {
    "Single-Use Red Key": 2,
    "Single-Use Blue Key": 3,
    "Single-Use Yellow Key": 4,
    "Single-Use Green Key": 5,
    "Single-Use Ice Skates": 6,
    "Single-Use Fire Boots": 7,
    "Single-Use Suction Boots": 8,
    "Single-Use Flippers": 9,
    "Time Bonus": 10,
    "Single-Use Helmet": 11,
    "Single-Use Secret Eye": 12,
    "Big Map": 13
}

trap_items:dict[str, int] = {
    "Time Penalty": 100,
    "Teeth": 101,
    "Walker": 102,
    "Blob": 103,
    "Confusion": 104,
    "Small Map": 105
}

item_name_to_id:dict[str, int] = { key: value for key, value in {**level_names, **game_items, **extra_items, **trap_items, **{"Filler": 1}}.items() }

def create_item(world:ChipsChallengeTestWorld, item_name:str, classification:ItemClassification, item_id:int) -> ChipsChallengeTestItem:
    item_from_table = all_items.get(item_name)
    return ChipsChallengeTestItem(item_name, item_from_table.classification, item_id, world.player)

def create_all_items(world:ChipsChallengeTestWorld) -> None:
    items:list[Item] = []

    ## Add level unlock items
    for level in level_data["levels"]:
        level_name = f"Map {level["level_number"]} - {level["level_name"]}"
        cc_item:ChipsChallengeTestItem = ChipsChallengeTestItem(level_name, ItemClassification.progression, item_name_to_id[level_name], world.player)
        all_items[level_name] = cc_item
        item_name_to_id[cc_item.name] = cc_item.code
        item = world.create_item(level_name)
        items.append(item)
        level_required_items:list[str] = level["items_required"]
        ## Add level to base item pool if it does not require game items to complete
        if len(level_required_items) == 0:
            base_levels[level_name] = cc_item

    ## Add key and boot items
    game_items = ["Blue Key", "Green Key", "Red Key", "Yellow Key", "Fire Boots", "Flippers", "Ice Skates", "Suction Boots"]
    for game_item in game_items:
        cc_item:ChipsChallengeTestItem = ChipsChallengeTestItem(game_item, ItemClassification.progression, item_name_to_id[game_item], world.player)
        all_items[game_item] = cc_item
        item_name_to_id[cc_item.name] = cc_item.code
        item = world.create_item(game_item)
        items.append(item)

    ## Add extra items
    for extra_item in extra_items.keys():
        cc_item: ChipsChallengeTestItem = ChipsChallengeTestItem(extra_item, ItemClassification.useful, item_name_to_id[extra_item], world.player)
        all_items[extra_item] = cc_item
        item_name_to_id[cc_item.name] = cc_item.code

    ## Add trap items
    for trap_item in trap_items.keys():
        cc_item: ChipsChallengeTestItem = ChipsChallengeTestItem(trap_item, ItemClassification.trap, item_name_to_id[trap_item], world.player)
        all_items[trap_item] = cc_item
        item_name_to_id[cc_item.name] = cc_item.code

    ## Add filler item
    filler_item:ChipsChallengeTestItem = ChipsChallengeTestItem("Filler", ItemClassification.filler, item_name_to_id["Filler"], world.player)
    all_items["Filler"] = filler_item
    item_name_to_id[filler_item.name] = filler_item.code

    num_items:int = len(items)
    num_unfilled_locations:int = len(world.multiworld.get_unfilled_locations(world.player))
    if num_unfilled_locations > 0:
        if world.options.proportion_of_filler_as_useful_items > 0:
            num_useful_items:int = int(num_unfilled_locations * world.options.proportion_of_filler_as_useful_items / 100.0)
            for _ in range(num_useful_items):
                item_name:str = random.choice(list(extra_items.keys()))
                items.append(world.create_item(item_name))
            num_unfilled_locations -= num_useful_items
        if world.options.proportion_of_filler_as_traps > 0:
            num_trap_items:int = int(num_unfilled_locations * world.options.proportion_of_filler_as_traps / 100.0)
            for _ in range(num_trap_items):
                item_name:str = random.choice(list(trap_items.keys()))
                items.append(world.create_item(item_name))
            num_unfilled_locations -= num_trap_items
        if num_unfilled_locations > 0:
            print(f"Adding filler for {num_unfilled_locations} location(s)")
            filler_item_count:int = num_unfilled_locations - num_items
            items += [world.create_filler() for _ in range(filler_item_count)]

    world.multiworld.itempool += items
