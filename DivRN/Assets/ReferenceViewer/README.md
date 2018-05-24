# ReferenceViewer

A small Unity tool to aid finding references to your GameObjects or Components in the scene.

### How to use this tool

1. Right-click any GameObject, Components, prefab, or other asset.
2. Click "Reference Viewer" in the context-menu.
3. If you selected a project asset, you can choose to look for references in either the current open scenes, or in the project.
4. Click on any found reference to immediately navigate to it in the scene or the asset browser.

### Notes

Searching for references through all assets can be quite slow if your project contains many assets.
All assets need to be loaded in memory to be able to search through them. This can consume a lot of RAM, so use with care.
A caching system is used so searching for references the second time will always be much faster than the first time.

### Asset Store

This tool is also available on the Unity Asset Store [here](https://www.assetstore.unity3d.com/en/#!/content/73961). You can purchase it there if you wish to support me.
