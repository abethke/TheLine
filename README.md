Unity 2022.3.26f1

This project was created by Alex Bethke as part of a take-home technical test for a job interview. 
The assignment was to clone this game minus the 6x paths, river jump, and music/sfx: https://apps.apple.com/us/app/the-line/id880338977


- Project is configured to support multiple screen resolutions on a portrait layout with all game features scaling relationally
- Project is configured for Portrait locking on iOS
- Landscape was deemed out of scope and not addressed. If I included landscape support I would aspect ratio lock the game into portrait based on screen dimensions and black gutter the excess sides of the app rather than continue with the current asset scaling relational values approach.
- Localization solutions were not implemented.
- Folder structure is using a specific naming schema. This is a style choice but also future proofs projects against the inclusion of plugins and asset store packages so the core project files stay grouped together and are quickly findable.
- Top bar menu items were positioned to stop collision with iPhone camera. Normally I would add special handling to move these elements based on device type checking rather than add the gutter space.


Original development log and technical test could are on a branch. 
