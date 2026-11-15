from __future__ import annotations
from typing import TYPE_CHECKING
from rule_builder.rules import HasAll, CanReachLocation
from .level_data import level_data

if TYPE_CHECKING:
    from .world import ChipsChallengeTestWorld

def set_all_rules(world: ChipsChallengeTestWorld) -> None:
    set_entrance_rules(world)
    set_location_rules(world)
    set_completion_condition(world)

def set_entrance_rules(world: ChipsChallengeTestWorld) -> None:
    for level in level_data["levels"]:
        level_name = f"Map {level["level_number"]} - {level["level_name"]}"
        items_required:list[str] = level["items_required"]
        items_required.append(level_name)
        entrance_rule = HasAll(*items_required)
        main_to_level = world.get_entrance(f"Main to {level_name}")
        world.set_rule(main_to_level, entrance_rule)

def set_location_rules(world: ChipsChallengeTestWorld) -> None:
    pass

def set_completion_condition(world: ChipsChallengeTestWorld) -> None:
    rule:CanReachLocation = CanReachLocation("Complete Map 1 - LESSON 1")
    for level in level_data["levels"]:
        location_name = f"Complete Map {level["level_number"]} - {level["level_name"]}"
        if location_name != "Complete Map 1 - LESSON 1":
            rule = rule & CanReachLocation(location_name)
    world.set_completion_rule(rule)
