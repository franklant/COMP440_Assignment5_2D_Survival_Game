# COMP440_Assignment5_2D_Survival_Game
The goal is to create a complete game by building four inter-connected game mechanics. Each team member is responsible for one game mechanic and its complete implementation/integration with other members systems.

# Members and Roles
- Emmanuel Lemi: **Recipe-based Crafting Engine**
> Define recipes with multiple ingredients. Implement crafting station requirements (campfire, workbench). Show craftable recipes vs. locked recipes. Handle resource consumption. Crafting takes time. Create 10+ useful items.
- Sean Crosdale: **Recource Gathering System**
> Players interact with resource nodes (trees, rocks, plants). Different tools required for different resources. Resources have health (multiple hits to gather). Implement node respawn timers. Inventory has weight/space limits.
- Mamudu Cole: **Survival Status System**
> Track hunger, thirst, health, temperature. Each degrades over time at different rates. Eating/drinking restores stats. Taking damage decreases health. Temperature affected by time of day. Death resets progress.
- Franklan Taylor: **Day/Night Cycle Controller**
> Time continuously advances (1 day = 5 real minutes). Sun position affects lighting. Different enemies spawn at night. The temperature drops at night. Certain crafting only works at specific times. Visual indicators for time of day.

# Additional Constraint
***Time constantly advances*** - day/night cycle affects all systems (gathering, crafting, survival needs)
