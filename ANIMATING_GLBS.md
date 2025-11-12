# Animating GLB Models in Unity (for DFA)

This guide explains how to take an imported `.glb` that already contains animation clips (for example `Clip_0`) and wire it up so the animation plays automatically when the prefab is spawned by `AprilTagModelCatalog`.

---

## 1. Import the GLB

1. Drag the `.glb` into your Unity Project (e.g. `Assets/Models/`).
2. Select the GLB in the Project window.  
   - If you’re using the glTF importer, the Inspector shows **GLTF Settings** and **Imported Object** (no Model/Rig/Animation tabs). That’s expected.

---

## 2. Verify the Animation Clip

1. In the Project window, click the disclosure triangle next to the GLB.
2. You’ll see sub-assets:
   - `Imported Object` – the prefab Unity generated.
   - `Clip_0`, `Clip_1`, etc. – animation clips.
3. Select `Clip_0` to open its settings in the Inspector.
   - Enable **Loop Time** if you want it to repeat.
   - Click **Apply** to save changes.

---

## 3. Instantiate the GLB and Create a Prefab Wrapper

1. Drag the GLB’s `Imported Object` into the Scene or Hierarchy.  
   Unity creates an instance with mesh renderers, an Animator component, and materials.
2. (Optional) Add any wrapper objects you need (e.g., `SpinRoot` with `TagModelSpinController`).
3. Drag the resulting GameObject hierarchy from the Hierarchy back into the Project window to create a prefab.  
   - This prefab is what you’ll reference in `AprilTagModelCatalog`.

---

## 4. Create & Assign an Animator Controller

If the Animator component on your prefab shows **Controller: None (Runtime Animator Controller)**, create one:

1. **Create a controller asset**  
   - In the Project window (for example under `Assets/Animations`), right-click → **Create → Animator Controller**.  
   - Name it something like `PricklyPearController`, `GhostDogController`, etc.
2. **Assign it to the Animator**  
   - Select your prefab instance (or the prefab in Prefab Mode).  
   - In the Animator component, drag the new controller asset into the `Controller` field (replacing “None”).
3. **Open the controller in the Animator window**  
   - Double-click the controller asset (or click the small target icon next to the field and choose **Open**).  
   - Unity opens an empty graph labeled **Entry**.

---

## 5. Add the Clip to the Controller

1. **Drag the clip into the graph**  
   - In the Project window, expand your GLB and locate `Clip_0`.  
   - Drag `Clip_0` into the Animator window. Unity creates a state box that uses the clip.
2. **Set it as the default**  
   - Right-click the new state → **Set as Layer Default State**. The box turns orange, meaning it runs automatically.
3. **Configure the clip if needed**  
   - Click `Clip_0` in the Project window. In the clip Inspector, tick **Loop Time** if you want it to keep spinning, then click **Apply**.

---

## 6. Apply Prefab Changes

If you edited an instance in the scene:

1. Select the root of the prefab instance in the Hierarchy.
2. In the Inspector, click **Apply** (top of the Inspector) or right-click → **Prefab → Apply All**.  
   This writes the Animator + controller setup back to the prefab asset.

---

## 7. Reference in `AprilTagModelCatalog`

1. Open `AprilTagModelCatalog.asset`.
2. For the relevant tag entry:
   - Set `Model Prefab` to the prefab you just configured.
   - (Optional) Enable spin and set RPM/ramp values.
3. Save the asset.

When AR Foundation detects the tag:
- `SimpleImageTracking` instantiates the prefab.
- The Animator finds the controller you set.
- The default state plays `Clip_0` automatically.
- If you added `TagModelSpinController`, its parameters are applied from the catalog.

---

## Troubleshooting

- **Why you don’t see the Animation tab:**  
  The glTF importer (GLTF Settings UI) replaces Unity’s default FBX-style inspector, so the Model/Rig/Animation tabs don’t appear. The clip assets are still available as sub-assets under the GLB—select them directly in the Project window to edit loop settings or properties. If you prefer the classic importer UI, convert the asset to FBX or use Unity’s built-in FBX importer, but it’s not required; you can configure everything using the clip sub-assets.

- **Animation doesn’t play:**  
  Check the Animator’s `Controller` field (must not be `None`) and ensure `Clip_0` is the orange default state.

- **Clip doesn’t loop:**  
  Select the clip asset and enable **Loop Time**.

- **Spin doesn’t work:**  
  Attach `TagModelSpinController` to the prefab root (or wrapper) and configure spin in `AprilTagModelCatalog`.

- **Imported object missing Animator:**  
  Some GLBs ship without an Animator. Add one manually, create a controller, and wire the clip as described above.

- **Controller field shows `None (Runtime Animator Controller)`:**  
  Unity hasn’t been told which state machine to run. Create a new **Animator Controller** (e.g., `PricklyPearController`), assign it to the prefab’s Animator component, open the controller in the Animator window, drag `Clip_0` into the graph, set it as the default state, optionally enable **Loop Time** on the clip, and apply the prefab changes. Once that’s done, the animation plays automatically when the prefab instantiates.

---

This workflow ensures every GLB-based prefab you add to `AprilTagModelCatalog` plays its embedded animation automatically and can be combined with runtime spin control.


