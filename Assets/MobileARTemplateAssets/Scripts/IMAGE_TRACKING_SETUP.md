# AR Foundation Image Tracking Setup Guide

This guide will help you switch from AprilTag detection to AR Foundation's native image tracking for more reliable tracking.

## Why AR Foundation Image Tracking?

- ✅ **More Reliable**: Native platform tracking (ARKit/ARCore)
- ✅ **Better Performance**: Hardware-accelerated
- ✅ **Automatic Rotation**: Model follows the image perfectly
- ✅ **Walk-Around View**: Natural 360° viewing
- ✅ **Simpler Code**: Less complexity, fewer bugs

---

## Part 1: Disable Old AprilTag System

### Step 1: Disable the AprilTagManager GameObject

1. Open `SampleScene` in Unity
2. In the **Hierarchy** window, find the GameObject named **"AprilTag Manager"**
3. **Uncheck the checkbox** next to its name in the Inspector to disable it
   - OR: Select it and delete it if you're sure you won't need it

### Step 2: Disable the AprilTag Setup GameObject (Optional)

1. In the **Hierarchy**, find **"AprilTag Setup"**
2. **Uncheck the checkbox** to disable it (this was just for initialization)

**Result**: The old AprilTag detection system is now disabled. ✅

---

## Part 2: Set Up AR Foundation Image Tracking

### Step 1: Create a Reference Image Library

1. In the **Project** window, navigate to `Assets/`
2. Right-click → **Create → XR → Reference Image Library**
3. Name it: **`AprilTagLibrary`**

### Step 2: Add Your AprilTag Images to the Library

#### A. Generate AprilTag Images

**This project uses `tagStandard41h12` AprilTags.**

**Generate your tags:**
1. Visit: [https://chaitanyantr.github.io/apriltag.html](https://chaitanyantr.github.io/apriltag.html)
2. Settings:
   - **Tag Family:** `tagStandard41h12` (2,115 unique IDs)
   - **Tag ID:** `0`, `1`, `2`, etc. (any number 0-2114)
   - **Total Size:** `90mm` (recommended for AR tracking)
3. Click **Save as SVG** or print to PDF
4. Convert to PNG/JPG for Unity (or take a photo of printed tag)

**Printing tips:**
- Print at 100% scale (verify with ruler!)
- Use matte paper (reduces glare)
- Mount on rigid backing for better tracking

**Alternative:** Take a clear photo of your printed AprilTag:
- Well-lit, no shadows
- Square crop showing just the tag
- Save as PNG or JPG

#### B. Import Images into Unity

1. Drag your AprilTag image files into Unity's `Assets/` folder
2. Select each image in the Project window
3. In the Inspector, ensure **Texture Type** is set to **Default**

#### C. Add Images to the Library

1. Select your **`AprilTagLibrary`** asset
2. In the Inspector, click **"Add Image"**
3. For each image:
   - **Texture**: Drag your AprilTag image
   - **Name**: Give it a name matching the tag ID (e.g., "0", "1", "2")
   - **Specify Size**: ✅ Check this
   - **Physical Size**: Enter the real-world size in meters
     - Standard: 90mm tag = **0.09** (recommended)
     - Or: 5cm tag = **0.05**, 10cm tag = **0.10**
   - **Keep Texture at Runtime**: ✅ Check this
4. Repeat for each AprilTag you want to track

**Important**: The physical size MUST match your real printed tag size! Measure with a ruler.

---

### Step 3: Set Up the XR Origin

#### A. Add ARTrackedImageManager

1. In the **Hierarchy**, find **"XR Origin"** (or **"AR Session Origin"**)
2. Select it
3. In the Inspector, click **Add Component**
4. Search for: **AR Tracked Image Manager**
5. Add it

#### B. Configure the ARTrackedImageManager

1. With the XR Origin still selected
2. Find the **AR Tracked Image Manager** component
3. Set these fields:
   - **Serialized Library**: Drag your **`AprilTagLibrary`** asset here
   - **Max Number of Moving Images**: Set to **1** or **2** (number of tags you'll track simultaneously)
   - **Tracked Image Prefab**: Leave empty (we'll use the script instead)

#### C. Add the SimpleImageTracking Script

1. With the XR Origin still selected
2. Click **Add Component**
3. Search for: **Simple Image Tracking**
4. Add it

#### D. Configure the SimpleImageTracking Script

1. Find the **Simple Image Tracking** component
2. Set these fields:
   - **Tracked Image Manager**: Should auto-assign (if not, drag the ARTrackedImageManager)
   - **Model Prefab**: Drag your 3D model prefab (the one that was in AprilTagManager)
     - Find it at: `Assets/Prefabs/` or wherever your model is
   - **Model Offset**: 
     - X: `0` (left/right)
     - Y: `0.3` (height above tag - adjust as needed)
     - Z: `0` (forward/back)
   - **Rotation Offset**: `(0, 0, 0)` (adjust if model faces wrong way)
   - **Scale**: `1.0` (adjust to make model bigger/smaller)
   - **Show Debug**: ✅ Check this to see tracking messages

---

## Part 3: Test It!

### Step 1: Build and Deploy

1. **File → Build Settings**
2. Select **iOS** platform
3. Click **Build** (or **Build and Run**)
4. Deploy to your iOS device

### Step 2: Test Tracking

1. Open the app on your device
2. Point the camera at your AprilTag
3. The model should appear above the tag!
4. Try:
   - ✅ Rotating the tag → Model rotates with it
   - ✅ Moving the tag → Model follows it
   - ✅ Walking around → See all sides of the model
   - ✅ Tilting the tag → Model tilts too

---

## Troubleshooting

### Model doesn't appear

**Check:**
- Is the Reference Image Library assigned to ARTrackedImageManager?
- Is the Model Prefab assigned to SimpleImageTracking?
- Is the Physical Size in the library correct?
- Is your printed AprilTag clear and well-lit?

**Console Messages:**
- Look for: `[ImageTracking] ✅ Detected: ...`
- If you see this, the image is detected but model might not be spawning

### Model appears in wrong location

**Fix:**
- Adjust the **Model Offset** Y value
  - Positive Y = higher above tag
  - Negative Y = lower (can go below tag)

### Model is upside down or rotated wrong

**Fix:**
- Adjust the **Rotation Offset**
  - Try: `(90, 0, 0)` or `(-90, 0, 0)` or `(0, 180, 0)`
  - Experiment until it looks right

### Model is too big/small

**Fix:**
- Adjust the **Scale** value
  - `0.5` = half size
  - `2.0` = double size

### Image tracking is slow or jittery

**Check:**
- Is the AprilTag image clear and high-contrast? ✅
- Is the Physical Size exactly correct? (measure with ruler!)
- Is the lighting good? (avoid shadows and glare)
- Try reducing **Max Number of Moving Images** to 1

---

## What You've Gained

✅ **Reliable Tracking**: Platform-native AR tracking  
✅ **Automatic Rotation**: Model follows tag perfectly  
✅ **360° Viewing**: Walk around and see all sides  
✅ **Simpler Code**: No complex coordinate transforms  
✅ **Better Performance**: Hardware-accelerated  

---

## Optional: Track Multiple AprilTags

Want to track multiple different tags?

1. Add all your AprilTag images to the **Reference Image Library**
2. Each tag will spawn its own model instance
3. They all track independently!
4. Increase **Max Number of Moving Images** if needed

---

## Need Help?

**Common Questions:**

Q: Can I still use AprilTags?  
A: Yes! You just photograph them and add the photos to the Reference Image Library. AR Foundation will track them.

Q: Do I need to print new tags?  
A: No! Use your existing AprilTags. Just photograph them and add to the library.

Q: What if I have multiple tags with the same appearance?  
A: Make sure each tag has a unique visual pattern. Standard AprilTag families (like tag36h11) have many unique tags.

Q: Can I switch between tags?  
A: Yes! All tags in your Reference Image Library are tracked simultaneously (up to Max Number of Moving Images).

---

**You're all set! Enjoy reliable AR tracking!** 🎉

