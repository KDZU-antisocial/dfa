# DFA - Dubplate Forensics Agent

KDZU’s Dubplate Forensics Agent (DFA) is a software-and-hardware platform that uses augmented reality (AR) fiduciary markers, specifically AprilTag markers, to summon and animate spectral 3D forms in real time as the DJ manipulates vinyl records. Built with Unity AR Foundation and optimized for iOS devices, DFA tracks AprilTag markers in real-time, making the invisible architectures of recorded music visible and performable. The DFA functions both as a practical performance instrument and a mythic device, created to uncover the hauntologies and mythic narratives embedded in recorded sound.

The DFA system pairs animated 3d models with the DJ's music tracks to create a manipulable multimedia storytelling system, where sound, imagery, and gesture commingle to reveal the unseen and mythic layers embedded in the DJ's music. 

## 🎬 Check Out this Video of Our First Test of the System

[![DFA Performance Demo](https://img.youtube.com/vi/cRf0pgQk89U/maxresdefault.jpg)](https://www.youtube.com/watch?v=cRf0pgQk89U)


## 🎯 What The Code Does

This Unity project uses your iPhone's camera to:
1. **Detect AprilTag markers** in the real world (printed labels)
2. **Display 3D models** positioned and oriented on each tag
3. **Track tags in real-time** as you move around them
4. **Support multiple tags** simultaneously (currently configured for 2 tags)

## ⚡ Quick Start

### Prerequisites
- **macOS** with Xcode installed
- **Unity** 2023 (probably works on other versions)
- **iOS device** with ARKit support (iPhone 6s or newer)
- **Apple Developer account** (for device deployment)

### 1. Generate Your AprilTags

Use our companion repository to create 3D-printable AprilTag labels:

🔗 **[dfa-tags](https://github.com/KDZU-antisocial/dfa-tags)** - Converts AprilTag SVGs into 3D printer-ready 3MF tags

This tool creates physical AprilTag labels optimized for:
- Multi-color 3D printing (black/white contrast)
- Record player-style circular design with center hole
- Optimal size for AR tracking (90mm recommended)

**Or** generate basic PNG/JPG AprilTags:
1. Visit [AprilTag Generator](https://chaitanyantr.github.io/apriltag.html)
2. Select **Tag Family:** `tagStandard41h12`
3. Generate tags 0, 1, 2, 3 (or your desired IDs)
4. Print at exactly 90mm x 90mm size

### 2. Open the Project

```bash
cd /path/to/dfa
# Open the project in Unity
open -a Unity dfa.sln
```

### 3. Configure Your Models

Add or modify tag-to-model mappings:

1. In Unity, navigate to `Assets/AprilTagModelCatalog.asset`
2. Edit entries to map your AprilTag IDs to 3D models:
   - **AprilTag 0** → Pyramid
   - **AprilTag 1** → Cylinder  
   - **AprilTag 2** → Cube
   - **AprilTag 3** → Arch
3. Adjust offsets and scales as needed

### 4. Build and Deploy to iOS

1. Go to `File` → `Build Settings`
2. Select **iOS** platform
3. Click **Build** (or **Build And Run** if device is connected)
4. Open the generated Xcode project
5. Sign with your Apple Developer account
6. Deploy to your iPhone

### 5. Use the App

1. Open the app on your iPhone
2. Point your camera at a printed AprilTag
3. Watch the corresponding 3D model appear on the tag
4. Move around - the model sticks to the tag and follows your perspective

## 🏗️ How It Works

### Architecture

```
AR Foundation (ARKit/ARCore)
    ↓
Reference Image Library (4 AprilTag images)
    ↓
SimpleImageTracking.cs (Detects tracked images)
    ↓
AprilTagModelCatalog.asset (Maps tag ID → 3D model)
    ↓
3D Model Instantiated & Positioned on Tag
```

### Current Configuration

- **Tag Family:** `tagStandard41h12` (2,115 unique IDs available)
- **Physical Size:** 90mm x 90mm (0.09m)
- **Max Simultaneous Tags:** 4
- **Detection Quality:** 0.2 (optimized for 15-90° viewing angles)
- **Models:** Pyramid, Cylinder, Cube, Arch (customizable)

### Key Components

1. **Reference Image Library** (`Assets/ReferenceImageLibrary.asset`)
   - Contains the AprilTag images used for detection
   - Must match your physical printed tags

2. **Model Catalog** (`Assets/AprilTagModelCatalog.asset`)
   - Maps each AprilTag ID to a specific 3D model prefab
   - Configures position offset, rotation, and scale per model

3. **Tracking Scripts** (`Assets/MobileARTemplateAssets/Scripts/`)
   - `SimpleImageTracking.cs` - Core AR Foundation image tracking
   - `ImprovedImageTracking.cs` - Performance monitoring
   - `AprilTagModelCatalog.cs` - Catalog system

## 📚 Documentation

### Getting Started
- 📖 [Complete Setup Guide](Assets/MobileARTemplateAssets/Scripts/IMAGE_TRACKING_SETUP.md) - Detailed AR Foundation setup
- ⚡ [Quick Start: Model Catalog](Assets/MobileARTemplateAssets/Scripts/README_QuickStart_Catalog.md) - Fast guide to mapping tags to models

### Configuration & Usage
- 📚 [Model Catalog System](Assets/MobileARTemplateAssets/Scripts/README_ModelCatalog.md) - Complete catalog documentation
- ⚡ [Performance Guide](Assets/MobileARTemplateAssets/Scripts/README_Performance.md) - Optimization and troubleshooting
- 📐 [Angle Detection Tips](Assets/MobileARTemplateAssets/Scripts/ANGLE_DETECTION_TIPS.md) - Improving detection at difficult angles

### Reference
- 🏷️ [AprilTag System Overview](Assets/MobileARTemplateAssets/Scripts/README_AprilTag.md) - Legacy system reference
- 🎨 [Directional Prefabs](Assets/MobileARTemplateAssets/Scripts/README_DirectionalPrefab.md) - Creating visualization prefabs
- 📖 [Main Scripts README](Assets/MobileARTemplateAssets/Scripts/README.md) - Complete script documentation

## 🔧 Common Tasks

### Add a New AprilTag

1. **Generate the tag:**
   - Use [dfa-tags](https://github.com/KDZU-antisocial/dfa-tags) for 3D-printable labels
   - Or use the [online generator](https://chaitanyantr.github.io/apriltag.html) for PNG/JPG

2. **Add to Reference Library:**
   - Open `Assets/ReferenceImageLibrary.asset`
   - Click "Add Image" and drag your AprilTag image
   - Set name (e.g., "4") and size (0.09 for 90mm)

3. **Map to a Model:**
   - Open `Assets/AprilTagModelCatalog.asset`
   - Add new entry with your tag ID and desired 3D model

4. **Rebuild and test**

### Change a Model for an Existing Tag

1. Open `Assets/AprilTagModelCatalog.asset`
2. Find your tag entry (e.g., "AprilTag Name: 0")
3. Drag a different prefab into "Model Prefab"
4. Adjust offset/scale if needed
5. Rebuild (no need to regenerate tags!)

### Adjust Model Position/Size

In `AprilTagModelCatalog.asset`, for each tag:
- **Custom Offset:** `(0, 0.03, 0)` - lifts model 3cm above tag
- **Custom Rotation:** `(0, 0, 0)` - rotates model (degrees)
- **Custom Scale:** `0.5` - scales model to 50% size

## 🎵 Record Player Label Workflow

For creating multi-color 3D printed AprilTag labels optimized for record players:

1. **Generate SVG AprilTags:**
   ```bash
   # Visit https://chaitanyantr.github.io/apriltag.html
   # Generate tagStandard41h12 tags 0-99 as needed
   ```

2. **Convert to 3D-printable 3MF:**
   ```bash
   # Clone the dfa-tags repository
   git clone https://github.com/KDZU-antisocial/dfa-tags.git
   cd dfa-tags
   
   # Follow setup instructions in the dfa-tags README
   # Converts SVGs to 3MF with circular clipping and center hole
   ```

3. **3D Print:**
   - Import 3MF files into your slicer (PrusaSlicer, Cura, Bambu Studio)
   - Assign black and white filaments to the two materials
   - Print at 140mm diameter with center hole

4. **Add to Unity:**
   - Import PNG/JPG snapshots of your tags to Unity
   - Add to Reference Image Library
   - Map to models in Model Catalog
   - Build and deploy

5. **Test:**
   - Place printed labels on records or surfaces
   - Point your iPhone at the labels
   - See your 3D models appear!

## 🛠️ Troubleshooting

### Models don't appear
- Verify physical tag size matches configured size (0.09m = 90mm)
- Ensure good lighting and clear view of tag
- Check Unity Console for `[ImageTracking]` messages

### iOS build fails with "actool error"
- Open `ReferenceImageLibrary.asset` and remove any entries with null/missing textures
- Ensure all AprilTag images are properly imported
- See [detailed troubleshooting](Assets/MobileARTemplateAssets/Scripts/README.md#ios-build-fails-with-actool-failed-with-exit-code-1)

### Poor tracking at angles
- Lower detection quality threshold to 0.15-0.2
- Improve lighting conditions
- Use higher resolution reference images
- See [Angle Detection Tips](Assets/MobileARTemplateAssets/Scripts/ANGLE_DETECTION_TIPS.md)

## 📦 Project Structure

```
dfa/
├── Assets/
│   ├── ReferenceImageLibrary.asset          # AprilTag images for detection
│   ├── AprilTagModelCatalog.asset           # Tag → Model mappings
│   ├── MobileARTemplateAssets/
│   │   ├── Scripts/                         # All tracking scripts
│   │   │   ├── README.md                   # Complete script documentation
│   │   │   ├── SimpleImageTracking.cs      # Core tracking
│   │   │   ├── ImprovedImageTracking.cs    # Performance monitoring
│   │   │   ├── AprilTagModelCatalog.cs     # Catalog system
│   │   │   └── [other scripts & docs]
│   │   ├── Prefabs/                        # 3D model prefabs
│   │   └── [other assets]
│   └── Scenes/                             # Unity scenes
├── ProjectSettings/                        # Unity project settings
├── Packages/                               # Unity packages
└── README.md                               # This file
```

## 🔗 Related Projects

- **[dfa-tags](https://github.com/KDZU-antisocial/dfa-tags)** - Converts AprilTag SVGs into 3D printer-ready 3MF tags with circular clipping, center holes, and multi-material support. Perfect for creating physical AprilTag labels for this AR system.

## 📖 Further Reading

- [Unity AR Foundation Documentation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@latest)
- [ARKit Image Tracking](https://developer.apple.com/documentation/arkit/tracking_and_visualizing_images)
- [AprilTag Official Site](https://april.eecs.umich.edu/software/apriltag)

## 🎯 System Status

| Component | Status | Count/Config |
|-----------|--------|--------------|
| AR Foundation Image Tracking | ✅ Active | Main system |
| Reference Image Library | ✅ Configured | 4 AprilTags (0-3) |
| Model Catalog | ✅ Configured | 4 models mapped |
| Max Simultaneous Tags | ✅ Active | 4 |
| Detection Quality | ✅ Optimized | 0.2 (15-90° angles) |

## 📝 License

[Add your license here]

## 🤝 Contributing

[Add contribution guidelines here]

---

**Last Updated:** November 2025 - 4 AprilTags configured with individual models, iOS build issues resolved, performance optimized.

