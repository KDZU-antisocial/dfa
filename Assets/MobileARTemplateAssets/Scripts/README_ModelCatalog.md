# AprilTag Model Catalog - Setup Guide

This guide explains how to use the Model Catalog system to map different AprilTags to different 3D models.

---

## 🎯 Overview

The **AprilTag Model Catalog** allows you to associate each AprilTag with a specific 3D model and descriptive name.

**Example:**
```
AprilTag "0" → "Leaping Coyote" → CoyoteModel.prefab
AprilTag "1" → "Running Fox" → FoxModel.prefab
AprilTag "2" → "Soaring Eagle" → EagleModel.prefab
...
AprilTag "99" → "Swimming Otter" → OtterModel.prefab
```

When the camera detects AprilTag "0", it spawns the Coyote model. When it detects "1", it spawns the Fox, and so on.

---

## 🏷️ **AprilTag Information**

### **Tag Family Used**

This project uses **`tagStandard41h12`** AprilTags.

- **Available IDs:** 0 to 2,114 (2,115 total unique tags)
- **Generation Tool:** [https://chaitanyantr.github.io/apriltag.html](https://chaitanyantr.github.io/apriltag.html)
- **Recommended Size:** 90mm (0.09 meters)

### **Generating AprilTags**

1. Visit: [https://chaitanyantr.github.io/apriltag.html](https://chaitanyantr.github.io/apriltag.html)
2. Select **Tag Family:** `tagStandard41h12`
3. Enter **Tag ID:** `0`, `1`, `2`, ... up to `2114`
4. Set **Total Size:** `90mm`
5. Download as **SVG** or **PDF**
6. Print at 100% scale (verify with ruler!)

### **Why tagStandard41h12?**

- ✅ 2,115 unique IDs (perfect for large catalogs)
- ✅ Good balance between size and error correction
- ✅ Reliable detection in AR Foundation
- ✅ Industry-standard format

---

## 📦 **Step-by-Step Setup**

### **Step 1: Create the Catalog Asset**

1. In Unity, navigate to your `Assets` folder
2. Right-click in the Project window
3. Select **Create → AR → AprilTag Model Catalog**
4. Name it: `AprilTagModelCatalog` (or your preferred name)
5. The catalog asset is created and ready to configure

---

### **Step 2: Add Model Mappings**

1. **Select** the catalog asset you just created
2. **In the Inspector**, you'll see:
   ```
   Model Mappings:
     Size: 0
   Fallback:
     Default Model: None
   ```

3. **Set the Size** to how many mappings you want (e.g., `100`)
   - Click the number next to "Size"
   - Type `100` and press Enter
   - 100 empty mapping slots will appear

4. **Fill in each mapping:**
   ```
   Element 0:
     AprilTag Info:
       April Tag Name: "0"
       Descriptive Name: "Leaping Coyote"
     Model:
       Model Prefab: [Drag your CoyoteModel prefab here]
     Optional Settings:
       Enabled: ✓
       Custom Offset: (0, 0.1, 0)     ← 10cm above tag (default)
       Custom Rotation: (0, 0, 0)
       Custom Scale: 1                 ← Normal size (default)
     Metadata:
       Notes: "A coyote in mid-leap pose"
   
   Element 1:
     AprilTag Info:
       April Tag Name: "1"
       Descriptive Name: "Running Fox"
     Model:
       Model Prefab: [Drag your FoxModel prefab here]
     Optional Settings:
       Enabled: ✓
       Custom Offset: (0, 0.1, 0)     ← Defaults are already set!
       Custom Scale: 1
       ...
   ```

5. **Repeat** for all your models (or add incrementally)

---

### **Step 3: Assign Catalog to SimpleImageTracking**

1. In your scene, select the **XR Origin (AR Rig)** GameObject
2. Find the **SimpleImageTracking** component in the Inspector
3. Find the **"Model Catalog"** field
4. **Drag** your `AprilTagModelCatalog` asset into this field
5. The catalog is now linked!

---

### **Step 4: Test with a Few Tags First**

**Start Small:**
```
1. Create catalog with just 3-5 mappings initially
2. Test detection and model spawning
3. Verify correct models appear for each tag
4. Add more mappings incrementally
```

**Example Test Setup:**
```
Mapping 0: "0" → "Test Cube" → CubePrefab
Mapping 1: "1" → "Test Sphere" → SpherePrefab
Mapping 2: "2" → "Test Cylinder" → CylinderPrefab
```

Point camera at AprilTag 0 → Cube appears  
Point camera at AprilTag 1 → Sphere appears  
Point camera at AprilTag 2 → Cylinder appears

---

## 🎨 **Advanced Features**

### **Custom Transform Per Model**

Each mapping can have its own custom offset, rotation, and scale:

```
Example: "Leaping Coyote" needs to be higher above tag
  Custom Offset: (0, 0.5, 0)  ← 0.5m above tag instead of default 0.3m
  Custom Rotation: (0, 45, 0) ← Rotated 45° on Y axis
  Custom Scale: 1.5           ← 50% larger than default
```

**When to use:**
- Different models need different heights
- Some models need rotation correction
- Models vary in size

**If left at defaults:**
- Uses the default settings from `SimpleImageTracking` component
- Keeps all models consistent

---

### **Fallback/Default Model**

Set a **Default Model** in the catalog for tags without mappings:

```
Catalog Settings:
  Fallback:
    Default Model: GenericModel.prefab
```

**Behavior:**
- Tag "0" mapped → Spawns "Leaping Coyote" model ✅
- Tag "99" not mapped → Spawns GenericModel (fallback) ⚠️
- Tag "50" disabled → Spawns GenericModel (fallback) ⚠️

**Without default model:**
- Unmapped tags won't spawn anything (warning logged)

---

### **Enable/Disable Individual Mappings**

**Use the "Enabled" checkbox** to temporarily disable mappings:

```
Element 5:
  April Tag Name: "5"
  Descriptive Name: "Broken Model"
  Model Prefab: BrokenPrefab
  Enabled: ☐  ← Unchecked = disabled
```

**When disabled:**
- Tag will be detected
- No model will spawn
- Falls back to default model (if set)
- Useful for testing or temporarily removing models

---

### **Notes/Metadata**

Use the **Notes** field to document each mapping:

```
Notes: "Large model - may need custom scale. 
       Created for habitat diorama #3.
       Last updated: Oct 2025"
```

**Helpful for:**
- Team collaboration
- Tracking model sources
- Special requirements
- Update history

---

## 📊 **Catalog Validation**

The catalog includes a validation method (can be called from custom editor):

```csharp
m_ModelCatalog.ValidateCatalog();
```

**Checks for:**
- Missing model prefabs
- Duplicate tag names
- Enabled vs total mappings
- Logs summary to console

**Example output:**
```
[Catalog] Validating 100 mappings...
[Catalog] Validation complete:
  ✅ Enabled mappings: 87
  ⚠️ Missing models: 13
  ⚠️ Duplicate tags: 0
  📊 Total mappings: 100
```

---

## 🚀 **Workflow for 100 Models**

### **Phase 1: Preparation**

1. **Generate/collect 100 AprilTag images**
   - Use consistent naming: `AprilTag_0.png` to `AprilTag_99.png`
   - High resolution (1024x1024+)
   - High contrast

2. **Prepare 3D model prefabs**
   - Create/import all models
   - Save as prefabs in `Assets/Models/` folder
   - Use descriptive names: `LeapingCoyote.prefab`, `RunningFox.prefab`, etc.

3. **Add to Reference Image Library**
   - Open `ReferenceImageLibrary.asset`
   - Add all 100 AprilTag images
   - Set names: "0", "1", "2", ... "99"
   - Set physical size (0.09m for 9cm tags)

---

### **Phase 2: Catalog Creation**

1. **Create catalog asset:**
   - Right-click in Project → Create → AR → AprilTag Model Catalog
   - Name: `AprilTagModelCatalog.asset`

2. **Set size to 100**

3. **Fill in mappings** (can be done incrementally):
   ```
   Start with first 10:
   [0] "0" → "Leaping Coyote" → LeapingCoyote.prefab
   [1] "1" → "Running Fox" → RunningFox.prefab
   ...
   [9] "9" → "Swimming Otter" → SwimmingOtter.prefab
   
   Test these 10 first, then add the rest
   ```

4. **Optional: Set default model**
   - Assign a generic model as fallback
   - Used for unmapped tags during development

---

### **Phase 3: Configuration**

1. **Assign catalog to SimpleImageTracking:**
   - Select `XR Origin (AR Rig)` GameObject
   - Find `SimpleImageTracking` component
   - Drag catalog asset into "Model Catalog" field

2. **Keep default settings:**
   - Model Prefab: Can leave assigned (used as fallback)
   - Model Offset: (0, 0.3, 0) - default for all models
   - Scale: 1 - default for all models

3. **Customize per-model if needed:**
   - In catalog, set Custom Offset/Rotation/Scale
   - Only needed for models that differ from defaults

---

### **Phase 4: Testing Strategy**

**Incremental Testing:**

1. **Test 1-5 models:**
   ```
   Add mappings for tags 0-4
   Print/display AprilTags 0-4
   Verify each spawns correct model
   Check descriptive names in logs
   ```

2. **Test 10-20 models:**
   ```
   Add more mappings
   Test detection speed (should be similar)
   Test switching between different tags
   Test 2 different tags simultaneously
   ```

3. **Scale to 100:**
   ```
   Add remaining mappings
   Monitor performance
   Test random selection of tags
   Verify catalog validation
   ```

---

## 🔍 **Debug Logs**

When catalog is working, you'll see:

**Startup:**
```
[ImageTracking] 📚 Reference Library has 100 images
[ImageTracking] 📖 Model Catalog loaded with 87 enabled mappings
```

**Detection:**
```
[ImageTracking] ✅ Detected: 0
  📝 Model: 'Leaping Coyote'
  Position: (0.08, -0.59, 0.47)
  Rotation: (358.85, 98.82, 359.76)
[ImageTracking] 📦 Spawned model at offset: (0.00, 0.30, 0.00)
```

**With custom transform:**
```
[ImageTracking] ✅ Detected: 15
  📝 Model: 'Tall Tree'
[ImageTracking] 🎨 Using custom transform for 'Tall Tree'
[ImageTracking] 📦 Spawned model at offset: (0.00, 0.80, 0.00)
```

---

## ⚠️ **Common Issues & Solutions**

### **Issue: "No model available for tag 'X'"**

**Cause:** Tag detected but no mapping in catalog

**Solutions:**
1. Check tag name matches exactly (case-sensitive: "0" not "Tag0")
2. Verify mapping is enabled (checkbox checked)
3. Check model prefab is assigned
4. Set a default model as fallback

---

### **Issue: Wrong model appears**

**Cause:** Tag name mismatch

**Check:**
1. Reference Library tag name: `"0"`
2. Catalog mapping name: `"0"` ← Must match exactly
3. Case-sensitive and no extra spaces

---

### **Issue: Performance slow with 100 tags**

**Unlikely, but if it happens:**
1. Reduce reference image resolution to 1024x1024
2. Ensure only 2 tags are visible at once
3. Check device isn't overheating
4. Verify Max Moving Images is still `2` (not higher)

---

### **Issue: Can't find catalog in Inspector**

**Solution:**
1. Make sure `AprilTagModelCatalog.cs` compiled without errors
2. Check Unity console for compilation errors
3. Refresh assets (Cmd+R / Ctrl+R)
4. Restart Unity if needed

---

## 📊 **Example Catalog Structure**

```
AprilTag Model Catalog (100 entries):

Wildlife Theme:
  [0]  "0"  → "Leaping Coyote"     → Models/Wildlife/Coyote.prefab
  [1]  "1"  → "Running Fox"        → Models/Wildlife/Fox.prefab
  [2]  "2"  → "Soaring Eagle"      → Models/Wildlife/Eagle.prefab
  [3]  "3"  → "Prowling Wolf"      → Models/Wildlife/Wolf.prefab
  [4]  "4"  → "Swimming Otter"     → Models/Wildlife/Otter.prefab
  ...

Vehicles Theme:
  [50] "50" → "Racing Car"         → Models/Vehicles/RaceCar.prefab
  [51] "51" → "Flying Plane"       → Models/Vehicles/Plane.prefab
  ...

Buildings Theme:
  [75] "75" → "City Skyscraper"    → Models/Buildings/Skyscraper.prefab
  [76] "76" → "Country Barn"       → Models/Buildings/Barn.prefab
  ...

Default:
  Default Model: GenericCube.prefab (for unmapped tags)
```

---

## 🎨 **Custom Transform Examples**

### **Example 1: Tall Model**
```
Tag: "10"
Name: "Giant Redwood Tree"
Model: RedwoodTree.prefab
Custom Offset: (0, 1.5, 0)    ← 1.5m tall, needs higher offset
Custom Scale: 2.0              ← Larger than other models
```

### **Example 2: Flat Model**
```
Tag: "20"
Name: "Floor Mat"
Model: MatModel.prefab
Custom Offset: (0, 0.01, 0)   ← Almost flush with tag
Custom Scale: 3.0              ← Covers larger area
```

### **Example 3: Rotated Model**
```
Tag: "30"
Name: "Arrow Sign"
Model: ArrowSign.prefab
Custom Rotation: (0, 90, 0)   ← Points right instead of forward
```

---

## 🔧 **Integration with SimpleImageTracking**

### **Inspector Configuration:**

```
SimpleImageTracking Component:

  Model Catalog:
    ┌─────────────────────────────────────┐
    │ AprilTagModelCatalog (Asset)       │ ← Drag catalog here
    └─────────────────────────────────────┘
  
  Visualization:
    Model Prefab: [Optional fallback]
    Model Offset: (0, 0.3, 0)            ← Default for all models
    Rotation Offset: (0, 0, 0)           ← Default for all models
    Scale: 1                              ← Default for all models
```

### **How It Works:**

1. **Tag detected** → `OnImageAdded()` called
2. **Check catalog** → `GetModelForTag(tagName)`
3. **Found in catalog** → Use catalog model + settings
4. **Not in catalog** → Use default Model Prefab
5. **Spawn model** as child of tracked image
6. **Apply transforms** (catalog custom or defaults)

---

## 📝 **Best Practices**

### **Naming Convention:**

**AprilTag Names in Reference Library:**
- Use simple numbers: `"0"`, `"1"`, `"2"`, etc.
- Keep them short and consistent
- Match exactly in catalog

**Descriptive Names in Catalog:**
- Use readable names: `"Leaping Coyote"`, not `"LC_001"`
- Include context if helpful: `"Red Fox (Adult)"` vs `"Red Fox (Juvenile)"`
- Keep under 50 characters for readability

**Model Prefab Names:**
- Descriptive file names: `LeapingCoyote.prefab`
- Organize in folders: `Models/Wildlife/`, `Models/Vehicles/`
- Include variant info if needed: `Fox_Running.prefab`, `Fox_Sitting.prefab`

---

### **Organization Tips:**

**For Large Catalogs (100+ entries):**

1. **Use consistent numbering:**
   ```
   0-24:   Wildlife
   25-49:  Vehicles
   50-74:  Buildings
   75-99:  Characters
   ```

2. **Document in Notes field:**
   ```
   Notes: "Part of Wildlife collection. 
          Category: Mammals
          Created: 2025-11-02"
   ```

3. **Keep a spreadsheet:**
   ```
   Tag ID | Name              | Model File           | Status
   0      | Leaping Coyote   | LeapingCoyote.prefab | ✓
   1      | Running Fox      | RunningFox.prefab    | ✓
   2      | Soaring Eagle    | SoaringEagle.prefab  | In Progress
   ```

---

## 🧪 **Testing Checklist**

### **Basic Testing:**

- [ ] Catalog asset created
- [ ] At least 2 mappings configured
- [ ] Catalog assigned to SimpleImageTracking
- [ ] AprilTags 0 and 1 in Reference Library
- [ ] Build and deploy to device

### **Functional Testing:**

- [ ] Detect AprilTag 0 → Correct model appears
- [ ] Detect AprilTag 1 → Different model appears
- [ ] Switch between tags → Models swap correctly
- [ ] Detect 2 tags simultaneously → Both correct models appear
- [ ] Descriptive names appear in logs

### **Advanced Testing:**

- [ ] Custom offset works (different height)
- [ ] Custom rotation works (model oriented differently)
- [ ] Custom scale works (model sized differently)
- [ ] Disabled mapping → Falls back to default
- [ ] Unmapped tag → Falls back to default (if set)

---

## 🎯 **Example Catalog Entry**

Here's a complete example of a well-configured mapping:

```
Element 0:
  AprilTag Info:
    April Tag Name: "0"
    Descriptive Name: "Leaping Coyote"
  
  Model:
    Model Prefab: Assets/Models/Wildlife/LeapingCoyote.prefab
  
  Optional Settings:
    Enabled: ✓
    Custom Offset: (0, 0.4, 0)     ← Slightly higher (coyote is jumping)
    Custom Rotation: (0, 0, 0)     ← No rotation needed
    Custom Scale: 1.2               ← 20% larger for visibility
  
  Metadata:
    Notes: "Coyote in mid-leap pose. 
           Higher offset needed for jump clearance.
           Slightly larger for better visibility at distance.
           Model created 2025-10-15."
```

**When detected, logs show:**
```
[ImageTracking] ✅ Detected: 0
  📝 Model: 'Leaping Coyote'
  Position: (0.08, -0.59, 0.47)
[ImageTracking] 🎨 Using custom transform for 'Leaping Coyote'
[ImageTracking] 📦 Spawned model at offset: (0.00, 0.40, 0.00)
```

---

## 🔬 **Under the Hood**

### **How Lookup Works:**

```
1. AprilTag detected → Name = "0"
2. SimpleImageTracking calls GetModelForTag("0")
3. Catalog searches mappings for aprilTagName = "0"
4. Found → Returns modelPrefab
5. SimpleImageTracking spawns that prefab
6. Applies custom transform if mapping.HasCustomTransform() = true
7. Otherwise uses default transform settings
```

### **Performance:**

- **Lookup:** O(n) linear search (fast for 100 entries)
- **Memory:** Catalog asset ~10KB + references to prefabs
- **Runtime:** Negligible overhead
- **Optimization:** Could use Dictionary if >1000 entries (not needed)

---

## 💡 **Tips for Managing 100 Entries**

### **Efficient Inspector Workflow:**

1. **Add in batches:**
   - Set size to 10, fill them in
   - Increase to 20, fill 10 more
   - Continue incrementally

2. **Use copy/paste:**
   - Configure one mapping completely
   - Duplicate the catalog asset
   - Edit names and prefabs

3. **Consider CSV import:**
   - Could create editor script to import from CSV
   - Format: `TagName,DescriptiveName,PrefabPath`
   - Bulk populate catalog

### **Maintenance:**

**Keep catalog organized:**
- Group related entries (comment in notes)
- Use consistent naming
- Document special cases
- Validate regularly

**Update workflow:**
- Test new models individually first
- Add to catalog incrementally
- Use "Enabled" checkbox for work-in-progress
- Keep notes updated

---

## 📊 **Expected Console Output**

### **Startup:**
```
[ImageTracking] ⚙️ Awake() called
[ImageTracking] ✅ ARTrackedImageManager found: XR Origin (AR Rig)
[ImageTracking] ▶️ OnEnable() called
[ImageTracking] ✅ Subscribed to trackedImagesChanged event
[ImageTracking] 📚 Reference Library has 100 images
[ImageTracking] 📖 Model Catalog loaded with 100 enabled mappings
```

### **Detection (Tag 0 - Leaping Coyote):**
```
[ImageTracking] ✅ Detected: 0
  📝 Model: 'Leaping Coyote'
  Position: (0.08, -0.59, 0.47)
  Rotation: (358.85, 98.82, 359.76)
[ImageTracking] 🎨 Using custom transform for 'Leaping Coyote'
[ImageTracking] 📦 Spawned model at offset: (0.00, 0.40, 0.00)
```

### **Detection (Tag 1 - Running Fox):**
```
[ImageTracking] ✅ Detected: 1
  📝 Model: 'Running Fox'
  Position: (-0.15, -0.62, 0.52)
  Rotation: (0.23, 105.44, 1.12)
[ImageTracking] 📦 Spawned model at offset: (0.00, 0.30, 0.00)
```

### **Simultaneous (Both visible):**
```
[ImageTracking] 👁️ Currently tracking 2 image(s)
```

---

## ✅ **Setup Complete Checklist**

- [ ] `AprilTagModelCatalog.cs` script created
- [ ] Catalog asset created in Project
- [ ] Mappings added (at least test set of 2-5)
- [ ] Catalog assigned to SimpleImageTracking component
- [ ] Reference Library has matching AprilTag images
- [ ] Model prefabs exist and are assigned
- [ ] Tested with 2-3 different tags
- [ ] Descriptive names appear in logs
- [ ] Ready to scale to 100 entries

---

## 🎬 **Next Steps**

1. **Create the catalog asset** (Unity: Create → AR → AprilTag Model Catalog)
2. **Add your first few mappings** (e.g., "0" → "Leaping Coyote")
3. **Assign to SimpleImageTracking**
4. **Test** with those few tags
5. **Scale up** to 100 as you create more models

The system is ready for your 100-model catalog with descriptive names! 🚀

