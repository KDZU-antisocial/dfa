# Quick Start: Model Catalog System

**Goal:** Map 100 different AprilTags to 100 different 3D models with descriptive names.

**Example:** AprilTag "0" → "Leaping Coyote" → CoyoteModel.prefab

---

## ⚡ **3-Step Setup**

### **Step 1: Create the Catalog** (Unity Editor)

1. Right-click in Project window
2. **Create → AR → AprilTag Model Catalog**
3. Name it: `AprilTagModelCatalog`

---

### **Step 2: Add Your Mappings**

1. **Select** the catalog asset
2. **Set Size** to `100` (or however many you need)
3. **Fill in each entry:**
   ```
   Element 0:
     April Tag Name: "0"              ← Must match Reference Library
     Descriptive Name: "Leaping Coyote"
     Model Prefab: [Drag prefab here]
     Enabled: ✓
   
   Element 1:
     April Tag Name: "1"
     Descriptive Name: "Running Fox"
     Model Prefab: [Drag prefab here]
     Enabled: ✓
   
   ... repeat for all 100 ...
   ```

---

### **Step 3: Link to SimpleImageTracking**

1. **Select:** `XR Origin (AR Rig)` GameObject in your scene
2. **Find:** `SimpleImageTracking` component
3. **Drag** your catalog asset into the **"Model Catalog"** field
4. **Done!** ✅

---

## 🎮 **How It Works**

```
Camera detects AprilTag "0"
    ↓
SimpleImageTracking receives event
    ↓
Looks up "0" in Model Catalog
    ↓
Finds: "Leaping Coyote" → CoyoteModel.prefab
    ↓
Spawns CoyoteModel on the AprilTag
    ↓
Model sticks to tag, rotates with tag
```

**Console logs:**
```
[ImageTracking] ✅ Detected: 0
  📝 Model: 'Leaping Coyote'
[ImageTracking] 📦 Spawned model at offset: (0.00, 0.30, 0.00)
```

---

## 🎨 **Optional: Custom Settings Per Model**

If a model needs different positioning:

```
Element 15:
  April Tag Name: "15"
  Descriptive Name: "Tall Tree"
  Model Prefab: TallTree.prefab
  
  Custom Offset: (0, 0.8, 0)      ← Higher offset for tall model
  Custom Rotation: (0, 45, 0)     ← Rotated 45°
  Custom Scale: 1.5                ← 50% larger
```

**Leave at defaults if model is standard size.**

---

## 📊 **Your Catalog At a Glance**

```
AprilTag "0"  → "Leaping Coyote"     → CoyoteModel.prefab
AprilTag "1"  → "Running Fox"        → FoxModel.prefab
AprilTag "2"  → "Soaring Eagle"      → EagleModel.prefab
...
AprilTag "99" → "Swimming Otter"     → OtterModel.prefab

Total: 100 mappings
Max Simultaneous: 2 (Max Moving Images setting)
```

---

## ✅ **Testing**

1. **Start small** - Add 2-3 mappings first
2. **Test detection:**
   - Point camera at AprilTag 0
   - Should see "Leaping Coyote" model
   - Point at AprilTag 1
   - Should see "Running Fox" model
3. **Verify logs show descriptive names**
4. **Scale up** to 100 once working

---

## 📖 **Full Documentation**

- [README_ModelCatalog.md](README_ModelCatalog.md) - Complete guide
- [README.md](README.md) - System overview
- [IMAGE_TRACKING_SETUP.md](IMAGE_TRACKING_SETUP.md) - Setup guide

---

## 🎯 **Summary**

**What You Have:**
- ✅ Catalog system created (`AprilTagModelCatalog.cs`)
- ✅ SimpleImageTracking modified to use catalog
- ✅ Support for 100+ unique tag-to-model mappings
- ✅ Descriptive names for each model
- ✅ Optional custom transforms per model
- ✅ Comprehensive documentation

**What To Do:**
1. Create catalog asset
2. Add your mappings (incrementally is fine)
3. Link to SimpleImageTracking
4. Test and deploy!

**Your use case is fully supported!** 🎉

