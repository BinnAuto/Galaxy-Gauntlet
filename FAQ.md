# FAQ / Common Issues

### I beat all the levels in my AP, but the game didn't release.

The game client currently does not recognize a win condition, **and therefore cannot release**. Beating all levels first try (No Resets) will involve reaching every check, effectively releasing the game.

### I'm getting a warning message trying to connect to Archipelago.

This warning message can happen if the set of levels in your ``levels`` folder does not exactly match the ones used to generate the AP World (levels 1 to 10). This message will appear even if you have extra levels. You can continue, but if the levels truly do not match then you run the risk of sending AP items that don't exist; or worse, you could miss AP checks entirely.

### Not all my levels appear when I enter Quick Play. / Not all my levels were included when generating an AP World.

Galaxy Gauntlet and the AP World generator tool both only look at levels named ``map###.c2m`` and loads them in order, starting at ``map001.c2m``. If a level is missing in the sequence, the tool will stop looking for level files and generate using the ones it has. For example, if the folder contains levels ``map001.c2m`` through ``map010.c2m``, and skips to ``map012.c2m``, only levels 1 to 10 will be loaded.
