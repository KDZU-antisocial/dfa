# AprilTag Image Tracking Setup Guide

This guide explains how to set up AprilTag tracking using AR Foundation's image tracking system.

## Why This Approach?

- ✅ **Better Rotation Tracking**: AR Foundation's image tracking handles rotation naturally
- ✅ **Walk Around Support**: The model stays anchored to the physical tag location
- ✅ **Automatic Updates**: Position and rotation update automatically
- ✅ **Robust Tracking**: Uses device's built-in image recognition

## Setup Steps

### 1. Create a Reference Image Library

1. **Export Your AprilTag as an Image**:
   - Go to https://github.com/AprilRobotics/apriltag-imgs
   - Download your specific AprilTag (e.g., tag36_11_00000.png)
   - Or take a high-quality photo of your printed AprilTag

2. **Import to Unity**:
   - Drag the AprilTag image into your `Assets` folder
   - Select the image in Unity
   - In Inspector, set:
     - **Texture Type**: `Sprite (2D and UI)` or `Default`
     - **Read/Write Enabled**: ✅ (checked)
     - Click **Apply**

3. **Create Reference Image Library**:
   - Right-click in Project window
   - Select **Create > XR > Reference Image Library**
   - Name it (e.g., "AprilTagLibrary")
   - Click "Add Image"
   - Drag your AprilTag image into the empty slot
   - Set **Name**: Give it a descriptive name (e.g., "AprilTag_0")
   - Set **Physical Size**: Enter the actual size in meters (e.g., 0.05 for 5cm tag)
   - **Keep Adding Images** for other AprilTags you want to track

### 2. Set Up AR Tracked Image Manager

1. **Select your XR Origin** in the Hierarchy
2. **Add Component**: `AR Tracked Image Manager`
3. In the Inspector:
   - **Serialized Library**: Drag your "AprilTagLibrary" here
   - **Max Number of Moving Images**: Set to how many tags you want to track simultaneously (e.g., 2)
   - **Tracked Image Prefab**: Leave empty (we'll handle spawning in code)

### 3. Add AprilTagImageTracking Script

1. **Select your XR Origin** (or create a new GameObject)
2. **Add Component**: `AprilTag Image Tracking` (the script we just created)
3. Configure in Inspector:
   - **AR Tracked Image Manager**: Will auto-assign if on same GameObject
   - **AR Camera**: Drag your AR Camera here (usually "Main Camera" under XR Origin)
   - **April Tag Prefab**: Drag your 3D model prefab here
   - **Model Offset**: Set position offset (e.g., `0, 0.3, 0` to place 30cm above tag)
   - **Model Rotation Offset**: Set rotation (e.g., `0, 0, 0` for no rotation, or `90, 0, 0` to stand up)
   - **Model Scale**: Set scale (e.g., `1` for normal size)
   - **Use AprilTag Detection**: ❌ (uncheck - not needed for basic tracking)
   - **Show Debug Info**: ✅ (check to see tracking logs)

### 4. Test Your Setup

1. **Build and Run** on your iOS device
2. **Point camera** at your physical AprilTag
3. The model should:
   - ✅ Appear above/on the tag at your specified offset
   - ✅ Rotate when you rotate the tag
   - ✅ Stay in place as you walk around it
   - ✅ Hide when tag is not visible, reappear when visible again

## Configuration Options

### Model Offset

Controls where the model appears relative to the tag:
- `(0, 0, 0)`: Directly on the tag
- `(0, 0.3, 0)`: 30cm above the tag (Y-up in Unity)
- `(0, 0, 0.5)`: 50cm in front of the tag

### Model Rotation Offset

Controls the model's orientation:
- `(0, 0, 0)`: No rotation (follows tag exactly)
- `(90, 0, 0)`: Rotate 90° around X-axis (stand up)
- `(0, 180, 0)`: Flip 180° around Y-axis (face opposite direction)

### Tracking Quality

AR Foundation automatically adjusts visibility based on tracking state:
- **Tracking**: Model is visible and updating
- **Limited**: Model may flicker or hide temporarily
- **None**: Model is hidden

## Troubleshooting

### Model Not Appearing

1. Check Console for errors
2. Verify Reference Image Library is assigned to ARTrackedImageManager
3. Ensure AprilTag image has "Read/Write Enabled" in texture settings
4. Confirm physical tag size matches the size set in Reference Image Library
5. Make sure lighting is good (AR tracking needs good lighting)

### Model Not Rotating

1. Verify the model is a **child** of the ARTrackedImage (should be automatic)
2. Check if Model Rotation Offset is correct
3. Try adjusting the physical tag orientation

### Poor Tracking

1. **Better Lighting**: AR image tracking needs good, even lighting
2. **Flat Surface**: Make sure AprilTag is on a flat, stable surface
3. **Image Quality**: Use high-contrast, sharp AprilTag prints
4. **Physical Size**: Match exactly in Unity settings
5. **Camera Distance**: Stay within 0.5-2 meters from the tag

## Advanced: Multiple AprilTags

To track multiple different AprilTags:

1. Add all AprilTag images to your Reference Image Library
2. Give each a unique name (e.g., "AprilTag_0", "AprilTag_1")
3. Set "Max Number of Moving Images" to however many you need
4. Each will spawn the same prefab (or modify script to spawn different prefabs per tag)

## Using AprilTag Detection (Optional)

If you need the specific tag ID from AprilTag detection:

1. Enable "Use AprilTag Detection" in the script
2. This runs AprilTag detection alongside image tracking
3. You can access tag IDs to spawn different models per ID
4. Note: This is slower and usually not needed if you name your reference images properly

---

## Summary

This approach gives you:
- ✅ Model appears on AprilTag
- ✅ Model rotates when tag rotates
- ✅ You can walk around and see all sides
- ✅ Robust tracking using device's built-in capabilities
- ✅ No "fixed anchor" issues

The model is a **child** of the tracked image transform, so it automatically inherits all position and rotation updates from AR Foundation's image tracking system!

