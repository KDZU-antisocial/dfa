# Improving AprilTag Detection at Angles

Your AR image tracking is now working! Here are proven techniques to improve detection at difficult angles:

## 🎯 Quick Wins

### 1. **Image Quality**
- ✅ **High Resolution**: Use at least 1024x1024px for your reference images
- ✅ **High Contrast**: Ensure your printed AprilTag has strong black/white contrast
- ✅ **Sharp Edges**: Avoid blur or pixelation in the printed tag

### 2. **Physical Setup**
```
GOOD Angles:          BAD Angles:
  📱 (30-90°)          📱 (<15°)
  ⬇️                   ⬇️↘️
  🏷️                    🏷️

Direct or moderate    Extreme shallow angle
```

### 3. **Environmental Factors**
- **Lighting**: Bright, even lighting (avoid shadows on the tag)
- **Surface**: Flat, non-reflective surface (matte finish best)
- **Distance**: Stay within 20cm - 2m from the tag
- **Movement**: Slow, steady camera movement (not rapid panning)

## ⚙️ Technical Improvements

### Option 1: Use the Enhanced Script (Included)

Add `ImprovedImageTracking.cs` to your ARTrackedImageManager GameObject:

1. In Unity, select the GameObject with `ARTrackedImageManager`
2. Add Component → `ImprovedImageTracking`
3. Configure:
   - **Max Moving Images**: `2` (or `1` for best performance)
   - **Enable Auto Scale**: `true` ✅
   - **Min Detection Quality**: `0.2` (lower = more lenient, optimized setting)

This script will:
- Monitor tracking quality in real-time
- Log warnings for poor viewing angles
- Provide quality metrics for debugging

---

## 📋 **Understanding Key Settings**

### **Max Number of Moving Images**

**What it does:** Controls how many images AR Foundation actively tracks and updates in real-time.

**How it works:**
- Each "moving" slot gets continuous per-frame updates (expensive)
- Images beyond the limit are tracked as "static" (occasional updates)
- ARKit prioritizes by distance, quality, and motion

**Recommended values:**
- `1` - Best performance, single marker tracking
- `2` - **Optimal for most use cases** - tracks 2 AprilTags simultaneously
- `3-4` - Multiple markers, modern devices only
- `8+` - Many markers, high-end devices, expect performance issues

**For your setup (2 AprilTags):**
- Set to `2` - each tag gets its own moving slot
- Don't increase above `2` - wastes resources, reduces per-tag quality
- Don't decrease to `1` - second tag won't track smoothly

**Performance impact:**
```
Setting 1: 100% resources to 1 tag
Setting 2: 50% resources per tag (2 tags)
Setting 4: 25% resources per tag (worse tracking)
```

### **Minimum Detection Quality**

**What it does:** Sets the threshold for acceptable tracking quality (0.0 to 1.0).

**How it affects behavior:**
- **Above threshold** → Model visible, tracking active
- **Below threshold** → Warning logged, may hide model
- **Quality calculation:** Based on viewing angle (perpendicular = 1.0, parallel = 0.0)

**Recommended values:**
- `0.1-0.15` - Very lenient, accepts almost any detection (may be jittery)
- `0.2` - **Optimized setting** - good balance of stability and angle tolerance
- `0.3` - Default, more stable but requires better angles
- `0.4+` - Strict, only near-perpendicular views

**Quality to angle mapping:**
| Quality | Approximate Angle | Tracking |
|---------|-------------------|----------|
| 1.0-0.7 | 60-90° | ✅ Excellent |
| 0.7-0.4 | 30-60° | ⚠️ Good |
| 0.4-0.2 | 15-30° | ⚠️ Acceptable (may lose tracking) |
| <0.2    | <15°  | ❌ Poor (likely to fail) |

**Current optimized setting: `0.2`**
- More lenient for difficult angles
- Accepts tracking down to ~15-30° angles
- May have slight jitter at extreme angles (trade-off for detection range)

### Option 2: Improve Reference Images

**Current**: `0-AprilTag-4x4-70mm.png` (probably 512x512 or less)

**Better**: Create high-resolution versions
```bash
# If you have the original AprilTag, scan it at high resolution
# OR generate new ones at 2048x2048px
```

**In Unity:**
1. Select your AprilTag image in Project window
2. Inspector → Texture Import Settings:
   - Max Size: `2048`
   - Compression: `None` or `High Quality`
   - Filter Mode: `Bilinear` or `Trilinear`
3. Apply and rebuild the Reference Image Library

### Option 3: Increase Max Number of Moving Images

In your scene (`XR Origin (AR Rig)` GameObject):
- Select `ARTrackedImageManager` component
- Set `Max Number Of Moving Images` to `3-4` (may impact performance)

This allows AR Foundation to track more potential matches simultaneously.

## 📊 Understanding Detection Quality

The `ImprovedImageTracking` script estimates quality based on viewing angle:

| Quality | Angle Range | Detection   | Action Needed            |
|---------|-------------|-------------|--------------------------|
| 1.0-0.7 | 60-90°      | ✅ Excellent | None                     |
| 0.7-0.4 | 30-60°      | ⚠️ Good     | Works, but track quality |
| 0.4-0.2 | 15-30°      | ⚠️ Poor     | Adjust angle if possible |
| <0.2    | <15°        | ❌ Very Poor | Likely to lose tracking  |

## 🚀 Performance Tips

**If tracking is slow or stuttering:**

1. **Reduce Max Moving Images** to `1` (tracks only the closest/best match)
2. **Lower Reference Image Resolution** to `1024x1024`
3. **Disable Auto Scale Estimation** (faster, but worse angle detection)
4. **Ensure good lighting** (poor lighting = slow processing)

## 🔬 Advanced: Multiple Reference Angles (Overkill)

For extreme angles, you can add multiple versions of the same AprilTag at different perspectives:

1. Take photos of your AprilTag from different angles
2. Add all versions to Reference Image Library with same name
3. AR Foundation will match any version

**⚠️ Usually not needed** - the default system handles moderate angles well.

## 🎯 Real-World Testing Tips

**Best practices for reliable detection:**

1. **Start Perpendicular**
   - Hold phone directly above/in front of tag
   - Wait for initial detection (model appears)
   - Then slowly move to test angles

2. **Good Lighting is Critical**
   - Overhead lighting or natural daylight
   - Avoid backlighting (window behind tag)
   - No shadows on the tag surface

3. **Physical Tag Quality**
   - Print on thick, flat cardstock
   - Use high-quality printer (laser preferred)
   - Laminate if possible (prevents wear, adds rigidity)

4. **Camera Movement**
   - Slow, smooth movements
   - Avoid rapid panning or rotation
   - Give the system time to adjust (0.5-1 second)

## 📱 Device-Specific Considerations

**ARKit (iOS):**
- Generally excellent angle detection
- Better in good lighting
- LiDAR devices (iPhone 12 Pro+) have slight advantage

**ARCore (Android):**
- Performance varies by device
- Flagship devices perform best
- May need higher quality reference images

## 🐛 Troubleshooting

**"Image detected briefly then lost at angles"**
→ Lower `m_MinimumDetectionQuality` in `ImprovedImageTracking`

**"No detection at any angle"**
→ Check reference image matches physical tag exactly

**"Jittery/unstable tracking at angles"**
→ Improve lighting, ensure flat surface, slow camera movement

**"Works great straight-on, terrible at angles"**
→ Enable automatic scale estimation, increase max moving images

---

## Summary: Best Configuration for Angle Detection

### **Optimized Settings:**

```csharp
// In ImprovedImageTracking.cs:
m_MaxNumberOfMovingImages = 2;                      // Tracks 2 AprilTags simultaneously
m_EnableAutomaticImageScaleEstimation = true;      // ✅ Better angle detection
m_MinimumDetectionQuality = 0.2f;                  // ✅ Optimized - accepts 15-30° angles
```

### **Why These Settings:**

**Max Moving Images = 2:**
- ✅ Matches your use case (2 AprilTags)
- ✅ Each tag gets 50% of processing power
- ✅ Smooth simultaneous tracking
- ✅ No wasted resources
- ❌ Don't increase (reduces per-tag quality)
- ❌ Don't decrease (can't track 2 tags smoothly)

**Min Detection Quality = 0.2:**
- ✅ Optimized for difficult angles (down to ~15-30°)
- ✅ More lenient than default (0.3)
- ✅ Still maintains reasonable stability
- ⚠️ May have slight jitter at extreme angles (acceptable trade-off)
- 💡 Can adjust between 0.15-0.3 based on your needs

**Physical setup:**
- High-contrast printed tag
- Matte surface, good lighting
- With optimized settings: 15-90° viewing angles work

**You're all set!** 🎉

---

## 🔧 **Performance Optimization**

The tracking scripts have been optimized to reduce overhead:
- Debug logging runs every 2 seconds (not every frame)
- Quality checks run every 5-10 frames (not every frame)
- Frame processing is more efficient
- Less resource contention = better angle detection

**Frame processing optimization benefits:**
- Reduced ARFrame retention warnings
- More CPU cycles for tracking
- Better performance at difficult angles
- Smoother overall experience

