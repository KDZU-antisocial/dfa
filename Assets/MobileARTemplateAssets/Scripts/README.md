# AR Image Tracking Scripts

This folder contains all scripts for AR image tracking and AprilTag detection in your Unity project.

## 📚 **Documentation Index**

### **Getting Started**
- 📖 [**IMAGE_TRACKING_SETUP.md**](IMAGE_TRACKING_SETUP.md) - Complete setup guide for AR Foundation image tracking
- 📖 [**SETUP_GUIDE.md**](SETUP_GUIDE.md) - Alternative setup guide with reference image library creation

### **Model Management**
- ⚡ [**README_QuickStart_Catalog.md**](README_QuickStart_Catalog.md) - **Quick start** for mapping tags to models
- 📚 [**README_ModelCatalog.md**](README_ModelCatalog.md) - Complete catalog system guide

### **Performance & Optimization**
- ⚡ [**README_Performance.md**](README_Performance.md) - Performance optimizations and configuration guide
- 📐 [**ANGLE_DETECTION_TIPS.md**](ANGLE_DETECTION_TIPS.md) - Improving detection at difficult angles

### **Component Reference**
- 🏷️ [**README_AprilTag.md**](README_AprilTag.md) - AprilTag system overview (legacy)
- 🎨 [**README_DirectionalPrefab.md**](README_DirectionalPrefab.md) - Creating directional visualization prefabs

### **Advanced Guides**
- 🔧 [**APRILTAG_IMAGE_TRACKING_SETUP.md**](APRILTAG_IMAGE_TRACKING_SETUP.md) - Hybrid AprilTag + Image Tracking setup

---

## 🎯 **Quick Start**

### **Current System: AR Foundation Image Tracking**

Your project is configured to use Unity's AR Foundation for image tracking. This provides:
- ✅ Native ARKit/ARCore integration
- ✅ Robust tracking at various angles
- ✅ Automatic position and rotation updates
- ✅ Model sticks to AprilTag surface

**Main Scripts:**
- `SimpleImageTracking.cs` - Core image tracking implementation
- `ImprovedImageTracking.cs` - Performance monitoring and optimization

**Configuration:**
- Max Moving Images: `4` (configured for 4 AprilTags)
- Min Detection Quality: `0.2` (optimized for angle tolerance)
- Angle Detection: 15-90° viewing angles

---

## 📦 **Script Overview**

### **Core Tracking Scripts**

| Script | Purpose | Documentation |
|--------|---------|---------------|
| `SimpleImageTracking.cs` | Main AR Foundation image tracking | [IMAGE_TRACKING_SETUP.md](IMAGE_TRACKING_SETUP.md) |
| `ImprovedImageTracking.cs` | Performance monitoring & quality checks | [README_Performance.md](README_Performance.md) |
| `AprilTagModelCatalog.cs` | ScriptableObject: Maps tags to models | [README_ModelCatalog.md](README_ModelCatalog.md) |
| `AprilTagImageTracking.cs` | Hybrid AprilTag + Image tracking (optional) | [APRILTAG_IMAGE_TRACKING_SETUP.md](APRILTAG_IMAGE_TRACKING_SETUP.md) |

### **Legacy AprilTag System** (Not Currently Active)

| Script | Purpose | Status |
|--------|---------|--------|
| `AprilTagManager.cs` | AprilTag detection manager | ⚠️ Disabled |
| `AprilTagVisualization.cs` | Visualization for detected tags | ⚠️ Disabled |
| `AprilTagSetup.cs` | Programmatic setup helper | ⚠️ Disabled |
| `AprilTagExample.cs` | Example implementation | ⚠️ Disabled |

### **Utility Scripts**

| Script | Purpose |
|--------|---------|
| `DisableObjectPlacement.cs` | Disables default tap-to-place functionality |
| `ARTemplateMenuManager.cs` | Menu system management |
| `GoalManager.cs` | Tutorial/goal system |
| `ARFeatheredPlaneMeshVisualizerCompanion.cs` | Plane visualization |

### **Editor Scripts** (Editor/ folder)

| Script | Purpose |
|--------|---------|
| `AprilTagSetupEditor.cs` | Custom inspector for AprilTag setup |
| `CreateDirectionalPrefabEditor.cs` | Prefab creation tool |
| `SimpleAprilTagPrefabCreator.cs` | Quick prefab generator |
| `AutoSigningPostProcessor.cs` | iOS code signing automation |

### **Prefab Creation**

| Script | Purpose | Documentation |
|--------|---------|---------------|
| `CreateAprilTagPrefab.cs` | Basic prefab creator | - |
| `CreateDirectionalAprilTagPrefab.cs` | Directional indicators prefab | [README_DirectionalPrefab.md](README_DirectionalPrefab.md) |

---

## ⚙️ **Current Configuration**

### **Active System: AR Foundation Image Tracking**

```
Scene Setup:
├─ XR Origin (AR Rig)
│  ├─ ARTrackedImageManager (AR Foundation)
│  │  ├─ Reference Library: ReferenceImageLibrary.asset
│  │  ├─ Max Moving Images: 4
│  │  └─ Tracked Image Prefab: None (handled by script)
│  ├─ SimpleImageTracking (Main tracker)
│  │  ├─ Model Catalog: AprilTagModelCatalog.asset (maps tags→models)
│  │  ├─ Model Prefab: AprilTagPrefab (fallback)
│  │  ├─ Model Offset: (0, 0.03, 0) [default if no catalog]
│  │  └─ Show Debug: True
│  └─ ImprovedImageTracking (Performance monitor)
│     ├─ Max Moving Images: 4
│     ├─ Auto Scale Estimation: True
│     └─ Min Detection Quality: 0.2
└─ AprilTag Setup (Disabled)
   └─ Old AprilTag system (inactive)

Project Assets:
├─ ReferenceImageLibrary.asset (4 AprilTag images: tags 0-3)
└─ AprilTagModelCatalog.asset (4 tag→model mappings: pyramid, cylinder, cube, arch)
```

### **AprilTag Generation**

**Tag Family:** `tagStandard41h12` (2,115 unique IDs available)

**Generator Tool:** [https://chaitanyantr.github.io/apriltag.html](https://chaitanyantr.github.io/apriltag.html)

**To generate new AprilTags:**
1. Visit the [AprilTag generator](https://chaitanyantr.github.io/apriltag.html)
2. Select **Tag Family:** `tagStandard41h12`
3. Enter **Tag ID:** `0`, `1`, `2`, etc. (0-2114 available)
4. Set **Total Size:** `90mm` (recommended for AR tracking)
5. Click **Save as SVG** or print to PDF
6. Print the tag at exact size (verify with ruler!)

**Why tagStandard41h12?**
- ✅ 2,115 unique IDs (plenty for large catalogs)
- ✅ Good detection performance
- ✅ Balanced between data density and error correction
- ✅ Well-supported by AR Foundation

**Printing Tips:**
- Print at 100% scale (no fit-to-page)
- Use matte paper (reduces glare)
- Measure printed tag with ruler to verify size
- Mount on rigid backing for better tracking

### **Reference Image Library**

Located at: `Assets/ReferenceImageLibrary.asset`

Contains:
- AprilTag #0 (tagStandard41h12, 0.09m x 0.09m) → Maps to **Pyramid**
- AprilTag #1 (tagStandard41h12, 0.09m x 0.09m) → Maps to **Cylinder**
- AprilTag #2 (tagStandard41h12, 0.09m x 0.09m) → Maps to **Cube**
- AprilTag #3 (tagStandard41h12, 0.09m x 0.09m) → Maps to **Arch**

**To update:**
1. In Unity, select `ReferenceImageLibrary.asset`
2. Add/modify images in the Inspector
3. Set physical size to match printed tags (0.09 = 90mm)
4. Ensure texture reference is not null (critical for iOS builds)
5. Rebuild the project

---

## 🚀 **Common Tasks**

### **Add a New AprilTag with Custom Model**
1. **Add to Reference Library:**
   - Open `Assets/ReferenceImageLibrary.asset`
   - Click "Add Image"
   - Drag your AprilTag image (PNG/JPG)
   - Set name (e.g., "0", "1", "2")
   - Set "Specify Size" to physical dimensions (e.g., 0.09 for 9cm)

2. **Add to Model Catalog:**
   - Open `Assets/AprilTagModelCatalog.asset`
   - Increase "Size" by 1
   - Fill in new entry:
     - AprilTag Name: (match Reference Library, e.g., "0")
     - Descriptive Name: (e.g., "Leaping Coyote")
     - Model Prefab: (drag your model prefab)
     - Custom Offset: (0, 0.03, 0) - typical value to lift model off tag
     - Custom Scale: 0.5 - adjust to fit your model
   - Enable: ✓

3. **Rebuild project**

📖 **See:** [README_ModelCatalog.md](README_ModelCatalog.md) for detailed catalog setup

### **Adjust Detection Sensitivity**
1. Select `XR Origin (AR Rig)` GameObject
2. Find `ImprovedImageTracking` component
3. Adjust "Min Detection Quality":
   - `0.1-0.15`: Very lenient (may be jittery)
   - `0.2`: **Optimized** (current setting)
   - `0.3`: More stable, requires better angles
   - `0.4+`: Strict, perpendicular views only

### **Change Max Tracked Images**
1. Select `XR Origin (AR Rig)` GameObject
2. Find `ARTrackedImageManager` component
3. Change "Max Number Of Moving Images":
   - `1`: Best performance, single marker
   - `2`: Good for 2 AprilTags
   - `3-4`: Multiple markers (current: 4)
   - `5+`: Many markers, modern devices only

### **Disable Debug Logging**
1. Select `XR Origin (AR Rig)` GameObject
2. Find `SimpleImageTracking` component
3. Uncheck "Show Debug"

---

## 📊 **Performance Notes**

**Current Optimizations:**
- Quality checks run every 10 frames (not every frame)
- Debug logging throttled to every 2 seconds
- Frame processing optimized for reduced overhead

**Expected Performance:**
- ARFrame retention: 2-5 frames (healthy)
- Detection at good angles (60-90°): Instant
- Detection at moderate angles (30-60°): 0.5-1s
- Detection at difficult angles (15-30°): 1-2s
- Tracking stability: Excellent at good angles, acceptable at difficult angles

See [README_Performance.md](README_Performance.md) for detailed optimization information.

---

## 🔧 **Troubleshooting**

### **No model appearing**
1. Check that Reference Image Library matches your physical AprilTag
2. Verify physical size is correct (measure your printed tag)
3. Ensure good lighting and clear view of tag
4. Check Console for `[ImageTracking]` messages

### **Model not sticking to tag**
1. Verify `SimpleImageTracking` has `ARTrackedImageManager` linked
2. Check that "AprilTag Setup" GameObject is **disabled**
3. Ensure `ARTrackedImageManager` has `Tracked Image Prefab` set to `None`

### **Poor angle detection**
1. See [ANGLE_DETECTION_TIPS.md](ANGLE_DETECTION_TIPS.md)
2. Lower "Min Detection Quality" to 0.15-0.2
3. Improve lighting and tag print quality
4. Use higher resolution reference images

### **Performance issues**
1. See [README_Performance.md](README_Performance.md)
2. Reduce "Max Moving Images" to 1
3. Disable debug logging
4. Check device temperature and battery

### **iOS Build fails with "actool failed with exit code 1"**
This error occurs when the Reference Image Library has null/missing texture references:
1. Open `Assets/ReferenceImageLibrary.asset` in Unity Inspector
2. Look for entries with empty texture slots (showing as `None`)
3. Either:
   - Remove entries with null textures (click minus button)
   - OR add the missing AprilTag images to those slots
4. Ensure all textures are marked as **Read/Write Enabled** in their import settings
5. Rebuild the project

**Important:** ARKit's `actool` cannot compile reference libraries with null texture references. All entries must have valid images.

---

## 📖 **Further Reading**

- [Unity AR Foundation Documentation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@latest)
- [ARTrackedImageManager Reference](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@latest/api/UnityEngine.XR.ARFoundation.ARTrackedImageManager.html)
- [ARKit Image Tracking](https://developer.apple.com/documentation/arkit/tracking_and_visualizing_images)

---

## 🎯 **System Status**

| Component | Status | Notes |
|-----------|--------|-------|
| AR Foundation Image Tracking | ✅ Active | Main system |
| SimpleImageTracking | ✅ Active | Core functionality |
| ImprovedImageTracking | ✅ Active | Performance monitoring |
| AprilTag Detection System | ⚠️ Disabled | Legacy system, kept for reference |
| Reference Image Library | ✅ Configured | 4 AprilTags (0-3) at 0.09m each |
| Model Catalog | ✅ Configured | 4 models: pyramid, cylinder, cube, arch |
| Performance Optimizations | ✅ Applied | Frame processing optimized |

**Last Updated:** 4 AprilTags configured with individual models, Max Moving Images set to 4, texture references validated for iOS builds.

