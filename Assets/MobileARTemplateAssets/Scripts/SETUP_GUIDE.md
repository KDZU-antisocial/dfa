# AprilTag Image Tracking Setup Guide

This guide will help you set up AR Foundation image tracking for your AprilTags. This approach provides robust tracking with proper rotation and allows you to walk around the 3D model.

## Why Use This Approach?

- **Native AR Foundation Support**: Uses the platform's built-in image tracking (ARKit on iOS, ARCore on Android)
- **Automatic Rotation**: The model automatically rotates with the AprilTag
- **Walk-Around Capability**: You can walk around the model and see all sides
- **Robust Tracking**: Better tracking stability compared to manual pose estimation
- **Simple Setup**: The model is parented to the tracked image, so position/rotation updates are automatic

---

## Step 1: Create a Reference Image Library

1. In Unity, right-click in your **Project** window
2. Select **Create > XR > Reference Image Library**
3. Name it something like `AprilTagReferenceLibrary`
4. Click on the newly created asset to open it in the Inspector

---

## Step 2: Add Your AprilTag Images

1. **Capture or generate your AprilTag images** as PNG/JPG files
   - Make sure they're clear, high-resolution images
   - The images should show just the AprilTag (black and white pattern)

2. In the **Reference Image Library Inspector**, click **Add Image**

3. For each AprilTag:
   - Drag your AprilTag image file into the **Texture 2D** field
   - Set a **Name** (e.g., "AprilTag_0", "AprilTag_1", etc.)
   - **Specify Physical Size**: Enter the **actual physical size** of your printed AprilTag in meters
     - Example: If your tag is 10cm x 10cm, enter `0.1` for X and `0.1` for Y
     - This is CRITICAL for accurate tracking!
   - Check **Keep Texture at Runtime** (if needed for your AR platform)

4. Repeat for all AprilTags you want to track

---

## Step 3: Set Up AR Session Origin

### 3a. Find Your AR Session Origin GameObject

In your scene hierarchy, find the GameObject with the `AR Session Origin` or `XR Origin` component.

### 3b. Add AR Tracked Image Manager

1. Select your AR Session Origin GameObject
2. In the Inspector, click **Add Component**
3. Search for **AR Tracked Image Manager** and add it
4. In the AR Tracked Image Manager component:
   - **Serialized Library**: Drag your Reference Image Library asset here
   - **Max Number Of Moving Images**: Set to the number of tags you want to track simultaneously (e.g., `3`)
   - Leave other settings at default

---

## Step 4: Add the AprilTagImageTracking Script

1. **Select the same GameObject** (AR Session Origin) where you added the AR Tracked Image Manager
2. In the Inspector, click **Add Component**
3. Search for **AprilTagImageTracking** and add it

---

## Step 5: Configure the AprilTagImageTracking Script

In the **AprilTagImageTracking** component:

### AR Foundation Components
- **Tracked Image Manager**: Should auto-assign (the component you just added)
- **AR Camera**: Should auto-assign to Main Camera

### Visualization
- **Model Prefab**: Drag your 3D model prefab here (the model you want to appear on the AprilTag)
- **Model Offset**: Position offset relative to the tag
  - `X`: Left/Right (positive = right)
  - `Y`: Up/Down (positive = up)
  - `Z`: Forward/Back (positive = forward)
  - Example: `(0, 0.2, 0)` places the model 20cm above the tag
- **Model Rotation Offset**: Rotation in Euler angles
  - Use this if your model needs to be rotated to face upright
  - Example: `(0, 180, 0)` rotates the model 180° around the Y-axis
- **Model Scale**: Overall scale multiplier (default `1.0`)

### Debug
- **Show Debug Info**: Check this to see detailed logging in Xcode/Logcat

---

## Step 6: Test Your Setup

### 6a. Build and Deploy

1. Go to **File > Build Settings**
2. Make sure your platform is set to **iOS** or **Android**
3. Click **Build And Run** to deploy to your device

### 6b. Testing

1. Launch the app on your device
2. Point your device camera at the printed AprilTag
3. You should see:
   - The 3D model appears on/above the AprilTag
   - The model rotates when you rotate the AprilTag
   - You can walk around the AprilTag and see all sides of the model
   - The model stays anchored to the tag even as you move

---

## Troubleshooting

### Model doesn't appear
- **Check Reference Image Library**: Make sure the AprilTag image is added with correct physical size
- **Check Model Prefab**: Ensure you've assigned a prefab in the AprilTagImageTracking script
- **Check AR Tracked Image Manager**: Ensure the Reference Image Library is assigned
- **Check lighting**: AR tracking works best in well-lit environments

### Model appears but doesn't rotate with the tag
- Make sure the model is instantiated as a **child** of the tracked image (the script does this automatically)
- Don't modify the parent-child relationship in code

### Model is sideways or upside-down
- Adjust the **Model Rotation Offset** in the AprilTagImageTracking component
- Try values like:
  - `(90, 0, 0)` - Rotate 90° around X-axis
  - `(0, 180, 0)` - Rotate 180° around Y-axis
  - `(-90, 0, 0)` - Rotate -90° around X-axis

### Model is too far from the tag
- Adjust the **Model Offset** Y value
- Positive Y moves the model up, negative Y moves it down

### Tracking is lost frequently
- **Improve lighting**: Make sure your environment is well-lit
- **Print quality**: Use high-quality printed AprilTags (not from a screen)
- **Physical size**: Double-check that the physical size in the Reference Image Library matches your actual printed tag size
- **Distance**: Stay within 0.5m - 3m from the tag for best tracking

### Model jitters or shakes
- This is normal with image tracking
- AR Foundation provides smooth tracking, but perfect stability is challenging
- Try:
  - Better lighting
  - Slower camera movement
  - Larger printed AprilTag

---

## Advanced: Using Multiple AprilTags

If you want to track multiple different AprilTags:

1. Add all AprilTag images to your Reference Image Library
2. Set **Max Number Of Moving Images** to the number you want to track simultaneously
3. The script will automatically spawn a model for each detected tag

If you want **different models for different tags**, you'll need to modify the script to check `trackedImage.referenceImage.name` and instantiate different prefabs accordingly.

---

## Switching from Old AprilTag Detection

If you were previously using the `AprilTagManager` script:

1. **Disable or remove** the old `AprilTagManager` component
2. Follow all the steps above to set up the new `AprilTagImageTracking` approach
3. **Rebuild and deploy** to your device

The old AprilTag detection was manual pose estimation. The new approach uses native AR Foundation image tracking, which is much more robust.

---

## Quick Reference: Component Checklist

✅ **AR Session Origin GameObject:**
- AR Tracked Image Manager (with Reference Image Library assigned)
- AprilTagImageTracking script (with Model Prefab assigned)

✅ **Reference Image Library:**
- All AprilTag images added
- Physical sizes correctly specified (in meters!)
- Names set for each image

✅ **Build Settings:**
- Platform set to iOS or Android
- ARKit/ARCore packages installed

---

## Still Having Issues?

Enable **Show Debug Info** in the AprilTagImageTracking component and check the device console logs (Xcode for iOS, Logcat for Android) for detailed information about what's being detected and tracked.

Look for log messages like:
- `[AprilTagImageTracking] New tracked image detected: AprilTag_0`
- `[AprilTagImageTracking] Spawned model...`
- `[AprilTagImageTracking] Image is now visible (tracking)`

If you see these messages, the system is working correctly!

